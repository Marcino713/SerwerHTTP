Public Class NaglowkiZapytania
    Private _Host As String = ""
    Private _UserAgent As String = ""
    Private _Accept As String = ""
    Private _AcceptLanguage As String = ""
    Private _AcceptEncoding As String = ""
    Private _Connection As String = ""
    Private _IfModifiedSince As Date = Date.MinValue
    Private _CacheControl As String = ""
    Private _Referer As String = ""
    Private _ContentLength As Integer = 0
    Private _ContentType As String = ""
    Private _Dnt As String = ""
    Private _UaCpu As String = ""
    Private _Range As String = ""
    Private _UpgradeInsecureRequests As String = ""
    Private _Pragma As String = ""
    Private _XRequestedWith As String = ""
    Private _Authorization As DaneAutoryzacji = Nothing
    Private _Cookie As ZmiennaHTTP() = Nothing

    Public Property Host As String
        Get
            Return _Host
        End Get
        Friend Set(value As String)
            _Host = value
        End Set
    End Property

    Public Property UserAgent As String
        Get
            Return _UserAgent
        End Get
        Friend Set(value As String)
            _UserAgent = value
        End Set
    End Property

    Public Property Accept As String
        Get
            Return _Accept
        End Get
        Friend Set(value As String)
            _Accept = value
        End Set
    End Property

    Public Property AcceptLanguage As String
        Get
            Return _AcceptLanguage
        End Get
        Friend Set(value As String)
            _AcceptLanguage = value
        End Set
    End Property

    Public Property AcceptEncoding As String
        Get
            Return _AcceptEncoding
        End Get
        Friend Set(value As String)
            _AcceptEncoding = value
        End Set
    End Property

    Public Property Connection As String
        Get
            Return _Connection
        End Get
        Friend Set(value As String)
            _Connection = value
        End Set
    End Property

    Public Property IfModifiedSince As Date
        Get
            Return _IfModifiedSince
        End Get
        Friend Set(value As Date)
            _IfModifiedSince = value
        End Set
    End Property

    Public Property CacheControl As String
        Get
            Return _CacheControl
        End Get
        Friend Set(value As String)
            _CacheControl = value
        End Set
    End Property

    Public Property Referer As String
        Get
            Return _Referer
        End Get
        Friend Set(value As String)
            _Referer = value
        End Set
    End Property

    Public Property ContentLength As Integer
        Get
            Return _ContentLength
        End Get
        Friend Set(value As Integer)
            _ContentLength = value
        End Set
    End Property

    Public Property ContentType As String
        Get
            Return _ContentType
        End Get
        Friend Set(value As String)
            _ContentType = value
        End Set
    End Property

    Public Property Dnt As String
        Get
            Return _Dnt
        End Get
        Friend Set(value As String)
            _Dnt = value
        End Set
    End Property

    Public Property UaCpu As String
        Get
            Return _UaCpu
        End Get
        Friend Set(value As String)
            _UaCpu = value
        End Set
    End Property

    Public Property Range As String
        Get
            Return _Range
        End Get
        Friend Set(value As String)
            _Range = value
        End Set
    End Property

    Public Property UpgradeInsecureRequests As String
        Get
            Return _UpgradeInsecureRequests
        End Get
        Friend Set(value As String)
            _UpgradeInsecureRequests = value
        End Set
    End Property

    Public Property Pragma As String
        Get
            Return _Pragma
        End Get
        Friend Set(value As String)
            _Pragma = value
        End Set
    End Property

    Public Property XRequestedWith As String
        Get
            Return _XRequestedWith
        End Get
        Friend Set(value As String)
            _XRequestedWith = value
        End Set
    End Property

    Public Property Authorization As DaneAutoryzacji
        Get
            Return _Authorization
        End Get
        Friend Set(value As DaneAutoryzacji)
            _Authorization = value
        End Set
    End Property

    Public Property Cookie As ZmiennaHTTP()
        Get
            If _Cookie IsNot Nothing AndAlso _Cookie.Length = 0 Then Return Nothing
            Return _Cookie
        End Get
        Friend Set(value As ZmiennaHTTP())
            _Cookie = value
        End Set
    End Property

End Class

Public Class DaneAutoryzacji
    Private _Username As String = ""
    Private _Realm As String = ""
    Private _Nonce As String = ""
    Private _Uri As String = ""
    Private _Response As String = ""
    Private _Opaque As String = ""
    Private _Password As String = ""
    Private _Method As MetodaHTTP = MetodaHTTP.Nieznana
    Private _Type As MetodaUwierzytelniania = MetodaUwierzytelniania.Prosta

    Public ReadOnly Property Uzytkownik As String
        Get
            Return _Username
        End Get
    End Property

    Public ReadOnly Property Metoda As MetodaUwierzytelniania
        Get
            Return _Type
        End Get
    End Property

    Public Sub New(Nazwa As String, Haslo As String)
        _Type = MetodaUwierzytelniania.Prosta
        _Username = Nazwa
        _Password = Haslo
    End Sub

    Public Sub New(Username As String, Realm As String, Nonce As String, Uri As String, Response As String, Opaque As String, Method As MetodaHTTP)
        _Type = MetodaUwierzytelniania.Zlozona
        _Username = Username
        _Realm = Realm
        _Nonce = Nonce
        _Uri = Uri
        _Response = Response
        _Opaque = Opaque
        _Method = Method
    End Sub

    Public Function CzyZgodneHaslo(Haslo As String) As Boolean
        If _Type = MetodaUwierzytelniania.Prosta Then
            Return Haslo = _Password
        ElseIf _Type = MetodaUwierzytelniania.Zlozona Then
            Dim metoda As String = ""
            Select Case _Method
                Case MetodaHTTP.GET : metoda = "GET"
                Case MetodaHTTP.POST : metoda = "POST"
                Case MetodaHTTP.HEAD : metoda = "HEAD"
                Case MetodaHTTP.PUT : metoda = "PUT"
                Case MetodaHTTP.DELETE : metoda = "DELETE"
            End Select
            If metoda = "" Then Return False

            Dim s1 As String = ObliczMD5(_Username & ":" & _Realm & ":" & Haslo)
            Dim s2 As String = ObliczMD5(metoda & ":" & _Uri)
            Dim s3 As String = ObliczMD5(s1 & ":" & _Nonce & ":" & s2)
            Return s3 = _Response
        End If

        Return False
    End Function
End Class