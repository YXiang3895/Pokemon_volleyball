Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Timers
Imports Microsoft.VisualBasic.Logging

Public Class Form1
    Public Property my_server As Server = New Server(Me)

    Private Sub start_server_Click(sender As Object, e As EventArgs) Handles start_server.Click
        my_server.StartServer()
    End Sub

    Private Sub restart_server_Click(sender As Object, e As EventArgs) Handles restart_server.Click
        Me.Close()
        Application.Restart()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources._123
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If (my_server IsNot Nothing) Then
            my_server.StopServer()
        End If
    End Sub

End Class

Public Class Server
    Public Shared player1 As Player
    Public Shared player2 As Player
    Private Shared player1Character As Integer = -1
    Private Shared player2Character As Integer = -1

    Public Shared ball As Ball
    Public Shared stick As Stick
    Public Shared gameLogic As GameLogic
    Public Shared playerDic As Dictionary(Of String, Player)
    Public Shared Property floorLevel As Integer = 669
    Public Shared Property playerClientWidth As Integer = 1184
    '1184
    Public Shared Property playerClientHeight As Integer = 761
    '761
    Public Shared gameStart = 0

    Private listener As TcpListener
    Public Shared clients As New List(Of TcpClient)
    Public Shared BallTimer As System.Timers.Timer = New System.Timers.Timer()
    Public Shared PlayerTimer As System.Timers.Timer = New System.Timers.Timer()
    Public Shared SyncTimer As System.Timers.Timer = New System.Timers.Timer()
    Private mainForm As Form1

    Public Sub New(frm As Form1)
        mainForm = frm
    End Sub
    Private Sub InitialPlayer()
        player1 = New Player(1, player1Character) With {
            .position_x = 800, '改過
            .position_y = 600'改過
        }
        player2 = New Player(2, player2Character) With {
            .position_x = 100,'改過
            .position_y = 600'改過
        }

        playerDic = New Dictionary(Of String, Player) From {
            {"player1", player1},
            {"player2", player2}
        }
        PlayerTimer.Interval = 30
        AddHandler PlayerTimer.Elapsed, AddressOf PlayerTimerTick
    End Sub
    Private Sub InitialBall()
        ball = New Ball With {
            .position_x = 150,
            .position_y = 200
        }
        BallTimer.Interval = 30
        AddHandler BallTimer.Elapsed, AddressOf BallTimerTick
    End Sub

    Private Sub InitialField()
        gameLogic = New GameLogic()
        stick = New Stick()
    End Sub
    Public Shared Sub ResetGameObject()
        If (player1 IsNot Nothing) Then
            player1.Dispose()
        End If
        If (player2 IsNot Nothing) Then
            player2.Dispose()
        End If
        If (ball IsNot Nothing) Then
            ball.Dispose()
        End If
        If (gameLogic IsNot Nothing) Then
            gameLogic.Dispose()
        End If
        If (BallTimer IsNot Nothing) Then
            BallTimer.Stop()
            BallTimer.Dispose()
        End If
        If (PlayerTimer IsNot Nothing) Then
            PlayerTimer.Stop()
            PlayerTimer.Dispose()
        End If
        If (SyncTimer IsNot Nothing) Then
            SyncTimer.Stop()
            SyncTimer.Dispose()
        End If
        gameStart = 0
        player1Character = -1
        player2Character = -1
    End Sub


    Public Sub StartServer()
        InitialServer()
    End Sub
    Private Sub InitialGame()
        InitialPlayer()
        InitialBall()
        InitialField()
        InitialTimer()
    End Sub
    Private Sub InitialTimer()
        SyncTimer.Interval = 2000
        AddHandler SyncTimer.Elapsed, AddressOf SyncTimerTick
        SyncTimer.Start()

        PlayerTimer.Interval = 20
        AddHandler PlayerTimer.Elapsed, AddressOf PlayerTimerTick
        PlayerTimer.Start()

        BallTimer.Interval = 20
        AddHandler BallTimer.Elapsed, AddressOf BallTimerTick
        BallTimer.Start()

    End Sub
    Private Sub InitialServer()
        Dim my_port As Integer
        If Integer.TryParse(Form1.port_input.Text, my_port) Then
            Try
                listener = New TcpListener(IPAddress.Any, my_port)
                listener.Start()
                Task.Run(Sub() AcceptClients())
                Form1.restart_server.Enabled = True
                Form1.port_input.Enabled = False
                Form1.start_server.Enabled = False
                Server_logging("伺服器啟動!")
            Catch ex As Exception
                Server_logging("無法開啟伺服器!")
            End Try
        Else
            MsgBox("請嘗試有效的PORT!")
        End If
    End Sub
    Private Sub AcceptClients()
        While True
            Try
                Dim client = listener.AcceptTcpClient()
                clients.Add(client)
                Server_logging("玩家連入: " & client.Client.RemoteEndPoint.ToString)
                Task.Run(Sub() HandleClient(client))
            Catch ex As Exception
                Server_logging("關閉listener")
                Exit While
            End Try
        End While
    End Sub

    Private Sub HandleClient(client As TcpClient)
        Try
            Using client
                Using stream As NetworkStream = client.GetStream()
                    Dim data(1024) As Byte
                    While True
                        Dim bytesRead = stream.Read(data, 0, data.Length)
                        If bytesRead = 0 Then
                            Exit While
                        End If
                        ' Dim clientData = Encoding.ASCII.GetString(data, 0, bytesRead).TrimEnd(Chr(0))
                        Dim clientData = Encoding.UTF8.GetString(data, 0, bytesRead).TrimEnd(Chr(0))
                        Server_logging("接收到: " & clientData)
                        HandleCommnd(clientData)
                    End While
                End Using
            End Using
        Catch ex As IOException
            RemoveClient(client)
        End Try
    End Sub

    Private Sub HandleCommnd(data As String)
        Dim parts = data.Split(","c)
        Console.WriteLine(parts(1))
        If parts.Length = 3 And gameStart = 0 Then
            Dim player = parts(0)
            Dim character = parts(1)
            SettingPlayer(player, character)
        End If
        If parts.Length = 3 And gameStart = 1 Then
            Dim player = parts(0)
            Dim command = parts(1)
            If command = "startFalling" And player = "player1" Then
                ball.isFalling = True
            End If
            Dim whichPlayer = playerDic(player)
            Select Case command
                Case "moveLeft"
                    whichPlayer.moveLeft = True
                Case "moveRight"
                    whichPlayer.moveRight = True
                Case "stopMoveLeft"
                    whichPlayer.moveLeft = False
                    whichPlayer.speed_x = 0
                Case "stopMoveRight"
                    whichPlayer.moveRight = False
                    whichPlayer.speed_x = 0
                Case "jump"
                    If (whichPlayer.isJumping = False And whichPlayer.jumpDisabled = False) Then
                        whichPlayer.isJumping = True
                        whichPlayer.speed_y = -whichPlayer.jumpPower
                    End If
                Case "changeHitState"
                    whichPlayer.hitState = If(whichPlayer.hitState = 0, 1, 0)
                    Dim response As String = If(whichPlayer.hitState = 0, "|player" & whichPlayer.id & ",hitUp|", "|player" & whichPlayer.id & ",hitDown|")
                    BroadcastToAllClients(response)
                Case "hit"
                    ball.PlayerHitBall(whichPlayer)
                Case "useSkill"
                    whichPlayer.UseSkill()
            End Select
        End If
    End Sub
    Private Sub SettingPlayer(player As String, character As String)
        Select Case player
            Case "player1"
                If Not Integer.TryParse(character, player1Character) Then
                    Exit Sub
                End If
            Case "player2"
                If Not Integer.TryParse(character, player2Character) Then
                    Exit Sub
                End If
        End Select
        If (CheckAllPlayersReady() = True) Then
            Server_logging("兩位玩家已就緒!")
            InitialGame()
            Dim message As String = player1Character & "," & player2Character
            BroadcastToAllClients(message)
            gameStart = 1
        End If
    End Sub

    Private Function CheckAllPlayersReady()
        If (player1Character <> -1 And player2Character <> -1) Then
            Return True
        End If
        Return False
    End Function
    Public Shared Sub BroadcastToAllClients(message As String)
        Dim responseData = Encoding.UTF8.GetBytes(message)
        Parallel.ForEach(clients, Sub(client)
                                      If client.Connected Then
                                          Dim stream = client.GetStream()
                                          stream.Write(responseData, 0, responseData.Length)
                                      End If
                                  End Sub)
    End Sub
    Private Shared Sub RemoveClient(client As TcpClient)
        If clients.Contains(client) Then
            clients.Remove(client)
            client.Close()
        End If
    End Sub
    Private Sub BallTimerTick(sender As Object, e As EventArgs)
        If (ball.isFalling = False) Then
            Return
        End If
        Dim players As List(Of Player) = GetPlayers()
        ball.BallUpdate(player1, player2)
        Dim response = GetObjPosition()
        BroadcastToAllClients(response)
    End Sub

    Private Sub PlayerTimerTick(sender As Object, e As EventArgs)
        player1.PlayerUpdate()
        player2.PlayerUpdate()
        Dim response = GetObjPosition()
        BroadcastToAllClients(response)
    End Sub
    Private Sub SyncTimerTick(sender As Object, e As EventArgs)
        Dim response As String = "|player1Score," & GameLogic.player1Score & "|" & "|player2Score," & GameLogic.player2Score & "|"
        BroadcastToAllClients(response)
    End Sub

    Private Function GetPlayers() As List(Of Player)
        Return New List(Of Player) From {player1, player2}
    End Function
    Public Shared Function GetObjPosition()
        Dim response = String.Join("|", playerDic.Select(Function(kvp) $"{kvp.Key},{kvp.Value.position_x},{kvp.Value.position_y}"))
        response = response & "|ball," & ball.position_x & "," & ball.position_y & "|"
        Return response
    End Function
    Public Sub Server_logging(message As String)
        If mainForm.log_text.InvokeRequired Then
            mainForm.log_text.Invoke(Sub() mainForm.log_text.AppendText(message & vbCrLf))
        Else
            mainForm.log_text.AppendText(message & vbCrLf)
        End If
    End Sub
    Public Sub StopServer()
        If (BallTimer IsNot Nothing) Then
            BallTimer.Stop()
            BallTimer.Dispose()
        End If
        If (PlayerTimer IsNot Nothing) Then
            PlayerTimer.Stop()
            PlayerTimer.Dispose()
        End If
        If (SyncTimer IsNot Nothing) Then
            SyncTimer.Stop()
            SyncTimer.Dispose()
        End If
        For Each client As TcpClient In clients
            If client IsNot Nothing AndAlso client.Connected Then
                client.Close()
            End If
        Next
        If (listener IsNot Nothing) Then
            listener.Stop()
            listener = Nothing
        End If
    End Sub
