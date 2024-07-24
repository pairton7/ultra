namespace UltraBank.WebApi.Controllers.TransferContext.Payloads;

public readonly struct CreateOutTransferPayloadInput
{
    public CreateOutTransferPayloadInput(decimal amount, string externalTransferId, string? description, CreateOutTransferPayloadInputMetadata[] metadata, 
        CreateOutTransferPayloadInputBeneficiary beneficiary, string[] notificationUrls)
    {
        Amount = amount;
        ExternalTransferId = externalTransferId;
        Description = description;
        Metadata = metadata;
        Beneficiary = beneficiary;
        NotificationUrls = notificationUrls;
    }

    public decimal Amount { get; init; }
    public string ExternalTransferId { get; init; }
    public string? Description { get; init; }
    public CreateOutTransferPayloadInputMetadata[] Metadata { get; init; }
    public CreateOutTransferPayloadInputBeneficiary Beneficiary { get; init; }
    public string[] NotificationUrls { get; init; }
}

public readonly struct CreateOutTransferPayloadInputMetadata
{
    public CreateOutTransferPayloadInputMetadata(string key, string metadata)
    {
        Key = key;
        Metadata = metadata;
    }

    public string Key { get; init; }
    public string Metadata { get; init; }
}

public readonly struct CreateOutTransferPayloadInputBeneficiary
{
    public CreateOutTransferPayloadInputBeneficiary(string type, string number, string branch, string holderName, string holderDocument, string ispbCode)
    {
        Type = type;
        Number = number;
        Branch = branch;
        HolderName = holderName;
        HolderDocument = holderDocument;
        IspbCode = ispbCode;
    }

    public string Type { get; init; }
    public string Number { get; init; }
    public string Branch { get; init; }
    public string HolderName { get; init; }
    public string HolderDocument { get; init; }
    public string IspbCode { get; init; }
}
