namespace UltraBank.WebApi.Controllers.ChargesContext.Payloads;

public readonly struct CreateDynamicChargePixPayloadInput
{
    public CreateDynamicChargePixPayloadInput(decimal amount, int expiresIn, CreateDynamicChargePixPayloadInputDebtor? debtor, string? description, 
        CreateDynamicChargePixPayloadInputMetadata[]? metadata, string[] notificationUrls)
    {
        Amount = amount;
        ExpiresIn = expiresIn;
        Debtor = debtor;
        Description = description;
        Metadata = metadata;
        NotificationUrls = notificationUrls;
    }

    public decimal Amount { get; init; }
    public int ExpiresIn { get; init; }
    public CreateDynamicChargePixPayloadInputDebtor? Debtor { get; init; }
    public string? Description { get; init; }
    public CreateDynamicChargePixPayloadInputMetadata[]? Metadata { get; init; }
    public string[] NotificationUrls { get; init; }
}

public readonly struct CreateDynamicChargePixPayloadInputDebtor
{
    public CreateDynamicChargePixPayloadInputDebtor(string documentType, string document, string name)
    {
        DocumentType = documentType;
        Document = document;
        Name = name;
    }

    public string DocumentType { get; init; }
    public string Document { get; init; }
    public string Name { get; init; }
}

public readonly struct CreateDynamicChargePixPayloadInputMetadata
{
    public CreateDynamicChargePixPayloadInputMetadata(string key, string metadata)
    {
        Key = key;
        Metadata = metadata;
    }

    public string Key { get; init; }
    public string Metadata { get; init; }
}