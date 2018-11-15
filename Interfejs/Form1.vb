Imports HTTP

Friend Class wndOkno
    Private Const URUCHOMIONY_TEKST As String = "Uruchomiony"
    Private Const ZATRZYMANY_TEKST As String = "Zatrzymany"

    Private uruchomiony As Boolean = False
    Private serwer_http As Serwer = Nothing

    Public Sub New()
        InitializeComponent()
        Icon = My.Resources.html
        ntfIkona.Icon = My.Resources.html
    End Sub

    Private Sub btnStart_Click() Handles btnStart.Click
        UruchomSerwer()
    End Sub

    Private Sub btnStop_Click() Handles btnStop.Click
        ZatrzymajSerwer()
    End Sub

    Private Sub UruchomSerwer()
        If uruchomiony Then Exit Sub

        Dim ust As New UstawieniaSerwera
        ust.Port = UShort.Parse(txtPort.Text)
        ReDim Preserve ust.PrzetwarzaneRozszerzenia(ust.PrzetwarzaneRozszerzenia.Length)
        ust.PrzetwarzaneRozszerzenia(ust.PrzetwarzaneRozszerzenia.Length - 1) = "about"
        ust.ListaAdresow = {Net.IPAddress.Parse("127.0.0.1")}
        ust.TypBlokowaniaListy = TypBlokowania.Zezwol
        ust.ZapiszDanePrzychodzace = True
        ust.ZapiszDaneWychodzace = True

        ust.FunkcjaZwrotna = AddressOf Funkcja
        'ust.Funkcje = {
        '    New DaneFunkcji(Of TestoweZapytanie)("/Test", MetodaHTTP.GET, Nothing, AddressOf Funkcja2),
        '    New DaneFunkcji(Of Zap2)("/Abc", MetodaHTTP.GET, {"Rok", "Miesiac"}, AddressOf Funkcja3),
        '    New DaneFunkcji(Of Object)("/api", MetodaHTTP.GET, Nothing, AddressOf f1),
        '    New DaneFunkcji(Of Zap3)("/api/test*", MetodaHTTP.GET, {"Liczba"}, AddressOf f2)
        '}

        serwer_http = New Serwer(ust)
        serwer_http.Uruchom()

        uruchomiony = True
        lblStan.Text = URUCHOMIONY_TEKST
        mnuStan.Text = URUCHOMIONY_TEKST
        mnuStan.BackColor = Color.SpringGreen
        pnlStan.BackColor = Color.SpringGreen
    End Sub

    Private Sub ZatrzymajSerwer()
        If Not uruchomiony Then Exit Sub

        serwer_http.Zatrzymaj()
        serwer_http = Nothing

        uruchomiony = False
        lblStan.Text = ZATRZYMANY_TEKST
        mnuStan.Text = ZATRZYMANY_TEKST
        mnuStan.BackColor = Color.Red
        pnlStan.BackColor = Color.Red
    End Sub

    Private Sub Funkcja(pol As Polaczenie)
        'If pol.ZmienneGET IsNot Nothing Then
        'Dim test As New TestoweZapytanie

        'CzytajArgumenty(pol, test)
        'test.Koniec += 1
        'End If


        'If pol.Zapytanie.Authorization Is Nothing Then
        '    pol.Wyslij401_Unauthorized("CzlowiekuKimTyJestes?", MetodaUwierzytelniania.Zlozona)
        'Else
        '    If pol.Zapytanie.Authorization.CzyZgodneHaslo("bb7") AndAlso pol.Zapytanie.Authorization.Metoda = MetodaUwierzytelniania.Zlozona Then
        '        If Not pol.WyslijFolder Then pol.WyslijPlik(True)
        '    Else
        '        pol.Wyslij401_Unauthorized("CzlowiekuKimTyJestes?", MetodaUwierzytelniania.Zlozona)
        '    End If

        'End If

        If Not pol.WyslijFolder Then pol.WyslijPlik(True)
    End Sub

    Private Sub Funkcja2(pol As Polaczenie, dane As TestoweZapytanie)

    End Sub

    Private Sub Funkcja3(pol As Polaczenie, dane As Zap2)

    End Sub

    Private Sub f1(pol As Polaczenie, dane As Object)

    End Sub

    Private Sub f2(pol As Polaczenie, dane As Zap3)

    End Sub

    Private Sub wndOkno_FormClosed() Handles Me.FormClosed
        ZatrzymajSerwer()
    End Sub

    Private Sub wndOkno_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Visible = False
        End If
    End Sub

    Private Sub mnuOtworzOkno_Click() Handles mnuOtworzOkno.Click
        If Not Visible Then Visible = True
    End Sub

    Private Sub mnuZamknijProgram_Click() Handles mnuZamknijProgram.Click
        Application.Exit()
    End Sub

    Private Sub mnuStan_Click() Handles mnuStan.Click
        If uruchomiony Then ZatrzymajSerwer() Else UruchomSerwer()
    End Sub



End Class

Public Class TestowaKlasa
    <ParametrGET("a")>
    Public Property pole1 As Integer
    <ParametrGET>
    Public Property pole2 As String
    Public ReadOnly Property pole3 As Boolean
    <ParametrGET>
    Private Property pole4 As Integer
    <ParametrPOST>
    Public Property Pole5 As Integer

    <ParametrGET>
    Public Property Pole10 As Integer
    <ParametrGET>
    Public Property Pole11 As Boolean
    <ParametrGET>
    Public Property Pole12 As Date
    <ParametrGET>
    Public Property Pole13 As String
End Class





Public Class TestoweZapytanie
    <ParametrGET> Public Property Poczatek As Integer
    <ParametrGET> Public Property Koniec As Integer
    <ParametrGET> Public Property Data As Date
    <ParametrGET("Teraz")> Public Property Czas As TimeSpan
    <ParametrGET> Public Property Tak As Boolean
    <ParametrGET> Public Property Nie As Boolean
End Class

Public Class Zap2
    <ParametrGET> Public Property Rok As Integer
    <ParametrGET> Public Property Miesiac As Integer
End Class

Public Class Zap3
    <ParametrGET> Public Property Liczba As Integer
End Class