namespace Lagoon.Server.Controllers;

/// <summary>
/// Ping controller
/// </summary>
[Route("ping")]
public class LgPingController : Controller
{
    /// <summary>
    /// Return active application indicator
    /// </summary>
    /// <returns></returns>
    [HttpGet]        
    [AllowAnonymous]
    public IActionResult Get()
    {            
        return Ok("OK");
    }
}
