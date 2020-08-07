Imports System.Drawing.Drawing2D
Imports System.Data

Public Class frmGraph
#Region "declarations"
    Private LastPeriodMenu As ToolStripMenuItem
    Private sFormCaption As String

    '-define text type+size
    Private graphTextFont As Font = New Font("Verdana", 8, FontStyle.Regular)
    Private graphTextFontLarge As Font = New Font("Verdana", 12, FontStyle.Bold)

    '-define colors
    Private color_line As New Pen(Color.FromArgb(255, 204, 204, 204), 1) '#ccc
    Private color_basis As New SolidBrush(Color.FromArgb(255, 255, 196, 33)) '#89a54e > 137,165,78 
    Private color_basis_mnu As Color = Color.FromArgb(255, 255, 196, 33) '#89a54e > 137,165,78 
    Private color_extra As New SolidBrush(Color.FromArgb(255, 247, 148, 47)) '#447019 > 68,112,25
    Private color_extra_mnu As Color = Color.FromArgb(255, 247, 148, 47) '#447019 > 68,112,25
    Private color_pay As New SolidBrush(Color.FromArgb(255, 128, 255, 128))
    Private color_pay_mnu As Color = Color.FromArgb(255, 128, 255, 128)
    Private color_wifi As New SolidBrush(Color.FromArgb(255, 76, 218, 0)) '232,218,0
    Private color_wifi_mnu As Color = Color.FromArgb(255, 76, 218, 0) '232,218,0
    Private color_text As New SolidBrush(Color.FromArgb(255, 153, 153, 153)) '#999
    Private color_text_legend As New SolidBrush(Color.FromArgb(255, 62, 87, 111)) '#3E576F

    '-data switch data
    Private Graph_Data As New List(Of TeleParse.GraphData)
    Private gUsage_DayMax As Long = 0

    'Private gBfup As Boolean = False
    Private bHistory As Boolean = False
#End Region

    Public Sub DataSwitch(ByVal iTag As Integer)
        Graph_Data.Clear()
        If iTag < 1 Then
            Try
                Graph_Data.AddRange(pData.Data)
                gUsage_DayMax = pData.Usage_DayMax
                bHistory = False
            Catch ex As Exception
                MsgBox(ex.Message, , "Error: LoadData")
                Add2Log("GRAPH::DATASWITCH_DATA_ERR: " & ex.Message)
                Exit Sub
            End Try
        Else
            Try
                Dim iCount As Short = 0
                Dim basic_volume_max As Long = 0, extra_volume_max As Long = 0, payg_volume_max As Long = 0, wifree_volume_max As Long = 0

                If db_UserId > 0 Then
                    'dbdata.GetRow("tblPeriod", "StartDate, EndDate", "Id=" & iTag & " AND UserId=" & db_UserId)
                    Dim dic As Dictionary(Of String, String) = dbdata.GetRow("Select StartDate, EndDate FROM tblPeriod where Id=" & iTag & " AND UserId=" & db_UserId & " LIMIT 1")
                    Dim dt As DataTable = dbdata.getData("SELECT * FROM tblUsage WHERE UserId=" & db_UserId & " AND (UsageDate BETWEEN '" & ParseDate(dic.Item("StartDate")) & "' AND '" & ParseDate(dic.Item("EndDate")) & "')")
                    If dt.Rows.Count = 0 Then Exit Sub
                    For i = 0 To dt.Rows.Count - 1
                        With Graph_Data
                            .Add(New TeleParse.GraphData With {
                            .Basic = CLng(dt.Rows(i).Item("UsageBasic")),
                            .Extra = CLng(dt.Rows(i).Item("UsageExtra")),
                            .Limit = CLng(dt.Rows(i).Item("UsageDayLimit")),
                            .WiFree = CLng(dt.Rows(i).Item("UsageWiFree")),
                            .Day = ParseDate(dt.Rows(i).Item("UsageDate").ToString, "dd/MM")})
                        End With
                        If basic_volume_max < Graph_Data(i).Basic Then basic_volume_max = Graph_Data(i).Basic
                        If extra_volume_max < Graph_Data(i).Extra Then extra_volume_max = Graph_Data(i).Extra
                        If wifree_volume_max < Graph_Data(i).WiFree Then wifree_volume_max = Graph_Data(i).WiFree
                    Next
                    dt.Dispose()
                    dt = Nothing
                End If
                gUsage_DayMax = basic_volume_max + extra_volume_max + payg_volume_max + wifree_volume_max
                bHistory = True
            Catch Ex As Exception
                MsgBox(Ex.Message, , "Error: LoadData")
                Add2Log("GRAPH::DATASWITCH_ERR: " & Ex.Message.Replace(Environment.NewLine, " "))
                Exit Sub
            End Try
        End If
        PictureBox1.Refresh()
    End Sub

    Private Sub FrmGraph_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Visible = False
        Dim x(4) As String
        x(0) = Location.X.ToString
        x(1) = Location.Y.ToString
        x(2) = Width.ToString
        x(3) = Height.ToString
        x(4) = CInt(WindowState).ToString
        My.Settings.LocationGraph = String.Join(",", x)
        My.Settings.Save()
        '-clear data from memory
        LastPeriodMenu.Dispose()
        LastPeriodMenu = Nothing
        graphTextFont.Dispose()
        graphTextFontLarge.Dispose()
        color_line.Dispose()
        color_basis.Dispose()
        color_extra.Dispose()
        color_pay.Dispose()
        color_wifi.Dispose()
        color_text.Dispose()
        color_text_legend.Dispose()
        graphTextFont = Nothing
        graphTextFontLarge = Nothing
        color_line = Nothing
        color_basis = Nothing
        color_basis_mnu = Nothing
        color_extra = Nothing
        color_extra_mnu = Nothing
        color_pay = Nothing
        color_pay_mnu = Nothing
        color_wifi = Nothing
        color_wifi_mnu = Nothing
        color_text = Nothing
        color_text_legend = Nothing
        bShowGraph = False
        Graph_Data = Nothing
    End Sub

    Private Sub FrmGraph_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If pData.Data(0).Day Is Nothing Then
            MsgBox("No data available")
            Close()
            Exit Sub
        End If

        '-do the dataswitch
        LastPeriodMenu = TryCast(mnuPeriodThis, ToolStripMenuItem)
        DataSwitch(-1)
        sFormCaption = Text

