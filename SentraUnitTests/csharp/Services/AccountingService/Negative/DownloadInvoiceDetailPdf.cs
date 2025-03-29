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
    [Test]
    public void DownloadInvoiceDetailPdf_WithInvalidInvoiceId_ReturnsNull()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        var result = accountingService.DownloadInvoiceDetailPdf(-1);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithNonExistentInvoiceId_ReturnsNull()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Simulate GetInvoiceById returning null
        mockDocumentFactory.Setup(factory => factory.GetInvoiceById(It.IsAny<int>())).Returns((InvoiceDto)null);

        // Act
        var result = accountingService.DownloadInvoiceDetailPdf(12345);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithZeroInvoiceId_ReturnsNull()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        var result = accountingService.DownloadInvoiceDetailPdf(0);

        // Assert
        Assert.IsNull(result);
    }
}