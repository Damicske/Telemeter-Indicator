Imports System.Data

Public Class frmLog
    Private Shared LastLogId As Integer = -1
    Private bLoad As Boolean = True
    Private Shared bListLock As Boolean = False

    Private Sub FrmLog_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Visible = False
        Dim x(4) As String
        x(0) = Location.X.ToString
        x(1) = Location.Y.ToString
        x(2) = Width.ToString
        x(3) = Height.ToString
        x(4) = CInt(WindowState).ToString
        My.Settings.LocationLog = String.Join(",", x)
        LastLogId = Nothing
        bShowLog = False
    End Sub

    Private Sub FrmLog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        gbFilter.Hide()
        pbLoad.Hide()
        gbFilter.Location = New Point(0, 0)
        bShowLog = True
#Region "place on screen"
        Dim sError As String = SetWindowSize(My.Settings.LocationLog, Me)
        If sError <> "" Then Add2Log("LOG::LOAD_ERR: " & sError)
        sError = Nothing
#End Region

        lvLog.Items.Clear()

        lvLog.Columns.Item(0).Width = CInt(My.Settings.LbColTime)
        chkShowErr.Checked = CBool(My.Settings.LbShowErr)
        ChkShowHtml.Checked = CBool(My.Settings.LbShowHtml)
        chkShowDbg.Checked = CBool(My.Settings.LbShowDbg)
        cmsColorLine.Checked = CBool(My.Settings.LbShowCol)
        mcFilter.SelectionStart = CDate(mcFilter.TodayDate & " 00:00:00")
        mcFilter.SelectionEnd = CDate(mcFilter.TodayDate & " 23:59:59")

        UpdateLogView()
        bLoad = False
    End Sub

    Public Sub UpdateLogView(Optional DayStart As String = "NOW", Optional DayStop As String = "NOW")
        'If InvokeRequired Then
        '    lvLog.Invoke(New Action(Sub() UpdateLogView(DayStart, DayStop)))
        '    Return
        'End If
        If bListLock Then Exit Sub
        bListLock = True
        If Not IsDate(DayStart) Then DayStart = Date.UtcNow.ToString("dd/MM/yyyy 00:00:00")
        If Not IsDate(DayStop) Then DayStop = Date.UtcNow.ToString("dd/MM/yyyy 23:59:59")
        '-set filter
        Dim filter As String = ""
        If Not chkShowErr.Checked Then filter &= "LogType!=1 and "
        If Not ChkShowHtml.Checked Then filter &= "LogType!=2 and "
        If Not chkShowDbg.Checked Then filter &= "LogType!=4 and "

        '-get the data
        Using dt As DataTable = dblog.GetData("SELECT LogId, Message, LogType, UTCtime FROM tblLog WHERE " & filter & "LogId> " & LastLogId & " and (UTCtime BETWEEN " & ToUnix(Date.Parse(DayStart)) & " AND " & ToUnix(Date.Parse(DayStop)) & ") order by UTCtime ASC;")
            If dt.Rows.Count <= 0 Then Exit Sub
            Try
                If LastLogId = CInt(dt.Rows(dt.Rows.Count - 1).Item("LogId")) Then lvLog.Items(lvLog.Items.Count - 1).Remove()
            Catch e As Exception
                Debug.WriteLine("RefreshLog: remove last entry " & e.Message)
            End Try

            LastLogId = CInt(dt.Rows(dt.Rows.Count - 1).Item("LogId"))
            Debug.WriteLine("LastId: " & LastLogId)
            Dim col As Color
            pbLoad.Value = 0
            pbLoad.Maximum = dt.Rows.Count
            For i = 0 To dt.Rows.Count - 1
                If Not pbLoad.Visible Then pbLoad.Show()
                Try
                    pbLoad.PerformStep()
                    lvLog.Items.Add(FromUnix(CInt(dt.Rows(i).Item("UTCtime"))).ToString("dd/MM/yyyy HH:mm:ss UTC"))
                    Select Case CInt(dt.Rows(i).Item("LogType"))
                        Case 1 'error and stuff
                            col = Color.Red
                        Case 2 'html
                            col = Color.LightGreen
                        Case 3 'fetch/request
                            col = Color.MediumPurple
                        Case 4 'debug
                            col = Color.Orange
                        Case Else
                            col = lvLog.BackColor
                    End Select
                    lvLog.Items(lvLog.Items.Count - 1).SubItems.Add(dt.Rows(i).Item("Message").ToString).BackColor = col
                    lvLog.Items(lvLog.Items.Count - 1).UseItemStyleForSubItems = Not cmsColorLine.Checked
                Catch ex As Exception
                    Debug.WriteLine("RefreshLog: " & ex.Message)
                End Try
            Next
            col = Nothing
        End Using
        lvLog.EnsureVisible(lvLog.Items.Count - 1)
        pbLoad.Hide()
        bListLock = False
    End Sub

    Private Sub FrmLog_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        With lvLog
            .Location = New Point(0, 0)
            .Size = New Size(ClientSize.Width, ClientSize.Height)
            If .ClientSize.Width - .Columns.Item(0).Width > 200 Then .Columns.Item(1).Width = .ClientSize.Width - 150
        End With
        pbLoad.Location = New Point(0, CInt(ClientSize.Height / 2 - pbLoad.Height / 2))
        pbLoad.Width = ClientSize.Width
    End Sub

    Private Sub bntFilterClose_Click(sender As Object, e As EventArgs) Handles bntFilterClose.Click
        gbFilter.Hide()
    End Sub

    Private Sub cmsFilter_Click(sender As Object, e As EventArgs) Handles cmsFilter.Click
        gbFilter.Show()
    End Sub

    Private Sub btnFilterApply_Click(sender As Object, e As EventArgs) Handles btnFilterApply.Click
        lvLog.Items.Clear()
        LastLogId = -1
        UpdateLogView(mcFilter.SelectionStart.ToString, mcFilter.SelectionEnd.ToString)
    End Sub

    Private Sub chkShowErr_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowErr.CheckedChanged
        My.Settings.LbShowErr = chkShowErr.Checked
    End Sub

    Private Sub ChkShowHtml_CheckedChanged(sender As Object, e As EventArgs) Handles ChkShowHtml.CheckedChanged
        My.Settings.LbShowHtml = ChkShowHtml.Checked
    End Sub

    Private Sub chkShowDbg_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowDbg.CheckedChanged
        My.Settings.LbShowDbg = chkShowDbg.Checked
    End Sub

    Private Sub lvLog_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles lvLog.ColumnWidthChanged
        If Not bLoad Then My.Settings.LbColTime = lvLog.Columns.Item(0).Width
    End Sub

    Private Sub cmsCopyLine_Click(sender As Object, e As EventArgs) Handles cmsCopyLine.Click
        If lvLog.SelectedItems.Count > 0 Then
            Dim CopyTxt As String = ""
            For i = 0 To lvLog.SelectedItems.Count - 1
                CopyTxt &= If(CopyTxt = "", "", Environment.NewLine) & "[" & lvLog.SelectedItems(i).SubItems(0).Text & "] " & lvLog.SelectedItems(i).SubItems(1).Text
            Next
            My.Computer.Clipboard.SetText(CopyTxt)
        End If
    End Sub

    Private Sub cmsColorLine_Click(sender As Object, e As EventArgs) Handles cmsColorLine.Click
        My.Settings.LbShowCol = cmsColorLine.Checked
        If bLoad Or bListLock Then Exit Sub
        If lvLog.Items.Count = 0 Then Exit Sub
        bListLock = True
        pbLoad.Show()
        pbLoad.Maximum = lvLog.Items.Count
        pbLoad.Value = 0
        For i = 0 To lvLog.Items.Count - 1
            lvLog.Items(i).UseItemStyleForSubItems = Not cmsColorLine.Checked
            pbLoad.PerformStep()
        Next i
        lvLog.Refresh()
        pbLoad.Hide()
        bListLock = False
    End Sub
End Class