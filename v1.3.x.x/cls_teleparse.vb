Imports System.Text

Public Class TeleParse
    Private _InputData As String = ""
    Private _CustomerNr As Int32 = 0
    Private _CustomerAccountNr As Int32 = 0
    Private _dateRangeId As String = ""
    Private _TeleDate As Date = CDate("01/01/1970")
    Private _TeleReset As Date = CDate("01/01/1970")

    '-graph data
    Public Graph_Day() As String
    Public Graph_Extra() As Int32
    Public Graph_Basic() As Int32
    Public Graph_Pay() As Int32
    Public Graph_WiFree() As Int32
    Public Graph_Limit() As Int32

    Private month_counter As Int16 = 0
    Private basic_volume_counter As Int16 = 0
    Private extra_volume_counter As Int16 = 0
    Private payg_volume_counter As Int16 = 0
    Private wifree_volume_counter As Int16 = 0

    Private basic_volume_max As Int32 = 0
    Private extra_volume_max As Int32 = 0
    Private payg_volume_max As Int32 = 0
    Private wifree_volume_max As Int32 = 0

    Private _Usage_DayMax As Int32 = 0
    Private _Usage_Percentage As Int16 = -1
    Private _Usage_MBLimit As Int32 = 0
    Private _Usage_MBUsed As Int32 = 0
    Private _Usage_MBDiff As Int32 = 0
    Private _Usage_Today As Int32 = 0
    Private _DateDiff As Int16 = 0

    Private FUP_Status As String = ""

    Private bUsageData As Boolean = False
    Private sTmp As String

    Public ReadOnly Property CustomerNr As Int32
        Get
            Return _CustomerNr
        End Get
    End Property

    Public ReadOnly Property Usage_Percentage_Day As Int32
        Get
            Return (_Usage_Today / ((_Usage_Today + _Usage_MBDiff) / _DateDiff) * 100)
        End Get
    End Property

    Public ReadOnly Property Usage_DateDiff As Int16
        Get
            Return _DateDiff
        End Get
    End Property

    Public ReadOnly Property CustomerBillAccountNr As Int32
        Get
            Return _CustomerAccountNr
        End Get
    End Property

    Public ReadOnly Property TeleDate As Date
        Get
            Return _TeleDate
        End Get
    End Property

    Public ReadOnly Property TeleReset As Date
        Get
            Return _TeleReset
        End Get
    End Property

    Public ReadOnly Property Usage_DayMax As Int32
        Get
            Return _Usage_DayMax
        End Get
    End Property

    Public ReadOnly Property Usage_Percentage As Int16
        Get
            Return _Usage_Percentage
        End Get
    End Property

    Public ReadOnly Property Usage_MBLimit As Int32
        Get
            Return _Usage_MBLimit
        End Get
    End Property

    Public ReadOnly Property Usage_MBUsed As Int32
        Get
            Return _Usage_MBUsed
        End Get
    End Property

    Public ReadOnly Property Usage_Today As Int32
        Get
            Return _Usage_Today
        End Get
    End Property

    Public ReadOnly Property Usage_MBDiff As Int32
        Get
            Return _Usage_MBDiff
        End Get
    End Property

    Public ReadOnly Property dateRangeId As String
        Get
            Return _dateRangeId
        End Get
    End Property

    Public WriteOnly Property InputData As String               'the filename of de log file
        Set(value As String)
            _InputData = value
        End Set
    End Property

    Public Sub Parse(Optional bfup As Boolean = False)
        If _InputData = "" Then
            'frmSettings.Add2Log("TeleParse: No input data")
            Exit Sub
        End If
        Me.Reset()
        '-goow on :p
        Dim arrLines As String() = _InputData.Split(vbLf), line As String = "", sSplit As String()
        Dim IndexToday As Int16 = 0, j As Int16

        Dim m As RegularExpressions.Match
        Try
            For I = 0 To arrLines.Length - 1
                If arrLines(I).Length < 5 Then GoTo NextLine
                line = arrLines(I)

                If (line.IndexOf("Mijn klantennummer:") > 0) And (_CustomerNr = 0) Then
                    m = RegularExpressions.Regex.Match(line, "breuertextbold"">([0-9]+)</span>")
                    If m.Success Then Me._CustomerNr = CInt(m.Groups(1).Value)
                End If

                If (line.IndexOf("index:0,label:") > 0) And (_dateRangeId = "") Then
                    m = RegularExpressions.Regex.Match(line, "\'(.*?)\'")
                    If m.Success Then _dateRangeId = m.Groups(1).Value
                ElseIf (line.IndexOf("Laatst berekend op") > 0) And (_TeleDate = CDate("01/01/1970")) Then
                    sTmp = line.Substring(line.IndexOf("Laatst") + 19, 10)
                    _TeleDate = CDate(sTmp)
                ElseIf (line.IndexOf("komt op 0 op") > 0) And (_TeleReset = CDate("01/01/1970")) Then
                    m = RegularExpressions.Regex.Match(line, "([0-9]{2})\/([0-9]{2})\/([0-9]{4})")
                    If m.Success Then
                        Me._TeleReset = CDate(Int(m.Groups(1).Value) & "/" & Int(m.Groups(2).Value) & "/" & Int(m.Groups(3).Value))
                    End If
                End If

                '- bars for everybody
                If line.IndexOf("days") > 0 And Not bUsageData Then
                    line = line.Replace("'", "")
                    bUsageData = True
                    m = RegularExpressions.Regex.Match(line, "days\:\[(.*?)\]")
                    If m.Success Then
                        sSplit = m.Groups(1).ToString.Split(",")
                        For j = 0 To sSplit.Length - 1
                            ReDim Preserve Graph_Day(j)
                            Graph_Day(j) = sSplit(j)
                            If Graph_Day(j) = Date.Today Then
                                IndexToday = j
                            End If
                        Next j
                    End If

                    m = RegularExpressions.Regex.Match(line, IIf(bfup, "detailedOffPeakUsage", "detailedUsage") & "\:\[(.*?)\]")
                    If m.Success Then
                        sSplit = m.Groups(1).ToString.Split(",")
                        For j = 0 To sSplit.Length - 1
                            ReDim Preserve Graph_Basic(j)
                            Graph_Basic(j) = CInt(sSplit(j).Replace(".", ",") * IIf(bfup, 1024, 1))
                            If basic_volume_max < Graph_Basic(j) Then
                                basic_volume_max = Graph_Basic(j)
                            End If
                            _Usage_MBUsed += Graph_Basic(j) 'cInt(sSplit(j).Replace(".", ",") * IIf(bfup, 1024, 1))
                            If j = IndexToday Then _Usage_Today = Graph_Basic(j)
                        Next j
                    End If

                    m = RegularExpressions.Regex.Match(line, IIf(bfup, "detailedPeakUsage", "detailedExtraUsage") & "\:\[(.*?)\]")
                    If m.Success Then
                        sSplit = m.Groups(1).ToString.Split(",")
                        For j = 0 To sSplit.Length - 1
                            ReDim Preserve Graph_Extra(j)
                            Graph_Extra(j) = CInt(sSplit(j).Replace(".", ",") * IIf(bfup, 1024, 1))
                            If extra_volume_max < Graph_Extra(j) Then
                                extra_volume_max = Graph_Extra(j)
                            End If
                            If j = IndexToday Then _Usage_Today += Graph_Extra(j)
                        Next j
                    End If

                    If Not bfup Then
                        m = RegularExpressions.Regex.Match(line, "detailedWiFreeUsage\:\[(.*?)\]")
                        If m.Success Then
                            sSplit = m.Groups(1).ToString.Split(",")
                            For j = 0 To sSplit.Length - 1
                                ReDim Preserve Graph_WiFree(j)
                                Graph_WiFree(j) = CInt(sSplit(j).Replace(".", ","))
                                If wifree_volume_max < Graph_WiFree(j) Then
                                    wifree_volume_max = Graph_WiFree(j)
                                End If
                                If j = IndexToday Then _Usage_Today += Graph_WiFree(j)
                            Next j
                        End If

                        m = RegularExpressions.Regex.Match(line, "totalUsagePercentage:(.*?)\,")
                        If m.Success Then
                            _Usage_Percentage = CInt(m.Groups(1).Value.Replace(".", ","))
                        End If

                        m = RegularExpressions.Regex.Match(line, "totalMax:(.*?)\,")
                        If m.Success Then
                            _Usage_MBLimit = CInt(m.Groups(1).Value.Replace(".", ",")) * 1024 'GB 2 MB
                        End If

                    Else
                        '-bfup
                        'peakUsage':25.91455,'offPeakUsage':24.789501,'squeeze':500
                        m = RegularExpressions.Regex.Match(line, "peakUsage:(.*?)\,")
                        If m.Success Then
                            _Usage_MBUsed += CInt(m.Groups(1).Value.Replace(".", ",") * 1024) 'GB 2 MB
                        End If

                        '    m = RegularExpressions.Regex.Match(line, "offPeakUsage:(.*?)\,")
                        '   If m.Success Then
                        '_Usage_MBUsed += cint(m.Groups(1).Value * 1024) 'GB 2 MB
                        'End If

                        m = RegularExpressions.Regex.Match(line, "squeeze:(.*?)\,")
                        If m.Success Then
                            _Usage_MBLimit = CInt(m.Groups(1).Value.Replace(".", ",") * 1024)  'GB 2 MB
                        End If
                        '-this has changed so not workie workie anymore
                        If line.IndexOf("fupUsageNormal") > 0 Then '-get status
                            m = RegularExpressions.Regex.Match(line, ">(.*)<\/span>")
                            If m.Success Then
                                FUP_Status = m.Groups(1).Value
                            End If
                        End If
                    End If
                End If

                If line.IndexOf("saldoPostitLink") > 0 Then '-when you need to pay a bill
                    line = arrLines(I + 1)
                    m = RegularExpressions.Regex.Match(line, "accountNumber=([0-9]+)""")
                    If m.Success Then
                        Me._CustomerAccountNr = CInt(m.Groups(1).Value)
                    End If
                End If
