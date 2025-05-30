Imports System.Diagnostics.Eventing.Reader
Imports System.Drawing.Text
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Windows

Public Class LocalGame
    Public player1 As Player
    Public player2 As Player
    Public ball As Ball
    Public stick As Stick
    Public gameLogic As GameLogic
    Public resetTimer As Timer

    Public WithEvents PlayerTimer As New Timer()
    Public WithEvents BallTimer As New Timer()
    Public Property floorLevel As Integer = 669
    Public whoMissed As Integer

    Public Property practiceCountDownLabel As Label
    Public Property PracticeTimer As Timer

#Region "模擬介面"
    Public Property _gameMode As Integer = 1
    Public Property p1_character As Integer
    Public Property p2_character As Integer
    Public Shared Property Instance As LocalGame

    Public Sub New(gameMode As Integer, p1_character As Integer, p2_character As Integer)
        InitializeComponent()
        _gameMode = gameMode
        _p1_character = p1_character
        _p2_character = p2_character
        Instance = Me
    End Sub

    Public Sub Closing_LocalGame() Handles Me.Closing
        If (_gameMode = 2) Then
            player1.Dispose()
        End If
        player2.Dispose()
        ball.Dispose()
        stick.Dispose()
        gameLogic.Dispose()
        If (resetTimer IsNot Nothing) Then
            RemoveHandler resetTimer.Tick, AddressOf ResetTimer_Tick
            resetTimer.Stop()
            resetTimer.Dispose()
        End If
        If (PracticeTimer IsNot Nothing) Then
            RemoveHandler PracticeTimer.Tick, AddressOf PracticeTimer_Tick
            PracticeTimer.Stop()
            PracticeTimer.Dispose()
        End If
        RemoveHandler PlayerTimer.Tick, AddressOf PlayerTimerTick
        PlayerTimer.Stop()
        PlayerTimer.Dispose()

        RemoveHandler BallTimer.Tick, AddressOf BallTimerTick
        BallTimer.Stop()
        BallTimer.Dispose()

        Form1.LocalGameStart = 0
        Form1.Show()
        Form1.TabControl1.SelectedIndex = 0
    End Sub
    Private Sub LocalGame_Closed() Handles MyBase.Closed
        If resetTimer IsNot Nothing Then
            resetTimer.Stop()
            resetTimer.Dispose()
        End If

        If BallTimer IsNot Nothing Then
            RemoveHandler BallTimer.Tick, AddressOf BallTimerTick
            BallTimer.Stop()
            BallTimer.Dispose()
        End If
    End Sub

    Private Sub LocalGame_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources._123
        If (_gameMode = 1) Then
            player1HitImage.Visible = False
            player1SKillImage.Visible = False
        End If
        InitialPlayer(_p1_character, _p2_character)
        InitialField()
        InitialBall()
    End Sub
#End Region

    Private Sub InitialField()

        Me.Size = New Size(1280, 760)
        Me.BackgroundImage = My.Resources.game_background
        Me.BackgroundImageLayout = ImageLayout.Stretch
        Me.DoubleBuffered = True '不然會有殘影


        stick = New Stick()
        Me.Controls.Add(stick)

        If (_gameMode = 1) Then
            Dim score_pic_size = Me.ClientSize.Width / 6
            Dim score_point_1 As PictureBox = New PictureBox With {
                .Size = New Size(score_pic_size, score_pic_size / 4),
                .Location = New Point(score_pic_size * 5, Me.ClientSize.Height - score_pic_size / 4),
                .Image = My.Resources.game_score_1,
                .SizeMode = PictureBoxSizeMode.StretchImage,
                .BorderStyle = BorderStyle.Fixed3D
            }

            Dim score_point_2 As PictureBox = New PictureBox With {
                .Size = New Size(score_pic_size + 1, score_pic_size / 4),
                .Image = My.Resources.game_score_2,
                .SizeMode = PictureBoxSizeMode.StretchImage,
                .Location = New Point(score_pic_size * 4, Me.ClientSize.Height - score_pic_size / 4),
                .BorderStyle = BorderStyle.Fixed3D
            }

            Dim score_point_3 As PictureBox = New PictureBox With {
                .Size = New Size(score_pic_size, score_pic_size / 4),
                .Image = My.Resources.game_score_3,
                .SizeMode = PictureBoxSizeMode.StretchImage,
                .Location = New Point(score_pic_size * 3, Me.ClientSize.Height - score_pic_size / 4),
                .BorderStyle = BorderStyle.Fixed3D
            }

            Me.Controls.Add(score_point_1)
            Me.Controls.Add(score_point_2)
            Me.Controls.Add(score_point_3)

            practiceCountDownLabel = New Label With {
                .Text = "08",
                .Size = New Size(200, 200),
                .Location = New Point(560, 10),
                .Font = New Font("新細明體", 80),
                .BackColor = Color.Transparent
            }
            Me.Controls.Add(practiceCountDownLabel)

            PracticeTimer = New Timer With {
                .Interval = 1000
            }
            AddHandler PracticeTimer.Tick, AddressOf PracticeTimer_Tick
            PracticeTimer.Start()
        End If

        PlayerTimer.Interval = 20
        AddHandler PlayerTimer.Tick, AddressOf PlayerTimerTick
        PlayerTimer.Start()
        BallTimer.Interval = 20
        AddHandler BallTimer.Tick, AddressOf BallTimerTick
        BallTimer.Start()


