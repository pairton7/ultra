namespace UltraBank.WebApi.Webhooks.DellBankContext.Inputs;

public readonly struct AccountUpdatedEventDellBankPayloadInput
{
    public AccountUpdatedEventDellBankPayloadInput(string document, string? bankAccountNumber)
    {
        this.document = document;
        this.bankAccountNumber = bankAccountNumber;
    }

    public string document { get; init; }
    public string? bankAccountNumber { get; init; }
}
