using UltraBank.Domain.ValueObjects;
using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.ChargesContext.Sendloads;

public readonly struct RefundChargePixSendloadOutput
{
    public Guid ChargeId { get; }
    public DateTime CreatedAt { get; }
    public string Type { get; }
    public string Status { get; }
    public decimal Amount { get; }
    public string? Description { get; }
    public string Txid { get; }
    public DateTime LastModifiedAt { get; }
    public RefundChargePixSendloadOutputPaymentReceive[] PaymentReceives { get; }
    public INotification[] Notifications { get; }

    private RefundChargePixSendloadOutput(IdValueObject chargeId, DateTime createdAt, TypeChargePixValueObject type, ChargePixStatusValueObject status,
        PixAmountValueObject amount, PixDescriptionValueObject description, TxidValueObject txid, DateTime lastModifiedAt,
        RefundChargePixSendloadOutputPaymentReceive[] paymentReceives, INotification[] notifications)
    {
        ChargeId = chargeId;
        CreatedAt = createdAt;
        Type = type.GetTypeChargePix().ToString();
        Status = status.GetPixStatus().ToString();
        Amount = amount;
        Description = description;
        Txid = txid;
        LastModifiedAt = lastModifiedAt;
        PaymentReceives = paymentReceives;
        Notifications = notifications;
    }

    public static RefundChargePixSendloadOutput Factory(IdValueObject chargeId, DateTime createdAt, TypeChargePixValueObject type, ChargePixStatusValueObject status,
        PixAmountValueObject amount, PixDescriptionValueObject description, TxidValueObject txid, DateTime lastModifiedAt,
        RefundChargePixSendloadOutputPaymentReceive[] paymentReceives, INotification[] notifications)
        => new(chargeId, createdAt, type, status, amount, description, txid, lastModifiedAt, paymentReceives, notifications);
}

public readonly struct RefundChargePixSendloadOutputPaymentReceive
{
    public Guid PaidId { get; }
    public DateTime PaidAt { get; }
    public decimal Amount { get; }
    public string EndToEndId { get; }
    public string Name { get; }
    public string Document { get; }
    public RefundChargePixSendloadOutputRefund[] Refunds { get; }

    private RefundChargePixSendloadOutputPaymentReceive(IdValueObject paidId, DateTime paidAt, PixAmountValueObject amount,
        EndToEndIdValueObject endToEndId, string name, string document, RefundChargePixSendloadOutputRefund[] refunds)
    {
        PaidId = paidId;
        PaidAt = paidAt;
        Amount = amount;
        EndToEndId = endToEndId;
        Name = name;
        Document = document;
        Refunds = refunds;
    }

    public static RefundChargePixSendloadOutputPaymentReceive Factory(IdValueObject paidId, DateTime paidAt, PixAmountValueObject amount,
        EndToEndIdValueObject endToEndId, string name, string document, RefundChargePixSendloadOutputRefund[] refunds)
        => new(paidId, paidAt, amount, endToEndId, name, document, refunds);
}

public readonly struct RefundChargePixSendloadOutputRefund
{
    public Guid RefundId { get; }
    public string Type { get; }
    public string RtrId { get; }
    public decimal Amount { get; }
    public string? Description { get; }

    private RefundChargePixSendloadOutputRefund(IdValueObject refundId, TypeChargeRefundValueObject type, EndToEndIdValueObject rtrId, PixAmountValueObject amount, PixDescriptionValueObject description)
    {
        RefundId = refundId;
        Type = type;
        RtrId = rtrId;
        Amount = amount;
        Description = description;
    }

    public static RefundChargePixSendloadOutputRefund Factory(IdValueObject refundId, TypeChargeRefundValueObject type, EndToEndIdValueObject rtrId, PixAmountValueObject amount,
        PixDescriptionValueObject description)
        => new(refundId, type, rtrId, amount, description);
}

