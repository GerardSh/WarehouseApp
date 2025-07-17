using System.ComponentModel.DataAnnotations;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Supplier;
using static WarehouseApp.Common.Constants.EntityConstants.Client;
using WarehouseApp.Common.OutputMessages;
using WarehouseApp.Common.Constants;

namespace WarehouseApp.Web.ViewModels.ImportInvoice
{
    public class CreateEditImportInvoiceInputModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = InvoiceNumberRequired)]
        [MaxLength(EntityConstants.ImportInvoice.InvoiceNumberMaxLength, ErrorMessage = ErrorMessages.ImportInvoice.InvoiceNumberMaxLength)]
        [MinLength(EntityConstants.ImportInvoice.InvoiceNumberMinLength, ErrorMessage = ErrorMessages.ImportInvoice.InvoiceNumberMaxLength)]
        public string InvoiceNumber { get; set; } = null!;

        [Required(ErrorMessage = DateRequired)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public Guid WarehouseId { get; set; }

        [Required(ErrorMessage = SupplierNameRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = SupplierNameMaxLength)]
        [MinLength(NameMinLength, ErrorMessage = SupplierNameMinLength)]
        public string SupplierName { get; set; } = null!;

        [Required(ErrorMessage = SupplierAddressRequired)]
        [MaxLength(AddressMaxLength, ErrorMessage = SupplierAddressMaxLength)]
        [MinLength(AddressMinLength, ErrorMessage = SupplierAddressMinLength)]
        public string SupplierAddress { get; set; } = null!;

        [Phone(ErrorMessage = SupplierPhoneNumberInvalid)]
        [MaxLength(PhoneNumberMaxLength, ErrorMessage = SupplierPhoneNumberMaxLength)]
        [MinLength(PhoneNumberMinLength, ErrorMessage = SupplierPhoneNumberMinLength)]
        public string? SupplierPhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = SupplierEmailInvalid)]
        [MaxLength(EmailMaxLength, ErrorMessage = SupplierEmailMaxLength)]
        [MinLength(EmailMinLength, ErrorMessage = SupplierEmailMinLength)]
        public string? SupplierEmail { get; set; }

        public List<ImportInvoiceDetailInputModel> Products { get; set; }
            = new List<ImportInvoiceDetailInputModel>();
    }
}