End Class
Public Class GameObj
    Inherits PictureBox
    Public Property position_x As Integer
    Public Property position_y As Integer
    Public Property speed_x As Double
    Public Property speed_y As Double
    Public Property accleration_x As Double
    Public Property accleration_y As Double

End Class

Public Class Player
    Inherits GameObj
    Public Property id As Integer
    Public Property character As Integer
    Public Property enemyPlayer As Player
    Public Property moveLeft As Boolean
    Public Property moveRight As Boolean
    Public Property jumpPower As Integer = 60
    Public Property isJumping As Boolean = False
    Public Property hitState As Integer = 0
    Public Property skillReady As Boolean = True
    Public Property jumpDisabled As Boolean = False

    Public Property moveReverse As Boolean = False
    Public Property moveDisabled As Boolean = False
    Public Property heavySmash As Boolean = False
    Private skillResetTimer As System.Timers.Timer
    Private skillDurationTimer As System.Timers.Timer


    Public Sub New(id As Integer, character As Integer)
        Me.character = character
        Me.id = id
        Me.Size = New Size(110, 120)
        Me.speed_x = 0
        Me.speed_y = 0
        Me.accleration_x = 0
        Me.accleration_y = 5
    End Sub

    Public Sub PlayerUpdate()
        If (moveDisabled = True) Then
            Return
        End If
        speed_x += accleration_x * Server.PlayerTimer.Interval / 100
        speed_y += accleration_y * Server.PlayerTimer.Interval / 100
        Me.position_x += speed_x * Server.PlayerTimer.Interval / 100 + 0.5 * accleration_x * (Server.PlayerTimer.Interval / 100) ^ 2
        Me.position_y += speed_y * Server.PlayerTimer.Interval / 100 + 0.5 * accleration_y * (Server.PlayerTimer.Interval / 100) ^ 2
        HandlePlayerMove()
    End Sub

    Private Sub HandlePlayerMove()
        Dim actualMoveLeft As Boolean = If(moveReverse, moveRight, moveLeft)
        Dim actualMoveRight As Boolean = If(moveReverse, moveLeft, moveRight)

        Dim rightBound = If(id = 2, Server.stick.Left, Server.playerClientWidth) '改過
        Dim leftBound = If(id = 2, 0, Server.stick.Right) '改過

        If (isJumping) Then
            position_y += speed_y
            speed_y += accleration_y
        End If
        If position_y < 0 Then
            position_y = 0
            speed_y = 0
        ElseIf position_y + Me.Height > Server.floorLevel Then
            position_y = Server.floorLevel - Me.Height
            speed_y = 0
            isJumping = False
        End If


        If actualMoveLeft Then
            If (position_x > leftBound) Then
                speed_x = -20
            Else
                speed_x = 0
                position_x = leftBound
            End If
        ElseIf actualMoveRight Then
            If (position_x + Me.Width < rightBound) Then
                speed_x = 20
            Else
                speed_x = 0
                position_x = rightBound - Me.Width
            End If
        End If

        ChangePlayerFace()

    End Sub

    Private Sub ChangePlayerFace()
        Dim response As String = ""

        Dim actualMovingLeft As Boolean = moveLeft
        Dim actualMovingRight As Boolean = moveRight

        If moveReverse Then
            actualMovingLeft = moveRight
            actualMovingRight = moveLeft
        End If

        If actualMovingLeft Then
            response = "|player" & id & ",faceLeft|"
        ElseIf actualMovingRight Then
            response = "|player" & id & ",faceRight|"
        End If

        Server.BroadcastToAllClients(response)
    End Sub


    Public Sub UseSkill()
        enemyPlayer = If(id = 1, Server.playerDic("player2"), Server.playerDic("player1"))
        If (skillReady = False) Then
            Return
        End If

        Dim response As String = "|player" & id & ",useSkill|"
        Server.BroadcastToAllClients(response)

        skillReady = False

        If skillResetTimer IsNot Nothing Then
            skillResetTimer.Stop()
            skillResetTimer.Dispose()
        End If
        skillResetTimer = New System.Timers.Timer()

        If skillDurationTimer IsNot Nothing Then
            skillDurationTimer.Stop()
            skillDurationTimer.Dispose()
        End If
        skillDurationTimer = New System.Timers.Timer()

        Select Case character
            Case 0
                Server.ball.causeNumb = True
                skillResetTimer.Interval = 5000
            Case 1
                response = "|player" & enemyPlayer.id & ",isReverse|"
                Server.BroadcastToAllClients(response)
                enemyPlayer.moveReverse = True
                skillResetTimer.Interval = 5000
                skillDurationTimer.Interval = 3000
            Case 2
                heavySmash = True
                skillResetTimer.Interval = 5000
            Case 3
                skillResetTimer.Interval = 5000
                response = "|player" & enemyPlayer.id & ",cantJump|"
                Server.BroadcastToAllClients(response)
                enemyPlayer.jumpDisabled = True
                skillResetTimer.Interval = 5000
                skillDurationTimer.Interval = 3000
        End Select
        AddHandler skillResetTimer.Elapsed, AddressOf SkillReset
        AddHandler skillDurationTimer.Elapsed, AddressOf SkillDuration
        skillResetTimer.AutoReset = False
        skillResetTimer.Start()
        skillDurationTimer.AutoReset = False
        skillDurationTimer.Start()
    End Sub

    Private Sub SkillReset(sender As Object, e As ElapsedEventArgs)
        Dim response As String = "|player" & id & ",resetSkill|"
        Server.BroadcastToAllClients(response)
        skillReady = True
        CType(sender, System.Timers.Timer).Dispose()
    End Sub
    Private Sub SkillDuration(sender As Object, e As ElapsedEventArgs)
        Dim response As String
        Select Case character
            Case 1
                response = "|player" & enemyPlayer.id & ",notReverse|"
                Server.BroadcastToAllClients(response)
                enemyPlayer.moveReverse = False
            Case 3
                response = "|player" & enemyPlayer.id & ",canJump|"
                Server.BroadcastToAllClients(response)
                enemyPlayer.jumpDisabled = False
        End Select
        CType(sender, System.Timers.Timer).Dispose()
    End Sub
    Public Sub ResetPlayerState()
        moveDisabled = False
        jumpDisabled = False
        moveReverse = False
        heavySmash = False
    End Sub

