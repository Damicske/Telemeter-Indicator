Imports System.Data.SQLite
Imports System.Data

Public Class frmImport

    Private Function DateStrToStr(ByVal sInput As String) As String
        DateStrToStr = sInput.Substring(0, 2) & "/" & sInput.Substring(2, 2) & "/" & sInput.Substring(4)
    End Function

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        btnStart.Enabled = False
        chkMoveBin.Enabled = False
        TabControl1.Enabled = False

        Dim dir As DirectoryInfo = New DirectoryInfo(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "data\"))
        Dim files As FileInfo() = dir.GetFiles("*.txt")
        Dim sql1 As String = "", last_user As String = "", sql_tmp As String = ""
        Dim tmp_uid As Integer = -1, stats_sql As Integer = 0, stats_filedel As Integer = 0, stats_periods As Integer = 0

        pbFiles.Maximum = files.Length
        pbFiles.Value = 0

        For Each fl As FileInfo In files
            Dim bParsed As Boolean = False
            Using objReader As New FileIO.TextFieldParser(fl.FullName)
                objReader.TextFieldType = FileIO.FieldType.Delimited
                objReader.SetDelimiters(";")

                Dim sStream As String(), bNewyear As Boolean = False
                Do
                    sStream = objReader.ReadFields
                    If sStream.Length = 8 And sStream(0).Length >= 7 Then
#Region "Set tmp_uid"
                        If last_user <> sStream(0) Then
                            Dim cmd As New SQLiteCommand("Select UserId from tblUser where UserName=@uname limit 1;")
                            cmd.Parameters.AddWithValue("@uname", sStream(0))
                            tmp_uid = CInt(dbdata.getValue(cmd))
                            If tmp_uid < 1 Then
                                Dim dic As New Dictionary(Of String, Object) From {{"UserName", sStream(0)}}
                                tmp_uid = dbdata.insert("tblUser", dic)
                                dic = Nothing
                                If tmp_uid < 1 Then
                                    Add2Log("IMPORT::ERR::DB: New user DB problem")
                                    Exit Do
                                End If
                            End If
                            cmd.Parameters.Clear()
                            cmd.Dispose()
                            cmd = Nothing
                            last_user = sStream(0)
                        End If
#End Region
                        If tmp_uid > 0 And CLng(sStream(7)) > 0 Then
#Region "Check period in db"
                            Dim sstart As String = ParseDate(DateStrToStr(sStream(1)))
                            Dim sstop As String = ParseDate(DateStrToStr(sStream(2)))
                            If CInt(dbdata.GetValue("SELECT count(UserId) FROM tblPeriod WHERE UserId=" & tmp_uid & " AND StartDate='" & sstart & "' AND EndDate='" & sstop & "';")) < 1 Then
                                dbdata.execute("INSERT INTO tblPeriod (UserId,StartDate,EndDate)VALUES(" & tmp_uid & ",'" & sstart & "','" & sstop & "');")
                                stats_periods += 1
                            End If
                            sstart = Nothing
                            sstop = Nothing
#End Region
                            If sStream(3) = "01/01" Then bNewyear = True
                            Dim YearSwitch As Integer = CInt(If(bNewyear, sStream(2).Substring(4), sStream(1).Substring(4)))

                            sql_tmp &= "INSERT OR IGNORE INTO tblUsage (UserId, UsageDate, UsageBasic, UsageExtra, UsageWiFree, UsageDayLimit) VALUES (" &
                                    tmp_uid & ",'" &
                                    ParseDate(sStream(3) & "/" & YearSwitch) & "'," &
                                    CLng(sStream(4)) & "," &
                                    CLng(sStream(5)) & "," &
                                    CLng(sStream(6)) & "," &
                                    CLng(sStream(7)) & ");"
                            bParsed = True
                        Else
                            Add2Log("IMPORT_FILE::ERR:" & fl.Name & " - UID or LIMIT is wrong")
                            Exit Do
                        End If
                    Else
                        Add2Log("IMPORT_FILE::ERR:" & fl.Name & " - UID.length or ROW.length is wrong")
                        Exit Do
                    End If
                Loop Until objReader.EndOfData
            End Using
#Region "Move file to recyclebin"
            If bParsed Then
                sql1 &= sql_tmp
                If chkMoveBin.Checked Then
                    My.Computer.FileSystem.DeleteFile(fl.FullName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                    stats_filedel += 1
                End If
            End If
#End Region
            pbFiles.Value += 1
            sql_tmp = ""
        Next
        If sql1 <> "" Then
            pbFiles.Value = 100
            pbFiles.Maximum = 100
            pbFiles.Style = ProgressBarStyle.Marquee
            pbFiles.Visible = True
            stats_sql = dbdata.Execute(sql1)
            pbFiles.Value = 0
            pbFiles.Style = ProgressBarStyle.Continuous
            pbFiles.Visible = True
        End If

        MsgBox("File's checked: " & pbFiles.Maximum & Environment.NewLine &
            "File's moved to recyle bin: " & stats_filedel & Environment.NewLine &
            "Added old periods: " & stats_periods & Environment.NewLine &
            "Added old usage data: " & stats_sql & " days",
             MsgBoxStyle.Information, "Imported old data files: stats")
        btnStart.Enabled = True
        chkMoveBin.Enabled = True
        TabControl1.Enabled = True

        '-cleaning up
        dir = Nothing
        files = Nothing
        sql1 = Nothing
        last_user = Nothing
        sql_tmp = Nothing
        tmp_uid = Nothing
        stats_sql = Nothing
        stats_filedel = Nothing
        stats_periods = Nothing
    End Sub

    Private Sub btnImportDb_Click(sender As Object, e As EventArgs) Handles btnImportDb.Click
        With OpenFileDialog1
            .Multiselect = False
            .Title = "Select database for import"
            .Filter = "Database|*.db"
            .FileName = "ti_data.db"
            .CheckFileExists = True

            If .ShowDialog() = DialogResult.OK Then
                chkMoveBin.Enabled = False
                TabControl1.Enabled = False
                Dim dbimport As Sqlite = New Sqlite("Data Source=" & .FileName)
                Dim iPeriods As Integer = 0, iUsage As Integer = 0, iUser As Integer = 0
                Try
                    'pseudo:
                    'check tables
                    'select * from tblUser check if UserId <> UserId current tblUser then create new
                    'select * from tblUsage where UserId= > insert into current tblUsage with correct UserId
                    'select * from tblPeriod where UserId= > insert into current tblPeriod with correct UserId
                    pbFiles.Value = 0
                    pbFiles.Style = ProgressBarStyle.Continuous
                    Using users As DataTable = dbimport.GetData("SELECT * FROM tblUser")
                        Dim uid As Integer
                        pbFiles.Maximum = users.Rows.Count
                        For i = 0 To users.Rows.Count - 1
                            '-check uid
                            uid = CInt(dbdata.GetValue("SELECT UserId FROM tblUser WHERE UserName='" & users.Rows(i).Item("UserName").ToString & "'"))
                            If uid < 1 Then
                                uid = dbdata.Insert("tblUser", New Dictionary(Of String, Object) From {{"UserName", users.Rows(i).Item("UserName").ToString}})
                                If uid > 0 Then iUser += 1
                            End If
                            If uid > 0 Then
                                pbFiles.PerformStep()
                                iUsage += ImportUsage(dbimport, CInt(users.Rows(i).Item("UserId")), uid)
                                iPeriods += ImportPeriods(dbimport, CInt(users.Rows(i).Item("UserId")), uid)
                            Else
                                Add2Log("IMPORT_DB::UID_ERR: < 1")
                            End If
                        Next i
                    End Using
                    dbimport = Nothing
                Catch ex As Exception
                    Add2Log("IMPORT_DB::ERR: " & ex.Message)
                    MsgBox("There has been an error, check the logbook for more info")
                End Try

                MsgBox("Added old users: " & iUser & Environment.NewLine &
            "Added old periods: " & iPeriods & Environment.NewLine &
            "Added old usage data: " & iUsage & " days",
             MsgBoxStyle.Information, "Imported 'old' database: stats")

                chkMoveBin.Enabled = True
                TabControl1.Enabled = True
            End If
        End With
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        With OpenFileDialog1
            .Multiselect = False
            .Title = "Select file for exporting"
            .Filter = "Comma-separated value file|*.csv"
            .FileName = "ti_data_" & Date.UtcNow.ToString & ".csv"

            If .ShowDialog() = DialogResult.OK Then
                chkMoveBin.Enabled = False
                TabControl1.Enabled = False
                If File.Exists(.FileName) Then
                    If MsgBox("Overwrite file?", CType(MsgBoxStyle.YesNo + MsgBoxStyle.Question, MsgBoxStyle), "Export file") = MsgBoxResult.No Then Exit Sub
                End If
                Try
                    Dim dbexport As Sqlite = New Sqlite("Data Source=" & .FileName)
                    Using dt As DataTable = dbdata.GetData("SELECT d.*, u.username FROM tblUsage as d, tblUser as u WHERE u.UserId=" & db_UserId & " and d.UserId=u.UserId")
                        If dt.Rows.Count > 0 Then
                            pbFiles.Maximum = dt.Rows.Count
                            pbFiles.Value = 0
                            Using fs As New StreamWriter(.FileName, False)
                                fs.WriteLine("TELENET_ID;DATE;BASIC;EXTRA;WI-FREE;LIMIT") 'header
                                For i = 0 To dt.Rows.Count - 1
                                    fs.WriteLine(dt.Rows(i).Item("UserName").ToString & ";" &
                                             dt.Rows(i).Item("UsageDate").ToString & ";" &
                                             dt.Rows(i).Item("UsageBasic").ToString & ";" &
                                             dt.Rows(i).Item("UsageExtra").ToString & ";" &
                                             dt.Rows(i).Item("UsageWiFree").ToString & ";" &
                                             dt.Rows(i).Item("UsageDayLimit").ToString
                                    )
                                    pbFiles.Value = i
                                Next
                            End Using
                        Else
                            MsgBox("No usage data found")
                        End If
                    End Using
                    dbexport = Nothing
                Catch ex As Exception
                    MsgBox("There has been an error, check the logbook for more info")
                    Add2Log("EXPORT_DB::ERR: " & ex.Message)
                End Try
                chkMoveBin.Enabled = True
                TabControl1.Enabled = True
            End If
        End With
    End Sub

    Private Function ImportUsage(ByRef db As Sqlite, OldUid As Integer, NewUid As Integer) As Integer
        Dim sql As String = ""
        ImportUsage = 0
        Try
            Using usage As DataTable = db.GetData("SELECT * FROM tblUsage WHERE UserId=" & OldUid)
                pbFiles.Maximum += usage.Rows.Count
                For j = 0 To usage.Rows.Count - 1
                    If CInt(dbdata.GetValue("SELECT count(UserId) FROM tblUsage WHERE UserId=" & NewUid & " AND UsageDate='" & ParseDate(usage.Rows(j).Item("UsageDate").ToString) & "';")) < 1 Then
                        sql &= "INSERT INTO tblUsage (UserId,UsageDate,UsageBasic,UsageExtra,UsageWiFree,UsageDayLimit)VALUES(" & NewUid & ",'" & ParseDate(usage.Rows(j).Item("UsageDate").ToString()) & "'," & usage.Rows(j).Item("UsageBasic").ToString & "," & usage.Rows(j).Item("UsageExtra").ToString & "," & usage.Rows(j).Item("UsageWiFree").ToString & "," & usage.Rows(j).Item("UsageDayLimit").ToString & ");"
                        ImportUsage += 1
                        pbFiles.PerformStep()
                    End If
                Next
            End Using

            If sql <> "" Then dbdata.Execute(sql)
        Catch ex As Exception
            Add2Log("IMPORT_DB::USAGE:" & ex.Message & " SQL: " & sql)
            ImportUsage = 0
        End Try
        sql = Nothing
    End Function

    Private Function ImportPeriods(ByRef db As Sqlite, OldUid As Integer, NewUid As Integer) As Integer
        Dim sql As String = ""
        ImportPeriods = 0
        Try
            Using period As DataTable = db.GetData("SELECT * FROM tblPeriod WHERE UserId=" & OldUid)
                pbFiles.Maximum += period.Rows.Count
                For j = 0 To period.Rows.Count - 1
                    If CInt(dbdata.GetValue("SELECT count(UserId) FROM tblPeriod WHERE UserId=" & NewUid & " AND StartDate='" & ParseDate(period.Rows(j).Item("StartDate").ToString) & "' AND EndDate='" & ParseDate(period.Rows(j).Item("EndDate").ToString) & "';")) < 1 Then
                        sql &= "INSERT INTO tblPeriod (UserId,StartDate,EndDate)VALUES(" & NewUid & ",'" & ParseDate(period.Rows(j).Item("StartDate").ToString) & "','" & ParseDate(period.Rows(j).Item("EndDate").ToString) & "');"
                        ImportPeriods += 1
                        pbFiles.PerformStep()
                    End If
                Next
            End Using
            If sql <> "" Then dbdata.Execute(sql)
        Catch ex As Exception
            Add2Log("IMPORT_DB::PERIOD:" & ex.Message & " SQL: " & sql)
            ImportPeriods = 0
        End Try
        sql = Nothing
    End Function
End Class