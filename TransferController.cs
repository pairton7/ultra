using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Mime;
using UltraBank.Application.UseCases.Interfaces;
using UltraBank.Application.UseCases.TransferContext.CreateBankTransfer.Inputs;
using UltraBank.Application.UseCases.TransferContext.CreateBankTransfer.Outputs;
using UltraBank.Application.UseCases.TransferContext.CreateOutBankTransfer.Inputs;
using UltraBank.Application.UseCases.TransferContext.CreateOutBankTransfer.Outputs;
using UltraBank.Application.UseCases.TransferContext.QueryBankTransfer.Inputs;
using UltraBank.Application.UseCases.TransferContext.QueryBankTransfer.Outputs;
using UltraBank.AuditableInfoContext;
using UltraBank.Domain.BoundedContexts.AccountContext.DataTransferObject;
using UltraBank.Domain.BoundedContexts.WhitelabelContext.DataTransferObject;
using UltraBank.Domain.ValueObjects;
using UltraBank.ObservabilityContext.Metric.Interfaces;
using UltraBank.ObservabilityContext.Trace.Interfaces;
using UltraBank.WebApi.Controllers.TransferContext.Payloads;
using UltraBank.WebApi.Controllers.TransferContext.Sendloads;

namespace UltraBank.WebApi.Controllers.TransferContext;