End Class

Public Class Ball
    Inherits GameObj
    Public Property radius As Integer = 50
    Public Property isFalling As Boolean = False
    Public Property causeNumb As Boolean = False
    Private Property whoNumb As Player

    Public Sub New()
        Me.speed_x = 2
        Me.speed_y = 5
        Me.accleration_x = 0
        Me.accleration_y = 4
    End Sub

    Public Sub ResetBallState()
        Me.speed_x = 2
        Me.speed_y = 5
        Me.accleration_x = 0
        Me.accleration_y = 4
        Me.causeNumb = False
    End Sub
    Public Sub BallUpdate(player1 As Player, Optional player2 As Player = Nothing)

        speed_x += accleration_x * Server.BallTimer.Interval / 100
        speed_y += accleration_y * Server.BallTimer.Interval / 100

        position_x += speed_x * Server.BallTimer.Interval / 100 + 0.5 * accleration_x * (Server.BallTimer.Interval / 100) ^ 2
        position_y += speed_y * Server.BallTimer.Interval / 100 + 0.5 * accleration_y * (Server.BallTimer.Interval / 100) ^ 2
        CheckPlayerCollision(player1)
        CheckPlayerCollision(player2)
        CheckStickCollision()
        CheckBoundCollision()
    End Sub

    Public Sub CheckPlayerCollision(player As Player)

        Dim ballRect As New Rectangle(Me.position_x, Me.position_y, 2 * Me.radius, 2 * Me.radius)

        Dim playerRect As New Rectangle(player.position_x, player.position_y, player.Width, player.Height)

        If ballRect.IntersectsWith(playerRect) Then

            Dim ballCenterX As Single = Me.position_x + radius

            Dim playerCenterX As Single = player.position_x + player.Width / 2

            If Me.position_y + 2 * Me.radius > player.position_y And
           (player.position_x <= Me.position_x And Me.position_x <= player.position_x + player.Width Or
            player.position_x <= Me.position_x + 2 * Me.radius And Me.position_x + 2 * Me.radius <= player.position_x + player.Width) Then

                Me.position_x += If(ballCenterX > playerCenterX, 2, -2)

                Me.speed_x = If(ballCenterX > playerCenterX, Math.Abs(Me.speed_x), -Math.Abs(Me.speed_x))
                Me.speed_y = -Math.Abs(Me.speed_y)
                If Me.speed_y < -55 Then
                    Me.speed_y = -55
                End If

                Me.position_y = player.position_y - Me.radius * 2
            End If

            'Pikachu Skill
            If causeNumb = True Then
                Dim response As String = "|player" & player.id & ",isNumb|"
                Server.BroadcastToAllClients(response)
                causeNumb = False
                player.moveDisabled = True
                Dim resetNumbTimer As New System.Timers.Timer()
                resetNumbTimer.Interval = 1500
                whoNumb = player
                AddHandler resetNumbTimer.Elapsed, AddressOf resetNumbTimerTick
                resetNumbTimer.AutoReset = False
                resetNumbTimer.Start()
            End If


        End If


    End Sub

    Private Sub resetNumbTimerTick(sender As Object, e As ElapsedEventArgs)
        Dim response As String = "|player" & whoNumb.id & ",notNumb|"
        Server.BroadcastToAllClients(response)
        whoNumb.moveDisabled = False
        DirectCast(sender, System.Timers.Timer).Dispose()
    End Sub
    Private Sub CheckBoundCollision()
        If position_y > Server.floorLevel Then
            position_y = Server.floorLevel
            If (Server.gameLogic.gamePaused = False) Then
                Dim whoMissed = If(position_x < Server.playerClientWidth / 2, 2, 1) '改過
                Server.gameLogic.HandleScore(whoMissed)
            End If
        End If

        If position_x < 0 Or position_x + radius > Server.playerClientWidth Then
            speed_x = -speed_x
        End If
        If position_y < 0 Or position_y + radius > Server.playerClientHeight Then
            speed_y = -speed_y
        End If
    End Sub

    Public Sub PlayerHitBall(player As Player)
        Dim ballCenterX As Integer = Me.position_x + radius
        Dim ballCenterY As Integer = Me.position_y + radius
        Dim hitdistance As Integer = 80
        If (((player.position_x - hitdistance <= ballCenterX) And (ballCenterX <= player.position_x + player.Width + hitdistance)) And (player.position_y - hitdistance <= ballCenterY) And (ballCenterY <= player.position_y + Height + hitdistance)) Then
            Select Case player.hitState
                Case 0
                    speed_x = 0
                    speed_y = -60
                Case 1
                    If (player.heavySmash = True) Then
                        player.heavySmash = False
                        speed_x = If(player.id = 2, 200, -200) '改過
                        speed_y = 160
                    Else
                        speed_x = If(player.id = 2, 120, -120) '改過
                        speed_y = 90
                    End If
            End Select
        End If
    End Sub
    Public Sub CheckStickCollision()
        Dim stickRect = Server.stick.Bounds
        Dim ballRect As New Rectangle(Me.position_x, Me.position_y, 2 * Me.radius, 2 * Me.radius)
        If ballRect.IntersectsWith(stickRect) Then
            Dim ballCenterX As Integer = ballRect.Left + radius
            Dim ballCenterY As Integer = ballRect.Top + radius
            Dim stickCenterX As Integer = stickRect.Left + Server.stick.Width / 2
            speed_x = If(ballCenterX < stickCenterX, -Math.Abs(speed_x), Math.Abs(speed_x))
            speed_y = If(ballCenterY < Server.stick.Top, -Math.Abs(speed_y), speed_y)
        End If

    End Sub

