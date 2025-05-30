Imports System.Deployment.Application
Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.IO
Imports System.Media
Imports System.Net
Imports System.Net.Sockets
Imports System.Reflection.Emit
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1

    Public pfc As New PrivateFontCollection()
    Private Declare Function GetAsyncKeyState Lib "user32" (ByVal nVirtkey As Integer) As Short
    Private WithEvents MediaPlayer As WMPLib.WindowsMediaPlayer
    Public LocalGameForm As LocalGame
    Public OnlineGameForm As OnlineGame
    Public client As TcpClient
    Public stream As NetworkStream
    Public LocalGameStart As Integer = 0
    Public OnlineGameStart As Integer = 0
#Region "formload"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ApplyFontToText()

        Me.StartPosition = FormStartPosition.CenterScreen
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.ClientSize = New System.Drawing.Size(1280, 720)


        TabControl1.SelectedIndex = 0
        currentTabPageIndex = TabControl1.SelectedIndex
        tabpage1_p_background.SendToBack()
        tabpage1_p_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
        tabpage10_p_background.SendToBack()
        tabpage10_p_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
        tabpage2_p_background.SendToBack()
        tabpage2_p_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
        tabpage11_P_background.SendToBack()
        tabpage11_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
        tabpage12_P_background.SendToBack()
        tabpage12_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)

        tabpage1_Timer_pikachu.Interval = 400
        tabpage1_Timer_pikachu.Start()
        Try
            ' 配置TabControl的外觀
            TabControl1.Appearance = TabAppearance.Buttons
            TabControl1.ItemSize = New Size(0, 1)
            TabControl1.SizeMode = TabSizeMode.Fixed

            ' 隱藏選項卡
        Catch ex As Exception
            ' 處理異常
        End Try

        ' 初始化 Windows Media Player 控件
        ' 設置音樂檔案的路徑
        MediaPlayer = New WMPLib.WindowsMediaPlayer With {
        .URL = "bgm.mp3"
          }
        ' 播放背景音樂
        MediaPlayer.settings.volume = 50
        MediaPlayer.settings.setMode("loop", True)
        MediaPlayer.controls.play()

    End Sub
#End Region
#Region "設定字型"
    Private Sub ApplyFontToText()
        tabpage1_L_game.Font = New Font("新細明體", 80)
        tabpage1_B_practice.Font = New Font("新細明體", 18)
        tabpage1_B_players.Font = New Font("新細明體", 18)
        tabpage1_B_connect.Font = New Font("新細明體", 18)
        tabpage1_B_leave.Font = New Font("新細明體", 18)
        tabpage1_B_set.Font = New Font("新細明體", 18)

        tabpage11_L_id.Font = New Font("新細明體", 36, FontStyle.Bold)
        tabpage11_L_port.Font = New Font("新細明體", 36, FontStyle.Bold)
        tabpage11_L_sound.Font = New Font("新細明體", 36, FontStyle.Bold)
        tabpage11_L_player.Font = New Font("新細明體", 36, FontStyle.Bold)
        tabpage11_C_player1.Font = New Font("新細明體", 28)
        tabpage11_C_player2.Font = New Font("新細明體", 28)
        tabpage11_L_keys.Font = New Font("新細明體", 36, FontStyle.Bold)
        tabpage11_B_look.Font = New Font("新細明體", 48)
    End Sub
#End Region
#Region "form1_resize"
    Private Sub form1_resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ' 取得視窗的寬度和高度
        Dim width As Integer = Me.ClientSize.Width
        Dim height As Integer = Me.ClientSize.Height


        ' 視窗大小改變時重新置中 
        Centertabpage1_B_players()
        Centertabpage2_P_flag()
        Centertabpage3_B_check()
        Centertabpage5_B_check()
        Centertabpage7_L_points()
        Centertabpage8_B_check()
        Centertabpage10_B_set()
        Centertabpage10_L_points()
        Centertabpage11()
        Centertabpage13_L_points()
    End Sub
