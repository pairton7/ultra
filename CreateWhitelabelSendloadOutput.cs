using UltraBank.Domain.BoundedContexts.WhitelabelContext.ENUMs;
using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.WhitelabelContext.Sendloads;

public readonly struct CreateWhitelabelSendloadOutput
{
    public Guid ClientId { get; }
    public Guid ClientSecret { get; }
    public string ComercialName { get; }
    public string Email { get; }
    public DateTime CreatedAt { get; }
    public INotification[] Notifications { get; }

    private CreateWhitelabelSendloadOutput(Guid clientId, Guid clientSecret, string comercialName, string email, DateTime createdAt,
        INotification[] notifications)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        ComercialName = comercialName;
        Email = email;
        CreatedAt = createdAt;
        Notifications = notifications;
    }

    public static CreateWhitelabelSendloadOutput Factory(Guid clientId, Guid clientSecret, string comercialName, string email, DateTime createdAt,
        INotification[] notifications)
        => new(clientId, clientSecret, comercialName, email, createdAt, notifications);
}
