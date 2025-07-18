using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Web.ViewModels.ImportInvoice
{
    public class EditImportInvoiceDetailInputModel : CreateImportInvoiceDetailInputModel
    {
        public Guid? Id { get; set; }
    }
}
