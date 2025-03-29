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

[TestFixture]
public class AccountingServiceNegativeTests
{
    private Mock<IAccountingDbContext> _mockDbContext;
    private Mock<IMapper> _mockMapper;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<IAccountingDbContext>();
        _mockMapper = new Mock<IMapper>();
        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsArgumentException_WhenInvoiceIdsIsNull()
    {
        // Arrange
        int[] invoiceIds = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingService.DownloadInvoicesCsv(invoiceIds));
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsArgumentException_WhenInvoiceIdsIsEmpty()
    {
        // Arrange
        int[] invoiceIds = Array.Empty<int>();

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingService.DownloadInvoicesCsv(invoiceIds));
    }

    [Test]
    public void DownloadInvoicesCsv_ReturnsEmptyFileResult_WhenNoInvoicesFound()
    {
        // Arrange
        int[] invoiceIds = { 1, 2, 3 };
        _mockDbContext.Setup(db => db.Invoices.Where(It.IsAny<Expression<Func<Invoice, bool>>>())).Returns(new List<Invoice>());

        // Act
        var result = _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("text/csv", result.ContentType);
        Assert.AreEqual(0, result.FileContents.Length);
        Assert.AreEqual($"Invoices_{DateTime.UtcNow:yyyyMMddHHmmss}.csv", result.FileDownloadName);
    }
}