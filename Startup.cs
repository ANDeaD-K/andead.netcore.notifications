using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using andead.netcore.notifications.Managers;
using andead.netcore.notifications.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace andead.netcore.notifications
{
    public class Startup
    {
        private const string PG_CONNECTION_STRING_KEY_NAME = "pg-connection-string";

        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(new NotificationTokens());
            services.AddSingleton<NotificationManager>();
            services.AddHttpClient();

            services.AddDbContext<NotificationContext>(options =>
            {
                options.UseNpgsql(_configuration.GetValue<string>(PG_CONNECTION_STRING_KEY_NAME, String.Empty));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
