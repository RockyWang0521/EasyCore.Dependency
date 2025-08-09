using Microsoft.AspNetCore.Mvc;
using Web.Dependencie.DependencieTest.Interfaces;
using Web.Dependencie.DependencieTest.Servers;

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

        private readonly ScopedNotAbstractionTest _scopedNotAbstractionTest;
        private readonly SingletonNotAbstractionTest _singletonNotAbstractionTest;
        private readonly TransientNotAbstractionTest _transientNotAbstractionTest;

        public DependencieController(
            IScopedTest scopedTest,
            ISingletonTest singletonTest,
            ITransientTest transientTest,
            ScopedNotAbstractionTest scopedNotAbstractionTest,
            SingletonNotAbstractionTest singletonNotAbstractionTest,
            TransientNotAbstractionTest transientNotAbstractionTest)
        {
            _scopedTest = scopedTest;
            _singletonTest = singletonTest;
            _transientTest = transientTest;

            _scopedNotAbstractionTest = scopedNotAbstractionTest;
            _singletonNotAbstractionTest = singletonNotAbstractionTest;
            _transientNotAbstractionTest = transientNotAbstractionTest;
        }

        [HttpGet("ScopedTest")]
        public string ScopedGet() => _scopedTest.Test();


        [HttpGet("SingletonTest")]
        public string SingletonGet() => _singletonTest.Test();

        [HttpGet("TransientTest")]
        public string TransientGet() => _transientTest.Test();

        [HttpGet("ScopedNotAbstractionTest")]
        public string ScopedNotAbstractionGet() => _scopedNotAbstractionTest.Test();


        [HttpGet("SingletonNotAbstractionTest")]
        public string SingletonNotAbstractionGet() => _singletonNotAbstractionTest.Test();

        [HttpGet("TransientNotAbstractionTest")]
        public string TransientNotAbstractionGet() => _transientNotAbstractionTest.Test();
    }
}
