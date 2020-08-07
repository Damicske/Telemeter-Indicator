'Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
'Imports System.IO

Public Class TeleParse
#Region "declarations"
    Private _InputData As String = ""
    Private _CustomerNr As Integer = 0
    Private _CustomerAccountNr As Integer = 0
    Private _dateRangeId As String = ""
    Private _TeleDate As Date = CDate("01/01/1970")
    Private _TeleReset As Date = CDate("01/01/1970")

    '-graph data
    Public Class GraphData
        Private mDay As String = String.Empty
        Private mBasic As Long = 0
        Private mExtra As Long = 0
        Private mWiFree As Long = 0
        Private mLimit As Long = 0

        Public Property Day As String
            Get
                Return mDay
            End Get
            Set(ByVal value As String)
                mDay = value
            End Set
        End Property
        Public Property Basic As Long
            Get
                Return mBasic
            End Get
            Set(ByVal value As Long)
                mBasic = value
            End Set
        End Property
        Public Property Extra As Long
            Get
                Return mExtra
            End Get
            Set(ByVal value As Long)
                mExtra = value
            End Set
        End Property
        Public Property WiFree As Long
            Get
                Return mWiFree
            End Get
            Set(ByVal value As Long)
                mWiFree = value
            End Set
        End Property
        Public Property Limit As Long
            Get
                Return mLimit
            End Get
            Set(ByVal value As Long)
                mLimit = value
            End Set
        End Property
    End Class
    Private _Data As New List(Of GraphData)

    Private month_counter As Short = 0
    Private basic_volume_counter As Short = 0
    Private extra_volume_counter As Short = 0
    Private payg_volume_counter As Short = 0
    Private wifree_volume_counter As Short = 0

    Private basic_volume_max As Long = 0
    Private extra_volume_max As Long = 0
    Private payg_volume_max As Long = 0
    Private wifree_volume_max As Long = 0

    Private _Usage_DayMax As Long = 0
    Private _Usage_Percentage As Short = -1
    Private _Usage_MBLimit As Long = 0
    Private _Usage_MBUsed As Long = 0
    Private _Usage_MBDiff As Long = 0
    Private _Usage_Today As Long = 0
    Private _DateDiff As Short = 0

    Private _FUP As Boolean = False

    Public ReadOnly Property Data As List(Of GraphData)
        Get
            Return _Data
        End Get
    End Property

    Public ReadOnly Property CustomerNr As Integer
        Get
            Return _CustomerNr
        End Get
    End Property

    Public ReadOnly Property Usage_Percentage_Day As Integer
        Get
            Return CInt((_Usage_Today / ((_Usage_Today + _Usage_MBDiff) / _DateDiff) * 100))
        End Get
    End Property

    Public ReadOnly Property Usage_DateDiff As Short
        Get
            Return _DateDiff
        End Get
    End Property

    Public ReadOnly Property CustomerBillAccountNr As Integer
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

    Public ReadOnly Property Usage_DayMax As Long
        Get
            Return _Usage_DayMax
        End Get
    End Property

    Public ReadOnly Property Usage_Percentage As Short
        Get
            Return _Usage_Percentage
        End Get
    End Property

    Public ReadOnly Property Usage_MBLimit As Long
        Get
            Return _Usage_MBLimit
        End Get
    End Property

    Public ReadOnly Property Usage_MBUsed As Long
        Get
            Return _Usage_MBUsed
        End Get
    End Property

    Public ReadOnly Property Usage_Today As Long
        Get
            Return _Usage_Today
        End Get
    End Property

    Public ReadOnly Property Usage_MBDiff As Long
        Get
            Return _Usage_MBDiff
        End Get
    End Property

    Public ReadOnly Property DateRangeId As String
        Get
            Return _dateRangeId
        End Get
    End Property

    Public WriteOnly Property InputData As String               'the html data
        Set(value As String)
            _InputData = value
        End Set
    End Property

    Public ReadOnly Property FUP As Boolean
        Get
            Return _FUP
        End Get
    End Property


