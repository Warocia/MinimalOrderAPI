Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Hosting
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.AspNetCore.Http.HttpResults
Imports Microsoft.AspNetCore.Http
Imports System.ComponentModel
Imports Microsoft.AspNetCore.Cors.Infrastructure
Imports Microsoft.OpenApi


Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        Dim connectionString As String = builder.Configuration.GetConnectionString("WebApiDatabase")

        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen()


        builder.Services.AddDbContext(Of DataContext)(Function(options)
                                                          Return options.UseSqlite(connectionString)
                                                      End Function)



        builder.Services.AddCors(Sub(policyBuilder)
                                     policyBuilder.AddPolicy("devPolicy", Function(policy)
                                                                              policy.WithOrigins("http://localhost:3000")
                                                                              policy.SetIsOriginAllowedToAllowWildcardSubdomains()
                                                                              policy.AllowAnyMethod()
                                                                              policy.AllowAnyHeader()
                                                                              Return policy
                                                                          End Function)
                                 End Sub)

        Dim app = builder.Build()

        If app.Environment.IsDevelopment() Then
            app.UseSwagger()
            app.UseSwaggerUI()
        End If

        Using scope As IServiceScope = app.Services.CreateScope()
            Dim services = scope.ServiceProvider

            Dim DbContext As DataContext = services.GetRequiredService(Of DataContext)

            If app.Environment.IsDevelopment() Then
                DbContextSeeder.Seed(DbContext)
            End If

        End Using

        app.MapGet("/products", Async Function(db As DataContext) As Task(Of List(Of Product))
                                    Return Await db.Products.ToListAsync()
                                End Function)

        app.MapGet("/products/{id}", Async Function(id As Integer, db As DataContext) As Task(Of IResult)
                                         Dim product = Await db.Products.FirstOrDefaultAsync(Function(t) t.Id = id)
                                         If product IsNot Nothing Then
                                             Return Results.Ok(product)
                                         Else
                                             Return Results.NotFound
                                         End If
                                     End Function)

        app.MapPost("/products", Async Function(product As Product, db As DataContext) As Task(Of IResult)
                                     db.Products.Add(product)
                                     Await db.SaveChangesAsync()
                                     Return Results.Created($"/products/{product.Id}", product)
                                 End Function)

        app.MapDelete("/products/{id}", Async Function(id As Integer, db As DataContext) As Task(Of IResult)
                                            Dim product = Await db.Products.FirstOrDefaultAsync(Function(t) t.Id = id)

                                            If product Is Nothing Then
                                                Return Results.NotFound
                                            End If

                                            Dim orderline = Await db.Orderlines.FirstOrDefaultAsync(Function(t) t.ProductId = product.Id)

                                            If orderline Is Nothing Then
                                                db.Products.Remove(product)

                                                Await db.SaveChangesAsync()
                                                Return Results.Ok(product)

                                            End If

                                            Return Results.Conflict()
                                        End Function)

        app.MapPut("/products/{id}", Async Function(id As Integer, product As Product, db As DataContext) As Task(Of IResult)

                                         Dim oldProduct = Await db.Products.FirstOrDefaultAsync(Function(t) t.Id = id)

                                         If oldProduct Is Nothing Then
                                             Return Results.NotFound()
                                         End If

                                         oldProduct.ProductName = product.ProductName
                                         oldProduct.Description = product.Description
                                         oldProduct.CostPrice = product.CostPrice
                                         oldProduct.SalesPrice = product.SalesPrice

                                         Await db.SaveChangesAsync()

                                         Dim dbProduct = Await db.Products.FirstOrDefaultAsync(Function(t) t.Id = id)
                                         Return Results.Ok(dbProduct)
                                     End Function)


        app.MapGet("/orders", Async Function(db As DataContext) As Task(Of List(Of Order))
                                  Return Await db.Orders.Include(Function(t) t.Orderlines).ThenInclude(Function(l) l.Product).ToListAsync()
                              End Function)


        app.MapGet("/orders/{id}", Async Function(id As Integer, db As DataContext) As Task(Of IResult)
                                       Dim order = Await db.Orders.Include(Function(t) t.Orderlines).ThenInclude(Function(l) l.Product).FirstOrDefaultAsync(Function(t) t.Id = id)
                                       If order IsNot Nothing Then
                                           Return Results.Ok(order)
                                       Else
                                           Return Results.NotFound
                                       End If
                                   End Function)

        app.MapPost("/orders", Async Function(order As Order, db As DataContext) As Task(Of IResult)
                                   db.Orders.Add(order)
                                   Await db.SaveChangesAsync()
                                   order.OrderNumber = "O" + (order.Id + 1).ToString
                                   Await db.SaveChangesAsync()
                                   Dim dbOrder = Await db.Orders.Include(Function(t) t.Orderlines).ThenInclude(Function(l) l.Product).FirstOrDefaultAsync(Function(t) t.Id = order.Id)

                                   Return Results.Created($"/orders/{order.Id}", dbOrder)
                               End Function)

        app.MapPut("/orders/{id}", Async Function(id As Integer, order As Order, db As DataContext) As Task(Of IResult)

                                       Dim oldOrder = Await db.Orders.Include(Function(t) t.Orderlines).ThenInclude(Function(l) l.Product).FirstOrDefaultAsync(Function(t) t.Id = id)

                                       If oldOrder Is Nothing Then
                                           Return Results.NotFound()
                                       End If

                                       oldOrder.OrderNumber = order.OrderNumber
                                       oldOrder.CustomerName = order.CustomerName
                                       oldOrder.CustomerEmail = order.CustomerEmail
                                       oldOrder.CustomerPhone = order.CustomerPhone
                                       oldOrder.DeliveryDate = order.DeliveryDate

                                       For Each orderline In oldOrder.Orderlines
                                           db.Entry(orderline).State = EntityState.Deleted
                                       Next

                                       oldOrder.Orderlines.Clear()


                                       For Each orderline In order.Orderlines
                                           orderline.Product = Nothing
                                           oldOrder.Orderlines.Add(orderline)
                                       Next

                                       Await db.SaveChangesAsync()

                                       Dim dbOrder = Await db.Orders.Include(Function(t) t.Orderlines).ThenInclude(Function(l) l.Product).FirstOrDefaultAsync(Function(t) t.Id = oldOrder.Id)
                                       Return Results.Ok(dbOrder)
                                   End Function)

        app.MapDelete("/orders/{id}", Async Function(id As Integer, db As DataContext) As Task(Of IResult)
                                          Dim order = Await db.Orders.Include(Function(t) t.Orderlines).FirstOrDefaultAsync(Function(t) t.Id = id)

                                          If order IsNot Nothing Then
                                              For Each orderline In order.Orderlines
                                                  db.Entry(orderline).State = EntityState.Deleted
                                              Next

                                              db.Orders.Remove(order)

                                              Await db.SaveChangesAsync()
                                              Return Results.Ok(order)
                                          End If

                                          Return Results.NotFound()
                                      End Function)

        app.UseCors("devPolicy")
        app.Run()
    End Sub
End Module