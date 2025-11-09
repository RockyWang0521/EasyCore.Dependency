using EasyCore.Dependency;

namespace Web.Dependency.DependencyTest.Services
{
    public class ScopedNotAbstractionTest : IScopedDependency
    {
        public string Test() => "This is a ScopedNotAbstraction Test";
    }
}
