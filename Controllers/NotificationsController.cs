using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using andead.netcore.notifications.Managers;
using andead.netcore.notifications.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace andead.netcore.notifications.Controllers
{
    [Route("api/[controller]")]
    public class NotificationsController : Controller
    {
        private readonly ILogger _logger;
        private NotificationTokens _tokens;
        private NotificationManager _notificationManager;

        public NotificationsController(
            ILogger<NotificationsController> logger,
            NotificationTokens tokens,
            NotificationManager notificationManager)
        {
            _logger = logger;
            _tokens = tokens;
            _notificationManager = notificationManager;
        }

        [HttpPost("add")]
        public IActionResult AddToken(int userId, [FromBody] DeviceToken request)
        {
            _logger.LogWarning(JsonConvert.SerializeObject(request));
            
            if (userId != 0 && !String.IsNullOrEmpty(request.token))
            {
                if (_tokens.AddToken(userId, request.token))
                {
                    _notificationManager.SubscribeTopic(request.token, userId.ToString());
                }

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

        [HttpGet("tokens")]
        public IActionResult GetMyTokens(int userId)
        {
            if (userId != 0)
            {
                return Ok(JsonConvert.SerializeObject(
                    new
                    {
                        tokens = _tokens.GetMyTokens(userId)
                    }
                ));
            }

            return BadRequest(JsonConvert.SerializeObject(
                new
                {
                    error = "invalid_user_id",
                    error_description = "User ID is invalid."
                })
            );
        }

        [HttpPost("send")]
        public IActionResult SendNotification(int userId, [FromBody] Notification notification)
        {
            _notificationManager.SendNotification(notification.token, notification.title, notification.body);

            return Ok(new
                {
                    success = DateTime.Now
                }
            );
        }

        [HttpPost("send-group")]
        public IActionResult SendGroupNotification(int userId, [FromBody] Notification notification)
        {
            _notificationManager.SendNotification($"/topics/{userId}", notification.title, notification.body);

            return Ok(new
                {
                    success = DateTime.Now
                }
            );
        }
    }
}