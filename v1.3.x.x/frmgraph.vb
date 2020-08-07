Imports System.IO
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class frmGraph
    Private graphValueIndex As Int32 = 0, graphWdth As Int32 = 0, graphHght As Int32 = 0, graphMaxValue As Int32 = 0
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
    Private gGraph_Day(0) As String
    Private gGraph_Extra(0) As Int32, gGraph_Basic(0) As Int32, gGraph_Pay(0) As Int32, gGraph_WiFree(0) As Int32, gGraph_Limit(0) As Int32
    Private gUsage_DayMax As Int32 = 0

    'Private gBfup As Boolean = False
    Private bHistory As Boolean = False

    Public Sub DataSwitch(ByVal sFile As String)
        If Not File.Exists(sFile) Then
            '-check if they are all set
            Try
                If pData.Graph_Basic.Length <> gGraph_Day.Length Then ReDim gGraph_Day(pData.Graph_Basic.Length - 1)
                If pData.Graph_Basic.Length <> gGraph_Basic.Length Then ReDim gGraph_Basic(pData.Graph_Basic.Length - 1)
                If pData.Graph_Basic.Length <> gGraph_Pay.Length Then ReDim gGraph_Pay(pData.Graph_Basic.Length - 1)
                If pData.Graph_Basic.Length <> gGraph_WiFree.Length Then ReDim gGraph_WiFree(pData.Graph_Basic.Length - 1)
                If pData.Graph_Basic.Length <> gGraph_Extra.Length Then ReDim gGraph_Extra(pData.Graph_Basic.Length - 1)
                If pData.Graph_Basic.Length <> gGraph_Limit.Length Then ReDim gGraph_Limit(pData.Graph_Basic.Length - 1)

                Array.Copy(pData.Graph_Basic, gGraph_Basic, pData.Graph_Basic.Length)
                Array.Copy(pData.Graph_Extra, gGraph_Extra, pData.Graph_Basic.Length)
                Array.Copy(pData.Graph_Day, gGraph_Day, pData.Graph_Basic.Length)
                Array.Copy(pData.Graph_Pay, gGraph_Pay, pData.Graph_Basic.Length)
                Array.Copy(pData.Graph_WiFree, gGraph_WiFree, pData.Graph_Basic.Length)
                Array.Copy(pData.Graph_Limit, gGraph_Limit, pData.Graph_Basic.Length)
                ' gbFup = bfup
                gUsage_DayMax = pData.Usage_DayMax
                bHistory = False
            Catch ex As Exception
                Add2Log("GRAPH::DATASWITCH_NOFILE_ERR: " & ex.Message)
            End Try
        Else
            ReDim gGraph_Basic(1)
            ReDim gGraph_Day(1)
            ReDim gGraph_Extra(1)
            ReDim gGraph_Limit(1)
            ReDim gGraph_Pay(1)
            ReDim gGraph_WiFree(1)

            '-open file 
            'userid;periode start; periode end;date;data basic;extra;wifree;limit
            'ex: a123456;18032015;17042015;18/03;2642;0;0;0
            Try
                Dim iCount As Int16 = 0
                Dim basic_volume_max As Int32 = 0, extra_volume_max As Int32 = 0, payg_volume_max As Int32 = 0, wifree_volume_max As Int32 = 0
                Using objReader As New FileIO.TextFieldParser(sFile)
                    objReader.TextFieldType = FileIO.FieldType.Delimited
                    objReader.SetDelimiters(";")

                    Dim sStream As String()
                    Do
                        sStream = objReader.ReadFields
                        '-old data check
                        If sStream(0).IndexOf("|") > -1 Then
                            Add2Log("TeleGrafiek::DataSwitch: Found OLD data, converting it")
                            Add2Log("FILE: " & sFile, False)
                            Dim sLine As String, sFileOut(0) As String, sChk() As String

                            '-first line check
                            sFileOut(0) = sStream(0).Replace("-", ";").Replace("|", ";")
                            '-rest of the lines
                            Do
                                ReDim Preserve sFileOut(sFileOut.Length)
                                sFileOut(sFileOut.Length - 1) = objReader.ReadLine.Replace("-", ";").Replace("|", ";")
                            Loop Until objReader.EndOfData
                            objReader.Close()
                            objReader.Dispose()

                            '-do the day limit
                            Dim iLimit As Int32 = pData.Usage_MBLimit
                            For i = 0 To sFileOut.Length - 1
                                sChk = sFileOut(i).Split(New Char() {";"c})
                                For j = 1 To 8 - sChk.Length
                                    sFileOut(i) &= ";" & CInt(iLimit / (sFileOut.Length - i))
                                Next j
                                iLimit -= (CInt(sChk(4)) + CInt(sChk(5))) 'iUsage
                            Next i

                            '-save it to the file
                            Try
                                Using objWriter As New StreamWriter(sFile, False)
                                    For i = 0 To sFileOut.Length - 1
                                        objWriter.WriteLine(sFileOut(i))
                                    Next i
                                End Using
                                Add2Log("Saved succesfully", False)
                            Catch ex As Exception
                                Add2Log("TeleGrafiek::DataSwitch::ERR: " & ex.Message, False)
                            End Try
                            sLine = Nothing
                            sChk = Nothing
                            sFileOut = Nothing
                            DataSwitch(sFile)
                            Exit Sub
                        End If

                        If iCount > gGraph_Basic.Length - 1 Then
                            ReDim Preserve gGraph_Day(iCount)
                            ReDim Preserve gGraph_Basic(iCount)
                            ReDim Preserve gGraph_Pay(iCount)
                            ReDim Preserve gGraph_WiFree(iCount)
                            ReDim Preserve gGraph_Extra(iCount)
                            ReDim Preserve gGraph_Limit(iCount)
                        End If
                        If sStream(0) = frmSettings.txtUserId.Text Then
                            gGraph_Day(iCount) = sStream(3)
                            gGraph_Basic(iCount) = CInt(sStream(4))
                            gGraph_Extra(iCount) = CInt(sStream(5))
                            gGraph_Pay(iCount) = 0
                            gGraph_WiFree(iCount) = CInt(sStream(6))
                            If CInt(sStream(7)) <= 0 Then
                                '-recalculate limits
                                objReader.Close()
                                objReader.Dispose()

                                Add2Log("TeleGrafiek::DataSwitch: day limit is wrong")
                                Dim sLine As String, sFileOut(0) As String, sChk() As String
                                Using objReader2 As New FileIO.TextFieldParser(sFile)
                                    Do
                                        sFileOut(sFileOut.Length - 1) = objReader2.ReadLine
                                        ReDim Preserve sFileOut(sFileOut.Length)
                                    Loop Until objReader2.EndOfData
                                    ReDim Preserve sFileOut(sFileOut.Length - 2)
                                    objReader2.Close()
                                    objReader2.Dispose()
                                End Using

                                '-do the day limit
                                Add2Log("Recalculating limit", False)
                                Dim iLimit As Int32 = pData.Usage_MBLimit
                                For i = 0 To sFileOut.Length - 1
                                    If sFileOut(i) IsNot Nothing Then
                                        sChk = sFileOut(i).Split(New Char() {";"c})
                                        sChk(7) = CInt(iLimit / (sFileOut.Length - i)).ToString
                                        iLimit -= (CInt(sChk(4)) + CInt(sChk(5)))
                                        sFileOut(i) = sChk(0) & ";" & sChk(1) & ";" & sChk(2) & ";" & sChk(3) & ";" & sChk(4) & ";" & sChk(5) & ";" & sChk(6) & ";" & sChk(7)
                                    End If
                                Next i

                                '-save it to the file
                                Try
                                    Using objWriter As New StreamWriter(sFile, False)
                                        For i = 0 To sFileOut.Length - 1
                                            objWriter.WriteLine(sFileOut(i))
                                        Next i
                                    End Using
                                    Add2Log("Saved succesfully", False)
                                Catch ex As Exception
                                    Add2Log("ERR: " & ex.Message, False)
                                End Try
                                sLine = Nothing
                                sChk = Nothing
                                sFileOut = Nothing
                                DataSwitch(sFile)
                                Exit Sub
                            End If

                            gGraph_Limit(iCount) = CInt(sStream(7))
                            If basic_volume_max < gGraph_Basic(iCount) Then basic_volume_max = gGraph_Basic(iCount)
                            If extra_volume_max < gGraph_Extra(iCount) Then extra_volume_max = gGraph_Extra(iCount)
                            If payg_volume_max < gGraph_Pay(iCount) Then payg_volume_max = gGraph_Pay(iCount)
                            If wifree_volume_max < gGraph_WiFree(iCount) Then wifree_volume_max = gGraph_WiFree(iCount)
                        Else
                            gGraph_Day(iCount) = "Error"
                            gGraph_Basic(iCount) = 0
                            gGraph_Extra(iCount) = 0
                            gGraph_Pay(iCount) = 0
                            gGraph_WiFree(iCount) = 0
                            gGraph_Limit(iCount) = 0
                        End If
                        iCount += 1
                    Loop Until objReader.EndOfData
                End Using
                iCount -= 1
                If iCount < (gGraph_Basic.Length - 1) Then
                    ReDim Preserve gGraph_Day(iCount)
                    ReDim Preserve gGraph_Basic(iCount)
                    ReDim Preserve gGraph_Pay(iCount)
                    ReDim Preserve gGraph_WiFree(iCount)
                    ReDim Preserve gGraph_Extra(iCount)
                    ReDim Preserve gGraph_Limit(iCount)
                End If
                gUsage_DayMax = basic_volume_max + extra_volume_max + payg_volume_max + wifree_volume_max
                bHistory = True
            Catch Ex As Exception
                MsgBox(Ex.Message, , "Error: LoadData")
                Add2Log("GRAPH::DATASWITCH_FILE_ERR: " & Ex.Message)
            End Try
        End If
        PictureBox1.Refresh()
    End Sub

    Private Sub frmGraph_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Me.Visible = False
        Dim x(4) As String
        x(0) = Me.Location.X.ToString
        x(1) = Me.Location.Y.ToString
        x(2) = Me.Width.ToString
        x(3) = Me.Height.ToString
        x(4) = CInt(Me.WindowState).ToString
        My.Settings.LocationGraph = String.Join(",", x)
        My.Settings.Save()
        '-clear data from memory
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
        GC.Collect()
    End Sub

    Private Sub frmGraph_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If IsNothing(pData.Graph_Day) Then
            MsgBox("No data available")
            Me.Close()
        End If

        '-check if they are all set
        If pData.Graph_Pay.Length < 2 Then ReDim pData.Graph_Pay(pData.Graph_Basic.Length)
        If pData.Graph_WiFree.Length < 2 Then ReDim pData.Graph_WiFree(pData.Graph_Basic.Length)
        If pData.Graph_Extra.Length < 2 Then ReDim pData.Graph_Extra(pData.Graph_Basic.Length)

        '-do the dataswitch
        LastPeriodMenu = TryCast(mnuPeriodThis, ToolStripMenuItem)
        DataSwitch("")

        '-load values
        mnuShowStandard.Text = GraphText(0)
        If bfup Then
            mnuShowStandard.Text = GraphText(4)
            mnuShowExtra.Text = GraphText(5)
            mnuShowExtra.Enabled = False
            mnuShowPay.Enabled = False
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

        '-load history usage file list
        Try
            Dim dir As DirectoryInfo = New DirectoryInfo(Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "data\"))
            Dim files As FileInfo() = dir.GetFiles("*.txt")
            Array.Sort(files, New clsCompareFileInfoName)
            For Each fl As FileInfo In files
                If fl.Name.IndexOf(frmSettings.txtUserId.Text) > -1 Then
                    Dim mnuItem As New ToolStripMenuItem() With {.Name = "mnuPeriodx", .Text = fl.Name.Replace(".txt", "").Replace("-", " - ").Replace(frmSettings.txtUserId.Text & "_", ""), .Visible = True, .Checked = False}
                    mnuPeriod.DropDownItems.Add(mnuItem)
                    AddHandler mnuItem.Click, AddressOf mnuPeriodThis_Click
                    ToolStripMenuItem3.Visible = True
                End If
            Next
        Catch ex As Exception
            Add2Log("GRAPH::LOAD_ERR: " & ex.Message)
        End Try
        '-set picturebox
        PictureBox1.BorderStyle = BorderStyle.None
        PictureBox1.Top = 0
        PictureBox1.Left = 0

        '-window
        Dim il As Point = New Point(0, 0)
        Dim sz As Size = Size

        Try
            Dim initLocation As String = My.Settings.LocationGraph
            If Not String.IsNullOrEmpty(initLocation) Then
                Dim parts As String() = initLocation.Split(CChar(","))
                If parts.Length >= 2 Then il = New Point(CInt(parts(0)), CInt(parts(1)))
                If parts.Length >= 4 Then sz = New Size(CInt(parts(2)), CInt(parts(3)))
                If parts.Length = 5 Then Me.WindowState = CType(parts(4), FormWindowState) Else Me.WindowState = FormWindowState.Normal
            End If
        Catch ex As Exception
            Add2Log("GRAPH::LOAD_ERR: " & ex.Message)
        Finally
            Size = sz
            Location = il
        End Try
        bShowGraph = True
    End Sub

    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint
        '-set Graph draw area dimensions
        graphWdth = PictureBox1.ClientRectangle.Width
        graphHght = PictureBox1.ClientRectangle.Height
        Application.DoEvents()

        '-show bgimage
        Dim imgWidth As Int16 = CShort(My.Resources.Telenet_wiki_transparent.Width), imgHeight As Int16 = CShort(My.Resources.Telenet_wiki_transparent.Height)
        If (graphWdth > imgWidth) And (graphHght > imgHeight) Then
            e.Graphics.DrawImage(My.Resources.Telenet_wiki_transparent, (graphWdth - imgWidth) \ 2, (graphHght - imgHeight) \ 2, imgWidth, imgHeight)
        Else
            e.Graphics.DrawImage(My.Resources.Telenet_wiki_transparent, graphWdth \ 4, graphHght \ 4, graphWdth \ 2, graphHght \ 2)
        End If

        '-set vars
        graphMaxValue = UBound(gGraph_Day)
        Dim stringSize As SizeF = e.Graphics.MeasureString("88/888", graphTextFont)
        Dim strSizeUsage As SizeF = e.Graphics.MeasureString("99.99 GB", graphTextFont)
        Dim ScaleBlocks As Single = CSng(gUsage_DayMax / 5) 'block size
        Dim TwoTFiveHBlocks As Single = (gUsage_DayMax / ScaleBlocks) + 1
        Dim FillProcent As Single = (gUsage_DayMax + ScaleBlocks) / (graphWdth - stringSize.Width - 11)
        Dim horizLines As Integer = graphMaxValue
        Const StartHght As Int16 = 7
        Dim CurrentMarker As Single = 0
        Dim rect As Rectangle
        Dim TextSize As SizeF = Nothing
        Dim BasicStart As Integer = CInt(stringSize.Width + 11) '-x position

        '-draw vertical lines 
        Dim GraphWdthStep As Single = (graphWdth - stringSize.Width + 10) / TwoTFiveHBlocks
        graphHght -= CInt(stringSize.Height)

        Dim j As Int32 = 0
        For i = stringSize.Width + 10 To graphWdth Step GraphWdthStep
            e.Graphics.DrawLine(color_line, i, 0, i, graphHght - IIf(j = 0 And mnuShowAvgCalc.Checked And mnuShowAvarage.Checked, stringSize.Height, -stringSize.Height))
            If CInt(j) > 0 Then
                TextSize = e.Graphics.MeasureString(Convert_MB(CLng(j), My.Settings.ShowBinair), graphTextFont)
                e.Graphics.DrawString(Convert_MB(CLng(j), My.Settings.ShowBinair), graphTextFont, color_text_legend, i, graphHght)
            End If
            j += CInt(ScaleBlocks + FillProcent)
        Next i
        j = Nothing

        Dim AvgCalc As Int64 = CLng((pData.Usage_MBDiff + pData.Usage_Today) / CInt(DateDiff(DateInterval.Day, Date.Now, pData.TeleReset) + 1))
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
        Dim iAvgValue As Int64 = 0, iAvgCount As Int16 = 0
        If mnuShowAvarage.Checked Then
            x_avg = BasicStart
            For i = 0 To gGraph_Basic.Length - 1
                If gGraph_Basic(i) > 0 Then
                    iAvgValue += gGraph_Basic(i) + IIf(bFUP, gGraph_Extra(i), 0)
                    iAvgCount += 1
                End If
            Next i
            If iAvgCount > 0 Then
                iAvgValue /= iAvgCount
                x_avg += Convert.ToSingle(iAvgValue / FillProcent)
            End If
            If x_avg > graphWdth Then x_avg = graphWdth - 5 '-if went of scale trow it back
            e.Graphics.DrawLine(Pens.DarkRed, x_avg, StartHght, x_avg, graphHght)
        End If

        '-green line
        If mnuShowAvgCalc.Checked And Not bHistory And AvgCalc > 0 Then
            TextSize = e.Graphics.MeasureString(Convert_MB(AvgCalc / 1024, My.Settings.ShowBinair), graphTextFont)
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

        '-Draw horizontal lines + usagebars + text
        If mnuShowAvgCalc.Checked Or mnuShowAvarage.Checked Then graphHght -= stringSize.Height '-move everything 1 text height up
        Dim chunk As Single = CSng((graphHght) / (horizLines + 1)) '-calculate the chunk height
        Dim iExtra As Int32, iWiFree As Int32, BasicValue As Int32
        For i As Single = StartHght To graphHght Step chunk
            BasicStart = CInt(stringSize.Width + 11) '-reset x position
            iWiFree = 0
            iExtra = 0
            '-draw day text on left side
            e.Graphics.DrawString(gGraph_Day(CurrentMarker), graphTextFont, Brushes.Black, 1, i - stringSize.Height / 3)
            e.Graphics.DrawLine(color_line, stringSize.Width, (i + chunk / 2), stringSize.Width + 10, (i + chunk / 2))
            '-draw the usage bar/day
            If gGraph_Basic(CurrentMarker) > 0 Or gGraph_Extra(CurrentMarker) > 0 Then
                BasicValue = gGraph_Basic(CurrentMarker) + IIf(bFUP, gGraph_Extra(CurrentMarker), 0)
                '-add the other bars
                If (gGraph_Extra(CurrentMarker) > 0) And (mnuShowExtra.Checked Or bFUP) And (UBound(gGraph_Extra) = UBound(gGraph_Basic)) Then
                    '-draw extra bar
                    rect = New Rectangle(BasicStart, i - (stringSize.Height / 3), (gGraph_Extra(CurrentMarker) / FillProcent), stringSize.Height)
                    e.Graphics.FillRectangle(color_extra, rect)
                    iExtra = gGraph_Extra(CurrentMarker)
                    BasicValue -= gGraph_Extra(CurrentMarker)
                    BasicStart += (gGraph_Extra(CurrentMarker) / FillProcent)
                End If
                If (gGraph_WiFree(CurrentMarker) > 0) And mnuShowWifi.Checked And Not bFUP Then
                    '-draw wi-free bar
                    rect = New Rectangle(BasicStart, i - (stringSize.Height / 3), (gGraph_WiFree(CurrentMarker) / FillProcent), stringSize.Height)
                    e.Graphics.FillRectangle(color_wifi, rect)
                    iWiFree = gGraph_WiFree(CurrentMarker)
                    BasicValue -= gGraph_WiFree(CurrentMarker)
                    BasicStart += (gGraph_WiFree(CurrentMarker) / FillProcent)
                End If
                rect = New Rectangle(BasicStart, i - (stringSize.Height / 3), (BasicValue / FillProcent), stringSize.Height)
                e.Graphics.FillRectangle(color_basis, rect)

                '-set to normal value
                If iExtra > 0 Or iWiFree > 0 Then BasicValue += iWiFree + iExtra
                If bFUP Then BasicValue -= gGraph_Basic(CurrentMarker)

                '-draw purple or when history red bar
                If mnuShowLimitDay.Checked Then
                    strSizeUsage = e.Graphics.MeasureString(Convert_MB(CLng(gGraph_Limit(CurrentMarker)), My.Settings.ShowBinair), graphTextFont)
                    If BasicValue > gGraph_Limit(CurrentMarker) Then
                        BasicStart = stringSize.Width + 11 + gGraph_Limit(CurrentMarker) / FillProcent
                        rect = New Rectangle(BasicStart, i - (strSizeUsage.Height / 3), IIf(bFUP, strSizeUsage.Width + (gGraph_Extra(CurrentMarker) / FillProcent - BasicStart), ((BasicValue - gGraph_Limit(CurrentMarker)) / FillProcent) - 1), strSizeUsage.Height)
                        e.Graphics.FillRectangle(IIf(bHistory, Brushes.Red, Brushes.Violet), rect)
                        '-text start
                        BasicStart = gGraph_Limit(CurrentMarker) / FillProcent + 5
                    Else
                        BasicStart = graphWdth - strSizeUsage.Width '-10 changed so its equaly lined out
                    End If
                    '-show day limit text
                    If gGraph_Day(CurrentMarker) <> Date.Today.ToString("dd/MM") Then e.Graphics.DrawString(Convert_MB(CLng(gGraph_Limit(CurrentMarker)), My.Settings.ShowBinair), graphTextFont, Brushes.Green, BasicStart, i - strSizeUsage.Height / 3)
                End If

                '-draw red bar
                If mnuShowLimit.Checked And BasicValue > AvgCalc And Not bHistory Then
                    BasicStart = stringSize.Width + 11 + AvgCalc / FillProcent
                    rect = New Rectangle(BasicStart, i - (stringSize.Height / 3), ((BasicValue - AvgCalc) / FillProcent) - 1, stringSize.Height)
                    e.Graphics.FillRectangle(Brushes.Red, rect)
                End If

                '-draw border
                rect = New Rectangle(stringSize.Width + 11, i - (stringSize.Height / 3), (IIf(bFUP, gGraph_Extra(CurrentMarker), 0) + gGraph_Basic(CurrentMarker)) / FillProcent, stringSize.Height)
                ControlPaint.DrawBorder(e.Graphics, rect, Color.Gray, ButtonBorderStyle.Solid)
                rect = Nothing
                '-draw usage text next of the bar
                sTmp = Convert_MB(gGraph_Basic(CurrentMarker) + IIf(bFUP, gGraph_Extra(CurrentMarker), 0), My.Settings.ShowBinair)
                If iExtra > 0 Or iWiFree > 0 Then
                    sTmp &= " ("
                    If iWiFree > 0 Then sTmp &= "Wi-free: " & Convert_MB(CLng(gGraph_WiFree(CurrentMarker)), My.Settings.ShowBinair)
                    If iExtra > 0 Then
                        If bFUP Then
                            sTmp &= GraphText(6).Replace("%1", Convert_MB(CLng(gGraph_Extra(CurrentMarker)), My.Settings.ShowBinair)).Replace("%2", Convert_MB(gGraph_Basic(CurrentMarker)))
                        Else
                            sTmp &= "Extra: " & Convert_MB(CLng(gGraph_Extra(CurrentMarker)), My.Settings.ShowBinair)
                        End If
                    End If
                    sTmp &= ")"
                End If

                e.Graphics.DrawString(sTmp, graphTextFont, Brushes.Black, stringSize.Width + 11 + ((IIf(bFUP, gGraph_Extra(CurrentMarker), 0) + gGraph_Basic(CurrentMarker)) / FillProcent), i - stringSize.Height / 3)

                ' If bDEBUG Then
                'End If
            End If
            '-set value for Next CurrentMarker
            CurrentMarker = (CurrentMarker + (graphMaxValue / horizLines))
            If CurrentMarker > graphMaxValue Then Exit For
        Next i

        '-draw values outline w/AntiAlias
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        '-clean up
        AvgCalc = Nothing
        stringSize = Nothing
        TwoTFiveHBlocks = Nothing
        FillProcent = Nothing
        horizLines = Nothing
        chunk = Nothing
        CurrentMarker = Nothing
        rect = Nothing
        TextSize = Nothing
        GC.Collect()
    End Sub

    Private Sub frmGraph_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If sFormCaption = "" Then sFormCaption = Me.Text
        If bFUP Then
            mnuShowStandard.Text = GraphText(4)
            mnuShowExtra.Text = GraphText(5)
        End If
    End Sub

    Private Sub frmGraph_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        If Me.WindowState = FormWindowState.Minimized Then Exit Sub
        PictureBox1.Width = Me.ClientSize.Width
        PictureBox1.Height = Me.ClientSize.Height
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
            Me.Text = sFormCaption
            DataSwitch("")
            mnuPeriodThis.Checked = True
        Else
            Me.Text = sFormCaption & " :: " & LastPeriodMenu.Text
            Dim sFile As String = Path.Combine(Directory.GetParent(Application.UserAppDataPath).ToString, "data\" & frmSettings.txtUserId.Text & "_" & LastPeriodMenu.Text.Replace(" - ", "-") & ".txt")
            If Not File.Exists(sFile) Then
                '-remove from menu
            Else
                DataSwitch(sFile)
            End If
            LastPeriodMenu.Checked = True
        End If
    End Sub
End Class

Public Class clsCompareFileInfoName
    Implements IComparer

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim File1 As FileInfo
        Dim File2 As FileInfo
        File1 = DirectCast(x, FileInfo)
        File2 = DirectCast(y, FileInfo)
        Compare = String.Compare(File1.Name, File2.Name)
    End Function
End Class