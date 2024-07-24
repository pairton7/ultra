namespace UltraBank.WebApi.Controllers.WhitelabelContext.Payloads;

public readonly struct OAuthWhitelabelAuthenticationPayloadInput
{
    public OAuthWhitelabelAuthenticationPayloadInput(string grantType)
    {
        GrantType = grantType;
    }

    public string GrantType { get; init; }
}
