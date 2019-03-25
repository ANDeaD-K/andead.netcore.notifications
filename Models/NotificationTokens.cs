
using System.Collections.Generic;
using System.Linq;

namespace andead.netcore.notifications.Models
{
    public class DeviceToken
    {
        public string token { get; set; }
    }

    public class NotificationToken
    {
        public int UserId { get; set; }

        public string Token { get; set; }
    }

    public class NotificationTokens
    {
        private List<NotificationToken> tokens = new List<NotificationToken>();

        public List<NotificationToken> GetTokens() => tokens;

        public void AddToken(int userId, string userToken)
        {
            NotificationToken token = tokens.FirstOrDefault(t => t.UserId == userId);

            if (token != null)
            {
                token.Token = userToken;
                return;
            }

            tokens.Add(new NotificationToken()
            {
                UserId = userId,
                Token = userToken
            });
        }
    }
}