using System.Text.Json.Serialization;
using UltraBank.Domain.ValueObjects;

namespace UltraBank.WebApi.Controllers.TransferContext.Sendloads;

public readonly struct QueryBankTransferSendloadOutput
{
    public Guid TransferId { get; }
    public DateTime CreatedAt { get; }
    public string Type { get; }
    public decimal Amount { get; }
    public QueryBankTransferSendloadOutputMetadata[] Metadata { get; }
    public QueryBankTransferSendloadOutputParticipant Issuer { get; }
    public QueryBankTransferSendloadOutputParticipant Beneficiary { get; }

    private QueryBankTransferSendloadOutput(
        IdValueObject transferId, DateTime createdAt, TransferTypeValueObject type, TransferAmountValueObject amount,
        MetadataValueObject[] metadata, QueryBankTransferSendloadOutputParticipant issuer, QueryBankTransferSendloadOutputParticipant beneficiary)
    {
        TransferId = transferId;
        CreatedAt = createdAt;
        Type = type.GetTypeTransfer().ToString();
        Amount = amount;
        Metadata = metadata.Select(p => QueryBankTransferSendloadOutputMetadata.Factory(
            key: p.GetMetadataKey(),
            metadata: p.GetMetadata())).ToArray();
        Issuer = issuer;
        Beneficiary = beneficiary;
    }

    public static QueryBankTransferSendloadOutput Factory(
        IdValueObject transferId, DateTime createdAt, TransferTypeValueObject type, TransferAmountValueObject amount,
        MetadataValueObject[] metadata, QueryBankTransferSendloadOutputParticipant issuer, QueryBankTransferSendloadOutputParticipant beneficiary)
        => new(transferId, createdAt, type, amount, metadata, issuer, beneficiary);
}

public readonly struct QueryBankTransferSendloadOutputMetadata
{
    public string Key { get; }
    public string Metadata { get; }

    private QueryBankTransferSendloadOutputMetadata(string key, string metadata)
    {
        Key = key;
        Metadata = metadata;
    }

    public static QueryBankTransferSendloadOutputMetadata Factory(string key, string metadata)
        => new(key, metadata);
}

public readonly struct QueryBankTransferSendloadOutputParticipant
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? AccountId { get; }
    public string Document { get; }
    public string Name { get; }
    public string ParticipantType { get; }

    private QueryBankTransferSendloadOutputParticipant(
        IdValueObject accountId,
        string document,
        string name,
        TransferParticipantTypeValueObject participantType)
    {
        AccountId = accountId == Guid.Empty ? null : accountId;
        Document = document;
        Name = name;
        ParticipantType = participantType.GetTypeTransferParticipant().ToString();
    }

    public static QueryBankTransferSendloadOutputParticipant Factory(
        IdValueObject accountId,
        string document,
        string name,
        TransferParticipantTypeValueObject participantType)
        => new(accountId, document, name, participantType);
}
