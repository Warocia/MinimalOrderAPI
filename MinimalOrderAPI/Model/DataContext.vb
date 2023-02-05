Imports System.ComponentModel
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration

Public Class DataContext
    Inherits DbContext

    Protected ReadOnly Configuration As IConfiguration
    Public Sub New(configuration As IConfiguration)
        Me.Configuration = configuration
    End Sub

    Protected Overrides Sub OnConfiguring(options As DbContextOptionsBuilder)
        ' connect to sqlite database
        options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"))
    End Sub

    Public Property Orders As DbSet(Of Order)
    Public Property Orderlines As DbSet(Of Orderline)
    Public Property Products As DbSet(Of Product)
End Class
