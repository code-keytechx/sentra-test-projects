```csharp
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using System.Collections.Generic;

[TestFixture]
public class InvoiceServiceTests
{
    private Mock<IRepository<Invoice>> _invoiceRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private InvoiceService _invoiceService;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IRepository<Invoice>>();
        _mapperMock = new Mock<IMapper>();
        _invoiceService = new InvoiceService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithEmptySearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = "", PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 200, Status = "Pending" }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate, TotalAmount = i.TotalAmount, Status = i.Status }).ToList());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual(2, result.Items.Count);
    }

    [Test]
    public void GetInvoiceSummaries_SortsByInvoiceDateDesc()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = "", PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 200, Status = "Pending" }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate, TotalAmount = i.TotalAmount, Status = i.Status }).ToList());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual("Customer2", result.Items[0].CustomerName);
        Assert.AreEqual("Customer1", result.Items[1].CustomerName);
    }

    [Test]
    public void GetInvoiceSummaries_WithLargePageNumber()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = "", PageNumber = 100, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 200, Status = "Pending" }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate, TotalAmount = i.TotalAmount, Status = i.Status }).ToList());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual