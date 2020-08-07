Imports System.ComponentModel
Imports System.Text.RegularExpressions

Public Class frmConnections
    Private LastColumn As Integer
    Private bLoad As Boolean = True

    Private Sub FrmConnections_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        lvCons.ListViewItemSorter = Nothing
    End Sub

    Private Sub FrmConnections_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim FileToRead As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, frmSettings.txtUserId.Text & "_advancedsettings.htm")
        If Not File.Exists(FileToRead) Then
            MsgBox("Advancedsettings file doesn't exist")
            frmSettings.mnuMijnTelenetConnecties.Enabled = False
            Exit Sub
        End If
        '  Dim dtFileLastWrite As DateTime = File.GetLastWriteTime(FileToRead)

        lvCons.Width = ClientSize.Width
        lvCons.Height = ClientSize.Height

        Dim sSize() As String = My.Settings.ConCols.Split(CChar(","))
        If sSize.Length = 5 Then
            lvCons.Columns(0).Width = CInt(sSize(0))
            lvCons.Columns(1).Width = CInt(sSize(1))
            lvCons.Columns(2).Width = CInt(sSize(2))
            lvCons.Columns(3).Width = CInt(sSize(3))
            lvCons.Columns(4).Width = CInt(sSize(4))
        Else
            Add2Log("CONNECTIONS_ERR: Column size is the wrong size")
        End If

#Region "window"
        Dim sError As String = SetWindowSize(My.Settings.LocationConn, Me, False)
        If sError <> "" Then Add2Log("CONNECTIONS_ERR::LOCATION: " & sError)
        sError = Nothing
#End Region

        Try
            Dim sLineTmp As String, sTmp As String
            Dim bToestellen As Boolean = False, bTR As Boolean = False
            Dim iCount As Integer = 0
            Dim m As Match

            Using objReader As New StreamReader(FileToRead, False)
                Do Until objReader.Peek <= 0
                    sLineTmp = objReader.ReadLine
                    If bToestellen Then
                        If sLineTmp.IndexOf("</table>") > -1 Then Exit Do
                        If bTR Then
                            If sLineTmp.IndexOf("</tr>") > -1 Then
                                bTR = False
                                iCount = 0
                            Else
                                'add to list
                                If sLineTmp.IndexOf("&nbsp;") < 0 And sLineTmp.IndexOf(">") > -1 And sLineTmp.IndexOf("</") > -1 Then
                                    m = Regex.Match(sLineTmp, ">(.*?)<\/")
                                    If m.Success Then
                                        sTmp = m.Groups(1).Value.Replace("IPv4: ", "").Replace("IPv6: ", "").Replace("Draadloos", OtherText(7)).Replace("Netwerkkabel", OtherText(6))
                                        If iCount = 0 Then
                                            lvCons.Items.Add(sTmp)
                                        Else
                                            lvCons.Items(lvCons.Items.Count - 1).SubItems.Add(sTmp)
                                        End If
                                        If m.Groups(1).Value.IndexOf("IPv4") > -1 Then
                                            lvCons.Items(lvCons.Items.Count - 1).SubItems.Add("")
                                            iCount = 2
                                        End If
                                    End If
                                    If sLineTmp.IndexOf("</td>") > -1 Then iCount += 1
                                End If
                            End If
                        Else
                            If sLineTmp.IndexOf("<tr align=") > -1 Then bTR = True
                        End If
                    Else
                        If sLineTmp.IndexOf("(toestellen)") > -1 Then bToestellen = True
                    End If
                Loop
            End Using
        Catch Ex As Exception
            Add2Log("CONNECTIONS::READ::" & Ex.Message)
        End Try
        lvCons.Sorting = SortOrder.Ascending
        lvCons.ListViewItemSorter = New ListViewItemComparer(0)
        lvCons.Sort()
        bLoad = False
    End Sub

    Private Sub lvCons_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles lvCons.ColumnClick
        If LastColumn = e.Column Then
            lvCons.Sorting = SortOrder.Descending
        Else
            lvCons.Sorting = SortOrder.Ascending
        End If
        lvCons.ListViewItemSorter = New ListViewItemComparer(e.Column)
        LastColumn = e.Column
    End Sub

    Private Sub lvCons_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles lvCons.ColumnWidthChanged
        If bLoad Then Exit Sub
        My.Settings.ConCols = lvCons.Columns(0).Width & "," & lvCons.Columns(1).Width & "," & lvCons.Columns(2).Width & "," & lvCons.Columns(3).Width & "," & lvCons.Columns(4).Width
    End Sub

    Private Sub frmConnections_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Dim x(1) As String
        x(0) = Location.X.ToString
        x(1) = Location.Y.ToString
        My.Settings.LocationConn = String.Join(",", x)
        My.Settings.Save()
    End Sub
End Class

'- Implements the manual sorting of items by columns.
'- https://msdn.microsoft.com/en-us/library/system.windows.forms.listview.columnclick%28v=vs.110%29.aspx
Class ListViewItemComparer
    Implements IComparer
    Private col As Integer

    Public Sub New()
        col = 0
    End Sub

    Public Sub New(ByVal column As Integer)
        col = column
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
       Implements IComparer.Compare
        Return [String].Compare(CType(x, ListViewItem).SubItems(col).Text, CType(y, ListViewItem).SubItems(col).Text)
    End Function
End Class
