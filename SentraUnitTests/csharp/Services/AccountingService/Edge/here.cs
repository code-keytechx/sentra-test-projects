using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;

[TestClass]
public class AccountingServiceTests
{
    [TestMethod]
    public void GeneratePdf_WithBoundaryMinInvoiceData_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<InvoiceDetailViewModel, InvoiceDto>()).CreateMapper();
        var factory = new DocumentFactory();
        var service = new AccountingService(mapper, factory);

        var invoice = new InvoiceDetailViewModel
        {
            Id = 1,
            CustomerName = "John Doe",
            TotalAmount = decimal.MinValue + 1m,
            Items = new List<InvoiceItemViewModel>
            {
                new InvoiceItemViewModel { Description = "Item 1", Quantity = 1, Price = decimal.MinValue + 1m }
            }
        };

        // Act
        var result = service.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void GeneratePdf_WithBoundaryMaxInvoiceData_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<InvoiceDetailViewModel, InvoiceDto>()).CreateMapper();
        var factory = new DocumentFactory();
        var service = new AccountingService(mapper, factory);

        var invoice = new InvoiceDetailViewModel
        {
            Id = int.MaxValue,
            CustomerName = new string('A', 255),
            TotalAmount = decimal.MaxValue,
            Items = new List<InvoiceItemViewModel>
            {
                new InvoiceItemViewModel { Description = new string('B', 255), Quantity = int.MaxValue, Price = decimal.MaxValue }
            }
        };

        // Act
        var result = service.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void GeneratePdf_WithLargeNumberOfItems_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<InvoiceDetailViewModel, InvoiceDto>()).CreateMapper();
        var factory = new DocumentFactory();
        var service = new AccountingService(mapper, factory);

        var invoice = new InvoiceDetailViewModel
        {
            Id = 1,
            CustomerName = "John Doe",
            TotalAmount = 1000m,
            Items = Enumerable.Range(1, 100).Select(i => new InvoiceItemViewModel { Description = $"Item {i}", Quantity = i, Price = 10m }).ToList()
        };

        // Act
        var result = service.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }
}