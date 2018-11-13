Imports System.Threading
Imports System.Net.Sockets
Imports HTTP

Public Class Serwer
    Friend ReadOnly SERWER_NAZWA As String = "Serwer HTTP 1.1 im. Marcina Orczyka"
    Private ReadOnly ODWIEDZINY_PLIK As String
    Private Const CZAS_BEZCZYNNOSCI As Integer = 10
    Private Const CZEKAJ_ZAMYKAJ_KLIENTOW As Integer = 3000
    Private Const MAX_ILOSC_WPISOW As Integer = 5

    Friend Ustawienia As UstawieniaSerwera
    Private Polaczenia As List(Of Polaczenie) = Nothing
    Private slock_lista As New Object
    Private WatekPrzyjmujKlientow As Thread
    Private WatekZamykajKlientow As Thread
    Private ZamknijPrzyjmujKlientow As Boolean = False
    Private ZamknijZamykajKlientow As Boolean = False
    Private Serwer As TcpListener
    Private Uruchomiony As Boolean = False
    Private Odwiedziny As FileStream
    Private OdwBW As BinaryWriter
    Private IloscWpisow As Integer = 0
    Private slock_odwiedziny As New Object

    Public ReadOnly Property CzyUruchomiony As Boolean
        Get
            Return Uruchomiony
        End Get
    End Property

    Public Sub New(Ustawienia As UstawieniaSerwera)
        If Not Ustawienia.FolderSerwisu.EndsWith(Path.DirectorySeparatorChar) Then Ustawienia.FolderSerwisu &= Path.DirectorySeparatorChar
        If Not Ustawienia.FolderSerwera.EndsWith(Path.DirectorySeparatorChar) Then Ustawienia.FolderSerwera &= Path.DirectorySeparatorChar
        If Not Ustawienia.FolderOpisowBledow.EndsWith(Path.DirectorySeparatorChar) Then Ustawienia.FolderOpisowBledow &= Path.DirectorySeparatorChar
        If Not Directory.Exists(Ustawienia.FolderSerwisu) Then Directory.CreateDirectory(Ustawienia.FolderSerwisu)
        If Not Directory.Exists(Ustawienia.FolderSerwera) Then Directory.CreateDirectory(Ustawienia.FolderSerwera)
        If Not Directory.Exists(Ustawienia.FolderOpisowBledow) Then Directory.CreateDirectory(Ustawienia.FolderOpisowBledow)

        Me.Ustawienia = Ustawienia
        If Ustawienia.OpisSerwera <> "" Then SERWER_NAZWA &= " " & Ustawienia.OpisSerwera
        ODWIEDZINY_PLIK = Ustawienia.FolderSerwera & "Połączenia.txt"
    End Sub

    Public Sub Uruchom()
        If Uruchomiony Then Exit Sub

        SyncLock slock_lista
            Polaczenia = New List(Of Polaczenie)
        End SyncLock

        Serwer = New TcpListener(IPAddress.Any, Ustawienia.Port)
        Serwer.Start()
        OtworzOdwiedziny()

        ZamknijPrzyjmujKlientow = False
        ZamknijZamykajKlientow = False

        WatekPrzyjmujKlientow = New Thread(AddressOf PrzyjmujKlientow)
        WatekPrzyjmujKlientow.Name = "Akceptacja polaczen"
        WatekPrzyjmujKlientow.Start()

        WatekZamykajKlientow = New Thread(AddressOf ZamknijNieaktywnychKlientow)
        WatekZamykajKlientow.Name = "Zamykanie polaczen"
        WatekZamykajKlientow.Start()

        Uruchomiony = True
    End Sub

    Public Sub Zatrzymaj()
        If Not Uruchomiony Then Exit Sub

        Serwer.Stop()
        ZamknijPrzyjmujKlientow = True
        ZamknijZamykajKlientow = True

        SyncLock slock_lista
            Dim it As List(Of Polaczenie).Enumerator = Polaczenia.GetEnumerator

            Do While it.MoveNext
                it.Current.Usuniety = True
                it.Current.Zamknij()
            Loop

            Polaczenia = Nothing
        End SyncLock

        ZamknijOdwiedziny()
        Uruchomiony = False
    End Sub

    Private Sub PrzyjmujKlientow()
        Dim p As Polaczenie

        Do Until ZamknijPrzyjmujKlientow
            Try
                p = New Polaczenie(Serwer.AcceptTcpClient, Me)
            Catch
                Exit Do
            End Try

            SyncLock slock_lista
                If Polaczenia IsNot Nothing Then Polaczenia.Add(p)
            End SyncLock
        Loop

    End Sub

    Private Sub ZamknijNieaktywnychKlientow()
        Dim czas As Date

        Do Until ZamknijZamykajKlientow
            czas = Now.AddSeconds(-CZAS_BEZCZYNNOSCI)

            SyncLock slock_lista

                If Polaczenia IsNot Nothing Then
                    Dim nieaktywne As Polaczenie() = (From p In Polaczenia Where Not p.Aktywny AndAlso p.OstatniaAktywnosc < czas Select p).ToArray()
                    For i As Integer = 0 To nieaktywne.Length - 1
                        nieaktywne(i).Usuniety = True
                        nieaktywne(i).Zamknij()
                        Polaczenia.Remove(nieaktywne(i))
                    Next
                End If

            End SyncLock

            Thread.Sleep(CZEKAJ_ZAMYKAJ_KLIENTOW)
        Loop

    End Sub

    Friend Sub UsunKlienta(Klient As Polaczenie)
        If Not Klient.Usuniety Then
            SyncLock slock_lista
                If Polaczenia IsNot Nothing Then Polaczenia.Remove(Klient)
            End SyncLock
            Klient.Usuniety = True
        End If
    End Sub

    Friend Sub OtworzOdwiedziny()
        If Not Ustawienia.ZapiszOdwiedziny Then Exit Sub

        SyncLock slock_odwiedziny
            Odwiedziny = New FileStream(ODWIEDZINY_PLIK, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
            OdwBW = New BinaryWriter(Odwiedziny)
        End SyncLock

    End Sub

    Friend Sub ZamknijOdwiedziny()
        If Not Ustawienia.ZapiszOdwiedziny Then Exit Sub

        SyncLock slock_odwiedziny
            Try
                OdwBW.Close()
                Odwiedziny.Close()
                OdwBW.Dispose()
                Odwiedziny.Dispose()
            Catch
            End Try
        End SyncLock

    End Sub

    Friend Sub ZapiszOdwiedzinyDoPliku(TekstOdwiedzin As String)
        If Not Ustawienia.ZapiszOdwiedziny Then Exit Sub

        SyncLock slock_odwiedziny
            Try
                OdwBW.Write(New UTF8Encoding().GetBytes(TekstOdwiedzin & vbCrLf))
                IloscWpisow += 1

                If IloscWpisow >= MAX_ILOSC_WPISOW Then
                    IloscWpisow = 0
                    OdwBW.Flush()
                End If

            Catch
            End Try
        End SyncLock

    End Sub

End Class

Public Delegate Sub PrzetwarzanePolaczenie(DanePolaczenia As Polaczenie)

Public Class UstawieniaSerwera
    Public Port As UShort = 80
    Public FolderSerwisu As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & Path.DirectorySeparatorChar & "HTTP" & Path.DirectorySeparatorChar
    Public FolderSerwera As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & Path.DirectorySeparatorChar & "HTTPDane" & Path.DirectorySeparatorChar
    Public FolderOpisowBledow As String = FolderSerwisu & "bl" & Path.DirectorySeparatorChar
    Public PrzetwarzaneRozszerzenia As String() = {".css", ".js", ".png", ".jpg", ".jpeg", ".gif"}
    Public FunkcjaZwrotna As PrzetwarzanePolaczenie = Nothing
    Public Funkcje As IDaneFunkcji()
    Public ZapiszBledy As Boolean = True
    Public ZapiszOdwiedziny As Boolean = True
    Public OpisSerwera As String = ""
End Class


Public Interface IDaneFunkcji
    Sub Przetworz(DanePolaczenia As Polaczenie)
    ReadOnly Property Adres As String
    ReadOnly Property Metoda As MetodaHTTP
    ReadOnly Property DokladnyAdres As Boolean
End Interface


Public Class DaneFunkcji(Of T As New)
    Implements IDaneFunkcji
    Public Delegate Sub PrzetwarzanePolaczenie(DanePolaczenia As Polaczenie, Parametry As T)

    Public ReadOnly Property Adres As String Implements IDaneFunkcji.Adres
    Public ReadOnly Property Metoda As MetodaHTTP Implements IDaneFunkcji.Metoda
    Public ReadOnly Property DokladnyAdres As Boolean Implements IDaneFunkcji.DokladnyAdres

    Private NazwyParametrow As String() = Nothing
    Private Funkcja As PrzetwarzanePolaczenie

    Public Sub New(Adres As String, Metoda As MetodaHTTP, Parametry As String(), Funkcja As PrzetwarzanePolaczenie)
        DokladnyAdres = True
        If Adres.EndsWith("*") Then
            Adres = Adres.Substring(0, Adres.Length - 1)
            DokladnyAdres = False
        End If
        Me.Adres = Adres
        Me.Metoda = Metoda
        NazwyParametrow = Parametry
        Me.Funkcja = Funkcja
    End Sub

    Public Sub Przetworz(DanePolaczenia As Polaczenie) Implements IDaneFunkcji.Przetworz

        'Czytaj dodatkowe parametry GET
        If NazwyParametrow IsNot Nothing Then
            Dim adr As String = DanePolaczenia.Adres.Substring(Adres.Length).Replace("\", "/")
            Dim param As String() = adr.Split({"/"c}, StringSplitOptions.RemoveEmptyEntries)
            Dim ile As Integer = Math.Min(NazwyParametrow.Length, param.Length)

            If ile > 0 Then
                Dim zmienne(ile - 1) As ZmiennaHTTP
                For i As Integer = 0 To ile - 1
                    zmienne(i) = New ZmiennaHTTP(NazwyParametrow(i), param(i))
                Next
                DanePolaczenia.DodajParametryGET(zmienne)
            End If
        End If

        'Wywołaj funkcję
        Dim dane As New T
        DanePolaczenia.DeserializujParametry(dane)
        Funkcja(DanePolaczenia, dane)
    End Sub
End Class