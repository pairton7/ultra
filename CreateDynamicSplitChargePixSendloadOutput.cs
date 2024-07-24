using UltraBank.Domain.ValueObjects;
using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.ChargesContext.Sendloads;

public readonly struct CreateDynamicSplitChargePixSendloadOutput
{
    public Guid ChargeId { get; }
    public DateTime CreatedAt { get; }
    public string Type { get; }
    public CreateDynamicSplitChargePixSendloadOutputPixInfo PixInfo { get; }
    public CreateDynamicSplitChargePixSendloadOutputMetadata[] Metadata { get; }
    public CreateDynamicSplitChargePixSendloadOutputOriginatorSplit OriginatorSplit { get; }
    public CreateDynamicSplitChargePixSendloadOutputSplit[] Secondaries { get; }
    public INotification[] Notifications { get; }

    private CreateDynamicSplitChargePixSendloadOutput(IdValueObject chargeId, DateTime createdAt, TypeChargePixValueObject type, ChargePixStatusValueObject status,
        TxidValueObject txid, PixCopyAndPasteValueObject copyAndPaste, PixAmountValueObject amount, PixExpirationValueObject expiresIn,
        string? description, MetadataValueObject[] metadata, string qrCodeBase64, INotification[] notifications,
        CreateDynamicSplitChargePixSendloadOutputOriginatorSplit originatorSplit,
        CreateDynamicSplitChargePixSendloadOutputSplit[] secondaries)
    {
        ChargeId = chargeId;
        CreatedAt = createdAt;
        Type = type.GetTypeChargePix().ToString();
        PixInfo = CreateDynamicSplitChargePixSendloadOutputPixInfo.Factory(
            status: status.GetPixStatus().ToString(),
            txid: txid,
            copyAndPaste: copyAndPaste,
            qrCodeBase64Image: qrCodeBase64,
            amount: amount,
            expiresIn: expiresIn,
            description: description);
        Metadata = metadata.Select(p => CreateDynamicSplitChargePixSendloadOutputMetadata.Factory(
            key: p.GetMetadataKey(),
            metadata: p.GetMetadata())).ToArray();
        Notifications = notifications;
        OriginatorSplit = originatorSplit;
        Secondaries = secondaries;
    }

    public static CreateDynamicSplitChargePixSendloadOutput Factory(IdValueObject chargeId, DateTime createdAt, TypeChargePixValueObject type, ChargePixStatusValueObject status,
        TxidValueObject txid, PixCopyAndPasteValueObject copyAndPaste, PixAmountValueObject amount, PixExpirationValueObject expiresIn,
        PixDescriptionValueObject description, MetadataValueObject[] metadata, string qrCodeBase64, INotification[] notifications,
        CreateDynamicSplitChargePixSendloadOutputOriginatorSplit originatorSplit,
        CreateDynamicSplitChargePixSendloadOutputSplit[] secondaries)
        => new(chargeId, createdAt, type, status, txid, copyAndPaste, amount, expiresIn, description, metadata, qrCodeBase64, notifications, originatorSplit,
            secondaries);
}

public readonly struct CreateDynamicSplitChargePixSendloadOutputMetadata
{
    public string Key { get; }
    public string Metadata { get; }

    private CreateDynamicSplitChargePixSendloadOutputMetadata(string key, string metadata)
    {
        Key = key;
        Metadata = metadata;
    }

    public static CreateDynamicSplitChargePixSendloadOutputMetadata Factory(string key, string metadata)
        => new(key, metadata);
}

public readonly struct CreateDynamicSplitChargePixSendloadOutputPixInfo
{
    public string Status { get; }
    public string Txid { get; }
    public string CopyAndPaste { get; }
    public string QrCodeBase64Image { get; }
    public decimal Amount { get; }
    public int ExpiresIn { get; }
    public string? Description { get; }

    private CreateDynamicSplitChargePixSendloadOutputPixInfo(string status, string txid, string copyAndPaste, string qrCodeBase64Image, decimal amount, int expiresIn, string? description)
    {
        Status = status;
        Txid = txid;
        CopyAndPaste = copyAndPaste;
        QrCodeBase64Image = qrCodeBase64Image;
        Amount = amount;
        ExpiresIn = expiresIn;
        Description = description;
    }

    public static CreateDynamicSplitChargePixSendloadOutputPixInfo Factory(string status, string txid, string copyAndPaste, string qrCodeBase64Image, decimal amount, int expiresIn,
        string? description)
        => new(status, txid, copyAndPaste, qrCodeBase64Image, amount, expiresIn, description);
}

public readonly struct CreateDynamicSplitChargePixSendloadOutputOriginatorSplit
{
    public CreateDynamicSplitChargePixSendloadOutputOriginatorSplit(string typeSplit, decimal amountSplit)
    {
        TypeSplit = typeSplit;
        AmountSplit = amountSplit;
    }

    public string TypeSplit { get; init; }
    public decimal AmountSplit { get; init; }
}

public readonly struct CreateDynamicSplitChargePixSendloadOutputSplit
{
    public CreateDynamicSplitChargePixSendloadOutputSplit(Guid receiverAccountId, string typeSplit, decimal amountSplit, string description, string extractGroup)
    {
        ReceiverAccountId = receiverAccountId;
        TypeSplit = typeSplit;
        AmountSplit = amountSplit;
        Description = description;
        ExtractGroup = extractGroup;
    }

    public Guid ReceiverAccountId { get; init; }
    public string TypeSplit { get; init; }
    public decimal AmountSplit { get; init; }
    public string Description { get; init; }
    public string ExtractGroup { get; init; }
}