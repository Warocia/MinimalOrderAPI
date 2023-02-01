Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

<Table("Orderline")>
Public Class Orderline
    <Key>
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)>
    Public Property Id As Integer

    <Required>
    Public Property ProductName As String

    <Required>
    Public Property Count As Integer


    <DataType(DataType.Currency)>
    Public Property UnitCost As Decimal

    <DataType(DataType.Currency)>
    Public Property TotalCost As Decimal

    Public Property CostUnit As String

End Class
