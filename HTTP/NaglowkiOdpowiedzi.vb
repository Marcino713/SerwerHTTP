Public Class NaglowkiOdpowiedzi
    Private _Allow As MetodaHTTP = MetodaHTTP.Nieznana
    Private _Length As Integer = 0
    Private _LastModified As Date = Date.MinValue
    Private _ContentType As String = ""
    Private _Ciasteczka As New List(Of Ciasteczko)

    Public Property Allow As MetodaHTTP
        Get
            Return _Allow
        End Get
        Set(value As MetodaHTTP)
            _Allow = value
        End Set
    End Property

    Public Property Length As Integer
        Get
            Return _Length
        End Get
        Set(value As Integer)
            _Length = value
        End Set
    End Property

    Public Property LastModified As Date
        Get
            Return _LastModified
        End Get
        Set(value As Date)
            _LastModified = value
        End Set
    End Property

    Public Property ContentType As String
        Get
            If _ContentType = "" Then Return MIME_APPLICATION_OCTET_STREAM
            Return _ContentType
        End Get
        Set(value As String)
            _ContentType = value
        End Set
    End Property

    Friend ReadOnly Property Ciasteczka As List(Of Ciasteczko)
        Get
            Return _Ciasteczka
        End Get
    End Property

    Public Sub DodajCiasteczko(DaneCiasteczka As Ciasteczko)
        Ciasteczka.Add(DaneCiasteczka)
    End Sub
End Class