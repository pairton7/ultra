using UltraBank.Domain.ValueObjects;
using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.AccountContext.Sendloads;

public readonly struct OAuthAccountAuthenticationSendloadOutput
{
    public string AccessToken { get; }
    public string TokenType { get; }
    public string GrantType { get; }
    public string Scope { get; }
    public int ExpiresIn { get; }
    public INotification[] Notifications { get; }

    private OAuthAccountAuthenticationSendloadOutput(string accessToken, string tokenType, string grantType, string scope, int expiresIn, INotification[] notifications)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        GrantType = grantType;
        Scope = scope;
        ExpiresIn = expiresIn;
        Notifications = notifications;
    }

    public static OAuthAccountAuthenticationSendloadOutput Factory(string accessToken, string tokenType, string grantType, string scope, int expiresIn,
        INotification[] notifications)
        => new(accessToken, tokenType, grantType, scope, expiresIn, notifications);
}
