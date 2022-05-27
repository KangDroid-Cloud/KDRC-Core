using MassTransit;

namespace KDRC_Core.Services;

public interface IEventService
{
    Task PublishMessageAsync<T>(T message) where T : class;
}

public class EventService : IEventService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventService(IPublishEndpoint endpoint)
    {
        _publishEndpoint = endpoint;
    }

    public async Task PublishMessageAsync<T>(T message) where T : class
    {
        await _publishEndpoint.Publish<T>(message);
    }
}