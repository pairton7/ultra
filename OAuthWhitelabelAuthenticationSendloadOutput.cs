namespace UltraBank.WebApi.Controllers.WhitelabelContext.Sendloads;

public readonly struct OAuthWhitelabelAuthenticationSendloadOutput
{
    public string AccessToken { get; }
    public string TokenType { get; }
    public int ExpiresIn { get; }
    public string GrantType { get; }

    private OAuthWhitelabelAuthenticationSendloadOutput(string accessToken, string tokenType, int expiresIn, string grantType)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
        GrantType = grantType;
    }

    public static OAuthWhitelabelAuthenticationSendloadOutput Factory(string accessToken, string tokenType, int expiresIn, string grantType)
        => new(accessToken, tokenType, expiresIn, grantType);
}
