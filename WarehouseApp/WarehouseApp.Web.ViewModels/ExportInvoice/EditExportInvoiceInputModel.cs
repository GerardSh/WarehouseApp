using System.ComponentModel.DataAnnotations;

using WarehouseApp.Common.Constants;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Client;
using static WarehouseApp.Common.Constants.EntityConstants.Client;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ExportInvoice;

namespace WarehouseApp.Web.ViewModels.ExportInvoice
{
    public class EditExportInvoiceInputModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid WarehouseId { get; set; }

        [Required(ErrorMessage = InvoiceNumberRequired)]
        [MaxLength(EntityConstants.ExportInvoice.InvoiceNumberMaxLength, ErrorMessage = InvoiceNumberMaxLength)]
        [MinLength(EntityConstants.ExportInvoice.InvoiceNumberMinLength, ErrorMessage = InvoiceNumberMinLength)]
        public string InvoiceNumber { get; set; } = null!;

        [Required(ErrorMessage = DateRequired)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = ClientNameRequired)]
        [MaxLength(NameMaxLength, ErrorMessage = ClientNameMaxLength)]
        [MinLength(NameMinLength, ErrorMessage = ClientNameMinLength)]
        public string ClientName { get; set; } = null!;

        [Required(ErrorMessage = ClientAddressRequired)]
        [MaxLength(AddressMaxLength, ErrorMessage = ClientAddressMaxLength)]
        [MinLength(AddressMinLength, ErrorMessage = ClientAddressMinLength)]
        public string ClientAddress { get; set; } = null!;

        [Phone(ErrorMessage = ClientPhoneNumberInvalid)]
        [MaxLength(PhoneNumberMaxLength, ErrorMessage = ClientPhoneNumberMaxLength)]
        [MinLength(PhoneNumberMinLength, ErrorMessage = ClientPhoneNumberMinLength)]
        public string? ClientPhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = ClientEmailInvalid)]
        [MaxLength(EmailMaxLength, ErrorMessage = ClientEmailMaxLength)]
        [MinLength(EmailMinLength, ErrorMessage = ClientEmailMinLength)]
        public string? ClientEmail { get; set; }

        public List<EditExportInvoiceDetailInputModel> ExportedProducts { get; set; }
            = new List<EditExportInvoiceDetailInputModel>();
    }
}