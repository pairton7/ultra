using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Mime;
using UltraBank.Application.UseCases.AccountContext.CreateLegalPersonAccount.Inputs;
using UltraBank.Application.UseCases.AccountContext.CreateLegalPersonAccount.Outputs;
using UltraBank.Application.UseCases.AccountContext.CreateNaturalPersonAccount.Inputs;
using UltraBank.Application.UseCases.AccountContext.CreateNaturalPersonAccount.Outputs;
using UltraBank.Application.UseCases.AccountContext.OAuthAccountAuthentication.Inputs;
using UltraBank.Application.UseCases.AccountContext.OAuthAccountAuthentication.Outputs;
using UltraBank.Application.UseCases.AccountContext.QueryBankAccount.Inputs;
using UltraBank.Application.UseCases.AccountContext.QueryBankAccountBalance.Inputs;
using UltraBank.Application.UseCases.AccountContext.QueryDataBankAccount.Inputs;
using UltraBank.Application.UseCases.AccountContext.QueryDataBankAccount.Outputs;
using UltraBank.Application.UseCases.Interfaces;
using UltraBank.AuditableInfoContext;
using UltraBank.Domain.BoundedContexts.AccountContext.DataTransferObject;
using UltraBank.Domain.BoundedContexts.AccountContext.ENUMs;
using UltraBank.Domain.BoundedContexts.AccountContext.Models;
using UltraBank.Domain.BoundedContexts.WhitelabelContext.DataTransferObject;
using UltraBank.Domain.Utils;
using UltraBank.Domain.ValueObjects;
using UltraBank.Domain.ValueObjects.ENUMs;
using UltraBank.ObservabilityContext.Metric.Interfaces;
using UltraBank.ObservabilityContext.Trace.Interfaces;
using UltraBank.WebApi.Controllers.AccountContext.Payloads;
using UltraBank.WebApi.Controllers.AccountContext.Sendloads;

namespace UltraBank.WebApi.Controllers.AccountContext;

[Route("api/v1/baas/account")]
[ApiController]
public sealed class AccountController : CustomizedControllerBase
{
    public AccountController(ITraceManager traceManager, IMetricManager metricManager) : base(traceManager, metricManager)
    {
    }

