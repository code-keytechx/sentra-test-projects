using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;

[TestClass]
public class AccountingServiceNegativeTests
{
    [TestMethod]
    public void GeneratePdf_WithInvalidInvoiceType_ThrowsArgumentException()
    {
        // Arrange
        var accountingService = new AccountingService();
        var invalidInvoice = new InvoiceDetailViewModel { Type = "Invalid" };

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => accountingService.GeneratePdf(invalidInvoice));
    }

    [TestMethod]
    public void GeneratePdf_WithNullInvoice_ThrowsArgumentNullException()
    {
        // Arrange
        var accountingService = new AccountingService();

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => accountingService.GeneratePdf(null));
    }

    [TestMethod]
    public void GeneratePdf_WithEmptyInvoiceData_ThrowsInvalidOperationException()
    {
        // Arrange
        var accountingService = new AccountingService();
        var emptyInvoice = new InvoiceDetailViewModel { Items = null };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => accountingService.GeneratePdf(emptyInvoice));
    }
}