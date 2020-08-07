<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInfo
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmInfo))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lblUsed = New System.Windows.Forms.Label()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.lblVersion = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lblSiteUrl = New System.Windows.Forms.Label()
        Me.lblSite = New System.Windows.Forms.Label()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.lblComments = New System.Windows.Forms.Label()
        Me.lblProduct = New System.Windows.Forms.Label()
        Me.lblThanks = New System.Windows.Forms.Label()
        Me.tmrTitleScrll = New System.Windows.Forms.Timer(Me.components)
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lblCopy = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lblUsed)
        Me.GroupBox1.Controls.Add(Me.lblStatus)
        Me.GroupBox1.Controls.Add(Me.lblVersion)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 227)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(332, 43)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Program info"
        '
        'lblUsed
        '
        Me.lblUsed.AutoSize = True
        Me.lblUsed.Location = New System.Drawing.Point(121, 16)
        Me.lblUsed.Name = "lblUsed"
        Me.lblUsed.Size = New System.Drawing.Size(30, 13)
        Me.lblUsed.TabIndex = 3
        Me.lblUsed.Text = "used"
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(209, 16)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(37, 13)
        Me.lblStatus.TabIndex = 2
        Me.lblStatus.Text = "Status"
        '
        'lblVersion
        '
        Me.lblVersion.AutoSize = True
        Me.lblVersion.Location = New System.Drawing.Point(6, 16)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(42, 13)
        Me.lblVersion.TabIndex = 1
        Me.lblVersion.Text = "Version"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lblSiteUrl)
        Me.GroupBox2.Controls.Add(Me.lblSite)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 276)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(332, 43)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Contact"
        '
        'lblSiteUrl
        '
        Me.lblSiteUrl.AutoSize = True
        Me.lblSiteUrl.Cursor = System.Windows.Forms.Cursors.Hand
        Me.lblSiteUrl.Location = New System.Drawing.Point(56, 16)
        Me.lblSiteUrl.Name = "lblSiteUrl"
        Me.lblSiteUrl.Size = New System.Drawing.Size(112, 13)
        Me.lblSiteUrl.TabIndex = 2
        Me.lblSiteUrl.Text = "https://www.cd-pc.be"
        '
        'lblSite
        '
        Me.lblSite.AutoSize = True
        Me.lblSite.Location = New System.Drawing.Point(8, 16)
        Me.lblSite.Name = "lblSite"
        Me.lblSite.Size = New System.Drawing.Size(28, 13)
        Me.lblSite.TabIndex = 0
        Me.lblSite.Text = "Site:"
        '
        'btnBack
        '
        Me.btnBack.Location = New System.Drawing.Point(287, 325)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(57, 25)
        Me.btnBack.TabIndex = 2
        Me.btnBack.Text = "Back"
        Me.btnBack.UseVisualStyleBackColor = True
        '
        'lblComments
        '
        Me.lblComments.Location = New System.Drawing.Point(13, 165)
        Me.lblComments.Name = "lblComments"
        Me.lblComments.Size = New System.Drawing.Size(331, 59)
        Me.lblComments.TabIndex = 4
        Me.lblComments.Text = "Comments"
        '
        'lblProduct
        '
        Me.lblProduct.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lblProduct.AutoSize = True
        Me.lblProduct.Font = New System.Drawing.Font("Comic Sans MS", 20.25!)
        Me.lblProduct.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblProduct.Location = New System.Drawing.Point(5, 0)
        Me.lblProduct.Name = "lblProduct"
        Me.lblProduct.Size = New System.Drawing.Size(111, 38)
        Me.lblProduct.TabIndex = 5
        Me.lblProduct.Text = "Version"
        Me.lblProduct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblThanks
        '
        Me.lblThanks.Location = New System.Drawing.Point(141, 32)
        Me.lblThanks.Name = "lblThanks"
        Me.lblThanks.Size = New System.Drawing.Size(203, 128)
        Me.lblThanks.TabIndex = 6
        Me.lblThanks.Text = "Thanks"
        '
        'tmrTitleScrll
        '
        Me.tmrTitleScrll.Interval = 500
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.InitialImage = Nothing
        Me.PictureBox1.Location = New System.Drawing.Point(12, 32)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(120, 128)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox1.TabIndex = 7
        Me.PictureBox1.TabStop = False
        '
        'lblCopy
        '
        Me.lblCopy.AutoSize = True
        Me.lblCopy.Location = New System.Drawing.Point(9, 332)
        Me.lblCopy.Name = "lblCopy"
        Me.lblCopy.Size = New System.Drawing.Size(81, 13)
        Me.lblCopy.TabIndex = 8
        Me.lblCopy.Text = "(c) CD-PC 2014"
        '
        'frmInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(356, 354)
        Me.Controls.Add(Me.lblCopy)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.lblThanks)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.lblProduct)
        Me.Controls.Add(Me.lblComments)
        Me.Controls.Add(Me.btnBack)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmInfo"
        Me.Padding = New System.Windows.Forms.Padding(9)
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmInfo"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents btnBack As System.Windows.Forms.Button
    Friend WithEvents lblUsed As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents lblComments As System.Windows.Forms.Label
    Friend WithEvents lblProduct As System.Windows.Forms.Label
    Friend WithEvents lblThanks As System.Windows.Forms.Label
    Friend WithEvents lblSiteUrl As System.Windows.Forms.Label
    Friend WithEvents tmrTitleScrll As System.Windows.Forms.Timer
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblSite As System.Windows.Forms.Label
    Friend WithEvents lblCopy As System.Windows.Forms.Label

End Class
