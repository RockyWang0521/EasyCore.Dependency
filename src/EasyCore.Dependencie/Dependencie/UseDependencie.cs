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

            string[] dllFiles = Directory.GetFiles(rootDirectory, "*.dll");

            var baseType = typeof(IBaseDependencie);

            var singletonType = typeof(IScopedDependencie);

            var transientType = typeof(ITransientDependencie);

            var scopedType = typeof(ISingletonDependencie);

            foreach (var dll in dllFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dll);

                    var types = assembly.GetTypes() .Where(type => baseType.IsAssignableFrom(type)).Where(type => !type.IsInterface).Where(type => !type.IsAbstract);

                    foreach (var type in types)
                    {
                        var Interface = type.GetInterfaces().Where(type => baseType.IsAssignableFrom(type)).FirstOrDefault();

                        if (Interface is null) continue;

                        var Ioctype = type.GetInterfaces().Where(i => i == singletonType || i == transientType || i == scopedType).FirstOrDefault();

                        if (Ioctype == singletonType) service.AddSingleton(Interface, type);

                        if (Ioctype == transientType) service.AddTransient(Interface, type);

                        if (Ioctype == scopedType) service.AddScoped(Interface, type);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