#End Region
#Region "進入畫面"
    Private Sub Tabpage1_B_practice_Click(sender As Object, e As EventArgs) Handles tabpage1_B_practice.Click
        Dim x As Integer = tabpage3_P_pikachu.Left + tabpage3_P_pikachu.Width / 2 - tabpage3_L_choose.Width / 2
        Dim y As Integer = tabpage3_L_choose.Top
        tabpage3_L_choose.Location = New Point(x, y)
        TabControl1.SelectedIndex = 2
        currentTabPageIndex = 2
        tabpage3_P_background.Image = My.Resources.皮卡丘1
        tabpage3_P_background.SendToBack()
        tabpage3_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
        playerindex = 0
        Playerchoose = True
        MediaPlayer.controls.stop()
        tabpage3_B_check.Visible = False
        tabpage3_B_check.Enabled = False
    End Sub
    Private Sub Tabpage1_B_players_Click(sender As Object, e As EventArgs) Handles tabpage1_B_players.Click
        currentTabPageIndex = 4
        Dim x As Integer = tabpage5_P_pikachu.Left + tabpage5_P_pikachu.Width / 2 - tabpage5_L_play1C.Width / 2
        Dim y As Integer = tabpage5_L_play1C.Top
        tabpage5_L_play1C.Location = New Point(x, y)
        TabControl1.SelectedIndex = 4
        tabpage5_P_background1.Image = My.Resources.皮卡丘1
        tabpage5_P_background1.SendToBack()
        tabpage5_P_background1.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
        tabpage5_P_background2.Location = New Point(tabpage5_P_background1.Width, 0)
        tabpage5_P_background2.Image = My.Resources.皮卡丘1
        tabpage5_P_background2.SendToBack()
        tabpage5_P_background2.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
        Player1index = 0
        Player2index = 0
        tabpage5_L_play2C.Location = New Point(tabpage5_L_play1C.Location.X, tabpage5_L_play1C.Location.Y)
        tabpage5_L_play2C.Location = New Point(tabpage5_L_play1C.Location.X, tabpage5_L_play1C.Location.Y - tabpage5_L_play1C.Height - 10)
        Player1choose = True
        Player2choose = True
        MediaPlayer.controls.stop()
        tabpage5_B_check.Visible = False
        tabpage5_B_check.Enabled = False
    End Sub
    Dim OnlineGameChoosing = False
    Private Sub Tabpage1_B_connect_Click(sender As Object, e As EventArgs) Handles tabpage1_B_connect.Click
        If (tabpage11_T_id.Text = "" Or tabpage11_T_port.Text = "") Then
            MsgBox("請輸入正確IP!", MsgBoxStyle.OkOnly, "打球嗎?寶!")
            Exit Sub
        End If

        If (Not tabpage11_C_player1.Checked And Not tabpage11_C_player2.Checked) Then
            MsgBox("您尚未選擇玩家!", MsgBoxStyle.OkOnly, "打球嗎?寶!")
            Exit Sub
            TabControl1.SelectedIndex = 11
        End If
        Try
            Connect_Server()
            ListenForUpdates()
        Catch ex As Exception
            MsgBox("連線失敗!", MsgBoxStyle.OkOnly, "打球嗎?寶!")
            Exit Sub
        End Try
        Dim x As Integer = tabpage8_P_pikachu.Left + tabpage8_P_pikachu.Width / 2 - tabpage8_L_choose.Width / 2
        Dim y As Integer = tabpage8_L_choose.Top
        tabpage8_L_choose.Location = New Point(x, y)
        TabControl1.SelectedIndex = 8
        currentTabPageIndex = 8
        tabpage8_P_background.Image = My.Resources.皮卡丘1
        tabpage8_P_background.SendToBack()
        tabpage8_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
        playerindex = 0
        Playerchoose = True
        OnlineGameChoosing = True
        MediaPlayer.controls.stop()
    End Sub


    Private Sub Tabpage1_B_leave_Click(sender As Object, e As EventArgs) Handles tabpage1_B_leave.Click
        ' 停止播放音樂
        If MediaPlayer IsNot Nothing Then
            MediaPlayer.controls.stop()
            MediaPlayer.close()
            MediaPlayer = Nothing
        End If
        ' 關閉視窗
        Me.Close()
    End Sub

    Private Sub Tabpage1_B_set_Click(sender As Object, e As EventArgs) Handles tabpage1_B_set.Click
        currentTabPageIndex = TabControl1.SelectedIndex
        TabControl1.SelectedIndex = 10
        'escapePressed = False
        MediaPlayer.controls.stop()
    End Sub
    Private Sub Centertabpage1_B_players()
        ' 計算 GroupBox 在 X 軸上的置中位置
        Dim x As Integer = (Me.ClientSize.Width - tabpage1_B_players.Width) / 2

        ' 計算 GroupBox 在 Y 軸上的置中位置
        Dim y As Integer = (Me.ClientSize.Height - tabpage1_B_players.Height) / 2

        ' 設定 GroupBox 的 Location 屬性，使其置中
        tabpage1_B_practice.Location = New Point(x, y - 70)
        tabpage1_B_players.Location = New Point(x, y)
        tabpage1_B_connect.Location = New Point(x, y + 70)
        tabpage1_B_leave.Location = New Point(x, y + 140)
        tabpage1_B_set.Location = New Point(x + 161, y + 140)
        tabpage1_P_pikamove.Location = New Point(x - 700, y + 250)
        tabpage1_L_game.Location = New Point(x - 100, y - 250)

        tabpage1_P_pikamove.Parent = tabpage1_p_background
        tabpage1_L_game.Parent = tabpage1_p_background
        tabpage1_B_set.Parent = tabpage1_p_background
        tabpage1_B_leave.Parent = tabpage1_p_background
        tabpage1_B_practice.Parent = tabpage1_p_background
        tabpage1_B_players.Parent = tabpage1_p_background
        tabpage1_B_connect.Parent = tabpage1_p_background

    End Sub
#Region "跑步皮卡丘"
    Private imageNames As String() = {"_01", "_02", "_03", "_04"} '圖像名稱
    Private imageIndex As Integer = 0 ' 圖像索引
    Private dx As Integer = 40 ' PictureBox 移動量
    Private Sub Tabpage1_Timer_pikachu_Tick(sender As Object, e As EventArgs) Handles tabpage1_Timer_pikachu.Tick

        tabpage1_P_pikamove.Left += dx

        If imageIndex >= imageNames.Length Then
            imageIndex = 0
        End If

        tabpage1_P_pikamove.Image = My.Resources.ResourceManager.GetObject(imageNames(imageIndex))

        ' 增加索引以顯示下一個圖示
        imageIndex += 1

        ' 如果圖片超出了Form的寬度，重新設置位置並切換圖片
        If tabpage1_P_pikamove.Left + tabpage1_P_pikamove.Width > Me.ClientSize.Width Then
            tabpage1_P_pikamove.Left = -tabpage1_P_pikamove.Width

        End If

    End Sub
