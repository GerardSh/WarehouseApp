using System.ComponentModel.DataAnnotations;

using static WarehouseApp.Common.OutputMessages.ErrorMessages.AdminRequest;
using static WarehouseApp.Common.Constants.EntityConstants.AdminRequest;

namespace WarehouseApp.Web.ViewModels.Admin.UserManagement
{
    public class AdminRequestFormModel
    {
        [Required(ErrorMessage = RequestReason)]
        [MinLength(ReasonMinLength, ErrorMessage = RequestMinLength)]
        [MaxLength(ReasonMaxLength, ErrorMessage = RequestMaxLength)]
        public string Reason { get; set; } = string.Empty;
    }
}
