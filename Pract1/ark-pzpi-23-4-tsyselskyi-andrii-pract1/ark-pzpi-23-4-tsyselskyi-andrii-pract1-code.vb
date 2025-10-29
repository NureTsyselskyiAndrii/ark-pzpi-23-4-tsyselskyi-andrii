Module Program
    Sub Main(args As String())

        ' �����
        Dim CustomerName As String
        Dim TotalPrice As Decimal

        ' ������
        Dim customer_name As String
        Dim totalprice As Decimal

        ' �����
        Dim HtmlDocument As String
        Dim UserURL As String

        ' ������ (������� ����� � ������)
        Dim HypertextMarkupLanguageDocument As String
        Dim WebAddr As String

        ' ��������� ��������� ���� 
        Dim Date As String = "10/23/2025"

        ' ���������� ������
        Dim currentDate As String = "10/23/2025"

        ' ���� ����� ���������� �� ����������� Date:
        Dim currentDate = DateTime.Date
        MsgBox(currentDate.ToString())

        ' ������
        Dim a As Integer : Dim b As Integer
        a = 5 : b = 10
        If a < b Then Console.WriteLine("Smaller")


        ' �����
        Dim a As Integer
        Dim b As Integer

        a = 5
        b = 10

        If a < b Then
            Console.WriteLine("Smaller")
        End If

        ' ������
        Dim letters2() As String = New String() {"a", "b", "c"}

        ' �����
        Dim letters1 As String() = {"a", "b", "c"}

        ' ������
        Dim letters3() As String = {"a", "b", "c"}

        Dim letters6(2) As String
        letters6(0) = "a"
        letters6(1) = "b"
        letters6(2) = "c"

        ' �����
        Dim letters4 As String() = {"a", "b", "c"}
        Dim letters5 As String() = {"a", "b", "c"}


        ' ������
        Dim q = From c In customers, o In orders
                Where c.CustomerID = o.CustomerID
                Select c.Name, o.ID

        ' �����
        Dim customerOrders = From customer In customers
                             Join order In orders
                               On customer.CustomerID Equals order.CustomerID
                             Where customer.City = "Seattle"
                             Select CustomerName = customer.Name,
                                    OrderID = order.ID
    End Sub



    ' ������
    Sub Salary()
        ' ...
    End Sub

    ' �����
    Sub CalculateSalary()
        ' ...
    End Sub

    ' ������
    Public Class Employ
        Public Property EmployeeName As String
    End Class

    ' �����
    Public Class Employee
        Public Property EmployeeName As String
    End Class

    ' ��������� (������)
    Public Interface Printable
        Sub PrintDocument()
    End Interface

    ' ��������� (�����)
    Public Interface IPrintable
        Sub PrintDocument()
    End Interface

    ' �������� ��䳿 (������)
    Public Sub MouseClick(sender As Object, e As EventArgs)
        MsgBox("Mouse clicked!")
    End Sub


    ' �������� ��䳿 (�����)
    Public Sub MouseClickEventHandler(sender As Object, e As EventArgs)
        MsgBox("Mouse clicked!")
    End Sub


    ' ���� ��������� ��䳿 (������)
    Public Class Order
        Inherits EventArgs
        Public Property OrderId As Integer
    End Class

    ' ���� ��������� ��䳿 (�����)
    Public Class OrderEventArgs
        Inherits EventArgs
        Public Property OrderId As Integer
    End Class


    ' ������
    Dim a As Integer, b As Integer, c As Integer
    Sub Test() : Console.WriteLine("Hello") : End Sub
    Property Name As String : Get : Return _name : End Get : Set(value As String) : _name = value : End Set : End Property

    ' �����
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

    ' ������
    Dim x As Integer = 10 'variable for count
    '************ Start of Function ************
    Function Add(a As Integer, b As Integer) As Integer
        Return a + b 'adds numbers
    End Function 'end
    '************ End ************

    ' �����
    ' Variable for counting.
    Dim x As Integer = 10

    ' Adds two numbers and returns the result.
    Function Add(a As Integer, b As Integer) As Integer
        Return a + b
    End Function

End Module