#End Region
#End Region
#Region "選角畫面單人"
    Private Sub Tabpage3_B_check_Click(sender As Object, e As EventArgs) Handles tabpage3_B_check.Click
        LocalGameForm = New LocalGame(1, -1, final_Playerindex)
        LocalGameForm.StartPosition = FormStartPosition.Manual
        LocalGameForm.Location = New Point(Me.Location.X, Me.Location.Y)
        LocalGameForm.Show()
        Me.Hide()
    End Sub
    Private Sub Centertabpage3_B_check()
        ' 計算 GroupBox 在 X 軸上的置中位置
        Dim x As Integer = (Me.ClientSize.Width - tabpage3_B_check.Width) / 2

        ' 計算 GroupBox 在 Y 軸上的置中位置
        Dim y As Integer = (Me.ClientSize.Height - tabpage3_B_check.Height) / 2

        ' 設定 GroupBox 的 Location 屬性，使其置中
        tabpage3_B_check.Location = New Point(x, y)
        tabpage3_L_choose.Location = New Point(x - 330, y + 50)
        tabpage3_P_pikachu.Location = New Point(x - 325, y + 100)
        tabpage3_P_Charmander.Location = New Point(x - 125, y + 100)
        tabpage3_P_Bulbasaur.Location = New Point(x + 275, y + 100)
        tabpage3_P_Squirtle.Location = New Point(x + 75, y + 100)
    End Sub
    Dim playerindex As Integer = 0
    Dim Playerchoose As Boolean = True
    Dim final_Playerindex As Integer = 0
    Private Sub Tabpage3_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        GetAsyncKeyState(e.KeyCode)
        If (tabpage3_P_background.Image IsNot Nothing) Then
            tabpage3_P_background.Image.Dispose()
        End If
        If TabControl1.SelectedIndex = 2 Then
            If (e.KeyCode = Keys.Escape) Then
                TabControl1.SelectedIndex = 9
                currentTabPageIndex = 2
            End If

            If e.KeyValue = Keys.D And Playerchoose Then
                playerindex = (playerindex + 1) Mod 4
            End If
            If e.KeyValue = Keys.A And Playerchoose Then
                playerindex -= 1
                If playerindex = -1 Then
                    playerindex = 3
                End If
            End If
            Select Case playerindex
                Case 0
                    tabpage3_L_choose.Location = New Point(tabpage3_P_pikachu.Left + tabpage3_P_pikachu.Width / 2 - tabpage3_L_choose.Width / 2, tabpage3_L_choose.Top)
                    tabpage3_P_background.Image = My.Resources.皮卡丘1
                    tabpage3_P_background.SendToBack()
                    tabpage3_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
                Case 1
                    tabpage3_L_choose.Location = New Point(tabpage3_P_Charmander.Left + tabpage3_P_Charmander.Width / 2 - tabpage3_L_choose.Width / 2, tabpage3_L_choose.Top)
                    tabpage3_P_background.Image = My.Resources.小火龍1
                    tabpage3_P_background.SendToBack()
                    tabpage3_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
                Case 2
                    tabpage3_L_choose.Location = New Point(tabpage3_P_Squirtle.Left + tabpage3_P_Squirtle.Width / 2 - tabpage3_L_choose.Width / 2, tabpage3_L_choose.Top)
                    tabpage3_P_background.Image = My.Resources.傑尼龜1
                    tabpage3_P_background.SendToBack()
                    tabpage3_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
                Case 3
                    tabpage3_L_choose.Location = New Point(tabpage3_P_Bulbasaur.Left + tabpage3_P_Bulbasaur.Width / 2 - tabpage3_L_choose.Width / 2, tabpage3_L_choose.Top)
                    tabpage3_P_background.Image = My.Resources.妙蛙種子1
                    tabpage3_P_background.SendToBack()
                    tabpage3_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
            End Select

            If e.KeyCode = Keys.J Then
                ' 最終選擇Player1
                final_Playerindex = playerindex
                ' 鎖定圖像
                LockedImage = GetPlayerImage(final_Playerindex)
                Playerchoose = False
                tabpage3_L_second.Text = 10
                onlyoneplayer.Start()
            End If
            If Playerchoose = False Then
                tabpage3_B_check.Visible = True
                tabpage3_B_check.Enabled = True
                If e.KeyCode = Keys.Enter Then
                    tabpage3_B_check.PerformClick()
                    LocalGameStart = 1
                End If
            End If
            GetPlayerImage(final_Playerindex)
        End If
    End Sub
#End Region
#Region "單人結束畫面"
    Private Sub Centertabpage10_L_points()
        Dim x As Integer = (Me.ClientSize.Width - tabpage4_L_points.Width) / 2
        ' 計算 GroupBox 在 Y 軸上的置中位置
        Dim y As Integer = (Me.ClientSize.Height - tabpage4_L_points.Height) / 2
        tabpage4_L_gameover.Location = New Point(x + 125, y - 120)
        tabpage4_L_points.Location = New Point(x, y)
        tabpage4_B_again.Location = New Point(x + 18, y + 120)
        tabpage4_B_home.Location = New Point(x + 188, y + 120)
        Tabpage4_P_backgroundImage.SendToBack()
        Tabpage4_P_backgroundImage.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
        tabpage4_L_gameover.Parent = Tabpage4_P_backgroundImage
        tabpage4_L_points.Parent = Tabpage4_P_backgroundImage
        tabpage4_B_again.Parent = Tabpage4_P_backgroundImage
        tabpage4_B_home.Parent = Tabpage4_P_backgroundImage
    End Sub
    Private Sub Tabpage4_B_again_Click(sender As Object, e As EventArgs) Handles tabpage4_B_again.Click
        TabControl1.SelectedIndex = 2
        tabpage3_P_background.Image = My.Resources.皮卡丘1
        tabpage3_P_background.SendToBack()
        tabpage3_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
        playerindex = 0
        Playerchoose = True
        tabpage3_B_check.Visible = False
        tabpage3_B_check.Enabled = False

    End Sub
    Private Sub Tabpage10_B_home_Click(sender As Object, e As EventArgs) Handles tabpage4_B_home.Click
        TabControl1.SelectedIndex = 0
        MediaPlayer.settings.setMode("loop", True)
        MediaPlayer.controls.play()
    End Sub
