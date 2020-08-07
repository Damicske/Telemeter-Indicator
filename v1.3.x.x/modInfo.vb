Imports System
Imports System.Text
Imports System.Reflection
Imports System.Windows.Forms
Imports System.Security.Cryptography

Module modInfo
    Public Use As Int64 = 1
    Public BETA As Boolean = False
    Public VersionType As Int64
    Public waitB As Int64
    Public sFileIni As String

    Public Const SDebugUrl As String = "https://www.cd-pc.be/bug_report.php"

    Function Clock(ByVal iTime As Int64, Optional GiveDays As Boolean = True, Optional GiveWeeks As Boolean = True, Optional MS As Boolean = True, Optional GiveMs As Boolean = True) As String
        Dim months As String = "", weeks As Int32 = 0, days As Int32 = 0, uren As Int16 = 0, minuten As Int32 = 0, seconden As Int32 = 0, msec As Int32 = 0
        '-set miliseconds to seconds
        If MS Then
            msec = iTime Mod 1000
            iTime = iTime / 1000
        End If
        '-extract Time
        uren = iTime \ 3600
        minuten = Int((iTime - (uren * 3600)) \ 60)
        seconden = iTime - (uren * 3600) - ((minuten * 60))
        '-days
        If GiveDays = True Then
            days = (uren \ 24)
            uren -= days * 24
        End If
        '-weeks
        If GiveWeeks = True Then
            weeks = (days \ 7)
            days -= weeks * 7
            '-months
            months = weeks \ (52 \ 12)
            If months = 0 Then
                months = ""
            Else
                weeks -= months * (52 \ 12)
                months = months & " Months "
            End If
        End If

        Clock = months & IIf(GiveWeeks, weeks & " Week" & IIf(weeks = 1, "", "s") & " ", "") & IIf(GiveDays, days & " Days ", "") & uren & ":" & IIf(minuten < 10, "0", "") & minuten & _
        ":" & IIf(seconden < 10, "0", "") & seconden & IIf(GiveMs, "." & msec, "")
    End Function

    Public Function VersionX(Optional bClean As Boolean = False) As String
        If bClean Then
            Return Application.ProductVersion & IIf(VersionType = 2 Or BETA, " BETA", "")
        Else
            Return My.Application.Info.Title & " [" & Application.ProductVersion & IIf(VersionType = 2 Or BETA, " BETA", "") & "]"
        End If
    End Function

    Public Function GetFileVersionInfo(ByVal filename As String) As String
        Return FileVersionInfo.GetVersionInfo(filename).FileVersion.ToString
    End Function

    'Sets the text for the specified NotifyIcon up to 127 characters in length.
    'A System.Windows.Forms.NotifyIcon representing the NotifyIcon for which to set the text.
    'A System.String representing the text to set for the NotifyIcon.
    Public Function SetNotifyIconText(ByVal notifyIcon As NotifyIcon, ByVal text As String)
        If (text.Length > 127) Then text = text.Substring(0, 127)

        Dim type As Type = GetType(NotifyIcon)
        Const Hidden As BindingFlags = (BindingFlags.NonPublic Or BindingFlags.Instance)
        Dim textField As FieldInfo = type.GetField("text", Hidden)
        If (textField Is Nothing) Then Return 0
        textField.SetValue(notifyIcon, text)
        Dim addedField As FieldInfo = type.GetField("added", Hidden)
        If (addedField Is Nothing) Then Return 0

        If CType(addedField.GetValue(notifyIcon), Boolean) Then
            type.GetMethod("UpdateIcon", Hidden).Invoke(notifyIcon, New Object() {True})
        End If
        Return 0
    End Function

    Public Function Convert_MB(ByVal InputMB As Int64, Optional ByVal bShowBinair As Boolean = True) As String
        Dim bIsNegative As Boolean = False
        If InputMB < 0 Then
            bIsNegative = True
            InputMB *= -1
        End If
        InputMB = InputMB * 1024 ^ 2 'convert to bytes
        Convert_MB = FormatNumber(InputMB, 2) & "B"
        If Not bShowBinair Then
            If InputMB >= (1000 ^ 4) Then
                Convert_MB = FormatNumber((InputMB / 1000 ^ 4), 2) & "TB"
            ElseIf InputMB >= (1000 ^ 3) Then
                Convert_MB = FormatNumber((InputMB / 1000 ^ 3), 2) & "GB"
            ElseIf InputMB >= (1000 ^ 2) Then
                Convert_MB = FormatNumber(InputMB / 1000 ^ 2, 2) & "MB"
            ElseIf InputMB >= 1000 Then
                Convert_MB = FormatNumber((InputMB / 1000), 2) & "kB"
            End If
        Else
            If InputMB >= (1024 ^ 4) Then
                Convert_MB = FormatNumber((InputMB / 1024 ^ 4), 2) & "TiB"
            ElseIf InputMB >= (1024 ^ 3) Then
                Convert_MB = FormatNumber((InputMB / 1024 ^ 3), 2) & "GiB"
            ElseIf InputMB >= (1024 ^ 2) Then
                Convert_MB = FormatNumber(InputMB / 1024 ^ 2, 2) & "MiB"
            ElseIf InputMB >= 1024 Then
                Convert_MB = FormatNumber((InputMB / 1024), 2) & "kiB"
            End If
        End If
        If bIsNegative Then Convert_MB = "-" & Convert_MB
    End Function

    Public Function GetMD5Hash(ByVal theInput As String) As String
        Using hasher As MD5 = MD5.Create()    ' create hash object
            ' Convert to byte array and get hash
            Dim dbytes As Byte() = hasher.ComputeHash(Encoding.UTF8.GetBytes(theInput))
            ' sb to create string from bytes
            Dim sBuilder As New StringBuilder()
            ' convert byte data to hex string
            For n As Int32 = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("X2"))
            Next
            Return sBuilder.ToString()
        End Using
    End Function

    Public Function GetMD5FileHash(ByVal file As FileStream) As String
        Using hasher As MD5 = md5.Create()    ' create hash object
            ' Convert to byte array and get hash
            Dim dbytes As Byte() = hasher.ComputeHash(file)
            ' sb to create string from bytes
            Dim sBuilder As New StringBuilder()
            ' convert byte data to hex string
            For n As Int32 = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("x2"))
            Next
            Return sBuilder.ToString()
        End Using
    End Function

    Public Function GetSHA512Hash(ByVal theInput As String) As String
        Using sha As New SHA512Managed ' declare sha as a new SHA1CryptoServiceProvider
            Dim dBytes() As Byte = sha.ComputeHash(Encoding.ASCII.GetBytes(theInput)) ' covert the password into ASCII code
            Dim sBuilder As New StringBuilder()
            For n As Int32 = 0 To dBytes.Length - 1
                sBuilder.Append(dBytes(n).ToString("X2"))
            Next
            Return sBuilder.ToString() ' boom there goes the encrypted password!
        End Using
    End Function
End Module
