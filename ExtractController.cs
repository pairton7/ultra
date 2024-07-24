using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using UltraBank.Application.UseCases.Interfaces;
using UltraBank.Application.UseCases.TransferContext.QueryBankTransfer.Inputs;
using UltraBank.Application.UseCases.TransferContext.QueryBankTransfer.Outputs;
using UltraBank.AuditableInfoContext;
using UltraBank.Domain.BoundedContexts.AccountContext.DataTransferObject;
using UltraBank.Domain.BoundedContexts.WhitelabelContext.DataTransferObject;
using UltraBank.ObservabilityContext.Metric.Interfaces;
using UltraBank.ObservabilityContext.Trace.Interfaces;
using UltraBank.WebApi.Controllers.TransferContext.Sendloads;
using UltraBank.WebApi.Controllers.TransferContext;
using UltraBank.Application.UseCases.ExtractContext.QueryExtract.Inputs;
using UltraBank.Application.UseCases.ExtractContext.QueryExtract.Outputs;

namespace UltraBank.WebApi.Controllers.ExtractContext;

[Route("api/v1/baas/extract")]
[ApiController]
public sealed class ExtractController : CustomizedControllerBase
{
    public ExtractController(ITraceManager traceManager, IMetricManager metricManager) : base(traceManager, metricManager)
    {
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Account)}")]
    public Task<IActionResult> HttpGetQueryExtractAsync(
        [FromServices] IUseCase<QueryExtractUseCaseInput, QueryExtractUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 25,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(ExtractController)}.{nameof(HttpGetQueryExtractAsync)}",
            activityKind: ActivityKind.Server,
            input: (page, limit, startDate, endDate),
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var issuerId = new Guid(HttpContext.User.FindFirst("AccountId")!.Value);
                var whitelabelId = new Guid(HttpContext.User.FindFirst("WhitelabelId")!.Value);
                var scope = HttpContext.User.FindFirst("Scope")!.Value;

                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: QueryExtractUseCaseInput.Factory(
                        whitelabelId: whitelabelId,
                        accountId: issuerId,
                        page: page,
                        limit: limit,
                        startDate: input.startDate,
                        endDate: input.endDate),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status200OK,
                    value: useCaseResult.Output.Extract);
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }
}
