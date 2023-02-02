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
            DbContextSeeder.Seed(DbContext)
        End Using

        app.MapGet("/orders", Async Function(db As DataContext) As Task(Of List(Of Order))
                                  Return Await db.Orders.Include(Function(t) t.Orderlines).ToListAsync()
                              End Function)


        app.MapGet("/orders/{id}", Async Function(id As Integer, db As DataContext) As Task(Of IResult)
                                       Dim order = Await db.Orders.Include(Function(t) t.Orderlines).FirstOrDefaultAsync(Function(t) t.Id = id)
                                       If order IsNot Nothing Then
                                           Return Results.Ok(order)
                                       Else
                                           Return Results.NotFound
                                       End If
                                   End Function)

        app.MapPost("/orders", Async Function(newOrder As Order, db As DataContext) As Task(Of IResult)
                                   db.Orders.Add(newOrder)
                                   Await db.SaveChangesAsync()
                                   Return Results.Created($"/todoitems/{newOrder.Id}", newOrder)
                               End Function)

        app.MapPut("/orders/{id}", Async Function(id As Integer, order As Order, db As DataContext) As Task(Of IResult)

                                       Dim oldOrder = Await db.Orders.Include(Function(t) t.Orderlines).FirstOrDefaultAsync(Function(t) t.Id = id)

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
                                           oldOrder.Orderlines.Add(orderline)
                                       Next

                                       Await db.SaveChangesAsync()
                                       Return Results.Ok(oldOrder)
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