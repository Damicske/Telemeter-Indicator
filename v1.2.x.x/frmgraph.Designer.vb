<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGraph
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGraph))
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuShowStandard = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuShowPay = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuShowExtra = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuShowWifi = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuShowAvarage = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuShowAvgCalc = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuShowLimit = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuShowLimitDay = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.mnuPeriod = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuPeriodThis = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(214, 161)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuShowStandard, Me.mnuShowPay, Me.mnuShowExtra, Me.mnuShowWifi, Me.ToolStripMenuItem1, Me.mnuShowAvarage, Me.mnuShowAvgCalc, Me.mnuShowLimit, Me.ToolStripMenuItem2, Me.mnuPeriod})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(215, 214)
        '
        'mnuShowStandard
        '
        Me.mnuShowStandard.Name = "mnuShowStandard"
        Me.mnuShowStandard.Size = New System.Drawing.Size(214, 22)
        Me.mnuShowStandard.Text = "Standaard"
        '
        'mnuShowPay
        '
        Me.mnuShowPay.BackColor = System.Drawing.SystemColors.Control
        Me.mnuShowPay.Checked = True
        Me.mnuShowPay.CheckOnClick = True
        Me.mnuShowPay.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuShowPay.Name = "mnuShowPay"
        Me.mnuShowPay.Size = New System.Drawing.Size(214, 22)
        Me.mnuShowPay.Text = "Toon betaald"
        Me.mnuShowPay.Visible = False
        '
        'mnuShowExtra
        '
        Me.mnuShowExtra.Checked = True
        Me.mnuShowExtra.CheckOnClick = True
        Me.mnuShowExtra.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuShowExtra.Name = "mnuShowExtra"
        Me.mnuShowExtra.Size = New System.Drawing.Size(214, 22)
        Me.mnuShowExtra.Text = "Toon Extra"
        '
        'mnuShowWifi
        '
        Me.mnuShowWifi.Checked = True
        Me.mnuShowWifi.CheckOnClick = True
        Me.mnuShowWifi.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuShowWifi.Name = "mnuShowWifi"
        Me.mnuShowWifi.Size = New System.Drawing.Size(214, 22)
        Me.mnuShowWifi.Text = "Toon Wifi"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(211, 6)
        '
        'mnuShowAvarage
        '
        Me.mnuShowAvarage.Checked = True
        Me.mnuShowAvarage.CheckOnClick = True
        Me.mnuShowAvarage.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuShowAvarage.Name = "mnuShowAvarage"
        Me.mnuShowAvarage.Size = New System.Drawing.Size(214, 22)
        Me.mnuShowAvarage.Text = "Toon gemiddeld verbruik"
        '
        'mnuShowAvgCalc
        '
        Me.mnuShowAvgCalc.Checked = True
        Me.mnuShowAvgCalc.CheckOnClick = True
        Me.mnuShowAvgCalc.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuShowAvgCalc.Name = "mnuShowAvgCalc"
        Me.mnuShowAvgCalc.Size = New System.Drawing.Size(214, 22)
        Me.mnuShowAvgCalc.Text = "Toon gemiddeld berekend"
        '
        'mnuShowLimit
        '
        Me.mnuShowLimit.Checked = True
        Me.mnuShowLimit.CheckOnClick = True
        Me.mnuShowLimit.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuShowLimit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuShowLimitDay})
        Me.mnuShowLimit.Name = "mnuShowLimit"
        Me.mnuShowLimit.Size = New System.Drawing.Size(214, 22)
        Me.mnuShowLimit.Text = "Toon over limiet"
        '
        'mnuShowLimitDay
        '
        Me.mnuShowLimitDay.CheckOnClick = True
        Me.mnuShowLimitDay.Name = "mnuShowLimitDay"
        Me.mnuShowLimitDay.Size = New System.Drawing.Size(114, 22)
        Me.mnuShowLimitDay.Text = "Per dag"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(211, 6)
        '
        'mnuPeriod
        '
        Me.mnuPeriod.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPeriodThis, Me.ToolStripMenuItem3})
        Me.mnuPeriod.Name = "mnuPeriod"
        Me.mnuPeriod.Size = New System.Drawing.Size(214, 22)
        Me.mnuPeriod.Text = "Period"
        '
        'mnuPeriodThis
        '
        Me.mnuPeriodThis.Checked = True
        Me.mnuPeriodThis.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuPeriodThis.Name = "mnuPeriodThis"
        Me.mnuPeriodThis.Size = New System.Drawing.Size(152, 22)
        Me.mnuPeriodThis.Text = "This period"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(149, 6)
        Me.ToolStripMenuItem3.Visible = False
        '
        'frmGraph
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 562)
        Me.Controls.Add(Me.PictureBox1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(600, 600)
        Me.Name = "frmGraph"
        Me.Text = "TeleGrafiek"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuShowPay As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuShowExtra As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuShowWifi As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuShowAvarage As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuShowAvgCalc As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuShowLimit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPeriod As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPeriodThis As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuShowStandard As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuShowLimitDay As System.Windows.Forms.ToolStripMenuItem

End Class
