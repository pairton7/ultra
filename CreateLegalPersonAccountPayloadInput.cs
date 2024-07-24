namespace UltraBank.WebApi.Controllers.AccountContext.Payloads;

public readonly struct CreateLegalPersonAccountPayloadInput
{
    public CreateLegalPersonAccountPayloadInput(string comercialName, string socialReason, string cnpj, string email, CreateLegalPersonAccountPayloadInputPhone phone, 
        DateTime foundationDate, string primaryCnaeCode, CreateLegalPersonAccountPayloadInputDocument[] documents, string[] secondaryCnaeCodes, string constitution, 
        DateTime constitutionDate, CreateLegalPersonAccountPayloadInputFinance finance, bool hasPoliticallyExposedPartner, 
        CreateLegalPersonAccountPayloadInputPartner[] partners, CreateLegalPersonAccountPayloadInputAddress address, string[] notificationUrls)
    {
        ComercialName = comercialName;
        SocialReason = socialReason;
        Cnpj = cnpj;
        Email = email;
        Phone = phone;
        FoundationDate = foundationDate;
        PrimaryCnaeCode = primaryCnaeCode;
        Documents = documents;
        SecondaryCnaeCodes = secondaryCnaeCodes;
        Constitution = constitution;
        ConstitutionDate = constitutionDate;
        Finance = finance;
        HasPoliticallyExposedPartner = hasPoliticallyExposedPartner;
        Partners = partners;
        Address = address;
        NotificationUrls = notificationUrls;
    }

    public string ComercialName { get; init; }
    public string SocialReason { get; init; }
    public string Cnpj { get; init; }
    public string Email { get; init; }
    public CreateLegalPersonAccountPayloadInputPhone Phone { get; init;  }
    public DateTime FoundationDate { get; init; }
    public string PrimaryCnaeCode { get; init; }
    public CreateLegalPersonAccountPayloadInputDocument[] Documents { get; init; }
    public string[] SecondaryCnaeCodes { get; init; }
    public string Constitution { get; init; }
    public DateTime ConstitutionDate { get; init; }
    public CreateLegalPersonAccountPayloadInputFinance Finance { get; init; }
    public bool HasPoliticallyExposedPartner { get; init; }
    public CreateLegalPersonAccountPayloadInputPartner[] Partners { get; init; }
    public CreateLegalPersonAccountPayloadInputAddress Address { get; init; }
    public string[] NotificationUrls { get; init; }
}

public readonly struct CreateLegalPersonAccountPayloadInputAddress
{
    public CreateLegalPersonAccountPayloadInputAddress(string zipCode, string streetName, string number, string? complement, string district, string city, string state, string country)
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

public readonly struct CreateLegalPersonAccountPayloadInputPartner
{
    public CreateLegalPersonAccountPayloadInputPartner(string legalName, string document, CreateLegalPersonAccountPayloadInputDocument[]? documents, 
        string email, CreateLegalPersonAccountPayloadInputPhone phone, decimal monthlyInvoicing, string gender, string maritalStatus, string educationLevel,
        DateTime birthDate, CreateLegalPersonAccountPayloadInputAddress address, bool isPoliticallyExposedPerson)
    {
        LegalName = legalName;
        Document = document;
        Documents = documents;
        Email = email;
        Phone = phone;
        MonthlyInvoicing = monthlyInvoicing;
        Gender = gender;
        MaritalStatus = maritalStatus;
        EducationLevel = educationLevel;
        BirthDate = birthDate;
        Address = address;
        IsPoliticallyExposedPerson = isPoliticallyExposedPerson;
    }

    public string LegalName { get; init; }
    public string Document { get; init; }
    public CreateLegalPersonAccountPayloadInputDocument[]? Documents { get; init; }
    public string Email { get; init; }
    public CreateLegalPersonAccountPayloadInputPhone Phone { get; init; }
    public decimal MonthlyInvoicing { get; init; }
    public string Gender { get; init; }
    public string MaritalStatus { get; init; }
    public string EducationLevel { get; init; }
    public DateTime BirthDate { get; init; }
    public CreateLegalPersonAccountPayloadInputAddress Address { get; init; }
    public bool IsPoliticallyExposedPerson { get; init; }
}

public readonly struct CreateLegalPersonAccountPayloadInputFinance
{
    public CreateLegalPersonAccountPayloadInputFinance(decimal patrimony, decimal shareCapital, decimal monthlyInvoicing)
    {
        Patrimony = patrimony;
        ShareCapital = shareCapital;
        MonthlyInvoicing = monthlyInvoicing;
    }

    public decimal Patrimony { get; init; }
    public decimal ShareCapital { get; init; }
    public decimal MonthlyInvoicing { get; init; }
}

public readonly struct CreateLegalPersonAccountPayloadInputPhone
{
    public CreateLegalPersonAccountPayloadInputPhone(string ddi, string ddd, string number)
    {
        Ddi = ddi;
        Ddd = ddd;
        Number = number;
    }

    public string Ddi { get; init; }
    public string Ddd { get; init; }
    public string Number { get; init; }
}


public readonly struct CreateLegalPersonAccountPayloadInputDocument
{
    public CreateLegalPersonAccountPayloadInputDocument(string documentType, string documentNumber)
    {
        DocumentType = documentType;
        DocumentNumber = documentNumber;
    }

    public string DocumentType { get; init; }
    public string DocumentNumber { get; init; }
}