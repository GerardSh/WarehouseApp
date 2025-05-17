using AutoMapper;

namespace WarehouseApp.Services.Mapping
{
    public interface IHaveCustomMappings
    {
        void CreateMappings(IProfileExpression configuration);
    }
}
