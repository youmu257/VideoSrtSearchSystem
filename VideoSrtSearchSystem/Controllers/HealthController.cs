using Microsoft.AspNetCore.Mvc;

namespace VideoSrtSearchSystem.Controllers
{
    [ApiController]
    public class HealthController(ILogger<HealthController> _logger) : BaseController<HealthController>(_logger)
    {
        [HttpGet]
        [Route("health")]
        public IActionResult ImportSrt()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }
    }
}
