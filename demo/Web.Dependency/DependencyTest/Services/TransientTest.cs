using Web.Dependency.DependencyTest.Interfaces;

namespace Web.Dependency.DependencyTest.Services
{
    public class TransientTest : ITransientTest
    {
        public string Test() => "This is a Transient test";
    }
}
