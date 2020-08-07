Imports System.Threading
Imports System.Data.SQLite
Imports System.Data
Imports System.Text.RegularExpressions
Imports System.Net
Imports System.Net.Security
Imports System.Text
Imports System.Security
Imports System.Security.Cryptography.X509Certificates

Module modTIFunc
    '-webrequest data
    Private tempCookies As New CookieContainer
    Private encoding As New UTF8Encoding
    Public sResponseUrl As String = ""
    Public bTimeOut As Boolean = False

    '-Bug rapport vars
    Public bTeleUp As Boolean = False
    Public iBugId As Integer

    '-data
    Public pData As New TeleParse
    Public sOutput As String
    Public bShowLog As Boolean = False, bShowGraph As Boolean = False, bShowMobile As Boolean = False, bFUP As Boolean = False
    Public bDEBUG As Boolean = False, bDEBUG_HTML As Boolean = True, bOfflineMode As Boolean = False, bUpdating As Boolean = False
    Public sMaintenance As String = "", MaintenanceDateStop As Date, MaintenanceDateStart As Date, TrayIconText As String = ""
    Public PeriodStartDate As String
    Public sLastUser As String = ""

    '-db
    Public dbdata As Sqlite
    Public dblog As Sqlite
    Public db_UserId As Integer = -1

    '-mobile abos
    Public Class Mobile
        Private mAbo As String = String.Empty
        Private mCell As String = String.Empty
        Private mUsage As String = String.Empty
        Private mLimit As String = String.Empty
        Private mExtra As String = String.Empty

        Public Property Abo As String
            Get
                Return mAbo
            End Get
            Set(ByVal value As String)
                mAbo = value
            End Set
        End Property
        Public Property Cell As String
            Get
                Return mCell
            End Get
            Set(ByVal value As String)
                mCell = value
            End Set
        End Property
        Public Property Usage As String
            Get
                Return mUsage
            End Get
            Set(ByVal value As String)
                mUsage = value
            End Set
        End Property
        Public Property Limit As String
            Get
                Return mLimit
            End Get
            Set(ByVal value As String)
                mLimit = value
            End Set
        End Property
        Public Property Extra As String
            Get
                Return mExtra
            End Get
            Set(ByVal value As String)
                mExtra = value
            End Set
        End Property
    End Class
    Public MobAbo As New List(Of Mobile)

    Public MainThreadId As Integer = -1

    Public ReadOnly Property Epoch() As Date
        Get
            Return New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        End Get
    End Property

    Public Sub CheckCopyFiles()
        Dim db_data As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "ti_data.db")
        Dim db_log As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "ti_log.db")
        '-Check db stuff
        If Not File.Exists(db_data) Then SQLiteConnection.CreateFile(db_data)
        If Not File.Exists(db_log) Then SQLiteConnection.CreateFile(db_log)

        If dbdata Is Nothing Then dbdata = New Sqlite("Data Source=" & db_data)
        dbdata.Execute("CREATE TABLE if not exists tblUser(UserId INTEGER PRIMARY KEY AUTOINCREMENT, UserName VARCHAR(10));
        CREATE TABLE if not exists tblUsage(Id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, UserId INTEGER, UsageDate DATE, UsageBasic INTEGER, UsageExtra INTEGER, UsageWiFree INTEGER, UsageDayLimit INTEGER);
        CREATE TABLE if not exists tblPeriod(Id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, UserId INTEGER, StartDate DATE, EndDate DATE);
        CREATE TABLE if not exists tblSettings ('Setting'	varchar(50), 'Value'	varchar(50),'Type'	INTEGER DEFAULT 0);")

        If dblog Is Nothing Then dblog = New Sqlite("Data Source=" & db_log)
        dblog.Execute("CREATE TABLE if not exists tblLog(LogId INTEGER PRIMARY KEY AUTOINCREMENT, UTCtime INTEGER, UserName VARCHAR(10), LogType INTEGER DEFAULT 0, Message TEXT);")

        '-update the tables
        If My.Settings.LastDbVersion < 1 Then
            Add2Log("Updating DB To v1")
            dbdata.Execute("CREATE TABLE `sqlitebrowser_rename_column_new_table` (	`Id`	Integer PRIMARY KEY AUTOINCREMENT UNIQUE,   `UserId`	INTEGER,    `StartDate`	DATE,   `EndDate`	DATE);INSERT INTO sqlitebrowser_rename_column_new_table Select `Id`,`UserId`,`StartDate`,`EndDate` FROM `tblPeriod`;PRAGMA foreign_keys;PRAGMA foreign_keys = 0;DROP TABLE `tblPeriod`;ALTER TABLE `sqlitebrowser_rename_column_new_table` RENAME To `tblPeriod`;
                        CREATE TABLE `sqlitebrowser_rename_column_new_table` ('Id' INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE,	`UserId`	INTEGER,	`UsageDate`	DATE ,	`UsageBasic`	INTEGER,	`UsageExtra`	INTEGER,	`UsageWiFree`	INTEGER,	`UsageDayLimit`	INTEGER);INSERT INTO sqlitebrowser_rename_column_new_table SELECT `Id`,`UserId`,`UsageDate`,`UsageBasic`,`UsageExtra`,`UsageWiFree`,`UsageDayLimit` FROM `tblUsage`;PRAGMA foreign_keys;PRAGMA foreign_keys = 0;DROP TABLE `tblUsage`;ALTER TABLE `sqlitebrowser_rename_column_new_table` RENAME TO `tblUsage`")
            CheckUsageDataOnDouble()
            My.Settings.LastDbVersion = 1
        End If
    End Sub

    Public Function Add2Log(ByVal sInput As String, Optional UpdateLogEntry As Integer = -1) As Integer
        Dim logtype As Short = 0
        If (sInput.ToLower.IndexOf("<html") > -1) Then
            UpdateLogEntry = -1
            logtype = 2
        ElseIf sInput.ToLower.IndexOf("timeout") > -1 Or sInput.IndexOf("TMR::") > -1 Or sInput.IndexOf("no or high") > -1 Or
                sInput.IndexOf("No active") > -1 Or sInput.ToLower.IndexOf("err") > -1 Or sInput.IndexOf("fail") > -1 Then
            logtype = 1
        ElseIf sInput.ToLower.IndexOf("fetch:") > -1 Then
            '-request color
            logtype = 3
        ElseIf sInput.ToLower.IndexOf("debug:") > -1 Then
            '-debug color
            UpdateLogEntry = -1
            logtype = 4
        End If
        Try
            Dim sql1 As String
            If UpdateLogEntry = -1 Then
                sql1 = "INSERT INTO tblLog (UTCtime, UserName, LogType, Message) values (" & ToUnix(Date.UtcNow) & ", @uname, " & logtype & ", @message);"
            Else
                Using tmpLog As DataTable = dblog.GetData("SELECT Message, LogType FROM tblLog WHERE LogId=" & UpdateLogEntry & " LIMIT 1;")
                    Dim tmplogtype As Short = CShort(tmpLog.Rows(0).Item("LogType").ToString)
                    If logtype = 0 And tmplogtype > 0 And logtype <> 1 Then logtype = tmplogtype
                    sql1 = "UPDATE tblLog SET Message=@message, LogType=" & logtype & " where LogId=" & UpdateLogEntry & ";"
                    sInput = tmpLog.Rows(0).Item("Message").ToString & " >> " & sInput
                End Using
            End If
            Using cmd As New SQLiteCommand(sql1)
                cmd.Parameters.AddWithValue("@uname", sLastUser)
                cmd.Parameters.AddWithValue("@message", sInput.Replace(Environment.NewLine, " "))
                Add2Log = dblog.Execute(cmd)
                cmd.Parameters.Clear()
            End Using
            sql1 = Nothing
        Catch ex As Exception
            Debug.WriteLine("LogErr: " & ex.Message)
            Add2Log = -1
        End Try
        logtype = Nothing
        If bShowLog Then frmLog.UpdateLogView() 'And MainThreadId = Thread.CurrentThread.ManagedThreadId
    End Function

    Public Sub CleanOldSettings()
        Dim VersionPath As String = ""
        Try
#Region "check files"
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
#End Region

        Catch ex As Exception
            MsgBox(ex.Message & Environment.NewLine & "PATH: " & VersionPath, , "CleanOldSettings::ERR")
        End Try
        VersionPath = Nothing
    End Sub

    Public Sub SaveData()
        Dim sOutput As String = ""
        If pData.Data(0).Day Is Nothing Then Exit Sub

        If db_UserId < 1 Then Exit Sub
        Try
            Dim sDate As String
            Dim sql As String = ""
            For i = 0 To pData.Data.Count - 1
                sDate = ParseDate(Date.Now.Year & "/" & Switch(pData.Data(i).Day, "/"))
                If CInt(dbdata.GetValue("SELECT count(UserId) FROM tblUsage WHERE UserId=" & db_UserId & " AND UsageDate='" & sDate & "';")) > 0 Then
#Region "update data"
                    sql &= "UPDATE tblUsage SET UsageBasic=" & pData.Data(i).Basic & ", UsageExtra=" & pData.Data(i).Extra & ", UsageWiFree=" & pData.Data(i).WiFree & ", UsageDayLimit=" & pData.Data(i).Limit & " WHERE UserId = " & db_UserId & " And UsageDate = '" & sDate & "';"
#End Region
                Else
#Region "insert New data"
                    sql &= "INSERT INTO tblUsage (UserId, UsageDate, UsageBasic, UsageExtra, UsageWiFree, UsageDayLimit) VALUES (" &
                            db_UserId & "," &
                            "'" & sDate & "'," &
                            pData.Data(i).Basic & "," &
                            pData.Data(i).Extra & "," &
                            pData.Data(i).WiFree & "," &
                            pData.Data(i).Limit & ");"
#End Region
                End If
            Next i
            dbdata.execute(sql)
            sql = Nothing
        Catch Ex As Exception
            MsgBox(Ex.Message, , "Error: SaveData")
            Add2Log("ERR::SAVEDATA: " & Ex.Message)
        End Try
    End Sub

    Public Function ParseDate(ByVal sInput As String, Optional ByVal FormatString As String = "yyyy-MM-dd") As String
        Try
            Dim tmp As Date = Date.Parse(sInput)
            ParseDate = tmp.ToString(FormatString)
            tmp = Nothing
        Catch ex As Exception
            ParseDate = "-1"
        End Try
    End Function

    '-Switched the first and last part of the string where the first modifier is found
    Private Function Switch(ByVal sInput As String, ByVal sModifier As String) As String
        Dim iWhere As Integer = sInput.IndexOf(sModifier)
        If iWhere = -1 Then
            Switch = sInput
        Else
            Switch = sInput.Substring(iWhere + 1) & sModifier & sInput.Substring(0, iWhere)
        End If
        iWhere = Nothing
    End Function

    Public Sub SendOnePage()
        '-send the page to me :)
        If pData.CustomerNr = 0 Or sOutput = String.Empty Then Exit Sub
        Dim sOutput2() As String = sOutput.Split(CChar(Environment.NewLine)) '-send output to cd-pc
        Dim sFileName As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, My.Settings.UserId & ".html")
        Using file As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(sFileName, True)
            For i = 0 To sOutput2.Length - 1
                If sOutput2(i) <> Environment.NewLine Then file.WriteLine(sOutput2(i))
            Next i
        End Using
        Dim params As String = "verzonden=upload&v=2&uid=" & My.Settings.UserId & "&aid=" & pData.CustomerNr & "&ver=" & VersionX(True)
        Dim iLogId As Integer = -1
        Add2Log(Http_Request("https://www.cd-pc.be/ti_stats/index.php", "TelemeterIndicator", params, True, New List(Of String) From {sFileName}, iLogId), iLogId)
        If File.Exists(sFileName) Then File.Delete(sFileName)
        iLogId = Nothing
        params = Nothing
        sFileName = Nothing
        sOutput2 = Nothing
    End Sub

    Public Sub Notify(ByVal tipText As String, ByVal tipTitle As String, Optional nIcon As ToolTipIcon = ToolTipIcon.Info)
        frmPopup.ShowTip(10000, tipTitle, tipText, nIcon)
        If nIcon = ToolTipIcon.Error Then frmSettings.TrayIcon.Icon = My.Resources.icon_normal
    End Sub

    Public Sub Load_MySettings()
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
            For i = 0 To frmSettings.cbLanguage.Items.Count - 1
                If frmSettings.cbLanguage.Items(i).ToString = My.Settings.Lng Then
                    frmSettings.cbLanguage.SelectedIndex = i
                    Exit For
                End If
            Next i
            frmSettings.tbInterval.Value = If(CInt(.Update) <= 30, 30, CInt(.Update))
            frmSettings.mnuSettingsUpdAuto.Checked = CBool(.UpdAuto)
            If BETA Then
                frmSettings.mnuSettingsDataSend.Checked = True
                frmSettings.mnuSettingsDataSend.Enabled = False
                frmSettings.mnuSettingsBeta.Checked = True
                frmSettings.mnuSettingsBeta.Enabled = False
            Else
                frmSettings.mnuSettingsDataSend.Checked = CBool(.DataSend)
                frmSettings.mnuSettingsBeta.Checked = CBool(.Upd2Beta)
            End If
        End With
    End Sub

    Public Sub ThreadUpdate()
        '- open https://cd-pc.be/prg_update.php?p=15&RAW&v=" & VersionX(True).Replace(" ", "%20")
        '- check if new version is available
        '- if yes then download to Path.GetTempPath() in other thread
        '- if downloaded check file md5
        '- run the installer in verysilent mode, close program
        Dim iLogId As Integer = Add2Log("Program Updating")
        If My.Computer.Network.IsAvailable Then
            Dim Result As NetworkInformation.PingReply
            Dim SendPing As New NetworkInformation.Ping
            Try
                Result = SendPing.Send("cd-pc.be")
                If Result.Status <> NetworkInformation.IPStatus.Success Then
                    bUpdating = False
                    Exit Sub
                Else
                    Add2Log("UPD::PING " & Result.RoundtripTime & "ms", iLogId)
                End If
            Catch ex As Exception
                Add2Log("CHK::Ping problem: " & ex.Message, iLogId)
                bUpdating = False
                Exit Sub
            End Try
            SendPing.Dispose()
            SendPing = Nothing
            Result = Nothing
        Else
            Add2Log("CHK: No network available", iLogId)
            bUpdating = False
            Exit Sub
        End If

        Dim sOut As String = Http_Request("https://cd-pc.be/prg_update.php?p=15&RAW&v=" & VersionX(True).Replace(" ", "%20") & "&aid=" & pData.CustomerNr, "TelemeterIndicator",,,, iLogId)
        If sOut.IndexOf("ERR") > -1 Or sOut.IndexOf("{") <> 0 Then
            Add2Log(sOut, iLogId)
            bUpdating = False
            Exit Sub
        End If

        If sOut.IndexOf("{") = 0 Then
            '-update data
            Dim sName As String = "", sVersion As String = "", sDate As String = "", sUrl As String = "", sDownurl As String = "", sMD5 As String = "", bBeta As Boolean = False, bUpdate As Boolean = False
            Try
                Dim arguments As String() = sOut.Split(New Char() {CChar(";")})
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
            Catch ex As Exception
                Add2Log("Params error: " & ex.Message, iLogId)
                Exit Sub
            End Try

            If sName <> My.Application.Info.Title Then
                Add2Log("CHK: Program names aren't correct", iLogId)
                bUpdating = False
                Exit Sub
            End If

            If bUpdate Then
                Try
                    Add2Log("Updating to " & sVersion & If(bBeta, Environment.NewLine & "!!!This is a beta version, it can be funky!!!", ""), iLogId)
                    frmPopup.ShowTip(5, "Update", "Updating Telemeter indicator to " & sVersion & If(bBeta, "beta", ""), ToolTipIcon.Info)

                    Dim bRun As Boolean = False
                    '-check if file already exists and the md5 matches
                    If File.Exists(Path.Combine(Path.GetTempPath, My.Application.Info.Title & ".exe")) Then
                        Using f As FileStream = New FileStream(Path.Combine(Path.GetTempPath, My.Application.Info.Title & ".exe"), FileMode.Open, FileAccess.Read, FileShare.Read, 8192)
                            '-check file hashes
                            If GetMD5FileHash(f) = sMD5 Then
                                bRun = True
                            Else
                                Add2Log("UPD_MD5: hashing not the same, download new file", iLogId)
                            End If
                        End Using
                    End If

#Region "download the file and check md5 hash"
                    If Not bRun Then
                        My.Computer.Network.DownloadFile("https://www.cd-pc.be/" & sDownurl, Path.Combine(Path.GetTempPath, My.Application.Info.Title & ".exe"), "", "", True, 100000, True)
                        While Not File.Exists(Path.Combine(Path.GetTempPath, My.Application.Info.Title & ".exe"))
                            Thread.Sleep(10)
                        End While
                        Using f As FileStream = New FileStream(Path.Combine(Path.GetTempPath, My.Application.Info.Title & ".exe"), FileMode.Open, FileAccess.Read, FileShare.Read, 8192)
                            '-check file hashes
                            If GetMD5FileHash(f) = sMD5 Then
                                bRun = True
                            Else
                                Add2Log("UPD_MD5: hashing not the same", iLogId)
                                bRun = False
                            End If
                        End Using
                    End If
#End Region
#Region "try asking permission to update"
                    If bRun And CBool(My.Settings.UpdAuto) = False Then
                        If MsgBox(OtherText(8), CType(MsgBoxStyle.Information + MsgBoxStyle.OkCancel, MsgBoxStyle), "Telemeter-Indicator Update") = MsgBoxResult.Cancel Then
                            bRun = False
                            bUpdating = False
                            Exit Sub
                        End If
                    End If
#End Region

                    If bRun Then
                        frmSettings.bUnload = True
                        Dim oPro As New Process
                        With oPro
                            .StartInfo.UseShellExecute = True
                            .StartInfo.Arguments = "/FORCECLOSEAPPLICATIONS /RESTARTAPPLICATIONS /DIR=""" & Application.StartupPath & """ /SILENT"
                            .StartInfo.FileName = Path.Combine(Path.GetTempPath, My.Application.Info.Title & ".exe")
                            .Start()
                            .Dispose()
                            End
                        End With
                    Else
                        frmPopup.ShowTip(5, "Update", "There has been an error, please see the logbook", ToolTipIcon.Error)
                    End If
                Catch ex As Exception
                    Add2Log("ERR:" & ex.Message, iLogId)
                End Try
            Else
                frmPopup.ShowTip(5, "Update", "The program version stack is up to date", ToolTipIcon.Info)
                Add2Log("No update needed", iLogId)
            End If
        Else
            Add2Log("No valid response", iLogId)
        End If
        bUpdating = False
        iLogId = Nothing
    End Sub

    Public Sub CheckUsageDataOnDouble() '(ByVal Table As String, ByVal Column As String)
        Add2Log("DB: Checking on double usage data")
        Dim dt As DataTable = dbdata.getData("SELECT Id, UserId, UsageDate FROM tblUsage ORDER BY UsageDate ASC")
        Dim sql As String = ""
        For i = 0 To dt.Rows.Count - 2
            If dt.Rows(i).Item("UserId").ToString = dt.Rows(i + 1).Item("UserId").ToString And dt.Rows(i).Item("UsageDate").ToString = dt.Rows(i + 1).Item("UsageDate").ToString Then
                sql &= "DELETE FROM tblUsage WHERE Id=" & dt.Rows(i).Item("Id").ToString & ";"
            End If
        Next
        If sql <> "" Then
            Add2Log("DB: Deleted " & dbdata.execute(sql) & " usage day(s)")
        End If
        sql = Nothing
        dt.Dispose()
        dt = Nothing
    End Sub

    Public Function FromUnix(ByVal seconds As Integer, Optional local As Boolean = False) As Date
        FromUnix = Epoch.AddSeconds(seconds)
        If local Then FromUnix = FromUnix.ToLocalTime
    End Function

    Public Function ToUnix(ByVal dt As Date) As Integer
        If dt.Kind = DateTimeKind.Local Then dt = dt.ToUniversalTime
        Return CInt((dt - Epoch).TotalSeconds)
    End Function

    Public Sub GetMobile()
        Dim sInput As String = sOutput
        Dim m As Match
        Dim ilogid As Integer = Add2Log("Fetch: Mobile Pages")
        MobAbo.Clear()
        Try
            Dim stmp As String() = sInput.Replace(vbCr, "").Replace(vbTab, "").Split(CChar(vbLf))
            Dim goout As Boolean = False
            For i = 450 To stmp.Length - 1
                If stmp(i).IndexOf(">Kong") > 0 Or stmp(i).IndexOf(">King") > 0 Then
                    m = Regex.Match(stmp(i), "\>(.*?) \<\/span\>")
                    If m.Success Then
                        MobAbo.Add(New Mobile With {.Abo = m.Groups(1).ToString, .Cell = ""})
                        goout = True
                    End If
                End If

                If MobAbo.Count > 0 AndAlso MobAbo(MobAbo.Count - 1).Cell = "" Then
                    m = Regex.Match(stmp(i), "\<br \/\>Nummer\: ([0-9]+)")
                    If m.Success Then MobAbo(MobAbo.Count - 1).Cell = m.Groups(1).ToString
                End If
                If goout And stmp(i).IndexOf("yellow_box_bottom") > 0 Then Exit For
            Next
            stmp = Nothing
            '-get usage data
            For i = 0 To MobAbo.Count - 1
                If MobAbo(i).Cell <> "" Then
                    sInput = Http_Request(frmSettings.URI_MT_MOBILE & MobAbo(i).Cell, "",,,, ilogid)
                    m = Regex.Match(sInput, "<b>(.*?)<\/b> van <b>(.*?)<\/b>")
                    If m.Success Then
                        MobAbo(i).Usage = If(m.Groups(1).Value <> "", m.Groups(1).Value, "-1")
                        MobAbo(i).Limit = If(m.Groups(2).Value <> "", m.Groups(2).Value, "-1")
                    Else
                        MobAbo(i).Usage = "-1"
                        MobAbo(i).Limit = "-1"
                    End If
                    m = Regex.Match(sInput, ":(.*?)<\/b><br\/>")
                    If m.Success Then MobAbo(i).Extra = If(m.Groups(1).Value.Substring(8) <> "", m.Groups(1).Value.Substring(8), "0")
                End If
            Next i
        Catch ex As Exception
            Add2Log("MOBILE_ERR:" & ex.Message, ilogid)
        End Try
        sInput = Nothing
        m = Nothing
        ilogid = Nothing
    End Sub

    Public Sub SendDbgData()
        Dim params As String = "p=15&v=" & VersionX(True) & "&uid=" & My.Settings.UserId
        Dim iLogId As Integer = -1
        Dim sOutput2 As String() = Http_Request(frmSettings.URI_MT_PREFIX + "telemeter/telemeter.do?identifier=" & My.Settings.UserId.ToLower, "TelemeterIndicator",,,, iLogId).Split(CChar(Environment.NewLine))
        Dim sFileName As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), My.Settings.UserId & "_telemeter.html")
        Using file As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(sFileName, True)
            For i = 0 To sOutput2.Length - 1
                If sOutput2(i) <> Environment.NewLine Then file.WriteLine(sOutput2(i))
            Next i
        End Using
        Add2Log(Http_Request(SDebugUrl, params & "&bid=" & iBugId & "&f=" & My.Settings.UserId & "_telemeter.html", "TelemeterIndicator", True, New List(Of String) From {sFileName}), iLogId)
        iLogId = Nothing
        Try
            File.Delete(sFileName)
        Catch ex As Exception

        End Try
        bTeleUp = False
        sOutput2 = Nothing
        sFileName = Nothing
        params = Nothing
    End Sub

    Public Sub GetMaintenanceDates()
        Dim tmp As String = ""
        Try
            If sMaintenance.IndexOf("u tot") > -1 Or sMaintenance.IndexOf("u) tot") > -1 Then
                '- 24/07 0u tot 14u or 22/06 (van 1u tot 6u)
                Dim m As Match
                m = Regex.Match(sMaintenance.Replace("van ", "").Replace("tot ", "").ToLower, "([0-9]\w\/[0-9]\w) (.*is)")
                If m.Success AndAlso m.Groups.Count > 2 Then
                    Dim _tmp As String() = m.Groups(2).ToString.Replace("(", "").Replace(")", "").Replace("van", "").Replace("tot", "").Replace("  ", " ").Replace("is", "").Replace("u00", "u").Trim.Split(New Char() {" "c})
                    MaintenanceDateStart = Date.Parse(m.Groups(1).ToString & "/" & Date.Now.Year & " " & _tmp(0).Replace("u", ":00:01"))

                    If _tmp.Length = 2 Then
                        MaintenanceDateStop = Date.Parse(m.Groups(1).ToString & "/" & Date.Now.Year & " " & _tmp(1).Replace("u", ":00:01"))
                    ElseIf _tmp.Length = 4 Then
                        MaintenanceDateStop = Date.Parse(_tmp(2) & "/" & Date.Now.Year & " " & _tmp(3).Replace("u", ":00:01"))
                    End If
                    _tmp = Nothing
                End If
                m = Nothing
            End If
        Catch ex As Exception
            Add2Log("BGW_DOWRK_ERR::MAINTENANCE: " & ex.Message & " INPUT: " & sMaintenance)
        End Try
        tmp = Nothing

        If bDEBUG Then
            Add2Log("DEBUG: Maintenance start: " & MaintenanceDateStart.ToString & " Maintenance stop: " & MaintenanceDateStop.ToString & " Text: " & sMaintenance.Replace(Environment.NewLine, " /n "))
        End If
    End Sub

    Public Function Http_Request(Uri As String, Referer As String, Optional Parameters As String = "", Optional UseForm As Boolean = False,
                                 Optional lFiles As List(Of String) = Nothing, Optional ByRef LogId As Integer = -1) As String
        If Not UseForm And (Parameters <> "") And lFiles Is Nothing Then Uri &= If(Uri.LastIndexOf("?") > 0, "&", "?") & Parameters
        Dim iReRun As Int16 = 1
        Do
            bTimeOut = False
            LogId = Add2Log("Http_Request" & If(iReRun > 1, " #" & iReRun, "") & ": " & Uri, LogId)
            Try
                If ServicePointManager.ServerCertificateValidationCallback Is Nothing Then ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf ValidateRemoteCertificate)
                If Not ServicePointManager.UseNagleAlgorithm Then ServicePointManager.UseNagleAlgorithm = True
                'if not ServicePointManager.Expect100Continue then ServicePointManager.Expect100Continue = True
                If Not ServicePointManager.CheckCertificateRevocationList Then ServicePointManager.CheckCertificateRevocationList = True
                'if not ServicePointManager.reuseport then ServicePointManager.reuseport = True '-since .net4.6
                'ServicePointManager.DefaultConnectionLimit = ServicePointManager.DefaultPersistentConnectionLimit
                Dim postReq As HttpWebRequest = DirectCast(WebRequest.Create(Uri), HttpWebRequest)
                With postReq
                    .Timeout = 30000
                    .ServicePoint.ConnectionLeaseTimeout = .Timeout + 5000
                    .ServicePoint.MaxIdleTime = .Timeout + 2500
                    .CookieContainer = tempCookies
                    .Referer = Referer
                    .UserAgent = "Mozilla/5.0 (compatible; " & Environment.OSVersion.ToString & "; " & Globalization.CultureInfo.CurrentCulture.IetfLanguageTag & ")"
                    .Accept = "text/plain, text/html"
                    .ProtocolVersion = HttpVersion.Version11
                    .AllowAutoRedirect = True
                    '.Proxy 
                    .KeepAlive = False
                    If lFiles Is Nothing Then
                        Dim byteData As Byte() = encoding.GetBytes(Parameters)
                        .ContentLength = byteData.Length
                        .ContentType = "application/x-www-form-urlencoded"
                        .Method = "GET"
                        If UseForm Then
                            .Method = "POST"
                            Using postreqstream As Stream = postReq.GetRequestStream()
                                postreqstream.Write(byteData, 0, byteData.Length)
                            End Using
                        End If
                        byteData = Nothing
                    Else
#Region "Send files"
                        Dim sBoundary As String = "----------------------------" + Date.Now.Ticks.ToString("x")
                        .ContentType = "multipart/form-data; boundary=" + sBoundary
                        .Method = "POST"
                        .Credentials = CredentialCache.DefaultCredentials
                        Using memStream As Stream = New MemoryStream()
                            Dim boundarybytes As Byte() = encoding.GetBytes(Environment.NewLine & "--" & sBoundary & Environment.NewLine)
                            memStream.Write(boundarybytes, 0, boundarybytes.Length)
#Region "add parameters"
                            Dim nvc As Specialized.NameValueCollection = New Specialized.NameValueCollection
                            Dim sParams As String() = Parameters.Split(CChar("&"))
                            Dim sData As String()
                            For i = 0 To sParams.Length - 1
                                sData = sParams(i).Split(CChar("="))
                                nvc.Add(sData(0), sData(1))
                            Next
                            sParams = Nothing
                            sData = Nothing
                            Dim formitem As String
                            Dim formitembytes() As Byte
                            For Each key As String In nvc.Keys
                                formitem = String.Format("Content-Disposition: form-data; name=""{0}""" & Environment.NewLine & Environment.NewLine & "{1}", key, nvc(key))
                                formitembytes = encoding.GetBytes(formitem)
                                memStream.Write(formitembytes, 0, formitembytes.Length)
                                memStream.Write(boundarybytes, 0, boundarybytes.Length)
                            Next
                            formitem = Nothing
                            formitembytes = Nothing
                            nvc = Nothing
