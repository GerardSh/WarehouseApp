using System.ComponentModel.DataAnnotations;

using static WarehouseApp.Common.Constants.EntityConstants.Warehouse;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.Warehouse;

namespace WarehouseApp.Web.ViewModels.Warehouse
{
    public class EditWarehouseInputModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = RequiredName)]
        [MaxLength(NameMaxLength-DeletedSuffixLength)]
        [MinLength(NameMinLength)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = RequiredAddress)]
        [MaxLength(AddressMaxLength)]
        [MinLength(AddressMinLength)]
        public string Address { get; set; } = null!;

        [Range(SizeMin, SizeMax, ErrorMessage = ZeroSize)]
        public double? Size { get; set; }
    }
}