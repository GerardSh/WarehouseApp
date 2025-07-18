using System.ComponentModel.DataAnnotations;

using static WarehouseApp.Common.Constants.EntityConstants;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Product;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoiceDetail;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Category;

namespace WarehouseApp.Web.ViewModels.ImportInvoice
{
    public class CreateImportInvoiceDetailInputModel
    {
        [Required(ErrorMessage = QuantityRequired)]
        [Range(1, int.MaxValue, ErrorMessage = QuantityRange)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = PriceRange)]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = ProductNameRequired)]
        [MaxLength(Product.NameMaxLength, ErrorMessage = ProductNameMaxLength)]
        [MinLength(Product.NameMinLength, ErrorMessage = ProductNameMinLength)]
        public string ProductName { get; set; } = null!;

        [MaxLength(Product.DescriptionMaxLength, ErrorMessage = ProductDescriptionMaxLength)]
        [MinLength(Product.DescriptionMinLength, ErrorMessage = ProductDescriptionMinLength)]
        public string? ProductDescription { get; set; }

        [Required(ErrorMessage = CategoryNameRequired)]
        [MaxLength(Category.NameMaxLength, ErrorMessage = CategoryNameMaxLength)]
        [MinLength(Category.NameMinLength, ErrorMessage = CategoryNameMinLength)]
        public string CategoryName { get; set; } = null!;

        
        [MaxLength(Category.DescriptionMaxLength, ErrorMessage = CategoryDescriptionMaxLength)]
        [MinLength(Category.DescriptionMinLength, ErrorMessage = CategoryDescriptionMinLength)]
        public string? CategoryDescription { get; set; }
    }
}
