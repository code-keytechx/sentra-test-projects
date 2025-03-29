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
    private Mock<IAccountingDbContext> _mockDbContext;
    private Mock<IMapper> _mockMapper;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<IAccountingDbContext>();
        _mockMapper = new Mock<IMapper>();
        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Test]
    public void GetInvoiceById_WithInvalidId_ThrowsArgumentException()
    {
        // Arrange
        int invalidId = -1;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingService.GetInvoiceById(invalidId));
    }

    [Test]
    public void GetInvoiceById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        int nonExistentId = 99999;
        _mockDbContext.Setup(db => db.Invoices.Where(It.IsAny<Func<Invoice, bool>>())).Returns(new List<Invoice>());

        // Act
        var result = _accountingService.GetInvoiceById(nonExistentId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void GetInvoiceById_WithNullId_ThrowsArgumentNullException()
    {
        // Arrange
        int? nullId = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.GetInvoiceById(nullId.Value));
    }
}