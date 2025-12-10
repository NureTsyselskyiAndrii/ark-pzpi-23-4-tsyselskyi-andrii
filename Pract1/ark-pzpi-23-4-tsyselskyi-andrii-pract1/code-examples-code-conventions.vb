Module Program
    Sub Main(args As String())

        ' Добре
        Dim CustomerName As String
        Dim TotalPrice As Decimal

        ' Погано
        Dim customer_name As String
        Dim totalprice As Decimal

        ' Добре
        Dim HtmlDocument As String
        Dim UserURL As String

        ' Погано (занадто довго і змішано)
        Dim HypertextMarkupLanguageDocument As String
        Dim WebAddr As String

        ' Уникайння однакових імен 
        Dim Date As String = "10/23/2025"

        ' Правильний варіант
        Dim currentDate As String = "10/23/2025"

        ' Якщо треба звернутися до вбудованого Date:
        Dim currentDate = DateTime.Date
        MsgBox(currentDate.ToString())

        ' Погано
        Dim a As Integer : Dim b As Integer
        a = 5 : b = 10
        If a < b Then Console.WriteLine("Smaller")


        ' Добре
        Dim a As Integer
        Dim b As Integer

        a = 5
        b = 10

        If a < b Then
            Console.WriteLine("Smaller")
        End If

        ' Погано
        Dim letters2() As String = New String() {"a", "b", "c"}

        ' Добре
        Dim letters1 As String() = {"a", "b", "c"}

        ' Погано
        Dim letters3() As String = {"a", "b", "c"}

        Dim letters6(2) As String
        letters6(0) = "a"
        letters6(1) = "b"
        letters6(2) = "c"

        ' Добре
        Dim letters4 As String() = {"a", "b", "c"}
        Dim letters5 As String() = {"a", "b", "c"}


        ' Погано
        Dim q = From c In customers, o In orders
                Where c.CustomerID = o.CustomerID
                Select c.Name, o.ID

        ' Добре
        Dim customerOrders = From customer In customers
                             Join order In orders
                               On customer.CustomerID Equals order.CustomerID
                             Where customer.City = "Seattle"
                             Select CustomerName = customer.Name,
                                    OrderID = order.ID
    End Sub



    ' Погано
    Sub Salary()
        ' ...
    End Sub

    ' Добре
    Sub CalculateSalary()
        ' ...
    End Sub

    ' Погано
    Public Class Employ
        Public Property EmployeeName As String
    End Class

    ' Добре
    Public Class Employee
        Public Property EmployeeName As String
    End Class

    ' Інтерфейс (Погано)
    Public Interface Printable
        Sub PrintDocument()
    End Interface

    ' Інтерфейс (Добре)
    Public Interface IPrintable
        Sub PrintDocument()
    End Interface

    ' Обробник події (Погано)
    Public Sub MouseClick(sender As Object, e As EventArgs)
        MsgBox("Mouse clicked!")
    End Sub


    ' Обробник події (Добре)
    Public Sub MouseClickEventHandler(sender As Object, e As EventArgs)
        MsgBox("Mouse clicked!")
    End Sub


    ' Клас аргументів події (Погано)
    Public Class Order
        Inherits EventArgs
        Public Property OrderId As Integer
    End Class

    ' Клас аргументів події (Добре)
    Public Class OrderEventArgs
        Inherits EventArgs
        Public Property OrderId As Integer
    End Class


    ' Погано
    Dim a As Integer, b As Integer, c As Integer
    Sub Test() : Console.WriteLine("Hello") : End Sub
    Property Name As String : Get : Return _name : End Get : Set(value As String) : _name = value : End Set : End Property

    ' Добре
    Dim a As Integer
    Dim b As Integer
    Dim c As Integer

    Sub Test()
        Console.WriteLine("Hello")
    End Sub

    Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    ' Погано
    Dim x As Integer = 10 'variable for count
    '************ Start of Function ************
    Function Add(a As Integer, b As Integer) As Integer
        Return a + b 'adds numbers
    End Function 'end
    '************ End ************

    ' Добре
    ' Variable for counting.
    Dim x As Integer = 10

    ' Adds two numbers and returns the result.
    Function Add(a As Integer, b As Integer) As Integer
        Return a + b
    End Function

End Module
