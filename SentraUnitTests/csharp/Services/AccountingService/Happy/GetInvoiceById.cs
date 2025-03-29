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
    public void GetInvoiceById_WithValidId_ReturnsInvoiceDetailViewModel()
    {
        // Arrange
        int invoiceId = 1;
        var mockInvoice = new Invoice
        {
            Id = invoiceId,
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            TotalAmount = 100.00m,
            LineItems = new List<LineItem>
            {
                new LineItem { Id = 1, Description = "Item 1", Quantity = 1, UnitPrice = 50.00m, LineTotal = 50.00m },
                new LineItem { Id = 2, Description = "Item 2", Quantity = 2, UnitPrice = 25.00m, LineTotal = 50.00m }
            }
        };

        _mockDbContext.Setup(db => db.Invoices.Where(i => i.Id == invoiceId)).Returns(new[] { mockInvoice }.AsQueryable());

        var expectedViewModel = new InvoiceDetailViewModel
        {
            Id = invoiceId,
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            TotalAmount = 100.00m,
            LineItems = new List<InvoiceLineItemViewModel>
            {
                new InvoiceLineItemViewModel { Id = 1, Description = "Item 1", Quantity = 1, UnitPrice = 50.00m, LineTotal = 50.00m },
                new InvoiceLineItemViewModel { Id = 2, Description = "Item 2", Quantity = 2, UnitPrice = 25.00m, LineTotal = 50.00m }
            }
        };

        _mockMapper.Setup(mapper => mapper.Map<InvoiceDetailViewModel>(mockInvoice)).Returns(expectedViewModel);

        // Act
        var result = _accountingService.GetInvoiceById(invoiceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedViewModel.Id, result.Id);
        Assert.Equal(expectedViewModel.CustomerName, result.CustomerName);
        Assert.Equal(expectedViewModel.InvoiceDate, result.InvoiceDate);
        Assert.Equal(expectedViewModel.TotalAmount, result.TotalAmount);
        Assert.Collection(result.LineItems, 
            item1 => 
            {
                Assert.Equal(1, item1.Id);
                Assert.Equal("Item 1", item1.Description);
                Assert.Equal(1, item1.Quantity);
                Assert.Equal(50.00m, item1.UnitPrice);
                Assert.Equal(50.00m, item1.LineTotal);
            },
            item2 => 
            {
                Assert.Equal(2, item2.Id);
                Assert.Equal("Item 2", item2.Description);
                Assert.Equal(2, item2.Quantity);
                Assert.Equal(25.00m, item2.UnitPrice);
                Assert.Equal(50.00m, item2.LineTotal);
            });
    }

    [Fact]
    public void GetInvoiceById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        int nonExistentId = 999;
        _mockDbContext.Setup(db => db.Invoices.Where(i => i.Id == nonExistentId)).Returns(new List<Invoice>().AsQueryable());

        // Act
        var result = _accountingService.GetInvoiceById(nonExistentId);

        // Assert
        Assert.Null(result);
    }
}