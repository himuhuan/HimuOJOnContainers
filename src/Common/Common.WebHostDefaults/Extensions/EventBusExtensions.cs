using DotNetCore.CAP;
using HimuOJ.Common.WebHostDefaults.Infrastructure.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Serilog;

namespace HimuOJ.Common.WebHostDefaults.Extensions;

public static class EventBusExtensions
{
    public static IHostApplicationBuilder AddEventBus<TDbContext>(this IHostApplicationBuilder builder)
        where TDbContext : DbContext
    {
        IConfigurationSection eventBusSection = builder.Configuration.GetRequiredSection("EventBus");

        string eventBusTransportHost = eventBusSection.GetValue<string>("Host")
                                       ?? throw new InvalidOperationException("Event Bus Host is not configured");
        string eventBusTransportUser = eventBusSection.GetValue<string>("User")
                                       ?? throw new InvalidOperationException("Event Bus User is not configured");
        string eventBusTransportPassword = eventBusSection.GetValue<string>("Password")
                                           ?? throw new InvalidOperationException(
                                               "EventBus:Password is not configured");
        int eventBusTransportPort = eventBusSection.GetValue<int>("Port");
        if (eventBusTransportPort == 0)
        {
            throw new InvalidOperationException("Event Bus Port is not configured");
        }

        builder.Services.AddCap(x =>
        {
            x.UseEntityFramework<TDbContext>();
            x.UseRabbitMQ(opt =>
            {
                opt.HostName = eventBusTransportHost;
                opt.UserName = eventBusTransportUser;
                opt.Password = eventBusTransportPassword;
                opt.Port     = eventBusTransportPort;
            });
        });

        Log.Information("Connecting Event Bus: {Host}:{Port}", eventBusTransportHost, eventBusTransportPort);
        return builder;
    }

    public static IHostApplicationBuilder AddEventBus(this IHostApplicationBuilder builder, string connectionString)
    {
        IConfigurationSection eventBusSection = builder.Configuration.GetRequiredSection("EventBus");

        string eventBusTransportHost = eventBusSection.GetValue<string>("Host")
                                       ?? throw new InvalidOperationException("Event Bus Host is not configured");
        string eventBusTransportUser = eventBusSection.GetValue<string>("User")
                                       ?? throw new InvalidOperationException("Event Bus User is not configured");
        string eventBusTransportPassword = eventBusSection.GetValue<string>("Password")
                                           ?? throw new InvalidOperationException(
                                               "EventBus:Password is not configured");
        int eventBusTransportPort = eventBusSection.GetValue<int>("Port");
        if (eventBusTransportPort == 0)
        {
            throw new InvalidOperationException("Event Bus Port is not configured");
        }

        builder.Services.AddCap(x =>
        {
            x.UsePostgreSql(connectionString);
            x.UseRabbitMQ(opt =>
            {
                opt.HostName = eventBusTransportHost;
                opt.UserName = eventBusTransportUser;
                opt.Password = eventBusTransportPassword;
                opt.Port     = eventBusTransportPort;
            });
        });

        Log.Information("Connecting Event Bus: {Host}:{Port}", eventBusTransportHost, eventBusTransportPort);
        return builder;
    }

    public static void PublishEvent<TEvent>(this ICapPublisher bus, TEvent @event)
        where TEvent : IIntegrationEvent
    {
        bus.Publish(@event.EventName, @event);
    }

    public static async Task PublishEventAsync<TEvent>(this ICapPublisher bus, TEvent @event)
        where TEvent : IIntegrationEvent
    {
        await bus.PublishAsync(@event.EventName, @event);
    }
}