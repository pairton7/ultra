namespace UltraBank.WebApi.Webhooks.DellBankContext.Inputs;

public readonly struct PixPaidEventDellBankPayloadInput
{
    public PixPaidEventDellBankPayloadInput(string eventType, string endToEndId, decimal amount, string correlationId, PixPaidEventDellBankPayloadInputProof proof)
    {
        this.eventType = eventType;
        this.endToEndId = endToEndId;
        this.amount = amount;
        this.correlationId = correlationId;
        this.proof = proof;
    }

    public string eventType { get; init; }
    public string endToEndId { get; init; }
    public decimal amount { get; init; }
    public string correlationId { get; init; }
    public PixPaidEventDellBankPayloadInputProof proof { get; init; }
}

public readonly struct PixPaidEventDellBankPayloadInputProof
{
    public PixPaidEventDellBankPayloadInputProof(string eventType, string endToEndId, string idempotencyKey, string status, decimal amount, PixPaidEventDellBankPayloadInputProofPayer payer, PixPaidEventDellBankPayloadInputProofPayer payee, PixPaidEventDellBankPayloadInputProofPayer beneficiary)
    {
        this.eventType = eventType;
        this.endToEndId = endToEndId;
        this.idempotencyKey = idempotencyKey;
        this.status = status;
        this.amount = amount;
        this.payer = payer;
        this.payee = payee;
        this.beneficiary = beneficiary;
    }

    public string eventType { get; init; }
    public string endToEndId { get; init; }
    public string idempotencyKey { get; init; }
    public string status { get; init; }
    public decimal amount { get; init; }
    public PixPaidEventDellBankPayloadInputProofPayer payer { get; init; }
    public PixPaidEventDellBankPayloadInputProofPayer payee { get; init; }
    public PixPaidEventDellBankPayloadInputProofPayer beneficiary { get; init; }
}

public readonly struct PixPaidEventDellBankPayloadInputProofPayer
{
    public PixPaidEventDellBankPayloadInputProofPayer(string number, string branch, PixPaidEventDellBankPayloadInputProofPayerParticipant participant, PixPaidEventDellBankPayloadInputProofPayerHolder holder)
    {
        this.number = number;
        this.branch = branch;
        this.participant = participant;
        this.holder = holder;
    }

    public string number { get; init; }
    public string branch { get; init; }
    public PixPaidEventDellBankPayloadInputProofPayerParticipant participant { get; init; }
    public PixPaidEventDellBankPayloadInputProofPayerHolder holder { get; init; }
}

public readonly struct PixPaidEventDellBankPayloadInputProofPayerHolder
{
    public PixPaidEventDellBankPayloadInputProofPayerHolder(string? name, string document, string type)
    {
        this.name = name;
        this.document = document;
        this.type = type;
    }

    public string? name { get; init; }
    public string document { get; init; }
    public string type { get; init; }
}

public readonly struct PixPaidEventDellBankPayloadInputProofPayerParticipant
{
    public PixPaidEventDellBankPayloadInputProofPayerParticipant(string ispb, string name)
    {
        this.ispb = ispb;
        this.name = name;
    }

    public string ispb { get; init; }
    public string name { get; init; }
}