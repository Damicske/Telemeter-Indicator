Public Class frmLog

    Private Sub frmLog_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Me.Visible = False
        Dim x(4) As String
        x(0) = Me.Location.X.ToString
        x(1) = Me.Location.Y.ToString
        x(2) = Me.Width.ToString
        x(3) = Me.Height.ToString
        x(4) = CInt(Me.WindowState).ToString
        My.Settings.LocationLog = String.Join(",", x)
        bShowLog = False
        GC.Collect()
    End Sub

    Private Sub frmLog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '-place on screen
        Dim il As Point = New Point(0, 0)
        Dim sz As Size = Size
        Try
            Dim initLocation As String = My.Settings.LocationLog
            If Not String.IsNullOrEmpty(initLocation) Then
                Dim parts As String() = initLocation.Split(CChar(","))
                If parts.Length >= 2 Then il = New Point(CInt(parts(0)), CInt(parts(1)))
                If parts.Length >= 4 Then sz = New Size(CInt(parts(2)), CInt(parts(3)))
                If parts.Length = 5 Then Me.WindowState = CType(parts(4), FormWindowState) Else Me.WindowState = FormWindowState.Normal
            End If
        Catch ex As Exception
            Add2Log("LOG::LOAD_ERR: " & ex.Message)
        Finally
            Size = sz
            Location = il
        End Try
        bShowLog = True
        Me.Show()
        With txtLog
            .Clear()
            If sLog.Length > 10 Then
                .Focus()
                For Each line As String In sLog.Split(CChar(Environment.NewLine))
                    color_log_line(line)
                Next
            End If
        End With
    End Sub

    Private Shared bHtml As Boolean = False
    Public Sub color_log_line(ByVal line As String)
        Dim iBracket As Int16 = 0
        With txtLog
            If Not bHtml Then
                '-reset to normal color
                .SelectionStart = .TextLength + line.Length
                .SelectionBackColor = SystemColors.Control
            Else
                .SelectionStart = .TextLength - line.Length
                .SelectionBackColor = Color.LightGreen
            End If
            .AppendText(line)
            iBracket = CShort(line.IndexOf("] ") + 2)
            If (line.ToLower.IndexOf("<html") > -1) Or bHtml Then
                '-html color
                .SelectionStart = .TextLength - line.Length + iBracket
                .SelectionLength = line.Length - iBracket
                .SelectionBackColor = Color.LightGreen
                bHtml = True
            ElseIf line.ToLower.IndexOf("timeout") > -1 Or line.IndexOf("TMR::") > -1 Or line.IndexOf("no or high") > -1 Or
                line.IndexOf("No active") > -1 Or line.ToLower.IndexOf("err") > -1 Or line.IndexOf("failed") > -1 Then
                '-error color
                .SelectionStart = .TextLength - line.Length + iBracket
                .SelectionLength = line.Length - iBracket
                .SelectionBackColor = Color.Red
            ElseIf line.ToLower.IndexOf("fetch:") > -1 Then
                '-request color
                .SelectionStart = .TextLength - line.Length + iBracket
                .SelectionLength = line.Length - iBracket
                .SelectionBackColor = Color.MediumPurple
            ElseIf line.ToLower.IndexOf("debug:") > -1 Then
                '-debug color
                .SelectionStart = .TextLength - line.Length + iBracket
                .SelectionLength = line.Length - iBracket
                .SelectionBackColor = Color.Orange
            End If
            If bHtml And line.ToLower.IndexOf("</html") > -1 Then
                bHtml = False
                .SelectionStart = .TextLength
                .SelectionBackColor = SystemColors.Control
            End If
        End With
        iBracket = Nothing
    End Sub
End Class