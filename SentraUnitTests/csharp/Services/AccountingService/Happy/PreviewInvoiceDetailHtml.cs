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
    [Test]
    public void PreviewInvoiceDetailHtml_WithValidInvoice_ReturnsHtmlContent()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository<Invoice>>();
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object);

        int invoiceId = 1;
        var invoiceDto = new InvoiceDto
        {
            Id = invoiceId,
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            TotalAmount = 100.00m
        };

        mockDocumentRepository.Setup(repo => repo.GetByIdAsync(invoiceId)).ReturnsAsync(new Invoice { Id = invoiceId, CustomerName = "John Doe", InvoiceDate = DateTime.Now, TotalAmount = 100.00m });

        // Act
        IActionResult result = accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.IsInstanceOf<ContentResult>(result);
        var contentResult = result as ContentResult;
        Assert.AreEqual("text/html", contentResult.ContentType);
        Assert.IsTrue(contentResult.Content.Contains($"<h1>Invoice #{invoiceId}</h1>"));
        Assert.IsTrue(contentResult.Content.Contains($"<p>Customer: John Doe</p>"));
        Assert.IsTrue(contentResult.Content.Contains($"<p>Date: {DateTime.Now.ToString("yyyy-MM-dd")}</p>"));
        Assert.IsTrue(contentResult.Content.Contains($"<p>Total: 100.00</p>"));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNonExistentInvoice_ReturnsNotFound()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository<Invoice>>();
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object);

        int invoiceId = 1;
        mockDocumentRepository.Setup(repo => repo.GetByIdAsync(invoiceId)).ReturnsAsync((Invoice)null);

        // Act
        IActionResult result = accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }
}