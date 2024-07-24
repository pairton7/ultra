using UltraBank.Domain.ValueObjects;
using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.ChargesContext.Sendloads;

public readonly struct CreateDynamicChargePixSendloadOutput
{
    public Guid ChargeId { get; }
    public DateTime CreatedAt { get; }
    public string Type { get; }
    public CreateDynamicChargePixSendloadOutputPixInfo PixInfo { get; }
    public CreateDynamicChargePixSendloadOutputMetadata[] Metadata { get; }
    public INotification[] Notifications { get; }

    private CreateDynamicChargePixSendloadOutput(IdValueObject chargeId, DateTime createdAt, TypeChargePixValueObject type, ChargePixStatusValueObject status,
        TxidValueObject txid, PixCopyAndPasteValueObject copyAndPaste, PixAmountValueObject amount, PixExpirationValueObject expiresIn,
        string? description, MetadataValueObject[] metadata, string qrCodeBase64, INotification[] notifications)
    {
        ChargeId = chargeId;
        CreatedAt = createdAt;
        Type = type.GetTypeChargePix().ToString();
        PixInfo = CreateDynamicChargePixSendloadOutputPixInfo.Factory(
            status: status.GetPixStatus().ToString(),
            txid: txid,
            copyAndPaste: copyAndPaste,
            qrCodeBase64Image: qrCodeBase64,
            amount: amount,
            expiresIn: expiresIn,
            description: description);
        Metadata = metadata.Select(p => CreateDynamicChargePixSendloadOutputMetadata.Factory(
            key: p.GetMetadataKey(),
            metadata: p.GetMetadata())).ToArray();
        Notifications = notifications;
    }

    public static CreateDynamicChargePixSendloadOutput Factory(IdValueObject chargeId, DateTime createdAt, TypeChargePixValueObject type, ChargePixStatusValueObject status,
        TxidValueObject txid, PixCopyAndPasteValueObject copyAndPaste, PixAmountValueObject amount, PixExpirationValueObject expiresIn,
        PixDescriptionValueObject description, MetadataValueObject[] metadata, string qrCodeBase64, INotification[] notifications)
        => new(chargeId, createdAt, type, status, txid, copyAndPaste, amount, expiresIn, description, metadata, qrCodeBase64, notifications);
}

public readonly struct CreateDynamicChargePixSendloadOutputMetadata
{
    public string Key { get; }
    public string Metadata { get; }

    private CreateDynamicChargePixSendloadOutputMetadata(string key, string metadata)
    {
        Key = key;
        Metadata = metadata;
    }

    public static CreateDynamicChargePixSendloadOutputMetadata Factory(string key, string metadata)
        => new(key, metadata);
}

public readonly struct CreateDynamicChargePixSendloadOutputPixInfo
{
    public string Status { get; }
    public string Txid { get; }
    public string CopyAndPaste { get; }
    public string QrCodeBase64Image { get; }
    public decimal Amount { get; }
    public int ExpiresIn { get; }
    public string? Description { get; }

    private CreateDynamicChargePixSendloadOutputPixInfo(string status, string txid, string copyAndPaste, string qrCodeBase64Image, decimal amount, int expiresIn, string? description)
    {
        Status = status;
        Txid = txid;
        CopyAndPaste = copyAndPaste;
        QrCodeBase64Image = qrCodeBase64Image;
        Amount = amount;
        ExpiresIn = expiresIn;
        Description = description;
    }

    public static CreateDynamicChargePixSendloadOutputPixInfo Factory(string status, string txid, string copyAndPaste, string qrCodeBase64Image, decimal amount, int expiresIn,
        string? description)
        => new(status, txid, copyAndPaste, qrCodeBase64Image, amount, expiresIn, description);
}