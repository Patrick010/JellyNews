using System;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;

namespace Jellyfin.Plugin.JellyNews.Controllers
{
    /// <summary>
    /// The JellyNews controller.
    /// </summary>
    [ApiController]
    [Route("jellynews")]
    public class JellyNewsController : ControllerBase
    {
        /// <summary>
        /// Sets the log level.
        /// </summary>
        /// <param name="level">The new log level.</param>
        [HttpPost("loglevel")]
        public void SetLogLevel([FromBody] string level)
        {
            if (Enum.TryParse(level, true, out LogEventLevel newLevel))
            {
                Plugin.SetLogLevel(newLevel);
                Plugin.Log?.Information("Admin set log level to {Level}", newLevel);
            }
        }
    }
}
