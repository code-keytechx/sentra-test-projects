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
    public void UpdateInvoice_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>
            {
                new DtoInvoiceLineItem { Description = "Item 1", Quantity = 1, UnitPrice = 100 }
            }
        };
        int invalidId = -1;

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, invalidId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void UpdateInvoice_WithNullDtoInvoice_ThrowsArgumentNullException()
    {
        // Arrange
        int validId = 1;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _accountingService.UpdateInvoice(null, validId));
    }

    [Test]
    public void UpdateInvoice_WithEmptyLineItems_UpdatesInvoiceWithoutLineItems()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>()
        };
        int validId = 1;
        var mockInvoice = new Invoice { Id = validId, CustomerName = "", InvoiceDate = DateTime.MinValue, TotalAmount = 0 };

        _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefault(i => i.Id == validId)).Returns(mockInvoice);

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, validId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(validId, result.Id);
        Assert.AreEqual(dtoInvoice.CustomerName, result.CustomerName);
        Assert.AreEqual(dtoInvoice.InvoiceDate, result.InvoiceDate);
        Assert.IsEmpty(result.LineItems);
    }
}