using EasyCore.Dependency;
using EasyCore.Dependency.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyCore.Dependency.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void EasyCoreDependency_RegistersInterfaceAndConcreteServices()
        {
            var services = new ServiceCollection();
            services.EasyCoreDependency(typeof(ScopedSample).Assembly);

            using var provider = services.BuildServiceProvider();

            Assert.Equal("scoped", provider.GetRequiredService<IScopedSample>().Name);
            Assert.Equal("singleton", provider.GetRequiredService<ISingletonSample>().Name);
            Assert.Equal("transient", provider.GetRequiredService<ITransientSample>().Name);
            Assert.Equal("scoped-concrete", provider.GetRequiredService<ScopedConcreteSample>().Name);
        }

        [Fact]
        public void EasyCoreDependency_RegistersAllBusinessInterfaces()
        {
            var services = new ServiceCollection();
            services.EasyCoreDependency(typeof(MultiInterfaceSample).Assembly);

            using var provider = services.BuildServiceProvider();

            var first = provider.GetRequiredService<IMultiFirst>();
            var second = provider.GetRequiredService<IMultiSecond>();

            Assert.Same(first, second);
            Assert.IsType<MultiInterfaceSample>(first);
        }

        [Fact]
        public void Singleton_ReturnsSameInstance_AcrossScopes()
        {
            var services = new ServiceCollection();
            services.EasyCoreDependency(typeof(SingletonSample).Assembly);

            using var provider = services.BuildServiceProvider();
            using var scope1 = provider.CreateScope();
            using var scope2 = provider.CreateScope();

            var a = scope1.ServiceProvider.GetRequiredService<ISingletonSample>();
            var b = scope2.ServiceProvider.GetRequiredService<ISingletonSample>();

            Assert.Same(a, b);
        }

        [Fact]
        public void Scoped_ReturnsSameInstance_WithinScope_AndDifferentAcrossScopes()
        {
            var services = new ServiceCollection();
            services.EasyCoreDependency(typeof(ScopedSample).Assembly);

            using var provider = services.BuildServiceProvider();
            using var scope1 = provider.CreateScope();
            using var scope2 = provider.CreateScope();

            var a1 = scope1.ServiceProvider.GetRequiredService<IScopedSample>();
            var a2 = scope1.ServiceProvider.GetRequiredService<IScopedSample>();
            var b = scope2.ServiceProvider.GetRequiredService<IScopedSample>();

            Assert.Same(a1, a2);
            Assert.NotSame(a1, b);
        }

        [Fact]
        public void Transient_ReturnsDifferentInstances()
        {
            var services = new ServiceCollection();
            services.EasyCoreDependency(typeof(TransientSample).Assembly);

            using var provider = services.BuildServiceProvider();

            var a = provider.GetRequiredService<ITransientSample>();
            var b = provider.GetRequiredService<ITransientSample>();

            Assert.NotSame(a, b);
        }

        [Fact]
        public void EasyCoreDependency_CalledTwice_DoesNotThrow()
        {
            var services = new ServiceCollection();
            var assembly = typeof(ScopedSample).Assembly;

            services.EasyCoreDependency(assembly);
            services.EasyCoreDependency(assembly);

            using var provider = services.BuildServiceProvider();
            Assert.NotNull(provider.GetRequiredService<IScopedSample>());
        }

        [Fact]
        public void AbstractTypes_AreNotRegistered()
        {
            var services = new ServiceCollection();
            services.EasyCoreDependency(typeof(AbstractSample).Assembly);

            Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(AbstractSample));
            Assert.DoesNotContain(services, descriptor => descriptor.ImplementationType == typeof(AbstractSample));
        }

        [Fact]
        public void AssemblyPrefixFilter_LimitsRegistration()
        {
            var services = new ServiceCollection();
            services.EasyCoreDependency(options =>
            {
                options.AddAssemblies(typeof(ScopedSample).Assembly);
                options.AssemblyNamePrefixes.Add("DoesNotMatchAnything");
            });

            Assert.Empty(services);
        }

        [Fact]
        public void MultipleLifetimeMarkers_Throws()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                ServiceCollectionExtensions.ResolveLifetimeMarker(typeof(InvalidLifetimeSample)));

            Assert.Contains(nameof(InvalidLifetimeSample), ex.Message);
        }

        private abstract class InvalidLifetimeSample : IScopedDependency, ITransientDependency
        {
        }
    }
}
