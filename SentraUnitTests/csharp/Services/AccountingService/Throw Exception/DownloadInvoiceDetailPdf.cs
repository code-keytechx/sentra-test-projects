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
    private Mock<IAccountingService> _accountingServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IDocumentFactory> _documentFactoryMock;

    [SetUp]
    public void Setup()
    {
        _accountingServiceMock = new Mock<IAccountingService>();
        _mapperMock = new Mock<IMapper>();
        _documentFactoryMock = new Mock<IDocumentFactory>();
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithNonExistentInvoiceId_ThrowsArgumentException()
    {
        // Arrange
        int nonExistentInvoiceId = -1;
        _accountingServiceMock.Setup(service => service.GetInvoiceById(nonExistentInvoiceId)).Returns((InvoiceDto)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.DownloadInvoiceDetailPdf(nonExistentInvoiceId));
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithInvalidInvoiceId_ThrowsArgumentException()
    {
        // Arrange
        int invalidInvoiceId = 0;
        _accountingServiceMock.Setup(service => service.GetInvoiceById(invalidInvoiceId)).Returns((InvoiceDto)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.DownloadInvoiceDetailPdf(invalidInvoiceId));
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithMaxIntInvoiceId_ThrowsArgumentException()
    {
        // Arrange
        int maxIntInvoiceId = int.MaxValue;
        _accountingServiceMock.Setup(service => service.GetInvoiceById(maxIntInvoiceId)).Returns((InvoiceDto)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.DownloadInvoiceDetailPdf(maxIntInvoiceId));
    }
}