[Route("api/v1/baas/transfer")]
[ApiController]
public sealed class TransferController : CustomizedControllerBase
{
    public TransferController(ITraceManager traceManager, IMetricManager metricManager) : base(traceManager, metricManager)
    {
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Account)},{nameof(Whitelabel)}")]
    [Route("{id}")]
    public Task<IActionResult> HttpGetQueryBankTransferAsync(
        [FromServices] IUseCase<QueryBankTransferUseCaseInput, QueryBankTransferUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [Required][FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(TransferController)}.{nameof(HttpGetQueryBankTransferAsync)}",
            activityKind: ActivityKind.Server,
            input: id,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var issuerId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                if (!HasScopeOnAuthentication(scope, "transfer.read"))
                    return StatusCode(
                        statusCode: StatusCodes.Status401Unauthorized,
                        value: ScopeInvalid);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: QueryBankTransferUseCaseInput.Factory(
                        accountId: issuerId,
                        whitelabelId: whitelabelId,
                        transferId: input),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status200OK,
                    value: QueryBankTransferSendloadOutput.Factory(
                        transferId: useCaseResult.Output.TransferId,
                        createdAt: useCaseResult.Output.CreatedAt,
                        type: useCaseResult.Output.Type,
                        amount: useCaseResult.Output.Amount,
                        metadata: useCaseResult.Output.Metadata,
                        issuer: QueryBankTransferSendloadOutputParticipant.Factory(
                            accountId: useCaseResult.Output.Issuer.AccountId,
                            document: useCaseResult.Output.Issuer.Document,
                            name: useCaseResult.Output.Issuer.Name,
                            participantType: useCaseResult.Output.Issuer.ParticipantType),
                        beneficiary: QueryBankTransferSendloadOutputParticipant.Factory(
                            accountId: useCaseResult.Output.Beneficiary.AccountId,
                            document: useCaseResult.Output.Beneficiary.Document,
                            name: useCaseResult.Output.Beneficiary.Name,
                            participantType: useCaseResult.Output.Beneficiary.ParticipantType)));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = $"{nameof(Account)}")]
    public Task<IActionResult> HttpPostCreateTransferAsync(
        [FromServices] IUseCase<CreateBankTransferUseCaseInput, CreateBankTransferUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromBody] CreateTransferPayloadInput input)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(TransferController)}.{nameof(HttpPostCreateTransferAsync)}",
            activityKind: ActivityKind.Server,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var issuerId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                if (!HasScopeOnAuthentication(scope, "transfer.create"))
                    return StatusCode(
                        statusCode: StatusCodes.Status401Unauthorized,
                        value: ScopeInvalid);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: CreateBankTransferUseCaseInput.Factory(
                        issuerId: issuerId,
                        whitelabelId: whitelabelId,
                        amount: input.Amount,
                        beneficiaryAccountId: input.BeneficiaryAccountId,
                        metadata: input.Metadata is null ? [] : input.Metadata.Select(p => MetadataValueObject.Factory(
                            key: p.Key,
                            metadata: p.Metadata)).ToArray(),
                        description: input.Description,
                        externalTransferId: input.ExternalTransferId),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status200OK,
                    value: CreateBankTransferSendloadOutput.Factory(
                        transferId: useCaseResult.Output.TransferId,
                        createdAt: useCaseResult.Output.CreatedAt,
                        type: useCaseResult.Output.Type,
                        amount: useCaseResult.Output.Amount,
                        metadata: useCaseResult.Output.Metadata,
                        beneficiaryId: useCaseResult.Output.Beneficiary.ParticipantId,
                        notifications: useCaseResult.Notifications ?? []));
            },
            auditableInfo: auditableInfo,
            cancellationToken: CancellationToken.None,
            keyValuePairs: []);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = $"{nameof(Account)}")]
    [Route("bank")]
    public Task<IActionResult> HttpPostCreateTransferAsync(
        [FromServices] IUseCase<CreateOutBankTransferUseCaseInput, CreateOutBankTransferUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromBody] CreateOutTransferPayloadInput input)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(TransferController)}.{nameof(HttpPostCreateTransferAsync)}",
            activityKind: ActivityKind.Server,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var issuerId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                if (!HasScopeOnAuthentication(scope, "transfer.create"))
                    return StatusCode(
                        statusCode: StatusCodes.Status401Unauthorized,
                        value: ScopeInvalid);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: CreateOutBankTransferUseCaseInput.Factory(
                        accountId: issuerId,
                        amount: input.Amount,
                        metadata: input.Metadata.Select(p => MetadataValueObject.Factory(
                            key: p.Key,
                            metadata: p.Metadata)).ToArray(),
                        description: input.Description,
                        externalTransferId: input.ExternalTransferId,
                        beneficiary: CreateOutBankTransferUseCaseInputBeneficiary.Factory(
                            type: input.Beneficiary.Type,
                            number: input.Beneficiary.Number,
                            branch: input.Beneficiary.Branch,
                            holderDocument: input.Beneficiary.HolderDocument,
                            holderName: input.Beneficiary.HolderName,
                            ispbCode: input.Beneficiary.IspbCode),
                        notificationUrls: input.NotificationUrls.Select(p => UrlValueObject.Factory(p)).ToArray()),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status200OK,
                    value: CreateOutBankTransferSendloadOutput.Factory(
                        transferId: useCaseResult.Output.TransferId,
                        createdAt: useCaseResult.Output.CreatedAt,
                        type: useCaseResult.Output.Type,
                        status: useCaseResult.Output.Status,
                        amount: useCaseResult.Output.Amount,
                        issuerId: useCaseResult.Output.IssuerId,
                        beneficiary: CreateOutBankTransferSendloadOutputParticipant.Factory(
                            type: useCaseResult.Output.Beneficiary.Type,
                            account: CreateOutBankTransferSendloadOutputParticipantAccount.Factory(
                                number: useCaseResult.Output.Beneficiary.Account.Number,
                                branch: useCaseResult.Output.Beneficiary.Account.Branch,
                                holderDocument: useCaseResult.Output.Beneficiary.Account.HolderDocument,
                                holderName: useCaseResult.Output.Beneficiary.Account.HolderName),
                            bankName: useCaseResult.Output.Beneficiary.BankName,
                            bankIspb: useCaseResult.Output.Beneficiary.BankIspb),
                        metadata: useCaseResult.Output.Metadata,
                        notifications: useCaseResult.Notifications ?? []));
            },
            auditableInfo: auditableInfo,
            cancellationToken: CancellationToken.None,
            keyValuePairs: []);
    }
}
