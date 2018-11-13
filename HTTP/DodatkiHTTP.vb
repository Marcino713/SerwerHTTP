Public Enum WersjaHTTP
    Inna = 0
    HTTP_1_1 = 1
End Enum

Public Enum MetodaHTTP
    Nieznana = 0
    [GET] = 1
    HEAD = 2
    POST = 4
    PUT = 8
    DELETE = 16
End Enum


Public Structure ZmiennaHTTP
    Public Nazwa As String
    Public Wartosc As String

    Public Sub New(NazwaZmiennej As String, WartoscZmiennej As String)
        Nazwa = NazwaZmiennej
        Wartosc = WartoscZmiennej
    End Sub

    Public Overrides Function ToString() As String
        Return Nazwa & "=" & Wartosc
    End Function

End Structure

Public Structure Ciasteczko
    Public Nazwa As String
    Public Wartosc As String
    Public Wygasa As Date
    Public Sciezka As String
    Public Sub New(Nazwa As String, Wartosc As String)
        Me.Nazwa = Nazwa
        Me.Wartosc = Wartosc
        Wygasa = Date.MinValue
        Sciezka = ""
    End Sub
End Structure

Public Structure DanePliku
    Public Nazwa As String
    Public Rozmiar As Long
    Public Data As Date
End Structure