
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MediatR;

public abstract class BaseNotificationHandler<TNotification> : INotificationHandler<TNotification> where TNotification : IEvent
{
    protected readonly ILogger logger;

    public BaseNotificationHandler(ILogger logger)
    {
        this.logger = logger;
    }

    public async Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        string notifcationName = typeof(TNotification).Name;
        string assemblyName = typeof(TNotification)?.Assembly?.FullName?.Split(",", StringSplitOptions.RemoveEmptyEntries)?.FirstOrDefault() ?? "";
        logger.LogInformation("notification is recieved via ({Assembly},{notifcationName}) to be handled , {When}", assemblyName, notifcationName, notification.DateOccurred);
        try
        {
            await HandleAsync(notification, cancellationToken);
            logger.LogInformation("notification is handled via ({Assembly},{notifcationName})", assemblyName, notifcationName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "App ({Assembly}) could not handle notification ({notifcationName})", assemblyName, notifcationName);
            await HandleOnError(ex);
        }
    }

    protected abstract Task HandleAsync(TNotification notification, CancellationToken cancellationToken);

    protected virtual Task HandleOnError(Exception exp)
    {
        return Task.CompletedTask;
    }
}