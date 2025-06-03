using Web.Dependencie.DependencieTest.Interfaces;

namespace Web.Dependencie.DependencieTest.Servers
{
    public class SingletonTest : ISingletonTest
    {
        public string Test()
        {
            return "This is a Singleton test";
        }
    }
}
