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
using System.IO;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<IAccountingRepository> _accountingRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _accountingRepositoryMock = new Mock<IAccountingRepository>();
        _mapperMock = new Mock<IMapper>();

        _accountingService = new AccountingService(_accountingRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithValidInvoice_ReturnsFileResult()
    {
        // Arrange
        int invoiceId = 1;
        var invoiceDto = new InvoiceDto { Id = invoiceId };
        var pdfBytes = new byte[0];

        _accountingRepositoryMock.Setup(repo => repo.GetInvoiceById(invoiceId)).Returns(invoiceDto);
        _mapperMock.Setup(mapper => mapper.Map<InvoiceDto>(invoiceDto)).Returns(new Invoice());
        _accountingRepositoryMock.Setup(repo => repo.GeneratePdf(It.IsAny<Invoice>())).Returns(pdfBytes);

        // Act
        var result = _accountingService.DownloadInvoiceDetailPdf(invoiceId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<FileContentResult>(result);
        Assert.AreEqual("application/pdf", result.ContentType);
        Assert.AreEqual($"Invoice_{invoiceId}.pdf", result.FileDownloadName);
        Assert.AreEqual(pdfBytes.Length, result.FileContents.Length);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithInvalidInvoice_ReturnsNull()
    {
        // Arrange
        int invoiceId = 1;

        _accountingRepositoryMock.Setup(repo => repo.GetInvoiceById(invoiceId)).Returns((InvoiceDto)null);

        // Act
        var result = _accountingService.DownloadInvoiceDetailPdf(invoiceId);

        // Assert
        Assert.IsNull(result);
    }
}