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
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<IRepository<Invoice>> _invoiceRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IRepository<Invoice>>();
        _mapperMock = new Mock<IMapper>();
        _accountingService = new AccountingService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void GetInvoiceSummaries_ShouldReturnPagedResults_WithFilterAndPagination()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "test" };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", CreatedDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending", CreatedDate = DateTime.Now }
        };
        var invoiceSummaries = invoices.Select(i => new InvoiceSummary
        {
            Id = i.Id,
            CustomerName = i.CustomerName,
            InvoiceDate = i.InvoiceDate,
            TotalAmount = i.TotalAmount,
            Status = i.Status,
            CreatedDate = i.CreatedDate
        }).ToList();

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoiceSummaries);

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual("Customer1", result.Items[0].CustomerName);
    }

    [Test]
    public void GetInvoiceSummaries_ShouldReturnEmptyPagedResults_WithoutInvoices()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>();

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(0, result.TotalCount);
        Assert.AreEqual(0, result.Items.Count);
    }

    [Test]
    public void GetInvoiceSummaries_ShouldSortByInvoiceDateDescending()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 100, Status = "Paid", CreatedDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending", CreatedDate = DateTime.Now }
        };
        var invoiceSummaries = invoices.OrderBy(i => i.InvoiceDate).Reverse().Select(i => new InvoiceSummary
        {
            Id = i.Id,
            CustomerName = i.CustomerName,
            InvoiceDate = i.InvoiceDate,
            TotalAmount = i.TotalAmount,
            Status = i.Status,
            CreatedDate = i.CreatedDate
        }).ToList();

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoiceSummaries);

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual(2, result.Items.Count);
        Assert.AreEqual("Customer2", result.Items[0].CustomerName);
    }

    [Test]
    public void GetInvoiceSummaries_ShouldApplyPagination()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { Page