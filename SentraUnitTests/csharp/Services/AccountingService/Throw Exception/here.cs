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
    [Test]
    public void GeneratePdf_WithNullInvoice_ThrowsArgumentNullException()
    {
        var mapperMock = new Mock<IMapper>();
        var factoryMock = new Mock<ICommandFactory>();

        var accountingService = new AccountingService(mapperMock.Object, factoryMock.Object);

        Assert.Throws<ArgumentNullException>(() => accountingService.GeneratePdf(null));
    }

    [Test]
    public void GeneratePdf_WithEmptyInvoiceData_ThrowsInvalidOperationException()
    {
        var mapperMock = new Mock<IMapper>();
        var factoryMock = new Mock<ICommandFactory>();

        var accountingService = new AccountingService(mapperMock.Object, factoryMock.Object);
        var invoice = new InvoiceDetailViewModel { /* Empty properties */ };

        Assert.Throws<InvalidOperationException>(() => accountingService.GeneratePdf(invoice));
    }

    [Test]
    public void GeneratePdf_WithInvalidInvoiceType_ThrowsArgumentException()
    {
        var mapperMock = new Mock<IMapper>();
        var factoryMock = new Mock<ICommandFactory>();

        var accountingService = new AccountingService(mapperMock.Object, factoryMock.Object);
        var invoice = new InvoiceDetailViewModel { Type = "InvalidType" };

        Assert.Throws<ArgumentException>(() => accountingService.GeneratePdf(invoice));
    }
}