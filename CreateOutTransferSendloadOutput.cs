using UltraBank.Domain.ValueObjects;
using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.TransferContext.Sendloads;

public readonly struct CreateOutBankTransferSendloadOutput
{
    public Guid TransferId { get; }
    public DateTime CreatedAt { get; }
    public string Type { get; }
    public string Status { get; }
    public decimal Amount { get; }
    public Guid IssuerId { get; }
    public CreateOutBankTransferSendloadOutputParticipant Beneficiary { get; }
    public CreateOutBankTransferSendloadOutputMetadata[] Metadata { get; }
    public INotification[] Notifications { get; }

    private CreateOutBankTransferSendloadOutput(IdValueObject transferId, DateTime createdAt, TransferTypeValueObject type, TransferStatusValueObject status,
        TransferAmountValueObject amount, IdValueObject issuerId,
        CreateOutBankTransferSendloadOutputParticipant beneficiary, MetadataValueObject[] metadata,
        INotification[] notifications)
    {
        TransferId = transferId;
        CreatedAt = createdAt;
        Type = type.GetTypeTransfer().ToString();
        Status = status.GetTransferStatus().ToString();
        Amount = amount;
        IssuerId = issuerId;
        Beneficiary = beneficiary;
        Metadata = metadata.Select(p => new CreateOutBankTransferSendloadOutputMetadata(
            key: p.GetMetadataKey(),
            metadata: p.GetMetadata())).ToArray();
        Notifications = notifications;
    }

    public static CreateOutBankTransferSendloadOutput Factory(IdValueObject transferId, DateTime createdAt, TransferTypeValueObject type, TransferStatusValueObject status,
        TransferAmountValueObject amount, IdValueObject issuerId,
        CreateOutBankTransferSendloadOutputParticipant beneficiary, MetadataValueObject[] metadata,
        INotification[] notifications)
        => new(transferId, createdAt, type, status, amount, issuerId, beneficiary, metadata, notifications);
}

public readonly struct CreateOutBankTransferSendloadOutputMetadata
{
    public CreateOutBankTransferSendloadOutputMetadata(string key, string metadata)
    {
        Key = key;
        Metadata = metadata;
    }

    public string Key { get; }
    public string Metadata { get; }
}

public readonly struct CreateOutBankTransferSendloadOutputParticipant
{
    public string Type { get; }
    public CreateOutBankTransferSendloadOutputParticipantAccount Account { get; }
    public string BankName { get; }
    public string BankIspb { get; }

    private CreateOutBankTransferSendloadOutputParticipant(TransferParticipantTypeValueObject type,
       CreateOutBankTransferSendloadOutputParticipantAccount account, DebtorNameValueObject bankName, BankAccountNumberValueObject bankIspb)
    {
        Type = type.GetTypeTransferParticipant().ToString();
        Account = account;
        BankName = bankName;
        BankIspb = bankIspb;
    }

    public static CreateOutBankTransferSendloadOutputParticipant Factory(TransferParticipantTypeValueObject type,
       CreateOutBankTransferSendloadOutputParticipantAccount account, DebtorNameValueObject bankName, BankAccountNumberValueObject bankIspb)
        => new(type, account, bankName, bankIspb);
}

public readonly struct CreateOutBankTransferSendloadOutputParticipantAccount
{
    public string Number { get; }
    public string Branch { get; }
    public string HolderDocument { get; }
    public string HolderName { get; }

    private CreateOutBankTransferSendloadOutputParticipantAccount(BankAccountNumberValueObject number,
        BankAccountBranchValueObject branch, DocumentValueObject holderDocument, DebtorNameValueObject holderName)
    {
        Number = number;
        Branch = branch;
        HolderDocument = holderDocument;
        HolderName = holderName;
    }

    public static CreateOutBankTransferSendloadOutputParticipantAccount Factory(BankAccountNumberValueObject number,
        BankAccountBranchValueObject branch, DocumentValueObject holderDocument, DebtorNameValueObject holderName)
        => new(number, branch, holderDocument, holderName);
}