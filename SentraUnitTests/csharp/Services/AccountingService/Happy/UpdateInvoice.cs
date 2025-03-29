using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;

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
    public void UpdateInvoice_WithValidInputs_ReturnsUpdatedInvoiceDetailViewModel()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>
            {
                new DtoInvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10 },
                new DtoInvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 5 }
            }
        };

        var id = 1;
        var invoice = new Invoice
        {
            Id = id,
            CustomerName = "Old Name",
            InvoiceDate = DateTime.Now.AddDays(-1),
            LineItems = new List<InvoiceLineItem>()
        };

        _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == id))
            .ReturnsAsync(invoice);

        _mockDbContext.Setup(db => db.SaveChangesAsync()).ReturnsAsync(1);

        _mockMapper.Setup(mapper => mapper.Map<InvoiceDetailViewModel>(invoice)).Returns(new InvoiceDetailViewModel());

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, id).Result;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual(dtoInvoice.CustomerName, result.CustomerName);
        Assert.AreEqual(dtoInvoice.InvoiceDate, result.InvoiceDate);
        Assert.AreEqual(dtoInvoice.LineItems.Count, result.LineItems.Count);
    }

    [Test]
    public void UpdateInvoice_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput();
        var id = 1;

        _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == id))
            .ReturnsAsync((Invoice)null);

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, id).Result;

        // Assert
        Assert.IsNull(result);
    }
}