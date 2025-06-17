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
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<SampleDbContext> _mockDbContext;
    private Mock<IMapper> _mockMapper;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<SampleDbContext>();
        _mockMapper = new Mock<IMapper>();

        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Test]
    public async Task DownloadInvoicesCsv_ReturnsFileResult_WithCorrectMimeAndFileName()
    {
        // Arrange
        int[] invoiceIds = { 1, 2, 3 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100.00m },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 200.00m },
            new Invoice { Id = 3, CustomerName = "Customer3", InvoiceDate = DateTime.Now, TotalAmount = 300.00m }
        };

        _mockDbContext.Setup(db => db.Invoices.Where(i => invoiceIds.Contains(i.Id)).ToList()).Returns(invoices);

        // Act
        var result = await _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<FileContentResult>(result);
        var fileResult = result as FileContentResult;
        Assert.AreEqual("text/csv", fileResult.ContentType);
        Assert.IsTrue(fileResult.FileDownloadName.StartsWith("Invoices_"));
        Assert.IsTrue(fileResult.FileDownloadName.EndsWith(".csv"));

        // Verify CSV content
        var csvContent = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        var lines = csvContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        Assert.AreEqual(4, lines.Length); // Header + 3 rows
        Assert.AreEqual("InvoiceId,CustomerName,InvoiceDate,TotalAmount", lines[0]);
        Assert.AreEqual("1,Customer1," + invoices[0].InvoiceDate.ToString("yyyy-MM-dd") + ",100.00", lines[1]);
        Assert.AreEqual("2,Customer2," + invoices[1].InvoiceDate.ToString("yyyy-MM-dd") + ",200.00", lines[2]);
        Assert.AreEqual("3,Customer3," + invoices[2].InvoiceDate.ToString("yyyy-MM-dd") + ",300.00", lines[3]);
    }

    [Test]
    public async Task DownloadInvoicesCsv_NoInvoices_ReturnsEmptyFileResult()
    {
        // Arrange
        int[] invoiceIds = { 1, 2, 3 };
        _mockDbContext.Setup(db => db.Invoices.Where(i => invoiceIds.Contains(i.Id)).ToList()).Returns(new List<Invoice>());

        // Act
        var result = await _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<FileContentResult>(result);
        var fileResult = result as FileContentResult;
        Assert.AreEqual("text/csv", fileResult.ContentType);
        Assert.IsTrue(fileResult.FileDownloadName.StartsWith("Invoices_"));
        Assert.IsTrue(fileResult.FileDownloadName.EndsWith(".csv"));

        // Verify CSV content
        var csvContent = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        var lines = csvContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        Assert.AreEqual(1, lines.Length); // Only header
        Assert.AreEqual("InvoiceId,CustomerName,InvoiceDate,TotalAmount", lines[0]);
    }

    [Test]
    public async Task DownloadInvoicesCsv_EmptyInvoiceIds_ReturnsEmptyFileResult()
    {
        // Arrange
        int[] invoiceIds = { };
        _mockDbContext.Setup(db => db.Invoices.Where(i => invoiceIds.Contains(i.Id)).ToList()).Returns(new List<Invoice>());

        // Act
        var result = await _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<FileContentResult>(result);
        var fileResult = result as FileContentResult;
        Assert.AreEqual("text/csv", fileResult.ContentType);
        Assert.IsTrue(fileResult.FileDownloadName.StartsWith("Invoices_"));
        Assert.IsTrue(fileResult.FileDownloadName.EndsWith(".csv"));

        // Verify CSV content
        var csvContent = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        var lines = csvContent