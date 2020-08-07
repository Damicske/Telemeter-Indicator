Public Class frmMobile
    Private bLoad As Boolean = True

    Private Sub FrmMobile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lvData.Width = ClientSize.Width
        lvData.Height = ClientSize.Height

        Dim sSize() As String = My.Settings.MobCols.Split(CChar(","))
        If sSize.Length >= 4 Then
            lvData.Columns(0).Width = CInt(sSize(0))
            lvData.Columns(1).Width = CInt(sSize(1))
            lvData.Columns(2).Width = CInt(sSize(2))
            lvData.Columns(3).Width = CInt(sSize(3))
            If sSize.Length = 5 Then lvData.Columns(4).Width = CInt(sSize(4))
        Else
            Add2Log("MOBILE_ERR: Column size is the wrong size")
        End If

#Region "window"
        Dim sError As String = SetWindowSize(My.Settings.LocationMob, Me, False)
        If sError <> "" Then Add2Log("MOBILE_ERR::LOCATION: " & sError)
        sError = Nothing
#End Region

        LoadData()
        bLoad = False
    End Sub

    Public Sub LoadData()
        lvData.Items.Clear()
        Dim Proc As Double = 0, Used As Double = 0
        Dim limit As Integer = 0
        With lvData
            For i = 0 To MobAbo.Count - 1
                Try
                    limit = CInt(MobAbo(i).Limit.Replace(" MB", "").Replace(" GB", "").Replace(" kB", ""))
                    Used = limit - CDbl(MobAbo(i).Usage.Replace(" MB", "").Replace(" GB", "").Replace(" KB", ""))
                    Proc = Used / limit '* 100
                    .Items.Add(MobAbo(i).Cell)
                    .Items(.Items.Count - 1).SubItems.Add(MobAbo(i).Abo)
                    .Items(.Items.Count - 1).SubItems.Add(MobAbo(i).Limit.ToString)
                    .Items(.Items.Count - 1).SubItems.Add(Used.ToString("#.00") & " MB " & Proc.ToString("#.00%"))
                    If MobAbo(i).Extra <> "" Then .Items(.Items.Count - 1).SubItems.Add("€ " & MobAbo(i).Extra)
                    .Items(.Items.Count - 1).UseItemStyleForSubItems = False
                Catch ex As Exception
                    Add2Log("MOBILE_ERR_LOADDATA: " & ex.Message & " Data: " & MobAbo(i).Abo & "_limit-" & MobAbo(i).Limit & "_usage-" & MobAbo(i).Usage & "_extra-" & MobAbo(i).Extra)
                End Try
            Next
        End With
    End Sub

    Private Sub lvData_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles lvData.ColumnWidthChanged
        If bLoad Then Exit Sub
        My.Settings.MobCols = lvData.Columns(0).Width & "," & lvData.Columns(1).Width & "," & lvData.Columns(2).Width & "," & lvData.Columns(3).Width & "," & lvData.Columns(4).Width
    End Sub

    Private Sub FrmMobile_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Visible = False
        Dim x(1) As String
        x(0) = Location.X.ToString
        x(1) = Location.Y.ToString
        My.Settings.LocationMob = String.Join(",", x)
        My.Settings.Save()
    End Sub
End Class