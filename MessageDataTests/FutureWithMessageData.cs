using MassTransit;

namespace MessageDataTests;

public class FutureWithMessageData: Future<Request, Response>
{
    public FutureWithMessageData()
    {
        ConfigureCommand(x => x.CorrelateById(context => context.Message.CorrelationId));

        SendRequest<ConsumerRequest>(x =>
        {
            x.UsingRequestFactory(_ => new ConsumerRequest(InVar.Id));
            x.TrackPendingRequest(message => message.CorrelationId);
        }).OnResponseReceived<ConsumerResponse>(x => x.CompletePendingRequest(message => message.CorrelationId));


        WhenAllCompleted(x => x.SetCompletedUsingInitializer(MapResponse));
    }
    private async Task<object> MapResponse(BehaviorContext<FutureState> context)
    {
        var response = context.SelectResults<ConsumerResponse>().Single();
        var payload = await response.Payload.Value;
        
        return new
        {
            response.CorrelationId,
            Payload = payload
        };
    }
}