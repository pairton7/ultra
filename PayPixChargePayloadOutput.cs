namespace UltraBank.WebApi.Controllers.ChargesContext.Sendloads;

public readonly struct PayPixChargePayloadOutput
{
    public Guid CorrelationId { get; }
    public string EndToEndId { get; }
    public DateTime PaidAt { get; }

    private PayPixChargePayloadOutput(Guid correlationId, string endToEndId, DateTime paidAt)
    {
        CorrelationId = correlationId;
        EndToEndId = endToEndId;
        PaidAt = paidAt;
    }

    public static PayPixChargePayloadOutput Factory(Guid correlationId, string endToEndId, DateTime paidAt)
        => new(correlationId, endToEndId, paidAt);
}
