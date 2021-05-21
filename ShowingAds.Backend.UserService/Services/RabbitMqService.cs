using MassTransit;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Services
{
    public class RabbitMqService : BackgroundService
    {
        public RabbitMqService(IBus bus) => Settings.RabbitMq = bus;

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
    }
}
