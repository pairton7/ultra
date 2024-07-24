namespace UltraBank.WebApi.Controllers.ChargesContext.Payloads;

public readonly struct CreateDynamicSplitChargePixPayloadInput
{
    public CreateDynamicSplitChargePixPayloadInput(decimal amount, int expiresIn, CreateDynamicSplitChargePixPayloadInputDebtor? debtor, string? description, 
        CreateDynamicSplitChargePixPayloadInputMetadata[]? metadata, string[] notificationUrls, CreateDynamicSplitChargePixPayloadInputOriginatorSplit originatorSplit, CreateDynamicSplitChargePixPayloadInputSplit[] secondaries)
    {
        Amount = amount;
        ExpiresIn = expiresIn;
        Debtor = debtor;
        Description = description;
        Metadata = metadata;
        NotificationUrls = notificationUrls;
        OriginatorSplit = originatorSplit;
        Secondaries = secondaries;
    }

    public decimal Amount { get; init; }
    public int ExpiresIn { get; init; }
    public CreateDynamicSplitChargePixPayloadInputDebtor? Debtor { get; init; }
    public string? Description { get; init; }
    public CreateDynamicSplitChargePixPayloadInputMetadata[]? Metadata { get; init; }
    public string[] NotificationUrls { get; init; }
    public CreateDynamicSplitChargePixPayloadInputOriginatorSplit OriginatorSplit { get; init; }
    public CreateDynamicSplitChargePixPayloadInputSplit[] Secondaries { get; init; }
}

public readonly struct CreateDynamicSplitChargePixPayloadInputDebtor
{
    public CreateDynamicSplitChargePixPayloadInputDebtor(string documentType, string document, string name)
    {
        DocumentType = documentType;
        Document = document;
        Name = name;
    }

    public string DocumentType { get; init; }
    public string Document { get; init; }
    public string Name { get; init; }
}

public readonly struct CreateDynamicSplitChargePixPayloadInputMetadata
{
    public CreateDynamicSplitChargePixPayloadInputMetadata(string key, string metadata)
    {
        Key = key;
        Metadata = metadata;
    }

    public string Key { get; init; }
    public string Metadata { get; init; }
}

public readonly struct CreateDynamicSplitChargePixPayloadInputOriginatorSplit
{
    public CreateDynamicSplitChargePixPayloadInputOriginatorSplit(string typeSplit, decimal amountSplit)
    {
        TypeSplit = typeSplit;
        AmountSplit = amountSplit;
    }

    public string TypeSplit { get; init; }
    public decimal AmountSplit { get; init; }
}

public readonly struct CreateDynamicSplitChargePixPayloadInputSplit
{
    public CreateDynamicSplitChargePixPayloadInputSplit(Guid receiverAccountId, string typeSplit, decimal amountSplit, string description, string extractGroup)
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