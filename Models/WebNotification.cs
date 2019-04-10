using System;

namespace andead.netcore.notifications.Models
{
    public class WebNotification
    {
        public long id { get; set; }
        public DateTime date_time { get; set; }
        public string user_id { get; set; }
        public bool read { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}