#Region "load values"
        mnuShowStandard.Text = GraphText(0)
        If bFUP Then
            mnuShowStandard.Text = GraphText(4)
            mnuShowExtra.Text = GraphText(5)
            mnuShowExtra.Enabled = False
            'mnuShowPay.Enabled = False
            mnuShowWifi.Enabled = False
        End If
        mnuShowStandard.BackColor = color_basis_mnu
        mnuShowExtra.Checked = CBool(My.Settings.ShowExtra)
        mnuShowExtra.BackColor = color_extra_mnu
        'mnuShowPay.Checked = CBool(My.Settings.ShowPay)
        'mnuShowPay.BackColor = color_pay_mnu
        mnuShowWifi.Checked = CBool(My.Settings.ShowWifi)
        mnuShowWifi.BackColor = color_wifi_mnu
        mnuShowAvarage.Checked = CBool(My.Settings.ShowAvg)
        mnuShowAvgCalc.Checked = CBool(My.Settings.ShowAvgCalc)
        mnuShowLimit.Checked = CBool(My.Settings.ShowLimit)
        mnuShowLimitDay.Checked = CBool(My.Settings.ShowLimitDay)
#End Region

#Region "load history usage file list"
        Try
            If db_UserId > 0 Then
                Using dt As DataTable = dbdata.GetData("SELECT * FROM tblPeriod WHERE UserId=" & db_UserId & " ORDER BY StartDate DESC")
                    If dt.Rows.Count > 1 Then ToolStripMenuItem3.Visible = True
                    Dim mnuItem As New ToolStripMenuItem()
                    For Each row As DataRow In dt.Rows
                        If ParseDate(row.Item("StartDate").ToString) <> PeriodStartDate Then
                            mnuItem = New ToolStripMenuItem() With {.Name = "mnuPeriodx", .Tag = CInt(row.Item("Id")), .Text = ParseDate(row.Item("StartDate").ToString, "dd-MM-yyyy") & " - " & ParseDate(row.Item("EndDate").ToString, "dd-MM-yyyy"), .Visible = True, .Checked = False, .Enabled = True}
                            mnuPeriod.DropDownItems.Add(mnuItem)
                            AddHandler mnuItem.Click, AddressOf mnuPeriodThis_Click
                        End If
                    Next
                    'mnuItem.Dispose()
                    'mnuItem = Nothing
                End Using
            End If
        Catch ex As Exception
            Add2Log("GRAPH::LOAD_ERR: " & ex.Message)
        End Try

