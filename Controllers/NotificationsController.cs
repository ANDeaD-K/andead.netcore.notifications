using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public NotificationsController(
            ILogger<NotificationsController> logger,
            NotificationTokens tokens,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _tokens = tokens;
            _httpClientFactory = httpClientFactory;
            _config = configuration;
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
        public IActionResult SendNotification([FromBody] DeviceToken request)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            
            string serverKey = _config.GetValue<string>("server-key", String.Empty);

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var fcmRrequest = new
            {
                notification = new
                {
                    title = "Push уведомление",
                    body = "Тестовое сообщение",
                    click_action = "https://linux-docker.westeurope.cloudapp.azure.com/",
                    icon = "https://linux-docker.westeurope.cloudapp.azure.com/images/icons/fraise-icon-72.png"
                },
                to = request.token
            };

            var response = httpClient.PostAsJsonAsync("https://fcm.googleapis.com/fcm/send", fcmRrequest).Result;
             _logger.LogWarning(JsonConvert.SerializeObject(response.Content));

            return Ok(new
                {
                    success = request.token
                }
            );
        }
    }
}