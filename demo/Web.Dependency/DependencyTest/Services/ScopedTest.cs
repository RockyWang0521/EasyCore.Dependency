using Web.Dependency.DependencyTest.Interfaces;

namespace Web.Dependency.DependencyTest.Services
{
    public class ScopedTest : IScopedTest
    {
        public string Test() => "This is a Scoped test";
    }
}
