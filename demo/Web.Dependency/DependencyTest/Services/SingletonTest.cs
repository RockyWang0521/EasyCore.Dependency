using Web.Dependency.DependencyTest.Interfaces;

namespace Web.Dependency.DependencyTest.Services
{
    public class SingletonTest : ISingletonTest
    {
        public string Test() => "This is a Singleton test";
    }
}
