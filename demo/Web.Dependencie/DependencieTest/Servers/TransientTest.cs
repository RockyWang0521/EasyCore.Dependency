using Web.Dependencie.DependencieTest.Interfaces;

namespace Web.Dependencie.DependencieTest.Servers
{
    public class TransientTest : ITransientTest 
    {
        public string Test()
        {
            return "This is a Transient test";
        }
    }
}
