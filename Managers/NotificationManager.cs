using System;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace andead.netcore.notifications.Managers
{
    public class NotificationManager
    {
        private const string CLICK_ACTION   = "https://linux-docker.westeurope.cloudapp.azure.com";
        private const string ICON           = "https://linux-docker.westeurope.cloudapp.azure.com/images/icons/fraise-icon-72.png";
        private const string FCM_SERVER     = "https://fcm.googleapis.com/fcm/send";
        private const string SUBSCRIBE_URL  = "https://iid.googleapis.com/iid/v1/{0}/rel/topics/{1}";

        private readonly ILogger _logger;
        private HttpClient _httpClient;

        public NotificationManager(ILogger<NotificationManager> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;

            _httpClient = httpClientFactory.CreateClient();
            string serverKey = configuration.GetValue<string>("server-key", String.Empty);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        }

        public void SubscribeTopic(string token, string topic)
        {
            var response = _httpClient.PostAsync(String.Format(SUBSCRIBE_URL, token, topic), 
                                                new StringContent(String.Empty, Encoding.UTF8, "application/json")).Result;
            
            _logger.LogWarning(response.Content.ReadAsStringAsync().Result);
        }

        public void SendNotification(string token, string title, string body)
        {
            var fcmRrequest = JsonConvert.SerializeObject(new
            {
                notification = new
                {
                    title = title,
                    body = body,
                    click_action = CLICK_ACTION,
                    icon = ICON
                },
                to = token
            });

            var response = _httpClient.PostAsync(FCM_SERVER, new StringContent(fcmRrequest, Encoding.UTF8, "application/json")).Result;
            _logger.LogWarning(response.Content.ReadAsStringAsync().Result);
        }
    }
}