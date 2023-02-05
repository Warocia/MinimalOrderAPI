Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

<Table("Orderline")>
Public Class Orderline
    <Key>
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)>
    Public Property Id As Integer

    <ForeignKey("Product")>
    Public Property ProductId As Nullable(Of Integer)

    Public Property Product As Product

    <Required>
    Public Property Count As Integer


    <DataType(DataType.Currency)>
    Public Property UnitCost As Decimal

    <DataType(DataType.Currency)>
    Public Property TotalCost As Decimal

    Public Property CostUnit As String

End Class
