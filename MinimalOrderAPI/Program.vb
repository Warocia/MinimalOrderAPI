Imports System
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Hosting

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        Dim connectionString As String = builder.Configuration.GetConnectionString("WebApiDatabase")

        builder.Services.AddDbContext(Of DataContext)(Function(options)
                                                          Return options.UseSqlite(connectionString)
                                                      End Function)

        Dim app = builder.Build()

        Using scope As IServiceScope = app.Services.CreateScope()
            Dim services = scope.ServiceProvider

            Dim DbContext As DataContext = services.GetRequiredService(Of DataContext)
            DbContextSeeder.Seed(DbContext)
        End Using

        app.MapGet("/", Function() "Hello World!")

        app.Run()
    End Sub
End Module