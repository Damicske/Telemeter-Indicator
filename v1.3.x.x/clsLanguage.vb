Imports System.IO
Public Class clsLanguage
    Public ErrorMessage(13) As String
    Public ErrorMessageTitle(13) As String
    Public StatusMessage(13) As String
    Public InetMessage(13) As String
    Public BoxMessage(10) As String
    Public OtherText(10) As String
    Public GraphText(3) As String

    Public bLanguageLoaded As Boolean = False
    Public LanguageIndex As Int16 = -1

    Public def_lng_name As String = ""
    Private Declare Unicode Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringW" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Int32, ByVal lpFileName As String) As Int32

    Private Structure LanguageFileInfo
        Public LanguageName As String
        Public LanguageFile As String
    End Structure
    Private LFILanguages() As LanguageFileInfo

    Public Function LoadLanguages() As Boolean
        Dim strFileSize As String = ""
        Dim di As New DirectoryInfo(Application.StartupPath & "\locale")
        Dim aryFi As FileInfo() = di.GetFiles("*.lng")
        Dim fi As FileInfo
        If Not di.Exists Or (aryFi.Length = 0) Then Return False
        ResetLanguages()
        LoadLanguages = False
        ReDim Preserve LFILanguages(0)
        LFILanguages(0).LanguageFile = ""
        LFILanguages(0).LanguageName = def_lng_name
        Try
            Dim intCount As Integer = 1
            For Each fi In aryFi
                ReDim Preserve LFILanguages(intCount)
                LFILanguages(intCount).LanguageFile = di.FullName & "\" & fi.Name
                LFILanguages(intCount).LanguageName = ReadIniFile(LFILanguages(intCount).LanguageFile, "IDENTIFICATION", "language", "")
                If LFILanguages(intCount).LanguageName <> "" Then
                    intCount += 1
                End If
            Next
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "clsLanguage :: Loadlanguages")
        Finally
            If UBound(LFILanguages) >= 0 Then
                LoadLanguages = True
                bLanguageLoaded = True
            End If
        End Try
    End Function

    Private Sub ResetLanguages()
        For i As Int16 = 0 To UBound(StatusMessage) : StatusMessage(i) = "" : Next
        For i = 0 To UBound(InetMessage) : InetMessage(i) = "" : Next
        For i = 0 To UBound(BoxMessage) : BoxMessage(i) = "" : Next
        For i = 0 To UBound(OtherText) : OtherText(i) = "" : Next
        For i = 0 To UBound(GraphText) : GraphText(i) = "" : Next
        bLanguageLoaded = False
        LFILanguages = Nothing
    End Sub

    Public Sub ShowLanguages(ByVal cboLanguages As ComboBox)
        If bLanguageLoaded Then
            cboLanguages.Items.Clear()
            For i As Integer = 0 To UBound(LFILanguages)
                cboLanguages.Items.Add(LFILanguages(i).LanguageName)
            Next i
            If cboLanguages.Items.Count > 1 Then cboLanguages.Enabled = True
        End If
    End Sub

    Public Sub ChangeControlsCaption(ByVal frmForm As Form)
        '**** This sub will get the captions for the controls in the passed form
        '**** in the language file.
        Dim strControls() As String
        Dim intCnt As Integer = 0
        Dim strName As String = "" '-return name
        Dim intIndex As Integer = 0 '-return index

        If (LanguageIndex > UBound(LFILanguages)) Or (LanguageIndex < 0) Then LanguageIndex = 0
        If LFILanguages(LanguageIndex).LanguageFile = "" Then
            frmSettings.Def_Lng_Vals()
            Exit Sub
        End If
        'We get all the keys in the controls section of the file (the control names),
        'and separate them in a table, the last one will be empty because of the
        'double null chars at the end
        '   strControls = Split(ReadIniFile(LFILanguages(LanguageIndex).LanguageFile, frmForm.Name, vbNullString, ""), Chr(0))
        'Loop through the controls, skip the last one, because it's empty
        Dim sControlText As String = ""
        frmForm.Text = ReadIniFile(LFILanguages(LanguageIndex).LanguageFile, frmForm.Name, frmForm.Name, frmForm.Text)
        For intCnt = 0 To frmForm.Controls.Count - 1
            On Error GoTo errControl
            frmForm.Controls(intCnt).Text = ReadIniFile(LFILanguages(LanguageIndex).LanguageFile, frmForm.Name, frmForm.Controls(intCnt).Name, frmForm.Controls(intCnt).Text)
            ' If Not frmForm.Controls(intCnt).tooltiptext Is Nothing Then
            'End If
            ' Else
            'If blnExtractNameIndex(strControls(intCnt), strName, intIndex) Then
            'If strName.IndexOf("_ttt") > 0 Then
            'strName = Left$(strName, Len(strName) - 4)
            ' frmForm.Controls(strName).Item(intIndex).ToolTipText = readinifile(LFILanguages(LanguageIndex).LanguageFile, frmForm.Name, strControls(intCnt), frmForm.Controls(strName).Item(intIndex).ToolTipText)
            'Else
            'frmForm.Controls(strName).Item(intIndex).text = ReadIniFile(LFILanguages(LanguageIndex).LanguageFile, frmForm.Name, strControls(intCnt), frmForm.Controls(strName).Item(intIndex).text)
            'End If
            ' Else
            'If strName.IndexOf("_ttt") > 0 Then
            'strName = strName.Replace("_ttt", "")
            'frmForm.tooltip1.settooltip(strName) = ReadIniFile(LFILanguages(LanguageIndex).LanguageFile, frmForm.Name, strControls(intCnt), frmForm.Controls(strName).ToolTipText)
            'Else
            'If IsNothing(frmForm.Controls(strName)) Then
            'frmForm.Controls("ContextMenuStrip1").Controls(strName).Text = ReadIniFile(LFILanguages(LanguageIndex).LanguageFile, frmForm.Name, strControls(intCnt), frmForm.Controls("ContextMenuStrip1").Controls(strName).Text)
            'frmForm.ControlCollection("ConentMenuStrip")
            ' Else
            'frmForm.Controls(strName).Text = ReadIniFile(LFILanguages(LanguageIndex).LanguageFile, frmForm.Name, strControls(intCnt), frmForm.Controls(strName).Text)
            'End If
            'End If
            'End If
            'End If
            On Error GoTo 0
        Next intCnt
        Exit Sub

