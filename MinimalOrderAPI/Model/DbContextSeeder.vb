Imports Microsoft.AspNetCore.Identity
Imports Microsoft.EntityFrameworkCore
Imports System.ComponentModel

Public Module DbContextSeeder
    Public Sub Seed(context As DataContext)
        context.Database.EnsureDeleted()
        context.Database.EnsureCreated()

        Dim orderline1 As New Orderline() With {
            .ProductName = "Toilet paper",
            .Count = 10,
            .UnitCost = 10,
            .TotalCost = 100,
            .CostUnit = "€"
        }

        Dim orderline2 As New Orderline() With {
            .ProductName = "L'Oréal Hair Oil",
            .Count = 1,
            .UnitCost = 10,
            .TotalCost = 10,
            .CostUnit = "€"
        }

        Dim orderline3 As New Orderline() With {
            .ProductName = "Super Sparrow Stainless Steel Water Bottle",
            .Count = 15,
            .UnitCost = 5,
            .TotalCost = 75,
            .CostUnit = "€"
        }

        Dim order As New Order() With {
            .OrderNumber = "O1432",
            .CustomerName = "Jacobson LLC",
            .CustomerEmail = "jacobson@gmail.com",
            .CustomerPhone = "340305454",
            .DeliveryDate = DateTime.Now,
            .Orderlines = New List(Of Orderline) From {orderline1, orderline2, orderline3}
        }

        context.Orders.Add(order)

        context.SaveChanges()
    End Sub
End Module
