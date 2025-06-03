using Microsoft.AspNetCore.Mvc;
using Web.Dependencie.DependencieTest.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Dependencie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DependencieController : ControllerBase
    {
        private readonly IScopedTest _scopedTest;
        private readonly ISingletonTest _singletonTest;
        private readonly ITransientTest _transientTest;

        public DependencieController(IScopedTest scopedTest, ISingletonTest singletonTest, ITransientTest transientTest)
        {
            _scopedTest = scopedTest;
            _singletonTest = singletonTest;
            _transientTest = transientTest;
        }

        [HttpGet("ScopedTest")]
        public string ScopedGet() => _scopedTest.Test();


        [HttpGet("SingletonTest")]
        public string SingletonGet() => _singletonTest.Test();

        [HttpGet("TransientTest")]
        public string TransientGet() => _transientTest.Test();
    }
}
