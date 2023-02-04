Imports Faker
Imports Microsoft.AspNetCore.Identity
Imports Microsoft.EntityFrameworkCore
Imports System.ComponentModel

Public Module DbContextSeeder
    Public Sub Seed(context As DataContext)
        context.Database.EnsureDeleted()
        context.Database.EnsureCreated()

        Dim random As New Random()
        Dim randomNumber As Integer = random.Next(10, 150)



        For index = 0 To randomNumber

            Dim randomNumberRow As Integer = random.Next(1, 10)

            Dim customerCompanyName As String = Faker.Company.Name

            Dim order As New Order() With {
                .OrderNumber = "O" + (index + 1).ToString,
                .CustomerName = Faker.Company.Name,
                .CustomerEmail = Faker.Internet.FreeEmail,
                .CustomerPhone = Faker.Phone.Number,
                .DeliveryDate = DateTime.Now.AddDays(index),
                .Orderlines = New List(Of Orderline)
            }

            For subIndex = 0 To randomNumberRow
                Dim orderline As New Orderline() With {
                  .ProductName = Faker.Lorem.Words(1).First,
                  .Count = random.Next(1, 15),
                  .UnitCost = random.Next(1, 150),
                  .TotalCost = 0,
                  .CostUnit = "€"
                }

                orderline.TotalCost = orderline.UnitCost * orderline.Count
                order.Orderlines.Add(orderline)

            Next

            context.Orders.Add(order)
        Next

        context.SaveChanges()

    End Sub
End Module
