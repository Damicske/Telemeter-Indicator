Public Class frmPopup
    Private iCounter As Int16 = 0, iShowTime As Int16 = 0

    Public Enum gtLocation
        Top
        Bottom
        Left
        Right
    End Enum

    Public Function GetTaskbarLocation() As gtLocation
        Dim bounds As Rectangle = My.Computer.Screen.Bounds
        Dim working As Rectangle = My.Computer.Screen.WorkingArea
        If working.Height < bounds.Height And working.Y > 0 Then
            Return gtLocation.Top
        ElseIf working.Height < bounds.Height And working.Y = 0 Then
            Return gtLocation.Bottom
        ElseIf working.Width < bounds.Width And working.X > 0 Then
            Return gtLocation.Left
        ElseIf working.Width < bounds.Width And working.X = 0 Then
            Return gtLocation.Right
        Else
            Return Nothing
        End If
    End Function

    Public Shared Sub ShowTip(ByVal ShowTimeInSeconds As Int16, ByVal TipTitle As String, ByVal TipText As String, ByVal TipIcon As ToolTipIcon)
        With frmPopup
            If TipTitle.Length > 50 Then
                .Text = TipTitle.Substring(1, 47) & "..."
            Else
                .Text = TipTitle
            End If
            .reset()
            .iShowTime = ShowTimeInSeconds
            .lblText.Text = TipText
            .ClientSize = New Size(210, .lblText.Height) '34 is win7 minimum height
            Dim taskbar As Rectangle = Nothing
            TaskBarPosition.GetTaskBarPosition(taskbar, .Handle)
            Select Case .GetTaskbarLocation()
                Case gtLocation.Bottom
                    .Location = New Point(My.Computer.Screen.WorkingArea.Width - .Width, My.Computer.Screen.WorkingArea.Height - .Height)
                Case gtLocation.Top
                    .Location = New Point(My.Computer.Screen.WorkingArea.Width - .Width, -1 * taskbar.Height)
                Case gtLocation.Left
                    .Location = New Point(taskbar.Width, My.Computer.Screen.WorkingArea.Height - .Height)
                Case gtLocation.Right
                    .Location = New Point(My.Computer.Screen.WorkingArea.Width - .Width, My.Computer.Screen.WorkingArea.Height - .Height)
            End Select
            taskbar = Nothing
            .Timer1.Enabled = True
            .Visible = True
        End With
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        iCounter += 1
        If iCounter > iShowTime Then
            Timer1.Interval = 100
            '-fade out
            Me.Opacity -= 0.02
            If iCounter > iShowTime + 500 Then Me.Close()
        End If
    End Sub

    Private Sub frmPopup_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        Me.Close()
    End Sub

    Private Sub frmPopup_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        reset()
    End Sub

    Private Sub reset()
        '-reset timer
        iCounter = 0
        Me.Opacity = 1
        Timer1.Interval = 1000
    End Sub

    Private Sub lblText_MouseMove(sender As Object, e As MouseEventArgs) Handles lblText.MouseMove
        reset()
    End Sub

    Private Sub lblText_Click(sender As Object, e As EventArgs) Handles lblText.Click
        Me.Close()
    End Sub
End Class

Public Class TaskBarPosition
    Private Declare Function SHAppBarMessage Lib "shell32.dll" Alias "SHAppBarMessage" (ByVal dwMessage As Integer, ByRef pData As APPBARDATA) As Integer

    Private Structure APPBARDATA
        Dim cbSize As Integer
        Dim hwnd As IntPtr
        Dim uCallbackMessage As Integer
        Dim uEdge As Integer
        Dim rc As RECT
        Dim lParam As Integer
    End Structure

    Private Structure RECT
        Dim Left As Integer
        Dim Top As Integer
        Dim Right As Integer
        Dim Bottom As Integer
    End Structure

    Const ABM_GETTASKBARPOS = &H5
    Const ABM_GETSTATE = &H4

    Shared Function GetTaskBarPosition(ByRef CoordinateRectangle As Rectangle, ByVal hwnd As IntPtr) As Boolean
        Try
            Dim abd As New APPBARDATA
            abd.hwnd = hwnd
            SHAppBarMessage(ABM_GETTASKBARPOS, abd)
            CoordinateRectangle = New Rectangle(abd.rc.Left, abd.rc.Top, abd.rc.Right - abd.rc.Left, abd.rc.Top - abd.rc.Bottom)
            'we made it here so success
            Return True
        Catch ex As Exception
            'something bad happened so return false
            Return False
        End Try
    End Function
End Class