using Microsoft.EntityFrameworkCore;

namespace andead.netcore.notifications.Models
{
    public class NotificationContext : DbContext
    {
        public NotificationContext(DbContextOptions<NotificationContext> options) : base(options) { }
        public DbSet<WebNotification> notifications { get; set; }
    } 
}