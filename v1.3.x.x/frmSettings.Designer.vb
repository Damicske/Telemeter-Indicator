<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSettings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSettings))
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.lblInterval = New System.Windows.Forms.Label()
        Me.tbInterval = New System.Windows.Forms.TrackBar()
        Me.tmrUpdate = New System.Windows.Forms.Timer(Me.components)
        Me.TrayIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuUsage = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuUpdate = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuGraph = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuLog = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuMijnTelenet = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuMijnTelenetConnecties = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuMijnTelenetMention = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuSettings = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSettingsShowMyTelenet = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSettingsShowBinair = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSettingsSaveLog = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSettingsStartup = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSettingsDynIcon = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSettingsSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSettingsDataSend = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuHelp = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuHelpSendBug = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuHelpUpdate = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuHelpHistory = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuHelpAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.cbLanguage = New System.Windows.Forms.ComboBox()
        Me.lblLanguage = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.mtbPassword = New System.Windows.Forms.MaskedTextBox()
        Me.txtUserId = New System.Windows.Forms.TextBox()
        Me.lblId = New System.Windows.Forms.Label()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.lblUpdChk = New System.Windows.Forms.Label()
        Me.cbUpdChk = New System.Windows.Forms.ComboBox()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.BGW = New System.ComponentModel.BackgroundWorker()
        CType(Me.tbInterval, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(206, 177)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(66, 26)
        Me.btnCancel.TabIndex = 6
        Me.btnCancel.Text = "Sluit"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'lblInterval
        '
        Me.lblInterval.AutoSize = True
        Me.lblInterval.Location = New System.Drawing.Point(12, 12)
        Me.lblInterval.Name = "lblInterval"
        Me.lblInterval.Size = New System.Drawing.Size(87, 13)
        Me.lblInterval.TabIndex = 3
        Me.lblInterval.Text = "Interval (minutes)"
        '
        'tbInterval
        '
        Me.tbInterval.LargeChange = 10
        Me.tbInterval.Location = New System.Drawing.Point(105, 12)
        Me.tbInterval.Maximum = 60
        Me.tbInterval.Minimum = 10
        Me.tbInterval.Name = "tbInterval"
        Me.tbInterval.Size = New System.Drawing.Size(174, 45)
        Me.tbInterval.SmallChange = 5
        Me.tbInterval.TabIndex = 1
        Me.tbInterval.TickFrequency = 5
        Me.tbInterval.Value = 30
        '
        'tmrUpdate
        '
        Me.tmrUpdate.Interval = 10000
        '
        'TrayIcon
        '
        Me.TrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.TrayIcon.ContextMenuStrip = Me.ContextMenuStrip1
        Me.TrayIcon.Visible = True
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuUsage, Me.ToolStripMenuItem4, Me.mnuUpdate, Me.mnuGraph, Me.mnuLog, Me.ToolStripMenuItem1, Me.mnuMijnTelenet, Me.mnuMijnTelenetMention, Me.ToolStripSeparator1, Me.mnuSettings, Me.mnuHelp, Me.ToolStripMenuItem2, Me.mnuExit})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(202, 226)
        '
        'mnuUsage
        '
        Me.mnuUsage.BackColor = System.Drawing.Color.DarkGray
        Me.mnuUsage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuUsage.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.mnuUsage.ForeColor = System.Drawing.Color.Black
        Me.mnuUsage.Name = "mnuUsage"
        Me.mnuUsage.ShowShortcutKeys = False
        Me.mnuUsage.Size = New System.Drawing.Size(201, 22)
        Me.mnuUsage.Text = "Data usage"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(198, 6)
        '
        'mnuUpdate
        '
        Me.mnuUpdate.Enabled = False
        Me.mnuUpdate.Image = Global.TelemeterIndicator.My.Resources.Resources.refresh_icon
        Me.mnuUpdate.Name = "mnuUpdate"
        Me.mnuUpdate.ShortcutKeys = System.Windows.Forms.Keys.F5
        Me.mnuUpdate.Size = New System.Drawing.Size(201, 22)
        Me.mnuUpdate.Text = "Update &TeleData"
        '
        'mnuGraph
        '
        Me.mnuGraph.Enabled = False
        Me.mnuGraph.Image = CType(resources.GetObject("mnuGraph.Image"), System.Drawing.Image)
        Me.mnuGraph.Name = "mnuGraph"
        Me.mnuGraph.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.G), System.Windows.Forms.Keys)
        Me.mnuGraph.Size = New System.Drawing.Size(201, 22)
        Me.mnuGraph.Text = "Toon Tele&grafiek"
        '
        'mnuLog
        '
        Me.mnuLog.Enabled = False
        Me.mnuLog.Image = Global.TelemeterIndicator.My.Resources.Resources.Icon_logbook_des_illu_de
        Me.mnuLog.Name = "mnuLog"
        Me.mnuLog.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.L), System.Windows.Forms.Keys)
        Me.mnuLog.Size = New System.Drawing.Size(201, 22)
        Me.mnuLog.Text = "Toon &logboek"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(198, 6)
        '
        'mnuMijnTelenet
        '
        Me.mnuMijnTelenet.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuMijnTelenetConnecties})
        Me.mnuMijnTelenet.Name = "mnuMijnTelenet"
        Me.mnuMijnTelenet.Size = New System.Drawing.Size(201, 22)
        Me.mnuMijnTelenet.Text = "&Mijn Telenet"
        '
        'mnuMijnTelenetConnecties
        '
        Me.mnuMijnTelenetConnecties.Enabled = False
        Me.mnuMijnTelenetConnecties.Image = CType(resources.GetObject("mnuMijnTelenetConnecties.Image"), System.Drawing.Image)
        Me.mnuMijnTelenetConnecties.Name = "mnuMijnTelenetConnecties"
        Me.mnuMijnTelenetConnecties.Size = New System.Drawing.Size(133, 22)
        Me.mnuMijnTelenetConnecties.Text = "&Connecties"
        '
        'mnuMijnTelenetMention
        '
        Me.mnuMijnTelenetMention.Enabled = False
        Me.mnuMijnTelenetMention.Name = "mnuMijnTelenetMention"
        Me.mnuMijnTelenetMention.Size = New System.Drawing.Size(201, 22)
        Me.mnuMijnTelenetMention.Text = "Mijn Telenet melding"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(198, 6)
        '
        'mnuSettings
        '
        Me.mnuSettings.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSettingsShowMyTelenet, Me.mnuSettingsShowBinair, Me.mnuSettingsSaveLog, Me.mnuSettingsStartup, Me.mnuSettingsDynIcon, Me.mnuSettingsSave, Me.mnuSettingsDataSend})
        Me.mnuSettings.Image = Global.TelemeterIndicator.My.Resources.Resources.settings1
        Me.mnuSettings.Name = "mnuSettings"
        Me.mnuSettings.Size = New System.Drawing.Size(201, 22)
        Me.mnuSettings.Text = "Toon In&stellingen"
        '
        'mnuSettingsShowMyTelenet
        '
        Me.mnuSettingsShowMyTelenet.Checked = True
        Me.mnuSettingsShowMyTelenet.CheckOnClick = True
        Me.mnuSettingsShowMyTelenet.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuSettingsShowMyTelenet.Name = "mnuSettingsShowMyTelenet"
        Me.mnuSettingsShowMyTelenet.Size = New System.Drawing.Size(231, 22)
        Me.mnuSettingsShowMyTelenet.Text = "Toon &Mijn Telenet meldingen"
        '
        'mnuSettingsShowBinair
        '
        Me.mnuSettingsShowBinair.Checked = True
        Me.mnuSettingsShowBinair.CheckOnClick = True
        Me.mnuSettingsShowBinair.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuSettingsShowBinair.Name = "mnuSettingsShowBinair"
        Me.mnuSettingsShowBinair.Size = New System.Drawing.Size(231, 22)
        Me.mnuSettingsShowBinair.Text = "Toon &binaire eenheid"
        '
        'mnuSettingsSaveLog
        '
        Me.mnuSettingsSaveLog.Checked = True
        Me.mnuSettingsSaveLog.CheckOnClick = True
        Me.mnuSettingsSaveLog.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuSettingsSaveLog.Name = "mnuSettingsSaveLog"
        Me.mnuSettingsSaveLog.Size = New System.Drawing.Size(231, 22)
        Me.mnuSettingsSaveLog.Text = "&Log opslaan als bestand"
        '
        'mnuSettingsStartup
        '
        Me.mnuSettingsStartup.Checked = True
        Me.mnuSettingsStartup.CheckOnClick = True
        Me.mnuSettingsStartup.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuSettingsStartup.Name = "mnuSettingsStartup"
        Me.mnuSettingsStartup.Size = New System.Drawing.Size(231, 22)
        Me.mnuSettingsStartup.Text = "Opstarten na &Windows login"
        '
        'mnuSettingsDynIcon
        '
        Me.mnuSettingsDynIcon.CheckOnClick = True
        Me.mnuSettingsDynIcon.Name = "mnuSettingsDynIcon"
        Me.mnuSettingsDynIcon.Size = New System.Drawing.Size(231, 22)
        Me.mnuSettingsDynIcon.Text = "&Dynamisch icoon"
        '
        'mnuSettingsSave
        '
        Me.mnuSettingsSave.CheckOnClick = True
        Me.mnuSettingsSave.Name = "mnuSettingsSave"
        Me.mnuSettingsSave.Size = New System.Drawing.Size(231, 22)
        Me.mnuSettingsSave.Text = "&Opslaan Verbruik"
        '
        'mnuSettingsDataSend
        '
        Me.mnuSettingsDataSend.Checked = True
        Me.mnuSettingsDataSend.CheckOnClick = True
        Me.mnuSettingsDataSend.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuSettingsDataSend.Name = "mnuSettingsDataSend"
        Me.mnuSettingsDataSend.Size = New System.Drawing.Size(231, 22)
        Me.mnuSettingsDataSend.Text = "Data send to server"
        '
        'mnuHelp
        '
        Me.mnuHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuHelpSendBug, Me.mnuHelpUpdate, Me.mnuHelpHistory, Me.ToolStripMenuItem3, Me.mnuHelpAbout})
        Me.mnuHelp.Image = Global.TelemeterIndicator.My.Resources.Resources.help
        Me.mnuHelp.Name = "mnuHelp"
        Me.mnuHelp.Size = New System.Drawing.Size(201, 22)
        Me.mnuHelp.Text = "&Help"
        '
        'mnuHelpSendBug
        '
        Me.mnuHelpSendBug.Name = "mnuHelpSendBug"
        Me.mnuHelpSendBug.Size = New System.Drawing.Size(152, 22)
        Me.mnuHelpSendBug.Text = "Bug rapport"
        '
        'mnuHelpUpdate
        '
        Me.mnuHelpUpdate.Name = "mnuHelpUpdate"
        Me.mnuHelpUpdate.Size = New System.Drawing.Size(152, 22)
        Me.mnuHelpUpdate.Text = "Update"
        '
        'mnuHelpHistory
        '
        Me.mnuHelpHistory.Name = "mnuHelpHistory"
        Me.mnuHelpHistory.Size = New System.Drawing.Size(152, 22)
        Me.mnuHelpHistory.Text = "History"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(149, 6)
        '
        'mnuHelpAbout
        '
        Me.mnuHelpAbout.Name = "mnuHelpAbout"
        Me.mnuHelpAbout.Size = New System.Drawing.Size(152, 22)
        Me.mnuHelpAbout.Text = "About"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(198, 6)
        '
        'mnuExit
        '
        Me.mnuExit.Name = "mnuExit"
        Me.mnuExit.Size = New System.Drawing.Size(201, 22)
        Me.mnuExit.Text = "&Exit"
        '
        'cbLanguage
        '
        Me.cbLanguage.Enabled = False
        Me.cbLanguage.FormattingEnabled = True
        Me.cbLanguage.Location = New System.Drawing.Point(117, 97)
        Me.cbLanguage.Name = "cbLanguage"
        Me.cbLanguage.Size = New System.Drawing.Size(155, 21)
        Me.cbLanguage.TabIndex = 4
        '
        'lblLanguage
        '
        Me.lblLanguage.AutoSize = True
        Me.lblLanguage.Location = New System.Drawing.Point(12, 100)
        Me.lblLanguage.Name = "lblLanguage"
        Me.lblLanguage.Size = New System.Drawing.Size(28, 13)
        Me.lblLanguage.TabIndex = 10
        Me.lblLanguage.Text = "Taal"
        '
        'ToolTip1
        '
        Me.ToolTip1.AutomaticDelay = 50
        Me.ToolTip1.AutoPopDelay = 5000
        Me.ToolTip1.InitialDelay = 50
        Me.ToolTip1.IsBalloon = True
        Me.ToolTip1.ReshowDelay = 10
        Me.ToolTip1.ShowAlways = True
        Me.ToolTip1.UseAnimation = False
        '
        'mtbPassword
        '
        Me.mtbPassword.Location = New System.Drawing.Point(117, 71)
        Me.mtbPassword.Name = "mtbPassword"
        Me.mtbPassword.Size = New System.Drawing.Size(155, 20)
        Me.mtbPassword.TabIndex = 3
        Me.mtbPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ToolTip1.SetToolTip(Me.mtbPassword, "Geef je Telenet paswoord in")
        Me.mtbPassword.UseSystemPasswordChar = True
        '
        'txtUserId
        '
        Me.txtUserId.Location = New System.Drawing.Point(117, 45)
        Me.txtUserId.MaxLength = 7
        Me.txtUserId.Name = "txtUserId"
        Me.txtUserId.Size = New System.Drawing.Size(155, 20)
        Me.txtUserId.TabIndex = 2
        Me.txtUserId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ToolTip1.SetToolTip(Me.txtUserId, "Geef je Telenet ID op deze is 7 karakters lang en beginnende met een letter.")
        '
        'lblId
        '
        Me.lblId.AutoSize = True
        Me.lblId.Location = New System.Drawing.Point(12, 44)
        Me.lblId.Name = "lblId"
        Me.lblId.Size = New System.Drawing.Size(57, 13)
        Me.lblId.TabIndex = 6
        Me.lblId.Text = "Telenet ID"
        '
        'lblPassword
        '
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Location = New System.Drawing.Point(12, 74)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Size = New System.Drawing.Size(54, 13)
        Me.lblPassword.TabIndex = 7
        Me.lblPassword.Text = "Paswoord"
        '
        'lblUpdChk
        '
        Me.lblUpdChk.AutoSize = True
        Me.lblUpdChk.Location = New System.Drawing.Point(12, 127)
        Me.lblUpdChk.Name = "lblUpdChk"
        Me.lblUpdChk.Size = New System.Drawing.Size(111, 13)
        Me.lblUpdChk.TabIndex = 14
        Me.lblUpdChk.Text = "Program update when"
        '
        'cbUpdChk
        '
        Me.cbUpdChk.FormattingEnabled = True
        Me.cbUpdChk.Location = New System.Drawing.Point(117, 150)
        Me.cbUpdChk.Name = "cbUpdChk"
        Me.cbUpdChk.Size = New System.Drawing.Size(155, 21)
        Me.cbUpdChk.TabIndex = 13
        '
        'btnSave
        '
        Me.btnSave.Enabled = False
        Me.btnSave.Location = New System.Drawing.Point(117, 177)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(66, 26)
        Me.btnSave.TabIndex = 5
        Me.btnSave.Text = "Opslaan"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'BGW
        '
        Me.BGW.WorkerReportsProgress = True
        Me.BGW.WorkerSupportsCancellation = True
        '
        'frmSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(281, 215)
        Me.Controls.Add(Me.lblUpdChk)
        Me.Controls.Add(Me.cbUpdChk)
        Me.Controls.Add(Me.lblLanguage)
        Me.Controls.Add(Me.cbLanguage)
        Me.Controls.Add(Me.txtUserId)
        Me.Controls.Add(Me.lblPassword)
        Me.Controls.Add(Me.lblId)
        Me.Controls.Add(Me.tbInterval)
        Me.Controls.Add(Me.mtbPassword)
        Me.Controls.Add(Me.lblInterval)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmSettings"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "Window text"
        CType(Me.tbInterval, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblInterval As System.Windows.Forms.Label
    Friend WithEvents tbInterval As System.Windows.Forms.TrackBar
    Friend WithEvents tmrUpdate As System.Windows.Forms.Timer
    Friend WithEvents TrayIcon As System.Windows.Forms.NotifyIcon
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuUpdate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuGraph As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuSettings As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuExit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLog As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuMijnTelenet As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSettingsShowMyTelenet As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSettingsShowBinair As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSettingsSaveLog As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSettingsStartup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cbLanguage As System.Windows.Forms.ComboBox
    Friend WithEvents lblLanguage As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents mnuSettingsDynIcon As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSettingsSave As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mtbPassword As System.Windows.Forms.MaskedTextBox
    Friend WithEvents lblId As System.Windows.Forms.Label
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents txtUserId As System.Windows.Forms.TextBox
    Friend WithEvents mnuMijnTelenetMention As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuMijnTelenetConnecties As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelpSendBug As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelpUpdate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelpAbout As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelpHistory As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuSettingsDataSend As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUsage As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents lblUpdChk As System.Windows.Forms.Label
    Friend WithEvents cbUpdChk As System.Windows.Forms.ComboBox
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Public WithEvents BGW As System.ComponentModel.BackgroundWorker
End Class