#End Region
#Region "選角畫面多人"
    Private Sub Tabpage5_B_check_Click(sender As Object, e As EventArgs) Handles tabpage5_B_check.Click
        LocalGameStart = 1
        LocalGameForm = New LocalGame(2, final_Player1index, final_Player2index)
        LocalGameForm.StartPosition = FormStartPosition.Manual
        LocalGameForm.Location = New Point(Me.Location.X, Me.Location.Y)
        LocalGameForm.Show()
        Me.Hide()
    End Sub
    Private Sub Centertabpage5_B_check()

        ' 計算 GroupBox 在 X 軸上的置中位置
        Dim x As Integer = (Me.ClientSize.Width - tabpage5_B_check.Width) / 2

        ' 計算 GroupBox 在 Y 軸上的置中位置
        Dim y As Integer = (Me.ClientSize.Height - tabpage5_B_check.Height) / 2

        ' 設定 GroupBox 的 Location 屬性，使其置中
        tabpage5_B_check.Location = New Point(x, y)
        tabpage5_P_pikachu.Location = New Point(x - 350, y + 100)
        tabpage5_P_Charmander.Location = New Point(x - 175, y + 100)
        tabpage5_P_Bulbasaur.Location = New Point(x + 350, y + 100)
        tabpage5_P_Squirtle.Location = New Point(x + 175, y + 100)
        tabpage5_L_play1C.Location = New Point(x - 330, y + 50)
    End Sub

    Dim Player1index As Integer = 0
    Dim Player2index As Integer = 0
    Dim Player1choose As Boolean = True
    Dim Player2choose As Boolean = True
    Dim final_Player1index As Integer = 0
    Dim final_Player2index As Integer = 0

#Region "選角畫面"
    Private Sub Tabpage5_keydown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        GetAsyncKeyState(e.KeyCode)
        If TabControl1.SelectedIndex = 4 Then
            tabpage5_B_check.Enabled = False
            If e.KeyCode = Keys.D And Player2choose Then
                Player2index = (Player2index + 1) Mod 4
            End If
            If e.KeyCode = Keys.A And Player2choose Then
                Player2index -= 1
                If Player2index = -1 Then
                    Player2index = 3
                End If
            End If
            If e.KeyValue = Keys.Right And Player1choose Then
                Player1index = (Player1index + 1) Mod 4
            End If
            If e.KeyValue = Keys.Left And Player1choose Then
                Player1index -= 1
                If Player1index = -1 Then
                    Player1index = 3
                End If
            End If
            Select Case Player2index
                Case 0
                    tabpage5_L_play1C.Location = New Point(tabpage5_P_pikachu.Left + tabpage5_P_pikachu.Width / 2 - tabpage5_L_play1C.Width / 2, tabpage5_L_play1C.Top)
                    tabpage5_P_background1.Image = My.Resources.皮卡丘1
                    tabpage5_P_background1.SendToBack()
                    tabpage5_P_background1.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)

                Case 1
                    tabpage5_L_play1C.Location = New Point(tabpage5_P_Charmander.Left + tabpage5_P_Charmander.Width / 2 - tabpage5_L_play1C.Width / 2, tabpage5_L_play1C.Top)
                    tabpage5_P_background1.Image = My.Resources.小火龍1
                    tabpage5_P_background1.SendToBack()
                    tabpage5_P_background1.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
                Case 3
                    tabpage5_L_play1C.Location = New Point(tabpage5_P_Bulbasaur.Left + tabpage5_P_Bulbasaur.Width / 2 - tabpage5_L_play1C.Width / 2, tabpage5_L_play1C.Top)
                    tabpage5_P_background1.Image = My.Resources.妙蛙種子1
                    tabpage5_P_background1.SendToBack()
                    tabpage5_P_background1.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
                Case 2
                    tabpage5_L_play1C.Location = New Point(tabpage5_P_Squirtle.Left + tabpage5_P_Squirtle.Width / 2 - tabpage5_L_play1C.Width / 2, tabpage5_L_play1C.Top)
                    tabpage5_P_background1.Image = My.Resources.傑尼龜1
                    tabpage5_P_background1.SendToBack()
                    tabpage5_P_background1.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
            End Select

            Select Case Player1index
                Case 0
                    tabpage5_L_play2C.Location = New Point(tabpage5_P_pikachu.Left + tabpage5_P_pikachu.Width / 2 - tabpage5_L_play2C.Width / 2, tabpage5_L_play1C.Top)
                    tabpage5_P_background2.Image = My.Resources.皮卡丘1
                    tabpage5_P_background2.SendToBack()
                    tabpage5_P_background2.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
                Case 1
                    tabpage5_L_play2C.Location = New Point(tabpage5_P_Charmander.Left + tabpage5_P_Charmander.Width / 2 - tabpage5_L_play2C.Width / 2, tabpage5_L_play1C.Top)
                    tabpage5_P_background2.Image = My.Resources.小火龍1
                    tabpage5_P_background2.SendToBack()
                    tabpage5_P_background2.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
                Case 3
                    tabpage5_L_play2C.Location = New Point(tabpage5_P_Bulbasaur.Left + tabpage5_P_Bulbasaur.Width / 2 - tabpage5_L_play2C.Width / 2, tabpage5_L_play1C.Top)
                    tabpage5_P_background2.Image = My.Resources.妙蛙種子1
                    tabpage5_P_background2.SendToBack()
                    tabpage5_P_background2.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
                Case 2
                    tabpage5_L_play2C.Location = New Point(tabpage5_P_Squirtle.Left + tabpage5_P_Squirtle.Width / 2 - tabpage5_L_play2C.Width / 2, tabpage5_L_play1C.Top)
                    tabpage5_P_background2.Image = My.Resources.傑尼龜1
                    tabpage5_P_background2.SendToBack()
                    tabpage5_P_background2.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
            End Select
            If Player1index = Player2index Then
                tabpage5_L_play2C.Location = New Point(tabpage5_L_play1C.Location.X, tabpage5_L_play1C.Location.Y - tabpage5_L_play1C.Height - 10)
            Else
                tabpage5_L_play2C.Location = New Point(tabpage5_L_play2C.Location.X, tabpage5_L_play1C.Location.Y)
            End If
            If e.KeyCode = Keys.J Then
                ' Player1最後選擇
                final_Player2index = Player2index
                '鎖定角色
                LockedImage = GetPlayerImage(final_Player1index)
                Player2choose = False
            End If

            If e.KeyValue = 97 Then
                ' Player2最後選擇
                final_Player1index = Player1index
                '鎖定角色
                LockedImage = GetPlayerImage(final_Player2index)
                Player1choose = False
            End If

            If Player1choose = False And Player2choose = False Then
                tabpage5_B_check.Visible = True
                tabpage5_B_check.Enabled = True
                If e.KeyCode = Keys.Enter Then
                    tabpage5_B_check.PerformClick()
                End If
            End If

            GetPlayerImage(final_Player1index)
            GetPlayerImage(final_Player2index)

        End If

    End Sub
