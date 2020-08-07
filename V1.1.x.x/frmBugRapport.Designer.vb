<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBugRapport
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBugRapport))
        Me.btnSend = New System.Windows.Forms.Button()
        Me.txtProblem = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.chkSendTelemeter = New System.Windows.Forms.CheckBox()
        Me.chkSendLog = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(384, 202)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(75, 64)
        Me.btnSend.TabIndex = 1
        Me.btnSend.Text = "Send it"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'txtProblem
        '
        Me.txtProblem.Location = New System.Drawing.Point(12, 31)
        Me.txtProblem.Multiline = True
        Me.txtProblem.Name = "txtProblem"
        Me.txtProblem.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtProblem.Size = New System.Drawing.Size(447, 165)
        Me.txtProblem.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(102, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Problem description:"
        '
        'txtEmail
        '
        Me.txtEmail.Location = New System.Drawing.Point(73, 13)
        Me.txtEmail.Name = "txtEmail"
        Me.txtEmail.Size = New System.Drawing.Size(183, 20)
        Me.txtEmail.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.txtEmail, "Your email adres for feedback")
        '
        'chkSendTelemeter
        '
        Me.chkSendTelemeter.AutoSize = True
        Me.chkSendTelemeter.Checked = True
        Me.chkSendTelemeter.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSendTelemeter.Location = New System.Drawing.Point(6, 39)
        Me.chkSendTelemeter.Name = "chkSendTelemeter"
        Me.chkSendTelemeter.Size = New System.Drawing.Size(128, 17)
        Me.chkSendTelemeter.TabIndex = 4
        Me.chkSendTelemeter.Text = "Send Telemeter page"
        Me.ToolTip1.SetToolTip(Me.chkSendTelemeter, "If it can be handy send this page allong")
        Me.chkSendTelemeter.UseVisualStyleBackColor = True
        '
        'chkSendLog
        '
        Me.chkSendLog.AutoSize = True
        Me.chkSendLog.Checked = True
        Me.chkSendLog.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSendLog.Location = New System.Drawing.Point(137, 39)
        Me.chkSendLog.Name = "chkSendLog"
        Me.chkSendLog.Size = New System.Drawing.Size(119, 17)
        Me.chkSendLog.TabIndex = 5
        Me.chkSendLog.Text = "Send logbook page"
        Me.ToolTip1.SetToolTip(Me.chkSendLog, "If it can be handy send this page allong")
        Me.chkSendLog.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.chkSendLog)
        Me.GroupBox1.Controls.Add(Me.chkSendTelemeter)
        Me.GroupBox1.Controls.Add(Me.txtEmail)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Location = New System.Drawing.Point(15, 202)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(268, 64)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Extra data"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(61, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Email adres"
        '
        'frmBugRapport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(471, 276)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtProblem)
        Me.Controls.Add(Me.btnSend)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmBugRapport"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "Bug Rapport"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents btnSend As System.Windows.Forms.Button
    Friend WithEvents txtProblem As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents chkSendTelemeter As System.Windows.Forms.CheckBox
    Friend WithEvents txtEmail As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents chkSendLog As System.Windows.Forms.CheckBox
End Class
