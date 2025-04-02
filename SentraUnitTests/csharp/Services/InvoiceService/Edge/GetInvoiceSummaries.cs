using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using Xunit;

[Collection("InvoiceServiceTests")]
public class InvoiceServiceEdgeTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceEdgeTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _mapperMock = new Mock<IMapper>();

        _invoiceService = new InvoiceService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithMinimumPageSize()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 1 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", CreatedDate = DateTime.Now }
        };

        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Items.Count);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithMaximumPageSize()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = int.MaxValue };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", CreatedDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending", CreatedDate = DateTime.Now }
        };

        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(int.MaxValue, result.PageSize);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithLargePageNumber()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = int.MaxValue, PageSize = 1 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", CreatedDate = DateTime.Now }
        };

        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(int.MaxValue, result.CurrentPage);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(0, result.TotalCount);
    }
}