#End Region

    Public Sub Parse(Optional bfup As Boolean = False)
        If _InputData = String.Empty Then Exit Sub
        Reset()

        '-goow on :p
        Try
            Dim json = JsonConvert.DeserializeObject(_InputData)
            _InputData = ""

            'json.item("internetusage")

            '-set FUP
            'If sOutput.IndexOf("Business Fibernet") = -1 And (
            '        sOutput.IndexOf("Internet 160") > 0 Or sOutput.IndexOf("Fibernet XL") > 0 Or
            '        sOutput.IndexOf("Fibernet 200") > 0 Or sOutput.IndexOf("Fiber 200") > 0) Then
            '    _FUP = True
            'End If

            'Dim arrLines As String() = _InputData.Split(CChar(vbLf)), line As String = "", sSplit As String()

            '            Dim IndexToday As Integer = 0
            '            Dim bUsageData As Boolean = False
            '            Dim m As Match
            '            For i = 0 To arrLines.Length - 1
            '                If arrLines(i).Length < 5 Then GoTo NextLine
            '                line = arrLines(i)

            '                If (line.IndexOf("Mijn klantennummer:") > 0) And (_CustomerNr = 0) Then
            '                    m = Regex.Match(line, "breuertextbold"">([0-9]+)</span>")
            '                    If m.Success Then _CustomerNr = CInt(m.Groups(1).Value)
            '                End If

            '                If (line.IndexOf("index:0,label:") > 0) And (_dateRangeId = "") Then
            '                    m = Regex.Match(line, "\'(.*?)\'")
            '                    If m.Success Then _dateRangeId = m.Groups(1).Value
            '                ElseIf (line.IndexOf("Laatst berekend op") > 0) And (_TeleDate = CDate("01/01/1970")) Then
            '                    _TeleDate = CDate(line.Substring(line.IndexOf("Laatst") + 19, 10))
            '                ElseIf (line.IndexOf("komt op 0 op") > 0) And (_TeleReset = CDate("01/01/1970")) Then
            '                    m = Regex.Match(line, "([0-9]{2})\/([0-9]{2})\/([0-9]{4})")
            '                    If m.Success And m.Groups.Count > 3 Then
            '                        _TeleReset = CDate(CInt(m.Groups(1).Value) & "/" & CInt(m.Groups(2).Value) & "/" & CInt(m.Groups(3).Value))
            '                    End If
            '                End If

            '                - bars for everybody
            '                If line.IndexOf("days") > 0 And Not bUsageData Then
            '                    line = line.Replace("'", "")
            '                    bUsageData = True
            '                    m = Regex.Match(line, "days\:\[(.*?)\]")
            '                    If m.Success Then
            '                        sSplit = m.Groups(1).ToString.Split(CChar(","))
            '                        For j = 0 To sSplit.Length - 1
            '                            _Data.Add(New GraphData With {.Day = sSplit(j)})
            '                            If CDate(_Data(_Data.Count - 1).Day) = Date.Today Then IndexToday = j
            '                        Next j
            '                    End If

            '                    Try
            '                        m = Regex.Match(line, If(bfup, "detailedPeakUsage", "detailedUsage") & "\:\[(.*?)\]")
            '                        If m.Success Then
            '                            sSplit = m.Groups(1).ToString.Split(CChar(","))
            '                            For j = 0 To _Data.Count - 1
            '                                _Data(j).Basic = CLng(CDbl(sSplit(j).Replace(".", ",")) * If(bfup, 1024, 1))
            '                                If basic_volume_max < _Data(j).Basic Then basic_volume_max = _Data(j).Basic
            '                                _Usage_MBUsed += _Data(j).Basic
            '                                If j = IndexToday Then _Usage_Today = _Data(j).Basic
            '                            Next j
            '                        End If
            '                    Catch ex As Exception

            '                    End Try

            '                    Try
            '                        m = Regex.Match(line, If(bfup, "detailedOffPeakUsage", "detailedExtraUsage") & "\:\[(.*?)\]")
            '                        If m.Success Then
            '                            sSplit = m.Groups(1).ToString.Split(CChar(","))
            '                            For j = 0 To _Data.Count - 1
            '                                _Data(j).Extra = CLng(CDbl(sSplit(j).Replace(".", ",")) * If(bfup, 1024, 1))
            '                                If extra_volume_max < _Data(j).Extra Then extra_volume_max = _Data(j).Extra
            '                                If j = IndexToday Then _Usage_Today += _Data(j).Extra
            '                            Next j
            '                        End If
            '                    Catch ex As Exception

            '                    End Try

            '                    If Not bfup Then
            '                        Try
            '                            m = Regex.Match(line, "detailedWiFreeUsage\:\[(.*?)\]")
            '                            If m.Success Then
            '                                sSplit = m.Groups(1).ToString.Split(CChar(","))
            '                                For j = 0 To _Data.Count - 1
            '                                    _Data(j).WiFree = CLng(sSplit(j).Replace(".", ","))
            '                                    If wifree_volume_max < _Data(j).WiFree Then wifree_volume_max = _Data(j).WiFree
            '                                    If j = IndexToday Then _Usage_Today += _Data(j).WiFree
            '                                Next j
            '                            End If
            '                        Catch ex As Exception

            '                        End Try

            '                        m = Regex.Match(line, "totalUsagePercentage:(.*?)\,")
            '                        If m.Success Then
            '                            _Usage_Percentage = CShort(m.Groups(1).Value.Replace(".", ","))
            '                        End If

            '                        m = Regex.Match(line, "totalMax:(.*?)\,")
            '                        If m.Success Then
            '                            _Usage_MBLimit = CLng(m.Groups(1).Value.Replace(".", ",")) * 1024 'GB 2 MB
            '                        End If

            '                    Else
            '                        -bfup
            '                        peakUsage':25.91455,'offPeakUsage':24.789501,'squeeze':500
            '                        m = RegularExpressions.Regex.Match(line, "peakUsage:(.*?)\,")
            '                        If m.Success Then
            '                        _Usage_MBUsed += CInt(CDbl(m.Groups(1).Value.Replace(".", ",")) * 1024) 'GB 2 MB
            '                        End If

            '                            m = RegularExpressions.Regex.Match(line, "offPeakUsage:(.*?)\,")
            '                           If m.Success Then
            '                        _Usage_MBUsed += cint(m.Groups(1).Value * 1024) 'GB 2 MB
            '                        End If

            '                        m = Regex.Match(line, "squeeze:(.*?)\,")
            '                        If m.Success Then
            '                            _Usage_MBLimit = CLng(CDbl(m.Groups(1).Value.Replace(".", ",")) * 1024)  'GB 2 MB
            '                        End If
            '                        -this has changed so not workie workie anymore
            '                         If line.IndexOf("fupUsageNormal") > 0 Then '-get status
            '                        m = RegularExpressions.Regex.Match(line, ">(.*)<\/span>")
            '                        If m.Success Then
            '                        FUP_Status = m.Groups(1).Value
            '                        End If
            '                        End If
            '                    End If
            '                End If

            '                If line.IndexOf("saldoPostitLink") > 0 Then '-when you need to pay a bill
            '                    line = arrLines(i + 1)
            '                    m = Regex.Match(line, "accountNumber=([0-9]+)""")
            '                    If m.Success Then
            '                        _CustomerAccountNr = CInt(m.Groups(1).Value)
            '                    End If
            '                End If
            'NextLine:
            '            Next i
            '            m = Nothing
            '            line = Nothing
            '            arrLines = Nothing
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "TeleParse.Parse()")
        End Try


        If bfup Then
            '-switch data
            Dim gTmp As Long
            For i = 0 To _Data.Count - 1
                gTmp = _Data(i).Extra
                _Data(i).Extra = _Data(i).Basic
                _Data(i).Basic = gTmp
            Next
            gTmp = Nothing
            'peakUsagePercentage':17,'offPeakUsagePercentage':17
            _Usage_Percentage = CShort(_Usage_MBUsed / _Usage_MBLimit * 100)
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
        Dim iUsage As Long = 0
        For i = 0 To CShort(_Data.Count - 1)
            _Data(i).Limit = _Usage_MBLimit
            If i > 0 Then
                iUsage += _Data(i - 1).Basic + If(bfup, 0, _Data(i - 1).Extra)
                _Data(i).Limit -= iUsage
            End If
            _Data(i).Limit = CInt(_Data(i).Limit / (_Data.Count - i))
        Next

        If TeleReset <> #1/1/1970# Then
            _DateDiff = CShort(DateDiff(DateInterval.Day, Date.Now, _TeleReset) + 1)
        Else
            _DateDiff = -1
        End If
    End Sub

    Private Sub Reset()
        '-reset values
        _CustomerNr = 0
        _CustomerAccountNr = 0
        _dateRangeId = ""
        _TeleDate = CDate("01/01/1970")
        _TeleReset = CDate("01/01/1970")
        _Data.Clear()
        _FUP = False
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
    End Sub
End Class
