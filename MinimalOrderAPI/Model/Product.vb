Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

<Table("Product")>
Public Class Product
    <Key>
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)>
    Public Property Id As Integer

    <Required>
    Public Property ProductName As String

    <Required>
    Public Property Description As String

    <Required>
    <DataType(DataType.Currency)>
    Public Property CostPrice As Decimal

    <Required>
    <DataType(DataType.Currency)>
    Public Property SalesPrice As Decimal

End Class