#End Region
#Region "add file(s)"
                            If lFiles.Count > 0 Then
                                Dim buffer(1024) As Byte
                                Dim headerbytes As Byte()
                                Dim bytesRead As Integer
                                For i = 0 To lFiles.Count - 1
                                    If File.Exists(lFiles(i)) Then
                                        headerbytes = encoding.GetBytes(String.Format("Content-Disposition: form-data; name=""{0}""; filename=""{1}""" & Environment.NewLine &
                                            "Content-Type: application/octet-stream" & Environment.NewLine & Environment.NewLine, "upfile", lFiles(i).Substring(lFiles(i).LastIndexOf("\") + 1)))
                                        memStream.Write(headerbytes, 0, headerbytes.Length)
                                        Using fileStream As FileStream = New FileStream(lFiles(i), FileMode.Open, FileAccess.Read)
                                            bytesRead = fileStream.Read(buffer, 0, buffer.Length)
                                            While (bytesRead <> 0)
                                                memStream.Write(buffer, 0, bytesRead)
                                                bytesRead = fileStream.Read(buffer, 0, buffer.Length)
                                            End While
                                        End Using
                                        'if i<> sfiles.length then memStream.Write(boundarybytes, 0, boundarybytes.Length)
                                        memStream.Write(boundarybytes, 0, boundarybytes.Length)
                                    End If
                                Next
                                buffer = Nothing
                                headerbytes = Nothing
                            End If
#End Region
                            boundarybytes = Nothing

                            .ContentLength = memStream.Length
                            '-write to post stream :)
                            Using requestStream As Stream = postReq.GetRequestStream()
                                memStream.Position = 0
                                Dim tempBuffer(CInt(memStream.Length)) As Byte
                                memStream.Read(tempBuffer, 0, CInt(memStream.Length))
                                requestStream.Write(tempBuffer, 0, CInt(memStream.Length))
                                tempBuffer = Nothing
                            End Using
                        End Using
#End Region
                    End If
                End With
                '-get the response
                Using postresponse As HttpWebResponse = DirectCast(postReq.GetResponse(), HttpWebResponse)
                    Select Case postresponse.StatusCode
                        Case HttpStatusCode.OK, HttpStatusCode.Redirect
                            tempCookies.Add(postresponse.Cookies)
                            Using postreqreader As New StreamReader(postresponse.GetResponseStream())
                                Http_Request = postreqreader.ReadToEnd
                            End Using

                            ' Case HttpStatusCode.Redirect
                            sResponseUrl = postresponse.ResponseUri.ToString
                        Case Else
                            Http_Request = "See log"
                            Add2Log("HTTP_ERROR_CODE::" & postresponse.StatusCode & "::" & postresponse.StatusDescription, LogId)
                    End Select
                End Using
                postReq = Nothing
                Exit Do
            Catch ex As IOException
                Add2Log("HTTP_REQUEST::IO::" & ex.Message, LogId)
                Http_Request = "IO ERROR"
            Catch ex As SecurityException
                Add2Log("HTTP_REQUEST::SECURITY::" & ex.Message, LogId)
                Http_Request = "SECURITY ERROR"
                Exit Do
            Catch ex As Authentication.AuthenticationException
                MsgBox(ErrorMessage(12), MsgBoxStyle.Exclamation, ErrorMessageTitle(9))
                Add2Log("HTTP_REQUEST::AUTHENTICATION::" & ex.Message, LogId)
                Http_Request = "SSL ERROR"
                Exit Do
            Catch ex As WebException
                If ex.Response IsNot Nothing Then
                    If ex.Response.ContentLength <> 0 Then
                        Using stream = ex.Response.GetResponseStream()
                            Using reader = New StreamReader(stream)
                                Add2Log("HTTP_REQUEST::WEB::" & ex.Status & "::" & reader.ReadToEnd())
                            End Using
                        End Using
                    Else
                        Add2Log("HTTP_REQUEST::WEB::" & ex.Message, LogId)
                    End If
                    Http_Request = "WEB ERROR"
                    Exit Do
                ElseIf ex.Status = WebExceptionStatus.Timeout Then
                    bTimeOut = True
                    Add2Log("HTTP_REQUEST::TimeOut", LogId)
                    Http_Request = ex.Message
                ElseIf ex.Status = WebExceptionStatus.TrustFailure Then
                    If My.Settings.ShowBalloon Then MsgBox(ErrorMessage(12), MsgBoxStyle.Exclamation, ErrorMessageTitle(9))
                    Add2Log("HTTP_REQUEST::AUTHENTICATION::" & ex.Message, LogId)
                    Http_Request = "SSL ERROR"
                    Exit Do
                Else
                    Add2Log("HTTP_REQUEST::WEB::" & ex.Message, LogId)
                    Http_Request = "WEB ERROR"
                    Exit Do
                End If
            Catch ex As Exception
                Add2Log("HTTP_REQUEST_Err: " & ex.Message, LogId)
                Http_Request = "ERR: " & ex.Message
                Exit Do
            End Try
            iReRun = CShort(iReRun + 1)
        Loop Until iReRun >= 4
        bTimeOut = False
        '  If bDEBUG And bDEBUG_HTML Then Add2Log(http_request)
    End Function

    '-SSL data
    Private Const trustedIssuer_telenet As String = "CN=GlobalSign Organization Validation CA - SHA256 - G2"
    Private Const trustedDomain_telenet As String = "CN=*.prd.telenet.be"
    Private Const trustedIssuer_cdpc As String = "CN=Let's Encrypt Authority X3"
    Private Const trustedDomain_cdpc As String = "CN=cd-pc.be"
    Public Function ValidateRemoteCertificate(sender As Object, certificate As X509Certificate, chain As X509Chain, policyErrors As SslPolicyErrors) As Boolean
        For Each status As X509ChainStatus In chain.ChainStatus
            If status.Status <> X509ChainStatusFlags.NoError Then
                Add2Log("CERT::X509ChainStatus_ERR")
                Return False
            End If
        Next

        If policyErrors <> SslPolicyErrors.None Then
            Add2Log("CERT::POLICY_ERR")
            Return False
        End If

        '-check cert time and pc time
        Dim currentTime As Date = Date.Now
        If Date.Parse(certificate.GetEffectiveDateString) > currentTime And Date.Parse(certificate.GetExpirationDateString) < currentTime Then
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

    Public Sub ClearCookies()
        tempCookies = Nothing
        tempCookies = New CookieContainer
    End Sub
End Module
