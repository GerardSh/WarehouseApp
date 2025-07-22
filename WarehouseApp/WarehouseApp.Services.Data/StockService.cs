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

    /// <summary>
    /// Returns available stock for the given import detail, optionally excluding one export detail.
    /// </summary>
    /// <param name="importDetailId">ID of the import detail to check.</param>
    /// <param name="excludeExportDetailId">Optional export detail to exclude from calculation.</param>
    /// <returns>Available quantity as an <see cref="OperationResult{T}"/>.</returns>
    public async Task<OperationResult<int>> GetAvailableQuantityAsync(
        Guid importDetailId,
        Guid? excludeExportDetailId = null)
    {
        var imported = await dbContext.ImportInvoiceDetails
            .Where(i => i.Id == importDetailId)
            .Select(i => (int?)i.Quantity)
            .FirstOrDefaultAsync() ?? 0;

        var exported = await dbContext.ExportInvoiceDetails
            .Where(e => e.ImportInvoiceDetailId == importDetailId &&
                (excludeExportDetailId == null || e.Id != excludeExportDetailId))
            .SumAsync(e => (int?)e.Quantity) ?? 0;

        var available = imported - exported;

        return OperationResult<int>.Ok(available);
    }
}
