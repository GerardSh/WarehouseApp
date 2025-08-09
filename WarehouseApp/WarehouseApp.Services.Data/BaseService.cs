using Microsoft.Extensions.Logging;

using WarehouseApp.Services.Data.Interfaces;

namespace WarehouseApp.Services.Data
{
    public abstract class BaseService : IBaseService
    {
        protected readonly ILogger logger;

        protected BaseService(ILogger logger)
        {
            this.logger = logger;
        }

        public bool IsGuidValid(string? id, ref Guid parsedGuid)
        {
            // Non-existing parameter in the URL
            if (String.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            // Invalid parameter in the URL
            bool isGuidValid = Guid.TryParse(id, out parsedGuid);
            if (!isGuidValid)
            {
                return false;
            }

            return true;
        }
    }
}
