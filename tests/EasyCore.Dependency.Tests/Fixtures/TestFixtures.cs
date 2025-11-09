using EasyCore.Dependency;

namespace EasyCore.Dependency.Tests.Fixtures
{
    public interface IScopedSample : IScopedDependency
    {
        string Name { get; }
    }

    public interface ISingletonSample : ISingletonDependency
    {
        string Name { get; }
    }

    public interface ITransientSample : ITransientDependency
    {
        string Name { get; }
    }

    public interface IMultiFirst : IScopedDependency
    {
    }

    public interface IMultiSecond : IScopedDependency
    {
    }

    public class ScopedSample : IScopedSample
    {
        public string Name => "scoped";
    }

    public class SingletonSample : ISingletonSample
    {
        public string Name => "singleton";
    }

    public class TransientSample : ITransientSample
    {
        public string Name => "transient";
    }

    public class ScopedConcreteSample : IScopedDependency
    {
        public string Name => "scoped-concrete";
    }

    public class MultiInterfaceSample : IMultiFirst, IMultiSecond
    {
    }

    public abstract class AbstractSample : IScopedDependency
    {
    }
}
