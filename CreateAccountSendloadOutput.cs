using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.AccountContext.Sendloads;

public readonly struct CreateAccountSendloadOutput
{
    public Guid AccountId { get; }
    public string TypeAccountStatus { get; }
    public INotification[] Notifications { get; }

    private CreateAccountSendloadOutput(Guid accountId, string typeAccountStatus, INotification[] notifications)
    {
        AccountId = accountId;
        TypeAccountStatus = typeAccountStatus;
        Notifications = notifications;
    }

    public static CreateAccountSendloadOutput Factory(Guid accountId, string typeAccountStatus, INotification[] notifications)
        => new(accountId, typeAccountStatus, notifications);
}
