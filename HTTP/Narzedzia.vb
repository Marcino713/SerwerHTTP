Public Module Narzedzia

    'Typy MIME
    Public Const MIME_APPLICATION_X_WWW_FORM_URLENCODED As String = "application/x-www-form-urlencoded"
    Public Const MIME_APPLICATION_JAVASCRIPT As String = "application/javascript"
    Public Const MIME_APPLICATION_JSON As String = "application/json"
    Public Const MIME_APPLICATION_ZIP As String = "application/zip"
    Public Const MIME_APPLICATION_XML As String = "application/xml"
    Public Const MIME_APPLICATION_OCTET_STREAM As String = "application/octet-stream"
    Public Const MIME_TEXT_PLAIN As String = "text/plain"
    Public Const MIME_TEXT_HTML As String = "text/html"
    Public Const MIME_TEXT_CSS As String = "text/css"
    Public Const MIME_TEXT_XC As String = "text/x-c"
    Public Const MIME_IMAGE_JPEG As String = "image/jpeg"
    Public Const MIME_IMAGE_PNG As String = "image/png"
    Public Const MIME_IMAGE_GIF As String = "image/gif"
    Public Const MIME_IMAGE_ICON As String = "image/vnd.microsoft.icon"
    Public Const MIME_MULTIPART_FORM_DATA As String = "multipart/form-data"


    Public Const HTTP_WERSJA_1_1 As String = "HTTP/1.1"
    Public Const DATA_FORMAT As String = "d-MM-yyyy H:mm:ss"
    Public Const DATA_FOLDER As String = "yyyy-MM-dd H:mm:ss"
    Public Const DATA_LOGI As String = "d-MM-yyyy H-mm-ss,fffff"

    Public Function SprzatnijDate(Data As Date) As Date
        Return New Date(Data.Year, Data.Month, Data.Day, Data.Hour, Data.Minute, Data.Second)
    End Function

    Public Function BajtyDoHex(dane As Byte()) As String
        Dim sBuilder As StringBuilder = New StringBuilder()
        For i As Integer = 0 To dane.Length - 1
            sBuilder.Append(dane(i).ToString("x2"))
        Next
        Return sBuilder.ToString()
    End Function

    Public Function ObliczMD5(tekst As String) As String
        Return BajtyDoHex(Security.Cryptography.MD5.Create().ComputeHash(New UTF8Encoding().GetBytes(tekst)))
    End Function

    Public Function RozmiarToString(rozm As Long) As String
        If rozm = 0 Then Return "---"
        If rozm < 1024 Then Return rozm & " B"

        Dim r As Double = rozm / 1024.0
        Dim jedn As String() = {" kB", " MB", " GB", " TB"}

        For i As Integer = 0 To jedn.Length - 1
            If r < 1024.0 Then Return r.ToString("f2") & jedn(i)
            r /= 1024.0
        Next

        Return r.ToString("f2") & jedn(jedn.Length - 1)

    End Function

End Module