End Class

Public Class Stick
    Inherits PictureBox
    Public Sub New()
        Me.Location = New Point(Server.playerClientWidth / 2, Server.playerClientHeight - 300)
        Me.Size = New Size(24, 394)
    End Sub
End Class

Public Class GameLogic
    Inherits Label
    Public Shared Property player1Score As Integer
    Public Shared Property player2Score As Integer
    Public Property gamePaused As Boolean
    Private Property response As String
    Private Property whoMissed As Integer

    Public Property resetTimer As System.Timers.Timer
    Public Property syncTimer As System.Timers.Timer
    Public Sub New()
        player1Score = 0
        player2Score = 0
        gamePaused = False
    End Sub

    Public Sub HandleScore(playerID As Integer) 'playerID代表誰沒有接到球
        whoMissed = playerID
        GamePause()
        UpdateScore(playerID)
        CheckWinner()
        ResetBall()
        Server.playerDic("player1").ResetPlayerState()
        Server.playerDic("player2").ResetPlayerState()
    End Sub
    Private Sub UpdateScore(playerID As Integer)
        If (playerID = 1) Then
            player2Score += 1
            response = "|player2,Goal|"
        ElseIf (playerID = 2) Then
            player1Score += 1
            response = "|player1,Goal|"
        End If
        Server.BroadcastToAllClients(response)
    End Sub
    Private Sub GamePause()
        gamePaused = True
        Server.BallTimer.Stop()
    End Sub
    Private Sub CheckWinner()
        If (player1Score = 2) Then
            response = "|player1,Win|"
            Server.BroadcastToAllClients(response)
            Server.ResetGameObject()
            Server.BallTimer.Stop()
            resetTimer.Stop()
        ElseIf (player2Score = 2) Then
            response = "|player2,Win|"
            Server.BroadcastToAllClients(response)
            Server.ResetGameObject()
            Server.BallTimer.Stop()
            resetTimer.Stop()
        End If
    End Sub

    Private Sub ResetBall()
        If resetTimer IsNot Nothing Then
            resetTimer.Stop()
            resetTimer.Dispose()
        End If

        resetTimer = New System.Timers.Timer With {
            .Interval = 1500,
            .AutoReset = False
        }
        AddHandler resetTimer.Elapsed, AddressOf ResetTimerTick

        resetTimer.Start()

    End Sub
    Private Sub ResetTimerTick(sender As Object, e As EventArgs)
        Dim newX = If(whoMissed = 2, 60, 827)
        Dim newY = 50
        Server.ball.ResetBallState()
        Server.ball.position_x = newX
        Server.ball.position_y = newY


        response = Server.GetObjPosition()
        response &= "|gameState,resetBall|"

        Server.BroadcastToAllClients(response)
        Try
            If (Server.BallTimer IsNot Nothing) Then
                Server.BallTimer.Start()
            End If
        Catch ex As ObjectDisposedException
            Console.WriteLine(ex.Message)
        End Try


        gamePaused = False

        CType(sender, System.Timers.Timer).Dispose()
    End Sub


End Class