#Region "鎖角畫面"

    Private LockedImage As Image = Nothing
    Private Function GetPlayerImage(index As Integer) As Image

        Select Case index
            Case 0
                Return My.Resources.皮卡丘1
            Case 1
                Return My.Resources.小火龍1
            Case 2
                Return My.Resources.妙蛙種子1
            Case 3
                Return My.Resources.傑尼龜1
            Case Else
                Return Nothing
        End Select
    End Function


#End Region
#End Region
#End Region
#Region "遊戲畫面"
    Private Sub Centertabpage2_P_flag()
        ' 計算 GroupBox 在 X 軸上的置中位置
        Dim x As Integer = (Me.ClientSize.Width - tabpage7_P_flag.Width) / 2

        ' 計算 GroupBox 在 Y 軸上的置中位置
        Dim y As Integer = (Me.ClientSize.Height - tabpage7_P_flag.Height) / 2

        ' 設定 GroupBox 的 Location 屬性，使其置中
        tabpage7_P_flag.Location = New Point(x, 2 * y + 120)
        tabpage7_P_ball.Location = New Point(x, y - 100)
        tabpage7_P_player1.Location = New Point(x - 150, y)
        tabpage7_P_player2.Location = New Point(x + 150, y)
        Button2.Location = New Point(x, y)
        Button2.Visible = False
        tabpage2_B_skill2.Location = New Point(1160, 350)
        tabpage7_L_score1.Location = New Point(x - 200, 0.5 * y - 50)
        tabpage7_L_score2.Location = New Point(x + 200, 0.5 * y - 50)


    End Sub
    Private Sub Gameover_Clik(sender As Object, e As EventArgs)
        If tabpage7_L_score1.Text = 7 Or tabpage7_L_score2.Text = 7 Then
            TabControl1.SelectedIndex = 6
        End If
    End Sub

#End Region
#Region "多人結束畫面"
    Private Sub Centertabpage7_L_points()
        Dim x As Integer = (Me.ClientSize.Width - tabpage7_L_win.Width) / 2
        ' 計算 GroupBox 在 Y 軸上的置中位置
        Dim y As Integer = (Me.ClientSize.Height - tabpage7_L_win.Height) / 2
        tabpage7_L_gameover.Location = New Point(x - 50, y - 120)
        tabpage7_L_win.Location = New Point(x - 15, y)
        tabpage7_B_again.Location = New Point(x - 50, y + 120)
        tabpage7_B_home.Location = New Point(x + 100, y + 120)
        tabpage7_P_backgroundImage.SendToBack()
        tabpage7_P_backgroundImage.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
        tabpage7_L_gameover.Parent = tabpage7_P_backgroundImage
        tabpage7_L_win.Parent = tabpage7_P_backgroundImage
        tabpage7_B_again.Parent = tabpage7_P_backgroundImage
        tabpage7_B_home.Parent = tabpage7_P_backgroundImage

    End Sub
    Private Sub Tabpage5_B_again_Click(sender As Object, e As EventArgs) Handles tabpage7_B_again.Click
        TabControl1.SelectedIndex = 4
        tabpage5_P_background1.Image = My.Resources.皮卡丘1
        tabpage5_P_background1.SendToBack()
        tabpage5_P_background1.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
        tabpage5_P_background2.Location = New Point(tabpage5_P_background1.Width, 0)
        tabpage5_P_background2.Image = My.Resources.皮卡丘1
        tabpage5_P_background2.SendToBack()
        tabpage5_P_background2.Size = New Size(Me.ClientSize.Width / 2, Me.ClientSize.Height)
        Player1index = 0
        Player2index = 0
        tabpage5_L_play2C.Location = New Point(tabpage5_L_play1C.Location.X, tabpage5_L_play1C.Location.Y)
        tabpage5_L_play2C.Location = New Point(tabpage5_L_play1C.Location.X, tabpage5_L_play1C.Location.Y - tabpage5_L_play1C.Height - 10)
        Player1choose = True
        Player2choose = True
        tabpage5_B_check.Visible = False
        tabpage5_B_check.Enabled = False

    End Sub
    Private Sub Tabpage7_B_home_Click(sender As Object, e As EventArgs) Handles tabpage7_B_home.Click
        TabControl1.SelectedIndex = 0
        MediaPlayer.settings.setMode("loop", True)
        MediaPlayer.controls.play()
    End Sub
