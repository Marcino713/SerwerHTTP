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
        ReDim Preserve ust.PrzetwarzaneRozszerzenia(0)
        ust.PrzetwarzaneRozszerzenia(0) = "about"
        ust.ListaAdresow = {Net.IPAddress.Parse("127.0.0.1")}
        ust.TypBlokowaniaListy = TypBlokowania.Zezwol
        ust.ZapiszDanePrzychodzace = True
        ust.ZapiszDaneWychodzace = True

        ust.FunkcjaZwrotna = AddressOf Funkcja

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
        If pol.Adres.StartsWith("/obr") Then
            If pol.Zapytanie.Authorization Is Nothing Then
                pol.Wyslij401_Unauthorized("CzlowiekuKimTyJestes?", MetodaUwierzytelniania.Zlozona)
            Else
                If pol.Zapytanie.Authorization.CzyZgodneHaslo("TajneHaslo5") AndAlso pol.Zapytanie.Authorization.Metoda = MetodaUwierzytelniania.Zlozona Then
                    If Not pol.WyslijFolder Then pol.WyslijPlik(True)
                Else
                    pol.Wyslij401_Unauthorized("CzlowiekuKimTyJestes?", MetodaUwierzytelniania.Zlozona)
                End If

            End If
        Else
            If pol.Adres = "/" Then
                pol.WyslijPlik(True)
            Else
                If Not pol.WyslijFolder Then pol.WyslijPlik(True)
            End If
        End If
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