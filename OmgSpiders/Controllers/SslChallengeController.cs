using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace OmgSpiders.Controllers
{
    public class SslChallengeController: Controller
    {
        [HttpGet]
        [Route(".well-known/acme-challenge/6MXYWJEr7CfALrelWEDSvyW4IDYCwKQG956-j6XLoQU")]
        public IActionResult Verify1()
        {
            var result =
                new ContentResult()
                {
                    Content =
                        "6MXYWJEr7CfALrelWEDSvyW4IDYCwKQG956-j6XLoQU.-sKXD09lBjWRhHjjc3tndgGnZoSdrdCQEIJue9xS8k4",
                    ContentType = "text/plain"

                };
            return result;

        }
        [HttpGet]
        [Route(".well-known/acme-challenge/-iqzXANopS5eF7BsEAoTyLsANV3_QFu1lJiIJeXuYkA")]
        public IActionResult Verify2()
        {
            var result =
                new ContentResult()
                {
                    Content =
                        "-iqzXANopS5eF7BsEAoTyLsANV3_QFu1lJiIJeXuYkA.-sKXD09lBjWRhHjjc3tndgGnZoSdrdCQEIJue9xS8k4",
                    ContentType = "text/plain"

                };
            return result;

        }
        [HttpGet]
        [Route("robots.txt")]
        public IActionResult Robots()
        {
            var result =
                new ContentResult()
                {
                    Content =
                        "User-Agent: *\nDisallow:",
                    ContentType = "text/plain"

                };
            return result;

        }
    }
}