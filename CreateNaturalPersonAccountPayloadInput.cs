using UltraBank.Domain.ValueObjects;

namespace UltraBank.WebApi.Controllers.AccountContext.Payloads;

public readonly struct CreateNaturalPersonAccountPayloadInput
{
    public CreateNaturalPersonAccountPayloadInput(string name, string surname, string cpf, string email, CreateNaturalPersonAccountPayloadInputPhone phone, 
        CreateNaturalPersonAccountPayloadInputDocument[] documents, DateTime birthDate, decimal monthlyInvoicing, string maritalStatus, string educationLevel, 
        string gender, CreateNaturalPersonAccountPayloadInputAddress address, bool isPoliticallyExposedPerson, string[] notificationUrls)
    {
        Name = name;
        Surname = surname;
        Cpf = cpf;
        Email = email;
        Phone = phone;
        Documents = documents;
        BirthDate = birthDate;
        MonthlyInvoicing = monthlyInvoicing;
        MaritalStatus = maritalStatus;
        EducationLevel = educationLevel;
        Gender = gender;
        Address = address;
        IsPoliticallyExposedPerson = isPoliticallyExposedPerson;
        NotificationUrls = notificationUrls;
    }

    public string Name { get; init; }
    public string Surname { get; init; }
    public string Cpf { get; init; }
    public string Email { get; init; }
    public CreateNaturalPersonAccountPayloadInputPhone Phone { get; init; }
    public CreateNaturalPersonAccountPayloadInputDocument[] Documents { get; init; }
    public DateTime BirthDate { get; init; }
    public decimal MonthlyInvoicing { get; init; }
    public string MaritalStatus { get; init; }
    public string EducationLevel { get; init; }
    public string Gender { get; init; }
    public CreateNaturalPersonAccountPayloadInputAddress Address { get; init; }
    public bool IsPoliticallyExposedPerson { get; init; }
    public string[] NotificationUrls { get; init; }
}

public readonly struct CreateNaturalPersonAccountPayloadInputDocument
{
    public CreateNaturalPersonAccountPayloadInputDocument(string documentType, string documentNumber)
    {
        DocumentType = documentType;
        DocumentNumber = documentNumber;
    }

    public string DocumentType { get; init; }
    public string DocumentNumber { get; init; }
}

public readonly struct CreateNaturalPersonAccountPayloadInputPhone
{
    public CreateNaturalPersonAccountPayloadInputPhone(string ddi, string ddd, string number)
    {
        Ddi = ddi;
        Ddd = ddd;
        Number = number;
    }

    public string Ddi { get; init; }
    public string Ddd { get; init; }
    public string Number { get; init; }
}


public readonly struct CreateNaturalPersonAccountPayloadInputAddress
{
    public CreateNaturalPersonAccountPayloadInputAddress(string zipCode, string streetName, string number, string? complement, string district, string city, string state, string country)
    {
        ZipCode = zipCode;
        StreetName = streetName;
        Number = number;
        Complement = complement;
        District = district;
        City = city;
        State = state;
        Country = country;
    }

    public string ZipCode { get; init; }
    public string StreetName { get; init; }
    public string Number { get; init; }
    public string? Complement { get; init; }
    public string District { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string Country { get; init; }
}
