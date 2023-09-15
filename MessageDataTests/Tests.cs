using MassTransit;
using MassTransit.MessageData;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace MessageDataTests;

public class Tests
{
    private readonly ServiceCollection _services = new();
    protected readonly ServiceProvider ServiceProvider;

    public Tests(ITestOutputHelper output)
    {
        _services.AddLogging(l => l.AddXunit(output));
        _services.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddConsumersFromNamespaceContaining<ConsumerRequestConsumer>();
            cfg.AddFuture<FutureWithMessageData>();
            
            cfg.UsingInMemory((context, x) =>
            {
                x.UseMessageData(new InMemoryMessageDataRepository());
                x.UseMessageScope(context);
                x.ConfigureEndpoints(context);
            });
        });

        ServiceProvider = _services.BuildServiceProvider(true);
    }

    protected ITestHarness TestHarness => ServiceProvider.GetRequiredService<ITestHarness>();
    
    [Fact]
    public async Task Send_MessageData()
    {
        await TestHarness.Start();

        var client = TestHarness.GetRequestClient<Request>();
        var response = await client.GetResponse<Response>(new Request(InVar.CorrelationId));

         var payload = await response.Message.Payload.Value;
         Assert.Equal(2, payload.Length);
    }
}