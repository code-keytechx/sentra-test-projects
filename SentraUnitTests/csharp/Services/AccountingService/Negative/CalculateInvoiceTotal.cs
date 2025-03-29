using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using Xunit;

[Collection("Database Collection")]
public class AccountingServiceNegativeTests
{
    private readonly Mock<IAccountingDbContext> _mockDbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AccountingService _accountingService;

    public AccountingServiceNegativeTests()
    {
        _mockDbContext = new Mock<IAccountingDbContext>();
        _mockMapper = new Mock<IMapper>();

        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Fact]
    public void CalculateInvoiceTotal_WithInvalidId_ThrowsArgumentException()
    {
        // Arrange
        int invalidId = -1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _accountingService.CalculateInvoiceTotal(invalidId));
    }

    [Fact]
    public void CalculateInvoiceTotal_WithNonExistentId_ThrowsInvalidOperationException()
    {
        // Arrange
        int nonExistentId = 999999;

        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include(It.IsAny<string>()).FirstOrDefault(It.IsAny<Func<Invoice, bool>>()))
            .Returns((Invoice)null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _accountingService.CalculateInvoiceTotal(nonExistentId));
    }

    [Fact]
    public void CalculateInvoiceTotal_WithNullLineItems_ThrowsInvalidOperationException()
    {
        // Arrange
        int validId = 1;
        var invoice = new Invoice { Id = validId };
        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include(It.IsAny<string>()).FirstOrDefault(It.IsAny<Func<Invoice, bool>>()))
            .Returns(invoice);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _accountingService.CalculateInvoiceTotal(validId));
    }
}