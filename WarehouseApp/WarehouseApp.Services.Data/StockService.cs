using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data;
using WarehouseApp.Services.Data;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Services.Data.Models;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.Stock;

public class StockService : BaseService, IStockService
{
    private readonly WarehouseDbContext dbContext;

    public StockService(WarehouseDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<int>> GetAvailableQuantityAsync(Guid importInvoiceDetailId)
    {
        var importDetail = await dbContext.ImportInvoiceDetails
            .Include(iid => iid.ExportInvoicesPerProduct)
            .FirstOrDefaultAsync(iid => iid.Id == importInvoiceDetailId);

        if (importDetail == null)
            return OperationResult<int>.Failure(ProductNotFound);

        var exportedQty = importDetail.ExportInvoicesPerProduct.Sum(e => e.Quantity);
        var availableQty = importDetail.Quantity - exportedQty;

        return OperationResult<int>.Ok(availableQty);
    }
}
