using EasyCore.Dependency;

namespace Web.Dependency.DependencyTest.Interfaces
{
    public interface IScopedTest : IScopedDependency
    {
        string Test();
    }
}
