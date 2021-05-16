using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ShowingAds.Backend.NotifyService.Consumers;
using ShowingAds.Backend.NotifyService.Hubs;
using ShowingAds.Shared.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Settings.RabbitMqPath = configuration.GetValue<Uri>("RabbitMqPath");
            Settings.RabbitMqUsername = configuration.GetValue<string>("RabbitMqUsername");
            Settings.RabbitMqPassword = configuration.GetValue<string>("RabbitMqPassword");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateLifetime = true
                    };
                });
            services.AddSignalR();
            services.AddControllers();
            services.AddHealthChecks();
            services.AddSingleton<UserHub>();
            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.UseHealthCheck(provider);
                    cfg.Host(Settings.RabbitMqPath, h =>
                    {
                        h.Username(Settings.RabbitMqUsername);
                        h.Password(Settings.RabbitMqPassword);
                    });
                    cfg.ReceiveEndpoint("notify", op => op.Consumer<NotifyPacketConsumer>());
                }));
            });
            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(options => options.WithOrigins("http://localhost:8080")
                .AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            //app.UseCors(options => options.AllowAnyOrigin()
            //    .AllowAnyMethod().AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
                endpoints.MapHub<UserHub>("/user");
            });
        }
    }
}
