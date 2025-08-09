using EasyCore.Dependencie;

namespace Web.Dependencie.DependencieTest.Servers
{
    public class TransientNotAbstractionTest : ITransientDependencie
    {
        public string Test()
        {
            return "This is a TransientNotAbstraction test";
        }
    }
}
