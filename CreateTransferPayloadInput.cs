namespace UltraBank.WebApi.Controllers.TransferContext.Payloads;

public readonly struct CreateTransferPayloadInput
{
    public CreateTransferPayloadInput(decimal amount, Guid beneficiaryAccountId, string description, string externalTransferId, CreateTransferPayloadInputMetadata[]? metadata)
    {
        Amount = amount;
        BeneficiaryAccountId = beneficiaryAccountId;
        Description = description;
        ExternalTransferId = externalTransferId;
        Metadata = metadata;
    }

    public decimal Amount { get; init; }
    public Guid BeneficiaryAccountId { get; init; }
    public string Description { get; init; }
    public string ExternalTransferId { get; init; }
    public CreateTransferPayloadInputMetadata[]? Metadata { get; init; }
}

public readonly struct CreateTransferPayloadInputMetadata
{
    public CreateTransferPayloadInputMetadata(string key, string metadata)
    {
        Key = key;
        Metadata = metadata;
    }

    public string Key { get; init; }
    public string Metadata { get; init; }
}