    [HttpGet]
    [Route("balance")]
    [Authorize(Roles = $"{nameof(Account)}")]
    public Task<IActionResult> HttpGetQueryBankAccountBalanceAsync(
        [FromServices] IUseCase<QueryBankAccountBalanceUseCaseInput, AccountBalanceModel> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(AccountController)}.{nameof(HttpPostOAuthAccountAuthenticationAsync)}",
            activityKind: ActivityKind.Internal,
            handler: async (auditableInfo, activity, cancellationToken) =>
            {
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var accountId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                if (!HasScopeOnAuthentication(scope, "balance.read"))
                    return StatusCode(
                        statusCode: StatusCodes.Status401Unauthorized,
                        value: ScopeInvalid);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: QueryBankAccountBalanceUseCaseInput.Factory(
                        whitelabelId: whitelabelId,
                        accountId: accountId),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                        statusCode: StatusCodes.Status200OK,
                        value: useCaseResult.Output);
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }

    [HttpGet]
    [Route("bank")]
    [Authorize(Roles = $"{nameof(Account)}")]
    public Task<IActionResult> HttpGetQueryDataBankAccountAsync(
        [FromServices] IUseCase<QueryDataBankAccountUseCaseInput, QueryDataBankAccountUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(AccountController)}.{nameof(HttpPostOAuthAccountAuthenticationAsync)}",
            activityKind: ActivityKind.Internal,
            handler: async (auditableInfo, activity, cancellationToken) =>
            {
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var accountId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                if (!HasScopeOnAuthentication(scope, "account.read"))
                    return StatusCode(
                        statusCode: StatusCodes.Status401Unauthorized,
                        value: ScopeInvalid);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: QueryDataBankAccountUseCaseInput.Factory(
                        accountId: accountId),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                        statusCode: StatusCodes.Status200OK,
                        value: QueryDataBankAccountSendloadOutput.Factory(
                            accountId: useCaseResult.Output.AccountId,
                            number: useCaseResult.Output.Number,
                            branch: useCaseResult.Output.Branch,
                            ispbCode: useCaseResult.Output.IspbCode,
                            notifications: useCaseResult.Notifications ?? []));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Account)}")]
    public Task<IActionResult> HttpGetQueryBankAccountAsync(
        [FromServices] IUseCase<QueryBankAccountUseCaseInput, Account> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(AccountController)}.{nameof(HttpPostOAuthAccountAuthenticationAsync)}",
            activityKind: ActivityKind.Internal,
            handler: async (auditableInfo, activity, cancellationToken) =>
            {
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var accountId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                if (!HasScopeOnAuthentication(scope, "account.read"))
                    return StatusCode(
                        statusCode: StatusCodes.Status401Unauthorized,
                        value: ScopeInvalid);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: QueryBankAccountUseCaseInput.Factory(
                        whitelabelId: whitelabelId,
                        queryAccountId: accountId),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status200OK,
                        value: QueryBankAccountSendloadOutput.Factory(
                            accountId: useCaseResult.Output!.Id,
                            type: useCaseResult.Output.TypeAccount.ToString(),
                            status: useCaseResult.Output.Status.ToString(),
                            createdAt: useCaseResult.Output.CreatedAt,
                            natural: useCaseResult.Output.TypeAccount == TypeAccount.LEGAL ? null
                                : QueryBankAccountSendloadOutputPhysical.Factory(
                                    firstName: useCaseResult.Output.FirstName!,
                                    surname: useCaseResult.Output.Surname!,
                                    document: QueryBankAccountSendloadOutputDocument.Factory(
                                        type: useCaseResult.Output.DocumentType,
                                        number: useCaseResult.Output.DocumentNumber),
                                    birthDate: useCaseResult.Output.FoundationDate.Date,
                                    monthlyInvoicing: useCaseResult.Output.MonthlyInvoicing,
                                    personalInfo: QueryBankAccountSendloadOutputPersonalInfo.Factory(
                                        maritalStatus: useCaseResult.Output.MaritalStatus!,
                                        gender: useCaseResult.Output.Gender!,
                                        educationLevel: useCaseResult.Output.EducationLevel!),
                                    contact: QueryBankAccountSendloadOutputContact.Factory(
                                        phone: QueryBankAccountSendloadOutputPhone.Factory(
                                            ddi: useCaseResult.Output.Ddi,
                                            ddd: useCaseResult.Output.Ddd,
                                            number: useCaseResult.Output.Phone),
                                        email: useCaseResult.Output.Email),
                                    address: QueryBankAccountSendloadOutputAddress.Factory(
                                        zipCode: useCaseResult.Output.AddressZipCode,
                                        streetName: useCaseResult.Output.AddressStreet,
                                        number: useCaseResult.Output.AddressNumber,
                                        complement: useCaseResult.Output.AddressComplement,
                                        district: useCaseResult.Output.AddressDistrict,
                                        city: useCaseResult.Output.AddressCity,
                                        state: useCaseResult.Output.AddressState,
                                        country: useCaseResult.Output.AddressCountry),
                                    isPoliticallyExposedPerson: useCaseResult.Output.HasPoliticallyExposedPerson),
                            legal: useCaseResult.Output.TypeAccount == TypeAccount.NATURAL ? null
                                : QueryBankAccountSendloadOutputLegal.Factory(
                                    comercialName: useCaseResult.Output.CommercialName!,
                                    socialReason: useCaseResult.Output.SocialReason!,
                                    document: QueryBankAccountSendloadOutputDocument.Factory(
                                        type: useCaseResult.Output.DocumentType,
                                        number: useCaseResult.Output.DocumentNumber),
                                    foundationDate: useCaseResult.Output.FoundationDate,
                                    constitutionType: useCaseResult.Output.TypeConstitution!,
                                    constitutionDate: ((DateTime)useCaseResult.Output.ConstitutionDate!).ToString("yyyy/MM/dd"),
                                    finance: QueryBankAccountSendloadOutputLegalFinance.Factory(
                                        monthlyInvoicing: useCaseResult.Output.MonthlyInvoicing,
                                        shareCapital: (decimal)useCaseResult.Output.ShareCapital!,
                                        patrimony: (decimal)useCaseResult.Output.Patrimony!),
                                    contact: QueryBankAccountSendloadOutputContact.Factory(
                                        phone: QueryBankAccountSendloadOutputPhone.Factory(
                                            ddi: useCaseResult.Output.Ddi,
                                            ddd: useCaseResult.Output.Ddd,
                                            number: useCaseResult.Output.Phone),
                                        email: useCaseResult.Output.Email),
                                    address: QueryBankAccountSendloadOutputAddress.Factory(
                                        zipCode: useCaseResult.Output.AddressZipCode,
                                        streetName: useCaseResult.Output.AddressStreet,
                                        number: useCaseResult.Output.AddressNumber,
                                        complement: useCaseResult.Output.AddressComplement,
                                        district: useCaseResult.Output.AddressDistrict,
                                        city: useCaseResult.Output.AddressCity,
                                        state: useCaseResult.Output.AddressState,
                                        country: useCaseResult.Output.AddressCountry),
                                    hasPoliticallyExposedPerson: useCaseResult.Output.HasPoliticallyExposedPerson,
                                    partners: useCaseResult.Output.Partners!.Select(p => QueryBankAccountSendloadOutputLegalPartner.Factory(
                                        legalName: p.LegalName,
                                        document: QueryBankAccountSendloadOutputDocument.Factory(
                                            type: p.DocumentType.ToString(),
                                            number: p.DocumentNumber),
                                        birthDate: p.BirthDate,
                                        personalInfo: QueryBankAccountSendloadOutputPersonalInfo.Factory(
                                        maritalStatus: p.MaritalStatus!,
                                        gender: p.Gender!,
                                        educationLevel: p.EducationLevel!),
                                    contact: QueryBankAccountSendloadOutputContact.Factory(
                                        phone: QueryBankAccountSendloadOutputPhone.Factory(
                                            ddi: p.Ddi,
                                            ddd: p.Ddd,
                                            number: p.Phone),
                                        email: p.Email),
                                    address: QueryBankAccountSendloadOutputAddress.Factory(
                                        zipCode: p.AddressZipCode,
                                        streetName: p.AddressStreet,
                                        number: p.AddressNumber,
                                        complement: p.AddressComplement,
                                        district: p.AddressDistrict,
                                        city: p.AddressCity,
                                        state: p.AddressState,
                                        country: p.AddressCountry),
                                    isPoliticallyExposedPerson: p.IsPoliticallyExposedPerson)).ToArray()),
                            lastModifiedAt: useCaseResult.Output.LastModifiedAt));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("oauth/token")]
    [Authorize(Roles = $"{nameof(Whitelabel)}")]
    public Task<IActionResult> HttpPostOAuthAccountAuthenticationAsync(
        [FromServices] IUseCase<OAuthAccountAuthenticationUseCaseInput, OAuthAccountAuthenticationUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [Required][FromHeader(Name = "X-UltraBank-Account-Id")] Guid accountId,
        [Required][FromHeader(Name = "X-UltraBank-Account-Secret")] string accountSecret,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromBody] OAuthAccountAuthenticationPayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(AccountController)}.{nameof(HttpPostOAuthAccountAuthenticationAsync)}",
            activityKind: ActivityKind.Internal,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var whitelabelId = new Guid(HttpContext.User.FindFirst("ClientId")!.Value);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: OAuthAccountAuthenticationUseCaseInput.Factory(
                        whitelabelId: whitelabelId,
                        accountId: accountId,
                        accountSecret: CryptographyTokenValueObject.Factory(accountSecret),
                        grantType: input.GrantType,
                        scope: ScopeValueObject.Factory(
                            typeScope: TypeScope.Account,
                            scope: input.Scope)),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status201Created,
                    value: OAuthAccountAuthenticationSendloadOutput.Factory(
                        accessToken: useCaseResult.Output.AccessToken,
                        tokenType: useCaseResult.Output.TokenType.GetTokenType(),
                        grantType: useCaseResult.Output.GrantType.GetGrantType(),
                        scope: useCaseResult.Output.Scope,
                        expiresIn: useCaseResult.Output.ExpiresIn.GetExpiresIn(),
                        notifications: useCaseResult.Notifications ?? []));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("natural-person")]
    [Authorize(Roles = $"{nameof(Whitelabel)}")]
    public Task<IActionResult> CreateNaturalPersonBaasAccountAsync(
        [FromServices] IUseCase<CreateNaturalPersonAccountUseCaseInput, CreateNaturalPersonAccountUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromQuery] string? provider,
        [FromBody] CreateNaturalPersonAccountPayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        var clientId = new Guid(HttpContext.User.FindFirst("ClientId")!.Value);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(AccountController)}.{nameof(CreateNaturalPersonBaasAccountAsync)}",
            activityKind: ActivityKind.Internal,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: CreateNaturalPersonAccountUseCaseInput.Factory(
                        whitelabelId: clientId,
                        provider: BaasProviderValueObject.Factory(
                            provider: provider ?? _providerDefault),
                        name: input.Name,
                        surname: input.Surname,
                        cpf: NaturalPersonDocumentValueObject.Factory(
                            typeDocument: TypeNaturalPersonDocument.CPF.ToString(),
                            documentValue: input.Cpf),
                        email: input.Email,
                        phone: PhoneValueObject.Factory(
                            ddi: input.Phone.Ddi,
                            ddd: input.Phone.Ddd,
                            phone: input.Phone.Number),
                        documents: input.Documents.Select(p => NaturalPersonDocumentValueObject.Factory(
                            typeDocument: p.DocumentType,
                            documentValue: p.DocumentNumber)).ToArray(),
                        birthDate: input.BirthDate,
                        monthlyInvoicing: input.MonthlyInvoicing,
                        maritalStatus: input.MaritalStatus,
                        educationLevel: input.EducationLevel,
                        gender: input.Gender,
                        address: AddressValueObject.Factory(
                            zipCode: input.Address.ZipCode,
                            streetName: input.Address.StreetName,
                            number: input.Address.Number,
                            complement: input.Address.Complement,
                            district: input.Address.District,
                            city: input.Address.City,
                            state: input.Address.State,
                            country: input.Address.Country),
                        isPoliticallyExposedPerson: input.IsPoliticallyExposedPerson,
                        notificationUrls: input.NotificationUrls.Select(p => UrlValueObject.Factory(p)).ToArray()),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status201Created,
                    value: CreateAccountSendloadOutput.Factory(
                        accountId: useCaseResult.Output.AccountId,
                        typeAccountStatus: useCaseResult.Output.Status.ToString(),
                        notifications: useCaseResult.Notifications ?? []));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: [
                KeyValuePair.Create(
                    key: TraceTitles.TraceHttpClientEndpoint,
                    value: HttpContext.Request.Path.Value ?? string.Empty),
                KeyValuePair.Create(
                    key: TraceTitles.TraceBaasProvider,
                    value: provider ?? _providerDefault),
                KeyValuePair.Create(
                    key: TraceTitles.TraceClientIdWhiteLabel,
                    value: clientId.ToString())
                ]);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("legal-person")]
    [Authorize(Roles = $"{nameof(Whitelabel)}")]
    public Task<IActionResult> CreateLegalPersonBaasAccountAsync(
        [FromServices] IUseCase<CreateLegalPersonAccountUseCaseInput, CreateLegalPersonAccountUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromQuery] string? provider,
        [FromBody] CreateLegalPersonAccountPayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        var clientId = new Guid(HttpContext.User.FindFirst("ClientId")!.Value);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(AccountController)}.{nameof(CreateLegalPersonBaasAccountAsync)}",
            activityKind: ActivityKind.Internal,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: CreateLegalPersonAccountUseCaseInput.Factory(
                        whitelabelId: clientId,
                        provider: BaasProviderValueObject.Factory(
                            provider: provider ?? _providerDefault),
                        comercialName: input.ComercialName,
                        socialReason: input.SocialReason,
                        cnpj: LegalPersonDocumentValueObject.Factory(
                            typeDocument: TypeLegalPersonDocument.CNPJ.ToString(),
                            content: input.Cnpj),
                        foundationDate: input.FoundationDate,
                        email: input.Email,
                        phone: PhoneValueObject.Factory(
                            ddi: input.Phone.Ddi,
                            ddd: input.Phone.Ddd,
                            phone: input.Phone.Number),
                        primaryCnaeCode: input.PrimaryCnaeCode,
                        documents: input.Documents.Select(p => LegalPersonDocumentValueObject.Factory(
                            typeDocument: p.DocumentType,
                            content: p.DocumentNumber)).ToArray(),
                        secondaryCnaeCodes: input.SecondaryCnaeCodes.Select(p => CnaeValueObject.Factory(p)).ToArray(),
                        constitution: input.Constitution,
                        constitutionDate: input.ConstitutionDate,
                        patrimony: input.Finance.Patrimony,
                        shareCapital: input.Finance.ShareCapital,
                        monthlyInvoicing: input.Finance.MonthlyInvoicing,
                        address: AddressValueObject.Factory(
                            zipCode: input.Address.ZipCode,
                            streetName: input.Address.StreetName,
                            number: input.Address.Number,
                            complement: input.Address.Complement,
                            district: input.Address.District,
                            city: input.Address.City,
                            state: input.Address.State,
                            country: input.Address.Country),
                        hasPoliticallyExposedPerson: input.HasPoliticallyExposedPartner,
                        partners: input.Partners.Select(p => CreateLegalPersonAccountUseCaseInputPartner.Factory(
                            legalName: p.LegalName,
                            document: p.Document,
                            email: p.Email,
                            phone: PhoneValueObject.Factory(
                                ddi: p.Phone.Ddi,
                                ddd: p.Phone.Ddd,
                                phone: p.Phone.Number),
                            naturalDocuments: p.Documents is not null && p.Document.Length == 11
                                ? p.Documents.Select(q => NaturalPersonDocumentValueObject.Factory(
                                    typeDocument: q.DocumentType,
                                    documentValue: q.DocumentNumber)).ToArray() 
                                : [],
                            legalDocuments: p.Documents is not null && p.Document.Length == 14
                                ? p.Documents.Select(q => LegalPersonDocumentValueObject.Factory(
                                    typeDocument: q.DocumentType,
                                    content: q.DocumentNumber)).ToArray()
                                : [],
                            birthDate: p.BirthDate,
                            monthlyInvoicing: p.MonthlyInvoicing,
                            maritalStatus: p.MaritalStatus,
                            educationLevel: p.EducationLevel,
                            gender: p.Gender,
                            address: AddressValueObject.Factory(
                                    zipCode: p.Address.ZipCode,
                                    streetName: p.Address.StreetName,
                                    number: p.Address.Number,
                                    complement: p.Address.Complement,
                                    district: p.Address.District,
                                    city: p.Address.City,
                                    state: p.Address.State,
                                    country: p.Address.Country),
                            isPoliticallyExposedPerson: p.IsPoliticallyExposedPerson)).ToArray(),
                        notificationUrls: input.NotificationUrls.Select(p => UrlValueObject.Factory(p)).ToArray()),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status201Created,
                    value: CreateAccountSendloadOutput.Factory(
                        accountId: useCaseResult.Output.AccountId,
                        typeAccountStatus: useCaseResult.Output.Status.ToString(),
                        notifications: useCaseResult.Notifications ?? []));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: [
                KeyValuePair.Create(
                    key: TraceTitles.TraceHttpClientEndpoint,
                    value: HttpContext.Request.Path.Value ?? string.Empty),
                KeyValuePair.Create(
                    key: TraceTitles.TraceBaasProvider,
                    value: provider ?? _providerDefault),
                KeyValuePair.Create(
                    key: TraceTitles.TraceClientIdWhiteLabel,
                    value: clientId.ToString())
                ]);
    }
}
