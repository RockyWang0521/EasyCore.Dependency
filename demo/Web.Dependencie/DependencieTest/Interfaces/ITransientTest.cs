using EasyCore.Dependencie;

namespace Web.Dependencie.DependencieTest.Interfaces
{
    public interface ITransientTest : ITransientDependencie
    {
        string Test();
    }
}
