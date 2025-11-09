using EasyCore.Dependency;

namespace Web.Dependency.DependencyTest.Interfaces
{
    public interface ITransientTest : ITransientDependency
    {
        string Test();
    }
}
