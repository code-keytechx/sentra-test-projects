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
using System.Threading.Tasks;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<DbContext> _mockDbContext;
    private Mock<IMapper> _mockMapper;
    private IAccountingService _accountingService;

    [SetUp]
    public void SetUp()
    {
        _mockDbContext = new Mock<DbContext>();
        _mockMapper = new Mock<IMapper>();

        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Test]
    public async Task GetInvoiceById_WithValidId_ReturnsInvoiceDetailViewModel()
    {
        // Arrange
        int validId = 1;
        var mockInvoice = new Invoice
        {
            Id = validId,
            CustomerName = "Customer1",
            InvoiceDate = DateTime.Now,
            TotalAmount = 100.0m,
            LineItems = new List<LineItem>
            {
                new LineItem { Id = 1, Description = "Item1", Quantity = 1, UnitPrice = 50.0m, LineTotal = 50.0m },
                new LineItem { Id = 2, Description = "Item2", Quantity = 2, UnitPrice = 25.0m, LineTotal = 50.0m }
            }
        };

        _mockDbContext.Setup(db => db.Invoices.Where(i => i.Id == validId)).Returns(new List<Invoice> { mockInvoice }.AsQueryable());

        var expectedViewModel = new InvoiceDetailViewModel
        {
            Id = validId,
            CustomerName = "Customer1",
            InvoiceDate = DateTime.Now,
            TotalAmount = 100.0m,
            LineItems = new List<InvoiceLineItemViewModel>
            {
                new InvoiceLineItemViewModel { Id = 1, Description = "Item1", Quantity = 1, UnitPrice = 50.0m, LineTotal = 50.0m },
                new InvoiceLineItemViewModel { Id = 2, Description = "Item2", Quantity = 2, UnitPrice = 25.0m, LineTotal = 50.0m }
            }
        };

        _mockMapper.Setup(mapper => mapper.Map<InvoiceDetailViewModel>(mockInvoice)).Returns(expectedViewModel);

        // Act
        var result = await _accountingService.GetInvoiceById(validId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedViewModel.Id, result.Id);
        Assert.AreEqual(expectedViewModel.CustomerName, result.CustomerName);
        Assert.AreEqual(expectedViewModel.InvoiceDate, result.InvoiceDate);
        Assert.AreEqual(expectedViewModel.TotalAmount, result.TotalAmount);
        Assert.AreEqual(expectedViewModel.LineItems.Count, result.LineItems.Count);
        for (int i = 0; i < expectedViewModel.LineItems.Count; i++)
        {
            Assert.AreEqual(expectedViewModel.LineItems[i].Id, result.LineItems[i].Id);
            Assert.AreEqual(expectedViewModel.LineItems[i].Description, result.LineItems[i].Description);
            Assert.AreEqual(expectedViewModel.LineItems[i].Quantity, result.LineItems[i].Quantity);
            Assert.AreEqual(expectedViewModel.LineItems[i].UnitPrice, result.LineItems[i].UnitPrice);
            Assert.AreEqual(expectedViewModel.LineItems[i].LineTotal, result.LineItems[i].LineTotal);
        }
    }

    [Test]
    public async Task GetInvoiceById_WithInvalidId_ReturnsNull()
    {
        // Arrange
        int invalidId = 999;
        _mockDbContext.Setup(db => db.Invoices.Where(i => i.Id == invalidId)).Returns(new List<Invoice>().AsQueryable());

        // Act
        var result = await _accountingService.GetInvoiceById(invalidId);

        // Assert
        Assert.IsNull(result);
    }
}