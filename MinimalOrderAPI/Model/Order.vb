Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

<Table("Order")>
Public Class Order
    <Key>
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)>
    Public Property Id As Integer

    <Required>
    Public Property OrderNumber As String

    <Required>
    Public Property CustomerName As String


    <DataType(DataType.EmailAddress)>
    Public Property CustomerEmail As String

    <DataType(DataType.PhoneNumber)>
    Public Property CustomerPhone As String

    <DataType(DataType.DateTime)>
    Public Property DeliveryDate As DateTime

    Public Overridable Property Orderlines As ICollection(Of Orderline)

End Class
