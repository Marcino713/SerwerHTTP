Partial Public Class Polaczenie

    Public Sub WyslijPlik(UwzglednijIfModifiedSince As Boolean)
        Dim Sciezka As String = Path.GetFullPath(SerwerHTTP.Ustawienia.FolderSerwisu & Adres.Replace("/"c, Path.DirectorySeparatorChar))

        If Sciezka.StartsWith(SerwerHTTP.Ustawienia.FolderSerwisu) Then
            If Directory.Exists(Sciezka) Then
                If Sciezka.EndsWith(Path.DirectorySeparatorChar) Then Sciezka &= "index.html" Else Sciezka &= Path.DirectorySeparatorChar & "index.html"
            End If

            WyslijPlik(Sciezka, True)
        Else
            Wyslij403_Forbidden()
        End If
    End Sub

    Public Sub WyslijPlik(Sciezka As String, UwzglednijIfModifiedSince As Boolean)
        Dim fi As FileInfo

        If File.Exists(Sciezka) Then

            fi = New FileInfo(Sciezka)
            If UwzglednijIfModifiedSince Then Odpowiedz.LastModified = SprzatnijDate(fi.LastWriteTimeUtc)

            'Czy plik był modyfikowany
            If UwzglednijIfModifiedSince AndAlso (Odpowiedz.LastModified <= SprzatnijDate(Zapytanie.IfModifiedSince)) Then
                Wyslij304_NotModified()
            Else

                UtworzContentType(Sciezka)
                If _Metoda = MetodaHTTP.HEAD Then
                    _Odpowiedz.Length = CInt(fi.Length)
                Else
                    _ZawartoscOdpowiedzi = File.ReadAllBytes(Sciezka)
                End If

                Wyslij200_OK()

            End If

        Else
            Wyslij404_NotFound()
        End If
    End Sub

    Public Function WyslijFolder() As Boolean
        Dim Sciezka As String = Path.GetFullPath(SerwerHTTP.Ustawienia.FolderSerwisu & Adres.Replace("/"c, Path.DirectorySeparatorChar))
        If Not Directory.Exists(Sciezka) Then Return False
        If Not Sciezka.StartsWith(SerwerHTTP.Ustawienia.FolderSerwisu) Then Return False

        Dim Nazwy As String() = Directory.GetDirectories(Sciezka)
        Dim Zawartosc As New List(Of DanePliku)

        Zawartosc.Add(New DanePliku With {.Nazwa = "./", .Rozmiar = 0, .Data = Directory.GetLastWriteTime(Sciezka)})
        If Sciezka <> SerwerHTTP.Ustawienia.FolderSerwisu Then
            Zawartosc.Add(New DanePliku With {.Nazwa = "../", .Rozmiar = 0, .Data = Directory.GetLastWriteTime(Path.GetFullPath(Sciezka & ".."))})
        End If

        Dim i As Integer
        For i = 0 To Nazwy.Length - 1
            Dim dp As New DanePliku
            dp.Nazwa = Path.GetFileName(Nazwy(i)) & "/"
            dp.Rozmiar = 0
            dp.Data = Directory.GetLastWriteTime(Nazwy(i))
            Zawartosc.Add(dp)
        Next

        Nazwy = Directory.GetFiles(Sciezka)
        For i = 0 To Nazwy.Length - 1
            Dim dp As New DanePliku
            Dim fi As New FileInfo(Nazwy(i))

            dp.Nazwa = Path.GetFileName(Nazwy(i))
            dp.Rozmiar = fi.Length
            dp.Data = fi.LastWriteTime
            Zawartosc.Add(dp)
        Next


        Dim sth As String = New UTF8Encoding().GetString(My.Resources.Folder)
        Dim t As New StringBuilder
        Dim zaw As DanePliku() = Zawartosc.ToArray()

        i = 0
        Dim id As String = ""
        Dim ix As Integer = 0
        Dim petla As Integer = -1

        While i < sth.Length

            If sth(i) = "#" Then
                If i + 3 < sth.Length Then
                    id = sth(i + 2).ToString() & sth(i + 3).ToString()
                Else
                    t.Append(sth(i))
                    i += 1
                    Continue While
                End If


                Select Case sth(i + 1)
                    Case "A"c    'Zmienna globalna

                        Select Case id
                            Case "01"
                                t.Append(Adres)

                        End Select

                    Case "B"c   'Petla

                        Select Case id
                            Case "01" 'Poczatek
                                petla = i
                                ix = 0

                            Case "02" 'Koniec
                                If petla > -1 Then
                                    ix += 1
                                    If ix < zaw.Length Then i = petla Else ix = 0
                                End If
                        End Select

                    Case "C"c    'Zmienna petlowa

                        If ix < zaw.Length AndAlso ix > -1 Then
                            Select Case id

                                Case "01"
                                    t.Append(zaw(ix).Nazwa)

                                Case "02"
                                    t.Append(RozmiarToString(zaw(ix).Rozmiar))

                                Case "03"
                                    t.Append(zaw(ix).Data.ToString(DATA_FOLDER))


                            End Select
                        End If

                End Select

                i += 3
            Else
                t.Append(sth(i))
            End If

            i += 1

        End While

        Dim b As Byte() = New UTF8Encoding().GetBytes(t.ToString())
        _Odpowiedz.ContentType = MIME_TEXT_HTML
        _Odpowiedz.Length = b.Length
        _ZawartoscOdpowiedzi = b
        Wyslij200_OK()

        Return True
    End Function

    Public Sub Wyslij200_OK()
        If _WyslanoOdpowiedz Then Exit Sub
        _WyslanoOdpowiedz = True

        WyslijPoczatek("200 OK")
        If Odpowiedz.LastModified <> Date.MinValue AndAlso Odpowiedz.LastModified < DataSerwera Then WyslijLinie("Last-Modified: " & Odpowiedz.LastModified.ToString("r"))
        WyslijContentType()

        If Metoda = MetodaHTTP.HEAD Then
            WyslijLinie("Content-Length: " & Odpowiedz.Length)
            WyslijLinie()
        Else
            WyslijLinie("Content-Length: " & ZawartoscOdpowiedzi.Length.ToString)
            WyslijLinie()
            Wyslij(ZawartoscOdpowiedzi)
        End If

    End Sub

    Public Sub Wyslij201_Created()
        If _WyslanoOdpowiedz Then Exit Sub
        _WyslanoOdpowiedz = True

        WyslijPoczatek("201 Created")
        WyslijContentType()
        WyslijLinie("Content-Length: " & ZawartoscOdpowiedzi.Length.ToString)
        WyslijLinie()

        Wyslij(ZawartoscOdpowiedzi)
    End Sub

    Public Sub Wyslij204_NoContent()
        If _WyslanoOdpowiedz Then Exit Sub
        _WyslanoOdpowiedz = True

        WyslijPoczatek("204 No Content")
        WyslijLinie()
    End Sub

    Public Sub Wyslij304_NotModified()
        If _WyslanoOdpowiedz Then Exit Sub
        _WyslanoOdpowiedz = True

        WyslijPoczatek("304 Not Modified")
        WyslijLinie()
    End Sub

    Public Sub Wyslij400_BadRequest()
        WyslijOpisBleduHTTP("400 Bad Request", "400.html")
    End Sub

    Public Sub Wyslij401_Unauthorized(Komunikat As String, Metoda As MetodaUwierzytelniania)
        If _WyslanoOdpowiedz Then Exit Sub
        _WyslanoOdpowiedz = True

        Zapytanie.Connection = "close"
        WyslijPoczatek("401 Unauthorized")

        Select Case Metoda
            Case MetodaUwierzytelniania.Prosta
                WyslijLinie("WWW-Authenticate: Basic realm=""" & Komunikat & """, charset=""UTF-8""")
            Case MetodaUwierzytelniania.Zlozona
                Dim rnd As New Random()
                Dim nonce As String = ObliczMD5(Now.Millisecond.ToString() & rnd.Next().ToString)
                Dim opaque As String = ObliczMD5(Now.Millisecond.ToString() & rnd.Next().ToString)

                WyslijLinie("WWW-Authenticate: Digest realm=""" & Komunikat & """, charset=""UTF-8"", nonce=""" & nonce & """, opaque=""" & opaque & """")
        End Select

        WyslijLinie()
    End Sub

    Public Sub Wyslij402_PaymentRequired()
        WyslijOpisBleduHTTP("402 Payment Required", "402.html")
    End Sub

    Public Sub Wyslij403_Forbidden()
        WyslijOpisBleduHTTP("403 Forbidden", "403.html")
    End Sub

    Public Sub Wyslij404_NotFound()
        WyslijOpisBleduHTTP("404 Not Found", "404.html")
    End Sub

    Private Sub Wyslij418_ImATeapot()
        If _WyslanoOdpowiedz Then Exit Sub
        _WyslanoOdpowiedz = True

        Dim b As Byte() = My.Resources.Czajniczek

        WyslijPoczatek("418 I'm a teapot")
        WyslijContentType(MIME_IMAGE_PNG)
        WyslijLinie("Content-Length: " & b.Length.ToString)
        WyslijLinie()

        If Metoda <> MetodaHTTP.HEAD Then Wyslij(b)
    End Sub

    Public Sub Wyslij500_InternalServerError()
        WyslijOpisBleduHTTP("500 Internal Server Error", "500.html")
    End Sub

    Public Sub Wyslij501_NotImplemented()
        WyslijOpisBleduHTTP("501 Not Implemented", "501.html")
    End Sub

    Public Sub Wyslij505_HTTPVersionNotSupported()
        WyslijOpisBleduHTTP("505 HTTP Version Not Supported", "505.html")
    End Sub

    Private Sub WyslijPoczatek(KodOdp As String)
        Wyslij(HTTP_WERSJA_1_1 & " ")

        WyslijLinie(KodOdp)

        WyslijLinie("Date: " & DataSerwera.ToString("r"))
        WyslijLinie("Server: " & SerwerHTTP.SERWER_NAZWA)

        If Odpowiedz.Allow <> 0 Then
            Dim lst As New List(Of String)

            If (Odpowiedz.Allow And MetodaHTTP.GET) <> 0 Then lst.Add("GET")
            If (Odpowiedz.Allow And MetodaHTTP.HEAD) <> 0 Then lst.Add("HEAD")
            If (Odpowiedz.Allow And MetodaHTTP.POST) <> 0 Then lst.Add("POST")
            If (Odpowiedz.Allow And MetodaHTTP.PUT) <> 0 Then lst.Add("PUT")
            If (Odpowiedz.Allow And MetodaHTTP.DELETE) <> 0 Then lst.Add("DELETE")

            WyslijLinie("Allow: " & Join(lst.ToArray(), ", "))
        End If

        If Zapytanie.Connection.ToLower = "keep-alive" Then
            WyslijLinie("Connection: keep-alive")
        Else
            WyslijLinie("Connection: close")
            Zapytanie.Connection = "close"
        End If

        WyslijLinie("Accept-Ranges: none")
        WyslijCiasteczka()
    End Sub

    Private Sub WyslijCiasteczka()
        Dim stb As New StringBuilder
        Dim it As List(Of Ciasteczko).Enumerator = Odpowiedz.Ciasteczka.GetEnumerator

        While it.MoveNext
            If (it.Current.Nazwa = "" OrElse it.Current.Wartosc = "") Then Continue While

            stb.Append("Set-Cookie: ")
            stb.Append(WebUtility.UrlEncode(it.Current.Nazwa))
            stb.Append("=")
            stb.Append(WebUtility.UrlEncode(it.Current.Wartosc))

            If it.Current.Wygasa <> Date.MinValue Then
                stb.Append("; expires=")
                stb.Append(it.Current.Wygasa.ToUniversalTime.ToString("r"))
            End If

            If it.Current.Sciezka <> "" Then
                stb.Append("; path=")
                stb.Append(WebUtility.UrlEncode(it.Current.Sciezka).Replace("%2f", "/").Replace("%2F", "/"))
            End If

            stb.AppendLine()
        End While

        Wyslij(stb.ToString)
    End Sub

    Private Sub WyslijContentType(Optional Typ As String = "")
        Wyslij("Content-Type: ")

        If Typ = "" Then
            WyslijLinie(Odpowiedz.ContentType)
        Else
            WyslijLinie(Typ)
        End If

    End Sub

    ''' <summary>
    ''' Otwiera plik o podanej ścieżce i wysyła jego zawartość. Jeśli plik nie istnieje, wysyła parametr Opis.
    ''' </summary>
    Private Sub WyslijOpisBleduHTTP(Kod As String, Plik As String, Optional Opis As String = "")
        If _WyslanoOdpowiedz Then Exit Sub
        _WyslanoOdpowiedz = True

        Dim b(0) As Byte
        Dim dl As Long = 0
        Plik = SerwerHTTP.Ustawienia.FolderOpisowBledow & Plik

        If File.Exists(Plik) Then

            If Metoda = MetodaHTTP.HEAD Then

                Dim fi As New FileInfo(Plik)
                dl = fi.Length

            Else

                Dim fs As New FileStream(Plik, FileMode.Open, FileAccess.Read)
                dl = fs.Length
                ReDim b(Convert.ToInt32(fs.Length - 1))
                fs.Read(b, 0, b.Length)
                fs.Close()
                fs.Dispose()

            End If

        Else

            If Opis = "" Then Opis = "<h1>" & Kod & "</h1>"
            b = utf.GetBytes(Opis)
            dl = b.Length

        End If

        WyslijPoczatek(Kod)
        WyslijContentType(MIME_TEXT_HTML)
        WyslijLinie("Content-Length: " & dl)
        WyslijLinie()

        If Metoda <> MetodaHTTP.HEAD Then Wyslij(b)
    End Sub

End Class