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

        public bool AddToken(int userId, string userToken)
        {
            if (tokens.FirstOrDefault(t => t.Token == userToken) == null)
            {
                tokens.Add(new NotificationToken()
                {
                    UserId = userId,
                    Token = userToken
                });

                return true;
            }

            return false;
        }

        public IEnumerable<NotificationToken> GetMyTokens(int userId)
        {
            return tokens.Where(t => t.UserId == userId);
        }
    }
}