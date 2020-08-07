<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConnections
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmConnections))
        Me.lvCons = New System.Windows.Forms.ListView()
        Me.chMac = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chV4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chV6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chConn = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
        '
        'lvCons
        '
        Me.lvCons.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chMac, Me.chV4, Me.chV6, Me.chName, Me.chConn})
        Me.lvCons.FullRowSelect = True
        Me.lvCons.GridLines = True
        Me.lvCons.Location = New System.Drawing.Point(0, 0)
        Me.lvCons.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.lvCons.MultiSelect = False
        Me.lvCons.Name = "lvCons"
        Me.lvCons.Size = New System.Drawing.Size(1034, 305)
        Me.lvCons.TabIndex = 1
        Me.lvCons.UseCompatibleStateImageBehavior = False
        Me.lvCons.View = System.Windows.Forms.View.Details
        '
        'chMac
        '
        Me.chMac.Text = "MAC Adress"
        Me.chMac.Width = 109
        '
        'chV4
        '
        Me.chV4.Text = "IPv4"
        Me.chV4.Width = 83
        '
        'chV6
        '
        Me.chV6.Text = "IPv6"
        Me.chV6.Width = 213
        '
        'chName
        '
        Me.chName.Text = "Name"
        Me.chName.Width = 205
        '
        'chConn
        '
        Me.chConn.Text = "Connection"
        Me.chConn.Width = 163
        '
        'frmConnections
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1174, 403)
        Me.Controls.Add(Me.lvCons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.Name = "frmConnections"
        Me.Text = "frmConnections"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lvCons As System.Windows.Forms.ListView
    Friend WithEvents chMac As System.Windows.Forms.ColumnHeader
    Friend WithEvents chV4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chV6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chConn As System.Windows.Forms.ColumnHeader
End Class
