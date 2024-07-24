using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mime;
using UltraBank.Application.UseCases.AccountContext.UpdateAccountAnalysis.Inputs;
using UltraBank.Application.UseCases.AccountContext.UpdateAccountAnalysis.Outputs;
using UltraBank.Application.UseCases.Interfaces;
using UltraBank.Application.UseCases.PixContext.UpdateDynamicChargePixPaid.Inputs;
using UltraBank.Application.UseCases.PixContext.UpdateDynamicChargePixPaid.Outputs;
using UltraBank.Application.UseCases.TransferContext.UpdateOutBankTransferEvent.Inputs;
using UltraBank.Application.UseCases.TransferContext.UpdateOutBankTransferEvent.Outputs;
using UltraBank.AuditableInfoContext;
using UltraBank.Domain.BoundedContexts.AccountContext.ENUMs;
using UltraBank.Domain.BoundedContexts.TransferContext.ENUMs;
using UltraBank.Domain.Utils;
using UltraBank.Domain.ValueObjects;
using UltraBank.ObservabilityContext.Metric.Interfaces;
using UltraBank.ObservabilityContext.Trace.Interfaces;
using UltraBank.WebApi.Controllers;
using UltraBank.WebApi.Webhooks.DellBankContext.Inputs;

namespace UltraBank.WebApi.Webhooks.DellBankContext;

[Route("api/v1/baas/dellbank")]
[ApiController]
public sealed class DellBankWebhook : CustomizedControllerBase
{
    public DellBankWebhook(ITraceManager traceManager, IMetricManager metricManager) : base(traceManager, metricManager)
    {
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("event")]
    [AllowAnonymous]
    public Task<IActionResult> HttpPostAccountHasCreatedEventAsync(
        [FromServices] IUseCase<UpdateAccountAnalysisUseCaseInput, UpdateAccountAnalysisUseCaseOutput> useCase,
        [FromBody] AccountUpdatedEventDellBankPayloadInput input,
        [FromQuery] bool isApproved)
    {
        var correlationId = HttpContext.Items[AuditableInfoValueObject.HeaderCorrelationIdKey];

        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId is null ? Guid.NewGuid() : new Guid(correlationId.ToString()!), 
            developerResponsible: null,
            sourcePlatform: null);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(DellBankWebhook)}.{nameof(HttpPostAccountHasCreatedEventAsync)}",
            activityKind: ActivityKind.Server,
            input: (input, isApproved),
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: UpdateAccountAnalysisUseCaseInput.Factory(
                        document: input.input.document,
                        status: input.isApproved ? TypeAccountStatus.ACTIVE : TypeAccountStatus.DECLINED),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: null);

                return NoContent();
            },
            auditableInfo: auditableInfo,
            cancellationToken: CancellationToken.None);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("transfers/event")]
    [AllowAnonymous]
    public Task<IActionResult> HttpPostOutTransferEventAsync(
        [FromServices] IUseCase<UpdateOutBankTransferEventUseCaseInput, UpdateOutBankTransferEventUseCaseOutput> useCase,
        [FromBody] OutTransferEventPayloadInput input)
    {
        var correlationId = HttpContext.Items[AuditableInfoValueObject.HeaderCorrelationIdKey];

        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId is null ? Guid.NewGuid() : new Guid(correlationId.ToString()!),
            developerResponsible: null,
            sourcePlatform: null);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(DellBankWebhook)}.{nameof(HttpPostAccountHasCreatedEventAsync)}",
            activityKind: ActivityKind.Server,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: UpdateOutBankTransferEventUseCaseInput.Factory(
                        status: input.Status == "PIX_EFFECTIVE" ? TransferStatusValueObject.Factory(
                            status: TypeTransferStatus.EFFECTIVE.ToString()) : TransferStatusValueObject.Factory(
                            status: TypeTransferStatus.DECLINED.ToString()),
                        endToEndId: input.EndToEndId,
                        rejectedDescription: input.Error is not null ? TransferDescriptionValueObject.Factory(input.Error.Value.Description) : null),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return (IActionResult)NoContent();
            },
            auditableInfo: auditableInfo,
            cancellationToken: CancellationToken.None);
    } 

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("charges/pix/event")]
    [AllowAnonymous]
    public Task<IActionResult> HttpPostChargePixPaidEventAsync(
        [FromServices] IUseCase<UpdateDynamicChargePixPaidUseCaseInput, UpdateDynamicChargePixPaidUseCaseOutput> useCase,
        [FromBody] PixPaidEventDellBankPayloadInput input)
    {
        var correlationId = HttpContext.Items[AuditableInfoValueObject.HeaderCorrelationIdKey];

        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId is null ? Guid.NewGuid() : new Guid(correlationId.ToString()!),
            developerResponsible: null,
            sourcePlatform: null);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(DellBankWebhook)}.{nameof(HttpPostAccountHasCreatedEventAsync)}",
            activityKind: ActivityKind.Server,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: UpdateDynamicChargePixPaidUseCaseInput.Factory(
                        chargeId: new Guid(input.correlationId),
                        amountPaid: input.amount,
                        payer: UpdateChargePixPaidUseCaseInputPayer.Factory(
                            name: input.proof.payer.holder.name!,
                            document: input.proof.payer.holder.document,
                            typeAccount: input.proof.payer.holder.type,
                            bankName: input.proof.payer.participant.name,
                            bankIspb: input.proof.payer.participant.ispb),
                        endToEndId: input.endToEndId),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return NoContent();
            },
            auditableInfo: auditableInfo,
            cancellationToken: CancellationToken.None,
            keyValuePairs: [
                KeyValuePair.Create(
                    key: TraceTitles.TracePixPayerDocumentNumber,
                    value: input.proof.payer.holder.document),
                KeyValuePair.Create(
                    key: TraceTitles.TracePixPayerName,
                    value: input.proof.payer.holder.name!),
                KeyValuePair.Create(
                    key: TraceTitles.TracePixPayerBankName,
                    value: input.proof.payer.participant.name),
                KeyValuePair.Create(
                    key: TraceTitles.TracePixPayerAccountNumber,
                    value: input.proof.payer.number),
                KeyValuePair.Create(
                    key: TraceTitles.TracePixPayerAccountBranch,
                    value: input.proof.payer.branch),
                KeyValuePair.Create(
                    key: TraceTitles.TracePixPayerAccountType,
                    value: input.proof.payer.holder.type),
                KeyValuePair.Create(
                    key: TraceTitles.TracePixInfoId,
                    value: input.correlationId),
                KeyValuePair.Create(
                    key: TraceTitles.TracePixInfoEndToEndId,
                    value: input.endToEndId)
                ]);
            
    }
}