errControl:
        Debug.WriteLine("Error #" & Str(Err.Number) & " :: " & Err.Description)
        Resume Next
    End Sub

    Public Sub ReadMessages(sArrayName As String, strArray() As String)
        Dim strName As String = ""
        Dim intCnt As Integer, intIndex As Integer
        Dim strControls() As String
        'Here we get the file of the chosen language
        If (LanguageIndex > UBound(LFILanguages)) Or (LanguageIndex < 0) Then LanguageIndex = 0
        If LFILanguages(LanguageIndex).LanguageFile = "" Then
            frmSettings.Def_Lng_Vals()
            Exit Sub
        End If

        'We get all the keys in the controls section of the file (the control names),
        'and separate them in a table, the last one will be empty because of the
        'double null chars at the end
        strControls = Split(ReadIniFile(LFILanguages(LanguageIndex).LanguageFile, ".", vbNullString, ""), Chr(0))
        On Error GoTo errControl
        For intCnt = 0 To UBound(strControls) - 1
            'Check if it's an indexed control
            If blnExtractNameIndex(strControls(intCnt), strName, intIndex) Then
                If strName = sArrayName Then strArray(intIndex) = ReadIniFile(LFILanguages(LanguageIndex).LanguageFile, ".", strControls(intCnt), strArray(intIndex))
            Else
                If strName = sArrayName Then strArray(0) = ReadIniFile(LFILanguages(LanguageIndex).LanguageFile, ".", strControls(intCnt), strArray(0))
            End If
        Next intCnt
        Exit Sub

errControl:
        Debug.WriteLine("Error # " & Str(Err.Number) & " was generated by " & Err.Source & ControlChars.CrLf & Err.Description)
        Resume Next
    End Sub

    Private Function ReadIniFile(ByVal strIniFileName As String, ByVal strSection As String, ByVal strItem As String, ByVal strDefault As String) As String
        'To get all the section names in the file, strSection should be null
        'The returned string will contain the section names separated by a null char
        'To get all the key names in one section, strItem should be null
        'The returned string will contain the key names separated by a null char
        Dim sBuffer As New String(" ", 2048)
        Dim n As Int32 = 0
        ReadIniFile = ""
        n = GetPrivateProfileString(strSection, strItem, strDefault, sBuffer, sBuffer.Length, strIniFileName)
        If n > 0 Then ' return whatever it gave us
            ReadIniFile = sBuffer.Substring(0, n)
        End If
    End Function

    Private Function blnExtractNameIndex(ByVal strControlName As String, ByRef strName As String, ByRef intIndex As Integer) As Boolean
        '**** This function checks if the control is indexed, contains "(<number>)", and
        '**** returns the name and the LanguageIndex separated.
        Dim intPos1 As Integer = 0, intPos2 As Integer = 0
        intIndex = 0
        Try
            intPos1 = InStr(1, strControlName, "(", vbTextCompare)
            If intPos1 Then
                intPos2 = InStr(intPos1 + 1, strControlName, ")", vbTextCompare)
                strName = Left$(strControlName, intPos1 - 1)
                intIndex = CInt(Mid$(strControlName, intPos1 + 1, intPos2 - intPos1 - 1))
                Return True
            Else
                strName = strControlName
                Return False
            End If
        Catch ex As Exception
            'The string between the parenthesis is not a number, something is wrong
            'since a control can't have parenthesis in its name
            Return False
        End Try
    End Function
End Class
