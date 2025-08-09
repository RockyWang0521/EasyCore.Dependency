using EasyCore.Dependencie;

namespace Web.Dependencie.DependencieTest.Servers
{
    public class SingletonNotAbstractionTest : ISingletonDependencie
    {
        public string Test()
        {
            return "This is a SingletonNotAbstraction test";
        }
    }
}
