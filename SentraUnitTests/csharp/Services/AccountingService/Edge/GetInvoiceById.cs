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

[Collection("AccountingServiceTests")]
public class AccountingServiceEdgeTests
{
    private readonly Mock<IAccountingDbContext> _mockDbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AccountingService _accountingService;

    public AccountingServiceEdgeTests()
    {
        _mockDbContext = new Mock<IAccountingDbContext>();
        _mockMapper = new Mock<IMapper>();

        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Fact]
    public void GetInvoiceById_WithNegativeId_ReturnsNull()
    {
        // Arrange
        int negativeId = -1;

        // Act
        var result = _accountingService.GetInvoiceById(negativeId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_WithZeroId_ReturnsNull()
    {
        // Arrange
        int zeroId = 0;

        // Act
        var result = _accountingService.GetInvoiceById(zeroId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_WithMaxIntId_ReturnsNull()
    {
        // Arrange
        int maxIntId = int.MaxValue;

        // Act
        var result = _accountingService.GetInvoiceById(maxIntId);

        // Assert
        Assert.Null(result);
    }
}