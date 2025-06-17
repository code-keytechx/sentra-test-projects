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
    public void CalculateInvoiceTotal_WithExistingInvoice_SumsLineItemsAndUpdatesTotal()
    {
        // Arrange
        int invoiceId = 1;
        var lineItem1 = new LineItem { Id = 1, InvoiceId = invoiceId, LineTotal = 100 };
        var lineItem2 = new LineItem { Id = 2, InvoiceId = invoiceId, LineTotal = 200 };
        var invoice = new Invoice { Id = invoiceId, LineItems = new List<LineItem> { lineItem1, lineItem2 } };

        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == invoiceId))
            .ReturnsAsync(invoice);

        // Act
        _accountingService.CalculateInvoiceTotal(invoiceId);

        // Assert
        Assert.AreEqual(300, invoice.TotalAmount);
        _mockDbContext.Verify(dbContext => dbContext.SaveChanges(), Times.Once());
    }

    [Test]
    public void CalculateInvoiceTotal_WithNonExistentInvoice_DoesNotUpdateTotal()
    {
        // Arrange
        int invoiceId = 1;
        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == invoiceId))
            .ReturnsAsync((Invoice)null);

        // Act
        _accountingService.CalculateInvoiceTotal(invoiceId);

        // Assert
        _mockDbContext.Verify(dbContext => dbContext.SaveChanges(), Times.Never());
    }

    [Test]
    public void CalculateInvoiceTotal_WithNoLineItems_SetsTotalToZero()
    {
        // Arrange
        int invoiceId = 1;
        var invoice = new Invoice { Id = invoiceId, LineItems = new List<LineItem>() };

        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == invoiceId))
            .ReturnsAsync(invoice);

        // Act
        _accountingService.CalculateInvoiceTotal(invoiceId);

        // Assert
        Assert.AreEqual(0, invoice.TotalAmount);
        _mockDbContext.Verify(dbContext => dbContext.SaveChanges(), Times.Once());
    }
}