using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using UltraBank.Application.UseCases.Interfaces;
using UltraBank.Application.UseCases.PixContext.CreateDynamicChargePix.Inputs;
using UltraBank.Application.UseCases.PixContext.CreateDynamicChargePix.Outputs;
using UltraBank.Application.UseCases.PixContext.CreateDynamicSplitChargePix.Inputs;
using UltraBank.Application.UseCases.PixContext.CreateDynamicSplitChargePix.Outputs;
using UltraBank.Application.UseCases.PixContext.PayDynamicChargePix.Inputs;
using UltraBank.Application.UseCases.PixContext.PayDynamicChargePix.Outputs;
using UltraBank.Application.UseCases.PixContext.QueryChargePix.Inputs;
using UltraBank.Application.UseCases.PixContext.QueryChargePix.Outputs;
using UltraBank.Application.UseCases.PixContext.RefundChargePix.Inputs;
using UltraBank.Application.UseCases.PixContext.RefundChargePix.Outputs;
using UltraBank.AuditableInfoContext;
using UltraBank.Domain.BoundedContexts.AccountContext.DataTransferObject;
using UltraBank.Domain.ValueObjects;
using UltraBank.ObservabilityContext.Metric.Interfaces;
using UltraBank.ObservabilityContext.Trace.Interfaces;
using UltraBank.WebApi.Controllers.ChargesContext.Payloads;
using UltraBank.WebApi.Controllers.ChargesContext.Sendloads;

namespace UltraBank.WebApi.Controllers.ChargesContext;

