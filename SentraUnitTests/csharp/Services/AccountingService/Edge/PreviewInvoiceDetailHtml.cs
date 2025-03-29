using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

[TestFixture]
public class AccountingServiceTests
{
    [Test]
    public void PreviewInvoiceDetailHtml_WithValidInvoiceId_ReturnsHtmlContent()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository<InvoiceDto>>();
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object);

        var invoiceId = 1;
        var invoiceDto = new InvoiceDto
        {
            Id = invoiceId,
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            TotalAmount = 100.00m
        };
        mockDocumentRepository.Setup(repo => repo.GetByIdAsync(invoiceId)).ReturnsAsync(invoiceDto);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.IsInstanceOf<ContentResult>(result);
        var contentResult = result as ContentResult;
        Assert.AreEqual("text/html", contentResult.ContentType);
        Assert.IsTrue(contentResult.Content.Contains($"Invoice #{invoiceId}"));
        Assert.IsTrue(contentResult.Content.Contains($"Customer: {invoiceDto.CustomerName}"));
        Assert.IsTrue(contentResult.Content.Contains($"Date: {invoiceDto.InvoiceDate.ToString("yyyy-MM-dd")}"));
        Assert.IsTrue(contentResult.Content.Contains($"Total: {invoiceDto.TotalAmount:F2}"));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNonExistentInvoiceId_ReturnsNotFound()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository<InvoiceDto>>();
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object);

        var invoiceId = 1;
        mockDocumentRepository.Setup(repo => repo.GetByIdAsync(invoiceId)).ReturnsAsync((InvoiceDto)null);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNegativeInvoiceId_ReturnsNotFound()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository<InvoiceDto>>();
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object);

        var invoiceId = -1;
        mockDocumentRepository.Setup(repo => repo.GetByIdAsync(invoiceId)).ReturnsAsync((InvoiceDto)null);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }
}