using Web.Dependencie.DependencieTest.Interfaces;

namespace Web.Dependencie.DependencieTest.Servers
{
    public class ScopedTest : IScopedTest
    {
        public string Test()
        {
            return "This is a Scoped test";
        }
    }
}
