using System.ComponentModel.DataAnnotations;

using WarehouseApp.Common.Constants;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Supplier;
using static WarehouseApp.Common.Constants.EntityConstants.Client;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ImportInvoice;

namespace WarehouseApp.Web.ViewModels.ImportInvoice
{
    public class CreateImportInvoiceInputModel
    {
        [Required]
        public Guid WarehouseId { get; set; }

        [Required(ErrorMessage = InvoiceNumberRequired)]
        [MaxLength(EntityConstants.ImportInvoice.InvoiceNumberMaxLength, ErrorMessage = InvoiceNumberMaxLength)]
        [MinLength(EntityConstants.ImportInvoice.InvoiceNumberMinLength, ErrorMessage = InvoiceNumberMinLength)]
        public string InvoiceNumber { get; set; } = null!;

        [Required(ErrorMessage = DateRequired)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

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

        public List<CreateImportInvoiceDetailInputModel> Products { get; set; }
            = new List<CreateImportInvoiceDetailInputModel>();
    }
}