#Region "P1_SET"
        If _gameMode = 2 Then
            player1ScoreText.Font = New Font("新細明體", 72)
            player1ScoreText.Text = "00"
            player1HitImage.Image = My.Resources.game_arrow_up
        Else
            player1ScoreText.Visible = False
            player1HitImage.Visible = False
        End If

#End Region

#Region "P2_SET"
        player2ScoreText.Font = New Font("新細明體", 72)
        player2ScoreText.Text = "00"
        player2HitImage.Image = My.Resources.game_arrow_up
#End Region

        gameLogic = New GameLogic()
    End Sub

    Private Sub InitialPlayer(p1_character As Integer, p2_character As Integer)

#Region "P1_set" '右邊玩家
        If _gameMode = 2 Then '整合UI介面這裡要改 多人模式不生成
            player1 = New Player(1, p1_character) '(PlayerID,Character)
            player1.Location = New Point(827, floorLevel - player1.Height)
            player1.SendToBack()
            Me.Controls.Add(player1)
            AddHandler Me.KeyDown, AddressOf player1.PlayerKeyDown
            AddHandler Me.KeyUp, AddressOf player1.PlayerKeyUp
        End If
#End Region

#Region "P2_set" '左邊玩家 一定生成不用判斷
        player2 = New Player(2, p2_character)
        player2.Location = New Point(60, floorLevel - player2.Height)
        player2.SendToBack()
        Me.Controls.Add(player2)
        AddHandler Me.KeyDown, AddressOf player2.PlayerKeyDown
        AddHandler Me.KeyUp, AddressOf player2.PlayerKeyUp
