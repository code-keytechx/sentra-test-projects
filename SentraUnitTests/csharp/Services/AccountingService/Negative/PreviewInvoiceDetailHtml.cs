using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

[TestFixture]
public class AccountingServiceTests
{
    [Test]
    public void PreviewInvoiceDetailHtml_WithInvalidInvoiceId_ReturnsNotFound()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var accountingService = new AccountingService(mockMapper.Object);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(-1);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNullInvoice_ReturnsNotFound()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository>();
        mockDocumentRepository.Setup(repo => repo.GetById(It.IsAny<int>())).Returns((InvoiceDto)null);
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(1);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithEmptyInvoiceDetails_ReturnsHtmlContent()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository>();
        mockDocumentRepository.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(new InvoiceDto());
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(1);

        // Assert
        Assert.IsInstanceOf<ContentResult>(result);
        var contentResult = result as ContentResult;
        Assert.AreEqual("text/html", contentResult.ContentType);
        Assert.AreEqual("<html><body><h1>Invoice #</h1><p>Customer: </p><p>Date: </p><p>Total: </p></body></html>", contentResult.Content);
    }
}