NextLine:
            Next I
            If bfup Then
                ''peakUsagePercentage':17,'offPeakUsagePercentage':17
                _Usage_Percentage = _Usage_MBUsed / _Usage_MBLimit * 100
                ' If FUP_Status <> "Vrij verbruik" Then 'needs to change
                '_Usage_Percentage = 200
                'End If
            End If

            _Usage_DayMax = basic_volume_max + extra_volume_max + payg_volume_max + wifree_volume_max
            If _Usage_MBLimit > 0 Then
                _Usage_MBDiff = _Usage_MBLimit - _Usage_MBUsed
            Else
                _Usage_MBLimit = _Usage_MBUsed
            End If

            '-day limits
            Dim iUsage As Int32 = 0
            For j = 0 To Graph_Day.Length - 1
                ReDim Preserve Graph_Limit(j)
                Graph_Limit(j) = _Usage_MBLimit
                If j > 0 Then
                    iUsage += Graph_Basic(j - 1) + Graph_Extra(j - 1)
                    Graph_Limit(j) -= iUsage
                End If
                Graph_Limit(j) /= Graph_Day.Length - j
            Next j

            If TeleReset <> #1/1/1970# Then
                _DateDiff = CInt(DateDiff(DateInterval.Day, Date.Now, _TeleReset) + 1)
            Else
                _DateDiff = -1
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "TeleParse.Parse()")
        End Try
    End Sub

    Private Sub Reset()
        '-reset values
        _CustomerNr = 0
        _CustomerAccountNr = 0
        _dateRangeId = ""
        _TeleDate = CDate("01/01/1970")
        _TeleReset = CDate("01/01/1970")
        ReDim Graph_Day(0)
        ReDim Graph_Extra(0)
        ReDim Graph_Basic(0)
        ReDim Graph_Pay(0)
        ReDim Graph_WiFree(0)
        month_counter = 0
        basic_volume_counter = 0
        extra_volume_counter = 0
        payg_volume_counter = 0
        wifree_volume_counter = 0
        basic_volume_max = 0
        extra_volume_max = 0
        payg_volume_max = 0
        wifree_volume_max = 0
        _Usage_DayMax = 0
        _Usage_Percentage = -1
        _Usage_MBLimit = 0
        _Usage_MBUsed = 0
        _Usage_MBDiff = 0
        bUsageData = False
    End Sub
End Class
