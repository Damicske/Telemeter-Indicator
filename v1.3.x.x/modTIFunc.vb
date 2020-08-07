Imports System
Imports System.IO
Imports System.Threading

Module modTIFunc
    Public pData As New TeleParse
    Public sOutput As String, sLog As String = ""
    Public bShowLog As Boolean = False, bShowGraph As Boolean = False, bFUP As Boolean = False
    Public bDEBUG As Boolean = False, bDEBUG_HTML As Boolean = True, bOfflineMode As Boolean = False, bUpdating As Boolean = False

    Public Sub CheckCopyFiles()
        '-check if directories exist
        If Not Directory.Exists(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "data")) Then Directory.CreateDirectory(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "data"))
        If Not Directory.Exists(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "pre1.0.7.12_logs")) Then Directory.CreateDirectory(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "pre1.0.7.12_logs"))
        If Not Directory.Exists(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "logs")) Then Directory.CreateDirectory(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "logs"))

        '-move files if needed
        If Directory.Exists(Path.Combine(Application.StartupPath, "data")) Then
            Add2Log("Moving data files to new directory")
            My.Computer.FileSystem.CopyDirectory(Path.Combine(Application.StartupPath, "data"), Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "data"), True)
            Directory.Delete(Path.Combine(Application.StartupPath, "data"), True)
        End If
        If Directory.Exists(Path.Combine(Application.StartupPath, "logs")) Then
            Add2Log("Moving log files to new directory")
            My.Computer.FileSystem.CopyDirectory(Path.Combine(Application.StartupPath, "logs"), Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "pre1.0.7.12_logs"), True)
            Directory.Delete(Path.Combine(Application.StartupPath, "logs"), True)
        End If
    End Sub

    Public Sub Add2Log(ByVal sInput As String, Optional ByVal bNewLine As Boolean = True)
        sInput = IIf(bNewLine, "[" & Date.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") & "UTC] ", " >> ") & sInput.Replace(Environment.NewLine, "") '.Replace(vbCr, "")'.Replace(vbLf, "")
        sLog &= IIf(sLog <> "" And bNewLine, Environment.NewLine, "") & sInput
        If bShowLog Then ' And BGW.IsBusy = False Then
            Add2LogText(sInput)
        End If
    End Sub

    Public Delegate Sub SetTextCallback(ByVal txt As String)
    Public Sub Add2LogText(ByVal txt As String)
        Try
            If frmLog.txtLog.InvokeRequired Then
                frmLog.txtLog.BeginInvoke(New SetTextCallback(AddressOf Add2LogText), txt)
            Else
                frmLog.color_log_line(IIf(txt.IndexOf("[") = 0 And frmLog.txtLog.TextLength > 0, Environment.NewLine, "") & txt)
            End If
        Catch ex As Exception
            Console.WriteLine("Add2logtext: " & ex.Message)
        End Try
    End Sub

    Public Sub CleanOldSettings()
        Dim VersionPath As String = ""
        Try
            Dim SettingsPath As String = Directory.GetParent(Directory.GetParent(Application.LocalUserAppDataPath).ToString).ToString
            Dim DirLst As New List(Of String)
            For Each Dir As String In Directory.GetDirectories(SettingsPath)
                If Dir.IndexOf(My.Application.Info.AssemblyName.Replace(" ", "_")) > -1 Then DirLst.Add(Dir)
            Next
            SettingsPath = Nothing
            Dim VerLst As New List(Of String)

            For i = 0 To DirLst.Count - 1
                For j = 0 To Directory.GetDirectories(DirLst(i)).Length - 2
                    VersionPath = Directory.GetDirectories(DirLst(i))(j).ToString
                    If New DirectoryInfo(VersionPath).Name < Application.ProductVersion Then
                        Directory.Delete(VersionPath, True)
                    End If
                Next
            Next i
            DirLst = Nothing
        Catch ex As Exception
            MsgBox(ex.Message & Environment.NewLine & "PATH: " & VersionPath, , "CleanOldSettings::ERR")
        End Try
        VersionPath = Nothing
        GC.Collect()
    End Sub

    Public Sub SaveData()
        'userid,periode(start,end),date,data basic,extra,wifree,day limit
        'ex: a123456;18032015;17042015;18/03;2642;0;0;0
        Dim sOutput As String = ""
        If pData.Graph_Basic Is Nothing Then Exit Sub
        Try
			For i = 0 To pData.Graph_Basic.Length - 1
				sOutput &= frmSettings.txtUserId.Text & ";" & pData.dateRangeId.Replace("/", "").Replace(" tot en met ", ";") &
						";" & pData.Graph_Day(i) &
						";" & pData.Graph_Basic(i) &
                        ";" & IIf(pData.Graph_Extra.Length < i, 0, pData.Graph_Extra(i)) &
                        ";" & IIf(pData.Graph_WiFree.Length < i, 0, pData.Graph_WiFree(i)) &
                        ";" & IIf(pData.Graph_Limit.Length < i, 0, pData.Graph_Limit(i)) & Environment.NewLine
			Next i
            Using objWriter As New StreamWriter(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "data\" & frmSettings.txtUserId.Text & "_" & pData.dateRangeId.Replace("/", "").Replace(" tot en met ", "-") & ".txt"), False)
                objWriter.Write(sOutput)
            End Using
        Catch Ex As Exception
            MsgBox(Ex.Message, , "Error: SaveData")
            Add2Log("ERR::SAVEDATA:" & Ex.Message)
        End Try
    End Sub

    Public Function SaveLog(Optional SaveDateDay As Int16 = 0, Optional ByRef FileToSaveTo As String = "")
        If sLog = "" Then Return True

        Try
            FileToSaveTo = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "logs\")
            ' If SaveDateDay <> 0 Then
            'FileToSaveTo &= IIf(SaveDateDay < 10, "0", "") & SaveDateDay '-add leading zero 
            ' Else
            ' FileToSaveTo &= Date.UtcNow.ToString("dd")
            ' End If
            FileToSaveTo &= Date.UtcNow.ToString("dd-MM-yyyy") & "UTC_TelemeterIndicator.log"

            Using objWriter As New StreamWriter(FileToSaveTo, File.Exists(FileToSaveTo))
                objWriter.Write(sLog)
            End Using
            Return True
        Catch Ex As Exception
            MsgBox(Ex.Message, MsgBoxStyle.Exclamation, "Error: SaveLog")
            Return False
        End Try
    End Function

    Public Sub SendOnePage(sOutput2 As String())
        '-send the page to me :)
        If pData.CustomerNr = 0 Then Exit Sub
        Dim sFileName As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, My.Settings.UserId & ".html")
        Using file As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(sFileName, True)
            For i = 0 To sOutput2.Length - 1
                If sOutput2(i) <> Environment.NewLine Then file.WriteLine(sOutput2(i))
            Next i
        End Using
        Dim params As String = "verzonden=upload&v=2&uid=" & My.Settings.UserId & "&aid=" & pData.CustomerNr
        Add2Log(frmSettings.http_request("https://www.cd-pc.be/ti_stats/index.php", params, True, sFileName), False)
        File.Delete(sFileName)
    End Sub

    Public Sub Notify(ByVal tipText As String, ByVal tipTitle As String, Optional nIcon As ToolTipIcon = ToolTipIcon.Info)
        frmSettings.TrayIcon.ShowBalloonTip(10000, tipTitle, tipText, nIcon)
        If nIcon = ToolTipIcon.Error Then frmSettings.TrayIcon.Icon = My.Resources.icon_normal
    End Sub

    Public Sub load_mysettings()
        '-load other settings
        With My.Settings
            frmSettings.mtbPassword.Text = .Pass
            frmSettings.txtUserId.Text = .UserId
            frmSettings.mnuSettingsStartup.Checked = CBool(.StartUp)
            frmSettings.mnuSettingsSaveLog.Checked = CBool(.SaveLog)
            frmSettings.mnuSettingsShowMyTelenet.Checked = CBool(.ShowMT)
            frmSettings.mnuSettingsShowBinair.Checked = CBool(.ShowBinair)
            frmSettings.mnuSettingsDynIcon.Checked = CBool(.ShowDyn)
            frmSettings.mnuSettingsSave.Checked = CBool(.SaveData)
            frmSettings.tbInterval.Value = CInt(.Update)
        End With

        For i = 0 To frmSettings.cbLanguage.Items.Count - 1
            If frmSettings.cbLanguage.Items(i).ToString = My.Settings.Lng Then
                frmSettings.cbLanguage.SelectedIndex = i
                Exit For
            End If
        Next i
    End Sub

    Public loadthr As Thread
    Public Sub ThreadTaskLogFile()
        '-load old "log" file
        Try
            If Directory.Exists(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "logs")) Then
                Dim LogFile As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "logs\")
                LogFile &= Date.UtcNow.ToString("dd-MM-yyyy") & "UTC_TelemeterIndicator.log"
                If File.Exists(LogFile) Then
                    Using objReader As New StreamReader(LogFile)
                        While objReader.Peek > 0
                            sLog &= objReader.ReadLine() & Environment.NewLine
                        End While
                    End Using
                    sLog &= Environment.NewLine & "-----" & VersionX() & "-----"
                End If
            End If
        Catch ex As Exception
            Add2Log("LOAD_THR::ERR: " & ex.Message)
        End Try
    End Sub
	
    Public Sub ThreadUpdate()
        '- open https://cd-pc.be/prg_update.php?p=15&RAW&v=" & VersionX(True).Replace(" ", "%20")
        '- check if new version is available
        '- if yes then download to Path.GetTempPath() in other thread
        '- if downloaded check file md5
        '- run the installer in verysilent mode, close program
        If My.Computer.Network.IsAvailable Then
            Dim Result As Net.NetworkInformation.PingReply
            Dim SendPing As New Net.NetworkInformation.Ping
            Try
                Result = SendPing.Send("cd-pc.be")
                If Result.Status <> Net.NetworkInformation.IPStatus.Success Then
                    bUpdating = False
                    Exit Sub
                Else
                    Add2Log("PING " & Result.RoundtripTime & "ms")
                End If
            Catch ex As Exception
                Add2Log("CHK::Ping problem: " & ex.Message, False)
                bUpdating = False
                Exit Sub
            End Try
        Else
            Add2Log("CHK: No network available", False)
            bUpdating = False
            Exit Sub
        End If

        Dim sOut As String = frmSettings.http_request("https://cd-pc.be/prg_update.php?p=15&RAW&v=" & VersionX(True).Replace(" ", "%20"))
        If sOut.IndexOf("ERR") > -1 Then
            Add2Log(sOut, False)
            bUpdating = False
            Exit Sub
        End If

        If sOut.IndexOf("{") = 0 Then
            '-update data
            Dim sName As String = "", sVersion As String = "", sDate As String = "", sUrl As String = "", sDownurl As String = "", sMD5 As String = "", bBeta As Boolean = False, bUpdate As Boolean = False
            Dim arguments As String() = sOut.Split(New Char() {";"})
            For i = 0 To arguments.Length - 1
                If arguments(i).IndexOf("program_name") > -1 Then sName = arguments(i).Substring(arguments(i).IndexOf("=") + 1)
                If arguments(i).IndexOf("program_version") > -1 Then sVersion = arguments(i).Substring(arguments(i).IndexOf("=") + 1)
                If arguments(i).IndexOf("program_date") > -1 Then sDate = arguments(i).Substring(arguments(i).IndexOf("=") + 1)
                If arguments(i).IndexOf("program_url") > -1 Then sUrl = arguments(i).Substring(arguments(i).IndexOf("=") + 1)
                If arguments(i).IndexOf("program_downurl") > -1 Then sDownurl = arguments(i).Substring(arguments(i).IndexOf("=") + 1)
                If arguments(i).IndexOf("program_md5") > -1 Then sMD5 = arguments(i).Substring(arguments(i).IndexOf("=") + 1)
                If arguments(i).IndexOf("program_beta") > -1 Then bBeta = CBool(arguments(i).Substring(arguments(i).IndexOf("=") + 1))
                If arguments(i).IndexOf("update_needed") > -1 Then bUpdate = CBool(arguments(i).Substring(arguments(i).IndexOf("=") + 1))
            Next
            arguments = Nothing

            If sName <> My.Application.Info.Title Then
                Add2Log("CHK: Program names aren't correct", False)
                bUpdating = False
                Exit Sub
            End If

            If bUpdate Then
                Try
                    Add2Log("Updating to " & sVersion & IIf(bBeta, "beta", ""), False)
                    frmPopup.ShowTip(5, "Update", "Updating Telemeter indicator to " & sVersion & IIf(bBeta, "beta", ""), ToolTipIcon.Info)
                    My.Computer.Network.DownloadFile("https://www.cd-pc.be/" & sDownurl, Path.Combine(Path.GetTempPath, My.Application.Info.Title & ".exe"), "", "", True, 100000, True)
                    While Not File.Exists(Path.Combine(Path.GetTempPath, My.Application.Info.Title & ".exe"))
                        Thread.Sleep(10)
                    End While

                    Using f As FileStream = New FileStream(Path.Combine(Path.GetTempPath, My.Application.Info.Title & ".exe"), FileMode.Open, FileAccess.Read, FileShare.Read, 8192)
                        '-check file hashes
                        If GetMD5FileHash(f) = sMD5 Then
                            frmSettings.bUnload = True
                            System.Diagnostics.Process.Start(Path.Combine(Path.GetTempPath, My.Application.Info.Title & ".exe"), "/FORCECLOSEAPPLICATIONS /RESTARTAPPLICATIONS /SILENT /DIR=" & Application.StartupPath)
                            Application.Exit()
                        Else
                            Add2Log("CHK: MD5 values do not match", False)
                        End If
                    End Using
                Catch ex As Exception
                    Add2Log("ERR:" & ex.Message)
                End Try
            Else
                Add2Log("No update needed", False)
            End If
        Else
            Add2Log("No valid response", False)
        End If
        bUpdating = False
    End Sub
End Module
