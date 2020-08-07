Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Security
Imports System.Text
Imports System.Security
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Threading
Imports System.Configuration

'-----------------------------------------------------------
'* First code based on Telemeter-Indicator by Alwin Roosen *
'-----------------------------------------------------------
Public Class frmSettings
    Inherits Form
    Public bUnload As Boolean = False

    '-webrequest data
    Private tempCookies As New CookieContainer
    Private encoding As New UTF8Encoding
    Private sResponseUrl As String = ""

    '- other things
    Private Structure strBgwError
        Dim Title As String
        Dim Icon As ToolTipIcon
        Dim Message As String
    End Structure

    Private sBGWStatus As String = 0
    '-if Mijn Telenet anounced a maintenance
    Private sMaintenance As String = "", MaintenanceDateStop As Date, MaintenanceDateStart As Date, TrayIconText As String = ""
    Private BgwError As strBgwError
    Private bTimeOut As Boolean = False, form_valid As Boolean, bStatus As Boolean = False, bSave As Boolean = False, bMijnTelenet As Boolean = False, bMaintenance As Boolean = False, bError As Boolean = False, bSaveBox As Boolean = False, bballoon As Boolean = False
    Private iTimerInterval As Int32 = 0 'interval in seconds
    Private iLastCycli As Int64 = 0 'time when last cycli is started
    Private LastSaveDate As Int16 = CInt(Date.UtcNow.ToString("dd"))

    '-Bug rapport vars
    Public bTeleUp As Boolean = False
    Public iBugId As Int32

    '-Various Uri 's for the Telenet site
    Private Const URI_T As String = "telenet.be"
    '-Various Uri 's for the Telenet 'Mijn Telenet' web portal
    Private Const URI_MT_PREFIX As String = "https://mijn." & URI_T & "/mijntelenet/"
    Private Const URI_MT_NAVIGATION As String = URI_MT_PREFIX + "navigation/navigation.do"
    Private Const URI_MT_LOGIN As String = URI_MT_PREFIX + "login/login.do"
    Private Const URI_MT_SSO As String = URI_MT_PREFIX + "session/sso.do"
    Private URI_MT_USAGE As String = ""
    Private Const URI_MT_BILLING As String = URI_MT_PREFIX + "billing/billingOverview.do?accountNumber="
    '-Various Uri 's for the Telenet 'SSO' pages
    Private Const URI_SSO_PREFIX As String = "https://login.prd." & URI_T & "/openid/"
    Private Const URI_SSO_LOGIN As String = URI_SSO_PREFIX + "login"
    Private Const URI_SSO_NOTIY As String = URI_SSO_PREFIX + "notifyEvent.do"
    Private Const URI_SSO_SIGNON As String = URI_SSO_PREFIX + "login.do"
    Private URI_SSO_SIGNOFF As String = URI_SSO_PREFIX + "signoff.do"
    '-SSL data
    Private Const trustedIssuer_telenet As String = "CN=GlobalSign Organization Validation CA - SHA256 - G2"
    Private Const trustedDomain_telenet As String = "CN=*.prd.telenet.be"
    Private Const trustedIssuer_cdpc As String = "CN=Let's Encrypt Authority X3"
    Private Const trustedDomain_cdpc As String = "CN=cd-pc.be"

    '-AadvancedSettings data
    Private sIpWanV4 As String = ""
    Private sIpWanV6 As String = ""
    Private sIpWanV6Prefix As String = ""
    Private sConDevices() As String

    '-threading
    Private BGWthr As Thread = New Thread(AddressOf BGWthrTask)

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If btnSave.Enabled Then
            btnSave.Enabled = False
            '-reload old values
            load_mysettings()
        End If
        Me.Close()
    End Sub

    Private Sub tmrUpdate_Tick(sender As Object, e As EventArgs) Handles tmrUpdate.Tick
        '-check if there is anythin in the logbook
        If sLog.Length > 0 Then Me.mnuLog.Enabled = True

        '-check last saved date
        If mnuSettingsSaveLog.Checked Then
            If Date.Now.ToString("dd") <> LastSaveDate Then
                SaveLog(LastSaveDate)
                sLog = ""
                LastSaveDate = CInt(Date.Now.ToString("dd"))
            End If
        End If

        '-check if the program is updating or not
        If Not bUpdating And Not mnuHelpUpdate.Enabled Then
            mnuHelpUpdate.Enabled = True
        End If
		
        '-check if save button is enabled else if updates when your changing settings = not GOOOOOOOD
        If btnSave.Enabled = True Then
			If bSaveBox Then Exit Sub
            bSaveBox = True
            If MsgBox(ErrorMessage(10), MsgBoxStyle.Information + MsgBoxStyle.YesNo, ErrorMessageTitle(7)) = MsgBoxResult.Yes Then
                btnSave_Click(Me, e)
            Else
                Exit Sub
            End If
        End If

        '-if last hours of periode, change iTimerInterval so it updates before the next day
        If mnuSettingsSave.Checked And (pData.TeleReset = CDate(Date.UtcNow.ToString("dd/MM/yyyy"))) Then
            Dim iMinutes As Int16 = CInt(Date.UtcNow.ToString("mm")) + ((tbInterval.Value * 6 - iTimerInterval) \ 6)
            If CInt(Date.UtcNow.ToString("HH")) = 23 And iMinutes > 59 Then
                iTimerInterval = 6 * (iMinutes - 59)
            End If
        End If

        '-check if a update is required or not
        If iTimerInterval < (tbInterval.Value * 6) Then
            If iTimerInterval >= 3 Then mnuUpdate.Enabled = True
            iTimerInterval += 1
            Dim itmp As Int16 = (tbInterval.Value * 6 - iTimerInterval) \ 6
            If itmp = 0 Then
                itmp = (tbInterval.Value * 6 - iTimerInterval) + 1 * 10
                mnuUpdate.Text = OtherText(0).Replace("{0}", itmp & "sec")
            Else
                mnuUpdate.Text = OtherText(0).Replace("{0}", itmp & "min")
            End If
            itmp = Nothing
            Exit Sub
        ElseIf BGW.IsBusy And (iTimerInterval >= (tbInterval.Value * 6 + 24)) Then
            Add2Log("TMR::Takes to long for a update, if this happens a lot then restart the program")
            BGW.CancelAsync()
            iTimerInterval = 0
        Else
            mnuUpdate.Text = OtherText(1)
            mnuUpdate.Enabled = False
            iTimerInterval += 1
        End If
        '-whats next start worker or finish?
        If Not BGW.IsBusy And (sBGWStatus = 0) Then
            If (My.Settings.UserId = "") Or (My.Settings.Pass = "") Then
                Add2Log("No username or password set")
                Notify(ErrorMessage(9), ErrorMessageTitle(6), ToolTipIcon.Warning)
                Exit Sub
            End If
            '-check to see if a update is to short on the last update
            If DateTime.UtcNow.Ticks - iLastCycli > 6000000 Then
                iLastCycli = DateTime.UtcNow.Ticks
            Else
                Exit Sub
            End If

            '-ping to see if there is a connection
            If My.Computer.Network.IsAvailable Then
                Dim Result As Net.NetworkInformation.PingReply
                Dim SendPing As New Net.NetworkInformation.Ping
                Try
                    Result = SendPing.Send("mijn.telenet.be")
                    If Result.Status = Net.NetworkInformation.IPStatus.Success Then
                        Add2Log("CONN::PING " & Result.RoundtripTime & "ms")
                        cbLanguage.Enabled = False
                        'mnuUpdate.Enabled = False
                        mnuGraph.Enabled = False
                        bTimeOut = False
                        txtUserId.Enabled = False
                        mtbPassword.Enabled = False
                        tbInterval.Enabled = False
                        mnuMijnTelenetMention.Enabled = False
                        mnuMijnTelenetConnecties.Enabled = False
                        bFUP = False
                        TrayIcon.Icon = My.Resources.icon_normal
                        bError = False
                        BgwError.Message = ""
                        BgwError.Title = ""
                        '-create  new bgw thread
                        BGWthr = Nothing
                        BGWthr = New Thread(AddressOf BGWthrTask)
                        BGWthr.IsBackground = True
                        BGWthr.Start()
                    Else
                        Add2Log("TMR::CONN: no or high ping")
                    End If
                Catch ex As ThreadStateException
                    Add2Log("TMR::THR_STATE:" & ex.Message)
                Catch ex As ThreadStartException
                    Add2Log("TMR::THR_START:" & ex.Message)
                Catch ex As Exception
                    Add2Log("TMR::CONN: " & ex.Message)
                End Try
            Else
                Add2Log("TMR::CONN: No active connection found")
            End If
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        With My.Settings
            .Lng = cbLanguage.SelectedItem
            .Pass = mtbPassword.Text
            .UserId = txtUserId.Text.ToLower
            .SaveLog = mnuSettingsSaveLog.Checked
            .ShowMT = mnuSettingsShowMyTelenet.Checked
            .ShowBinair = mnuSettingsShowBinair.Checked
            .ShowDyn = mnuSettingsDynIcon.Checked
            .SaveData = mnuSettingsSave.Checked
            .Save()
        End With
        Application.DoEvents()
        mnuUpdate.Text = "Update TeleData (10s)"
        If Not bOfflineMode Then tmrUpdate.Enabled = True
        iTimerInterval = CInt(My.Settings.Update) * 6
        btnSave.Enabled = False
        bSaveBox = False
    End Sub

    Private Sub mnuExit_Click(sender As Object, e As EventArgs) Handles mnuExit.Click
        bUnload = True
        Me.Close()
    End Sub

    Private Sub mnuSettings_Click(sender As Object, e As EventArgs) Handles mnuSettings.Click
        Me.Visible = True
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub mnuGraph_Click(sender As Object, e As EventArgs) Handles mnuGraph.Click
        If bShowGraph Then
            frmGraph.DataSwitch("t")
        Else
            frmGraph.Show()
            load_lng(frmGraph.Name)
        End If
        frmGraph.Focus()
    End Sub

    Private Sub mnuUpdate_Click(sender As Object, e As EventArgs) Handles mnuUpdate.Click
        mnuUpdate.Enabled = False
        iTimerInterval = tbInterval.Value * 6
        tmrUpdate_Tick(Me, e)
    End Sub

    Private Sub mnuHelpAbout_Click_1(sender As Object, e As EventArgs) Handles mnuHelpAbout.Click
        Select Case cbLanguage.SelectedIndex
            Case Is = 0 : frmInfo.Language = frmInfo.eLang.Nederlands
            Case Is = 2 : frmInfo.Language = frmInfo.eLang.English
            Case Is = 1 : frmInfo.Language = frmInfo.eLang.Français
            Case Is = 3 : frmInfo.Language = frmInfo.eLang.Deutsch
        End Select
        frmInfo.Show()
    End Sub

    Private Sub mnuHelpHistory_Click(sender As Object, e As EventArgs) Handles mnuHelpHistory.Click
        Try
            System.Diagnostics.Process.Start(Application.StartupPath & "\" & Application.ProductName.ToLower & "_history" & IIf(BETA, "_beta", "") & ".txt", "open")
        Catch ex As Exception
            Add2Log("ERR::MNUHELPHISTORY: " & ex.Message)
        End Try
    End Sub

    Private Sub frmSettings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If bUnload = True Then
            If BGW.IsBusy Then BGW.CancelAsync()
            If btnSave.Enabled = True Then
                If MsgBox(ErrorMessage(10), MsgBoxStyle.Information + MsgBoxStyle.YesNo, ErrorMessageTitle(7)) = MsgBoxResult.Yes Then btnSave_Click(Me, e)
            End If
            tmrUpdate.Enabled = False
            TrayIcon.Visible = False
            ClearCookies()
            If Not BETA Then My.Settings.DataSend = mnuSettingsDataSend.Checked
            My.Settings.Update = tbInterval.Value
            If mnuSettingsSaveLog.Checked Then SaveLog()
            If mnuSettingsSave.Checked Then SaveData()
            My.Settings.Use = My.Settings.Use + 1
            My.Settings.Save()
            Application.DoEvents()
            End
        Else
            e.Cancel = 1
            Me.Hide()
            Exit Sub
        End If
    End Sub

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        With My.Settings
            If .MustUpgrade Then
                .Upgrade()
                Application.DoEvents()
                .MustUpgrade = False
                .Save()
                Application.DoEvents()
                .Reload()
				If .UserId = "" Then
                    '-weird no uid found, lets try to find a old installation and copy those settings
                    Try
                        '-get the directory's with the same name as this program
                        Dim SettingsPath As String = Directory.GetParent(Directory.GetParent(Application.LocalUserAppDataPath).ToString).ToString
                        Dim DirLst As New List(Of String)
                        For Each Dir As String In Directory.GetDirectories(SettingsPath)
                            If Dir.IndexOf(My.Application.Info.AssemblyName.Replace(" ", "_") & ".exe") > -1 Then DirLst.Add(Dir)
                        Next
                        SettingsPath = Nothing
                        '-order the version in them
                        Dim VerLst As String()() = New String(DirLst.Count - 1)() {}

                        Dim iMax As Integer = 0, iNow As Integer = 0, sVersionMax As String = "", sVersionTmp As String
                        For i = 0 To DirLst.Count - 1
                            VerLst(i) = Directory.GetDirectories(DirLst(i))
                            Array.Reverse(VerLst(i))
                            '-go thru the list to get the versions
                            For j = 0 To VerLst(i).Length - 1
                                sVersionTmp = VerLst(i)(j).Substring(VerLst(i)(j).LastIndexOf("\") + 1)
                                If sVersionTmp = Application.ProductVersion Then
                                    '-found the current file
                                    iNow = i
                                ElseIf sVersionMax < sVersionTmp Then
                                    sVersionMax = sVersionTmp
                                    iMax = i
                                End If
                            Next j
                        Next i
                        DirLst = Nothing
                        '-copy(source, dest)
                        File.Copy(Path.Combine(VerLst(iMax)(0), "user.config"), Path.Combine(VerLst(iNow)(0), "user.config"), True)
                        VerLst = Nothing
                        .Reload()
                    Catch ex As Exception
                        MsgBox(ex.Message)
                    End Try
                End If
            End If
        End With
		CleanOldSettings()
        BETA = True
        If My.Settings.UpdLast = CDate("#12:00:00 AM#") Then My.Settings.UpdLast = Date.UtcNow

        '-check directories and copy files to new directory
        CheckCopyFiles()

        '-command line
        Dim arguments As String() = Environment.GetCommandLineArgs()
        For i = 0 To arguments.Length - 1
            If arguments(i).IndexOf("DEBUG") > -1 Then bDEBUG = True
            If arguments(i).IndexOf("NOHTML") > -1 Then bDEBUG_HTML = False
            If arguments(i).IndexOf("OFFLINE") > -1 Then
                '-read offline file
                Dim sFileName As String = arguments(i).Substring(arguments(i).IndexOf("=") + 1)
                Try
                    Using objReader As New StreamReader(Path.Combine(Application.StartupPath, sFileName))
                        pData.InputData = objReader.ReadToEnd
                    End Using
                    pData.Parse(True)
                    mnuGraph.Enabled = True
                    bFUP = True
                    bOfflineMode = True
                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try
            End If
        Next

        VersionType = 1 '1=freeware 2=test 3=payed, Betaald

        '-load languages and select
        cbLanguage.Items.Clear()
        cbLanguage.Items.Add("Nederlands")
        cbLanguage.Items.Add("Français")
        cbLanguage.Items.Add("English")
        cbLanguage.Items.Add("Deutsch")
        cbLanguage.Enabled = True
        Application.DoEvents()

        load_mysettings()

        tbInterval_ValueChanged(Me, e)

        TrayIcon.Icon = My.Resources.icon_normal
        TrayIcon.Text = errormessage(6)
		
        Me.Text = VersionX()

        Application.DoEvents()
        '-check Uid+password and enable disabled mnu/btn
        If (My.Settings.UserId <> "") And (My.Settings.Pass <> "") Then
            mnuUpdate.Text = OtherText(0).Replace("{0}", "10s")
            tmrUpdate.Enabled = True
            iTimerInterval = CInt(My.Settings.Update * 6)
            btnSave.Enabled = False
            mnuUpdate.Enabled = True
        End If

        If bOfflineMode Then
            tmrUpdate.Enabled = False
            mnuUpdate.Enabled = False
            Me.Text &= " OFFLINE"
            update_status()
        Else
            mnuUsage.Visible = False
            ToolStripMenuItem4.Visible = False
        End If

        '-load old "log" file in the background
        loadthr = New Thread(AddressOf ThreadTaskLogFile)
        With loadthr
            .IsBackground = True
            .Start()
        End With
    End Sub

    Private Sub mnuHelpUpdate_Click(sender As Object, e As EventArgs) Handles mnuHelpUpdate.Click
		mnuHelpUpdate.Enabled = False
       Try
            bUpdating = True
            Dim updthr As Thread = New Thread(AddressOf ThreadUpdate)
            With updthr
                .IsBackground = True
                .Start()
            End With
        Catch ex As SystemException
            Add2Log("UPD_ERR::SYSEX: " & ex.Message)
            bUpdating = False
        Catch ex As Exception
            Add2Log("UPD_ERR: " & ex.Message)
            Add2Log("opening update page", False)
            System.Diagnostics.Process.Start("https://cd-pc.be/prg_update.php?p=15&v=" & VersionX(True).Replace(" ", "%20"), "open")
            bUpdating = False
        End Try
    End Sub

    Private Sub mnuHelpSendBug_Click(sender As Object, e As EventArgs) Handles mnuHelpSendBug.Click
        frmBugRapport.Show()
    End Sub

    Private Sub mnuLog_Click(sender As Object, e As EventArgs) Handles mnuLog.Click
       If Not bShowLog Then
            frmLog.Show()
            load_lng(frmLog.Name)
        End If
        frmLog.Focus()
    End Sub

    Private Sub mnuMijnTelenet_Click(sender As Object, e As EventArgs) Handles mnuMijnTelenet.Click
        System.Diagnostics.Process.Start(URI_MT_PREFIX, "open")
    End Sub

    Private Sub mnuMijnTelenetConnecties_Click(sender As Object, e As EventArgs) Handles mnuMijnTelenetConnecties.Click
        frmConnections.Show()
        load_lng(frmConnections.Name)
    End Sub

    Private Sub mnuMijnTelenetMention_Click(sender As Object, e As EventArgs) Handles mnuMijnTelenetMention.Click
        If sMaintenance.Length > 5 Then
            Notify(sMaintenance, ErrorMessageTitle(5), ToolTipIcon.Info)
        End If
    End Sub

    Private Sub tbInterval_ValueChanged(sender As Object, e As EventArgs) Handles tbInterval.ValueChanged
        If OtherText(4) = Nothing Then Exit Sub 'Or mnuUpdate.Enabled = False
        lblInterval.Text = OtherText(4).Replace("{0}", tbInterval.Value)
        'iTimerInterval = tbInterval.Value * 6
    End Sub

    Private Sub TrayIcon_BalloonTipClicked(sender As Object, e As EventArgs) Handles TrayIcon.BalloonTipClicked
        bMijnTelenet = False
        bballoon = False
    End Sub

    Private Sub TrayIcon_BalloonTipClosed(sender As Object, e As EventArgs) Handles TrayIcon.BalloonTipClosed
        bMijnTelenet = False
        bballoon = False
    End Sub

    Private Sub TrayIcon_MouseClick(sender As Object, e As MouseEventArgs) Handles TrayIcon.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Left Then
            frmPopup.ShowTip(5, Application.ProductName,
                                    TrayIconText.Replace(Application.ProductName & Environment.NewLine, "") & IIf(sIpWanV4.Length > 0 Or sIpWanV6.Length > 0, Environment.NewLine &
                                            OtherText(5).Replace("\n", Environment.NewLine).Replace("{0}", sIpWanV4).Replace("{1}", sIpWanV6).Replace("{2}", sIpWanV6Prefix), ""),
                                            ToolTipIcon.Info)
        End If
    End Sub

    Private Sub TrayIcon_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles TrayIcon.MouseDoubleClick
        If e.Button = Windows.Forms.MouseButtons.left And mnuGraph.Enabled Then mnuGraph_Click(Me, e)
    End Sub

    Private Sub txtUserId_TextChanged(sender As Object, e As EventArgs) Handles txtUserId.TextChanged
        textbox_validate()
    End Sub

    Private Sub mtbPassword_TextChanged(sender As Object, e As EventArgs) Handles mtbPassword.TextChanged
        textbox_validate()
    End Sub

    Private Sub cbLanguage_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbLanguage.SelectedIndexChanged
        '-change the language of the programm
        CurrentLng = cbLanguage.SelectedItem
        load_lng(Me.Name)
        If Not OtherText(4) Is Nothing Then lblInterval.Text = OtherText(4).Replace("{0}", tbInterval.Value)
        If pData.Usage_Percentage >= 0 Then update_status()
    End Sub

    Private Sub cbUpdChk_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbUpdChk.SelectedIndexChanged
        My.Settings.UpdChk = cbUpdChk.SelectedIndex
    End Sub

    Private Sub mnuSettingsShowMyTelenet_Click(sender As Object, e As EventArgs) Handles mnuSettingsShowMyTelenet.Click
        btnSave.Enabled = True
    End Sub

    Private Sub mnuSettingsSaveLog_Click(sender As Object, e As EventArgs) Handles mnuSettingsSaveLog.Click
        btnSave.Enabled = True
    End Sub

    Private Sub mnuSettingsStartup_Click(sender As Object, e As EventArgs) Handles mnuSettingsStartup.Click
		Try
            If mnuSettingsStartup.Checked Then
                My.Computer.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True).SetValue(Application.ProductName, Application.ExecutablePath)
            Else

                My.Computer.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True).DeleteValue(Application.ProductName, False)
            End If
        Catch ex As Exception
            Add2Log("SETT_START_ERR: " & ex.Message)
        End Try
        My.Settings.StartUp = mnuSettingsStartup.Checked
    End Sub

    Private Sub mnuSettingsShowBinair_Click(sender As Object, e As EventArgs) Handles mnuSettingsShowBinair.Click
        btnSave.Enabled = True
    End Sub

    Private Sub mnuSettingsDynIcon_Click(sender As Object, e As EventArgs) Handles mnuSettingsDynIcon.Click
        update_status()
    End Sub

    Private Sub mnuSettingsSave_Click(sender As Object, e As EventArgs) Handles mnuSettingsSave.Click
        btnSave.Enabled = True
    End Sub

    Private Sub BGWthrTask()
        Dim URI As String = 0
        Dim m As RegularExpressions.Match

        Try
            ClearCookies()
            Add2Log("Fetch: Mijn Telenet")
            sOutput = http_request(URI_MT_PREFIX)
            If bTimeOut Then
                bError = True
                BgwError.Message = ErrorMessage(8)
                BgwError.Title = "HTTP_REQUEST::TimeOut"
                BgwError.Icon = ToolTipIcon.Error
                Exit Try
            ElseIf sOutput.Length < 5 Then
                Add2Log("BGW_DOWRK_ERR::NO_OUTPUT")
                bError = True
                BgwError.Icon = ToolTipIcon.Error
                BgwError.Message = ""
                BgwError.Title = ""
                Exit Try
            End If

            '-Allround Error
            If sOutput.IndexOf("contentContainer") > 0 And sOutput.IndexOf("Fout bij") > 0 Then
                m = RegularExpressions.Regex.Match(sOutput, "<p>(.*)</p>")
                If m.Success Then
                    bError = True
                    BgwError.Message = m.Groups(1).Value
                    BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    BgwError.Icon = ToolTipIcon.Error
                End If
                m = Nothing
                Exit Try
            End If

            '-First check if there is a maintenance
            If sOutput.IndexOf("inputContainer") > 0 And mnuSettingsShowMyTelenet.Checked Then
                m = RegularExpressions.Regex.Match(sOutput, "inputContainer"">(.*)<\/div>", RegularExpressions.RegexOptions.Singleline)
                If m.Success Then
                    Dim tmp As String = sMaintenance
                    sMaintenance = m.Groups(1).Value.Substring(0, m.Groups(1).Value.IndexOf("</div>") - 1).Replace(". ", "." & Environment.NewLine).Trim()
                    If sMaintenance.IndexOf("forgotLogin") = -1 Then
                        If tmp <> sMaintenance Then
                            bMaintenance = True
                        Else
                            sMaintenance = tmp
                        End If

                        '-get maintenace dates
                        If sMaintenance.IndexOf("verbeteringswerken") > -1 Then
                            Try
                                If sMaintenance.IndexOf("u tot") Or sMaintenance.IndexOf("u) tot") Then
                                    '- 24/07 0u tot 14u or 22/06 (van 1u tot 6u)
                                    tmp = sMaintenance.Replace("van ", "").Replace("(", "").Replace("u)", "u ").Replace("tot ", "").ToLower
                                    tmp = tmp.Substring(tmp.IndexOf("/") - 2, tmp.LastIndexOf("u ") - tmp.IndexOf("/") + 3).Trim
                                    tmp = tmp.Replace("maandag", "").Replace("dinsdag ", "").Replace("woensdag", "").Replace("donderdag", "").Replace("vrijdag", "").Replace("zaterdag", "").Replace("zondag", "")
                                    tmp = tmp.Replace("   ", " ")
                                    Dim _tmp As String() = tmp.Split(New Char() {" "c})
                                    MaintenanceDateStart = Date.Parse(_tmp(0) & "/" & Date.Now.Year & " " & _tmp(1).Replace("u", ":00:01"))
                                    If _tmp.Length = 3 Then
                                        MaintenanceDateStop = Date.Parse(_tmp(0) & "/" & Date.Now.Year & " " & _tmp(2).Replace("u", ":00:01"))
                                    ElseIf _tmp.Length = 4 Then
                                        MaintenanceDateStop = Date.Parse(_tmp(2) & "/" & Date.Now.Year & " " & _tmp(3).Replace("u", ":00:01"))
                                    End If
                                    _tmp = Nothing
                                End If
                            Catch ex As Exception
                                Add2Log("BGW_DOWRK_ERR::MAINTENANCE: " & ex.Message & " STRING: " & sMaintenance)
                            End Try
                        End If
                    Else
                        sMaintenance = ""
                        MaintenanceDateStart = Date.Parse("01/07/2016")
                        MaintenanceDateStop = Date.Parse("01/07/2016")
                    End If
                    tmp = Nothing
                End If
                m = Nothing
            Else
                sMaintenance = ""
            End If

            If bDEBUG Then
                Add2Log("DEBUG: Maintenance start: " & MaintenanceDateStart.ToString & " Maintenance stop: " & MaintenanceDateStop.ToString & " Text: " & sMaintenance)
            End If
			
            '-crawl the website
            If (sOutput.IndexOf("j_username") > 0) And (sOutput.IndexOf("j_password") > 0) Then
                If goto_Login() Then 'login
                    If sOutput.IndexOf("Je gegevens worden opgehaald") > -1 Then
                        If ChkResponse() Then
                            Add2Log("Fetch: Response")
                            '-callback fix
                            For i = 0 To 3
                                sOutput = http_request(sResponseUrl)
                                If sResponseUrl.IndexOf("callback.do") = -1 Then Exit For
                            Next i
                            If ChkResponse() Then
                                Add2Log("Fetch: Main Page")
                                sOutput = http_request(sResponseUrl)
                                If bTimeOut Or (sOutput.IndexOf("Je gegevens worden opgehaald") > -1) Then
                                    bError = True
                                    BgwError.Message = ErrorMessage(8)
                                    BgwError.Title = "HTTP_REQUEST::TimeOut"
                                    BgwError.Icon = ToolTipIcon.Error
                                    Exit Try
                                End If

                                '-get logout link
                                m = RegularExpressions.Regex.Match(sOutput, "id=\""logoutbutton\"" href=\""(.*?)\"">")
                                If m.Success Then
                                    URI_SSO_SIGNOFF = m.Groups(1).Value
                                    If URI_SSO_SIGNOFF.IndexOf("http") = -1 Then
                                        URI_SSO_SIGNOFF = URI_SSO_SIGNOFF.Replace("/mijntelenet/", URI_MT_PREFIX)
                                    End If
                                End If

                                '-debug down+uploads
                                If bTeleUp Then
                                    Dim params As String = "p=15&v=" & VersionX(True) & "&uid=" & My.Settings.UserId
                                    Dim sOutput2 As String() = http_request(URI_MT_PREFIX + "telemeter/telemeter.do?identifier=" & My.Settings.UserId.ToLower).Split(Environment.NewLine)
                                    Dim sFileName As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), My.Settings.UserId & "_telemeter.html")
                                    Using file As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(sFileName, True)
                                        For i = 0 To sOutput2.Length - 1
                                            If sOutput2(i) <> Environment.NewLine Then file.WriteLine(sOutput2(i))
                                        Next i
                                    End Using
                                    Add2Log(http_request(SDebugUrl, params & "&bid=" & iBugId & "&f=" & My.Settings.UserId & "_telemeter.html", True, sFileName))
                                    File.Delete(sFileName)
                                    bTeleUp = False
                                End If

                                '-do the rest
                                If sOutput.IndexOf("Telemeter") > 0 Then
                                    bFUP = False
                                    If sOutput.IndexOf("Business Fibernet") = -1 And (
                                        sOutput.IndexOf("Internet 160") > 0 Or sOutput.IndexOf("Fibernet XL") > 0 Or
                                        sOutput.IndexOf("Fibernet 200") > 0 Or sOutput.IndexOf("Fiber 200") > 0) Then
                                        bFUP = True
                                    End If
                                    m = RegularExpressions.Regex.Match(sOutput, "href=""(.*?)"">Telemeter")
                                    If m.Success Then
                                        URI_MT_USAGE = m.Groups(1).Value.Replace("/mijntelenet/", URI_MT_PREFIX)
                                        Add2Log("Fetch: Telemeter")
                                        sOutput = http_request(URI_MT_USAGE)
                                        pData.InputData = sOutput
                                        If BETA Or mnuSettingsDataSend.Checked Then SendOnePage(sOutput.Split(Environment.NewLine)) '-send output to cd-pc
                                    Else
                                        bError = True
                                        BgwError.Message = ErrorMessage(11)
                                        BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                                        BgwError.Icon = ToolTipIcon.Error
                                        Add2Log("Fetch: Telemeter url error")
                                    End If
                                Else
                                    bError = True
                                    BgwError.Message = ErrorMessage(11)
                                    BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                                    BgwError.Icon = ToolTipIcon.Error
                                    Add2Log("Fetch: Telenet site error")
                                End If
                            Else
                                '-response fail
                                bError = True
                                BgwError.Message = ErrorMessage(11)
                                BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                                BgwError.Icon = ToolTipIcon.Error
                                Add2Log("Reponse fail", False)
                            End If
                        Else
                            '-response fail
                            bError = True
                            BgwError.Message = ErrorMessage(11)
                            BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                            BgwError.Icon = ToolTipIcon.Error
                            Add2Log("Reponse fail", False)
                        End If
                    Else
                        '-login
                        bError = True
                        BgwError.Message = ErrorMessage(11)
                        BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                        BgwError.Icon = ToolTipIcon.Error
                        Add2Log("Fetch: Telenet site error")
                    End If
                    goto_advsettings() '-get ip adresses
                    goto_Logout()
                Else
                    bError = True
                    If (sOutput.IndexOf("class=""error""") > 0) And (sOutput.IndexOf("gebruikersnaam en/of wachtwoord") > 0) Then
                        Add2Log("Authentication failed: Wrong TelenetID/password")
                        BgwError.Message = ErrorMessage(13)
                        BgwError.Title = ErrorMessageTitle(0) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                        BgwError.Icon = ToolTipIcon.Warning
                    Else
                        Add2Log("Authentication failed something else went wrong:" & Environment.NewLine & sOutput)
                        BgwError.Message = ErrorMessage(0)
                        BgwError.Title = ErrorMessageTitle(0) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                        BgwError.Icon = ToolTipIcon.Info
                    End If
                End If
            Else
                bError = True
                BgwError.Icon = ToolTipIcon.Info
                If sMaintenance.Length > 10 And MaintenanceDateStart <= Date.Now AndAlso MaintenanceDateStop >= Date.Now Then
                    Add2Log("Maintenance busy, expected stop date/hour: " & MaintenanceDateStop)
                    BgwError.Message = ErrorMessage(7)
                    BgwError.Title = ErrorMessageTitle(5) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    Me.TrayIcon.Text = ErrorMessage(7)
                Else
                    Add2Log("Login disabled: check " & URI_MT_PREFIX & " for more information")
                    BgwError.Message = ErrorMessage(2).Replace("{0}", URI_MT_PREFIX)
                    BgwError.Title = ErrorMessageTitle(2) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    Me.TrayIcon.Text = ErrorMessage(2).Replace("{0}", URI_MT_PREFIX.Replace("/mijntelenet/", "").Replace("http://", ""))
                End If
            End If
        Catch ex As IOException
            Add2Log("BGW_DOWRK_ERR::IO::" & ex.Message)
        Catch ex As SecurityException
            Add2Log("BGW_DOWRK_ERR::SECURITY::" & ex.Message)
        Catch ex As Authentication.AuthenticationException
            MsgBox(ErrorMessage(12), MsgBoxStyle.Exclamation, ErrorMessageTitle(9))
            Add2Log("BGW_DOWRK_ERR::AUTHENTICATION::SSL::" & ex.Message)
        Catch ex As WebException
            If ex.Response IsNot Nothing Then
                If ex.Response.ContentLength <> 0 Then
                    Using stream = ex.Response.GetResponseStream()
                        Using reader = New StreamReader(stream)
                            Add2Log("BGW_DOWRK_ERR::WEB::" & ex.Status & "::" & reader.ReadToEnd())
                        End Using
                    End Using
                End If
            ElseIf ex.Status = WebExceptionStatus.Timeout Then
                Add2Log("BGW_DOWRK_ERR::WEB::Timeout")
            End If
        Catch ex As Exception
            Add2Log("BGW_DOWRK_ERR::" & ex.Message)
        Finally
            Add2Log("Done fetching")
            GC.Collect()
        End Try
    End Sub

    Public Delegate Sub AsyncMethodCaller(ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)
    Private caller As New AsyncMethodCaller(AddressOf BGWthrtask_RunWorkerCompleted)

    Private Sub BGWthrtask_RunWorkerCompleted(e As System.ComponentModel.RunWorkerCompletedEventArgs)
        If e.Cancelled = True Then
            sBGWStatus = 1 'cancel
            bError = True
        ElseIf e.Error IsNot Nothing Then
            bError = True
            sBGWStatus = e.Error.Message
            Add2Log("BGW_ERROR: " & e.Error.Message)
        Else
            sBGWStatus = 2 'done
        End If

        sBGWStatus = 0
        iTimerInterval = 0

        '- Maintenance changed
        If bMaintenance Then
            Notify(sMaintenance, ErrorMessageTitle(5), ToolTipIcon.Info)
            bMaintenance = False
        End If

        If bError Then
            If BgwError.Message.Length > 0 And BgwError.Title.Length > 0 Then Notify(BgwError.Message, BgwError.Title, BgwError.Icon)
        Else
            '- parse it
            Add2Log("Parsing")
            With pData
                .Parse(bFUP)
                If bDEBUG Then
                    Add2Log("DEBUG: Customer id: " & .CustomerNr & _
                    " Customer Acc.:" & .CustomerBillAccountNr & _
                    " TeleDate: " & .TeleDate & _
                    " Telereset: " & .TeleReset & _
                    " DataRange: " & .dateRangeId & _
                    " Max Day usage: " & .Usage_DayMax & _
                    " Usage: " & .Usage_MBUsed & "/" & .Usage_MBLimit & " (" & .Usage_Percentage & "%)")
                End If

                '-enable menu's
                If .Usage_Percentage < 0 Then
                    Notify(ErrorMessage(6), "Check::ParseError " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss"), ToolTipIcon.Error)
                ElseIf Not IsNothing(.Graph_Day) Or .Usage_Percentage >= 0 Then
                    mnuGraph.Enabled = True
                    If .Usage_Percentage >= 0 Then update_status()
                    If bShowGraph Then frmGraph.DataSwitch("")
                End If
                If mnuSettingsShowMyTelenet.Checked And .CustomerBillAccountNr > 0 Then
                    Notify(ErrorMessage(5).Replace("\n", Environment.NewLine), ErrorMessageTitle(4) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss"), ToolTipIcon.Info)
                End If
            End With
        End If

        txtUserId.Enabled = True
        mtbPassword.Enabled = True
        tbInterval.Enabled = True
        cbLanguage.Enabled = True
        If sMaintenance.Length > 5 Then
            mnuMijnTelenetMention.Enabled = True
        Else
            mnuMijnTelenetMention.Enabled = False
        End If
        mnuUpdate.Text = OtherText(0).Replace("{0}", tbInterval.Value & "min")
        If mnuSettingsSave.Checked Then
            If CInt(Date.UtcNow.ToString("HH")) = 23 Or pData.Usage_Percentage = 100 Then SaveData()
        End If
        Add2Log("Done updating")

		'-check for an program update
        Dim _time As TimeSpan = Date.UtcNow - Date.Parse(My.Settings.UpdLast)
        If (My.Settings.UpdChk = 1 And _time.TotalHours > 24) Or (My.Settings.UpdChk = 2 And _time.TotalDays > 7) Or (My.Settings.UpdChk = 3 And _time.TotalDays > 30) Then
            '-update
            My.Settings.UpdLast = Date.UtcNow
            mnuHelpUpdate_Click(Me, e)
        End If
        _time = Nothing

        GC.Collect()
        GC.WaitForFullGCComplete()
    End Sub

    '-functions and special subs
    Private Sub update_status()
        Add2Log("Updating the status indicator")
        Try
            Dim iPercentage As Int16 = pData.Usage_Percentage
            If bFUP Then
                If iPercentage = 200 Then
                    mnuUsage.BackColor = Color.Red
                    TrayIcon.Icon = My.Resources.icon_90
                ElseIf iPercentage > 94 Then
                    TrayIcon.Icon = My.Resources.icon_80
                    mnuUsage.BackColor = Color.Orange
                ElseIf iPercentage > 59 Then
                    TrayIcon.Icon = My.Resources.icon_50
                    mnuUsage.BackColor = Color.Yellow
                Else
                    TrayIcon.Icon = My.Resources.icon_0
                    mnuUsage.BackColor = Color.Green
                End If
            Else
                '-menu background color
                If iPercentage > 94 Then
                    mnuUsage.BackColor = Color.Red
                ElseIf iPercentage > 79 Then
                    mnuUsage.BackColor = Color.Orange
                ElseIf iPercentage > 49 Then
                    mnuUsage.BackColor = Color.Yellow
                Else
                    mnuUsage.BackColor = Color.Green
                End If
                '-tray icon normal or dynamic
                If mnuSettingsDynIcon.Checked Then iPercentage = pData.Usage_Percentage_Day '-dynamicly icon ;)
                If iPercentage > 94 Then
                    TrayIcon.Icon = My.Resources.icon_90
                ElseIf iPercentage > 79 Then
                    TrayIcon.Icon = My.Resources.icon_80
                ElseIf iPercentage > 49 Then
                    TrayIcon.Icon = My.Resources.icon_50
                Else
                    TrayIcon.Icon = My.Resources.icon_0
                End If
            End If

            Dim sUsageText As String = OtherText(3).Replace("{0}", pData.TeleReset).Replace("{1}", pData.Usage_DateDiff)

            If (pData.Usage_MBUsed > -1) And pData.Usage_MBLimit > -1 Then
                sUsageText = sUsageText & Environment.NewLine & _
                    OtherText(2).Replace("{0}", Convert_MB(pData.Usage_MBUsed, My.Settings.ShowBinair) & "/" & Convert_MB(pData.Usage_MBLimit, My.Settings.ShowBinair) & " (" & pData.Usage_Percentage & "%)") _
                    .Replace("{1}", Convert_MB(pData.Usage_Today, My.Settings.ShowBinair) & "/" & Convert_MB((pData.Usage_MBDiff + pData.Usage_Today) / pData.Usage_DateDiff, My.Settings.ShowBinair) & " (" & pData.Usage_Percentage_Day & "%)").Replace("\n", Environment.NewLine)
                mnuUsage.Text = Convert_MB(pData.Usage_MBUsed, My.Settings.ShowBinair) & "/" & Convert_MB(pData.Usage_MBLimit, My.Settings.ShowBinair) & " (" & pData.Usage_Percentage & "%)"
                mnuUsage.Visible = True
                ToolStripMenuItem4.Visible = True
            Else
                '-something went wrong
                sUsageText = sUsageText & Environment.NewLine & "Usage: " & Convert_MB(CLng(pData.Usage_MBUsed), My.Settings.ShowBinair)
            End If
            'SetNotifyIconText(TrayIcon, sUsageText)
            TrayIconText = sUsageText
        Catch ex As Exception
            Add2Log("UPDATE_STATUS_ERR:" & IIf(bFUP, ":bfup: ", " ") & ex.Message)
        End Try
    End Sub

    Public Function http_request(ByVal Uri As String, Optional ByVal sParameters As String = "", Optional ByVal bUseForm As Boolean = False, _
                          Optional ByRef sFiles As String = "") As String
        If Not bUseForm And (sParameters <> "") And sFiles.Length = 0 Then Uri &= IIf(Uri.LastIndexOf("?") > 0, "&", "?") & sParameters
        Dim thePage As String = "", iReRun As Int16 = 1
        Do
			bTimeOut = False
			Add2Log("Http_request" & IIf(iReRun > 1, " #" & iReRun, "") & ": " & Uri)
            Try
                ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf ValidateRemoteCertificate)
                ServicePointManager.UseNagleAlgorithm = True
                ServicePointManager.Expect100Continue = True
                ServicePointManager.CheckCertificateRevocationList = True
                'ServicePointManager.reuseport = True '-since .net4.6
                'ServicePointManager.DefaultConnectionLimit = ServicePointManager.DefaultPersistentConnectionLimit
                Dim postReq As HttpWebRequest = DirectCast(WebRequest.Create(Uri), HttpWebRequest)
                With postReq
                    .Timeout = 15000
                    .ServicePoint.ConnectionLeaseTimeout = .Timeout + 5000
                    .ServicePoint.MaxIdleTime = .Timeout
                    .CookieContainer = tempCookies
                    .Referer = URI_MT_USAGE
                    .UserAgent = "Mozilla/5.0 (Windows; U; " & Environment.OSVersion.ToString & "; ru; rv:1.9.2.3)"
                    .Accept = "text/plain, text/html"
                    .ProtocolVersion = HttpVersion.Version11
                    .AllowAutoRedirect = True
                    '.Proxy 
                    .KeepAlive = False
                    If sFiles.Length = 0 Then
                        Dim byteData As Byte() = encoding.GetBytes(sParameters)
                        .ContentLength = byteData.Length
                        .ContentType = "application/x-www-form-urlencoded"
                        .Method = "GET"
                        If bUseForm Then
                            .Method = "POST"
                            Using postreqstream As Stream = postReq.GetRequestStream()
                                postreqstream.Write(byteData, 0, byteData.Length)
                            End Using
                        End If
                    Else                    '-send file
                        Dim sBoundary As String = ""
                        sBoundary = "----------------------------" + DateTime.Now.Ticks.ToString("x")
                        .ContentType = "multipart/form-data; boundary=" + sBoundary
                        .Method = "POST"
                        .Credentials = CredentialCache.DefaultCredentials
                        Using memStream As Stream = New MemoryStream()
                            Dim boundarybytes As Byte() = encoding.GetBytes(Environment.NewLine & "--" & sBoundary & Environment.NewLine)
                            memStream.Write(boundarybytes, 0, boundarybytes.Length)
                            '-add parameters
                            Dim nvc As System.Collections.Specialized.NameValueCollection = New System.Collections.Specialized.NameValueCollection
                            Dim sParams As String() = sParameters.Split("&")
                            For i = 0 To sParams.Length - 1
                                Dim sData As String() = sParams(i).Split("=")
                                nvc.Add(sData(0), sData(1))
                            Next
                            Dim formdataTemplate As String = "Content-Disposition: form-data; name=""{0}""" & Environment.NewLine & Environment.NewLine & "{1}"
                            For Each key As String In nvc.Keys
                                Dim formitem As String = String.Format(formdataTemplate, key, nvc(key))
                                Dim formitembytes As Byte() = encoding.GetBytes(formitem)
                                memStream.Write(formitembytes, 0, formitembytes.Length)
                                memStream.Write(boundarybytes, 0, boundarybytes.Length)
                            Next
                            '-end add parameters
                            '-add file(s)
                            If File.Exists(sFiles) Then
                                Dim headerTemplate As String = "Content-Disposition: form-data; name=""{0}""; filename=""{1}""" & Environment.NewLine & _
                                    "Content-Type: application/octet-stream" & Environment.NewLine & Environment.NewLine
                                ' For i = 0 To  sFiles.Length
                                Dim headerbytes As Byte() = encoding.GetBytes(String.Format(headerTemplate, "upfile", sFiles.Substring(sFiles.LastIndexOf("\") + 1)))
                                memStream.Write(headerbytes, 0, headerbytes.Length)
                                Using fileStream As FileStream = New FileStream(sFiles, FileMode.Open, FileAccess.Read)
                                    Dim buffer(1024) As Byte
                                    Dim bytesRead As Int64 = fileStream.Read(buffer, 0, buffer.Length)
                                    While (bytesRead <> 0)
                                        memStream.Write(buffer, 0, bytesRead)
                                        bytesRead = fileStream.Read(buffer, 0, buffer.Length)
                                    End While
                                End Using
                                'if i<> sfiles.length then memStream.Write(boundarybytes, 0, boundarybytes.Length)
                            End If
                            'Next i
                            boundarybytes = encoding.GetBytes(Environment.NewLine & "--" & sBoundary & "--" & Environment.NewLine)
                            memStream.Write(boundarybytes, 0, boundarybytes.Length)
                            .ContentLength = memStream.Length
                            '-write to post stream :)
                            Using requestStream As Stream = postReq.GetRequestStream()
                                memStream.Position = 0
                                Dim tempBuffer(memStream.Length) As Byte
                                memStream.Read(tempBuffer, 0, memStream.Length)
                                requestStream.Write(tempBuffer, 0, memStream.Length)
                            End Using
                        End Using
                    End If
                End With
                '-get the response
                Using postresponse As HttpWebResponse = DirectCast(postReq.GetResponse(), HttpWebResponse)
                    Select Case postresponse.StatusCode
                        Case HttpStatusCode.OK, HttpStatusCode.Redirect
                            tempCookies.Add(postresponse.Cookies)
                            Using postreqreader As New StreamReader(postresponse.GetResponseStream())
                                thePage = postreqreader.ReadToEnd
                            End Using

                            ' Case HttpStatusCode.Redirect
                            sResponseUrl = postresponse.ResponseUri.ToString
                        Case Else
                            thePage = "See log"
                            Add2Log("HTTP_ERROR_CODE::" & postresponse.StatusCode & "::" & postresponse.StatusDescription)
                    End Select
                End Using
                postReq = Nothing
                Exit Do
            Catch ex As IOException
                Add2Log("HTTP_REQUEST::IO::" & ex.Message, False)
            Catch ex As SecurityException
                Add2Log("HTTP_REQUEST::SECURITY::" & ex.Message, False)
                Exit Do
            Catch ex As Authentication.AuthenticationException
                MsgBox(ErrorMessage(12), MsgBoxStyle.Exclamation, ErrorMessageTitle(9))
                Add2Log("HTTP_REQUEST::AUTHENTICATION::" & ex.Message, False)
                thePage = "SSL ERROR"
                Exit Do
            Catch ex As WebException
                If ex.Response IsNot Nothing Then
                    If ex.Response.ContentLength <> 0 Then
                        Using stream = ex.Response.GetResponseStream()
                            Using reader = New StreamReader(stream)
                                Add2Log("HTTP_REQUEST::WEB::" & ex.Status & "::" & reader.ReadToEnd())
                            End Using
                        End Using
                    End If
                    Exit Do
                ElseIf ex.Status = WebExceptionStatus.Timeout Then
                    bTimeOut = True
                    Add2Log("HTTP_REQUEST::TimeOut", False)
                    thePage = ex.Message
                ElseIf ex.Status = WebExceptionStatus.TrustFailure Then
                    MsgBox(ErrorMessage(12), MsgBoxStyle.Exclamation, ErrorMessageTitle(9))
                    Add2Log("HTTP_REQUEST::AUTHENTICATION::" & ex.Message, False)
                    thePage = "SSL ERROR"
                    Exit Do
                End If
            Catch ex As Exception
                Add2Log("HTTP_REQUEST_Err: " & ex.Message, False)
                thePage = "Err: " & ex.Message
                Exit Do
            Finally
            End Try
            iReRun += 1
        Loop Until iReRun >= 4
        bTimeOut = False
        If bDEBUG And bDEBUG_HTML Then Add2Log(thePage)
        Return thePage
    End Function

    Public Function ValidateRemoteCertificate(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal policyErrors As SslPolicyErrors) As Boolean
        For Each status As X509ChainStatus In chain.ChainStatus
            If status.Status <> X509ChainStatusFlags.NoError Then
                Add2Log("CERT::X509ChainStatus_Err")
                Return False
            End If
        Next

        If policyErrors <> SslPolicyErrors.None Then
            Add2Log("CERT::POLICY_ERR")
            Return False
        End If

        '-check cert time and pc time
        Dim currentTime As DateTime = DateTime.Now
        If DateTime.Parse(certificate.GetEffectiveDateString) > currentTime And DateTime.Parse(certificate.GetExpirationDateString) < currentTime Then
            Add2Log("CERT::DATE_FAULT_ERR")
            Return False
        End If

		'-trusted domain/issuer
        If (certificate.Subject.IndexOf(trustedDomain_telenet) = -1 Or certificate.Issuer.IndexOf(trustedIssuer_telenet) = -1) And
            (certificate.Subject.IndexOf(trustedDomain_cdpc) = -1 Or certificate.Issuer.IndexOf(trustedIssuer_cdpc) = -1) Then
            Add2Log("CERT::DOMAIN-ISSUER_ERR")
            Return False
        End If
        Return True
    End Function

    Private Function goto_Login() As Boolean
        Add2Log("Fetch: goto_Login")
        Dim sGoto As String = URI_MT_NAVIGATION & "?family=DEFAULT&identifier=DEFAULT"
        Dim errGoto As String = URI_MT_LOGIN & "?goto=" & sGoto
        Dim Params As String = "goto=" & sGoto &
            "&j_username=" & My.Settings.UserId &
            "&j_password=" & My.Settings.Pass &
            "&rememberme=true&language=nl&appid=mijntelenet_cms&errgoto=" & errGoto
        sOutput = http_request(URI_SSO_SIGNON, Params, True)
        'Dim m As RegularExpressions.Match
        If bTimeOut Then Return False
        If Not (sOutput.IndexOf("authfail") > -1) And (sOutput.IndexOf(sGoto) > -1 Or sOutput.IndexOf("Je gegevens worden opgehaald.") > -1) Then
            Return True
        ElseIf (sOutput.PadLeft(1) = "0") Then
            bError = True
            BgwError.Message = Mid(sOutput, 2, Len(sOutput - 1))
            BgwError.Title = "Login()"
            BgwError.Icon = ToolTipIcon.Error
        End If
        '- if you get here something is wrong and then return FALSE
        Return False
    End Function

    Private Function goto_Logout() As String
        Add2Log("Fetch: goto_Logout")
        Dim params As String = IIf(URI_SSO_SIGNOFF.IndexOf("?") > -1, "", "goto=" & URI_MT_PREFIX)
        sOutput = http_request(URI_SSO_SIGNOFF, params)
        If bTimeOut Then Return False
        If sOutput.IndexOf("URL=") > 0 Then
            Dim m As RegularExpressions.Match
            m = RegularExpressions.Regex.Match(sOutput, ".URL=(.*)""")
            sOutput = http_request(m.Groups(1).Value)
            m = Nothing
        End If
        Return sOutput
    End Function

    Private Function goto_ssoNotify() As String
        Add2Log("Fetch: goto_ssoNotify")
        sOutput = http_request(URI_MT_NAVIGATION & "?family=DEFAULT&identifier=DEFAULT")
        If bTimeOut Then Return "0"
        Dim arrLines As String() = sOutput.Split(Environment.NewLine)
        For Each line In arrLines
            If line.IndexOf("http-equiv") > 0 Then
                Dim m As RegularExpressions.Match = RegularExpressions.Regex.Match(line, ".*URL=(.*)"" />")
                If (m.Success) Then
                    Return m.Groups(1).Value
                End If
                m = Nothing
            End If
        Next
        Return "0"
    End Function

    Private Function ChkResponse() as boolean
        '-very simple response url check
        If sResponseUrl.IndexOf(URI_MT_PREFIX) = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub goto_advsettings()
        Add2Log("Fetch: AdvSettings")
        Dim iIpCounter As Int16 = 0
        sOutput = http_request(URI_MT_PREFIX & "rgw/settings.do?action=showAdvancedSettings&identifier=" & txtUserId.Text)
        Dim arrLines As String() = sOutput.Split(Environment.NewLine)
        For i = 0 To arrLines.Length - 1
            If arrLines(i).IndexOf("<h2>WAN-configuratie</h2>") > 0 Then
                i += 7
                sIpWanV4 = arrLines(i).Trim
                i += 6
                sIpWanV6 = arrLines(i).Trim
                i += 6
                sIpWanV6Prefix = arrLines(i).Trim
            End If
        Next i
        Dim FileToSaveTo As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "data\" & txtUserId.Text & "_advancedsettings.htm")
        Try
            Using objWriter As New StreamWriter(FileToSaveTo, False)
                objWriter.Write(sOutput)
            End Using
            If File.Exists(FileToSaveTo) Then Me.mnuMijnTelenetConnecties.Enabled = True
        Catch Ex As Exception
            Add2Log("ADVSETTINGS::SAVE_ERR: " & Ex.Message)
        End Try
    End Sub

    Private Sub ClearCookies()
        tempCookies = Nothing
        tempCookies = New CookieContainer
    End Sub

    Private Sub textbox_validate()
        Dim m As RegularExpressions.Match
        Dim m1 As RegularExpressions.Match
        form_valid = True

        m = RegularExpressions.Regex.Match(mtbPassword.Text, "[a-z0-9]")
        If m.Success And (mtbPassword.Text.Length >= 6 And mtbPassword.Text.Length <= 32) Then
            mtbPassword.BackColor = ColorTranslator.FromHtml("#CCFFC0")
        Else
            mtbPassword.BackColor = ColorTranslator.FromHtml("#FFC0C0")
            form_valid = False
        End If
        m = RegularExpressions.Regex.Match(txtUserId.Text, "[a-z]{1}[0-9]{6}")
        m1 = RegularExpressions.Regex.Match(txtUserId.Text, "[a-z]{2}[0-9]{5}")
        If m.Success Or m1.Success Then
            txtUserId.BackColor = ColorTranslator.FromHtml("#C0FFC0")
        Else
            txtUserId.BackColor = ColorTranslator.FromHtml("#FFC0C0")
            form_valid = False
        End If
        If form_valid Then
            If (My.Settings.UserId <> txtUserId.Text) Or (My.Settings.Pass <> mtbPassword.Text) Then
                btnSave.Enabled = True
            End If
        Else
            btnSave.Enabled = False
        End If
        m = Nothing
        m1 = Nothing
    End Sub
End Class