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
public class AccountingServiceTests
{
    private Mock<IAccountingRepository> _repositoryMock;
    private Mock<IMapper> _mapperMock;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IAccountingRepository>();
        _mapperMock = new Mock<IMapper>();
        _accountingService = new AccountingService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithValidInvoice_ReturnsHtmlContent()
    {
        // Arrange
        int invoiceId = 1;
        var invoiceDto = new InvoiceDto { Id = invoiceId, CustomerName = "John Doe", InvoiceDate = DateTime.Now, TotalAmount = 100.00m };
        var invoiceModel = new InvoiceModel { Id = invoiceId, CustomerName = "John Doe", InvoiceDate = DateTime.Now, TotalAmount = 100.00m };

        _repositoryMock.Setup(repo => repo.GetInvoiceById(invoiceId)).Returns(invoiceModel);

        // Act
        IActionResult result = _accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.IsInstanceOf<ContentResult>(result);
        var contentResult = result as ContentResult;
        Assert.AreEqual("text/html", contentResult.ContentType);
        Assert.IsTrue(contentResult.Content.Contains($"Invoice #{invoiceId}"));
        Assert.IsTrue(contentResult.Content.Contains($"Customer: John Doe"));
        Assert.IsTrue(contentResult.Content.Contains($"Date: {DateTime.Now.ToString("yyyy-MM-dd")}"));
        Assert.IsTrue(contentResult.Content.Contains($"Total: 100.00"));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithInvalidInvoice_ReturnsNotFound()
    {
        // Arrange
        int invoiceId = 1;
        _repositoryMock.Setup(repo => repo.GetInvoiceById(invoiceId)).Returns((InvoiceModel)null);

        // Act
        IActionResult result = _accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }
}