<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMobile
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMobile))
        Me.lvData = New System.Windows.Forms.ListView()
        Me.colTel = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colAbo = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colLimit = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colUsage = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colExtra = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
        '
        'lvData
        '
        Me.lvData.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colTel, Me.colAbo, Me.colLimit, Me.colUsage, Me.colExtra})
        Me.lvData.Location = New System.Drawing.Point(0, 0)
        Me.lvData.Name = "lvData"
        Me.lvData.Size = New System.Drawing.Size(822, 150)
        Me.lvData.TabIndex = 0
        Me.lvData.UseCompatibleStateImageBehavior = False
        Me.lvData.View = System.Windows.Forms.View.Details
        '
        'colTel
        '
        Me.colTel.Text = "Tel. nummer"
        Me.colTel.Width = 125
        '
        'colAbo
        '
        Me.colAbo.Text = "Abo"
        Me.colAbo.Width = 160
        '
        'colLimit
        '
        Me.colLimit.Text = "Limit"
        Me.colLimit.Width = 97
        '
        'colUsage
        '
        Me.colUsage.Text = "Usage"
        Me.colUsage.Width = 164
        '
        'colExtra
        '
        Me.colExtra.Text = "Extra"
        '
        'frmMobile
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(858, 236)
        Me.Controls.Add(Me.lvData)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmMobile"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "frmMobile"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lvData As ListView
    Friend WithEvents colTel As ColumnHeader
    Friend WithEvents colAbo As ColumnHeader
    Friend WithEvents colLimit As ColumnHeader
    Friend WithEvents colUsage As ColumnHeader
    Friend WithEvents colExtra As ColumnHeader
End Class
