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
    public void DownloadInvoiceDetailPdf_WithValidInvoice_ReturnsFileResult()
    {
        // Arrange
        int invoiceId = 123;
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository>();
        var mockFactory = new Mock<IResponseFactory>();

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object, mockFactory.Object);

        var invoiceDto = new InvoiceDto { Id = invoiceId };
        mockDocumentRepository.Setup(repo => repo.GetInvoiceById(invoiceId)).Returns(invoiceDto);

        byte[] pdfBytes = new byte[0];
        mockFactory.Setup(factory => factory.CreatePdfFromInvoice(It.IsAny<InvoiceDto>())).Returns(pdfBytes);

        // Act
        var result = accountingService.DownloadInvoiceDetailPdf(invoiceId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<FileContentResult>(result);
        Assert.AreEqual("application/pdf", result.ContentType);
        Assert.AreEqual($"Invoice_{invoiceId}.pdf", result.FileDownloadName);
        Assert.AreEqual(pdfBytes, result.FileContents);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithNonExistentInvoice_ReturnsNull()
    {
        // Arrange
        int invoiceId = 123;
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository>();
        var mockFactory = new Mock<IResponseFactory>();

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object, mockFactory.Object);

        mockDocumentRepository.Setup(repo => repo.GetInvoiceById(invoiceId)).Returns((InvoiceDto)null);

        // Act
        var result = accountingService.DownloadInvoiceDetailPdf(invoiceId);

        // Assert
        Assert.IsNull(result);
    }
}