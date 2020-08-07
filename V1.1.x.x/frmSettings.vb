Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Security
Imports System.Threading
Imports System.Data.SQLite
Imports System.ComponentModel

'-----------------------------------------------------------
'* First code based on Telemeter-Indicator by Alwin Roosen *
'-----------------------------------------------------------
Public Class frmSettings
#Region "declarations"
    Inherits Form
    Public bUnload As Boolean = False
    'Private bMemClean As Boolean = True
    Private form_valid As Boolean = False

    '- other things
    Private Structure BgwErrorStruct
        Public Title As String
        Public Icon As ToolTipIcon
        Public Message As String
    End Structure

    Private sBGWstatus As String = "0"
    Private Shared BgwError As BgwErrorStruct
    Private Shared bStatus As Boolean = False, bSave As Boolean = False, bMijnTelenet As Boolean = False, bMaintenance As Boolean = False, bError As Boolean = False, bSaveBox As Boolean = False, bBalloon As Boolean = False
    Private iTimerInterval As Integer = 0 'interval in seconds
    Private iLastCycli As Long = 0 'time when last cycli is started
    'Private LastSaveDate As Short = CShort(Date.UtcNow.ToString("dd"))

#Region "Various Uri 's for the Telenet site"
    '-Various Uri's for the Telenet 'Mijn Telenet' web portal
    Public Const URI_MT_PREFIX As String = "https://mijn.telenet.be/mijntelenet/"
    Private Const URI_MT_NAVIGATION As String = URI_MT_PREFIX + "navigation/navigation.do"
    Private Const URI_MT_LOGIN As String = URI_MT_PREFIX + "login/login.do"
    Private Const URI_MT_SSO As String = URI_MT_PREFIX + "session/sso.do"
    Public URI_MT_USAGE As String = ""
    Private Const URI_MT_BILLING As String = URI_MT_PREFIX + "billing/billingOverview.do?accountNumber="
    Public Const URI_MT_MOBILE As String = URI_MT_PREFIX + "mobile/unbilledUsage.do?identifier="

    '-Various Uri's for the Telenet 'SSO' pages
    Private Const URI_SSO_PREFIX As String = "https://login.prd.telenet.be/openid/"
    Private Const URI_SSO_LOGIN As String = URI_SSO_PREFIX + "login"
    Private Const URI_SSO_NOTIY As String = URI_SSO_PREFIX + "notifyEvent.do"
    Private Const URI_SSO_SIGNON As String = URI_SSO_PREFIX + "login.do"
    Private URI_SSO_SIGNOFF As String = URI_SSO_PREFIX + "signoff.do"
#End Region

    '-AdvancedSettings data
    Private sIpWanV4 As String = ""
    Private sIpWanV6 As String = ""
    Private sIpWanV6Prefix As String = ""
#End Region

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If btnSave.Enabled Then
            btnSave.Enabled = False
            '-reload old values
            Load_MySettings()
        End If
        Close()
    End Sub

    Private Sub tmrUpdate_Tick(sender As Object, e As EventArgs) Handles tmrUpdate.Tick
        '-check if the program is updating or not
        If Not bUpdating And Not mnuHelpUpdate.Enabled Then
            mnuHelpUpdate.Enabled = True
        End If

#Region "check if save button is enabled else if updates when your changing settings = not GOOOOOOOD"
        If btnSave.Enabled = True Then
            If bDEBUG Then Add2Log("DEBUG::TMR: Save button")
            If bSaveBox Then Exit Sub
            bSaveBox = True
            If MsgBox(ErrorMessage(10), CType(MsgBoxStyle.Information + MsgBoxStyle.YesNo, Global.Microsoft.VisualBasic.MsgBoxStyle), ErrorMessageTitle(7)) = MsgBoxResult.Yes Then
                btnSave_Click(Me, e)
            Else
                Exit Sub
            End If
        End If
#End Region

#Region "if last hours of periode, change iTimerInterval so it updates before the next day"
        If mnuSettingsSave.Checked And (pData.TeleReset = CDate(Date.UtcNow.ToString("dd/MM/yyyy"))) Then
            If bDEBUG Then Add2Log("DEBUG::TMR: set timer before next period change")
            Dim iMinutes As Short = CShort(CShort(Date.UtcNow.ToString("mm")) + ((tbInterval.Value * 6 - iTimerInterval) \ 6))
            If CInt(Date.UtcNow.ToString("HH")) = 23 And iMinutes > 59 Then
                iTimerInterval = 6 * (iMinutes - 59)
            End If
        End If
#End Region

