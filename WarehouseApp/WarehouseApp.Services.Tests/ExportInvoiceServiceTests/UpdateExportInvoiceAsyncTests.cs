using Moq;
using MockQueryable;
using System.Linq.Expressions;

using WarehouseApp.Web.ViewModels.ExportInvoice;
using WarehouseApp.Data.Models;
using WarehouseApp.Services.Data.Models;

using WarehouseApp.Common.OutputMessages;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Application;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoiceDetail;

namespace WarehouseApp.Services.Tests.ExportInvoiceServiceTests
{
    [TestFixture]
    public class UpdateExportInvoiceAsyncTests : ExportInvoiceServiceBaseTests
    {
        private EditExportInvoiceInputModel inputModel;
        private ExportInvoice exportInvoice;

        [SetUp]
        public void TestSetup()
        {
            exportInvoice = exportInvoices[0];

            var importInvoiceDetails = new List<ImportInvoiceDetail>()
            {
               importInvoiceDetail1,
               importInvoiceDetail2
            }
            .AsQueryable()
            .BuildMock();

            var exportInvoiceDetails = new List<ExportInvoiceDetail>()
            {
                exportInvoiceDetail1,
                exportInvoiceDetail2
            }
            .AsQueryable()
            .BuildMock();

            inputModel = new EditExportInvoiceInputModel
            {
                Id = exportInvoice.Id,
                WarehouseId = exportInvoice.WarehouseId,
                InvoiceNumber = exportInvoice.InvoiceNumber,
                Date = exportInvoice.Date,
                ClientName = exportInvoice.Client.Name,
                ClientAddress = exportInvoice.Client.Address,
                ClientPhoneNumber = exportInvoice.Client.PhoneNumber,
                ClientEmail = exportInvoice.Client.Email,
                ExportedProducts = new List<EditExportInvoiceDetailInputModel>
                {
                    new EditExportInvoiceDetailInputModel
                    {
                        Id = exportInvoiceDetail1.Id,
                        ImportInvoiceNumber = exportInvoiceDetail1.ImportInvoiceDetail.ImportInvoice.InvoiceNumber,
                        ProductName = exportInvoiceDetail1.ImportInvoiceDetail.Product.Name,
                        CategoryName = exportInvoiceDetail1.ImportInvoiceDetail.Product.Category.Name,
                        Quantity = exportInvoiceDetail1.Quantity,
                        UnitPrice = exportInvoiceDetail1.UnitPrice
                    },
                    new EditExportInvoiceDetailInputModel
                    {
                        Id = exportInvoiceDetail2.Id,
                        ImportInvoiceNumber = exportInvoiceDetail2.ImportInvoiceDetail.ImportInvoice.InvoiceNumber,
                        ProductName = exportInvoiceDetail2.ImportInvoiceDetail.Product.Name,
                        CategoryName = exportInvoiceDetail2.ImportInvoiceDetail.Product.Category.Name,
                        Quantity = exportInvoiceDetail2.Quantity,
                        UnitPrice = exportInvoiceDetail2.UnitPrice
                    }
                }
            };

            exportInvoiceRepo.Setup(r => r.AllTracked())
                .Returns(new[] { exportInvoice }.AsQueryable().BuildMock());

            exportInvoiceRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ExportInvoice, bool>>>()))
                .ReturnsAsync(false);

            clientService.Setup(x => x.GetOrCreateOrUpdateClientAsync(
               inputModel.ClientName,
               inputModel.ClientAddress,
               inputModel.ClientPhoneNumber,
               inputModel.ClientEmail))
            .ReturnsAsync(OperationResult<Client>.Ok(exportClient));

            importInvoiceDetailRepo
                .Setup(r => r.AllTracked())
                .Returns(importInvoiceDetails);

            stockService
                .Setup(s => s.GetAvailableQuantityAsync(importInvoiceDetail1.Id, exportInvoiceDetail1.Id))
                .ReturnsAsync(OperationResult<int>.Ok(importInvoiceDetail1.Quantity));

            stockService
                .Setup(s => s.GetAvailableQuantityAsync(importInvoiceDetail2.Id, exportInvoiceDetail2.Id))
                .ReturnsAsync(OperationResult<int>.Ok(importInvoiceDetail2.Quantity));

            exportInvoiceDetailRepo
                .Setup(r => r.GetByIdAsync(exportInvoiceDetail1.Id))
                .ReturnsAsync(exportInvoiceDetail1);

            exportInvoiceDetailRepo
                .Setup(r => r.GetByIdAsync(exportInvoiceDetail2.Id))
                .ReturnsAsync(exportInvoiceDetail2);

            exportInvoiceDetailRepo
                .Setup(r => r.AllTracked())
                .Returns(exportInvoiceDetails);
        }

        [Test]
        public async Task ShouldUpdateImportInvoiceSuccessfully_WhenInputIsValid()
        {
            // Act
            var result = await exportInvoiceService.UpdateExportInvoiceAsync(inputModel, userId);

            // Assert
            Assert.That(result.Success, Is.True);
        }
    }
}