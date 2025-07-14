using System.ComponentModel.DataAnnotations;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Supplier;
using static WarehouseApp.Common.Constants.EntityConstants.Client;
using static WarehouseApp.Common.Constants.EntityConstants.ImportInvoice;

namespace WarehouseApp.Web.ViewModels.ImportInvoice
{
    public class CreateImportInvoiceInputModel
    {
        [Required(ErrorMessage = InvoiceNumberRequired)]
        [MaxLength(InvoiceNumberMaxLength, ErrorMessage = InvoiceNumberLength)]
        [MinLength(InvoiceNumberMinLength, ErrorMessage = InvoiceNumberLength)]
        public string InvoiceNumber { get; set; } = null!;

        [Required(ErrorMessage = DateRequired)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public Guid WarehouseId { get; set; }

        [Required(ErrorMessage = SupplierNameRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = SupplierNameLength)]
        [MinLength(NameMinLength, ErrorMessage = SupplierNameLength)]
        public string SupplierName { get; set; } = null!;

        [Required(ErrorMessage = SupplierAddressRequired)]
        [MaxLength(AddressMaxLength, ErrorMessage = SupplierAddressLength)]
        [MinLength(AddressMinLength, ErrorMessage = SupplierAddressLength)]
        public string SupplierAddress { get; set; } = null!;

        [Phone(ErrorMessage = SupplierPhoneNumberInvalid)]
        [MaxLength(PhoneNumberMaxLength, ErrorMessage = SupplierPhoneNumberLength)]
        [MinLength(PhoneNumberMinLength, ErrorMessage = SupplierPhoneNumberLength)]
        public string? SupplierPhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = SupplierEmailInvalid)]
        [MaxLength(EmailMaxLength, ErrorMessage = SupplierEmailLength)]
        [MinLength(EmailMinLength, ErrorMessage = SupplierEmailLength)]
        public string? SupplierEmail { get; set; }

        public List<ImportInvoiceDetailInputModel> Products { get; set; }
            = new List<ImportInvoiceDetailInputModel>();
    }
}