using UltraBank.Domain.ValueObjects;

namespace UltraBank.WebApi.Controllers.ChargesContext.Payloads;

public readonly struct RefundChargePixPayloadInput
{
    public RefundChargePixPayloadInput(decimal amount, string? description)
    {
        Amount = amount;
        Description = description;
    }

    public decimal Amount { get; init; }
    public string? Description { get; init; }
}
