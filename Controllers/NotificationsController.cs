using System;
using System.Linq;
using andead.netcore.notifications.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace andead.netcore.notifications.Controllers
{
    [Route("api/[controller]")]
    public class NotificationsController : Controller
    {
        private readonly ILogger _logger;
        private NotificationTokens _tokens;

        public NotificationsController(ILogger<NotificationsController> logger, NotificationTokens tokens)
        {
            _logger = logger;
            _tokens = tokens;
        }

        [HttpPost("add"), HttpGet("add")]
        public IActionResult AddToken(int userId, string userToken)
        {
            _tokens.AddToken(userId, userToken);

            return Ok($"This is Add method: {userId}");
        }

        [HttpGet("tokens")]
        public IActionResult GetTokens()
        {
            var tokens = _tokens.GetTokens().Select(t => $"{t.UserId} = {t.Token}").ToArray();
            _logger.LogWarning($"Tokens: {String.Join(", ", tokens)}");

            return Ok($"GetTokens");
        }
    }
}