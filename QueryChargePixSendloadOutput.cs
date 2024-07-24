using System.Text.Json.Serialization;
using UltraBank.Domain.ValueObjects;
using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.ChargesContext.Sendloads;

public readonly struct QueryChargePixSendloadOutput
{
    public Guid ChargeId { get; }
    public DateTime CreatedAt { get; }
    public string Type { get; }
    public string Status { get; }
    public string Txid { get; }
    public decimal Amount { get; }
    public string? Description { get; }
    public QueryChargePixSendloadOutputMetadata[] Metadata { get; }
    public QueryChargePixSendloadOutputPaid[] Payments { get; }
    public string[] NotificationUrls { get; }
    public Guid IssuedBy { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public QueryChargePixSendloadOutputSplitInfo? OriginatorSplit { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public QueryChargePixSendloadOutputSplit[]? Secondaries { get; }
    public DateTime LastModifiedAt { get; }
    public INotification[] Notifications { get; }

    private QueryChargePixSendloadOutput(IdValueObject chargeId, DateTime createdAt, TypeChargePixValueObject type, ChargePixStatusValueObject status,
        TxidValueObject txid, PixAmountValueObject amount, PixDescriptionValueObject description, MetadataValueObject[] metadata,
        QueryChargePixSendloadOutputPaid[] payments, UrlValueObject[] notificationUrls,
        IdValueObject issuedBy, SplitValueObject? originatorSplit, QueryChargePixSendloadOutputSplit[]? secondaries, DateTime lastModifiedAt,
        INotification[] notifications)
    {
        ChargeId = chargeId;
        CreatedAt = createdAt;
        Type = type.GetTypeChargePix().ToString();
        Status = status.GetPixStatus().ToString();
        Txid = txid;
        Amount = amount;
        Description = description;
        Metadata = metadata.Select(p => new QueryChargePixSendloadOutputMetadata(
            key: p.GetMetadataKey(),
            metadata: p.GetMetadata())).ToArray();
        Payments = payments;
        NotificationUrls = notificationUrls.Select(p => p.GetUrl()).ToArray();
        IssuedBy = issuedBy;
        OriginatorSplit = originatorSplit is null ? null
            : new QueryChargePixSendloadOutputSplitInfo(
                typeSplit: originatorSplit.Value.GetTypeSplit().ToString(),
                amountSplit: originatorSplit.Value.GetAmountSplit());
        Secondaries = secondaries is null ? null
            : secondaries.Select(p => QueryChargePixSendloadOutputSplit.Factory(
                receiverAccountId: p.ReceiverAccountId,
                description: p.Description,
                split:  SplitValueObject.Factory(
                    typeSplit: p.Split.TypeSplit,
                    amountSplit: p.Split.AmountSplit))).ToArray();
        LastModifiedAt = lastModifiedAt;
        Notifications = notifications;
    }

    public static QueryChargePixSendloadOutput Factory(IdValueObject chargeId, DateTime createdAt, TypeChargePixValueObject type, ChargePixStatusValueObject status,
        TxidValueObject txid, PixAmountValueObject amount, PixDescriptionValueObject description, MetadataValueObject[] metadata,
        QueryChargePixSendloadOutputPaid[] payments, UrlValueObject[] notificationUrls,
        IdValueObject issuedBy, SplitValueObject? originatorSplit, QueryChargePixSendloadOutputSplit[]? secondaries, DateTime lastModifiedAt,
        INotification[] notifications)
        => new(chargeId, createdAt, type, status, txid, amount, description, metadata, payments, notificationUrls, issuedBy, originatorSplit, secondaries,
            lastModifiedAt, notifications);
}

public readonly struct QueryChargePixSendloadOutputSplitInfo
{
    public QueryChargePixSendloadOutputSplitInfo(string typeSplit, decimal amountSplit)
    {
        TypeSplit = typeSplit;
        AmountSplit = amountSplit;
    }

    public string TypeSplit { get; }
    public decimal AmountSplit { get; }
}

public readonly struct QueryChargePixSendloadOutputMetadata
{
    public QueryChargePixSendloadOutputMetadata(string key, string metadata)
    {
        Key = key;
        Metadata = metadata;
    }

    public string Key { get; }
    public string Metadata { get; }
}

public readonly struct QueryChargePixSendloadOutputPaid
{
    public Guid PaidId { get; }
    public DateTime PaidAt { get; }
    public decimal Amount { get; }
    public string EndToEndId { get; }
    public string Name { get; }
    public string Document { get; }
    public QueryChargePixSendloadOutputPaidRefund[] Refunds { get; }

    private QueryChargePixSendloadOutputPaid(IdValueObject paidId, DateTime paidAt, PixAmountValueObject amount, EndToEndIdValueObject endToEndId,
        DebtorNameValueObject name, DocumentValueObject document, QueryChargePixSendloadOutputPaidRefund[] refunds)
    {
        PaidId = paidId;
        PaidAt = paidAt;
        Amount = amount;
        EndToEndId = endToEndId;
        Name = name;
        Document = document.GetDocument();
        Refunds = refunds;
    }

    public static QueryChargePixSendloadOutputPaid Factory(IdValueObject paidId, DateTime paidAt, PixAmountValueObject amount, EndToEndIdValueObject endToEndId,
        DebtorNameValueObject name, DocumentValueObject document, QueryChargePixSendloadOutputPaidRefund[] refunds)
        => new(paidId, paidAt, amount, endToEndId, name, document, refunds);
}

public readonly struct QueryChargePixSendloadOutputPaidRefund
{
    public Guid RefundId { get; }
    public DateTime RefundedAt { get; }
    public string Type { get; }
    public string RtrId { get; }
    public decimal Amount { get; }
    public string? Description { get; }

    private QueryChargePixSendloadOutputPaidRefund(IdValueObject refundId, DateTime refundedAt, TypeChargeRefundValueObject type,
        EndToEndIdValueObject rtrId, PixAmountValueObject amount, PixDescriptionValueObject description)
    {
        RefundId = refundId;
        RefundedAt = refundedAt;
        Type = type.GetTypeChargeRefund().ToString();
        RtrId = rtrId;
        Amount = amount;
        Description = description;
    }

    public static QueryChargePixSendloadOutputPaidRefund Factory(IdValueObject refundId, DateTime refundedAt, TypeChargeRefundValueObject type,
        EndToEndIdValueObject rtrId, PixAmountValueObject amount, PixDescriptionValueObject description)
        => new(refundId, refundedAt, type, rtrId, amount, description);
}

public readonly struct QueryChargePixSendloadOutputSplit
{
    public Guid ReceiverAccountId { get; }
    public string Description { get; }
    public QueryChargePixSendloadOutputSplitInfo Split { get; }

    private QueryChargePixSendloadOutputSplit(IdValueObject receiverAccountId, SplitDescriptionValueObject description, SplitValueObject split)
    {
        ReceiverAccountId = receiverAccountId;
        Description = description;
        Split = new QueryChargePixSendloadOutputSplitInfo(
            typeSplit: split.GetTypeSplit().ToString(),
            amountSplit: split.GetAmountSplit());
    }

    public static QueryChargePixSendloadOutputSplit Factory(IdValueObject receiverAccountId, SplitDescriptionValueObject description, SplitValueObject split)
        => new(receiverAccountId, description, split);
}
