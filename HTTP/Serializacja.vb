Imports System.Reflection

Partial Public Class Polaczenie

    Public Sub DeserializujParametry(Of T)(ByRef klasa As T)
        Dim zmienne As New List(Of Zmienna)
        Dim typ As TypeInfo = GetType(T).GetTypeInfo()
        Dim pola As IEnumerable(Of PropertyInfo) = typ.DeclaredProperties
        Dim nazwa As String = ""
        Dim metoda As MetodaHTTP = MetodaHTTP.Nieznana


        'Czytaj zmienne GET i POST
        If ZmienneGET IsNot Nothing Then
            For i As Integer = 0 To ZmienneGET.Length - 1
                zmienne.Add(New Zmienna() With {.Nazwa = ZmienneGET(i).Nazwa.ToLower(), .Wartosc = ZmienneGET(i).Wartosc, .Metoda = MetodaHTTP.GET})
            Next
        End If

        If ZmiennePOST IsNot Nothing Then
            For i As Integer = 0 To ZmiennePOST.Length - 1
                zmienne.Add(New Zmienna() With {.Nazwa = ZmiennePOST(i).Nazwa.ToLower(), .Wartosc = ZmiennePOST(i).Wartosc, .Metoda = MetodaHTTP.POST})
            Next
        End If


        'Czytaj własności w klasie
        For Each p As PropertyInfo In pola

            'Czy da się użyć
            If Not p.CanWrite Then Continue For
            If p.SetMethod Is Nothing Then Continue For
            If Not p.SetMethod.IsPublic Then Continue For

            nazwa = ""
            metoda = MetodaHTTP.Nieznana


            'Sprawdż metodę i ewentualną nazwę parametru
            For i As Integer = 0 To p.CustomAttributes.Count - 1

                If p.CustomAttributes(i).AttributeType.Name = "ParametrGET" Then
                    metoda = MetodaHTTP.GET
                    If p.CustomAttributes(i).ConstructorArguments.Count > 0 Then
                        nazwa = p.CustomAttributes(i).ConstructorArguments(0).Value.ToString()
                    End If
                    Exit For
                End If

                If p.CustomAttributes(i).AttributeType.Name = "ParametrPOST" Then
                    metoda = MetodaHTTP.POST
                    If p.CustomAttributes(i).ConstructorArguments.Count > 0 Then
                        nazwa = p.CustomAttributes(i).ConstructorArguments(0).Value.ToString()
                    End If
                    Exit For
                End If

            Next


            If metoda = MetodaHTTP.Nieznana Then Continue For

            'Nazwa parametru
            If nazwa = "" Then nazwa = p.Name
            nazwa = nazwa.ToLower()


            'Znajdź w zapytaniu zmienną o podanej nazwie
            Dim wartosc As Zmienna = (From z In zmienne Where z.Metoda = metoda AndAlso z.Nazwa = nazwa Select z).FirstOrDefault()
            If wartosc Is Nothing Then Continue For
            Dim wartosc2 As Object = Nothing


            'Konwertuj typ
            Select Case p.PropertyType.FullName
                Case "System.Int32"
                    Dim l As Integer = 0
                    Integer.TryParse(wartosc.Wartosc, l)
                    wartosc2 = l

                Case "System.Boolean"
                    Dim w As String = wartosc.Wartosc.ToLower()
                    If w = "0" OrElse w = "false" Then wartosc2 = False
                    If w = "1" OrElse w = "true" Then wartosc2 = True

                Case "System.DateTime"
                    Dim d As Date
                    Date.TryParse(wartosc.Wartosc, d)
                    wartosc2 = d

                Case "System.TimeSpan"
                    Dim ts As TimeSpan
                    TimeSpan.TryParse(wartosc.Wartosc, ts)
                    wartosc2 = ts

                Case "System.String"
                    wartosc2 = wartosc.Wartosc

            End Select

            If wartosc2 IsNot Nothing Then p.SetValue(klasa, wartosc2)
        Next
    End Sub

    Private Class Zmienna
        Public Nazwa As String = ""
        Public Wartosc As String = ""
        Public Metoda As MetodaHTTP = MetodaHTTP.Nieznana
    End Class

End Class


Public Module Serializacja
    Public MustInherit Class Parametr
        Inherits Attribute
        Public Property Nazwa As String = ""
        Public Sub New()
        End Sub
        Public Sub New(NazwaParametru As String)
            Nazwa = NazwaParametru
        End Sub
    End Class

    Public Class ParametrGET
        Inherits Parametr
        Public Sub New()
        End Sub
        Public Sub New(NazwaParametru As String)
            MyBase.New(NazwaParametru)
        End Sub
    End Class

    Public Class ParametrPOST
        Inherits Parametr
        Public Sub New()
        End Sub
        Public Sub New(NazwaParametru As String)
            MyBase.New(NazwaParametru)
        End Sub
    End Class
End Module