using EasyCore.Dependency;

namespace Web.Dependency.DependencyTest.Services
{
    public class TransientNotAbstractionTest : ITransientDependency
    {
        public string Test() => "This is a TransientNotAbstraction test";
    }
}
