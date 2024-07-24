using UltraBank.Domain.ValueObjects;
using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.AccountContext.Sendloads;

public readonly struct QueryDataBankAccountSendloadOutput
{
    public Guid AccountId { get; }
    public string Number { get; }
    public string Branch { get; }
    public string IspbCode { get; }
    public INotification[] Notifications { get; }

    private QueryDataBankAccountSendloadOutput(IdValueObject accountId, BankAccountNumberValueObject number, BankAccountBranchValueObject branch,
        IspbCodeValueObject ispbCode, INotification[] notifications)
    {
        AccountId = accountId;
        Number = number;
        Branch = branch;
        IspbCode = ispbCode;
        Notifications = notifications;
    }

    public static QueryDataBankAccountSendloadOutput Factory(IdValueObject accountId, BankAccountNumberValueObject number, BankAccountBranchValueObject branch,
        IspbCodeValueObject ispbCode, INotification[] notifications)
        => new(accountId, number, branch, ispbCode, notifications);
}
