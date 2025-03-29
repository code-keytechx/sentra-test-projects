using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<IAccountingDocumentRepository> _repositoryMock;
    private Mock<IMapper> _mapperMock;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IAccountingDocumentRepository>();
        _mapperMock = new Mock<IMapper>();
        _accountingService = new AccountingService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNonExistentInvoiceId_ReturnsNotFound()
    {
        int nonExistentInvoiceId = 12345;
        _repositoryMock.Setup(repo => repo.GetById(nonExistentInvoiceId)).Returns((InvoiceDto)null);

        IActionResult result = _accountingService.PreviewInvoiceDetailHtml(nonExistentInvoiceId);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNegativeInvoiceId_ThrowsArgumentException()
    {
        int negativeInvoiceId = -1;

        Assert.Throws<ArgumentException>(() => _accountingService.PreviewInvoiceDetailHtml(negativeInvoiceId));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNullInvoice_ThrowsArgumentNullException()
    {
        int validInvoiceId = 1;
        _repositoryMock.Setup(repo => repo.GetById(validInvoiceId)).Returns(new InvoiceDto());

        Assert.Throws<ArgumentNullException>(() => _accountingService.PreviewInvoiceDetailHtml(validInvoiceId));
    }
}