#End Region
#Region "選角畫面連線"
    Private Sub Connect_Server()
        Dim IP As String = tabpage11_T_id.Text
        Dim Port As String = tabpage11_T_port.Text
        client = New TcpClient(IP, Port)
        stream = client.GetStream()
    End Sub
    Private Sub tabpage8_B_check_Click(sender As Object, e As EventArgs) Handles tabpage8_B_check.Click
        Dim Player As String = If(tabpage11_C_player1.Checked, "player1", If(tabpage11_C_player2.Checked, "player2", "-1"))
        Dim Character As String = final_Playerindex
        Try
            SendCommand(Player, Character)
            tabpage8_B_check.Enabled = False
        Catch ex As Exception
            Console.WriteLine("Error: " & ex.Message)
        End Try
    End Sub
    Private Async Sub SendCommand(playerId As String, command As String)
        Dim playerCommand = $"{playerId},{command},"
        Dim commandBytes = Encoding.UTF8.GetBytes(playerCommand)
        Await stream.WriteAsync(commandBytes, 0, commandBytes.Length)
    End Sub

    Private Async Sub ListenForUpdates()
        While client IsNot Nothing AndAlso client.Connected
            Try
                Dim buffer(5196) As Byte
                Dim bytesRead = Await stream.ReadAsync(buffer, 0, buffer.Length)
                If bytesRead > 0 Then
                    Dim response = Encoding.UTF8.GetString(buffer, 0, bytesRead).TrimEnd(Chr(0))
                    Console.WriteLine(response)
                    If (OnlineGameStart = 1) Then
                        OnlineGameForm.HandleGameData(response)
                    Else
                        HandleResponse(response)
                    End If
                End If
            Catch ex As Exception
                Console.WriteLine("Error: " & ex.Message)
                Exit While
            End Try
        End While
    End Sub
    Dim player1Character As Integer = -1
    Dim player2Character As Integer = -1
    Private Sub HandleResponse(response As String)
        Dim serverData = response.Split(","c)
        If Not Integer.TryParse(serverData(0), player1Character) Then
            Exit Sub
        End If
        If Not Integer.TryParse(serverData(1), player2Character) Then
            Exit Sub
        End If
        Dim player As String = If(tabpage11_C_player1.Checked, "player1", If(tabpage11_C_player2.Checked, "player2", "-1"))
        If (player1Character <> -1 And player2Character <> -1) Then
            OnlineGameForm = New OnlineGame(player, player1Character, player2Character) With {
                .StartPosition = FormStartPosition.Manual,
                .Location = New Point(Me.ClientRectangle.Location.X, Me.ClientRectangle.Location.Y)
            }
            OnlineGameForm.Show()
            Me.Hide()
            OnlineGameStart = 1
            OnlineGameChoosing = False
        End If

    End Sub
    Private Sub Centertabpage8_B_check()

        Dim x As Integer = (Me.ClientSize.Width - tabpage8_B_check.Width) / 2

        Dim y As Integer = (Me.ClientSize.Height - tabpage8_B_check.Height) / 2

        tabpage8_B_check.Location = New Point(x, y)
        tabpage8_P_pikachu.Location = New Point(x - 325, y + 100)
        tabpage8_P_Charmander.Location = New Point(x - 125, y + 100)
        tabpage8_P_Bulbasaur.Location = New Point(x + 275, y + 100)
        tabpage8_P_Squirtle.Location = New Point(x + 75, y + 100)
    End Sub

    Private Sub CloseClient()
        If (stream IsNot Nothing) Then
            Try
                stream.Close()
            Catch ex As Exception
                Console.WriteLine("無法關閉stream: " & ex.Message)
            Finally
                stream = Nothing
            End Try
        End If
        If (client IsNot Nothing) Then
            Try
                client.Close()
            Catch ex As Exception
                Console.WriteLine("無法關閉client: " & ex.Message)
            Finally
                client = Nothing
            End Try
        End If
    End Sub

    Private Sub tabpage8_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        GetAsyncKeyState(e.KeyCode)
        If (tabpage8_P_background.Image IsNot Nothing) Then
            tabpage8_P_background.Image.Dispose()
        End If
        If TabControl1.SelectedIndex = 8 Then
            If (e.KeyCode = Keys.Escape) Then
                TabControl1.SelectedIndex = 9
                currentTabPageIndex = 2
            End If
            If e.KeyValue = Keys.D And Playerchoose Then
                playerindex = (playerindex + 1) Mod 4
            End If
            If e.KeyValue = Keys.A And Playerchoose Then
                playerindex -= 1
                If playerindex = -1 Then
                    playerindex = 3
                End If
            End If
            Select Case playerindex
                Case 0
                    tabpage8_P_background.Image = My.Resources.皮卡丘1
                    tabpage8_P_background.SendToBack()
                    tabpage8_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
                Case 1
                    tabpage8_P_background.Image = My.Resources.小火龍1
                    tabpage8_P_background.SendToBack()
                    tabpage8_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
                Case 2
                    tabpage8_P_background.Image = My.Resources.傑尼龜1
                    tabpage8_P_background.SendToBack()
                    tabpage8_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
                Case 3
                    tabpage8_P_background.Image = My.Resources.妙蛙種子1
                    tabpage8_P_background.SendToBack()
                    tabpage8_P_background.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
            End Select

            If e.KeyCode = Keys.J Then
                ' 最終選擇Player1
                final_Playerindex = playerindex
                ' 鎖定圖像
                LockedImage = GetPlayerImage(final_Playerindex)
                Playerchoose = False
                onlyoneplayer.Start()
            End If
            If Playerchoose = False Then
                tabpage8_B_check.Visible = True
                tabpage8_B_check.Enabled = True
                If e.KeyCode = Keys.Enter Then
                    tabpage8_B_check.PerformClick()

                End If
            End If
            GetPlayerImage(final_Playerindex)
        End If
    End Sub