#Region "check if a update is required or not"
        If iTimerInterval < (tbInterval.Value * 6) Then
            If iTimerInterval >= 3 Then mnuUpdate.Enabled = True
            iTimerInterval += 1
            Dim itmp As Short = CShort((tbInterval.Value * 6 - iTimerInterval) \ 6)
            If itmp = 0 Then
                'itmp = CShort((tbInterval.Value * 6 - iTimerInterval) + 1 * 10)
                mnuUpdate.Text = OtherText(0).Replace("{0}", "<60sec")
            Else
                mnuUpdate.Text = OtherText(0).Replace("{0}", itmp & "min")
            End If
            itmp = Nothing
            Exit Sub
        ElseIf BGW.IsBusy AndAlso (iTimerInterval >= (tbInterval.Value * 6 + 24)) Then
            Add2Log("TMR::Takes to long for a update, if this happens a lot then restart the program")
            BGW.CancelAsync() ' = Nothing
            iTimerInterval = 0
        Else
            mnuUpdate.Text = OtherText(1)
            mnuUpdate.Enabled = False
            iTimerInterval += 1
        End If
#End Region

        '-whats next start worker or finish?
        If Not BGW.IsBusy AndAlso (sBGWstatus = "0") Then
            If (My.Settings.UserId = "") Or (My.Settings.Pass = "") Then
                Add2Log("No username or password set")
                Notify(ErrorMessage(9), ErrorMessageTitle(6), ToolTipIcon.Warning)
                Exit Sub
            End If
            '-check to see if an update is to short from the last update
            If Date.UtcNow.Ticks - iLastCycli > 6000000 Then
                iLastCycli = Date.UtcNow.Ticks
            Else
                If bDEBUG Then Add2Log("DEBUG::TMR: Cycle to short")
                Exit Sub
            End If

#Region "Get userid"
            If sLastUser <> txtUserId.Text Then
                If bDEBUG Then Add2Log("DEBUG::TMR: Get userid: " & sLastUser & " <> " & txtUserId.Text)
                Using cmd As New SQLiteCommand("Select UserId from tblUser where UserName=@uname limit 1;")
                    cmd.Parameters.AddWithValue("@uname", txtUserId.Text)
                    db_UserId = CInt(dbdata.GetValue(cmd))
                    If db_UserId < 1 Then
                        db_UserId = dbdata.Insert("tblUser", New Dictionary(Of String, Object) From {{"UserName", txtUserId.Text}})
                        If db_UserId < 1 Then
                            Add2Log("TMR::DB: New user DB problem")
                            Exit Sub
                        End If
                    End If
                    cmd.Parameters.Clear()
                End Using
                sLastUser = txtUserId.Text
            End If
