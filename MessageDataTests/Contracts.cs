using MassTransit;

namespace MessageDataTests;

public record Request(Guid CorrelationId);
//MessageDataTests requires parameterless constructor
public record Response
{
    public Guid CorrelationId { get; init; }
    public MessageData<InnerResponse[]> Payload { get; init; } = null!;
}

public record InnerResponse
{
    public IReadOnlyList<string> Data { get; init; } = null!;
}

public record ConsumerRequest(Guid CorrelationId);
public record ConsumerResponse
{
    public Guid CorrelationId { get; init; }
    public MessageData<InnerResponse[]> Payload { get; init; } = null!;
}