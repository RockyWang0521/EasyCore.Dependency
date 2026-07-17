using System.Reflection;
using EasyCore.Dependency.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyCore.Dependency
{
    /// <summary>
    /// Extension methods for automatic dependency injection registration.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private static readonly Type BaseDependencyType = typeof(IBaseDependency);
        private static readonly Type SingletonDependencyType = typeof(ISingletonDependency);
        private static readonly Type TransientDependencyType = typeof(ITransientDependency);
        private static readonly Type ScopedDependencyType = typeof(IScopedDependency);

        /// <summary>
        /// Scans default assemblies and registers types that implement EasyCore dependency markers.
        /// </summary>
        public static IServiceCollection AddEasyCoreDependency(this IServiceCollection services)
        {
            return services.AddEasyCoreDependency(_ => { });
        }

        /// <summary>
        /// Scans the specified assemblies and registers marked dependency types.
        /// </summary>
        public static IServiceCollection AddEasyCoreDependency(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            return services.AddEasyCoreDependency(options => options.AddAssemblies(assemblies));
        }

        /// <summary>
        /// Scans assemblies according to <paramref name="configure"/> and registers marked dependency types.
        /// </summary>
        public static IServiceCollection AddEasyCoreDependency(
            this IServiceCollection services,
            Action<DependencyRegistrationOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configure);

            var options = new DependencyRegistrationOptions();
            configure(options);

            var assemblies = ResolveAssemblies(options);

            foreach (var assembly in assemblies)
            {
                foreach (var implementationType in GetConcreteDependencyTypes(assembly))
                {
                    RegisterType(services, implementationType);
                }
            }

            return services;
        }

        private static IReadOnlyList<Assembly> ResolveAssemblies(DependencyRegistrationOptions options)
        {
            IEnumerable<Assembly> candidates;

            if (options.Assemblies.Count > 0)
            {
                candidates = options.Assemblies;
            }
            else
            {
                var loaded = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(assembly => !assembly.IsDynamic)
                    .Where(assembly => !IsFrameworkAssembly(assembly));

                var entry = Assembly.GetEntryAssembly();
                if (entry is not null && !IsFrameworkAssembly(entry))
                {
                    candidates = loaded.Append(entry);
                }
                else
                {
                    candidates = loaded;
                }
            }

            if (options.AssemblyNamePrefixes.Count > 0)
            {
                candidates = candidates.Where(assembly =>
                    options.AssemblyNamePrefixes.Any(prefix =>
                        assembly.GetName().Name?.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) == true));
            }

            return candidates
                .Where(assembly => assembly is not null)
                .Distinct()
                .ToArray();
        }

        private static bool IsFrameworkAssembly(Assembly assembly)
        {
            var name = assembly.GetName().Name;
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            return name.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase)
                || name.Equals("System", StringComparison.OrdinalIgnoreCase)
                || name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase)
                || name.Equals("netstandard", StringComparison.OrdinalIgnoreCase);
        }

        private static IEnumerable<Type> GetConcreteDependencyTypes(Assembly assembly)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(type => type is not null).Cast<Type>().ToArray();
            }

            return types.Where(type =>
                type is { IsClass: true, IsAbstract: false }
                && !type.IsGenericTypeDefinition
                && BaseDependencyType.IsAssignableFrom(type));
        }

        private static void RegisterType(IServiceCollection services, Type implementationType)
        {
            var lifetimeMarker = ResolveLifetimeMarker(implementationType);
            if (lifetimeMarker is null)
            {
                return;
            }

            var serviceInterfaces = implementationType.GetInterfaces()
                .Where(IsBusinessDependencyInterface)
                .ToArray();

            if (serviceInterfaces.Length == 0)
            {
                TryAdd(services, implementationType, implementationType, lifetimeMarker);
                return;
            }

            if (serviceInterfaces.Length == 1)
            {
                TryAdd(services, serviceInterfaces[0], implementationType, lifetimeMarker);
                return;
            }

            // Multiple business interfaces: register concrete once, forward each interface to the same instance.
            TryAdd(services, implementationType, implementationType, lifetimeMarker);
            foreach (var serviceInterface in serviceInterfaces)
            {
                TryAddForward(services, serviceInterface, implementationType, lifetimeMarker);
            }
        }

        /// <summary>
        /// Resolves the single lifetime marker implemented by <paramref name="implementationType"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when more than one lifetime marker is implemented.
        /// </exception>
        internal static Type? ResolveLifetimeMarker(Type implementationType)
        {
            var markers = implementationType.GetInterfaces()
                .Where(type =>
                    type == SingletonDependencyType
                    || type == TransientDependencyType
                    || type == ScopedDependencyType)
                .Distinct()
                .ToArray();

            if (markers.Length == 0)
            {
                return null;
            }

            if (markers.Length > 1)
            {
                throw new InvalidOperationException(
                    $"Type '{implementationType.FullName}' implements multiple lifetime markers: " +
                    string.Join(", ", markers.Select(type => type.Name)) +
                    ". Implement only one of IScopedDependency, ISingletonDependency, or ITransientDependency.");
            }

            return markers[0];
        }

        private static bool IsBusinessDependencyInterface(Type interfaceType)
        {
            return interfaceType != BaseDependencyType
                && interfaceType != SingletonDependencyType
                && interfaceType != TransientDependencyType
                && interfaceType != ScopedDependencyType
                && BaseDependencyType.IsAssignableFrom(interfaceType);
        }

        private static void TryAdd(
            IServiceCollection services,
            Type serviceType,
            Type implementationType,
            Type lifetimeMarker)
        {
            var descriptor = CreateDescriptor(serviceType, implementationType, lifetimeMarker);
            services.TryAdd(descriptor);
        }

        private static void TryAddForward(
            IServiceCollection services,
            Type serviceType,
            Type implementationType,
            Type lifetimeMarker)
        {
            var lifetime = GetServiceLifetime(lifetimeMarker);
            var descriptor = ServiceDescriptor.Describe(
                serviceType,
                provider => provider.GetRequiredService(implementationType),
                lifetime);
            services.TryAdd(descriptor);
        }

        private static ServiceLifetime GetServiceLifetime(Type lifetimeMarker)
        {
            if (lifetimeMarker == SingletonDependencyType)
            {
                return ServiceLifetime.Singleton;
            }

            if (lifetimeMarker == TransientDependencyType)
            {
                return ServiceLifetime.Transient;
            }

            return ServiceLifetime.Scoped;
        }

        private static ServiceDescriptor CreateDescriptor(
            Type serviceType,
            Type implementationType,
            Type lifetimeMarker)
        {
            return ServiceDescriptor.Describe(serviceType, implementationType, GetServiceLifetime(lifetimeMarker));
        }
    }
}
