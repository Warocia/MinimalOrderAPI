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
        Dim randomNumberForProduct As Integer = random.Next(10, 40)

        Dim productList = New List(Of Product)

        For productIndex = 1 To randomNumberForProduct

            Dim price As Decimal = random.Next(10, 200)

            Dim product As New Product() With {
                 .ProductName = Faker.Lorem.Words(1).First,
                 .Description = Faker.Lorem.Paragraph(1),
                 .CostPrice = price,
                 .SalesPrice = price * 1.2
            }

            productList.Add(product)
        Next


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

                Dim productIndex = random.Next(0, randomNumberForProduct - 1)
                Dim orderline As New Orderline() With {
                  .Product = productList(productIndex),
                  .Count = random.Next(1, 15),
                  .UnitCostPrice = productList(productIndex).CostPrice,
                  .UnitSalesPrice = productList(productIndex).SalesPrice,
                  .TotalUnitCostPrice = 0,
                  .SalesPriceTotal = 0,
                  .PriceUnit = "€"
                }


                orderline.SalesPriceTotal = orderline.UnitSalesPrice * orderline.Count
                orderline.TotalUnitCostPrice = orderline.UnitCostPrice * orderline.Count
                order.Orderlines.Add(orderline)

            Next

            context.Orders.Add(order)
        Next

        context.SaveChanges()

    End Sub
End Module
