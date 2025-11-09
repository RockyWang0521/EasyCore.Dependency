using EasyCore.Dependency;

namespace Web.Dependency.DependencyTest.Interfaces
{
    public interface ISingletonTest : ISingletonDependency
    {
        string Test();
    }
}
