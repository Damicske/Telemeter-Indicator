Public NotInheritable Class frmInfo
    Private i As Int32, lNormalColor As System.Drawing.Color
    Private Path As String

    Private myFont As New Font("Microsoft Sans Serif", 9, FontStyle.Regular)
    Private myFontBold As New Font("Microsoft Sans Serif", 9, FontStyle.Bold)

    Enum eLang
        English = 0
        Nederlands = 1
        Français = 2
        Deutsch = 3
    End Enum
    Public Language As eLang = eLang.english

    Private Sub frmInfo_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        On Error Resume Next
        Select Case VersionType
            Case 1
                Select Case Language
                    Case eLang.Nederlands
                        lblStatus.Text = "Status: Gratis"
                    Case eLang.Deutsch
                        lblStatus.Text = "Status: kostenlos"
                    Case eLang.Français
                        lblStatus.Text = "Statut: gratuit"
                    Case Else
                        lblStatus.Text = "Status: Freeware"
                End Select
            Case 2
                lblStatus.Text = "Status: TEST"
                Select Case Language
                    Case eLang.Nederlands
                        lblStatus.Text = "Status: TEST"
                    Case eLang.Deutsch
                        lblStatus.Text = "Status: TEST"
                    Case eLang.Français
                        lblStatus.Text = "Statut: TEST"
                    Case Else
                        lblStatus.Text = "Status: TEST"
                End Select
            Case 3
                Select Case Language
                    Case eLang.Nederlands
                        lblStatus.Text = "Status: Betaald"
                    Case eLang.Deutsch
                        lblStatus.Text = "Status: bezahlt"
                    Case eLang.Français
                        lblStatus.Text = "Statut: payé"
                    Case Else
                        lblStatus.Text = "Status: Payed"
                End Select
        End Select

        Select Case Language
            Case eLang.Nederlands
                lblVersion.Text = "Versie: " & VersionX(True).ToString
                lblThanks.Text = "Dank U voor het gebruik van dit programma." &
                "Als er een update is of je wilt iets zeggen van het programma, AUB mail mij of zie eens op de CD-PC site." & Environment.NewLine &
                "De CD-PC crew kan niet verantwoordelijk gesteld worden voor PC crashes en dergelijke te wijten van slecht werkende programma's!"
                lblUsed.Text = "Gebruikt: " & My.Settings.Use & " keer"
            Case eLang.Français
                lblVersion.Text = "version: " & VersionX(True).ToString
                lblThanks.Text = "Merci d 'utiliser ce programme." &
                "Si une mise à jour est si vous voulez dire quelque chose au programme, s'il vous plaît écrivez-moi ou eyeful sur le site CD-PC." & Environment.NewLine &
                "L'équipage CD PC ne peut pas être tenu pour responsable des accidents de PC et autres grâce à des programmes qui fonctionnent mal!"
                lblUsed.Text = "Utilisé " & My.Settings.Use & " fois"
            Case eLang.Deutsch
                lblVersion.Text = "Version: " & VersionX(True).ToString
                lblThanks.Text = "Vielen Dank für dieses Programm." &
                "Wenn ein Update, wenn Sie etwas, um das Programm zu sagen wollen, bitte mich oder eyeful auf der CD-PC-Website Mail." & Environment.NewLine &
                "Die Crew CD PC kann nicht haftbar gemacht werden für PC-Abstürze und dergleichen aufgrund einer Fehlfunktion Programme!"
                lblUsed.Text = "Gebraucht: " & My.Settings.Use & " mal."
            Case Else
                lblVersion.Text = "Version: " & VersionX(True).ToString
                lblThanks.Text = "Thank you for the use of this program." &
                "If there is a update or you want to place a comment, please mail me or see my site." & Environment.NewLine &
                "The CD-PC crew can't be responsible for PC crashes and other stuff from bad working programs!"
                lblUsed.Text = "Used: " & My.Settings.Use & " times"
        End Select
        Me.Text = "Info [3.1]"
        lNormalColor = lblSiteUrl.ForeColor
    End Sub

    Private Sub frmInfo_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        GC.Collect()
    End Sub

    Private Sub frmInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim Date2 As Integer = Date.Today.Year
        If Date2 < 2016 Then Date2 = 2016 'year of last change
        lblProduct.Text = My.Application.Info.Title
        If lblProduct.Width > Me.ClientSize.Width Then
            tmrTitleScrll.Enabled = True
        Else
            lblProduct.Left = CInt(Me.ClientSize.Width / 2 - lblProduct.Width / 2)
        End If
        lblComments.Text = My.Application.Info.Description
        lblCopy.Text = Chr(169) & " CD-PC 2000 - " & Date2.ToString
        lblCopy.Left = CInt(Me.ClientSize.Width / 2 - lblCopy.Width / 2)
    End Sub

    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Me.Close()
    End Sub

    Private Sub lblSiteUrl_Click(sender As Object, e As EventArgs) Handles lblSiteUrl.Click
        System.Diagnostics.Process.Start("http://www.cd-pc.be")
    End Sub

    Private Sub lblSiteUrl_MouseHover(sender As Object, e As EventArgs) Handles lblSiteUrl.MouseHover
        CType(sender, Label).Font = myFontBold
        lblSiteUrl.ForeColor = System.Drawing.Color.Red
    End Sub

    Private Sub lblSiteUrl_MouseLeave(sender As Object, e As EventArgs) Handles lblSiteUrl.MouseLeave
        lblSiteUrl.ForeColor = lNormalColor
        CType(sender, Label).Font = myFont
    End Sub

    Private Sub tmrTitleScrll_Tick(sender As Object, e As EventArgs) Handles tmrTitleScrll.Tick
        If lblProduct.Left <= -lblProduct.Width Then
            lblProduct.Left = Me.ClientSize.Width
        End If
        lblProduct.Left = lblProduct.Left - 58 'HScroll2.Value
    End Sub

End Class
