using EasyCore.Dependencie.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EasyCore.Dependencie
{
    public static class UseDependencie
    {
        public static void EasyCoreDependencie(this IServiceCollection service)
        {
            var rootDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string[] dllFiles = Directory.GetFiles(rootDirectory, "*.dll", SearchOption.TopDirectoryOnly).Where(path =>
            {
                string fileName = Path.GetFileName(path);
                return !(fileName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) || fileName.StartsWith("System.", StringComparison.OrdinalIgnoreCase));
            }).ToArray();

            var baseType = typeof(IBaseDependencie);

            var singletonType = typeof(IScopedDependencie);

            var transientType = typeof(ITransientDependencie);

            var scopedType = typeof(ISingletonDependencie);

            foreach (var dll in dllFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dll);

                    var types = assembly.GetTypes().Where(type => baseType.IsAssignableFrom(type)).Where(type => !type.IsInterface).Where(type => !type.IsAbstract);

                    foreach (var type in types)
                    {
                        var Interface = type.GetInterfaces()
                            .Where(type => baseType.IsAssignableFrom(type) &&
                            type != singletonType && type != transientType &&
                            type != scopedType && type != baseType)
                            .FirstOrDefault();

                        if (Interface is not null)
                        {
                            var Ioctype = type.GetInterfaces().Where(i => i == singletonType || i == transientType || i == scopedType).FirstOrDefault();

                            if (Ioctype == singletonType) service.AddSingleton(Interface, type);

                            if (Ioctype == transientType) service.AddTransient(Interface, type);

                            if (Ioctype == scopedType) service.AddScoped(Interface, type);

                            continue;
                        }
                        else
                        {
                            var Ioctype = type.GetInterfaces().Where(i => i == singletonType || i == transientType || i == scopedType).FirstOrDefault();

                            if (Ioctype == singletonType) service.AddSingleton(type);

                            if (Ioctype == transientType) service.AddTransient(type);

                            if (Ioctype == scopedType) service.AddScoped(type);

                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
