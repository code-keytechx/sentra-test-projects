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
    public void GeneratePdf_WithValidInvoice_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        var invoice = new InvoiceDetailViewModel { /* Initialize with valid data */ };

        // Act
        var result = accountingService.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [Test]
    public void GeneratePdf_WithNullInvoice_ReturnsEmptyByteArray()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        var invoice = null;

        // Act
        var result = accountingService.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }

    [Test]
    public void GeneratePdf_WithEmptyInvoice_ReturnsEmptyByteArray()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        var invoice = new InvoiceDetailViewModel(); // Empty object

        // Act
        var result = accountingService.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }
}