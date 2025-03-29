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
    public void CalculateInvoiceTotal_WithNonExistentId_ThrowsInvalidOperationException()
    {
        int nonExistentId = 99999;

        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include(It.IsAny<string>()).FirstOrDefault(It.IsAny<Func<Invoice, bool>>()))
            .Returns((Invoice)null);

        Assert.Throws<InvalidOperationException>(() => _accountingService.CalculateInvoiceTotal(nonExistentId));
    }

    [Test]
    public void CalculateInvoiceTotal_WithNegativeId_ThrowsArgumentException()
    {
        int negativeId = -1;

        Assert.Throws<ArgumentException>(() => _accountingService.CalculateInvoiceTotal(negativeId));
    }

    [Test]
    public void CalculateInvoiceTotal_WithNullLineItems_ThrowsInvalidOperationException()
    {
        int validId = 1;
        var invoice = new Invoice { Id = validId };
        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include(It.IsAny<string>()).FirstOrDefault(It.IsAny<Func<Invoice, bool>>()))
            .Returns(invoice);

        invoice.LineItems = null;

        Assert.Throws<InvalidOperationException>(() => _accountingService.CalculateInvoiceTotal(validId));
    }
}