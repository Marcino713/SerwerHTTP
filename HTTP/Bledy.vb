Friend Class ZarzadzanieBledami
    Private _JestBlad As Boolean = False
    Private lista As New List(Of Blad)
    Private stb As New StringBuilder
    Private Sciezka As String

    Friend Sub New(Sciezka As String)
        Me.Sciezka = Sciezka
        If Not Sciezka.EndsWith(Path.DirectorySeparatorChar) Then Sciezka &= Path.DirectorySeparatorChar
    End Sub

    Friend ReadOnly Property JestBlad As Boolean
        Get
            Return _JestBlad
        End Get
    End Property

    Friend Sub DodajBlad(Typ_bledu As TypBledu, OpisBledu As String)
        Dim b As New Blad
        b.Typ = Typ_bledu
        b.Opis = OpisBledu
        lista.Add(b)
        _JestBlad = True
    End Sub

    Friend Sub ZapiszBledyZapytania(DaneZapytania As String)
        If Not _JestBlad Then Exit Sub

        Dim bl As Blad
        Dim e As List(Of Blad).Enumerator = lista.GetEnumerator

        stb.AppendLine(DaneZapytania)

        Do While e.MoveNext
            bl = e.Current

            Select Case bl.Typ
                Case TypBledu.WersjaHTTP : stb.Append("Nieznana wersja HTTP: ")
                Case TypBledu.Metoda : stb.Append("Nieznana metoda: ")
                Case TypBledu.Naglowek : stb.Append("Nieznany nagłówek: ")
                Case TypBledu.Typ_pliku : stb.Append("Nieznany typ pliku: ")
            End Select

            stb.AppendLine(bl.Opis)
        Loop

        stb.AppendLine()
        lista.Clear()
        _JestBlad = False

    End Sub

    Friend Sub ZapiszBledy()
        If stb.Length = 0 Then Exit Sub

        Try
            Dim bajty As Byte() = New UTF8Encoding().GetBytes(stb.ToString)
            Dim fs As New FileStream(Sciezka & "http " & Now.ToString(DATA_LOGI) & ".txt", FileMode.Create)
            fs.Write(bajty, 0, bajty.Length)
            fs.Close()
            fs.Dispose()
        Catch
        End Try

        stb.Clear()
    End Sub

    Private Class Blad
        Friend Typ As TypBledu
        Friend Opis As String
        Overrides Function ToString() As String
            Return Typ.ToString & " " & Opis
        End Function
    End Class
End Class

Friend Enum TypBledu
    WersjaHTTP
    Metoda
    Naglowek
    Typ_pliku
End Enum