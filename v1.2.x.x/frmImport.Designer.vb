<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmImport
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
        Me.pbFiles = New System.Windows.Forms.ProgressBar()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.chkMoveBin = New System.Windows.Forms.CheckBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.btnImportDb = New System.Windows.Forms.Button()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.SuspendLayout()
        '
        'pbFiles
        '
        Me.pbFiles.Location = New System.Drawing.Point(17, 146)
        Me.pbFiles.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pbFiles.Name = "pbFiles"
        Me.pbFiles.Size = New System.Drawing.Size(382, 35)
        Me.pbFiles.Step = 1
        Me.pbFiles.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.pbFiles.TabIndex = 0
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(135, 42)
        Me.btnStart.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(112, 35)
        Me.btnStart.TabIndex = 1
        Me.btnStart.Text = "Start"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'chkMoveBin
        '
        Me.chkMoveBin.AutoSize = True
        Me.chkMoveBin.Checked = True
        Me.chkMoveBin.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkMoveBin.Location = New System.Drawing.Point(0, 8)
        Me.chkMoveBin.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.chkMoveBin.Name = "chkMoveBin"
        Me.chkMoveBin.Size = New System.Drawing.Size(210, 24)
        Me.chkMoveBin.TabIndex = 2
        Me.chkMoveBin.Text = "Move files to Recycle Bin"
        Me.chkMoveBin.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(13, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(390, 126)
        Me.TabControl1.TabIndex = 3
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.btnStart)
        Me.TabPage1.Controls.Add(Me.chkMoveBin)
        Me.TabPage1.Location = New System.Drawing.Point(4, 29)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(382, 93)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Files"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.btnExport)
        Me.TabPage2.Controls.Add(Me.btnImportDb)
        Me.TabPage2.Location = New System.Drawing.Point(4, 29)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(382, 93)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Database"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'btnExport
        '
        Me.btnExport.Location = New System.Drawing.Point(87, 51)
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Size = New System.Drawing.Size(182, 30)
        Me.btnExport.TabIndex = 4
        Me.btnExport.Text = "Export Browse..."
        Me.btnExport.UseVisualStyleBackColor = True
        '
        'btnImportDb
        '
        Me.btnImportDb.Location = New System.Drawing.Point(87, 15)
        Me.btnImportDb.Name = "btnImportDb"
        Me.btnImportDb.Size = New System.Drawing.Size(182, 30)
        Me.btnImportDb.TabIndex = 3
        Me.btnImportDb.Text = "Import Browse..."
        Me.btnImportDb.UseVisualStyleBackColor = True
        '
        'frmImport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(417, 192)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.pbFiles)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmImport"
        Me.Text = "Import usage data"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pbFiles As ProgressBar
    Friend WithEvents btnStart As Button
    Friend WithEvents chkMoveBin As CheckBox
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents btnImportDb As Button
    Friend WithEvents btnExport As Button
End Class
