Imports System.Media
Imports System.Net.Sockets
Imports System.Runtime.Remoting
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

Public Class OnlineGame
    Private client As TcpClient
    Private stream As NetworkStream
    Public Player1 As PictureBox
    Public Player2 As PictureBox
    Public Player1Character As Integer
    Public Player2Character As Integer
    Public activePlayerId As String
    Public Stick As PictureBox
    Public Ball As PictureBox

    Public Shared Property Instance As OnlineGame

    Public Sub New(who As String, p1Character As Integer, p2Character As Integer)
        InitializeComponent()
        activePlayerId = who
        Player1Character = p1Character
        Player2Character = p2Character
        stream = Form1.stream
        Instance = Me
    End Sub

    Private Sub OnlineGameForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources._123
        InitialField()
        InitialPlayer()
    End Sub
    Private Sub OnlineGameForm_Closing() Handles MyBase.Closing
        If (client IsNot Nothing) Then
            client.Dispose()
        End If
        If (stream IsNot Nothing) Then
            stream.Dispose()
        End If
        If (Player1 IsNot Nothing) Then
            Player1.Dispose()
        End If
        If (Player2 IsNot Nothing) Then
            Player2.Dispose()
        End If
        Form1.OnlineGameStart = 0
    End Sub
    Private Sub InitialField()
        With Me
            .StartPosition = FormStartPosition.CenterScreen
            .Size = New Size(1260, 740)
            .KeyPreview = True
            .DoubleBuffered = True
            .BackgroundImage = My.Resources.game_background
            .BackgroundImageLayout = ImageLayout.Stretch
        End With

        Stick = New PictureBox With {
            .Size = New Size(24, 394),
            .Image = My.Resources.stick,
            .Location = New Point((Me.ClientSize.Width - 75) / 2, Me.ClientSize.Height - 300)
        }
        Me.Controls.Add(Stick)

        Ball = New PictureBox With {
            .Size = New Size(100, 100),
            .SizeMode = PictureBoxSizeMode.StretchImage,
            .BackColor = Color.Transparent,
            .Image = My.Resources.ball,
            .Location = New Point(50, 100)
        }
        Me.Controls.Add(Ball)
        Ball.BringToFront()

        AddHandler Me.KeyDown, AddressOf Form1_KeyDown
        AddHandler Me.KeyUp, AddressOf Form1_KeyUp
    End Sub

    Private Sub InitialPlayer()
        Player1Character = Player1Character
        Player2Character = Player2Character

        Player1 = New PictureBox With {
                .Size = New Size(110, 120),
                .Location = New Point(827, 669 - .Height),
                .SizeMode = PictureBoxSizeMode.StretchImage,
                .BackColor = Color.Transparent
        }

        Player2 = New PictureBox With {
            .Size = New Size(110, 120),
            .Location = New Point(60, 669 - .Height),
            .SizeMode = PictureBoxSizeMode.StretchImage,
            .BackColor = Color.Transparent
        }

        SetPlayerImage()


        Me.Controls.Add(Player1)
        Me.Controls.Add(Player2)


    End Sub
    Private Sub SetPlayerImage()

        player2HitImage.Image = My.Resources.game_arrow_down
        player2HitImage.SizeMode = PictureBoxSizeMode.StretchImage
        player1HitImage.Image = My.Resources.game_arrow_down
        player1HitImage.SizeMode = PictureBoxSizeMode.StretchImage
        player2SkillImage.BackColor = Color.Green
        player1SkillImage.BackColor = Color.Green


        SetPlayerSkillImages(Player1, Player1Character, player1SkillImage)
        SetPlayerSkillImages(Player2, Player2Character, player2SkillImage)
    End Sub

    Private Sub SetPlayerSkillImages(playerPictureBox As PictureBox, character As Integer, skillPictureBox As PictureBox)
        Select Case character
            Case 0
                playerPictureBox.Image = If(playerPictureBox Is Player2, My.Resources.game_pikachu_right, My.Resources.game_pikachu_left)
                skillPictureBox.Image = My.Resources.game_thunder
            Case 1
                playerPictureBox.Image = If(playerPictureBox Is Player2, My.Resources.game_charmander_right, My.Resources.game_charmander_left)
                skillPictureBox.Image = My.Resources.game_fire_icon_png
            Case 2
                playerPictureBox.Image = If(playerPictureBox Is Player2, My.Resources.game_squirtle_right, My.Resources.game_squirtle_left)
                skillPictureBox.Image = My.Resources.game_strength
            Case 3
                playerPictureBox.Image = If(playerPictureBox Is Player2, My.Resources.game_bulbasaur_right, My.Resources.game_bulbasaur_left)
                skillPictureBox.Image = My.Resources.game_vine_icon
        End Select
    End Sub



    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.A Then
            SendCommand(activePlayerId, "moveLeft")
        ElseIf e.KeyCode = Keys.D Then
            SendCommand(activePlayerId, "moveRight")
        ElseIf e.KeyCode = Keys.W Then
            SendCommand(activePlayerId, "jump")
        ElseIf e.KeyCode = Keys.Escape Then
            Me.Hide()
            Form1.Show()
            Form1.TabControl1.SelectedIndex = 9
        End If
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Z Then
            SendCommand(activePlayerId, "startFalling")
        ElseIf e.KeyCode = Keys.A Then
            SendCommand(activePlayerId, "stopMoveLeft")
        ElseIf e.KeyCode = Keys.D Then
            SendCommand(activePlayerId, "stopMoveRight")
        ElseIf e.KeyCode = Keys.J Then
            SendCommand(activePlayerId, "changeHitState")
        ElseIf e.KeyCode = Keys.K Then
            SendCommand(activePlayerId, "hit")
        ElseIf e.KeyCode = Keys.U Then
            SendCommand(activePlayerId, "useSkill")
        End If
    End Sub

    Private Async Sub SendCommand(playerId As String, command As String)
        Dim playerCommand = $"{playerId},{command},"
        'Dim commandBytes = Encoding.ASCII.GetBytes(playerCommand)
        Dim commandBytes = Encoding.UTF8.GetBytes(playerCommand)
        Await stream.WriteAsync(commandBytes, 0, commandBytes.Length)
    End Sub

    Public Sub HandleGameData(serverData As String)
        Dim serverDataSplit = serverData.Split("|"c)
        For Each data As String In serverDataSplit
            'Console.WriteLine(data)
            If String.IsNullOrWhiteSpace(data) Then
                Continue For
            End If
            Dim parts = data.Split(","c)
            Select Case parts.Length
                Case 3
                    UpdateObjectPosition(parts)
                Case 2
                    UpdateGameState(parts)
            End Select
        Next
    End Sub

    Private Sub UpdateObjectPosition(parts As String())
        Dim obj = parts(0)
        Dim x As Integer
        Dim y As Integer
        If Not Integer.TryParse(parts(1), x) Or Not Integer.TryParse(parts(2), y) Then
            Exit Sub
        End If
        Select Case obj
            Case "player1"
                UpdateObjPosition(Player1, x, y)
            Case "player2"
                UpdateObjPosition(Player2, x, y)
            Case "ball"
                UpdateObjPosition(Ball, x, y)
        End Select
    End Sub

    Private Sub UpdateGameState(parts As String())
        Dim who = parts(0)
        Dim state = parts(1)
        If String.IsNullOrWhiteSpace(state) Then
            Exit Sub
        End If
        Select Case who
            Case "player1", "player2"
                UpdatePlayerState(who, state)
            Case "gameState"
                UpdateGameStatus(state)
            Case "player1Score,player2Score"
                UpdateSyncScore(who, state)
        End Select
    End Sub


    Private Sub UpdateSyncScore(who As String, state As String)
        If who = "player1Score" Then
            player1ScoreText.Text = state
        End If
        If who = "player2Score" Then
            player2ScoreText.Text = state
        End If
    End Sub
    Private Sub UpdatePlayerState(who As String, state As String)
        Select Case state
            Case "Goal"
                UpdateScore(who)
            Case "Win"
                Me.Close()
                Me.Dispose()
                Form1.tabpage13_L_gameover.Text = who
                Form1.TabControl1.SelectedIndex = 12
                Form1.Show()
            Case "faceLeft", "faceRight"
                ChangeFace(who, state)
            Case "hitUp", "hitDown"
                UpdateHitImage(who, state)
            Case "useSkill", "resetSkill"
                UpdateSkillStateImage(who, state)
            Case "isNumb", "notNumb", "isReverse", "notReverse", "canJump", "cantJump"
                UpdateSkillImage(who, state)
        End Select
    End Sub



    Private Sub UpdateGameStatus(state As String)
        Select Case state
            Case "resetBall"
                Ball.Visible = True
        End Select
    End Sub

    Private Sub UpdateScore(who As String)
        Dim scoreText = If(who = "player1", player1ScoreText, player2ScoreText)
        scoreText.Text = Format(Val(scoreText.Text) + 1, "00")
        Ball.Visible = False
    End Sub

    Private Sub UpdateHitImage(who As String, state As String)
        Dim hitImage = If(who = "player1", player1HitImage, player2HitImage)
        hitImage.Image = If(state = "hitUp", My.Resources.game_arrow_up, My.Resources.game_arrow_down)
    End Sub

    Private Sub UpdateSkillStateImage(who As String, state As String)
        Dim skillImage = If(who = "player1", player1SkillImage, player2SkillImage)
        skillImage.BackColor = If(state = "useSkill", Color.Red, Color.Green)
    End Sub

    Private Sub UpdateSkillImage(who As String, state As String)
        Dim playerPictureBox As PictureBox = If(who = "player1", Player1, Player2)
        Select Case state
            Case "isNumb"
                Dim thunderImage As PictureBox = New PictureBox()
                thunderImage = New PictureBox With {
                    .Name = "thunderImageFor" & who,
                    .SizeMode = PictureBoxSizeMode.StretchImage,
                    .Image = My.Resources.game_thunder,
                    .Size = New Size(90, 160),
                    .Location = New Point(playerPictureBox.Location.X + 30, playerPictureBox.Location.Y - 160),
                    .BackColor = Color.Transparent
                }
                Me.Controls.Add(thunderImage)
            Case "notNumb"
                Dim thunderImageControl As Control = Me.Controls.Find("thunderImageFor" & who, False).FirstOrDefault()
                If thunderImageControl IsNot Nothing Then
                    Me.Controls.Remove(thunderImageControl)
                    thunderImageControl.Dispose()
                End If
            Case "cantJump"
                Dim vineX As Integer = If(who = "player2", 0, Stick.Location.X + Stick.Width)
                Dim vineY As Integer = 639
                Dim vine As PictureBox = New PictureBox With {
                    .Name = "vineImageFor" & who,
                    .SizeMode = PictureBoxSizeMode.StretchImage,
                    .Image = My.Resources.game_vine,
                    .Size = New Size(Me.ClientSize.Width / 2, 100),
                    .BackColor = Color.Transparent,
                    .Location = New Point(vineX, vineY)
                }
                Me.Controls.Add(vine)
            Case "canJump"
                Dim vineImageControl As Control = Me.Controls.Find("vineImageFor" & who, False).FirstOrDefault()
                If vineImageControl IsNot Nothing Then
                    Me.Controls.Remove(vineImageControl)
                    vineImageControl.Dispose()
                End If
            Case "isReverse"
                Dim fireX As Integer = If(who = "player2", 0, Stick.Location.X + Stick.Width)
                Dim fireY As Integer = 619
                Dim fire As PictureBox = New PictureBox With {
                    .Name = "fireImageFor" & who,
                    .SizeMode = PictureBoxSizeMode.StretchImage,
                    .Image = My.Resources.game_fire,
                    .Size = New Size(Me.ClientSize.Width / 2, 150),
                    .BackColor = Color.Transparent,
                    .Location = New Point(fireX, fireY)
                }
                Me.Controls.Add(fire)
            Case "notReverse"
                Dim fireImageControl As Control = Me.Controls.Find("fireImageFor" & who, False).FirstOrDefault()
                If fireImageControl IsNot Nothing Then
                    Me.Controls.Remove(fireImageControl)
                    fireImageControl.Dispose()
                End If
        End Select
    End Sub
    Private Sub ChangeFace(player As String, face As String)
        Dim currentPlayer As PictureBox = If(player = "player1", Player1, Player2)
        Dim currentCharacter As Integer = If(player = "player1", Player1Character, Player2Character)

        Dim newImage As Image = GetNewFaceImage(currentCharacter, face)
        If currentPlayer.Image IsNot newImage Then
            currentPlayer.Image = newImage
        End If
    End Sub

    Private Function GetNewFaceImage(character As Integer, face As String) As Image
        Select Case character
            Case 0
                Return If(face = "faceLeft", My.Resources.game_pikachu_left, My.Resources.game_pikachu_right)
            Case 1
                Return If(face = "faceLeft", My.Resources.game_charmander_left, My.Resources.game_charmander_right)
            Case 2
                Return If(face = "faceLeft", My.Resources.game_squirtle_left, My.Resources.game_squirtle_right)
            Case 3
                Return If(face = "faceLeft", My.Resources.game_bulbasaur_left, My.Resources.game_bulbasaur_right)
            Case Else
                Return Nothing
        End Select
    End Function



    Private Sub UpdateObjPosition(playerPictureBox As PictureBox, x As Integer, y As Integer)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() UpdateObjPosition(playerPictureBox, x, y))
        Else
            playerPictureBox.Location = New Point(x, y)
        End If
    End Sub


End Class
