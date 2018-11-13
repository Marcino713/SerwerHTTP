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

    Public Function SprzatnijDate(Data As Date) As Date
        Return New Date(Data.Year, Data.Month, Data.Day, Data.Hour, Data.Minute, Data.Second)
    End Function

End Module