<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class wndOkno
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtPort = New System.Windows.Forms.TextBox()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.lblStan = New System.Windows.Forms.Label()
        Me.pnlStan = New System.Windows.Forms.Panel()
        Me.ntfIkona = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.ctxKontrolaTlo = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuOtworzOkno = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuZamknijProgram = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuStan = New System.Windows.Forms.ToolStripMenuItem()
        Me.ctxKontrolaTlo.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 37)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(29, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Port:"
        '
        'txtPort
        '
        Me.txtPort.Location = New System.Drawing.Point(44, 34)
        Me.txtPort.Name = "txtPort"
        Me.txtPort.Size = New System.Drawing.Size(144, 20)
        Me.txtPort.TabIndex = 1
        Me.txtPort.Text = "443"
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(12, 60)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(85, 23)
        Me.btnStart.TabIndex = 2
        Me.btnStart.Text = "Start"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Location = New System.Drawing.Point(103, 60)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(85, 23)
        Me.btnStop.TabIndex = 3
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'lblStan
        '
        Me.lblStan.AutoSize = True
        Me.lblStan.Location = New System.Drawing.Point(38, 18)
        Me.lblStan.Name = "lblStan"
        Me.lblStan.Size = New System.Drawing.Size(61, 13)
        Me.lblStan.TabIndex = 4
        Me.lblStan.Text = "Zatrzymany"
        '
        'pnlStan
        '
        Me.pnlStan.BackColor = System.Drawing.Color.Red
        Me.pnlStan.Location = New System.Drawing.Point(12, 12)
        Me.pnlStan.Name = "pnlStan"
        Me.pnlStan.Size = New System.Drawing.Size(20, 20)
        Me.pnlStan.TabIndex = 5
        '
        'ntfIkona
        '
        Me.ntfIkona.ContextMenuStrip = Me.ctxKontrolaTlo
        Me.ntfIkona.Text = "Serwer HTTP"
        Me.ntfIkona.Visible = True
        '
        'ctxKontrolaTlo
        '
        Me.ctxKontrolaTlo.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuOtworzOkno, Me.mnuZamknijProgram, Me.mnuStan})
        Me.ctxKontrolaTlo.Name = "ctxKontrolaTlo"
        Me.ctxKontrolaTlo.Size = New System.Drawing.Size(143, 70)
        '
        'mnuOtworzOkno
        '
        Me.mnuOtworzOkno.Name = "mnuOtworzOkno"
        Me.mnuOtworzOkno.Size = New System.Drawing.Size(142, 22)
        Me.mnuOtworzOkno.Text = "Otwórz okno"
        '
        'mnuZamknijProgram
        '
        Me.mnuZamknijProgram.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.mnuZamknijProgram.Name = "mnuZamknijProgram"
        Me.mnuZamknijProgram.Size = New System.Drawing.Size(142, 22)
        Me.mnuZamknijProgram.Text = "Zakończ..."
        '
        'mnuStan
        '
        Me.mnuStan.BackColor = System.Drawing.Color.Red
        Me.mnuStan.Name = "mnuStan"
        Me.mnuStan.Size = New System.Drawing.Size(142, 22)
        Me.mnuStan.Text = "Zatrzymany"
        '
        'wndOkno
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(201, 89)
        Me.Controls.Add(Me.pnlStan)
        Me.Controls.Add(Me.lblStan)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.txtPort)
        Me.Controls.Add(Me.Label1)
        Me.Name = "wndOkno"
        Me.Text = "Serwer HTTP"
        Me.ctxKontrolaTlo.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtPort As System.Windows.Forms.TextBox
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents lblStan As System.Windows.Forms.Label
    Friend WithEvents pnlStan As System.Windows.Forms.Panel
    Friend WithEvents ntfIkona As NotifyIcon
    Friend WithEvents ctxKontrolaTlo As ContextMenuStrip
    Friend WithEvents mnuOtworzOkno As ToolStripMenuItem
    Friend WithEvents mnuZamknijProgram As ToolStripMenuItem
    Friend WithEvents mnuStan As ToolStripMenuItem
End Class