[Route("api/v1/baas/charges")]
[ApiController]
public sealed class ChargesController : CustomizedControllerBase
{
    public ChargesController(ITraceManager traceManager, IMetricManager metricManager) : base(traceManager, metricManager)
    {
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = $"{nameof(Account)}")]
    [Route("pay")]
    public Task<IActionResult> HttpPostPayPixChargeAsync(
        [FromServices] IUseCase<PayDynamicChargePixUseCaseInput, PayDynamicChargePixUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromBody] PayPixChargePayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(ChargesController)}.{nameof(HttpPostPayPixChargeAsync)}",
            activityKind: ActivityKind.Server,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var issuerId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: PayDynamicChargePixUseCaseInput.Factory(
                        accountId: issuerId,
                        whitelabelId: whitelabelId,
                        pixCopiaECola: input.PixCopiaECola.Replace("ultrabank", "delbank").Replace("ULTRABANK", "DELBANK")),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status200OK,
                    value: PayPixChargePayloadOutput.Factory(
                        correlationId: auditableInfo.GetCorrelationId(),
                        endToEndId: useCaseResult.Output.EndToEndId,
                        paidAt: auditableInfo.GetRequestTime()));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }


    [HttpPut]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = $"{nameof(Account)}")]
    [Route("{endToEndId}/refund")]
    public Task<IActionResult> HttpPutCreateRefundChargePixAsync(
        [FromServices] IUseCase<RefundChargePixUseCaseInput, RefundChargePixUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromRoute] string endToEndId,
        [FromBody] RefundChargePixPayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(ChargesController)}.{nameof(HttpPutCreateRefundChargePixAsync)}",
            activityKind: ActivityKind.Server,
            input: input,
            handler: async (input, auditableInfo, activtiy, cancellationToken) =>
            {
                var issuerId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                if (!HasScopeOnAuthentication(scope, "refund.create"))
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status401Unauthorized,
                        value: ScopeInvalid);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: RefundChargePixUseCaseInput.Factory(
                        accountId: issuerId,
                        endToEndId: endToEndId,
                        amount: input.Amount,
                        description: input.Description),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status200OK,
                    value: RefundChargePixSendloadOutput.Factory(
                        chargeId: useCaseResult.Output.ChargeId,
                        createdAt: useCaseResult.Output.CreatedAt,
                        type: useCaseResult.Output.Type,
                        status: useCaseResult.Output.Status,
                        amount: useCaseResult.Output.Amount,
                        description: useCaseResult.Output.Description,
                        txid: useCaseResult.Output.Txid,
                        lastModifiedAt: useCaseResult.Output.LastModifiedAt,
                        paymentReceives: [
                            RefundChargePixSendloadOutputPaymentReceive.Factory(
                                paidId: useCaseResult.Output.PaymentReceives[0].PaidId,
                                paidAt: useCaseResult.Output.PaymentReceives[0].PaidAt,
                                amount: useCaseResult.Output.PaymentReceives[0].Amount,
                                endToEndId: useCaseResult.Output.PaymentReceives[0].EndToEndId,
                                name: useCaseResult.Output.PaymentReceives[0].Name,
                                document: useCaseResult.Output.PaymentReceives[0].Document,
                                refunds: useCaseResult.Output.PaymentReceives[0].Refunds.Select(p =>
                                        RefundChargePixSendloadOutputRefund.Factory(
                                            refundId: p.RefundId,
                                            type: p.Type,
                                            rtrId: p.RtrId,
                                            amount: p.Amount,
                                            description: p.Description)
                                ).ToArray())
                            ],
                        notifications: useCaseResult.Notifications ?? []));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Account)}")]
    [Route("pix/{id}")]
    public Task<IActionResult> HttpGetQueryChargePixAsync(
        [FromServices] IUseCase<QueryChargePixUseCaseInput, QueryChargePixUseCaseOutput> useCase,
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
            traceName: $"{nameof(ChargesController)}.{nameof(HttpPostCreateDynamicChargePixAsync)}",
            activityKind: ActivityKind.Server,
            input: id,
            handler: async (chargeId, auditableInfo, activtiy, cancellationToken) =>
            {
                var issuerId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                if (!HasScopeOnAuthentication(scope, "pix.read"))
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status401Unauthorized,
                        value: ScopeInvalid);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: QueryChargePixUseCaseInput.Factory(
                        chargeId: chargeId,
                        accountId: issuerId),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                        statusCode: StatusCodes.Status200OK,
                        value: QueryChargePixSendloadOutput.Factory(
                            chargeId: useCaseResult.Output.ChargeId,
                            createdAt: useCaseResult.Output.CreatedAt,
                            type: useCaseResult.Output.Type,
                            status: useCaseResult.Output.Status,
                            txid: useCaseResult.Output.Txid,
                            amount: useCaseResult.Output.Amount,
                            description: useCaseResult.Output.Description,
                            metadata: useCaseResult.Output.Metadata,
                            payments: useCaseResult.Output.Payments.Select(p => QueryChargePixSendloadOutputPaid.Factory(
                                paidId: p.PaidId,
                                paidAt: p.PaidAt,
                                amount: p.Amount,
                                endToEndId: p.EndToEndId,
                                name: p.Name,
                                document: p.Document,
                                refunds: p.Refunds.Select(q => QueryChargePixSendloadOutputPaidRefund.Factory(
                                    refundId: q.RefundId,
                                    refundedAt: q.RefundedAt,
                                    type: q.Type,
                                    rtrId: q.RtrId,
                                    amount: q.Amount,
                                    description: q.Description)).ToArray())).ToArray(),
                            notificationUrls: useCaseResult.Output.NotificationUrls,
                            issuedBy: useCaseResult.Output.IssuedBy,
                            originatorSplit: useCaseResult.Output.OriginatorSplit,
                            secondaries: useCaseResult.Output.Secondaries is not null
                                ? useCaseResult.Output.Secondaries.Select(p => QueryChargePixSendloadOutputSplit.Factory(
                                    receiverAccountId: p.ReceiverAccountId,
                                    description: p.Description,
                                    split: p.Split)).ToArray()
                                : null,
                            lastModifiedAt: useCaseResult.Output.LastModifiedAt,
                            notifications: useCaseResult.Notifications ?? []));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Account)}")]
    [Route("dynamic/pix")]
    public Task<IActionResult> HttpPostCreateDynamicChargePixAsync(
        [FromServices] IUseCase<CreateDynamicChargePixUseCaseInput, CreateDynamicChargePixUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromBody] CreateDynamicChargePixPayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(ChargesController)}.{nameof(HttpPostCreateDynamicChargePixAsync)}",
            activityKind: ActivityKind.Server,
            input: input,
            handler: async (input, auditableInfo, activtiy, cancellationToken) =>
            {
                var issuerId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                if (!HasScopeOnAuthentication(scope, "pix.create"))
                    return StatusCode(
                        statusCode: StatusCodes.Status401Unauthorized,
                        value: ScopeInvalid);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: CreateDynamicChargePixUseCaseInput.Factory(
                        whitelabelId: whitelabelId,
                        accountId: issuerId,
                        amount: input.Amount,
                        expiresIn: PixExpirationValueObject.Factory(input.ExpiresIn),
                        document: input.Debtor is not null ? 
                            DocumentValueObject.Factory(
                                type: input.Debtor.Value.DocumentType,
                                document: input.Debtor.Value.Document)
                            : (DocumentValueObject?)null,
                        name: input.Debtor is not null ? 
                            DebtorNameValueObject.Factory(input.Debtor.Value.Name) :
                            (DebtorNameValueObject?)null,
                        description: input.Description,
                        metadata: input.Metadata is null 
                            ? []
                            : input.Metadata.Select(p => MetadataValueObject.Factory(
                                key: p.Key,
                                metadata: p.Metadata)).ToArray(),
                        notificationUrls: input.NotificationUrls.Select(p => UrlValueObject.Factory(p)).ToArray()),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                        statusCode: StatusCodes.Status201Created,
                        value: CreateDynamicChargePixSendloadOutput.Factory(
                            chargeId: useCaseResult.Output.ChargeId,
                            createdAt: useCaseResult.Output.CreatedAt,
                            type: useCaseResult.Output.Type,
                            status: useCaseResult.Output.Status,
                            txid: useCaseResult.Output.Txid,
                            copyAndPaste: useCaseResult.Output.CopyAndPaste,
                            amount: useCaseResult.Output.Amount,
                            expiresIn: useCaseResult.Output.ExpiresIn,
                            description: useCaseResult.Output.Description,
                            metadata: useCaseResult.Output.Metadata,
                            qrCodeBase64: useCaseResult.Output.QrCodeBase64,
                            notifications: useCaseResult.Notifications ?? []));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Account)}")]
    [Route("dynamic/pix/split")]
    public Task<IActionResult> HttpPostCreateDynamicSplitChargePixAsync(
        [FromServices] IUseCase<CreateDynamicSplitChargePixUseCaseInput, CreateDynamicSplitChargePixUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromBody] CreateDynamicSplitChargePixPayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(ChargesController)}.{nameof(HttpPostCreateDynamicChargePixAsync)}",
            activityKind: ActivityKind.Server,
            input: input,
            handler: async (input, auditableInfo, activtiy, cancellationToken) =>
            {
                var issuerId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                if (!HasScopeOnAuthentication(scope, "pix.create"))
                    return StatusCode(
                        statusCode: StatusCodes.Status401Unauthorized,
                        value: ScopeInvalid);

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: CreateDynamicSplitChargePixUseCaseInput.Factory(
                        originatorId: issuerId,
                        amount: input.Amount,
                        expiresIn: PixExpirationValueObject.Factory(input.ExpiresIn),
                        document: input.Debtor is not null ?
                            DocumentValueObject.Factory(
                                type: input.Debtor.Value.DocumentType,
                                document: input.Debtor.Value.Document)
                            : (DocumentValueObject?)null,
                        name: input.Debtor is not null ?
                            DebtorNameValueObject.Factory(input.Debtor.Value.Name) :
                            (DebtorNameValueObject?)null,
                        description: input.Description,
                        metadata: input.Metadata is null
                            ? []
                            : input.Metadata.Select(p => MetadataValueObject.Factory(
                                key: p.Key,
                                metadata: p.Metadata)).ToArray(),
                        notificationUrls: input.NotificationUrls.Select(p => UrlValueObject.Factory(p)).ToArray(),
                        originatorSplit: SplitValueObject.Factory(
                            typeSplit: input.OriginatorSplit.TypeSplit,
                            amountSplit: input.OriginatorSplit.AmountSplit),
                        secondaries: input.Secondaries.Select(p => CreateDynamicSplitChargePixUseCaseInputSecondary.Factory(
                            receiverAccountId: p.ReceiverAccountId,
                            split: SplitValueObject.Factory(
                                typeSplit: p.TypeSplit,
                                amountSplit: p.AmountSplit),
                            description: p.Description,
                            extractGroup: p.ExtractGroup)).ToArray()),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                        statusCode: StatusCodes.Status201Created,
                        value: CreateDynamicSplitChargePixSendloadOutput.Factory(
                            chargeId: useCaseResult.Output.ChargeId,
                            createdAt: useCaseResult.Output.CreatedAt,
                            type: useCaseResult.Output.Type,
                            status: useCaseResult.Output.Status,
                            txid: useCaseResult.Output.Txid,
                            copyAndPaste: useCaseResult.Output.CopyAndPaste,
                            amount: useCaseResult.Output.Amount,
                            expiresIn: useCaseResult.Output.ExpiresIn,
                            description: useCaseResult.Output.Description,
                            metadata: useCaseResult.Output.Metadata,
                            qrCodeBase64: useCaseResult.Output.QrCodeBase64,
                            notifications: useCaseResult.Notifications ?? [],
                            originatorSplit: new CreateDynamicSplitChargePixSendloadOutputOriginatorSplit(
                                typeSplit: useCaseResult.Output.OriginatorSplit.GetTypeSplit().ToString(),
                                amountSplit: useCaseResult.Output.OriginatorSplit.GetAmountSplit()),
                            secondaries: useCaseResult.Output.Secondaries.Select(p => new CreateDynamicSplitChargePixSendloadOutputSplit(
                                receiverAccountId: p.ReceiverAccountId,
                                typeSplit: p.Split.GetTypeSplit().ToString(),
                                amountSplit: p.Split.GetAmountSplit(),
                                description: p.Description,
                                extractGroup: p.ExtractGroup)).ToArray()));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }
}
