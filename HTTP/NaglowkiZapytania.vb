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