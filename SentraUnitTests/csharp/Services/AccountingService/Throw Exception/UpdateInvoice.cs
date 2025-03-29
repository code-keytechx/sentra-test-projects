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
public class AccountingServiceTests
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
    public void UpdateInvoice_WithNullDtoInvoice_ThrowsArgumentNullException()
    {
        // Arrange
        DtoUpdateInvoiceInput dtoInvoice = null;
        int id = 1;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.UpdateInvoice(dtoInvoice, id));
    }

    [Test]
    public void UpdateInvoice_WithNonExistentId_ThrowsInvalidOperationException()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>
            {
                new DtoInvoiceLineItem { Description = "Item 1", Quantity = 1, UnitPrice = 10 }
            }
        };
        int id = 999; // Non-existent ID

        _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefault(It.IsAny<int>())).Returns((Invoice)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _accountingService.UpdateInvoice(dtoInvoice, id));
    }
}