#End Region
#Region "ESC"
    Private escapePressed As Boolean = False
    Private currentTabPageIndex As Integer = 0

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            TabControl1.SelectedIndex = 9
            escapePressed = True
            MediaPlayer.controls.stop()
        End If
    End Sub

    Private Sub Tabpage10_B_keepplay_Click(sender As Object, e As EventArgs) Handles tabpage10_B_keepplay.Click
        If (LocalGameForm IsNot Nothing) Then
            If (LocalGameStart = 1 And LocalGameForm._gameMode = 1) Then
                Me.Hide()
                LocalGameForm.Show()
                LocalGameForm.BallTimer.Start()
                LocalGameForm.PracticeTimer.Start()
                Exit Sub
            ElseIf (LocalGameStart = 1 And LocalGameForm._gameMode = 2) Then
                Me.Hide()
                LocalGameForm.Show()
                LocalGameForm.BallTimer.Start()
                Exit Sub
            End If
        ElseIf (OnlineGameForm IsNot Nothing) Then
            If (OnlineGameStart = 1) Then
                Me.Hide()
                OnlineGameForm.Show()
                Exit Sub
            End If
        End If
        TabControl1.SelectedIndex = currentTabPageIndex

        If TabControl1.SelectedIndex = 0 Then
            MediaPlayer.settings.setMode("loop", True)
            MediaPlayer.controls.play()
        End If
    End Sub
    Private Sub Tabpage10_B_set_Click(sender As Object, e As EventArgs) Handles tabpage10_B_set.Click
        If (OnlineGameStart = 1) Then
            Exit Sub
        End If
        TabControl1.SelectedIndex = 10
        MediaPlayer.controls.stop()
    End Sub
    Private Sub Tabpage10_B_leave_Click(sender As Object, e As EventArgs) Handles tabpage10_B_leave.Click
        If (LocalGameStart = 1) Then
            LocalGameForm.Close()
            LocalGameForm.Dispose()
            LocalGameStart = 0
        ElseIf (OnlineGameStart = 1) Then
            OnlineGameForm.Close()
            OnlineGameForm.Dispose()
            CloseClient()
            OnlineGameStart = 0
        End If
        If (stream IsNot Nothing) Then
            stream.Close()
            stream.Dispose()
        End If
        If (client IsNot Nothing) Then
            client.Close()
            client.Dispose()
        End If
        If (OnlineGameChoosing = True) Then
            tabpage8_B_check.Enabled = False
            tabpage8_B_check.Visible = False
        End If

        TabControl1.SelectedIndex = 0
        'escapePressed = False
        MediaPlayer.settings.setMode("loop", True)
        MediaPlayer.controls.play()
    End Sub
    Private Sub Centertabpage10_B_set()
        ' 計算 GroupBox 在 X 軸上的置中位置
        Dim x As Integer = (Me.ClientSize.Width - tabpage10_B_set.Width) / 2

        ' 計算 GroupBox 在 Y 軸上的置中位置
        Dim y As Integer = (Me.ClientSize.Height - tabpage10_B_set.Height) / 2

        ' 設定 GroupBox 的 Location 屬性，使其置中
        tabpage10_B_keepplay.Location = New Point(x, y - 70)
        tabpage10_B_set.Location = New Point(x, y)
        tabpage10_B_leave.Location = New Point(x, y + 70)
        tabpage10_B_keepplay.Parent = tabpage10_p_background
        tabpage10_B_set.Parent = tabpage10_p_background
        tabpage10_B_leave.Parent = tabpage10_p_background
    End Sub