#End Region
        '-set picturebox
        PictureBox1.BorderStyle = BorderStyle.None
        PictureBox1.Top = 0
        PictureBox1.Left = 0

#Region "window"
        Dim sError As String = SetWindowSize(My.Settings.LocationGraph, Me)
        If sError <> "" Then Add2Log("GRAPH::LOAD_ERR: " & sError)
        sError = Nothing
#End Region

        bShowGraph = True
    End Sub

    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint

        '-set Graph draw area dimensions
        Dim graphWdth As Integer = PictureBox1.ClientRectangle.Width
        Dim graphHght As Integer = PictureBox1.ClientRectangle.Height
        Application.DoEvents()

        '-show bgimage
        Dim imgWidth As Short = CShort(My.Resources.Telenet_wiki_transparent.Width), imgHeight As Short = CShort(My.Resources.Telenet_wiki_transparent.Height)
        If (graphWdth > imgWidth) And (graphHght > imgHeight) Then
            e.Graphics.DrawImage(My.Resources.Telenet_wiki_transparent, (graphWdth - imgWidth) \ 2, (graphHght - imgHeight) \ 2, imgWidth, imgHeight)
        Else
            e.Graphics.DrawImage(My.Resources.Telenet_wiki_transparent, graphWdth \ 4, graphHght \ 4, graphWdth \ 2, graphHght \ 2)
        End If
        imgHeight = Nothing
        imgWidth = Nothing

        If Graph_Data.Count = 0 Then
            e.Graphics.DrawString("No data", New Font("Verdana", 20, FontStyle.Bold), Brushes.Black, (graphWdth - imgWidth) \ 2, (graphHght - imgHeight) \ 2)
            Exit Sub
        End If

        '-set vars
        Dim stringSize As SizeF = e.Graphics.MeasureString("88/888", graphTextFont)
        Dim strSizeUsage As SizeF = e.Graphics.MeasureString("99.99 GB", graphTextFont)
        Dim ScaleBlocks As Single = CSng(gUsage_DayMax / 5) 'block size
        Dim FillProcent As Single = (gUsage_DayMax + ScaleBlocks) / (graphWdth - stringSize.Width - 11)
        Dim horizLines As Integer = Graph_Data.Count - 1
        Const StartHght As Short = 7
        Dim CurrentMarker As Integer = 0
        Dim rect As Rectangle
        Dim TextSize As SizeF
        Dim BasicStart As Integer = CInt(stringSize.Width + 11) '-x position

        '-draw vertical lines 
        Dim GraphWdthStep As Single = (graphWdth - stringSize.Width + 10) / (gUsage_DayMax / ScaleBlocks + 1)
        graphHght -= CInt(stringSize.Height)

        Dim j As Int32 = 0
        For i = stringSize.Width + 10 To graphWdth Step GraphWdthStep
            e.Graphics.DrawLine(color_line, i, 0, i, graphHght - If(j = 0 And mnuShowAvgCalc.Checked And mnuShowAvarage.Checked, stringSize.Height, -stringSize.Height))
            If CInt(j) > 0 Then
                TextSize = e.Graphics.MeasureString(Convert_MB(CLng(j), My.Settings.ShowBinair), graphTextFont)
                e.Graphics.DrawString(Convert_MB(CLng(j), My.Settings.ShowBinair), graphTextFont, color_text_legend, i, graphHght)
            End If
            j += CInt(ScaleBlocks + FillProcent)
        Next i
        j = Nothing
        ScaleBlocks = Nothing

        Dim AvgCalc As Long = CLng((pData.Usage_MBDiff + pData.Usage_Today) / CInt(DateDiff(DateInterval.Day, Date.Now, pData.TeleReset) + 1))
        '-green line = what you still may use the next days
        Dim x_avgc As Single = 0, x_avg As Single = 0
        If mnuShowAvgCalc.Checked And Not bHistory Then
            If AvgCalc > 0 Then
                x_avgc = BasicStart + Convert.ToSingle(AvgCalc / FillProcent)
                If x_avgc > graphWdth Then x_avgc = graphWdth - 5 '-if went of scale trow it back
                e.Graphics.DrawLine(Pens.Green, x_avgc, StartHght, x_avgc, graphHght)
            End If
        End If

        '-red line avarage usage
        Dim iAvgValue As Int64 = 0, iAvgCount As Short = 0
        If mnuShowAvarage.Checked Then
            x_avg = BasicStart
            For i = 0 To Graph_Data.Count - 1
                If Graph_Data.Count > 0 Then
                    iAvgValue += Graph_Data(i).Basic + If(bFUP, Graph_Data(i).Extra, 0)
                    iAvgCount = CShort(iAvgCount + 1)
                End If
            Next i
            If iAvgCount > 0 Then
                iAvgValue = CLng(iAvgValue / iAvgCount)
                x_avg += Convert.ToSingle(iAvgValue / FillProcent)
            End If
            If x_avg > graphWdth Then x_avg = graphWdth - 5 '-if went of scale trow it back
            e.Graphics.DrawLine(Pens.DarkRed, x_avg, StartHght, x_avg, graphHght)
        End If

        '-green line
        If mnuShowAvgCalc.Checked And Not bHistory And AvgCalc > 0 Then
            TextSize = e.Graphics.MeasureString(Convert_MB(CLng(AvgCalc / 1024), My.Settings.ShowBinair), graphTextFont)
            If (x_avgc - TextSize.Width) >= (x_avg - TextSize.Width) AndAlso (x_avgc - TextSize.Width) <= x_avg Then
                '-flip to right
                x_avgc += TextSize.Width
            End If
            e.Graphics.DrawString(Convert_MB(AvgCalc, My.Settings.ShowBinair), graphTextFont, Brushes.Green, x_avgc - TextSize.Width, graphHght - stringSize.Height) ' - BasisSize.Height * 1.7))
        End If
        If mnuShowAvarage.Checked Then 'red
            TextSize = e.Graphics.MeasureString(Convert_MB(iAvgValue, My.Settings.ShowBinair), graphTextFont)
            If (x_avg - TextSize.Width) >= (x_avgc - TextSize.Width) AndAlso (x_avg - TextSize.Width) <= x_avgc Then
                '-flip to right
                x_avg += TextSize.Width
            End If
            e.Graphics.DrawString(Convert_MB(iAvgValue, My.Settings.ShowBinair), graphTextFont, Brushes.DarkRed, x_avg - TextSize.Width, graphHght - stringSize.Height) ' - BasisSize.Height * 1.7))
        End If
        iAvgValue = Nothing
        iAvgCount = Nothing

        '-draw usage text in left bottom corner
        Dim sTmp As String = Convert_MB(pData.Usage_MBUsed, My.Settings.ShowBinair) & "/" & Convert_MB(pData.Usage_MBLimit, My.Settings.ShowBinair)
        TextSize = e.Graphics.MeasureString(sTmp, graphTextFont)
        rect = New Rectangle(0, graphHght, CInt(TextSize.Width + 2), CInt(TextSize.Height))
        e.Graphics.FillRectangle(New SolidBrush(frmSettings.mnuUsage.BackColor), rect)
        e.Graphics.DrawString(sTmp, graphTextFont, Brushes.Black, 0, graphHght)
        TextSize = Nothing

        '-Draw horizontal lines + usagebars + text
        If mnuShowAvgCalc.Checked Or mnuShowAvarage.Checked Then graphHght = CInt(graphHght - stringSize.Height) '-move everything 1 text height up
        Dim chunk As Single = CSng((graphHght) / (horizLines + 1)) '-calculate the chunk height
        Dim iExtra As Long, iWiFree As Long, BasicValue As Long
        For i As Single = StartHght To graphHght Step chunk
            BasicStart = CInt(stringSize.Width + 11) '-reset x position
            iWiFree = 0
            iExtra = 0
            '-draw day text on left side
            e.Graphics.DrawString(Graph_Data(CurrentMarker).Day, graphTextFont, Brushes.Black, 1, i - stringSize.Height / 3)
            e.Graphics.DrawLine(color_line, stringSize.Width, (i + chunk / 2), stringSize.Width + 10, (i + chunk / 2))
            '-draw the usage bar/day
            If Graph_Data(CurrentMarker).Basic > 0 Or Graph_Data(CurrentMarker).Extra > 0 Then
                BasicValue = Graph_Data(CurrentMarker).Basic + If(bFUP, Graph_Data(CurrentMarker).Extra, 0)
                '-add the other bars
                If (Graph_Data(CurrentMarker).Extra > 0) And (mnuShowExtra.Checked Or bFUP) Then
                    '-draw extra bar
                    rect = New Rectangle(BasicStart, CInt(i - (stringSize.Height / 3)), CInt((Graph_Data(CurrentMarker).Extra / FillProcent)), CInt(stringSize.Height))
                    e.Graphics.FillRectangle(color_extra, rect)
                    iExtra = Graph_Data(CurrentMarker).Extra
                    BasicValue -= Graph_Data(CurrentMarker).Extra
                    BasicStart = CInt(BasicStart + (Graph_Data(CurrentMarker).Extra / FillProcent))
                End If
                If (Graph_Data(CurrentMarker).WiFree > 0) And mnuShowWifi.Checked And Not bFUP Then
                    '-draw wi-free bar
                    rect = New Rectangle(BasicStart, CInt(i - (stringSize.Height / 3)), CInt((Graph_Data(CurrentMarker).WiFree / FillProcent)), CInt(stringSize.Height))
                    e.Graphics.FillRectangle(color_wifi, rect)
                    iWiFree = Graph_Data(CurrentMarker).WiFree
                    BasicValue -= Graph_Data(CurrentMarker).WiFree
                    BasicStart = CInt(BasicStart + (Graph_Data(CurrentMarker).WiFree / FillProcent))
                End If
                rect = New Rectangle(BasicStart, CInt(i - (stringSize.Height / 3)), CInt((BasicValue / FillProcent)), CInt(stringSize.Height))
                e.Graphics.FillRectangle(color_basis, rect)

                '-set to normal value
                If iExtra > 0 Or iWiFree > 0 Then BasicValue += iWiFree + iExtra
                If bFUP Then BasicValue -= Graph_Data(CurrentMarker).Basic

                '-draw purple or when history red bar
                If mnuShowLimitDay.Checked Then
                    strSizeUsage = e.Graphics.MeasureString(Convert_MB(CLng(Graph_Data(CurrentMarker).Limit), My.Settings.ShowBinair), graphTextFont)
                    If BasicValue > Graph_Data(CurrentMarker).Limit Then
                        BasicStart = CInt(stringSize.Width + 11 + Graph_Data(CurrentMarker).Limit / FillProcent)
                        rect = New Rectangle(BasicStart, CInt(i - (strSizeUsage.Height / 3)), CInt(If(bFUP, strSizeUsage.Width + (Graph_Data(CurrentMarker).Extra / FillProcent - BasicStart), ((BasicValue - Graph_Data(CurrentMarker).Limit) / FillProcent) - 1)), CInt(strSizeUsage.Height))
                        e.Graphics.FillRectangle(If(bHistory, Brushes.Red, Brushes.Violet), rect)
                    End If
                End If

                '-draw red bar
                If mnuShowLimit.Checked And BasicValue > AvgCalc And Not bHistory Then
                    BasicStart = CInt(stringSize.Width + 11 + AvgCalc / FillProcent)
                    rect = New Rectangle(BasicStart, CInt(i - (stringSize.Height / 3)), CInt(((BasicValue - AvgCalc) / FillProcent) - 1), CInt(stringSize.Height))
                    e.Graphics.FillRectangle(Brushes.Red, rect)
                End If

                '-show day limit text
                If mnuShowLimitDay.Checked Then
                    If BasicValue > Graph_Data(CurrentMarker).Limit Then
                        '-text start
                        BasicStart = CInt(Graph_Data(CurrentMarker).Limit / FillProcent + 5)
                    Else
                        BasicStart = CInt(graphWdth - strSizeUsage.Width) '-10 changed so its equaly lined out
                    End If
                    If Graph_Data(CurrentMarker).Day <> Date.Today.ToString("dd/MM") Then e.Graphics.DrawString(Convert_MB(CLng(Graph_Data(CurrentMarker).Limit), My.Settings.ShowBinair), graphTextFont, Brushes.Green, BasicStart, i - strSizeUsage.Height / 3)
                End If

                '-draw border
                rect = New Rectangle(CInt(stringSize.Width + 11), CInt(i - (stringSize.Height / 3)), CInt((If(bFUP, Graph_Data(CurrentMarker).Extra, 0) + Graph_Data(CurrentMarker).Basic) / FillProcent), CInt(stringSize.Height))
                ControlPaint.DrawBorder(e.Graphics, rect, Color.Gray, ButtonBorderStyle.Solid)
                rect = Nothing
                '-draw usage text next of the bar
                sTmp = Convert_MB(Graph_Data(CurrentMarker).Basic + If(bFUP, Graph_Data(CurrentMarker).Extra, 0), My.Settings.ShowBinair)
                If iExtra > 0 Or iWiFree > 0 Then
                    sTmp &= " ("
                    If iWiFree > 0 Then sTmp &= "Wi-free: " & Convert_MB(CLng(Graph_Data(CurrentMarker).WiFree), My.Settings.ShowBinair)
                    If iExtra > 0 Then
                        If bFUP Then
                            sTmp &= GraphText(6).Replace("%1", Convert_MB(CLng(Graph_Data(CurrentMarker).Extra), My.Settings.ShowBinair)).Replace("%2", Convert_MB(Graph_Data(CurrentMarker).Basic))
                        Else
                            sTmp &= "Extra: " & Convert_MB(CLng(Graph_Data(CurrentMarker).Extra), My.Settings.ShowBinair)
                        End If
                    End If
                    sTmp &= ")"
                End If

                e.Graphics.DrawString(sTmp, graphTextFont, Brushes.Black, stringSize.Width + 11 + ((If(bFUP, Graph_Data(CurrentMarker).Extra, 0) + Graph_Data(CurrentMarker).Basic) / FillProcent), i - stringSize.Height / 3)

                ' If bDEBUG Then
                'End If
            End If
            '-set value for Next CurrentMarker
            CurrentMarker += 1
            If CurrentMarker > horizLines Then Exit For
        Next i

        '-draw values outline w/AntiAlias
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        '-clean up
        AvgCalc = Nothing
        stringSize = Nothing
        strSizeUsage = Nothing
        FillProcent = Nothing
        horizLines = Nothing
        chunk = Nothing
        CurrentMarker = Nothing
        rect = Nothing
        graphWdth = Nothing
        graphHght = Nothing
        BasicStart = Nothing
        sTmp = Nothing
        GC.Collect()
    End Sub

    Private Sub FrmGraph_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        If Me.WindowState = FormWindowState.Minimized Then Exit Sub
        PictureBox1.Width = ClientSize.Width
        PictureBox1.Height = ClientSize.Height
        PictureBox1.Refresh()
    End Sub

    Private Sub mnuShowExtra_Click(sender As Object, e As EventArgs) Handles mnuShowExtra.Click
        My.Settings.ShowExtra = mnuShowExtra.Checked
        PictureBox1.Refresh()
    End Sub

    Private Sub mnuShowPay_Click(sender As Object, e As EventArgs) Handles mnuShowPay.Click
        My.Settings.ShowPay = mnuShowPay.Checked
        PictureBox1.Refresh()
    End Sub

    Private Sub mnuShowWifi_Click(sender As Object, e As EventArgs) Handles mnuShowWifi.Click
        My.Settings.ShowWifi = mnuShowWifi.Checked
        PictureBox1.Refresh()
    End Sub

    Private Sub mnuShowAvarage_Click(sender As Object, e As EventArgs) Handles mnuShowAvarage.Click
        My.Settings.ShowAvg = mnuShowAvarage.Checked
        PictureBox1.Refresh()
    End Sub

    Private Sub mnuShowAvgCalc_Click(sender As Object, e As EventArgs) Handles mnuShowAvgCalc.Click
        My.Settings.ShowAvgCalc = mnuShowAvgCalc.Checked
        PictureBox1.Refresh()
    End Sub

    Private Sub mnuShowLimit_Click(sender As Object, e As EventArgs) Handles mnuShowLimit.Click
        My.Settings.ShowLimit = mnuShowLimit.Checked
        PictureBox1.Refresh()
    End Sub

    Private Sub mnuShowLimitDay_Click(sender As Object, e As EventArgs) Handles mnuShowLimitDay.Click
        My.Settings.ShowLimitDay = mnuShowLimitDay.Checked
        PictureBox1.Refresh()
    End Sub

    Private Sub mnuPeriodThis_Click(sender As Object, e As EventArgs) Handles mnuPeriodThis.Click
        Try
            LastPeriodMenu.Checked = False
        Catch x As Exception
        End Try
        LastPeriodMenu = TryCast(sender, ToolStripMenuItem)

        If LastPeriodMenu.Name.IndexOf("This") > -1 Then
            Text = sFormCaption
            DataSwitch(-1)
            mnuPeriodThis.Checked = True
        Else
            Text = sFormCaption & " :: " & LastPeriodMenu.Text
            'Dim sFile As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "data\" & frmSettings.txtUserId.Text & "_" & LastPeriodMenu.Text.Replace(" - ", "-") & ".txt")
            'If Not File.Exists(sFile) Then
            '-remove from menu
            'Else
            DataSwitch(CInt(LastPeriodMenu.Tag))
            'End If
            LastPeriodMenu.Checked = True
        End If
    End Sub

End Class