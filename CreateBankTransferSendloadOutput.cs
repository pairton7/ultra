using UltraBank.Domain.ValueObjects;
using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.TransferContext.Sendloads;

public readonly struct CreateBankTransferSendloadOutput
{
    public Guid TransferId { get; }
    public DateTime CreatedAt { get; }
    public string Type { get; }
    public decimal Amount { get; }
    public CreateBankTransferSendloadOutputMetadata[] Metadata { get; }
    public Guid BeneficiaryId { get; }
    public INotification[] Notifications { get; }

    private CreateBankTransferSendloadOutput(IdValueObject transferId, DateTime createdAt, TransferTypeValueObject type,
        TransferAmountValueObject amount, 
        MetadataValueObject[] metadata,
        IdValueObject beneficiaryId,
        INotification[] notifications)
    {
        TransferId = transferId;
        CreatedAt = createdAt;
        Type = type.GetTypeTransfer().ToString();
        Amount = amount;
        Metadata = metadata.Select(p => new CreateBankTransferSendloadOutputMetadata(
            key: p.GetMetadataKey(),
            metadata: p.GetMetadata())).ToArray();
        BeneficiaryId = beneficiaryId;
        Notifications = notifications;
    }

    public static CreateBankTransferSendloadOutput Factory(IdValueObject transferId, DateTime createdAt, TransferTypeValueObject type,
        TransferAmountValueObject amount, 
        MetadataValueObject[] metadata,
        IdValueObject beneficiaryId,
        INotification[] notifications)
        => new(transferId, createdAt, type, amount, metadata, beneficiaryId, notifications);
}

public readonly struct CreateBankTransferSendloadOutputMetadata
{
    public CreateBankTransferSendloadOutputMetadata(string key, string metadata)
    {
        Key = key;
        Metadata = metadata;
    }

    public string Key { get; }
    public string Metadata { get; }
}