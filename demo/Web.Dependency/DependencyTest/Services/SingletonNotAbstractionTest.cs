using EasyCore.Dependency;

namespace Web.Dependency.DependencyTest.Services
{
    public class SingletonNotAbstractionTest : ISingletonDependency
    {
        public string Test() => "This is a SingletonNotAbstraction test";
    }
}
