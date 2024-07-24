namespace UltraBank.WebApi.Controllers.AccountContext.Payloads;

public readonly struct OAuthAccountAuthenticationPayloadInput
{
    public OAuthAccountAuthenticationPayloadInput(string grantType, string scope)
    {
        GrantType = grantType;
        Scope = scope;
    }

    public string GrantType { get; init; }
    public string Scope { get; init; }
}
