using AutoMapper;
using Moq;
using NUnit.Framework;
using Sample.Api.Models;
using Sample.Api.Services.Accounting;
using Sample.Api.Services.Accounting.Dto;
using System;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<IMapper> _mapperMock;
    private AccountingService _accountingService;

    [SetUp]
    public void SetUp()
    {
        _mapperMock = new Mock<IMapper>();
        _accountingService = new AccountingService(_mapperMock.Object);
    }

    [Test]
    public void GeneratePdf_WithValidInvoice_ReturnsEmptyByteArray()
    {
        // Arrange
        var invoice = new InvoiceDetailViewModel { /* Initialize properties */ };

        // Act
        var result = _accountingService.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }

    [Test]
    public void GeneratePdf_WithNullInvoice_ThrowsArgumentNullException()
    {
        // Arrange
        InvoiceDetailViewModel invoice = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _accountingService.GeneratePdf(invoice));
    }
}