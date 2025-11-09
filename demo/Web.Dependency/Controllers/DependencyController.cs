using Microsoft.AspNetCore.Mvc;
using Web.Dependency.DependencyTest.Interfaces;
using Web.Dependency.DependencyTest.Services;

namespace Web.Dependency.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DependencyController : ControllerBase
    {
        private readonly IScopedTest _scopedTest;
        private readonly ISingletonTest _singletonTest;
        private readonly ITransientTest _transientTest;
        private readonly ScopedNotAbstractionTest _scopedNotAbstractionTest;
        private readonly SingletonNotAbstractionTest _singletonNotAbstractionTest;
        private readonly TransientNotAbstractionTest _transientNotAbstractionTest;

        public DependencyController(
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
