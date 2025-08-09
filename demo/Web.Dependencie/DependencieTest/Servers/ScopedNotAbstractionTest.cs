using EasyCore.Dependencie;

namespace Web.Dependencie.DependencieTest.Servers
{
    public class ScopedNotAbstractionTest : IScopedDependencie
    {
        public string Test()
        {
            return "This is a ScopedNotAbstraction Test";
        }
    }
}
