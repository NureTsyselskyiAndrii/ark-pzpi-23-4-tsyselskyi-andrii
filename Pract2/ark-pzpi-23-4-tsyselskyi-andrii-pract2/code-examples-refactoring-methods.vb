Module Program
    Sub Main(args As String())


    End Sub

    ' поганий приклад
    Public Class ReportGenerator
        Public Sub GenerateReport(data As List(Of Integer))
            Console.WriteLine("Generating report...")

            Dim sum As Integer = 0
            For Each value In data
                sum += value
            Next
            Console.WriteLine("Total sum: " & sum)

            Dim avg As Double = 0
            For Each value In data
                avg += value
            Next
            avg /= data.Count
            Console.WriteLine("Average: " & avg)

            Console.WriteLine("Report generated.")
        End Sub
    End Class

    ' гарний приклад
    Public Class ReportGeneratorCorrected
        Public Sub GenerateReport(data As List(Of Integer))
            Console.WriteLine("Generating report...")

            Dim sum As Integer = CalculateSum(data)
            Console.WriteLine("Total sum: " & sum)

            Dim avg As Double = CalculateAverage(data)
            Console.WriteLine("Average: " & avg)

            Console.WriteLine("Report generated.")
        End Sub

        Private Function CalculateSum(data As List(Of Integer)) As Integer
            Dim result As Integer = 0
            For Each value In data
                result += value
            Next
            Return result
        End Function

        Private Function CalculateAverage(data As List(Of Integer)) As Double
            Return CalculateSum(data) / data.Count
        End Function
    End Class


    ' поганий приклад
    Public Class UserService
        Public Function DoIt(u As String, a As Integer) As Boolean
            If a > 18 Then
                Console.WriteLine("User " & u & " is adult.")
                Return True
            Else
                Console.WriteLine("User " & u & " is not adult.")
                Return False
            End If
        End Function
    End Class

    ' гарний приклад
    Public Class UserServiceСorrected
        Public Function IsUserAdult(userName As String, age As Integer) As Boolean
            If age > 18 Then
                Console.WriteLine("User " & userName & " is adult.")
                Return True
            Else
                Console.WriteLine("User " & userName & " is not adult.")
                Return False
            End If
        End Function
    End Class

    ' поганий приклад
    Public Class PaymentProcessor
        Public Function ProcessPayment(type As String, amount As Double) As Double
            If type = "Card" Then
                Console.WriteLine("Processing card payment...")
                Return amount * 0.98 ' 2% комісія
            ElseIf type = "PayPal" Then
                Console.WriteLine("Processing PayPal payment...")
                Return amount * 0.96 ' 4% комісія
            ElseIf type = "Crypto" Then
                Console.WriteLine("Processing crypto payment...")
                Return amount * 0.9 ' 10% комісія
            Else
                Throw New Exception("Unknown payment method")
            End If
        End Function
    End Class

    ' гарний приклад
    Public MustInherit Class PaymentMethod
        Public MustOverride Function Process(amount As Double) As Double
    End Class

    Public Class CardPayment
        Inherits PaymentMethod
        Public Overrides Function Process(amount As Double) As Double
            Console.WriteLine("Processing card payment...")
            Return amount * 0.98
        End Function
    End Class

    Public Class PayPalPayment
        Inherits PaymentMethod
        Public Overrides Function Process(amount As Double) As Double
            Console.WriteLine("Processing PayPal payment...")
            Return amount * 0.96
        End Function
    End Class

    Public Class CryptoPayment
        Inherits PaymentMethod
        Public Overrides Function Process(amount As Double) As Double
            Console.WriteLine("Processing crypto payment...")
            Return amount * 0.9
        End Function
    End Class

    Public Class PaymentProcessorСorrected
        Public Function ProcessPayment(method As PaymentMethod, amount As Double) As Double
            Return method.Process(amount)
        End Function
    End Class


End Module
