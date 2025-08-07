using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WarehouseApp.Data.Models;
using WarehouseApp.Data.Repository;
using WarehouseApp.Data.Repository.Interfaces;

namespace WarehouseApp.Web.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterRepositories(this IServiceCollection services, Assembly modelsAssembly, Assembly repositoriesAssembly)
        {
            // 1. Register generic base repositories for models
            Type[] typesToExclude = new Type[] { typeof(ApplicationUser) };

            Type[] modelTypes = modelsAssembly
                .GetTypes()
                .Where(t => !t.IsAbstract
                            && !t.IsInterface
                            && !t.IsEnum
                            && !t.Name.ToLower().EndsWith("attribute"))
                .ToArray();

            foreach (Type type in modelTypes)
            {
                if (!typesToExclude.Contains(type))
                {
                    Type repositoryInterface = typeof(IRepository<,>);
                    Type repositoryInstanceType = typeof(BaseRepository<,>);

                    PropertyInfo? idPropInfo = type
                        .GetProperties()
                        .SingleOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));

                    Type idType = idPropInfo?.PropertyType ?? typeof(object);

                    Type[] typeArgs = new Type[] { type, idType };
                    Type interfaceType = repositoryInterface.MakeGenericType(typeArgs);
                    Type implementationType = repositoryInstanceType.MakeGenericType(typeArgs);

                    services.AddScoped(interfaceType, implementationType);
                }
            }

            // 2. Register custom repositories from repositoriesAssembly
            Type[] customRepositories = repositoriesAssembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"))
                .ToArray();

            foreach (var implType in customRepositories)
            {
                var interfaceType = implType.GetInterfaces()
                    .FirstOrDefault(i => i.Name == $"I{implType.Name}");

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, implType);
                }
            }
        }

        public static void RegisterUserDefinedServices(this IServiceCollection services, Assembly serviceAssembly)
        {
            Type[] serviceInterfaceTypes = serviceAssembly
                .GetTypes()
                .Where(t => t.IsInterface)
                .ToArray();
            Type[] serviceTypes = serviceAssembly
                .GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract &&
                                t.Name.ToLower().EndsWith("service"))
                .ToArray();

            foreach (Type serviceInterfaceType in serviceInterfaceTypes)
            {
                Type? serviceType = serviceTypes
                    .SingleOrDefault(t => "i" + t.Name.ToLower() == serviceInterfaceType.Name.ToLower());
                if (serviceType == null)
                {
                    throw new NullReferenceException($"Service type could not be obtained for the service {serviceInterfaceType.Name}");
                }

                services.AddScoped(serviceInterfaceType, serviceType);
            }
        }
    }
}
