namespace UltraBank.WebApi.Webhooks.DellBankContext.Inputs;

public readonly struct OutTransferEventPayloadInput
{
    public OutTransferEventPayloadInput(string eventType, string endToEndId, string idempotencyKey, string status, 
        decimal amount, OutTransferEventPayloadInputParticipant payer, OutTransferEventPayloadInputParticipant beneficiary, OutTransferEventPayloadInputParticipant payee,
        OutTransferEventPayloadInputError? error = null)
    {
        EventType = eventType;
        EndToEndId = endToEndId;
        IdempotencyKey = idempotencyKey;
        Status = status;
        Amount = amount;
        Payer = payer;
        Beneficiary = beneficiary;
        Payee = payee;
        Error = error;
    }

    public string EventType { get; init; }
    public string EndToEndId { get; init; }
    public string IdempotencyKey { get; init; }
    public string Status { get; init; }
    public decimal Amount { get; init; }
    public OutTransferEventPayloadInputParticipant Payer { get; init; }
    public OutTransferEventPayloadInputParticipant Beneficiary { get; init; }
    public OutTransferEventPayloadInputParticipant Payee { get; init; }
    public OutTransferEventPayloadInputError? Error { get; init; }
}

public readonly struct OutTransferEventPayloadInputError
{
    public OutTransferEventPayloadInputError(string code, string description)
    {
        Code = code;
        Description = description;
    }

    public string Code { get; init; }
    public string Description { get; init; }
}

public readonly struct OutTransferEventPayloadInputParticipant
{
    public OutTransferEventPayloadInputParticipant(string number, string branch, string type, 
        OutTransferEventPayloadInputParticipantInfo participant, OutTransferEventPayloadInputParticipantHolder holder)
    {
        Number = number;
        Branch = branch;
        Type = type;
        Participant = participant;
        Holder = holder;
    }

    public string Number { get; init; }
    public string Branch { get; init; }
    public string Type { get; init; }
    public OutTransferEventPayloadInputParticipantInfo Participant { get; init; }
    public OutTransferEventPayloadInputParticipantHolder Holder { get; init; }
}

public readonly struct OutTransferEventPayloadInputParticipantInfo
{
    public OutTransferEventPayloadInputParticipantInfo(string name, string ispb)
    {
        Name = name;
        Ispb = ispb;
    }

    public string Name { get; init; } 
    public string Ispb { get; init; } 
}

public readonly struct OutTransferEventPayloadInputParticipantHolder
{
    public OutTransferEventPayloadInputParticipantHolder(string name, string document, string type)
    {
        Name = name;
        Document = document;
        Type = type;
    }

    public string Name { get; init; } 
    public string Document { get; init; } 
    public string Type { get; init; } 
}
