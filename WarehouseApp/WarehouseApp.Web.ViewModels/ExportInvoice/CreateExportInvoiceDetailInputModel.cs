using System.ComponentModel.DataAnnotations;

using static WarehouseApp.Common.Constants.EntityConstants;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Product;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoiceDetail;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Category;

namespace WarehouseApp.Web.ViewModels.ExportInvoice
{
    public class CreateExportInvoiceDetailInputModel
    {
        [Required(ErrorMessage = ImportInvoiceNumberRequired)]
        public string ImportInvoiceNumber { get; set; } = null!;

        [Required(ErrorMessage = QuantityRequired)]
        [Range(1, int.MaxValue, ErrorMessage = QuantityRange)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = PriceRange)]
        public decimal? UnitPrice { get; set; }

        [Required(ErrorMessage = ProductNameRequired)]
        [MaxLength(Product.NameMaxLength, ErrorMessage = ProductNameMaxLength)]
        [MinLength(Product.NameMinLength, ErrorMessage = ProductNameMinLength)]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = CategoryNameRequired)]
        [MaxLength(Category.NameMaxLength, ErrorMessage = CategoryNameMaxLength)]
        [MinLength(Category.NameMinLength, ErrorMessage = CategoryNameMinLength)]
        public string CategoryName { get; set; } = null!;
    }
}
