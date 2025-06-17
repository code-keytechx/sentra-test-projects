using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<DbContext> _mockDbContext;
    private Mock<IMapper> _mockMapper;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<DbContext>();
        _mockMapper = new Mock<IMapper>();

        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Test]
    public void AddInvoice_ShouldMapDtoToEntityAndSaveToDatabase()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoAddInvoiceLineItem>
            {
                new DtoAddInvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10 },
                new DtoAddInvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 5 }
            }
        };

        var expectedInvoice = new Invoice
        {
            CustomerName = dtoInvoice.CustomerName,
            InvoiceDate = dtoInvoice.InvoiceDate,
            TotalAmount = 30,
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10, LineTotal = 20 },
                new InvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 5, LineTotal = 5 }
            }
        };

        _mockMapper.Setup(m => m.Map<Invoice>(dtoInvoice)).Returns(expectedInvoice);

        // Act
        var result = _accountingService.AddInvoice(dtoInvoice);

        // Assert
        _mockDbContext.Verify(db => db.Invoices.Add(It.IsAny<Invoice>()), Times.Once);
        _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
        Assert.AreEqual(expectedInvoice.Id, result);
    }

    [Test]
    public void AddInvoice_ShouldHandleEmptyLineItems()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoAddInvoiceLineItem>()
        };

        var expectedInvoice = new Invoice
        {
            CustomerName = dtoInvoice.CustomerName,
            InvoiceDate = dtoInvoice.InvoiceDate,
            TotalAmount = 0,
            LineItems = new List<InvoiceLineItem>()
        };

        _mockMapper.Setup(m => m.Map<Invoice>(dtoInvoice)).Returns(expectedInvoice);

        // Act
        var result = _accountingService.AddInvoice(dtoInvoice);

        // Assert
        _mockDbContext.Verify(db => db.Invoices.Add(It.IsAny<Invoice>()), Times.Once);
        _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
        Assert.AreEqual(expectedInvoice.Id, result);
    }

    [Test]
    public void AddInvoice_ShouldThrowArgumentNullException_IfDtoIsNull()
    {
        // Arrange
        DtoAddInvoiceInput dtoInvoice = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _accountingService.AddInvoice(dtoInvoice));
    }
}