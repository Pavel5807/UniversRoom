using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using UniversRoom.BuildingBlocks.EventBus;
using UniversRoom.BuildingBlocks.EventBus.Abstractions;
using UniversRoom.BuildingBlocks.EventBusRabbitMQ;

namespace UniversRoom.Services.Common;

public static class CommonExtensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        builder.Services.AddEventBus(builder.Configuration);

        return builder;
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        //  {
        //    "ConnectionStrings": {
        //      "EventBus": "..."
        //    },

        // {
        //   "EventBus": {
        //     "ProviderName": "RabbitMQ",
        //     "SubscriptionClientName": "...",
        //     "UserName": "...",
        //     "Password": "...",
        //     "RetryCount": 1
        //   }
        // }

        // {
        //   "EventBus": {
        //     "ProviderName": "ServiceBus",
        //     "SubscriptionClientName": "eshop_event_bus"
        //   }
        // }

        var eventBusSection = configuration.GetSection("EventBus");

        if (eventBusSection.Exists() is false)
        {
            return services;
        }

        services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
        {
            var factory = new ConnectionFactory()
            {
                DispatchConsumersAsync = true,
                HostName = configuration.GetConnectionString("EventBus") ?? throw new InvalidOperationException($"Configuration missing value for: ConnectionStrings:EventBus"),
                UserName = eventBusSection["UserName"],
                Password = eventBusSection["Password"],
            };
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
            var retryCount = eventBusSection.GetValue("RetryCount", 5);

            return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
        });

        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
            var subscriptionClientName = eventBusSection["SubscriptionClientName"] ?? throw new InvalidOperationException($"Configuration missing value for: EventBus:SubscriptionClientName");
            var retryCount = eventBusSection.GetValue("RetryCount", 5);

            return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, sp, eventBusSubscriptionsManager, subscriptionClientName, retryCount);
        });

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        return services;
    }
}