#End Region

            '-ping to see if there is a connection
            If My.Computer.Network.IsAvailable Then
                If bDEBUG Then Add2Log("DEBUG::TMR: START UPDATING")
                Dim Result As NetworkInformation.PingReply
                Using SendPing As New NetworkInformation.Ping
                    Try
                        Result = SendPing.Send("mijn.telenet.be")
                        If Result.Status = NetworkInformation.IPStatus.Success Then
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
                            mnuMobile.Enabled = False
                            bFUP = False
                            TrayIcon.Icon = My.Resources.icon_normal
                            bError = False
                            BgwError.Message = ""
                            BgwError.Title = ""

                            BGW.RunWorkerAsync()
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
                End Using
                Result = Nothing
            Else
                Add2Log("TMR::CONN: No active connection found")
            End If
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        With My.Settings
            .Lng = cbLanguage.SelectedItem.ToString
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
        'mnuUpdate.Text = "Update TeleData (10s)"
        If Not bOfflineMode Then tmrUpdate.Enabled = True
        'iTimerInterval = CInt(My.Settings.Update) * 6
        btnSave.Enabled = False
        bSaveBox = False
    End Sub

    Private Sub mnuExit_Click(sender As Object, e As EventArgs) Handles mnuExit.Click
        bUnload = True
        Close()
    End Sub

    Private Sub mnuSettings_Click(sender As Object, e As EventArgs) Handles mnuSettings.Click
        Show()
        Activate()
        'Visible = True
        'WindowState = FormWindowState.Normal
    End Sub

    Private Sub mnuGraph_Click(sender As Object, e As EventArgs) Handles mnuGraph.Click
        If bShowGraph Then
            frmGraph.DataSwitch(-1)
        Else
            frmGraph.Show()
            load_lng(frmGraph.Name)
        End If
        frmGraph.Focus()
    End Sub

    Private Sub mnuMobile_Click(sender As Object, e As EventArgs) Handles mnuMobile.Click
        If bShowMobile Then
            frmMobile.LoadData()
        Else
            frmMobile.Show()
            load_lng(frmMobile.Name)
        End If
        frmMobile.Focus()
    End Sub

    Private Sub mnuUpdate_Click(sender As Object, e As EventArgs) Handles mnuUpdate.Click
        mnuUpdate.Enabled = False
        iTimerInterval = tbInterval.Value * 6
        tmrUpdate_Tick(Me, e)
    End Sub

    Private Sub mnuHelpAbout_Click(sender As Object, e As EventArgs) Handles mnuHelpAbout.Click
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
            Process.Start(Application.StartupPath & "\" & Application.ProductName.ToLower & "_history" & If(BETA, "_beta", "") & ".txt", "open")
        Catch ex As Exception
            Add2Log("ERR::MNUHELPHISTORY: " & ex.Message)
        End Try
    End Sub

    Private Sub FrmSettings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If bUnload = True Then
            If BGW.IsBusy Then BGW.CancelAsync()
            If btnSave.Enabled = True Then
                If MsgBox(ErrorMessage(10), CType(MsgBoxStyle.Information & MsgBoxStyle.YesNo, Global.Microsoft.VisualBasic.MsgBoxStyle), ErrorMessageTitle(7)) = MsgBoxResult.Yes Then btnSave_Click(Me, e)
            End If
            If bShowGraph Then frmGraph.Close()
            If bShowLog Then frmLog.Close()

            tmrUpdate.Enabled = False
            TrayIcon.Visible = False
            ClearCookies()
            If Not BETA Then My.Settings.DataSend = mnuSettingsDataSend.Checked

            My.Settings.Update = tbInterval.Value
            If Not mnuSettingsSaveLog.Checked Then dblog.Execute("DELETE FROM tblLog WHERE (UTCtime BETWEEN " & ToUnix(Date.Parse(Date.UtcNow.Day & "/" & Date.UtcNow.Month & "/" & Date.UtcNow.Year & " 00:00:00")) & " AND " & ToUnix(Date.Parse(Date.UtcNow.Day & "/" & Date.UtcNow.Month & "/" & Date.UtcNow.Year & " 23:59:59")) & ");") 'SaveLog()
            If mnuSettingsSave.Checked Then SaveData()
            My.Settings.Use = My.Settings.Use + 1
            My.Settings.Save()
            dblog = Nothing
            dbdata = Nothing
            Application.DoEvents()
            End
        Else
            Dim x(2) As String
            x(0) = Location.X.ToString
            x(1) = Location.Y.ToString
            My.Settings.LocationMain = String.Join(",", x)
            e.Cancel = True
            Hide()
            Exit Sub
        End If
    End Sub

    Private Sub FrmSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
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
        'BETA = True
        If My.Settings.UpdLast = CDate("#12:00:00 AM#") Then My.Settings.UpdLast = Date.UtcNow

        '-check directories and copy files to new directory
        CheckCopyFiles()

#Region "command line"
        Dim arguments As String() = Environment.GetCommandLineArgs()
        For i = 0 To arguments.Length - 1
            If arguments(i).IndexOf("DEBUG") > -1 Then bDEBUG = True
            If arguments(i).IndexOf("NOHTML") > -1 Then bDEBUG_HTML = False
            'If arguments(i).IndexOf("NOMEMCLEAN") > -1 Then bMemClean = False 'doesn't do anything
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
#End Region

        VersionType = 1 '1=freeware 2=test 3=payed, Betaald

        '-load languages and select
        cbLanguage.Items.Clear()
        cbLanguage.Items.Add("Nederlands")
        cbLanguage.Items.Add("Français")
        cbLanguage.Items.Add("English")
        cbLanguage.Items.Add("Deutsch")
        cbLanguage.Enabled = True
        Application.DoEvents()

        Load_MySettings()

#Region "place on screen"
        Dim sError As String = SetWindowSize(My.Settings.LocationMain, Me, False)
        If sError <> "" Then Add2Log("MAIN::LOCATION: " & sError)
#End Region

        IntervalChanged()

        TrayIcon.Icon = My.Resources.icon_normal
        TrayIcon.Text = ErrorMessage(6)

        Text = VersionX()

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
            Text &= " OFFLINE"
            Update_Status()
        Else
            mnuUsage.Visible = False
            ToolStripMenuItem4.Visible = False
        End If
    End Sub

    Private Sub Updatemenu()
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
            Add2Log("UPD_ERR: " & ex.Message & " >> opening update page")
            Process.Start("https://cd-pc.be/prg_update.php?p=15&v=" & VersionX(True).Replace(" ", "%20"), "open")
            bUpdating = False
        End Try
    End Sub
    Private Sub mnuHelpUpdate_Click(sender As Object, e As EventArgs) Handles mnuHelpUpdate.Click
        Updatemenu()
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
        Process.Start(URI_MT_PREFIX, "open")
    End Sub

    Private Sub mnuMijnTelenetConnecties_Click(sender As Object, e As EventArgs) Handles mnuMijnTelenetConnecties.Click
        frmConnections.Show()
        load_lng(frmConnections.Name)
    End Sub

    Private Sub MijnTelenetMention()
        If sMaintenance.Length > 5 Then
            Notify(sMaintenance, ErrorMessageTitle(5), ToolTipIcon.Info)
        End If
    End Sub
    Private Sub mnuMijnTelenetMention_Click(sender As Object, e As EventArgs) Handles mnuMijnTelenetMention.Click
        MijnTelenetMention()
    End Sub

    Private Sub IntervalChanged()
        If OtherText(4) = Nothing Then Exit Sub 'Or mnuUpdate.Enabled = False
        lblInterval.Text = OtherText(4).Replace("{0}", tbInterval.Value.ToString)
        My.Settings.Update = tbInterval.Value
        'iTimerInterval = tbInterval.Value * 6
    End Sub
    Private Sub tbInterval_ValueChanged(sender As Object, e As EventArgs) Handles tbInterval.ValueChanged
        IntervalChanged()
    End Sub

    Private Sub TrayIcon_BalloonTipClicked(sender As Object, e As EventArgs) Handles TrayIcon.BalloonTipClicked
        bMijnTelenet = False
        bBalloon = False
    End Sub

    Private Sub TrayIcon_BalloonTipClosed(sender As Object, e As EventArgs) Handles TrayIcon.BalloonTipClosed
        bMijnTelenet = False
        bBalloon = False
    End Sub

    Private Sub TrayIcon_MouseClick(sender As Object, e As MouseEventArgs) Handles TrayIcon.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Left Then
            frmPopup.ShowTip(5, Application.ProductName,
                                    TrayIconText.Replace(Application.ProductName & Environment.NewLine, "") & If(sIpWanV4.Length > 0 Or sIpWanV6.Length > 0, Environment.NewLine &
                                            OtherText(5).Replace("\n", Environment.NewLine).Replace("{0}", sIpWanV4).Replace("{1}", sIpWanV6).Replace("{2}", sIpWanV6Prefix), ""),
                                            ToolTipIcon.Info)
        End If
    End Sub

    Private Sub TrayIcon_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles TrayIcon.MouseDoubleClick
        If e.Button = Windows.Forms.MouseButtons.Left And mnuGraph.Enabled Then mnuGraph_Click(Me, e)
    End Sub

    Private Sub txtUserId_TextChanged(sender As Object, e As EventArgs) Handles txtUserId.TextChanged
        Textbox_Validate()
    End Sub

    Private Sub mtbPassword_TextChanged(sender As Object, e As EventArgs) Handles mtbPassword.TextChanged
        Textbox_Validate()
    End Sub

    Private Sub cbLanguage_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbLanguage.SelectedIndexChanged
        '-change the language of the programm
        CurrentLng = cbLanguage.SelectedItem.ToString
        load_lng(Name)
        If Not OtherText(4) Is Nothing Then lblInterval.Text = OtherText(4).Replace("{0}", tbInterval.Value.ToString)
        If pData.Usage_Percentage >= 0 Then Update_Status()
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
        Update_Status()
    End Sub

    Private Sub mnuSettingsSave_Click(sender As Object, e As EventArgs) Handles mnuSettingsSave.Click
        btnSave.Enabled = True
    End Sub

    Private Sub mnuSettingsDataSend_Click(sender As Object, e As EventArgs) Handles mnuSettingsDataSend.Click
        If Not BETA Then My.Settings.DataSend = mnuSettingsDataSend.Checked
    End Sub

    Private Sub mnuSettingsDataSavedData_Click(sender As Object, e As EventArgs) Handles mnuSettingsDataSavedData.Click
        Process.Start(Directory.GetParent(Application.UserAppDataPath).ToString())
    End Sub

    Private Sub mnuSettingsDataImport_Click(sender As Object, e As EventArgs) Handles mnuSettingsDataImport.Click
        frmImport.Show()
    End Sub

    Private Sub mnuSettingsBeta_Click(sender As Object, e As EventArgs) Handles mnuSettingsBeta.Click
        If Not BETA Then My.Settings.Upd2Beta = mnuSettingsBeta.Checked
    End Sub

    Private Sub DoWork(sender As Object, e As DoWorkEventArgs) Handles BGW.DoWork
        'Private Sub BgwDoWork()
        Dim m As Match
        Dim iLogId As Integer
        Try
            ClearCookies()
            iLogId = Add2Log("Fetch: Mijn Telenet")
            sOutput = Http_Request(URI_MT_PREFIX, URI_MT_USAGE,,,, iLogId)
            If bTimeOut Then
                bError = True
                BgwError.Message = ErrorMessage(8)
                BgwError.Title = "HTTP_REQUEST::TimeOut"
                BgwError.Icon = ToolTipIcon.Error
                Exit Try
            ElseIf sOutput.Length < 5 Then
                Add2Log("BGW_DOWRK_ERR::NO_OUTPUT", iLogId)
                bError = True
                BgwError.Icon = ToolTipIcon.Error
                BgwError.Message = ""
                BgwError.Title = ""
                Exit Try
            End If
#Region "Allround Error"
            If sOutput.IndexOf("contentContainer") > 0 And sOutput.IndexOf("Fout bij") > 0 Then
                m = Regex.Match(sOutput, "<p>(.*)</p>")
                If m.Success Then
                    bError = True
                    BgwError.Message = m.Groups(1).Value
                    BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    BgwError.Icon = ToolTipIcon.Error
                End If
                Exit Try
            End If
#End Region
            BGW.ReportProgress(1)
#Region "check if there is a maintenance"
            If sOutput.IndexOf("inputContainer") > 0 And mnuSettingsShowMyTelenet.Checked Then
                m = Regex.Match(sOutput, "inputContainer"">(.*)<\/div>", RegexOptions.Singleline)
                If m.Success Then
                    Dim tmp As String = m.Groups(1).Value.Substring(0, m.Groups(1).Value.IndexOf("</div>") - 1).Replace(". ", "." & Environment.NewLine).Trim()
                    If tmp.IndexOf("forgotLogin") = -1 Then
                        If tmp <> sMaintenance Then
                            bMaintenance = True
                            sMaintenance = tmp
                        End If

                        '-get maintenace dates
                        If sMaintenance.IndexOf("verbeteringswerken") > -1 Then
                            Dim gmdthr As Thread = New Thread(AddressOf GetMaintenanceDates)
                            With gmdthr
                                .Name = "gmdthr"
                                .IsBackground = True
                                .Start()
                            End With
                        End If
                    Else
                        sMaintenance = ""
                        MaintenanceDateStart = Date.Parse("01/07/2016")
                        MaintenanceDateStop = Date.Parse("01/07/2016")
                    End If
                    tmp = Nothing
                End If
            Else
                sMaintenance = ""
            End If
#End Region
            BGW.ReportProgress(10)

            '-crawl the website
            If (sOutput.IndexOf("j_username") = -1) Or (sOutput.IndexOf("j_password") = -1) Then
                bError = True
                BgwError.Icon = ToolTipIcon.Info
                If sMaintenance.Length > 10 And MaintenanceDateStart <= Date.Now AndAlso MaintenanceDateStop >= Date.Now Then
                    Add2Log("Maintenance busy, expected stop date/hour: " & MaintenanceDateStop)
                    TrayIcon.Text = ErrorMessage(7)
                    TrayIconText = ErrorMessage(7)
                Else
                    Add2Log("Login disabled: check " & URI_MT_PREFIX & " for more information")
                    BgwError.Message = ErrorMessage(2).Replace("{0}", URI_MT_PREFIX)
                    BgwError.Title = ErrorMessageTitle(2) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                End If
                Exit Try
            End If
            iLogId = -1
            BGW.ReportProgress(10)
            If Not Goto_Login(iLogId) Then
                '-login fail
                bError = True
                If (sOutput.IndexOf("class=""error""") > 0) And (sOutput.IndexOf("gebruikersnaam en/of wachtwoord") > 0) Then
                    Add2Log("Authentication failed: Wrong TelenetID/password", iLogId)
                    BgwError.Message = ErrorMessage(13)
                    BgwError.Title = ErrorMessageTitle(0) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    BgwError.Icon = ToolTipIcon.Warning
                Else
                    Add2Log("Authentication failed something else went wrong:" & Environment.NewLine & sOutput, iLogId)
                    BgwError.Message = ErrorMessage(0)
                    BgwError.Title = ErrorMessageTitle(0) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    BgwError.Icon = ToolTipIcon.Info
                End If
                Exit Try
            End If

            If sOutput.IndexOf("Je gegevens worden opgehaald") = -1 Then
                '-login fail
                bError = True
                BgwError.Message = ErrorMessage(11)
                BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                BgwError.Icon = ToolTipIcon.Error
                Add2Log("Telenet site error")
                Exit Try
            End If
            BGW.ReportProgress(20)
            If Not ChkResponse() Then
                '-response fail
                bError = True
                BgwError.Message = ErrorMessage(11)
                BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                BgwError.Icon = ToolTipIcon.Error
                Add2Log("Reponse fail", iLogId)
                Exit Try
            End If

            iLogId = Add2Log("Fetch: Response")
            '-callback fix
            For i = 0 To 3
                sOutput = Http_Request(sResponseUrl, URI_MT_USAGE,,,, iLogId)
                If sResponseUrl.IndexOf("callback.do") = -1 Then Exit For
            Next i
            If Not ChkResponse() Then
                '-response fail
                bError = True
                BgwError.Message = ErrorMessage(11)
                BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                BgwError.Icon = ToolTipIcon.Error
                Add2Log("Reponse fail", iLogId)
                Exit Try
            End If

            iLogId = Add2Log("Fetch: Main Page")
            sOutput = Http_Request(sResponseUrl, URI_MT_USAGE,,,, iLogId)
            If bTimeOut Or (sOutput.IndexOf("Je gegevens worden opgehaald") > -1) Then
                bError = True
                BgwError.Message = ErrorMessage(8)
                BgwError.Title = "HTTP_REQUEST::TimeOut"
                BgwError.Icon = ToolTipIcon.Error
                Exit Try
            End If

#Region "get logout link"
            m = Regex.Match(sOutput, "id=\""logoutbutton\"" href=\""(.*?)\"">")
            If m.Success Then
                URI_SSO_SIGNOFF = m.Groups(1).Value
                If URI_SSO_SIGNOFF.IndexOf("http") = -1 Then
                    URI_SSO_SIGNOFF = URI_SSO_SIGNOFF.Replace("/mijntelenet/", URI_MT_PREFIX)
                End If
            End If
#End Region
#Region "debug down+uploads"
            If bTeleUp Then
                Dim telthr As Thread = New Thread(New ThreadStart(AddressOf SendDbgData))
                With telthr
                    .Name = "telthr"
                    .IsBackground = True
                    .Start()
                End With
            End If
#End Region
            '-do the rest starting with the mobile abos
#Region "Mobile"
            If sOutput.IndexOf("King") > 0 Or sOutput.IndexOf("Kong") > 0 Then
                Dim mobthr As Thread = New Thread(New ThreadStart(AddressOf GetMobile))
                With mobthr
                    .Name = "mobthr"
                    .IsBackground = True
                    .Start()
                End With
            End If
#End Region
            '-get advanced settings = ip adresses
            Dim advthr As Thread = New Thread(New ThreadStart(AddressOf Goto_Advsettings))
            With advthr
                .Name = "advthr"
                .IsBackground = True
                .Start()
            End With

            '-get the telemeter page
            If sOutput.IndexOf("Telemeter") = -1 Then
                bError = True
                BgwError.Message = ErrorMessage(11)
                BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                BgwError.Icon = ToolTipIcon.Error
                Add2Log("Telenet site error", iLogId)
                Exit Try
            End If

            '-Is it FUP?
            bFUP = False
            If sOutput.IndexOf("Business Fibernet") = -1 And (
                    sOutput.IndexOf("Internet 160") > 0 Or sOutput.IndexOf("Fibernet XL") > 0 Or
                    sOutput.IndexOf("Fibernet 200") > 0 Or sOutput.IndexOf("Fiber 200") > 0) Then
                bFUP = True
            End If
            BGW.ReportProgress(50)
            m = Regex.Match(sOutput, "href=""(.*?)"">Telemeter")
            If m.Success Then
                URI_MT_USAGE = m.Groups(1).Value.Replace("/mijntelenet/", URI_MT_PREFIX)
                iLogId = Add2Log("Fetch: Telemeter")
                pData.InputData = Http_Request(URI_MT_USAGE, URI_MT_USAGE,,,, iLogId)
                'If sOutput.IndexOf("totalUsagePercentage") = -1 Then
                '    sOutput = http_request("https://www2.telenet.be/nl/klantenservice/raadpleeg-je-internetverbruik/",,,,ilogid)
                'End If
                If BETA Or mnuSettingsDataSend.Checked Then
                    Dim sopthr As Thread = New Thread(New ThreadStart(AddressOf SendOnePage))
                    With sopthr
                        .Name = "sopthr"
                        .IsBackground = True
                        .Start()
                    End With
                End If
            Else
                bError = True
                BgwError.Message = ErrorMessage(11)
                BgwError.Title = ErrorMessageTitle(8) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss")
                BgwError.Icon = ToolTipIcon.Error
                Add2Log("Telemeter url error", iLogId)
            End If
            BGW.ReportProgress(80)
#Region "Logout"
            iLogId = Add2Log("Fetch: Logout")
            Dim params As String = If(URI_SSO_SIGNOFF.IndexOf("?") > -1, "", "goto=" & URI_MT_PREFIX)
            sOutput = Http_Request(URI_SSO_SIGNOFF, params,,,, iLogId)
            params = Nothing
            If bTimeOut Then Exit Try
            m = Regex.Match(sOutput, ".URL=(.*)""")
            If m.Success Then sOutput = Http_Request(m.Groups(1).Value, URI_MT_USAGE,,,, iLogId)
#End Region
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
        End Try
        sOutput = String.Empty
        m = Nothing

        '  BgwDone()
    End Sub

    Private Sub RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BGW.RunWorkerCompleted
        'Private Sub BgwDone()
        ' If InvokeRequired Then
        'Invoke(New Action(Sub() BgwDone()))
        'Return
        'End If


        'If e.Cancelled = True Then
        '    sBGWtatus = "1" 'cancel
        '    bError = True
        'ElseIf e.Error IsNot Nothing Then
        '    bError = True
        '    sBGWtatus = e.Error.Message
        '    Add2Log("BGW_ERROR: " & e.Error.Message)
        'Else
        'sBGWtatus = "2" 'done
        'End If

        sBGWstatus = "0"
        iTimerInterval = 0

        '- Maintenance changed
        If bMaintenance Then
            If CBool(My.Settings.ShowBalloon) Then Notify(sMaintenance, ErrorMessageTitle(5), ToolTipIcon.Info)
            bMaintenance = False
        End If

        If bError Then
            If BgwError.Message.Length > 0 And BgwError.Title.Length > 0 And CBool(My.Settings.ShowBalloon) Then Notify(BgwError.Message, BgwError.Title, BgwError.Icon)
        Else
            '- parse it
            Add2Log("Parsing")
            With pData
                .Parse(bFUP)
                If bDEBUG Then
                    Add2Log("DEBUG: Customer id: " & .CustomerNr &
                    " Customer Acc.:" & .CustomerBillAccountNr &
                    " TeleDate: " & .TeleDate &
                    " TeleReset: " & .TeleReset &
                    " DataRange: " & .DateRangeId &
                    " Max Day Usage: " & .Usage_DayMax &
                    " Usage: " & .Usage_MBUsed & "/" & .Usage_MBLimit & " (" & .Usage_Percentage & "%)")
                End If

#Region "Check period in db"
                If db_UserId > 0 Then
                    Dim tmp As String() = .DateRangeId.Replace(" tot en met ", "|").Split(New Char() {"|"c})
                    If tmp(0) <> PeriodStartDate Then
                        PeriodStartDate = ParseDate(tmp(0))
                        If CInt(dbdata.GetValue("SELECT count(UserId) FROM tblPeriod WHERE UserId=" & db_UserId & " AND StartDate='" & PeriodStartDate & "' AND EndDate='" & ParseDate(tmp(1)) & "';")) < 1 Then
                            dbdata.Execute("INSERT INTO tblPeriod (UserId,StartDate,EndDate)VALUES(" & db_UserId & ",'" & PeriodStartDate & "','" & ParseDate(tmp(1)) & "');")
                            Add2Log("BGW::DB: Add period -  " & .DateRangeId.Replace("tot en met", "-"))
                        End If
                    End If
                    tmp = Nothing
                End If
#End Region
#Region "enable menu's"
                If .Usage_Percentage < 0 Then
                    If CBool(My.Settings.ShowBalloon) Then Notify(ErrorMessage(6), "Check::ParseError " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss"), ToolTipIcon.Error)
                ElseIf Not IsNothing(.Data(0)) Or .Usage_Percentage >= 0 Then
                    mnuGraph.Enabled = True
                    If .Usage_Percentage >= 0 Then Update_Status()
                    If bShowGraph Then frmGraph.DataSwitch(-1)
                    If MobAbo.Count > 0 AndAlso MobAbo(0).Cell IsNot Nothing Then
                        mnuMobile.Enabled = True
                        If bShowMobile Then frmMobile.LoadData()
                    End If
                End If
#End Region
                If mnuSettingsShowMyTelenet.Checked And .CustomerBillAccountNr > 0 And CBool(My.Settings.ShowBalloon) Then
                    Notify(ErrorMessage(5).Replace("\n", Environment.NewLine), ErrorMessageTitle(4) & " " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss"), ToolTipIcon.Info)
                End If
            End With
        End If

        txtUserId.Enabled = True
        mtbPassword.Enabled = True
        tbInterval.Enabled = True
        cbLanguage.Enabled = True
        If sMaintenance.Length > 5 Then
            If bMaintenance And mnuSettingsShowMyTelenet.Checked And CBool(My.Settings.ShowBalloon) Then MijnTelenetMention()
            mnuMijnTelenetMention.Enabled = True
        Else
            mnuMijnTelenetMention.Enabled = False
        End If
        Dim FileToSaveTo As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, txtUserId.Text & "_advancedsettings.htm")
        If File.Exists(FileToSaveTo) Then mnuMijnTelenetConnecties.Enabled = True
        FileToSaveTo = Nothing

        mnuUpdate.Text = OtherText(0).Replace("{0}", tbInterval.Value & "min")
        If mnuSettingsSave.Checked Then
            If CInt(Date.UtcNow.ToString("HH")) = 23 Or pData.Usage_Percentage = 100 Then SaveData()
        End If
        Add2Log("Done updating")

#Region "check for an program update"
        Dim _time As TimeSpan = Date.UtcNow - Date.Parse(My.Settings.UpdLast.ToString)
        If (My.Settings.UpdChk = 1 And _time.TotalHours > 24) Or (My.Settings.UpdChk = 2 And _time.TotalDays > 7) Or (My.Settings.UpdChk = 3 And _time.TotalDays > 30) Then
            If bDEBUG Then Add2Log("DEBUG: check for prg update")
            '-update
            My.Settings.UpdLast = Date.UtcNow
            Updatemenu()
        End If
        _time = Nothing
#End Region

        'If bMemClean Then SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1)
        GC.Collect()
        'bgw = Nothing
    End Sub

    Private Sub Bgw_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BGW.ProgressChanged
        If bShowLog Then frmLog.UpdateLogView()
    End Sub

    '-functions and special subs
    Private Sub Update_Status()
        Dim iLogId As Integer = Add2Log("Updating the status indicator")
        Try
            Dim iPercentage As Short = pData.Usage_Percentage
#Region "menu background color and trayicon"
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
                If mnuSettingsDynIcon.Checked Then iPercentage = CShort(pData.Usage_Percentage_Day) '-dynamicly icon ;)
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
#End Region
            iPercentage = Nothing

            Dim sUsageText As String = OtherText(3).Replace("{0}", pData.TeleReset.ToString("dd/MM")).Replace("{1}", pData.Usage_DateDiff.ToString)
            If (pData.Usage_MBUsed > -1) And pData.Usage_MBLimit > -1 Then
                sUsageText = sUsageText & Environment.NewLine &
                    OtherText(2).Replace("{0}", Convert_MB(pData.Usage_MBUsed, My.Settings.ShowBinair) & "/" & Convert_MB(pData.Usage_MBLimit, My.Settings.ShowBinair) & " (" & pData.Usage_Percentage & "%)") _
                    .Replace("{1}", Convert_MB(pData.Usage_Today, My.Settings.ShowBinair) & "/" & Convert_MB(CLng((pData.Usage_MBDiff + pData.Usage_Today) / pData.Usage_DateDiff), My.Settings.ShowBinair) & " (" & pData.Usage_Percentage_Day & "%)").Replace("\n", Environment.NewLine)
                mnuUsage.Text = Convert_MB(pData.Usage_MBUsed, My.Settings.ShowBinair) & "/" & Convert_MB(pData.Usage_MBLimit, My.Settings.ShowBinair) & " (" & pData.Usage_Percentage & "%)"
                mnuUsage.Visible = True
                ToolStripMenuItem4.Visible = True
            Else
                '-something went wrong
                sUsageText = sUsageText & Environment.NewLine & "Usage: " & Convert_MB(pData.Usage_MBUsed, My.Settings.ShowBinair)
            End If
            SetNotifyIconText(TrayIcon, sUsageText)
            TrayIconText = sUsageText
            sUsageText = Nothing
        Catch ex As Exception
            Add2Log("UPDATE_STATUS_ERR:" & If(bFUP, ":bfup: ", " ") & ex.Message, iLogId)
        End Try
    End Sub

    Private Function Goto_Login(ByRef LogId As Integer) As Boolean
        LogId = Add2Log("Fetch: goto_Login")
        Dim sGoto As String = URI_MT_NAVIGATION & "?family=DEFAULT&identifier=DEFAULT"
        Dim Params As String = "goto=" & sGoto &
            "&j_username=" & My.Settings.UserId &
            "&j_password=" & My.Settings.Pass &
            "&rememberme=true&language=nl&appid=mijntelenet_cms&errgoto=" & URI_MT_LOGIN & "?goto=" & sGoto
        sOutput = Http_Request(URI_SSO_SIGNON, URI_MT_USAGE, Params, True,, LogId)
        'Dim m As RegularExpressions.Match
        If bTimeOut Then Return False
        If Not (sOutput.IndexOf("authfail") > -1) And (sOutput.IndexOf(sGoto) > -1 Or sOutput.IndexOf("Je gegevens worden opgehaald.") > -1) Then
            Goto_Login = True
        ElseIf (sOutput.PadLeft(1) = "0") Then
            bError = True
            BgwError.Message = Mid(sOutput, 2, Len(sOutput) - 1)
            BgwError.Title = "Login()"
            BgwError.Icon = ToolTipIcon.Error
            Goto_Login = False
        Else
            Add2Log("Goto_login_err", LogId)
            Goto_Login = False
        End If
        sGoto = Nothing
        Params = Nothing
    End Function

    Private Function ChkResponse() As Boolean
        '-very simple response url check
        If sResponseUrl.IndexOf(URI_MT_PREFIX) = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub Goto_Advsettings()
        Dim iLogId As Integer = Add2Log("Fetch: AdvSettings")
        Dim sOutput2 = Http_Request(URI_MT_PREFIX & "rgw/settings.do?action=showAdvancedSettings&identifier=" & txtUserId.Text, URI_MT_USAGE,,,, iLogId)
        Try
            Dim arrLines As String() = sOutput2.Split(CChar(Environment.NewLine))
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
            arrLines = Nothing
        Catch ex As Exception
            Add2Log("ADVSETTINGS::PARSE_ERR: " & ex.Message, iLogId)
            sOutput2 = ""
        End Try
        Try
            If sOutput2 <> "" Then
                Using objWriter As New StreamWriter(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, txtUserId.Text & "_advancedsettings.htm"), False)
                    objWriter.Write(sOutput2)
                End Using
            Else
                File.Delete(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, txtUserId.Text & "_advancedsettings.htm"))
            End If
        Catch Ex As Exception
            Add2Log("ADVSETTINGS::SAVE_ERR: " & Ex.Message, iLogId)
        End Try
        sOutput2 = Nothing
        iLogId = Nothing
    End Sub

    Private Sub Textbox_Validate()
        Dim m As Match
        Dim m1 As Match
        form_valid = True

        m = Regex.Match(mtbPassword.Text, "[a-z0-9]")
        If m.Success And (mtbPassword.Text.Length >= 6 And mtbPassword.Text.Length <= 32) Then
            mtbPassword.BackColor = ColorTranslator.FromHtml("#CCFFC0")
        Else
            mtbPassword.BackColor = ColorTranslator.FromHtml("#FFC0C0")
            form_valid = False
        End If
        m = Regex.Match(txtUserId.Text, "[a-z]{1}[0-9]{6}")
        m1 = Regex.Match(txtUserId.Text, "[a-z]{2}[0-9]{5}")
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