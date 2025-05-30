<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OnlineGame
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    '為 Windows Form 設計工具的必要項
    Private components As System.ComponentModel.IContainer

    '注意: 以下為 Windows Form 設計工具所需的程序
    '可以使用 Windows Form 設計工具進行修改。
    '請勿使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.player1HitImage = New System.Windows.Forms.PictureBox()
        Me.player2HitImage = New System.Windows.Forms.PictureBox()
        Me.player1SkillImage = New System.Windows.Forms.PictureBox()
        Me.player2SkillImage = New System.Windows.Forms.PictureBox()
        Me.player1ScoreText = New System.Windows.Forms.Label()
        Me.player2ScoreText = New System.Windows.Forms.Label()
        CType(Me.player1HitImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.player2HitImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.player1SkillImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.player2SkillImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'player1HitImage
        '
        Me.player1HitImage.BackColor = System.Drawing.Color.Transparent
        Me.player1HitImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.player1HitImage.Location = New System.Drawing.Point(1458, 45)
        Me.player1HitImage.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.player1HitImage.Name = "player1HitImage"
        Me.player1HitImage.Size = New System.Drawing.Size(117, 111)
        Me.player1HitImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.player1HitImage.TabIndex = 19
        Me.player1HitImage.TabStop = False
        '
        'player2HitImage
        '
        Me.player2HitImage.BackColor = System.Drawing.Color.Transparent
        Me.player2HitImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.player2HitImage.Location = New System.Drawing.Point(44, 45)
        Me.player2HitImage.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.player2HitImage.Name = "player2HitImage"
        Me.player2HitImage.Size = New System.Drawing.Size(117, 111)
        Me.player2HitImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.player2HitImage.TabIndex = 18
        Me.player2HitImage.TabStop = False
        '
        'player1SkillImage
        '
        Me.player1SkillImage.BackColor = System.Drawing.Color.Transparent
        Me.player1SkillImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.player1SkillImage.Location = New System.Drawing.Point(1304, 45)
        Me.player1SkillImage.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.player1SkillImage.Name = "player1SkillImage"
        Me.player1SkillImage.Size = New System.Drawing.Size(117, 111)
        Me.player1SkillImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.player1SkillImage.TabIndex = 17
        Me.player1SkillImage.TabStop = False
        '
        'player2SkillImage
        '
        Me.player2SkillImage.BackColor = System.Drawing.Color.Transparent
        Me.player2SkillImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.player2SkillImage.Location = New System.Drawing.Point(192, 45)
        Me.player2SkillImage.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.player2SkillImage.Name = "player2SkillImage"
        Me.player2SkillImage.Size = New System.Drawing.Size(117, 111)
        Me.player2SkillImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.player2SkillImage.TabIndex = 16
        Me.player2SkillImage.TabStop = False
        '
        'player1ScoreText
        '
        Me.player1ScoreText.AutoSize = True
        Me.player1ScoreText.BackColor = System.Drawing.Color.Transparent
        Me.player1ScoreText.Font = New System.Drawing.Font("新細明體", 72.0!, System.Drawing.FontStyle.Bold)
        Me.player1ScoreText.Location = New System.Drawing.Point(1102, 45)
        Me.player1ScoreText.Name = "player1ScoreText"
        Me.player1ScoreText.Size = New System.Drawing.Size(161, 138)
        Me.player1ScoreText.TabIndex = 15
        Me.player1ScoreText.Text = "00"
        Me.player1ScoreText.UseCompatibleTextRendering = True
        '
        'player2ScoreText
        '
        Me.player2ScoreText.AutoSize = True
        Me.player2ScoreText.BackColor = System.Drawing.Color.Transparent
        Me.player2ScoreText.Font = New System.Drawing.Font("新細明體", 72.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.player2ScoreText.Location = New System.Drawing.Point(363, 45)
        Me.player2ScoreText.Name = "player2ScoreText"
        Me.player2ScoreText.Size = New System.Drawing.Size(161, 138)
        Me.player2ScoreText.TabIndex = 14
        Me.player2ScoreText.Text = "00"
        Me.player2ScoreText.UseCompatibleTextRendering = True
        '
        'OnlineGame
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1735, 868)
        Me.Controls.Add(Me.player1HitImage)
        Me.Controls.Add(Me.player2HitImage)
        Me.Controls.Add(Me.player1SkillImage)
        Me.Controls.Add(Me.player2SkillImage)
        Me.Controls.Add(Me.player1ScoreText)
        Me.Controls.Add(Me.player2ScoreText)
        Me.Name = "OnlineGame"
        Me.Text = "打球嗎?寶!"
        CType(Me.player1HitImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.player2HitImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.player1SkillImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.player2SkillImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents player1HitImage As PictureBox
    Friend WithEvents player2HitImage As PictureBox
    Friend WithEvents player1SkillImage As PictureBox
    Friend WithEvents player2SkillImage As PictureBox
    Friend WithEvents player1ScoreText As Label
    Friend WithEvents player2ScoreText As Label
End Class
