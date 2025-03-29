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
public class AccountingServiceHappyTests
{
    private readonly Mock<IAccountingDbContext> _mockDbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AccountingService _accountingService;

    public AccountingServiceHappyTests()
    {
        _mockDbContext = new Mock<IAccountingDbContext>();
        _mockMapper = new Mock<IMapper>();

        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Fact]
    public void AddInvoice_WithValidInput_ReturnsNewInvoiceId()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoAddInvoiceLineItem>
            {
                new DtoAddInvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10.0m },
                new DtoAddInvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 5.0m }
            }
        };

        var expectedInvoiceId = 1;
        _mockDbContext.Setup(db => db.Invoices.Add(It.IsAny<Invoice>())).Callback(() => expectedInvoiceId++);
        _mockDbContext.Setup(db => db.SaveChanges()).Returns(1);

        // Act
        var result = _accountingService.AddInvoice(dtoInvoice);

        // Assert
        Assert.Equal(expectedInvoiceId - 1, result);
    }

    [Fact]
    public void AddInvoice_WithNoLineItems_ReturnsNewInvoiceId()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = "Jane Smith",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoAddInvoiceLineItem>()
        };

        var expectedInvoiceId = 1;
        _mockDbContext.Setup(db => db.Invoices.Add(It.IsAny<Invoice>())).Callback(() => expectedInvoiceId++);
        _mockDbContext.Setup(db => db.SaveChanges()).Returns(1);

        // Act
        var result = _accountingService.AddInvoice(dtoInvoice);

        // Assert
        Assert.Equal(expectedInvoiceId - 1, result);
    }

    [Fact]
    public void AddInvoice_WithMultipleLineItems_ReturnsNewInvoiceId()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = "Alice Johnson",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoAddInvoiceLineItem>
            {
                new DtoAddInvoiceLineItem { Description = "Item A", Quantity = 3, UnitPrice = 20.0m },
                new DtoAddInvoiceLineItem { Description = "Item B", Quantity = 5, UnitPrice = 15.0m }
            }
        };

        var expectedInvoiceId = 1;
        _mockDbContext.Setup(db => db.Invoices.Add(It.IsAny<Invoice>())).Callback(() => expectedInvoiceId++);
        _mockDbContext.Setup(db => db.SaveChanges()).Returns(1);

        // Act
        var result = _accountingService.AddInvoice(dtoInvoice);

        // Assert
        Assert.Equal(expectedInvoiceId - 1, result);
    }
}