#End Region

    End Sub
    Private Sub InitialBall()
        ball = New Ball()
        Me.Controls.Add(ball)
        ball.BringToFront()
    End Sub
    Public Sub ResetBall(missed As Integer)
        BallTimer.Stop()
        If Me.Controls.Contains(ball) Then
            Me.Controls.Remove(ball)
            ball.Dispose()
        End If
        whoMissed = missed
        If resetTimer IsNot Nothing Then
            resetTimer.Stop()
            RemoveHandler resetTimer.Tick, AddressOf ResetTimer_Tick
            resetTimer.Dispose()
        End If
        resetTimer = New Timer()
        resetTimer.Interval = If(_gameMode = 1, 500, 1500)
        AddHandler resetTimer.Tick, AddressOf ResetTimer_Tick
        resetTimer.Start()
    End Sub
    Private Sub PlayerTimerTick(sender As Object, e As EventArgs) Handles PlayerTimer.Tick
        If _gameMode = 2 Then
            player1.PlayerUpdate()
        End If
        player2.PlayerUpdate()
    End Sub
    Private Sub BallTimerTick(sender As Object, e As EventArgs) Handles BallTimer.Tick
        ball.BallUpdate(player2, player1)
    End Sub
    Private Sub ResetTimer_Tick(sender As Object, e As EventArgs)
        Debug.Write("reset")
        If Me.Controls.Contains(ball) Then
            Me.Controls.Remove(ball)
            ball.Dispose()
        End If
        ball = New Ball()
        Me.Controls.Add(ball)
        ball.BringToFront()

        Dim newX As Integer
        If _gameMode = 2 Then
            newX = If(whoMissed = 2, 60, 827)
        ElseIf _gameMode = 1 Then
            newX = 60
        End If

        Dim newY As Integer = 50
        ball.ballX = newX
        ball.ballY = newY
        ball.Location = New Point(newX, newY)
        DirectCast(sender, Timer).Stop()
        DirectCast(sender, Timer).Dispose()
        BallTimer.Start()

    End Sub
    Private Sub PracticeTimer_Tick(sender As Object, e As EventArgs)
        gameLogic.practiceCountDown()
    End Sub
    Public Sub HandlesPlayerAction(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            BallTimer.Stop()
            If (_gameMode = 1) Then
                PracticeTimer.Stop()
            End If
            Me.Hide()
            Form1.Show()
            Form1.TabControl1.SelectedIndex = 9
        End If
    End Sub
End Class
Public Class Player
    '移動、打擊、撞牆
    Inherits PictureBox
    Public Property playerID As Integer
    Public Property facingLeft As Boolean
    Public Property character As Integer '0 = 皮卡丘 1 = 小火龍 2 = 傑尼龜 3 = 玅娃種子
    Public Property moveLeft As Boolean
    Public Property moveRight As Boolean
    Public Property moveDown As Boolean
    Public Property jumpDisabled As Boolean = False
    Public Property jumpState As Integer = 0
    Public Property jumpPower As Integer = 60
    Public Property playerSpeedY As Integer = 0
    Public Property playerSpeedX As Integer = 0
    Public Property playerAccelerationX As Double = 0
    Public Property playerAccelerationY As Double = 5 '重力
    Public Property hitState As Boolean = 0 '0代表低手、1代表殺球
    Public Property skillReady As Boolean = True
    Public Property enemyPlayer As Player
    Public fire As PictureBox
    Public vine As PictureBox
    Public moveReverse As Boolean = False
    Public moveDisabled As Boolean = False
    Public heavySmash As Boolean = False

    Public Sub New(playerID As Integer, playerCharacter As Integer)
        Me.playerID = playerID
        Me.character = playerCharacter
        Me.SizeMode = PictureBoxSizeMode.StretchImage
        Me.BackColor = Color.Transparent
        Me.Size = New Size(110, 120)

        If playerID = 1 Then
            facingLeft = True
        ElseIf playerID = 2 Then
            facingLeft = False
        End If
        SetCharacterImage()
        SetSkillImage()
    End Sub
    Private Sub SetCharacterImage()
        Select Case character
            Case 0 ' 皮卡丘
                Me.Image = If(facingLeft, My.Resources.game_pikachu_left, My.Resources.game_pikachu_right)
            Case 1 ' 小火龍
                Me.Image = If(facingLeft, My.Resources.game_charmander_left, My.Resources.game_charmander_right)
            Case 2 ' 傑尼龜
                Me.Image = If(facingLeft, My.Resources.game_squirtle_left, My.Resources.game_squirtle_right)
            Case 3 ' 妙娃種子
                Me.Image = If(facingLeft, My.Resources.game_bulbasaur_left, My.Resources.game_bulbasaur_right)
        End Select
    End Sub
    Private Sub SetSkillImage()

        If LocalGame.Instance._gameMode = 2 Then
            LocalGame.Instance.player1SKillImage.SizeMode = PictureBoxSizeMode.StretchImage
            LocalGame.Instance.player1SKillImage.BackColor = Color.Green
        End If

        LocalGame.Instance.player2SkillImage.SizeMode = PictureBoxSizeMode.StretchImage
        LocalGame.Instance.player2SkillImage.BackColor = Color.Green

        Select Case playerID
            Case 1
                Select Case character
                    Case 0
                        LocalGame.Instance.player1SKillImage.Image = My.Resources.game_thunder
                    Case 1
                        LocalGame.Instance.player1SKillImage.Image = My.Resources.game_fire_icon_png
                    Case 2
                        LocalGame.Instance.player1SKillImage.Image = My.Resources.game_strength
                    Case 3
                        LocalGame.Instance.player1SKillImage.Image = My.Resources.game_vine_icon
                End Select
            Case 2
                Select Case character
                    Case 0
                        LocalGame.Instance.player2SkillImage.Image = My.Resources.game_thunder
                    Case 1
                        LocalGame.Instance.player2SkillImage.Image = My.Resources.game_fire_icon_png
                    Case 2
                        LocalGame.Instance.player2SkillImage.Image = My.Resources.game_strength
                    Case 3
                        LocalGame.Instance.player2SkillImage.Image = My.Resources.game_vine_icon
                End Select
        End Select
    End Sub
    Public Sub PlayerKeyDown(sender As Object, e As KeyEventArgs)
        Select Case playerID
            Case 1
                Select Case e.KeyCode
                    Case Keys.Left
                        If (moveReverse = True) Then
                            moveRight = True
                            facingLeft = False
                        Else
                            moveLeft = True
                            facingLeft = True
                        End If
                        SetCharacterImage()
                    Case Keys.Right
                        If (moveReverse = True) Then
                            moveLeft = True
                            facingLeft = True
                        Else
                            moveRight = True
                            facingLeft = False
                        End If
                        SetCharacterImage()
                    Case Keys.Up
                        If Not jumpDisabled AndAlso jumpState = 0 Then
                            jumpState = 1
                            playerSpeedY = -jumpPower
                        End If
                    Case Keys.Down
                        moveDown = True
                End Select
            Case 2
                Select Case e.KeyCode
                    Case Keys.A
                        If (moveReverse = True) Then
                            moveRight = True
                            facingLeft = False
                        Else
                            moveLeft = True
                            facingLeft = True
                        End If
                        SetCharacterImage()
                    Case Keys.D
                        If (moveReverse = True) Then
                            moveLeft = True
                            facingLeft = True
                        Else
                            moveRight = True
                            facingLeft = False
                        End If
                        SetCharacterImage()
                    Case Keys.W
                        If Not jumpDisabled AndAlso jumpState = 0 Then
                            jumpState = 1
                            playerSpeedY = -jumpPower
                        End If
                    Case Keys.S
                        moveDown = True

                End Select
        End Select
    End Sub

    Public Sub PlayerKeyUp(sender As Object, e As KeyEventArgs)
        Select Case playerID
            Case 1
                Select Case e.KeyCode
                    Case Keys.Left
                        If (moveReverse = True) Then
                            moveRight = False
                            playerSpeedX = 0
                        Else
                            moveLeft = False
                            playerSpeedX = 0
                        End If
                    Case Keys.Right
                        If (moveReverse = True) Then
                            moveLeft = False
                            playerSpeedX = 0
                        Else
                            moveRight = False
                            playerSpeedX = 0
                        End If
                    Case Keys.NumPad1
                        hitState = If(hitState = 0, 1, 0)
                        LocalGame.Instance.player1HitImage.Image = If(hitState = 0, My.Resources.game_arrow_up, My.Resources.game_arrow_down)
                    Case Keys.NumPad2
                        LocalGame.Instance.ball.PlayerHitBall(Me, hitState)
                    Case Keys.NumPad3
                        UseSkill()
                End Select
            Case 2
                Select Case e.KeyCode
                    Case Keys.A
                        If (moveReverse = True) Then
                            moveRight = False
                            playerSpeedX = 0
                        Else
                            moveLeft = False
                            playerSpeedX = 0
                        End If
                    Case Keys.D
                        If (moveReverse = True) Then
                            moveLeft = False
                            playerSpeedX = 0
                        Else
                            moveRight = False
                            playerSpeedX = 0
                        End If
                    Case Keys.J
                        hitState = If(hitState = 0, 1, 0)
                        LocalGame.Instance.player2HitImage.Image = If(hitState = 0, My.Resources.game_arrow_up, My.Resources.game_arrow_down)
                    Case Keys.K
                        LocalGame.Instance.ball.PlayerHitBall(Me, hitState)
                    Case Keys.U
                        UseSkill()
                End Select
        End Select
    End Sub


    Public Sub PlayerUpdate()
        If (moveDisabled = True) Then
            Return
        End If
        playerSpeedX += playerAccelerationX * LocalGame.Instance.PlayerTimer.Interval / 100
        playerSpeedY += playerAccelerationY * LocalGame.Instance.PlayerTimer.Interval / 100
        Me.Left += playerSpeedX * LocalGame.Instance.PlayerTimer.Interval / 100 + 0.5 * playerAccelerationX * (LocalGame.Instance.PlayerTimer.Interval / 100) ^ 2
        Me.Top += playerSpeedY * LocalGame.Instance.PlayerTimer.Interval / 100 + 0.5 * playerAccelerationY * (LocalGame.Instance.PlayerTimer.Interval / 100) ^ 2

        HandlePlayerMove()
        Me.Refresh()
    End Sub


    Private Sub HandlePlayerMove()
        Dim rightBound = If(playerID = 2, LocalGame.Instance.stick.Left, 1152)
        Dim leftBound = If(playerID = 2, 12, LocalGame.Instance.stick.Right)

        If (jumpState = 1) Then
            Me.Top += playerSpeedY
            playerSpeedY += playerAccelerationY
        End If
        If Me.Top < 0 Then
            Me.Top = 0
            playerSpeedY = 0
        ElseIf Me.Top + Me.Height > LocalGame.Instance.floorLevel Then
            Me.Top = LocalGame.Instance.floorLevel - Me.Height
            playerSpeedY = 0
            jumpState = 0
        End If

        ' 檢查向左移動
        If moveLeft Then
            If Me.Left > leftBound Then
                playerSpeedX = -30
            Else
                playerSpeedX = 0
                Me.Left = leftBound
            End If
        End If

        ' 檢查向右移動
        If moveRight Then
            If Me.Left + Me.Width < rightBound Then
                playerSpeedX = 30
            Else
                playerSpeedX = 0
                Me.Left = rightBound - Me.Width
            End If
        End If

        ' 確保玩家不會超出左邊界
        If Me.Left < leftBound Then
            Me.Left = leftBound
            playerSpeedX = 0
        End If

        ' 確保玩家不會超出右邊界
        If Me.Left + Me.Width > rightBound Then
            Me.Left = rightBound - Me.Width
            playerSpeedX = 0
        End If
    End Sub

    Public Sub UseSkill()
        If skillReady Then
            skillReady = False

            If LocalGame.Instance._gameMode = 2 Then
                enemyPlayer = If(playerID = 1, LocalGame.Instance.player2, LocalGame.Instance.player1)
            End If

            Dim skillCoolDownTimer As New Timer()
            Dim skillDurationTimer As New Timer()

            Select Case character
                Case 0 ' Pikachu
                    LocalGame.Instance.ball.causeNumb = True
                    skillCoolDownTimer.Interval = 5000
                    skillDurationTimer.Interval = 1500
                Case 1 ' Charmander

                    If LocalGame.Instance._gameMode = 2 Then
                        enemyPlayer.moveReverse = True
                    End If

                    If Not LocalGame.Instance.Controls.Contains(fire) Then
                        fire = New PictureBox()
                        fire.SizeMode = PictureBoxSizeMode.StretchImage
                        fire.Image = My.Resources.game_fire
                        fire.Size = New Size(LocalGame.Instance.ClientSize.Width / 2, 150)
                        If LocalGame.Instance._gameMode = 2 Then
                            If (enemyPlayer.playerID = 1) Then
                                fire.Location = New Point(LocalGame.Instance.stick.Location.X + LocalGame.Instance.stick.Width, LocalGame.Instance.floorLevel - 50)
                            Else
                                fire.Location = New Point(0, LocalGame.Instance.floorLevel - 50)
                            End If
                        Else
                            fire.Location = New Point(LocalGame.Instance.stick.Location.X + LocalGame.Instance.stick.Width, LocalGame.Instance.floorLevel - 50)
                        End If
                        fire.BackColor = Color.Transparent
                        LocalGame.Instance.Controls.Add(fire)
                    End If
                    fire.Visible = True
                    skillCoolDownTimer.Interval = 5000
                    skillDurationTimer.Interval = 1500
                Case 2 ' Squirtle
                    heavySmash = True
                    skillCoolDownTimer.Interval = 5000
                Case 3 ' Bulbasaur

                    If LocalGame.Instance._gameMode = 2 Then
                        enemyPlayer.jumpDisabled = True
                    End If

                    If Not LocalGame.Instance.Controls.Contains(vine) Then
                        vine = New PictureBox()
                        vine.SizeMode = PictureBoxSizeMode.StretchImage
                        vine.Image = My.Resources.game_vine
                        vine.Size = New Size(LocalGame.Instance.ClientSize.Width / 2, 100)
                        Select Case LocalGame.Instance._gameMode
                            Case 1
                                vine.Location = New Point(LocalGame.Instance.stick.Location.X + LocalGame.Instance.stick.Width, LocalGame.Instance.floorLevel - 30)
                            Case 2
                                If (enemyPlayer.playerID = 1) Then
                                    vine.Location = New Point(LocalGame.Instance.stick.Location.X + LocalGame.Instance.stick.Width, LocalGame.Instance.floorLevel - 30)
                                Else
                                    vine.Location = New Point(0, LocalGame.Instance.floorLevel - 30)
                                End If
                        End Select

                        vine.BackColor = Color.Transparent
                        LocalGame.Instance.Controls.Add(vine)
                    End If
                    vine.Visible = True
                    skillCoolDownTimer.Interval = 5000
                    skillDurationTimer.Interval = 1500
            End Select
            If (playerID = 1) Then
                LocalGame.Instance.player1SKillImage.BackColor = Color.Red
            ElseIf (playerID = 2) Then
                LocalGame.Instance.player2SkillImage.BackColor = Color.Red
            End If
            AddHandler skillCoolDownTimer.Tick, AddressOf ResetSkill
            AddHandler skillDurationTimer.Tick, AddressOf SkillDuration
            skillCoolDownTimer.Start()
            skillDurationTimer.Start()
        End If

    End Sub
    Private Sub ResetSkill(sender As Object, e As EventArgs)
        skillReady = True
        Select Case playerID
            Case 1
                LocalGame.Instance.player1SKillImage.BackColor = Color.Green
            Case 2
                LocalGame.Instance.player2SkillImage.BackColor = Color.Green
        End Select
        DirectCast(sender, Timer).Stop()
        DirectCast(sender, Timer).Dispose()
    End Sub

    Private Sub SkillDuration(sender As Object, e As EventArgs)
        Select Case character
            Case 0
                Me.moveDisabled = False
                Me.jumpDisabled = False

                If LocalGame.Instance._gameMode = 2 Then
                    enemyPlayer.moveDisabled = False
                    enemyPlayer.jumpDisabled = False
                End If

                Dim thunderImageControl As Control = LocalGame.Instance.Controls.Find("thunderImage", True).FirstOrDefault()
                If thunderImageControl IsNot Nothing Then
                    Me.Controls.Remove(thunderImageControl)
                    thunderImageControl.Dispose()
                End If
            Case 1
                fire.Visible = False
                If LocalGame.Instance._gameMode = 2 Then
                    enemyPlayer.moveReverse = False
                    enemyPlayer.moveRight = False
                    enemyPlayer.moveLeft = False
                End If
            Case 3
                vine.Visible = False
                If LocalGame.Instance._gameMode = 2 Then
                    enemyPlayer.jumpDisabled = False
                End If
        End Select
        DirectCast(sender, Timer).Stop()
        DirectCast(sender, Timer).Dispose()
    End Sub
End Class
Public Class Ball
    Inherits PictureBox
    Public Property ballRadius As Integer = 50
    Public Property ballX As Integer = 50
    Public Property ballY As Integer = 50
    Public Property ballSpeedX As Double = 5
    Public Property ballSpeedY As Double = 5
    Public Property BallaccelerationX As Double = 0
    Public Property BallaccelerationY As Double = 10

    Public Property causeNumb As Boolean = False
    Public Property thunderImage As PictureBox

    Public Sub New()
        Me.BringToFront()
        Me.BackgroundImage = My.Resources.ball
        Me.BackColor = Color.Transparent
        Me.BackgroundImageLayout = ImageLayout.Stretch
        Me.Size = New Size(2 * ballRadius, 2 * ballRadius)
        Me.Location = New Point(50, 50)
    End Sub

    Public Sub BallUpdate(player2 As Player, Optional player1 As Player = Nothing)

        ballSpeedX += BallaccelerationX * LocalGame.Instance.BallTimer.Interval / 100
        ballSpeedY += BallaccelerationY * LocalGame.Instance.BallTimer.Interval / 100
        If (LocalGame.Instance.gameLogic.GamePaused = True) Then
            Return
        End If

        ballX += ballSpeedX * LocalGame.Instance.BallTimer.Interval / 100 + 0.5 * BallaccelerationX * (LocalGame.Instance.BallTimer.Interval / 100) ^ 2
        ballY += ballSpeedY * LocalGame.Instance.BallTimer.Interval / 100 + 0.5 * BallaccelerationY * (LocalGame.Instance.BallTimer.Interval / 100) ^ 2
        If player1 IsNot Nothing Then
            CheckPlayerCollision(player1)
        End If
        CheckPlayerCollision(player2)
        CheckBoundCollision()
        CheckStickCollision()
        UpdateBallLocation()
    End Sub
    Public Sub PlayerHitBall(player As Player, hitstate As Boolean)
        Dim centerballx As Single = (Me.Left + Me.Right) / 2
        Dim centerbally As Single = (Me.Top + Me.Bottom) / 2
        Dim hitdistance As Integer = 100
        If (((player.Left - hitdistance <= centerballx) And (centerballx <= player.Right + hitdistance)) And (player.Top - hitdistance <= centerbally) And (centerbally <= player.Bottom + hitdistance)) Then
            Select Case hitstate
                Case 0
                    ballSpeedX = 0
                    If (ballSpeedY < 200) Then
                        ballSpeedY = -90
                    Else
                        ballSpeedY = -Math.Abs(ballSpeedY)
                        ballSpeedY -= 90
                    End If
                Case 1
                    ballSpeedX = If(player.playerID = 1, -120, 150)
                    ballSpeedY = 100
                    If (player.heavySmash = True) Then
                        ballSpeedX *= 1.5
                        ballSpeedY *= 1.5
                        player.heavySmash = False
                    End If
            End Select
        End If
        UpdateBallLocation()
    End Sub
    Private Sub CheckBoundCollision()
        If ballY + ballRadius > LocalGame.Instance.floorLevel Then
            ballY = LocalGame.Instance.floorLevel - ballRadius
            ballSpeedY = -ballSpeedY * 0.7
            Dim whoMissed As Integer = If(ballX < LocalGame.Instance.ClientSize.Width / 2, 2, 1) '左沒接到球whomissed = 2
            If (LocalGame.Instance.gameLogic.GamePaused = False) Then
                LocalGame.Instance.gameLogic.HandleScore(whoMissed)
            End If
        End If

        If ballX - ballRadius < 0 Or ballX + ballRadius > LocalGame.Instance.ClientRectangle.Width Then
            ballSpeedX = -ballSpeedX
        End If
        If ballY - ballRadius < 0 Or ballY + ballRadius > LocalGame.Instance.ClientRectangle.Height Then
            ballSpeedY = -ballSpeedY
        End If
    End Sub
    Private Sub CheckStickCollision()
        Dim stickRect As Rectangle = LocalGame.Instance.stick.Bounds
        Dim ballRect As New Rectangle(CInt(ballX - ballRadius), CInt(ballY - ballRadius), 2 * ballRadius, 2 * ballRadius)
        If ballRect.IntersectsWith(stickRect) Then
            Dim ballCenterX As Integer = ballRect.Left + ballRadius
            Dim ballCenterY As Integer = ballRect.Top + ballRadius

            Dim stickCenterX As Integer = stickRect.Left + LocalGame.Instance.stick.Width / 2

            If ballCenterX < stickCenterX Then
                ballSpeedX = -Math.Abs(ballSpeedX) ' 向左反彈
            ElseIf ballCenterX > stickCenterX Then
                ballSpeedX = Math.Abs(ballSpeedX) ' 向右反彈
            End If

            If ballCenterY < LocalGame.Instance.stick.Top Then
                ballSpeedY = -Math.Abs(ballSpeedY) ' 向上反彈
            End If
        End If
    End Sub
    Private Sub CheckPlayerCollision(player As Player)
        Dim ballRect As New Rectangle(CInt(ballX - ballRadius), CInt(ballY - ballRadius), 2 * ballRadius, 2 * ballRadius)
        Dim playerRect As Rectangle = player.Bounds
        Dim centerballx As Single = (Me.Left + Me.Right) / 2
        Dim centerbally As Single = (Me.Top + Me.Bottom) / 2
        Dim centerplayerx As Single = (player.Left + player.Right) / 2
        Dim centerplayery As Single = (player.Top + player.Bottom) / 2


        If ballRect.IntersectsWith(playerRect) Then
            If Me.Bottom > player.Top And (player.Left <= Me.Left <= player.Right Or player.Left <= Me.Right <= player.Right) Then
                Me.Top = player.Top - Me.Height
                ballSpeedX += 0.5
                ballSpeedY = -Math.Abs(ballSpeedY)
                ballSpeedY -= 1
                ballSpeedX *= -1
            ElseIf ballSpeedX >= 0 Then '球從左邊來的碰撞
                If centerballx < centerplayerx Then
                    If 0 < ballSpeedY < 30 Then
                        ballSpeedY = 80
                    End If
                    ballSpeedX += 0.5
                    ballSpeedY = Math.Abs(ballSpeedY)
                    ballSpeedY -= 1
                    ballSpeedX *= -1
                Else
                    ballSpeedX += 0.5
                    ballSpeedY = Math.Abs(ballSpeedY)
                    ballSpeedY -= 1
                End If
            ElseIf ballSpeedX < 0 Then '球從右邊來的碰撞
                If centerballx > centerplayerx Then
                    If 0 < ballSpeedY < 30 Then
                        ballSpeedY = 80
                    End If
                    ballSpeedX -= 0.5
                    ballSpeedY = Math.Abs(ballSpeedY)
                    ballSpeedY -= 1
                    ballSpeedX *= -1
                Else
                    ballSpeedX -= 0.5
                    ballSpeedY = Math.Abs(ballSpeedY)
                    ballSpeedY -= 1
                End If
            End If
            If (causeNumb = True) Then
                causeNumb = False
                player.moveDisabled = True
                player.jumpDisabled = True
                thunderImage = New PictureBox With {
                    .Name = "thunderImage",
                    .SizeMode = PictureBoxSizeMode.StretchImage,
                    .Image = My.Resources.game_thunder,
                    .Size = New Size(70, 160),
                    .Location = New Point(player.Location.X + 30, player.Location.Y - 160),
                    .BackColor = Color.Transparent
                }
                LocalGame.Instance.Controls.Add(thunderImage)
                thunderImage.SendToBack()
            End If
        End If


    End Sub
    Private Sub UpdateBallLocation()
        Me.Location = New Point(CInt(ballX - ballRadius), CInt(ballY - ballRadius))
        Me.Refresh()
    End Sub

End Class
Public Class Stick
    Inherits PictureBox
    Public Sub New()
        Me.Size = New Size(24, 394)
        Me.Image = My.Resources.stick
        Me.Location = New Point(LocalGame.Instance.ClientSize.Width / 2, LocalGame.Instance.ClientSize.Height - 300)
    End Sub
End Class
Public Class GameLogic
    Inherits Label
    Public Property GamePaused As Boolean = False
    Private Property player1Score = 0
    Private Property player2Score = 0

    Public Sub HandleScore(playerID As Integer) 'playerID是哪個人沒接到球
        UpdateScore(playerID)
        If LocalGame.Instance.Controls.Contains(LocalGame.Instance.ball) Then
            LocalGame.Instance.Controls.Remove(LocalGame.Instance.ball)
            LocalGame.Instance.ball.Dispose()
        End If
        GamePaused = True
        LocalGame.Instance.BallTimer.Stop()
        CheckForWinner()
        ResetPlayerState()
        LocalGame.Instance.ResetBall(playerID)
        GamePaused = False
    End Sub
    Public Sub UpdateScore(playerID As Integer)
        If (LocalGame.Instance._gameMode = 1) Then
            Dim score_pic_size = LocalGame.Instance.ClientSize.Width / 6
            If LocalGame.Instance.ball.ballX >= score_pic_size * 3 AndAlso LocalGame.Instance.ball.ballX < score_pic_size * 4 Then
                player2Score += 3
            ElseIf LocalGame.Instance.ball.ballX >= score_pic_size * 4 AndAlso LocalGame.Instance.ball.ballX < score_pic_size * 5 Then
                player2Score += 2
            ElseIf LocalGame.Instance.ball.ballX >= score_pic_size * 5 AndAlso LocalGame.Instance.ball.ballX < score_pic_size * 6 Then
                player2Score += 1
            End If
            LocalGame.Instance.player2ScoreText.Text = Format(player2Score, "00")
        Else
            If (playerID = 1) Then
                player2Score += 1
                LocalGame.Instance.player2ScoreText.Text = Format(player2Score, "00")
            ElseIf (playerID = 2) Then
                player1Score += 1
                LocalGame.Instance.player1ScoreText.Text = Format(player1Score, "00")
            End If
        End If
    End Sub

    Private Sub CheckForWinner()
        If (LocalGame.Instance._gameMode = 2) Then
            If (player1Score = 2) Then
                PauseGame()
                LocalGame.Instance.Close()
                LocalGame.Instance.Dispose()
                Form1.TabControl1.SelectedIndex = 6
                Form1.tabpage7_L_gameover.Text = "player1"
                Form1.Show()
            ElseIf (player2Score = 2) Then
                PauseGame()
                LocalGame.Instance.Close()
                LocalGame.Instance.Dispose()
                Form1.TabControl1.SelectedIndex = 6
                Form1.tabpage7_L_gameover.Text = "player2"
                Form1.Show()
            End If
        End If

    End Sub
    Private Sub ResetPlayerState()

        If LocalGame.Instance._gameMode = 2 Then
            LocalGame.Instance.player1.skillReady = True
            LocalGame.Instance.player1.moveReverse = False
            LocalGame.Instance.player1.moveDisabled = False
            LocalGame.Instance.player1.jumpDisabled = False
        End If

        LocalGame.Instance.player2.skillReady = True
        LocalGame.Instance.player2.moveReverse = False
        LocalGame.Instance.player2.moveDisabled = False
        LocalGame.Instance.player2.jumpDisabled = False
    End Sub
    Private Sub PauseGame()
        If LocalGame.Instance.Controls.Contains(LocalGame.Instance.ball) Then
            LocalGame.Instance.Controls.Remove(LocalGame.Instance.ball)
            LocalGame.Instance.ball.Dispose()
        End If
        LocalGame.Instance.BallTimer.Stop()
        LocalGame.Instance.PlayerTimer.Stop()
    End Sub

    Public Sub practiceCountDown()
        If (Val(LocalGame.Instance.practiceCountDownLabel.Text) - 1 < 0) Then
            LocalGame.Instance.practiceCountDownLabel.Text = "00"
            PauseGame()
            GamePaused = True
            LocalGame.Instance.PracticeTimer.Stop()
            LocalGame.Instance.Close()
            LocalGame.Instance.Dispose()
            Form1.TabControl1.SelectedIndex = 3
            Form1.tabpage4_L_gameover.Text = player2Score
            Form1.Show()
            'MsgBox("您共獲得" & player2Score & "分", MsgBoxStyle.OkOnly, "打球嗎?寶!")
            ' LocalGame.Instance.Close()
            ' LocalGame.Instance.Dispose()
        Else
            LocalGame.Instance.practiceCountDownLabel.Text = Format(Val(LocalGame.Instance.practiceCountDownLabel.Text) - 1, "00")
        End If
    End Sub


End Class