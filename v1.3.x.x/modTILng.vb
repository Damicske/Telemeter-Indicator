Module modTILng
    Public ErrorMessage(14) As String
    Public ErrorMessageTitle(10) As String
    ' Public BoxMessage(10) As String
    Public OtherText(10) As String
    Public GraphText(6) As String

    Dim lngName As String, lngNameShort As String, last_update As String
    Public CurrentLng As String

    Public Sub load_lng(frmName As String)
        Select Case CurrentLng.ToLower
            Case "français"
                lng_fr(False, frmName)
            Case "english"
                lng_en(False, frmName)
            Case "deutsch"
                lng_de(False, frmName)
            Case Else
                lng_nl(False, frmName)
        End Select
    End Sub

    Private Sub lng_nl(bHeader As Boolean, frmName As String)
        If bHeader Then
            lngName = "Nederlands"
            lngNameShort = "nl"
            last_update = "29-07-2016"
            Exit Sub
        End If

        Select Case frmName
            Case "frmSettings"
                With frmSettings
                    .lblPassword.Text = "Paswoord"
                    .lblLanguage.Text = "Taal"
                    .lblUpdChk.Text = "Controleer op updates"
                    .btnSave.Text = "Opslaan"
                    .btnCancel.Text = "Sluit"
                    .mnuUpdate.ToolTipText = "Ververs de data"
                    .mnuGraph.Text = "Toon Tele&grafiek"
                    .mnuGraph.ToolTipText = "Toon de Telenet Telemeter verbruiksgrafiek"
                    .mnuLog.Text = "Toon &logboek"
                    .mnuLog.ToolTipText = "Toon het logboek"
                    .mnuMijnTelenet.Text = "&Mijn Telenet"
                    .mnuMijnTelenet.ToolTipText = "Ga naar Mijn Telenet"
                    .mnuMijnTelenetMention.Text = "Mijn Telenet melding"
                    .mnuMijnTelenetMention.ToolTipText = "Toon de laatste mijn Telenet melding"
                    .mnuMijnTelenetConnecties.Text = "Verbi&ndingen"
                    .mnuMijnTelenetConnecties.ToolTipText = "Toon de modem DHCP verbindingen"
                    .mnuSettings.Text = "Toon In&stellingen"
                    .mnuSettingsShowMyTelenet.Text = "Toon Mijn Telenet meldingen"
                    .mnuSettingsShowMyTelenet.ToolTipText = ""
                    .mnuSettingsShowBinair.Text = "Toon binaire eenheid"
                    .mnuSettingsShowBinair.ToolTipText = "Toon binaire eenheid (1024) of de SI eenheid (1000)"
                    .mnuSettingsDynIcon.Text = "&Dynamisch icoon"
                    .mnuSettingsDynIcon.ToolTipText = "Toon dynamisch icoon in systray"
                    .mnuSettingsSaveLog.Text = "Logboek opslaan als bestand"
                    .mnuSettingsSaveLog.ToolTipText = "Sla het logboek op in een bestand per dag"
                    .mnuSettingsSave.Text = "Verbruik opslaan"
                    .mnuSettingsSave.ToolTipText = "Sla je periodiek verbruik op"
                    .mnuSettingsStartup.Text = "Opstarten na Windows login"
                    .mnuSettingsStartup.ToolTipText = "Start mee op met Windows na dat je bent ingelogd"
                    .mnuHelp.Text = "&Help"
                    .mnuHelpSendBug.Text = "Verzend bug rapport"
                    .mnuHelpSendBug.ToolTipText = "Maak een bug rapport aan en verzend naar CD-PC"
                    .mnuHelpUpdate.Text = "Controleer op &update"
                    .mnuHelpUpdate.ToolTipText = "Controleer op een nieuwe versie"
                    .mnuHelpHistory.Text = "Gesc&hiedenis"
                    .mnuHelpHistory.ToolTipText = "Bekijk de geschiedenis van het programma"
                    .mnuHelpAbout.Text = "&Over ons..."
                    .mnuExit.Text = "&Einde"
                    .mnuExit.ToolTipText = "Beeïndig het programma"

                    .mnuSettingsDataSend.Text = "Zend Telenet pagina"
                    .mnuSettingsDataSend.ToolTipText = "Zend Telenet pagina data naar CD-PC"
                    .ToolTip1.SetToolTip(.tbInterval, "Sneller of trager op nieuwe gegevens controleren")
                    .ToolTip1.SetToolTip(.txtUserId, "Geef je Telenet ID op deze is 7 karakters lang en beginnende met een letter.")
                    .ToolTip1.SetToolTip(.mtbPassword, "Geef je Telenet paswoord in")
                    .ToolTip1.SetToolTip(.cbLanguage, "Verander de taal van het programma")
                    .ToolTip1.SetToolTip(.btnCancel, "Sluit dit venster")
                    .ToolTip1.SetToolTip(.btnSave, "Sla de wijzigingen op")
                    '.ToolTip1.SetToolTip(, "")

                    '-load new items and select the right one
                    With .cbUpdChk
                        .Items.Clear()
                        .Items.Add("Manueel")
                        .Items.Add("Dagelijks")
                        .Items.Add("Wekelijks")
                        .Items.Add("Maandelijks")
                        If CInt(My.Settings.UpdChk) < .Items.Count - 1 Then
                            .SelectedIndex = CInt(My.Settings.UpdChk)
                        Else
                            .SelectedIndex = 1
                        End If
                    End With
                End With

                ErrorMessage(0) = "Er is iets verkeerd gegaan, maar we weten niet wat, kijk in het logboek voor meer informatie"
                ErrorMessage(1) = "Login gefaald: kijk in het logboek voor meer informatie"
                ErrorMessage(2) = "Login disabled: controleer de site voor meer informatie"
                ErrorMessage(3) = ""
                ErrorMessage(4) = ""
                ErrorMessage(5) = "Denk er zeker aan om de betaling van je Telenet-aanrekening in orde te brengen.\n Zo vermijd je eventuele herinneringskosten."
                ErrorMessage(6) = "Geen data gevonden"
                ErrorMessage(7) = "Telenet is de mijn.Telenet aan het aanpassen."
                ErrorMessage(8) = "Controleer je Firewall instellingen"
                ErrorMessage(9) = "Controleer je ingegeven Telenet Id en bijbehorend paswoord"
                ErrorMessage(10) = "Wilt u je wijzigingen opslaan of niet?"
                ErrorMessage(11) = "Iets mis met de site, probeer later opnieuw"
                ErrorMessage(12) = "Er heeft zich een fout voorgedaan met het SSL Certificaat."
                ErrorMessage(13) = "Je opgegeven Telenet ID/paswoord is verkeerd, controleer het en probeer opnieuw"

                ErrorMessageTitle(0) = "Authenticatie gefaald"
                ErrorMessageTitle(1) = ""
                ErrorMessageTitle(2) = "Mijn Telenet Offline"
                ErrorMessageTitle(3) = ""
                ErrorMessageTitle(4) = "Telenet-geheugensteuntje"
                ErrorMessageTitle(5) = "Mijn Telenet werken"
                ErrorMessageTitle(6) = "Instellingen :: Telenet ID/Paswoord"
                ErrorMessageTitle(7) = "Instellingen :: Opslaan"
                ErrorMessageTitle(8) = "mijn.Telenet Error"
                ErrorMessageTitle(9) = "SSL gefaald"

                OtherText(0) = "Ververs &TeleData ({0})"
                OtherText(1) = "Aan het verversen..."
                OtherText(2) = "Verbruik: {0}\nDag verbruik: {1}"
                OtherText(3) = "Reset op {0} nog {1} dag(en)"
                OtherText(4) = "Interval ({0}min)"
                OtherText(5) = "Publieke ip adres\nV4: {0}\nV6: {1}\nV6Prefix: {2}"
                OtherText(6) = "Netwerkkabel"
                OtherText(7) = "Draadloos"

                GraphText(0) = "Basisvolume"
                GraphText(1) = "Extravolume"
                GraphText(2) = "Wi-Free verbruik"
                GraphText(3) = "Betaald verbruik"
                GraphText(4) = "Daluren (24u > 12u)"
                GraphText(5) = "Piekuren (12u > 24u)"
                GraphText(6) = "Piek:%1/Dal:%2"

            Case "frmGraph"
                With frmGraph
                    .Text = "Telegrafiek"
                    .mnuShowPay.Text = "Toon Betaald"
                    .mnuShowExtra.Text = "Toon Extra"
                    .mnuShowWifi.Text = "Toon Wi-free"
                    .mnuShowAvarage.Text = "Gemiddeld verbruik (rood)"
                    .mnuShowAvgCalc.Text = "Dag limiet (groen)"
                    .mnuShowLimit.Text = "Over limiet"
                    .mnuShowLimitDay.Text = "per dag"
                    .mnuPeriod.Text = "Periode"
                    .mnuPeriodThis.Text = "Deze periode"
                    ' .mnuPeriodThis.ToolTipText = ""
                End With
            Case "frmLog"
                frmLog.Text = "Logboek"

            Case "frmConnections"
                With frmConnections
                    .Text = "Verbindingen"
                    .ListView1.Columns(0).Text = "MAC Adres"
                    .ListView1.Columns(1).Text = "IPv4"
                    .ListView1.Columns(2).Text = "IPv6"
                    .ListView1.Columns(3).Text = "Naam"
                    .ListView1.Columns(4).Text = "Connectie"
                End With
        End Select
    End Sub

    Private Sub lng_en(bHeader As Boolean, frmName As String)
        If bHeader Then
            lngName = "English"
            last_update = "25-07-2016"
            Exit Sub
        End If
        Select Case frmName
            Case "frmSettings"
                With frmSettings
                    .lblPassword.Text = "Password"
                    .lblLanguage.Text = "Language"
                    .lblUpdChk.Text = "Check for updates"
                    .btnSave.Text = "Save"
                    .btnCancel.Text = "Close"
                    .mnuUpdate.ToolTipText = "Update the data"
                    .mnuGraph.Text = "Show Tele&graph"
                    .mnuGraph.ToolTipText = "Show the Telenet Telemeter usage graph"
                    .mnuLog.Text = "Show &logbook"
                    .mnuLog.ToolTipText = "Show logbook"
                    .mnuMijnTelenet.Text = "&My Telenet"
                    .mnuMijnTelenet.ToolTipText = "Goto My Telenet"
                    .mnuMijnTelenetMention.Text = "My Telenet mention"
                    .mnuMijnTelenetMention.ToolTipText = "Show the last My Telenet mentioning"
                    .mnuMijnTelenetConnecties.Text = "Co&nnections"
                    .mnuMijnTelenetConnecties.ToolTipText = "Show the modem DHCP connections"
                    .mnuSettings.Text = "Show &Settings"
                    .mnuSettingsShowMyTelenet.Text = "Show My Telenet announcements"
                    .mnuSettingsShowMyTelenet.ToolTipText = ""
                    .mnuSettingsShowBinair.Text = "Show binaire units"
                    .mnuSettingsShowBinair.ToolTipText = "Show the binaire units (1024) or the SI units (1000)"
                    .mnuSettingsDynIcon.Text = "&Dynamic icon"
                    .mnuSettingsDynIcon.ToolTipText = "Show dynamic icon in systray"
                    .mnuSettingsSaveLog.Text = "Save Logbook as a file"
                    .mnuSettingsSaveLog.ToolTipText = "Save the logbook as a day file"
                    .mnuSettingsSave.Text = "Sa&ve usage"
                    .mnuSettingsSave.Text = "Save usage"
                    .mnuSettingsSave.ToolTipText = "Save your periodic usage in a file"
                    .mnuSettingsStartup.Text = "Startup after Windows logon"
                    .mnuSettingsStartup.ToolTipText = "Startup after you logon"
                    .mnuSettingsDataSend.Text = "Send Telenet page"
                    .mnuSettingsDataSend.ToolTipText = "Send Telenet page data to CD-PC"
                    .mnuHelp.Text = "&Help"
                    .mnuHelpSendBug.Text = "Send bug rapport"
                    .mnuHelpSendBug.ToolTipText = "Create a bug rapport send to creator"
                    .mnuHelpUpdate.Text = "Check for an &update"
                    .mnuHelpUpdate.ToolTipText = "Check for a new version"
                    .mnuHelpHistory.Text = "&History"
                    .mnuHelpHistory.ToolTipText = "View the history of the program"
                    .mnuHelpAbout.Text = "Ab&out us..."
                    .mnuExit.Text = "&Exit"
                    .mnuExit.ToolTipText = "Exit the program"
                    .ToolTip1.SetToolTip(.tbInterval, "Check faster or slower on new data")
                    .ToolTip1.SetToolTip(.txtUserId, "Enter your Telenet ID on this is 7 characters long, starting with a letter")
                    .ToolTip1.SetToolTip(.mtbPassword, "Enter your Telenet password in")
                    .ToolTip1.SetToolTip(.cbLanguage, "Change the language of the program")
                    .ToolTip1.SetToolTip(.btnCancel, "Close this window")
                    .ToolTip1.SetToolTip(.btnSave, "Save your changes")
                    '.ToolTip1.SetToolTip(, "")

                    '-load new items and select the right one
                    With .cbUpdChk
                        .Items.Clear()
                        .Items.Add("Manual")
                        .Items.Add("Daily")
                        .Items.Add("Weekly")
                        .Items.Add("Monthly")
                        If CInt(My.Settings.UpdChk) < .Items.Count - 1 Then
                            .SelectedIndex = CInt(My.Settings.UpdChk)
                        Else
                            .SelectedIndex = 1
                        End If
                    End With
                End With
                ErrorMessage(0) = "Something went wrong, but we don't know what, look in the logbook for more information"
                ErrorMessage(1) = "Login failed: look in the logbook for more information"
                ErrorMessage(2) = "Login disabled: check the site for more information"""
                ErrorMessage(3) = "Your Telenet ID is wrong, check and change and try again"
                ErrorMessage(4) = "Your password is wrong, check and change and try again"
                ErrorMessage(5) = "Do be sure to bring the payment of your Telenet invoice in order.\n This will avoid any reminder costs."
                ErrorMessage(6) = "No usable data found"
                ErrorMessage(7) = "Telenet is updating the mijn.Telenet pages."
                ErrorMessage(8) = "Check your Firewall settings"
                ErrorMessage(9) = "Check your Telenet ID and password"
                ErrorMessage(10) = "Do you want to save the changes or not?"
                ErrorMessage(11) = "Something wrong with the site, try again later"
                ErrorMessage(12) = "There has been an error occurred with the SSL Certificate."
                ErrorMessage(13) = "Your Telenet ID/password is wrong, check and change and try again"

                ErrorMessageTitle(0) = "Authentication failed"
                ErrorMessageTitle(1) = ""
                ErrorMessageTitle(2) = "My Telenet Offline"
                ErrorMessageTitle(3) = ""
                ErrorMessageTitle(4) = "Telenet mnemonic"
                ErrorMessageTitle(5) = "My Telenet work"
                ErrorMessageTitle(6) = "Settings :: Telenet ID/Password"
                ErrorMessageTitle(7) = "Settings :: Save settings"
                ErrorMessageTitle(8) = "mijn.Telenet Error"
                ErrorMessageTitle(9) = "SSL failed"

                OtherText(0) = "Refresh &TeleData ({0})"
                OtherText(1) = "Refreshing..."
                OtherText(2) = "Usage: {0}\nDaily usage over: {1}"
                OtherText(3) = "Reset on {0} yet {1} day(s)"
                OtherText(4) = "Interval ({0}min)"
                OtherText(5) = "Public ip adress\nV4: {0}\nV6: {1}\nV6Prefix: {2}"
                OtherText(6) = "Networkcable"
                OtherText(7) = "Wireless"

                GraphText(0) = "Basic volume"
                GraphText(1) = "Extra volume"
                GraphText(2) = "Wi-Free usage"
                GraphText(3) = "Paid usage"
                GraphText(4) = "Off-peak hours (24h > 12h)"
                GraphText(5) = "Peak hours  (12h > 24h)"
                GraphText(6) = "Peak:%1/Off-peak:%2"

            Case "frmGraph"
                With frmGraph
                    .Text = "Telegraph"
                    .mnuShowPay.Text = "Show Paid"
                    .mnuShowExtra.Text = "Show Extra"
                    .mnuShowWifi.Text = "Show Wi-free"
                    .mnuShowAvarage.Text = "Average usage (red)"
                    .mnuShowAvgCalc.Text = "Day limit (green)"
                    .mnuShowLimit.Text = "Over limit"
                    .mnuShowLimitDay.Text = "per day"
                    .mnuPeriod.Text = "Period"
                    .mnuPeriodThis.Text = "This period"
                End With

            Case "frmLog"
                frmLog.Text = "Logbook"

            Case "frmConnections"
                With frmConnections
                    .Text = "Connections"
                    .ListView1.Columns(0).Text = "MAC Adress"
                    .ListView1.Columns(1).Text = "IPv4"
                    .ListView1.Columns(2).Text = "IPv6"
                    .ListView1.Columns(3).Text = "Name"
                    .ListView1.Columns(4).Text = "Connection"
                End With
        End Select
    End Sub

    Private Sub lng_fr(bHeader As Boolean, frmName As String)
        If bHeader Then
            lngName = "Français"
            last_update = "29-07-2016"
            Exit Sub
        End If
        Select Case frmName
            Case "frmSettings"
                With frmSettings
                    .lblPassword.Text = "Mot de passe"
                    .lblLanguage.Text = "langue"
                    .lblUpdChk.Text = "Vérifier les mises à jour"
                    .btnSave.Text = "magasin"
                    .btnCancel.Text = "Près"
                    .mnuUpdate.ToolTipText = "Changer les dates"
                    .mnuGraph.Text = "Afficher Tele &Graphique"
                    .mnuGraph.ToolTipText = "Voir le graphique d'utilisation Telenet Telemeter"
                    .mnuLog.Text = "Voir le journa&l"
                    .mnuLog.ToolTipText = "Afficher le journal"
                    .mnuMijnTelenet.Text = "&Mon Telenet"
                    .mnuMijnTelenet.ToolTipText = "Allez àMon Telenet"
                    .mnuMijnTelenetMention.Text = "Mon rapport Telenet"
                    .mnuMijnTelenetMention.ToolTipText = "Montrer dernière mon rapport Telenet"
                    .mnuMijnTelenetConnecties.Text = "Co&nnexions"
                    .mnuMijnTelenetConnecties.ToolTipText = "Afficher les connexions modem DHCP"
                    .mnuSettings.Text = "Afficher les paramètre&s"
                    .mnuSettingsShowMyTelenet.Text = "Rapports montrent My Telenet"
                    .mnuSettingsShowMyTelenet.ToolTipText = ""
                    .mnuSettingsShowBinair.Text = "Afficher unité binaire"
                    .mnuSettingsShowBinair.ToolTipText = "Afficher unité binaire (1024) ou de l'unité SI (1000)"
                    .mnuSettingsDynIcon.Text = "icône &dynamique"
                    .mnuSettingsDynIcon.ToolTipText = "Afficher l'icône systray dynamique"
                    .mnuSettingsSaveLog.Text = "Enregistrer le journal sous forme de fichier"
                    .mnuSettingsSaveLog.ToolTipText = "Enregistrez le journal dans un fichier par jour"
                    .mnuSettingsSave.Text = "enregistrer utilisation"
                    .mnuSettingsSave.ToolTipText = "Sauvegardez votre utilisation de période dans un fichier"
                    .mnuSettingsStartup.Text = "Après le démarrage de Windows connexion"
                    .mnuSettingsStartup.ToolTipText = "Lancer sur Windows après vous êtes connecté"
                    .mnuSettingsDataSend.Text = "Envoyer cette page Telenet"
                    .mnuSettingsDataSend.ToolTipText = "Envoyer Telenet pages données sur CD-PC"
                    .mnuHelp.Text = "&Aidez-Moi"
                    .mnuHelpSendBug.Text = "Envoyer un rapport de bug"
                    .mnuHelpSendBug.ToolTipText = "Créer un rapport de bogue et envoyez-moi"
                    .mnuHelpUpdate.Text = "Vérifier les mises à jo&ur"
                    .mnuHelpUpdate.ToolTipText = "Vérifiez nouvelle version"
                    .mnuHelpHistory.Text = "&Histoire"
                    .mnuHelpHistory.ToolTipText = "Voir l'historique du programme"
                    .mnuHelpAbout.Text = "À pr&opos de nous..."
                    .mnuExit.Text = "&fin"
                    .mnuExit.ToolTipText = "Terminez le programme"
                    .ToolTip1.SetToolTip(.tbInterval, "Consultez vite ou plus lentement sur les nouvelles données")
                    .ToolTip1.SetToolTip(.txtUserId, "Entrez votre ID Telenet sur ce est de 7 caractères de long, en commençant par une lettre.")
                    .ToolTip1.SetToolTip(.mtbPassword, "Entrez votre mot de passe dans Telenet")
                    .ToolTip1.SetToolTip(.cbLanguage, "Changer la langue du programme")
                    .ToolTip1.SetToolTip(.btnCancel, "Fermez cette fenêtre")
                    .ToolTip1.SetToolTip(.btnSave, "Enregistrer les modifications")
                    '.ToolTip1.SetToolTip(, "")

                    '-load new items and select the right one
                    With .cbUpdChk
                        .Items.Clear()
                        .Items.Add("Manuellement")
                        .Items.Add("Chaque jour")
                        .Items.Add("Chaque semaine")
                        .Items.Add("Mensuel")
                        If CInt(My.Settings.UpdChk) < .Items.Count - 1 Then
                            .SelectedIndex = CInt(My.Settings.UpdChk)
                        Else
                            .SelectedIndex = 1
                        End If
                    End With
                End With
                ErrorMessage(0) = "Quelque chose a mal tourné, mais nous ne savons pas, regardez dans le journal pour plus d'informations"
                ErrorMessage(1) = "Échec de la connexion: Regardez dans le journal pour plus d'informations"
                ErrorMessage(2) = "Connectez-vous désactivé: vérifier le site pour plus d'informations"
                ErrorMessage(3) = "Telenet ID que vous avez entré est incorrect, vérifier et essayer à nouveau"
                ErrorMessage(4) = "Vous avez entré un mot de passe est incorrect, vérifier et essayer à nouveau"
                ErrorMessage(5) = "Pensez-vous d'apporter le paiement de votre facture Telenet dans l'ordre.\n Cela évite des frais de rappel."
                ErrorMessage(6) = "Rien na été trouvé"
                ErrorMessage(7) = "Telenet ajuster mijn.Telenet."
                ErrorMessage(8) = "Vérifiez vos paramètres de pare-feu"
                ErrorMessage(9) = "Assurez-vous que vous avez entré Telenet ID et mot de passe associé"
                ErrorMessage(10) = "Voulez-vous enregistrer les modifications ou non?"
                ErrorMessage(11) = "Quelque chose de mal avec le site, s'il vous plaît réessayer plus tard"
                ErrorMessage(12) = "Il y avait eu une erreur avec le certificat SSL."
                ErrorMessage(13) = "Telenet ID/un mot de passe que vous avez entré est incorrect, vérifier et essayer à nouveau"

                ErrorMessageTitle(0) = "L'authentification a échoué"
                ErrorMessageTitle(1) = ""
                ErrorMessageTitle(2) = "Mon Telenet Offline"
                ErrorMessageTitle(3) = ""
                ErrorMessageTitle(4) = "Telenet mnémonique"
                ErrorMessageTitle(5) = "Mes œuvres Telenet"
                ErrorMessageTitle(6) = "Réglages :: Telenet ID / Mot de passe"
                ErrorMessageTitle(7) = "Réglages :: Sauvegarde des données"
                ErrorMessageTitle(8) = "Erreur mijn.Telenet"
                ErrorMessageTitle(9) = "SSL a échoué"

                OtherText(0) = "Actualiser &Teledata ({0})"
                OtherText(1) = "Pour actualiser..."
                OtherText(2) = "consommation: {0}\nJournée sur la consommation: {1}"
                OtherText(3) = "Réinitialiser {0} encore {1} jour(s)"
                OtherText(4) = "intervalle ({0}min)"
                OtherText(5) = "adresse IP publique\nV4: {0}\nV6: {1}\nV6Prefix: {2}"
                OtherText(6) = "Réseau"
                OtherText(7) = "Réseau sans fil"

                GraphText(0) = "Volume de base"
                GraphText(1) = "Volume d'extension"
                GraphText(2) = "Utilisation de Wi-Free"
                GraphText(3) = "Utilisation payés"
                GraphText(4) = "Les heures creuses (24h > 12h)"
                GraphText(5) = "Les heures de pointe  (12h > 24h)"
                GraphText(6) = "de pointe:%1/creuses:%2"

            Case "frmGraph"
                With frmGraph
                    .Text = "Télécharger Graphique"
                    .mnuShowPay.Text = "Afficher payé"
                    .mnuShowExtra.Text = "Afficher supplémentaire"
                    .mnuShowWifi.Text = "Afficher Wi-free"
                    .mnuShowAvarage.Text = "Afficher consommation moyenne (Rouge)"
                    .mnuShowAvgCalc.Text = "La limite quotidienne. (vert)"
                    .mnuShowLimit.Text = "Dessus de la limite"
                    .mnuShowLimitDay.Text = "par jour"
                    .mnuPeriod.Text = "Période"
                    .mnuPeriodThis.Text = "Cette période"
                End With

            Case "frmLog"
                frmLog.Text = "le journal"

            Case "frmConnections"
                With frmConnections
                    .Text = "Connexions"
                    .ListView1.Columns(0).Text = "MAC Adresse"
                    .ListView1.Columns(1).Text = "IPv4"
                    .ListView1.Columns(2).Text = "IPv6"
                    .ListView1.Columns(3).Text = "Nom"
                    .ListView1.Columns(4).Text = "Connexion"
                End With
        End Select
    End Sub

    Private Sub lng_de(bHeader As Boolean, frnName As String)
        If bHeader Then
            lngName = "Deutsch"
            last_update = "29-07-2016"
        End If
        Select Case frnName
            Case "frmSettings"
                With frmSettings
                    .lblPassword.Text = "Passwort"
                    .lblLanguage.Text = "Sprache"
                    .lblUpdChk.Text = "Nach Updates suchen"
                    .btnSave.Text = "Speicher"
                    .btnCancel.Text = "in der Nähe"
                    .mnuUpdate.ToolTipText = "Ändern Sie die Daten"
                    .mnuGraph.Text = "Anzeigen Tele &Graph"
                    .mnuGraph.ToolTipText = "Lassen Sie sich die Telenet Telemeter Verbrauchstabelle"
                    .mnuLog.Text = "Protoko&lle anzeigen"
                    .mnuLog.ToolTipText = "Protoko&lle anzeigen"
                    .mnuMijnTelenet.Text = "&Meine Telenet"
                    .mnuMijnTelenet.ToolTipText = "Gehen Sie zu Mein Telenet"
                    .mnuMijnTelenetMention.Text = "Meine Telenet Bericht"
                    .mnuMijnTelenetMention.ToolTipText = "Zeige die letzten meiner Telenet Bericht"
                    .mnuMijnTelenetConnecties.Text = "Verbi&ndungen"
                    .mnuMijnTelenetConnecties.ToolTipText = "Die Modem-DHCP-Verbindungen anzeigen"
                    .mnuSettings.Text = "Toon In&stellingen"
                    .mnuSettingsShowMyTelenet.Text = "Zeigen Sie Ihr Telenet Berichte"
                    .mnuSettingsShowMyTelenet.ToolTipText = ""
                    .mnuSettingsShowBinair.Text = "Zeigen binäre Einheit"
                    .mnuSettingsShowBinair.ToolTipText = "Zeige binäre Einheit (1024) oder die SI-Einheit (1000)"
                    .mnuSettingsDynIcon.Text = "&Dynamische Symbol"
                    .mnuSettingsDynIcon.ToolTipText = "Zeigen Sie dynamische Symbol im Systray"
                    .mnuSettingsSaveLog.Text = "Protokoll als Datei speichern"
                    .mnuSettingsSaveLog.ToolTipText = "Speichern Sie das Protokoll in einer Datei pro Tag"
                    .mnuSettingsSave.Text = "Speichern Verbrauch"
                    .mnuSettingsSave.ToolTipText = "Speichern Sie Ihre regelmäßige Verzehr"
                    .mnuSettingsStartup.Text = "Nach dem Start der Windows-Anmeldung"
                    .mnuSettingsStartup.ToolTipText = "Starten Sie unter Windows nach dem Sie angemeldet sind"
                    .mnuSettingsDataSend.Text = "senden Telenet Seite"
                    .mnuSettingsDataSend.ToolTipText = "senden Telenet Seite Daten auf CD-PC"
                    .mnuHelp.Text = "&Hilfe"
                    .mnuHelpSendBug.Text = "Fehlerbericht senden"
                    .mnuHelpSendBug.ToolTipText = "Erstellen Sie einen Fehlerbericht und senden Sie mir"
                    .mnuHelpUpdate.Text = "Auf &Updates prüfen"
                    .mnuHelpUpdate.ToolTipText = "Auf neue Version prüfen"
                    .mnuHelpHistory.Text = "Geschic&hte"
                    .mnuHelpHistory.ToolTipText = "Sehen Sie sich die Geschichte des Programms"
                    .mnuHelpAbout.Text = "&über uns..."
                    .mnuExit.Text = "&Ende"
                    .mnuExit.ToolTipText = "Beenden Sie das Programm"
                    .ToolTip1.SetToolTip(.tbInterval, "Überprüfen Sie schneller oder langsamer auf neue Daten")
                    .ToolTip1.SetToolTip(.txtUserId, "Geben Sie Ihre Telenet ID auf diesem ist 7 Zeichen, beginnend mit einem Buchstaben")
                    .ToolTip1.SetToolTip(.mtbPassword, "Geben Sie Ihr Telenet Passwort in")
                    .ToolTip1.SetToolTip(.cbLanguage, "Ändern Sie die Sprache des Programms")
                    .ToolTip1.SetToolTip(.btnCancel, "Dieses Fenster schließen")
                    .ToolTip1.SetToolTip(.btnSave, "Änderungen speichern")
                    '.ToolTip1.SetToolTip(, "")

                    '-load new items and select the right one
                    With .cbUpdChk
                        .Items.Clear()
                        .Items.Add("Manuell")
                        .Items.Add("Täglich")
                        .Items.Add("wöchentlich")
                        .Items.Add("Monatlich")
                        If CInt(My.Settings.UpdChk) < .Items.Count - 1 Then
                            .SelectedIndex = CInt(My.Settings.UpdChk)
                        Else
                            .SelectedIndex = 1
                        End If
                    End With
                End With

                ErrorMessage(0) = "Da lief was falsch, aber wir wissen nicht, suchen Sie im Protokoll für weitere Informationen"
                ErrorMessage(1) = "Fehler bei der Anmeldung: Sehen Sie in der Log für weitere Informationen"
                ErrorMessage(2) = "Melden deaktiviert: prüfen die site für weitere Informationen"
                ErrorMessage(3) = "Sie spezifiziert Telenet ID falsch ist, überprüfen sie und versuchen Sie es erneut"
                ErrorMessage(4) = "Eingegebene Passwort falsch ist, überprüfen es bitte und versuchen es noch einmal"
                ErrorMessage(5) = "Denken Sie, dass die Zahlung Ihrer Rechnung Telenet in Ordnung zu bringen.\nDies vermeidet Mahnkosten."
                ErrorMessage(6) = "Keine Einträge gefunden"
                ErrorMessage(7) = "Telenet ist die mijn.Telenet Seiten anpassen."
                ErrorMessage(8) = "Überprüfen Sie Ihre Firewall-Einstellungen."
                ErrorMessage(9) = "Achten Sie darauf, von Ihnen eingegebenen Telenet-ID und das zugehörige Passwort."
                ErrorMessage(10) = "Möchten Sie die Änderungen speichern oder nicht?"
                ErrorMessage(11) = "Etwas falsch mit der Website, bitte versuchen Sie es später noch einmal."
                ErrorMessage(12) = "Es gab einen Fehler mit dem SSL-Zertifikat aufgetreten."
                ErrorMessage(13) = "Sie spezifiziert Telenet ID/Passwort falsch ist, überprüfen sie und versuchen Sie es erneut"

                ErrorMessageTitle(0) = "Authentifizierung fehlgeschlagen"
                ErrorMessageTitle(1) = ""
                ErrorMessageTitle(2) = "Meine Telenet nicht verfügbar"
                ErrorMessageTitle(3) = ""
                ErrorMessageTitle(4) = "Telenet mnemonic"
                ErrorMessageTitle(5) = "Meine Telenet Werke"
                ErrorMessageTitle(6) = "Einstellungen :: Telenet ID / Passwort"
                ErrorMessageTitle(7) = "Einstellungen :: speichern"
                ErrorMessageTitle(8) = "mijn.Telenet Felher"
                ErrorMessageTitle(9) = "SSL fehlgeschlagen"

                OtherText(0) = "Aktualisieren &Teledata ({0})"
                OtherText(1) = "So aktualisieren..."
                OtherText(2) = "Verbrauch: {0}\nTagesverbrauch über: {1}"
                OtherText(3) = "Zurücksetzen {0} noch {1} Tag(en)"
                OtherText(4) = "Intervall ({0}min)"
                OtherText(5) = "Öffentliche IP-Adresse\nV4: {0}\nV6: {1}\nV6Prefix: {2}"
                OtherText(6) = "Netzewerk"
                OtherText(7) = "Wireless"

                GraphText(0) = "Grundlautstärke"
                GraphText(1) = "Extra Verbrauch"
                GraphText(2) = "Wi-Free Verbrauch"
                GraphText(3) = "bezahlte Verbrauch"
                GraphText(4) = "Stoßzeiten (24h > 12h)"
                GraphText(5) = "Spitzenzeiten (12u > 24u)"
                GraphText(6) = "Spitzenzeiten:%1/Stoßzeiten:%2"

            Case "frmgraph"
                With frmGraph
                    .Text = "Tele Graph"
                    .mnuShowPay.Text = "Bezahlte anzeigen"
                    .mnuShowExtra.Text = "Bezahlte Extra"
                    .mnuShowWifi.Text = "Bezahlte Wi-free"
                    .mnuShowAvarage.Text = "Zeigen Durchschnittsverbrauch (rot)"
                    .mnuShowAvgCalc.Text = "Tageslimit (grün)"
                    .mnuShowLimit.Text = "Über dem Limit"
                    .mnuShowLimitDay.Text = "pro Tag"
                    .mnuPeriod.Text = "Zeitraum"
                    .mnuPeriodThis.Text = "Diezer Zeitraum"
                End With

            Case "frmLog"
                frmLog.Text = "Protokolle"

            Case "frmConnections"
                With frmConnections
                    .Text = "Verbindungen"
                    .ListView1.Columns(0).Text = "MAC Adresse"
                    .ListView1.Columns(1).Text = "IPv4"
                    .ListView1.Columns(2).Text = "IPv6"
                    .ListView1.Columns(3).Text = "Name"
                    .ListView1.Columns(4).Text = "Verbindung"
                End With
        End Select
    End Sub
End Module
