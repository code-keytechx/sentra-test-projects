```csharp
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

[Collection("InvoiceServiceTests")]
public class InvoiceServiceHappyTests
{
    private readonly Mock<IRepository<Invoice>> _invoiceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceHappyTests()
    {
        _invoiceRepositoryMock = new Mock<IRepository<Invoice>>();
        _mapperMock = new Mock<IMapper>();
        _invoiceService = new InvoiceService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithValidInputs()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = null };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", CreatedDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending", CreatedDate = DateTime.Now }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate, TotalAmount = i.TotalAmount, Status = i.Status, CreatedDate = i.CreatedDate }).ToList());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsFilteredResults_WithSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "Customer1" };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", CreatedDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending", CreatedDate = DateTime.Now }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate, TotalAmount = i.TotalAmount, Status = i.Status, CreatedDate = i.CreatedDate }).ToList());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Items.First().Id);
        Assert.Equal("Customer1", result.Items.First().CustomerName);
    }

    [Fact]
    public void GetInvoiceSummaries_SortsByInvoiceDateDescending()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = null };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Parse("2023-01-01"), TotalAmount = 100, Status = "Paid", CreatedDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Parse("2023-01-02"), TotalAmount = 200, Status = "Pending", CreatedDate = DateTime.Now }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate, TotalAmount = i.TotalAmount, Status = i.Status, CreatedDate = i.CreatedDate }).ToList());

        // Act
        var result = _