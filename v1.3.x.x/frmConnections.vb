Imports System.IO

Public Class frmConnections
    Private LastColumn As Int32

    Private Sub frmConnections_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim FileToRead As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "data\" & frmSettings.txtUserId.Text & "_advancedsettings.htm")
        If Not File.Exists(FileToRead) Then
            MsgBox("Advancedsettings file doesn't exist")
            frmSettings.mnuMijnTelenetConnecties.Enabled = False
            Exit Sub
        End If
        '  Dim dtFileLastWrite As DateTime = File.GetLastWriteTime(FileToRead)
        Try
            Dim sLineTmp As String, str(5) As String, sTmp As String
            Dim bToestellen As Boolean = False, bTR As Boolean = False
            Dim iStart As Int16, iCount As Int16 = 0
            Dim lstList As ListViewItem

            Using objReader As New StreamReader(FileToRead, False)
                Do Until objReader.Peek <= 0
                    sLineTmp = objReader.ReadLine
                    If bToestellen Then
                        If sLineTmp.IndexOf("</table>") > -1 Then Exit Do
                        If bTR Then
                            If sLineTmp.IndexOf("</tr>") > -1 Then
                                bTR = False
                                If str(0).Length > 0 Then
                                    lstList = New ListViewItem(str)
                                    ListView1.Items.Add(lstList)
                                    For i = 0 To 4
                                        str(i) = ""
                                    Next
                                    iCount = 0
                                End If
                            Else
                                'add to list
                                If sLineTmp.IndexOf("&nbsp;") < 0 And sLineTmp.IndexOf(">") > -1 And sLineTmp.IndexOf("</") > -1 Then
                                    iStart = cshort(sLineTmp.IndexOf(">") + 1)
                                    If sLineTmp.IndexOf("</") > iStart Then
                                        sTmp = sLineTmp.Substring(iStart, sLineTmp.IndexOf("</") - iStart)
                                        str(iCount) = sTmp.Replace("IPv4: ", "").Replace("IPv6: ", "").Replace("Draadloos", OtherText(7)).Replace("Netwerkkabel", OtherText(6))
                                        If sTmp.IndexOf("IPv4") > -1 Then iCount = 2
                                    End If
                                End If
                                If sLineTmp.IndexOf("</td>") > -1 Then iCount += 1
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
        ListView1.Sorting = SortOrder.Ascending
        ListView1.ListViewItemSorter = New ListViewItemComparer(0)
        ListView1.Sort()
    End Sub

    Private Sub ListView1_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles ListView1.ColumnClick
        If LastColumn = e.Column Then
            ListView1.Sorting = SortOrder.Descending
        Else
            ListView1.Sorting = SortOrder.Ascending
        End If
        ListView1.ListViewItemSorter = New ListViewItemComparer(e.Column)
        LastColumn = e.Column
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
