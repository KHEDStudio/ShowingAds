using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShowingAds.Backend.UserService.ChannelJsonHandlers;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Backend.Models.DeviceService;
using ShowingAds.Shared.Backend.Models.NotifyService;
using ShowingAds.Shared.Core.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using ShowingAds.Shared.Backend;
using ShowingAds.Backend.UserService.Consumers;
using ShowingAds.Backend.UserService.Services;

namespace ShowingAds.Backend.UserService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Settings.DjangoPath = configuration.GetValue<Uri>("DjangoPath");
            Settings.RabbitMqPath = configuration.GetValue<Uri>("RabbitMqPath");
            Settings.RabbitMqUsername = configuration.GetValue<string>("RabbitMqUsername");
            Settings.RabbitMqPassword = configuration.GetValue<string>("RabbitMqPassword");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            ChannelJsonPublisher.GetInstance();
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
            services.AddControllers().AddNewtonsoftJson();
            services.AddHealthChecks();
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
                    cfg.Publish<NotifyPacket>(x => x.BindQueue("notify", "notify"));
                    cfg.Publish<NotifyChannelJson>(x => x.BindQueue("channel_json", "channel_json"));
                    cfg.ReceiveEndpoint("update_channels", op => op.Consumer<ChannelJsonUpdaterConsumer>());
                }));
            });
            services.AddMassTransitHostedService(true);
            services.AddHostedService<RabbitMqService>();
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

            app.UseCors(options => options.AllowAnyOrigin()
                .AllowAnyMethod().AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
