using System.Text.Json.Serialization;
using UltraBank.Domain.BoundedContexts.AccountContext.ENUMs;

namespace UltraBank.WebApi.Controllers.AccountContext.Sendloads;

public readonly struct QueryBankAccountSendloadOutput
{
    public Guid AccountId { get; }
    public string Type { get; }
    public string Status { get; }
    public DateTime CreatedAt { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public QueryBankAccountSendloadOutputPhysical? Natural { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public QueryBankAccountSendloadOutputLegal? Legal { get; }

    public DateTime LastModifiedAt { get; }

    private QueryBankAccountSendloadOutput(Guid accountId, string type, string status, DateTime createdAt, QueryBankAccountSendloadOutputPhysical? natural,
        QueryBankAccountSendloadOutputLegal? legal, DateTime lastModifiedAt)
    {
        AccountId = accountId;
        Type = type;
        Status = status;
        CreatedAt = createdAt;
        Natural = natural;
        Legal = legal;
        LastModifiedAt = lastModifiedAt;
    }

    public static QueryBankAccountSendloadOutput Factory(Guid accountId, string type, string status, DateTime createdAt, QueryBankAccountSendloadOutputPhysical? natural,
        QueryBankAccountSendloadOutputLegal? legal, DateTime lastModifiedAt)
        => new(accountId, type, status, createdAt, natural, legal, lastModifiedAt);
}

public readonly struct QueryBankAccountSendloadOutputContact
{
    public QueryBankAccountSendloadOutputPhone Phone { get; }
    public string Email { get; }

    private QueryBankAccountSendloadOutputContact(QueryBankAccountSendloadOutputPhone phone, string email)
    {
        Phone = phone;
        Email = email;
    }

    public static QueryBankAccountSendloadOutputContact Factory(QueryBankAccountSendloadOutputPhone phone, string email)
        => new(phone, email);
}

public readonly struct QueryBankAccountSendloadOutputPhone
{
    public string Ddi { get; }
    public string Ddd { get; }
    public string Number { get; }

    private QueryBankAccountSendloadOutputPhone(string ddi, string ddd, string number)
    {
        Ddi = ddi;
        Ddd = ddd;
        Number = number;
    }

    public static QueryBankAccountSendloadOutputPhone Factory(string ddi, string ddd, string number)
        => new(ddi, ddd, number);
}

public readonly struct QueryBankAccountSendloadOutputAddress
{
    public string ZipCode { get; }
    public string StreetName { get; }
    public string Number { get; }
    public string? Complement { get; }
    public string District { get; }
    public string City { get; }
    public string State { get; }
    public string Country { get; }

    private QueryBankAccountSendloadOutputAddress(string zipCode, string streetName, string number, string? complement, string district, string city, string state, string country)
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

    public static QueryBankAccountSendloadOutputAddress Factory(string zipCode, string streetName, string number, string? complement, string district, string city, string state,
        string country)
        => new(zipCode, streetName, number, complement, district, city, state, country);
}

public readonly struct QueryBankAccountSendloadOutputDocument
{
    public string Type { get; }
    public string Number { get; }

    private QueryBankAccountSendloadOutputDocument(string type, string number)
    {
        Type = type;
        Number = number;
    }

    public static QueryBankAccountSendloadOutputDocument Factory(string type, string number)
        => new(type, number);
}

public readonly struct QueryBankAccountSendloadOutputPersonalInfo
{
    public string MaritalStatus { get; }
    public string Gender { get; }
    public string EducationLevel { get; }

    private QueryBankAccountSendloadOutputPersonalInfo(string maritalStatus, string gender, string educationLevel)
    {
        MaritalStatus = maritalStatus;
        Gender = gender;
        EducationLevel = educationLevel;
    }

    public static QueryBankAccountSendloadOutputPersonalInfo Factory(string maritalStatus, string gender, string educationLevel)
        => new(maritalStatus, gender, educationLevel);
}

public sealed record QueryBankAccountSendloadOutputPhysical
{
    public string FirstName { get; }
    public string Surname { get; }
    public QueryBankAccountSendloadOutputDocument Document { get; }
    public DateTime BirthDate { get; }
    public decimal MonthlyInvoicing { get; }
    public QueryBankAccountSendloadOutputPersonalInfo PersonalInfo { get; }
    public QueryBankAccountSendloadOutputContact Contact { get; }
    public QueryBankAccountSendloadOutputAddress Address { get; }
    public bool IsPoliticallyExposedPerson { get; }

    private QueryBankAccountSendloadOutputPhysical(string firstName, string surname, QueryBankAccountSendloadOutputDocument document, DateTime birthDate,
        decimal monthlyInvoicing,
        QueryBankAccountSendloadOutputPersonalInfo personalInfo, QueryBankAccountSendloadOutputContact contact, QueryBankAccountSendloadOutputAddress address,
        bool isPoliticallyExposedPerson)
    {
        FirstName = firstName;
        Surname = surname;
        Document = document;
        BirthDate = birthDate;
        MonthlyInvoicing = monthlyInvoicing;
        PersonalInfo = personalInfo;
        Contact = contact;
        Address = address;
        IsPoliticallyExposedPerson = isPoliticallyExposedPerson;
    }

    public static QueryBankAccountSendloadOutputPhysical Factory(string firstName, string surname, QueryBankAccountSendloadOutputDocument document, DateTime birthDate,
        decimal monthlyInvoicing,
        QueryBankAccountSendloadOutputPersonalInfo personalInfo, QueryBankAccountSendloadOutputContact contact, QueryBankAccountSendloadOutputAddress address,
        bool isPoliticallyExposedPerson)
        => new(firstName, surname, document, birthDate, monthlyInvoicing, personalInfo, contact, address, isPoliticallyExposedPerson);
}

public readonly struct QueryBankAccountSendloadOutputLegalFinance
{
    public decimal MonthlyInvoicing { get; }
    public decimal ShareCapital { get; }
    public decimal Patrimony { get; }

    private QueryBankAccountSendloadOutputLegalFinance(decimal monthlyInvoicing, decimal shareCapital, decimal patrimony)
    {
        MonthlyInvoicing = monthlyInvoicing;
        ShareCapital = shareCapital;
        Patrimony = patrimony;
    }

    public static QueryBankAccountSendloadOutputLegalFinance Factory(decimal monthlyInvoicing, decimal shareCapital, decimal patrimony)
        => new(monthlyInvoicing, shareCapital, patrimony);
}

public sealed record QueryBankAccountSendloadOutputLegal
{
    public string ComercialName { get; }
    public string SocialReason { get; }
    public QueryBankAccountSendloadOutputDocument Document { get; }
    public DateTime FoundationDate { get; }
    public string ConstitutionType { get; }
    public string ConstitutionDate { get; }
    public QueryBankAccountSendloadOutputLegalFinance Finance { get; }
    public QueryBankAccountSendloadOutputContact Contact { get; }
    public QueryBankAccountSendloadOutputAddress Address { get; }
    public bool HasPoliticallyExposedPerson { get; }
    public QueryBankAccountSendloadOutputLegalPartner[] Partners { get; }

    private QueryBankAccountSendloadOutputLegal(string comercialName, string socialReason, QueryBankAccountSendloadOutputDocument document, DateTime foundationDate, 
        string constitutionType, string constitutionDate, QueryBankAccountSendloadOutputLegalFinance finance, QueryBankAccountSendloadOutputContact contact, QueryBankAccountSendloadOutputAddress address, 
        bool hasPoliticallyExposedPerson, QueryBankAccountSendloadOutputLegalPartner[] partners)
    {
        ComercialName = comercialName;
        SocialReason = socialReason;
        Document = document;
        FoundationDate = foundationDate;
        ConstitutionType = constitutionType;
        ConstitutionDate = constitutionDate;
        Finance = finance;
        Contact = contact;
        Address = address;
        HasPoliticallyExposedPerson = hasPoliticallyExposedPerson;
        Partners = partners;
    }

    public static QueryBankAccountSendloadOutputLegal Factory(string comercialName, string socialReason, QueryBankAccountSendloadOutputDocument document,
        DateTime foundationDate, string constitutionType, string constitutionDate, QueryBankAccountSendloadOutputLegalFinance finance, QueryBankAccountSendloadOutputContact contact,
        QueryBankAccountSendloadOutputAddress address, bool hasPoliticallyExposedPerson, QueryBankAccountSendloadOutputLegalPartner[] partners)
        => new(comercialName, socialReason, document, foundationDate, constitutionType, constitutionDate, finance, contact, address, hasPoliticallyExposedPerson, partners);
}

public readonly struct QueryBankAccountSendloadOutputLegalPartner
{
    public string LegalName { get; }
    public QueryBankAccountSendloadOutputDocument Document { get; }
    public DateTime BirthDate { get; }
    public QueryBankAccountSendloadOutputPersonalInfo PersonalInfo { get; }
    public QueryBankAccountSendloadOutputContact Contact { get; }
    public QueryBankAccountSendloadOutputAddress Address { get; }
    public bool IsPoliticallyExposedPerson { get; }

    private QueryBankAccountSendloadOutputLegalPartner(string legalName, QueryBankAccountSendloadOutputDocument document, DateTime birthDate, 
        QueryBankAccountSendloadOutputPersonalInfo personalInfo, QueryBankAccountSendloadOutputContact contact, QueryBankAccountSendloadOutputAddress address, 
        bool isPoliticallyExposedPerson)
    {
        LegalName = legalName;
        Document = document;
        BirthDate = birthDate;
        PersonalInfo = personalInfo;
        Contact = contact;
        Address = address;
        IsPoliticallyExposedPerson = isPoliticallyExposedPerson;
    }

    public static QueryBankAccountSendloadOutputLegalPartner Factory(string legalName, QueryBankAccountSendloadOutputDocument document, DateTime birthDate,
        QueryBankAccountSendloadOutputPersonalInfo personalInfo, QueryBankAccountSendloadOutputContact contact, QueryBankAccountSendloadOutputAddress address,
        bool isPoliticallyExposedPerson)
        => new(legalName, document, birthDate, personalInfo, contact, address, isPoliticallyExposedPerson);
}
