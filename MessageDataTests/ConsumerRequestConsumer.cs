using MassTransit;

namespace MessageDataTests;


public class ConsumerRequestConsumer : IConsumer<ConsumerRequest>
{
    public async Task Consume(ConsumeContext<ConsumerRequest> context)
    {
        if (context.IsResponseAccepted<ConsumerResponse>())
            await context.RespondAsync<ConsumerResponse>(new
            {
                context.Message.CorrelationId,
                Payload = new InnerResponse[] {
                    new InnerResponse { Data =  new List<string> { "Test Data" } },
                    new InnerResponse { Data =  new List<string> { "Test Data 2" } }
                }
            });
    }
}