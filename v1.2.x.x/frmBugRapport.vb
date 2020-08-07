Imports System.Data

Public Class frmBugRapport

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        '-disabled buttons and stuff
        btnSend.Enabled = False
        txtEmail.Enabled = False
        txtProblem.Enabled = False
        chkSendTelemeter.Enabled = False
        chkSendLog.Enabled = False
        '-get data, put it in a string and send it
        Dim sParams As String = "p=15&v=" & VersionX(True).ToString & "&uid=" & frmSettings.txtUserId.Text & "&pw=" & GetSHA512Hash(frmSettings.mtbPassword.Text & frmSettings.txtUserId.Text) & If(txtEmail.Text <> "", "&e=" & txtEmail.Text, "") & "&b=" & txtProblem.Text & "&aid=" & modTIFunc.pData.CustomerNr

        Dim iLogId As Integer = -1
        Dim sOutput As String = Http_Request(SDebugUrl, "TelemeterIndicator", sParams, True,, iLogId) '

        Dim sMsg As String
        Dim bError As Boolean = True
        If sOutput.LastIndexOf("SuccesFull") > -1 Then
            bError = False
            If sOutput.LastIndexOf("File:") > -1 Then
                sMsg = "Your file has arrived"
            Else
                sMsg = "Your feedback is saved and will be looked at in the near future." & If(chkSendTelemeter.Checked, Environment.NewLine & "The extra debug pages will be send on the next refresh", "")
                txtProblem.Text = ""
                iBugId = CInt(sOutput.Substring(sOutput.LastIndexOf("#") + 2))
                If chkSendTelemeter.Checked And (sOutput.LastIndexOf("#") > 0) Then
                    bTeleUp = True
                End If
                If chkSendLog.Checked And iBugId > 0 Then
                    Dim sFileName As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "ti_lb_" & My.Settings.UserId & "_" & Date.UtcNow.ToString("dd-MM-yyyy HH.mm.ssUTC") & ".log")
                    '-create file
                    Dim dt As DataTable = dblog.getData("SELECT UserName, LogId, Message, LogType, UTCtime FROM tblLog WHERE (UTCtime BETWEEN " & ToUnix(Date.Parse(Date.UtcNow.Day & "/" & Date.UtcNow.Month & "/" & Date.UtcNow.Year & " 00:00:00")) & " AND " & ToUnix(Date.Parse(Date.UtcNow.Day & "/" & Date.UtcNow.Month & "/" & Date.UtcNow.Year & " 23:59:59")) & ") order by UTCtime ASC;")
                    If dt.Rows.Count > 0 Then
                        Using writer As New StreamWriter(sFileName)
                            For i = 0 To dt.Rows.Count - 1
                                writer.WriteLine(FromUnix(CInt(dt.Rows(i).Item("UTCtime"))).ToString("dd/MM/yyyy HH:mm:ss UTC") & ";" &
                                    dt.Rows(i).Item("UserName").ToString & ";" &
                                    dt.Rows(i).Item("LogType").ToString & ";" &
                                    dt.Rows(i).Item("Message").ToString())
                            Next
                        End Using
                        Add2Log(Http_Request(SDebugUrl, "TelemeterIndicator", "p=15&v=" & VersionX(True).ToString & "&uid=" & My.Settings.UserId & "&bid=" & iBugId & "&f=" & My.Settings.UserId & sFileName.Substring(sFileName.LastIndexOf("\")), True, New List(Of String) From {sFileName}))
                    End If
                    If File.Exists(sFileName) Then File.Delete(sFileName)
                    sFileName = Nothing
                    dt = Nothing
                End If
            End If
        ElseIf (sOutput.LastIndexOf("::") > 0) Then
            If sOutput.LastIndexOf("File") = 0 Then
                sMsg = "File Error: " & sOutput.Replace("File::", "")
            ElseIf sOutput.LastIndexOf("SQL") = 0 Then
                sMsg = "SQL Error: " & sOutput.Replace("SQL::", "")
            Else
                sMsg = "Other Error: " & sOutput.Replace("Err::", "")
            End If
        ElseIf sOutput.LastIndexOf("NoUid") > 0 Then
            sMsg = "Error: No user id given"
        Else
            sMsg = "Return: " & sOutput
        End If
        '-debug stuff
        ' If bError Then
        'Dim sOutput2 As String() = sOutput.Split(Environment.NewLine)
        'Using file As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\telemeter_debug", True)
        'File.WriteLine(Date.Now.ToString("dd/MM/yyyy HH:mm:ss") & "<p />\n")
        'For i = 0 To sOutput2.Length - 1
        'If sOutput2(i) <> Environment.NewLine Then File.WriteLine(sOutput2(i))
        'Next i
        'End Using
        'End If
        Add2Log(sMsg, ilogid)
        MsgBox(sMsg, CType(If(bError, MsgBoxStyle.Exclamation, MsgBoxStyle.Information), MsgBoxStyle), "TelemeterIndicator Bug Rapport")

        '-enable everything
        btnSend.Enabled = True
        txtEmail.Enabled = True
        txtProblem.Enabled = True
        chkSendTelemeter.Enabled = True
        chkSendLog.Enabled = True
        bError = False
    End Sub

    Private Sub FrmBugRapport_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If txtProblem.Text.Length > 10 Then
            If MsgBox("Inputted data will be removed, continu?", CType(MsgBoxStyle.Exclamation & MsgBoxStyle.YesNo, MsgBoxStyle)) = MsgBoxResult.No Then e.Cancel = True
        End If
    End Sub
End Class