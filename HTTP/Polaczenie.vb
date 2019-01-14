Imports System.Net.Security
Imports System.Threading
Imports System.Net.Sockets
Imports System.Security.Cryptography.X509Certificates

Public Class Polaczenie

    'Dane obiektu
    Friend Usuniety As Boolean = False
    Friend OstatniaAktywnosc As Date = Now
    Friend Aktywny As Boolean = False
    Private DataSerwera As Date = Now.ToUniversalTime
    Private _WyslanoOdpowiedz As Boolean = False
    Private ZapisanoOdwiedziny As Boolean = False
    Private _AdresIP As String = ""
    Private SerwerHTTP As Serwer
    Private utf As New UTF8Encoding()
    Private bledy As ZarzadzanieBledami
    Private certyfikat As String = "od.p7b"

    Public ReadOnly Property WyslanoOdpowiedz As Boolean
        Get
            Return _WyslanoOdpowiedz
        End Get
    End Property

    'Obsługa połączenia
    Private klient As TcpClient
    Private strumien As SslStream
    Private br As BinaryReader
    Private bw As BinaryWriter
    Private strumienPlik As FileStream
    Private bwp As BinaryWriter
    Private watek As Thread

    'Zapytanie
    Private _Zapytanie As NaglowkiZapytania
    Private _Metoda As MetodaHTTP
    Private _Adres As String
    Private _Wersja As WersjaHTTP
    Private _ZmiennePOST As ZmiennaHTTP()
    Private _ZmienneGET As ZmiennaHTTP()
    Private _ZawartoscZapytania As Byte() = New List(Of Byte)().ToArray()

    'Odpowiedź
    Private _Odpowiedz As NaglowkiOdpowiedzi
    Private _ZawartoscOdpowiedzi As Byte() = New List(Of Byte)().ToArray()

    Public ReadOnly Property Zapytanie As NaglowkiZapytania
        Get
            Return _Zapytanie
        End Get
    End Property

    Public ReadOnly Property Metoda As MetodaHTTP
        Get
            Return _Metoda
        End Get
    End Property

    Public ReadOnly Property Adres As String
        Get
            Return _Adres
        End Get
    End Property

    Public ReadOnly Property Wersja As WersjaHTTP
        Get
            Return _Wersja
        End Get
    End Property

    Public ReadOnly Property ZmiennePOST As ZmiennaHTTP()
        Get
            If _ZmiennePOST IsNot Nothing AndAlso _ZmiennePOST.Length = 0 Then Return Nothing
            Return _ZmiennePOST
        End Get
    End Property

    Public ReadOnly Property ZmienneGET As ZmiennaHTTP()
        Get
            If _ZmienneGET IsNot Nothing AndAlso _ZmienneGET.Length = 0 Then Return Nothing
            Return _ZmienneGET
        End Get
    End Property

    Public ReadOnly Property ZawartoscZapytania As Byte()
        Get
            Return _ZawartoscZapytania
        End Get
    End Property

    Public ReadOnly Property Odpowiedz As NaglowkiOdpowiedzi
        Get
            Return _Odpowiedz
        End Get
    End Property

    Public Property ZawartoscOdpowiedzi As Byte()
        Get
            Return _ZawartoscOdpowiedzi
        End Get
        Set(value As Byte())
            If value Is Nothing Then _ZawartoscOdpowiedzi = New List(Of Byte)().ToArray() Else _ZawartoscOdpowiedzi = value
        End Set
    End Property

    Public ReadOnly Property AdresIP As String
        Get
            Return _AdresIP
        End Get
    End Property

    Friend Sub New(Klient As TcpClient, SerwerHTTP As Serwer)
        Me.SerwerHTTP = SerwerHTTP
        bledy = New ZarzadzanieBledami(SerwerHTTP.Ustawienia.FolderSerwera)
        _AdresIP = Klient.Client.RemoteEndPoint.ToString
        Me.klient = Klient
        strumien = New SslStream(Klient.GetStream(), False)
        strumien.AuthenticateAsServer(X509Certificate.CreateFromCertFile(certyfikat))
        strumien.ReadTimeout = 5000
        strumien.WriteTimeout = 5000
        br = New BinaryReader(strumien)
        bw = New BinaryWriter(strumien)
        If SerwerHTTP.Ustawienia.ZapiszDanePrzychodzace OrElse SerwerHTTP.Ustawienia.ZapiszDaneWychodzace Then
            strumienPlik = SerwerHTTP.OtworzWolnyPlik
            bwp = New BinaryWriter(strumienPlik)
        End If
        watek = New Thread(AddressOf ObsluzKlienta)
        watek.Name = AdresIP
        watek.Start()
    End Sub

    Private Sub ObsluzKlienta()
        Try
            OdbierzDane()
        Catch
        End Try

        ZwolnijZasoby()
    End Sub

    Private Sub ZwolnijZasoby()
        SerwerHTTP.UsunKlienta(Me)

        Try
            klient.Close()
        Catch
        End Try

        Try
            bw.Close()
        Catch
        End Try

        Try
            bw.Dispose()
        Catch
        End Try

        Try
            br.Dispose()
        Catch
        End Try

        If bwp IsNot Nothing Then
            Try
                bwp.Close()
            Catch
            End Try

            Try
                bwp.Dispose()
            Catch
            End Try
        End If

        If strumienPlik IsNot Nothing Then
            Try
                strumienPlik.Close()
            Catch
            End Try

            Try
                strumienPlik.Dispose()
            Catch
            End Try
        End If

        Try
            strumien.Close()
        Catch
        End Try

        Try
            strumien.Dispose()
        Catch
        End Try

        If (Not ZapisanoOdwiedziny) And SerwerHTTP.Ustawienia.ZapiszOdwiedziny Then
            ZapiszOdwiedzinyKlientaHTTP("---" & vbTab & "---" & vbTab & AdresIP & vbTab & Now.ToString(DATA_FORMAT))
        End If

        If SerwerHTTP.Ustawienia.ZapiszBledy Then bledy.ZapiszBledy()
    End Sub

    Private Sub OdbierzDane()
        Dim liczba_bajtow As Integer = 0
        Dim wyslij_plik As Boolean

        Do
            Czysc()

            '----------------------------------------Zapytanie---------------------------------
            PrzetworzPierwszaLinie()
            PrzetworzNaglowki()

            'Zapisanie informacji o odwiedzinach
            If SerwerHTTP.Ustawienia.ZapiszOdwiedziny Then ZapiszOdwiedzinyKlientaHTTP()

            '----------------------------------------Odpowiedź---------------------------------
            DataSerwera = Now.ToUniversalTime


            'Przetworzenie otrzymanych danych - POST
            If Zapytanie.ContentLength <> 0 AndAlso (Metoda = MetodaHTTP.POST OrElse Metoda = MetodaHTTP.PUT) Then

                ReDim _ZawartoscZapytania(Zapytanie.ContentLength - 1)
                Dim pocz As Integer = 0

                Do While pocz < Zapytanie.ContentLength
                    liczba_bajtow = strumien.Read(_ZawartoscZapytania, pocz, _ZawartoscZapytania.Length - pocz)
                    pocz += liczba_bajtow
                Loop

                If SerwerHTTP.Ustawienia.ZapiszDanePrzychodzace Then strumienPlik.Write(_ZawartoscZapytania, 0, _ZawartoscZapytania.Length)

                PrzetworzOdebraneDane()

            End If


            'Wyślij odpowiedź
            If Not _WyslanoOdpowiedz Then

                'Czy wysłać plik
                wyslij_plik = False

                If Metoda = MetodaHTTP.GET Then
                    For Each roz As String In SerwerHTTP.Ustawienia.PrzetwarzaneRozszerzenia
                        If Adres.EndsWith(roz) Then
                            wyslij_plik = True
                            Exit For
                        End If
                    Next
                End If

                If wyslij_plik Then

                    If Adres.ToLower.EndsWith("about") Then
                        Wyslij418_ImATeapot()
                    Else
                        WyslijPlik(True)
                    End If

                Else

                    'Szczegółowa funkcja zwrotna
                    Dim funkcja As IDaneFunkcji = Nothing
                    If SerwerHTTP.Ustawienia.Funkcje IsNot Nothing Then
                        funkcja = (From d In SerwerHTTP.Ustawienia.Funkcje Where (((Not d.DokladnyAdres) And _Adres.StartsWith(d.Adres)) Or _Adres = d.Adres) And (d.Metoda And _Metoda) = _Metoda Order By d.Adres.Length Select d).FirstOrDefault()
                    End If

                    If funkcja Is Nothing Then
                        'Ogólna funkcja zwrotna
                        If SerwerHTTP.Ustawienia.FunkcjaZwrotna IsNot Nothing Then SerwerHTTP.Ustawienia.FunkcjaZwrotna(Me)
                    Else
                        funkcja.Przetworz(Me)
                    End If

                    'Jeśli program nie wysłał żadnej odpowiedzi
                    If Not _WyslanoOdpowiedz Then Wyslij500_InternalServerError()

                End If

            End If


            'Zapisanie komunikatu o błędzie
            If SerwerHTTP.Ustawienia.ZapiszBledy Then
                If bledy.JestBlad Then bledy.ZapiszBledyZapytania(UtworzOpisPolaczenia)
            End If

            If Zapytanie.Connection.ToLower = "close" Then Exit Sub
            OstatniaAktywnosc = Now
            Aktywny = False

            If SerwerHTTP.Ustawienia.ZapiszDaneWychodzace Then bwp.Write(utf.GetBytes(vbCrLf & vbCrLf))

        Loop

    End Sub

    Private Sub Czysc()
        Dim t As Byte() = New List(Of Byte)().ToArray()

        _Zapytanie = New NaglowkiZapytania
        _Metoda = MetodaHTTP.Nieznana
        _Adres = ""
        _Wersja = WersjaHTTP.Inna
        _ZmiennePOST = Nothing
        _ZmienneGET = Nothing
        _ZawartoscZapytania = t
        _Odpowiedz = New NaglowkiOdpowiedzi
        _ZawartoscOdpowiedzi = t

        DataSerwera = Now.ToUniversalTime
        _WyslanoOdpowiedz = False
        Aktywny = False
    End Sub

    Private Sub Wyslij(dane As Byte())
        bw.Write(dane)
        If SerwerHTTP.Ustawienia.ZapiszDaneWychodzace Then bwp.Write(dane)
    End Sub

    Private Sub Wyslij(tekst As String)
        Dim b As Byte() = utf.GetBytes(tekst)
        bw.Write(b)
        If SerwerHTTP.Ustawienia.ZapiszDaneWychodzace Then bwp.Write(b)
    End Sub

    Private Sub WyslijLinie(Optional tekst As String = "")
        Dim b As Byte() = utf.GetBytes(tekst & vbCrLf)
        bw.Write(b)
        If SerwerHTTP.Ustawienia.ZapiszDaneWychodzace Then bwp.Write(b)
    End Sub

    Private Function CzytajLinie() As String
        Dim stb As New StringBuilder("")
        Dim z As Char

        Do
            z = br.ReadChar

            If z = vbCr Then Continue Do
            If z = vbLf Then Exit Do
            stb.Append(z)
        Loop

        Dim str As String = stb.ToString
        If SerwerHTTP.Ustawienia.ZapiszDanePrzychodzace Then bwp.Write(utf.GetBytes(stb.Append(vbCrLf).ToString))

        Return str
    End Function

    Private Sub ZapiszOdwiedzinyKlientaHTTP(Optional tekst As String = "")
        If tekst = "" Then tekst = UtworzOpisPolaczenia()
        SerwerHTTP.ZapiszOdwiedzinyDoPliku(tekst)
        ZapisanoOdwiedziny = True
    End Sub

    Private Function UtworzOpisPolaczenia() As String
        Dim stb As New StringBuilder
        stb.Append(Metoda.ToString)
        stb.Append(vbTab)
        stb.Append(Adres)
        stb.Append(vbTab)
        stb.Append(AdresIP)
        stb.Append(vbTab)
        stb.Append(Zapytanie.UserAgent)
        stb.Append(vbTab)
        stb.Append(Now.ToString(DATA_FORMAT))

        If Zapytanie.Referer <> "" Then
            stb.Append(vbTab)
            stb.Append(Zapytanie.Referer)
        End If

        Return stb.ToString
    End Function

    Public Overrides Function ToString() As String
        Return AdresIP
    End Function

    Friend Sub Zamknij()
        Try
            strumien.Close()
        Catch
        End Try
    End Sub

    Private Sub UtworzContentType(Sciezka As String)
        Dim typ As String
        Dim roz As String = Mid(Sciezka, Sciezka.LastIndexOf("."c) + 2).ToLower

        Select Case roz
            Case "html", "htm" : typ = MIME_TEXT_HTML
            Case "css" : typ = MIME_TEXT_CSS
            Case "cpp", "c", "hpp", "h" : typ = MIME_TEXT_XC
            Case "js" : typ = MIME_APPLICATION_JAVASCRIPT
            Case "json" : typ = MIME_APPLICATION_JSON
            Case "xml" : typ = MIME_APPLICATION_XML
            Case "jpg", "jpeg" : typ = MIME_IMAGE_JPEG
            Case "png" : typ = MIME_IMAGE_PNG
            Case "gif" : typ = MIME_IMAGE_GIF
            Case "ico" : typ = MIME_IMAGE_ICON
            Case "txt" : typ = MIME_TEXT_PLAIN
            Case "zip" : typ = MIME_APPLICATION_ZIP
            Case "exe", "dll" : typ = MIME_APPLICATION_EXE
            Case "coffee" : typ = MIME_TEXT_COFFEE
            Case Else
                typ = MIME_APPLICATION_OCTET_STREAM
                If SerwerHTTP.Ustawienia.ZapiszBledy Then bledy.DodajBlad(TypBledu.Typ_pliku, roz)
        End Select

        Odpowiedz.ContentType = typ
    End Sub

    Friend Sub DodajParametryGET(Parametry As ZmiennaHTTP())
        Dim l As New List(Of ZmiennaHTTP)
        If ZmienneGET IsNot Nothing Then l.AddRange(ZmienneGET)
        l.AddRange(Parametry)
        _ZmienneGET = l.ToArray()
    End Sub

End Class