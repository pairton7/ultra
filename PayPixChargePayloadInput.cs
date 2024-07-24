namespace UltraBank.WebApi.Controllers.ChargesContext.Payloads;

public readonly struct PayPixChargePayloadInput
{
    public PayPixChargePayloadInput(string pixCopiaECola)
    {
        PixCopiaECola = pixCopiaECola;
    }

    public string PixCopiaECola { get; init; }
}
