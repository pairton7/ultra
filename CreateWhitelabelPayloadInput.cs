namespace UltraBank.WebApi.Controllers.WhitelabelContext.Payloads;

public readonly struct CreateWhitelabelPayloadInput
{
    public CreateWhitelabelPayloadInput(string comercialName, string email)
    {
        ComercialName = comercialName;
        Email = email;
    }

    public string ComercialName { get; init; }
    public string Email { get; init; }
}
