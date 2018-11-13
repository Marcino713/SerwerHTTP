Partial Public Class Polaczenie

    Private Sub PrzetworzPierwszaLinie()
        Dim tekst As String
        Dim adres As String()
        Dim pozycja As Integer
        Dim zmienna_http As String()
        Dim tekst_tablica As String()

        tekst = CzytajLinie()
        Aktywny = True
        adres = tekst.Split(" "c)

        'Czy linia składa się z trzech elementów:
        'Request-Line = Method SP Request-URI SP HTTP-Version CRLF
        If adres.Length <> 3 Then
            If SerwerHTTP.Ustawienia.ZapiszOdwiedziny Then ZapiszOdwiedzinyKlientaHTTP("----" & vbTab & AdresIP & vbTab & "Błędne zapytanie: " & tekst & vbTab & Now.ToString(DATA_FORMAT))
            Wyslij400_BadRequest()
        End If

        'Wersja HTTP
        Select Case adres(2).ToUpper
            Case HTTP_WERSJA_1_1
                _Wersja = WersjaHTTP.HTTP_1_1

            Case Else
                _Wersja = WersjaHTTP.Inna
                If SerwerHTTP.Ustawienia.ZapiszOdwiedziny Then ZapiszOdwiedzinyKlientaHTTP(adres(0).ToUpper & vbTab & AdresIP & vbTab & "Błędna wersja, zapytanie: " & tekst & vbTab & Now.ToString(DATA_FORMAT))
                If SerwerHTTP.Ustawienia.ZapiszBledy Then bledy.DodajBlad(TypBledu.WersjaHTTP, adres(2))
                Wyslij505_HTTPVersionNotSupported()
        End Select

        'Metoda
        Select Case adres(0).ToUpper
            Case "GET" : _Metoda = MetodaHTTP.[GET]
            Case "HEAD" : _Metoda = MetodaHTTP.HEAD
            Case "POST" : _Metoda = MetodaHTTP.POST
            Case "PUT" : _Metoda = MetodaHTTP.PUT
            Case "DELETE" : _Metoda = MetodaHTTP.DELETE
            Case Else
                If SerwerHTTP.Ustawienia.ZapiszOdwiedziny Then ZapiszOdwiedzinyKlientaHTTP(adres(0).ToUpper & vbTab & AdresIP & vbTab & "Błędna metoda, zapytanie: " & tekst & vbTab & Now.ToString(DATA_FORMAT))
                _Metoda = MetodaHTTP.Nieznana
                If SerwerHTTP.Ustawienia.ZapiszBledy Then bledy.DodajBlad(TypBledu.Metoda, adres(0))
                Wyslij501_NotImplemented()
        End Select

        'Adres i parametry GET
        pozycja = adres(1).IndexOf("?"c)

        If pozycja = -1 Then    'Brak danych GET

            _Adres = WebUtility.UrlDecode(adres(1))

        Else    'Są dane do przetworzenia - GET

            _Adres = WebUtility.UrlDecode(Mid(adres(1), 1, pozycja))
            tekst = Mid(adres(1), pozycja + 2)
            zmienna_http = tekst.Split("&"c)
            ReDim _ZmienneGET(zmienna_http.Length - 1)

            For i As Integer = 0 To zmienna_http.Length - 1
                tekst_tablica = zmienna_http(i).Split("="c)
                _ZmienneGET(i) = New ZmiennaHTTP(WebUtility.UrlDecode(tekst_tablica(0)), WebUtility.UrlDecode(tekst_tablica(1)))
            Next

        End If

    End Sub

    Private Sub PrzetworzNaglowki()
        Dim tekst As String
        Dim naglowek As String
        Dim wartosc As String
        Dim pozycja As Integer
        Dim wyslij501 As Boolean = False

        Do

            tekst = CzytajLinie()
            If tekst = "" Then Exit Do  'Koniec nagłówków

            'Zapisanie nagłówków
            pozycja = tekst.IndexOf(":"c)
            naglowek = Mid(tekst, 1, pozycja).ToLower
            wartosc = WebUtility.UrlDecode(Mid(tekst, pozycja + 2).Trim)

            Select Case naglowek
                Case "host" : Zapytanie.Host = wartosc
                Case "user-agent" : Zapytanie.UserAgent = wartosc
                Case "accept" : Zapytanie.Accept = wartosc
                Case "accept-language" : Zapytanie.AcceptLanguage = wartosc
                Case "accept-encoding" : Zapytanie.AcceptEncoding = wartosc
                Case "connection" : Zapytanie.Connection = wartosc
                Case "if-modified-since"
                    Dim d As Date
                    If Date.TryParse(wartosc, d) Then Zapytanie.IfModifiedSince = d.ToUniversalTime
                Case "cache-control" : Zapytanie.CacheControl = wartosc
                Case "referer" : Zapytanie.Referer = wartosc
                Case "content-length"
                    Dim l As Integer
                    If Integer.TryParse(wartosc, l) Then Zapytanie.ContentLength = l
                Case "content-type" : Zapytanie.ContentType = wartosc
                Case "dnt" : Zapytanie.Dnt = wartosc
                Case "ua-cpu" : Zapytanie.UaCpu = wartosc
                Case "range" : Zapytanie.Range = wartosc
                Case "upgrade-insecure-requests" : Zapytanie.UpgradeInsecureRequests = wartosc
                Case "pragma" : Zapytanie.Pragma = wartosc
                Case "x-requested-with" : Zapytanie.XRequestedWith = wartosc
                Case "cookie"

                    Dim zmienna_http As String() = Mid(tekst, pozycja + 2).Trim.Split({"; "}, StringSplitOptions.None)
                    Dim ciasteczka As New List(Of ZmiennaHTTP)
                    Dim tekst_tablica As String()

                    For i As Integer = 0 To zmienna_http.Length - 1
                        tekst_tablica = zmienna_http(i).Split("="c)
                        If tekst_tablica.Length > 1 Then ciasteczka.Add(New ZmiennaHTTP(WebUtility.UrlDecode(tekst_tablica(0)), WebUtility.UrlDecode(tekst_tablica(1))))
                    Next

                    Zapytanie.Cookie = ciasteczka.ToArray()

                Case Else
                    If SerwerHTTP.Ustawienia.ZapiszBledy Then bledy.DodajBlad(TypBledu.Naglowek, tekst)
                    If naglowek.StartsWith("content-") Then wyslij501 = True

            End Select

        Loop

        If wyslij501 And Metoda = MetodaHTTP.PUT Then Wyslij501_NotImplemented()

    End Sub

    Private Sub PrzetworzOdebraneDane()
        Dim zmienna_http As String()
        Dim tekst_tablica As String()
        Dim pozycja As Integer
        Dim zmienne As New List(Of ZmiennaHTTP)

        Select Case Zapytanie.ContentType.ToLower

            Case MIME_APPLICATION_X_WWW_FORM_URLENCODED

                zmienna_http = utf.GetString(ZawartoscZapytania).Split("&"c)

                For i As Integer = 0 To zmienna_http.Length - 1
                    tekst_tablica = zmienna_http(i).Split("="c)
                    If tekst_tablica.Length > 1 Then zmienne.Add(New ZmiennaHTTP(WebUtility.UrlDecode(tekst_tablica(0)), WebUtility.UrlDecode(tekst_tablica(1))))
                Next

                _ZmiennePOST = zmienne.ToArray()

            Case MIME_TEXT_PLAIN

                zmienna_http = utf.GetString(ZawartoscZapytania).Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)

                For i As Integer = 0 To zmienna_http.Length - 1
                    pozycja = zmienna_http(i).IndexOf("="c)
                    If pozycja <> -1 Then zmienne.Add(New ZmiennaHTTP(WebUtility.UrlDecode(Mid(zmienna_http(i), 1, pozycja)), WebUtility.UrlDecode(Mid(zmienna_http(i), pozycja + 2))))
                Next

                _ZmiennePOST = zmienne.ToArray()

        End Select

    End Sub

End Class