<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.start_server = New System.Windows.Forms.Button()
        Me.log_text = New System.Windows.Forms.TextBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.restart_server = New System.Windows.Forms.Button()
        Me.port_input = New System.Windows.Forms.TextBox()
        Me.port_text = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'start_server
        '
        Me.start_server.Location = New System.Drawing.Point(579, 89)
        Me.start_server.Name = "start_server"
        Me.start_server.Size = New System.Drawing.Size(138, 83)
        Me.start_server.TabIndex = 0
        Me.start_server.Text = "開始伺服器"
        Me.start_server.UseVisualStyleBackColor = True
        '
        'log_text
        '
        Me.log_text.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.log_text.Location = New System.Drawing.Point(28, 89)
        Me.log_text.Multiline = True
        Me.log_text.Name = "log_text"
        Me.log_text.ReadOnly = True
        Me.log_text.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.log_text.Size = New System.Drawing.Size(521, 312)
        Me.log_text.TabIndex = 1
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Font = New System.Drawing.Font("新細明體", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.label1.Location = New System.Drawing.Point(205, 47)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(163, 30)
        Me.label1.TabIndex = 2
        Me.label1.Text = "伺服器紀錄"
        '
        'restart_server
        '
        Me.restart_server.Enabled = False
        Me.restart_server.Location = New System.Drawing.Point(579, 318)
        Me.restart_server.Name = "restart_server"
        Me.restart_server.Size = New System.Drawing.Size(138, 83)
        Me.restart_server.TabIndex = 3
        Me.restart_server.Text = "重啟伺服器"
        Me.restart_server.UseVisualStyleBackColor = True
        '
        'port_input
        '
        Me.port_input.Location = New System.Drawing.Point(579, 239)
        Me.port_input.Name = "port_input"
        Me.port_input.Size = New System.Drawing.Size(138, 25)
        Me.port_input.TabIndex = 4
        '
        'port_text
        '
        Me.port_text.AutoSize = True
        Me.port_text.Location = New System.Drawing.Point(630, 212)
        Me.port_text.Name = "port_text"
        Me.port_text.Size = New System.Drawing.Size(43, 15)
        Me.port_text.TabIndex = 5
        Me.port_text.Text = "PORT"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(826, 533)
        Me.Controls.Add(Me.port_text)
        Me.Controls.Add(Me.port_input)
        Me.Controls.Add(Me.restart_server)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.log_text)
        Me.Controls.Add(Me.start_server)
        Me.Name = "Form1"
        Me.Text = "打球嗎?寶_伺服器"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents start_server As Button
    Friend WithEvents log_text As TextBox
    Friend WithEvents label1 As Label
    Friend WithEvents restart_server As Button
    Friend WithEvents port_input As TextBox
    Friend WithEvents port_text As Label
End Class