#End Region
#Region "設定"

    Private Sub Tabpage11_Trackbar_sound_Scroll(sender As Object, e As EventArgs) Handles tabpage11_Trackbar_sound.Scroll
        ' 您的 TrackBar 控制項的值範圍
        Dim minValue As Integer = 0 ' 假設最小值是0
        Dim maxValue As Integer = 100 ' 假設最大值是100

        ' 獲取 TrackBar 控制項的值
        Dim trackBarValue As Integer = tabpage11_Trackbar_sound.Value

        ' 將 TrackBar 控制項的值映射到 Windows Media Player 的音量範圍（0 到 100）
        Dim volume As Integer = CInt((trackBarValue - minValue) / (maxValue - minValue) * 100)

        ' 設置 Windows Media Player 的音量
        MediaPlayer.settings.volume = volume
    End Sub
    Private Sub Centertabpage11()
        ' 計算 GroupBox 在 X 軸上的置中位置
        Dim x As Integer = (Me.ClientSize.Width) / 2

        ' 計算 GroupBox 在 Y 軸上的置中位置
        Dim y As Integer = (Me.ClientSize.Height) / 2

        ' 設定 GroupBox 的 Location 屬性，使其置中
        tabpage11_L_id.Location = New Point(x - 500, y - 220)
        tabpage11_T_id.Location = New Point(x - 500, y - 150)
        tabpage11_L_port.Location = New Point(x - 200, y - 220)
        tabpage11_T_port.Location = New Point(x - 200, y - 150)
        tabpage11_L_sound.Location = New Point(x + 100, y + 50)
        tabpage11_Trackbar_sound.Location = New Point(x + 100, y + 120)
        tabpage11_L_player.Location = New Point(x + 100, y - 220)
        tabpage11_C_player1.Location = New Point(x + 100, y - 140)
        tabpage11_C_player2.Location = New Point(x + 270, y - 140)
        tabpage11_L_keys.Location = New Point(x - 500, y + 50)
        tabpage11_B_look.Location = New Point(x - 500, y + 120)

        tabpage11_L_id.Parent = tabpage11_P_background
        tabpage11_L_port.Parent = tabpage11_P_background
        tabpage11_L_sound.Parent = tabpage11_P_background
        tabpage11_L_player.Parent = tabpage11_P_background
        tabpage11_T_id.Parent = tabpage11_P_background
        tabpage11_Trackbar_sound.Parent = tabpage11_P_background
        tabpage11_L_keys.Parent = tabpage11_P_background
        tabpage11_B_look.Parent = tabpage11_P_background
        tabpage11_C_player1.Parent = tabpage11_P_background
        tabpage11_C_player2.Parent = tabpage11_P_background
    End Sub
    Private Sub Tabpage11_B_look_Click(sender As Object, e As EventArgs) Handles tabpage11_B_look.Click
        TabControl1.SelectedIndex = 11
    End Sub
    Private Sub Tabpage11_keys(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If TabControl1.SelectedIndex = 11 Then
            If e.KeyCode <> Keys.None Then
                TabControl1.SelectedIndex = 10
            End If
        End If
    End Sub
    Private Sub GoBack1_Click(sender As Object, e As EventArgs) Handles GoBack1.Click
        If (LocalGameForm IsNot Nothing) Then
            If (LocalGameStart = 1 And LocalGameForm._gameMode = 1) Then
                Me.Hide()
                LocalGameForm.Show()
                LocalGameForm.BallTimer.Start()
                LocalGameForm.PracticeTimer.Start()
            ElseIf (LocalGameStart = 1 And LocalGameForm._gameMode = 2) Then
                Me.Hide()
                LocalGameForm.Show()
                LocalGameForm.BallTimer.Start()
            End If
        End If
        If (currentTabPageIndex = 0) Then
            MediaPlayer.settings.setMode("loop", True)
            MediaPlayer.controls.play()
        End If
        TabControl1.SelectedIndex = currentTabPageIndex
    End Sub
#End Region
#Region "多人連線結束畫面"
    Private Sub Centertabpage13_L_points()
        Dim x As Integer = (Me.ClientSize.Width - tabpage13_L_win.Width) / 2
        ' 計算 GroupBox 在 Y 軸上的置中位置
        Dim y As Integer = (Me.ClientSize.Height - tabpage13_L_win.Height) / 2
        tabpage13_L_gameover.Location = New Point(x - 50, y - 120)
        tabpage13_L_win.Location = New Point(x - 15, y)
        tabpage13_B_home.Location = New Point(x - 35, y + 120)
        PictureBox1.SendToBack()
        PictureBox1.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height)
        tabpage13_L_gameover.Parent = PictureBox1
        tabpage13_L_win.Parent = PictureBox1
        tabpage13_B_home.Parent = PictureBox1
    End Sub
    Private Sub Tabpage13_B_home_Click(sender As Object, e As EventArgs) Handles tabpage13_B_home.Click
        CloseClient()
        MediaPlayer.settings.setMode("loop", True)
        MediaPlayer.controls.play()
        OnlineGameStart = 0
        TabControl1.SelectedIndex = 0
        tabpage8_B_check.Visible = False
        tabpage8_B_check.Enabled = False
        MediaPlayer.controls.play()
    End Sub
#End Region
#Region "formclosing"
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If MediaPlayer IsNot Nothing Then
            MediaPlayer.controls.stop()
            MediaPlayer.close()
            MediaPlayer = Nothing
        End If
        If (OnlineGameForm IsNot Nothing) Then
            OnlineGameForm.Close()
            OnlineGameForm.Dispose()
        End If
        If (LocalGameForm IsNot Nothing) Then
            LocalGameForm.Close()
            LocalGameForm.Dispose()
        End If
        If (stream IsNot Nothing) Then
            stream.Close()
            stream.Dispose()
        End If
        If (client IsNot Nothing) Then
            client.Close()
            client.Dispose()
        End If
    End Sub










#End Region

End Class
