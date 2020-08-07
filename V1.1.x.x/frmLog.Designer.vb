<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLog
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
        Dim ListViewItem1 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem(New String() {"", "", ""}, -1)
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmLog))
        Me.gbFilter = New System.Windows.Forms.GroupBox()
        Me.chkShowDbg = New System.Windows.Forms.CheckBox()
        Me.ChkShowHtml = New System.Windows.Forms.CheckBox()
        Me.chkShowErr = New System.Windows.Forms.CheckBox()
        Me.bntFilterClose = New System.Windows.Forms.Button()
        Me.btnFilterApply = New System.Windows.Forms.Button()
        Me.mcFilter = New System.Windows.Forms.MonthCalendar()
        Me.lvLog = New System.Windows.Forms.ListView()
        Me.chTime = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chMsg = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.cmsFilter = New System.Windows.Forms.ToolStripMenuItem()
        Me.cmsCopyLine = New System.Windows.Forms.ToolStripMenuItem()
        Me.pbLoad = New System.Windows.Forms.ProgressBar()
        Me.cmsColorLine = New System.Windows.Forms.ToolStripMenuItem()
        Me.gbFilter.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'gbFilter
        '
        Me.gbFilter.Controls.Add(Me.chkShowDbg)
        Me.gbFilter.Controls.Add(Me.ChkShowHtml)
        Me.gbFilter.Controls.Add(Me.chkShowErr)
        Me.gbFilter.Controls.Add(Me.bntFilterClose)
        Me.gbFilter.Controls.Add(Me.btnFilterApply)
        Me.gbFilter.Controls.Add(Me.mcFilter)
        Me.gbFilter.Location = New System.Drawing.Point(121, 101)
        Me.gbFilter.Name = "gbFilter"
        Me.gbFilter.Size = New System.Drawing.Size(542, 340)
        Me.gbFilter.TabIndex = 1
        Me.gbFilter.TabStop = False
        Me.gbFilter.Text = "Filter"
        '
        'chkShowDbg
        '
        Me.chkShowDbg.AutoSize = True
        Me.chkShowDbg.Checked = True
        Me.chkShowDbg.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkShowDbg.Location = New System.Drawing.Point(305, 91)
        Me.chkShowDbg.Name = "chkShowDbg"
        Me.chkShowDbg.Size = New System.Drawing.Size(83, 24)
        Me.chkShowDbg.TabIndex = 6
        Me.chkShowDbg.Text = "Debug"
        Me.chkShowDbg.UseVisualStyleBackColor = True
        '
        'ChkShowHtml
        '
        Me.ChkShowHtml.AutoSize = True
        Me.ChkShowHtml.Checked = True
        Me.ChkShowHtml.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChkShowHtml.Location = New System.Drawing.Point(305, 61)
        Me.ChkShowHtml.Name = "ChkShowHtml"
        Me.ChkShowHtml.Size = New System.Drawing.Size(78, 24)
        Me.ChkShowHtml.TabIndex = 5
        Me.ChkShowHtml.Text = "HTML"
        Me.ChkShowHtml.UseVisualStyleBackColor = True
        '
        'chkShowErr
        '
        Me.chkShowErr.AutoSize = True
        Me.chkShowErr.Checked = True
        Me.chkShowErr.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkShowErr.Location = New System.Drawing.Point(305, 31)
        Me.chkShowErr.Name = "chkShowErr"
        Me.chkShowErr.Size = New System.Drawing.Size(81, 24)
        Me.chkShowErr.TabIndex = 4
        Me.chkShowErr.Text = "Error's"
        Me.chkShowErr.UseVisualStyleBackColor = True
        '
        'bntFilterClose
        '
        Me.bntFilterClose.Location = New System.Drawing.Point(513, 0)
        Me.bntFilterClose.Name = "bntFilterClose"
        Me.bntFilterClose.Size = New System.Drawing.Size(29, 29)
        Me.bntFilterClose.TabIndex = 3
        Me.bntFilterClose.Text = "X"
        Me.bntFilterClose.UseVisualStyleBackColor = True
        '
        'btnFilterApply
        '
        Me.btnFilterApply.Location = New System.Drawing.Point(194, 295)
        Me.btnFilterApply.Name = "btnFilterApply"
        Me.btnFilterApply.Size = New System.Drawing.Size(75, 32)
        Me.btnFilterApply.TabIndex = 2
        Me.btnFilterApply.Text = "Filter"
        Me.btnFilterApply.UseVisualStyleBackColor = True
        '
        'mcFilter
        '
        Me.mcFilter.Location = New System.Drawing.Point(12, 31)
        Me.mcFilter.Name = "mcFilter"
        Me.mcFilter.ShowWeekNumbers = True
        Me.mcFilter.TabIndex = 1
        '
        'lvLog
        '
        Me.lvLog.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chTime, Me.chMsg})
        Me.lvLog.ContextMenuStrip = Me.ContextMenuStrip1
        Me.lvLog.FullRowSelect = True
        Me.lvLog.GridLines = True
        Me.lvLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvLog.Items.AddRange(New System.Windows.Forms.ListViewItem() {ListViewItem1})
        Me.lvLog.Location = New System.Drawing.Point(64, 457)
        Me.lvLog.Name = "lvLog"
        Me.lvLog.Size = New System.Drawing.Size(615, 97)
        Me.lvLog.TabIndex = 2
        Me.lvLog.UseCompatibleStateImageBehavior = False
        Me.lvLog.View = System.Windows.Forms.View.Details
        '
        'chTime
        '
        Me.chTime.Text = "Date Time"
        Me.chTime.Width = 349
        '
        'chMsg
        '
        Me.chMsg.Text = "Message"
        Me.chMsg.Width = 250
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.cmsFilter, Me.cmsCopyLine, Me.cmsColorLine})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(234, 127)
        '
        'cmsFilter
        '
        Me.cmsFilter.Name = "cmsFilter"
        Me.cmsFilter.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F), System.Windows.Forms.Keys)
        Me.cmsFilter.Size = New System.Drawing.Size(233, 30)
        Me.cmsFilter.Text = "&Filter"
        '
        'cmsCopyLine
        '
        Me.cmsCopyLine.Name = "cmsCopyLine"
        Me.cmsCopyLine.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.cmsCopyLine.Size = New System.Drawing.Size(233, 30)
        Me.cmsCopyLine.Text = "&Copy line"
        '
        'pbLoad
        '
        Me.pbLoad.Location = New System.Drawing.Point(101, 520)
        Me.pbLoad.Name = "pbLoad"
        Me.pbLoad.Size = New System.Drawing.Size(487, 50)
        Me.pbLoad.Step = 1
        Me.pbLoad.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.pbLoad.TabIndex = 3
        '
        'cmsColorLine
        '
        Me.cmsColorLine.Checked = True
        Me.cmsColorLine.CheckOnClick = True
        Me.cmsColorLine.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cmsColorLine.Name = "cmsColorLine"
        Me.cmsColorLine.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.L), System.Windows.Forms.Keys)
        Me.cmsColorLine.Size = New System.Drawing.Size(233, 30)
        Me.cmsColorLine.Text = "Co&lor line"
        '
        'frmLog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(813, 682)
        Me.Controls.Add(Me.pbLoad)
        Me.Controls.Add(Me.gbFilter)
        Me.Controls.Add(Me.lvLog)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "frmLog"
        Me.Text = "Log"
        Me.gbFilter.ResumeLayout(False)
        Me.gbFilter.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gbFilter As GroupBox
    Friend WithEvents mcFilter As MonthCalendar
    Friend WithEvents lvLog As ListView
    Friend WithEvents chTime As ColumnHeader
    Friend WithEvents chMsg As ColumnHeader
    Friend WithEvents bntFilterClose As Button
    Friend WithEvents btnFilterApply As Button
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents cmsFilter As ToolStripMenuItem
    Friend WithEvents chkShowErr As CheckBox
    Friend WithEvents chkShowDbg As CheckBox
    Friend WithEvents ChkShowHtml As CheckBox
    Friend WithEvents cmsCopyLine As ToolStripMenuItem
    Friend WithEvents pbLoad As ProgressBar
    Friend WithEvents cmsColorLine As ToolStripMenuItem
End Class
