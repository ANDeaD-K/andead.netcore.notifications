using System;
using System.Linq;
using andead.netcore.notifications.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

        [HttpPost("add")]
        public IActionResult AddToken(int userId, [FromBody] DeviceToken request)
        {
            _logger.LogWarning(JsonConvert.SerializeObject(request));
            
            if (userId != 0 && !String.IsNullOrEmpty(request.token))
            {
                _tokens.AddToken(userId, request.token);
                _logger.LogWarning(GetTokens());

                return Ok(JsonConvert.SerializeObject(
                    new
                    {
                        success = userId,
                        token = request.token
                    })
                );
            }

            return BadRequest(JsonConvert.SerializeObject(
                new
                {
                    error = "invalid_request",
                    error_description = "Token is invalid."
                })
            );
        }

        private string GetTokens()
        {
            var tokens = _tokens.GetTokens().Select(t => $"UserId: {t.UserId} = {t.Token}").ToArray();
            return $"Tokens: {String.Join(", ", tokens)}";
        }
    }
}