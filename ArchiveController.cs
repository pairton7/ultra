using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Mime;
using UltraBank.Application.UseCases.ArchiveContext.SubmitArchive.Inputs;
using UltraBank.Application.UseCases.ArchiveContext.SubmitArchive.Outputs;
using UltraBank.Application.UseCases.Interfaces;
using UltraBank.AuditableInfoContext;
using UltraBank.Domain.BoundedContexts.ArchiveContext.ENUMs;
using UltraBank.Domain.BoundedContexts.WhitelabelContext.DataTransferObject;
using UltraBank.Domain.Utils;
using UltraBank.Domain.ValueObjects;
using UltraBank.ObservabilityContext.Metric.Interfaces;
using UltraBank.ObservabilityContext.Trace.Interfaces;
using UltraBank.WebApi.Controllers.ArchiveContext.Sendloads;

namespace UltraBank.WebApi.Controllers.ArchiveContext;

[Route("api/v1/baas/archives")]
[ApiController]
public sealed class ArchiveController : CustomizedControllerBase
{
    public ArchiveController(ITraceManager traceManager, IMetricManager metricManager) : base(traceManager, metricManager)
    {
    }

    [HttpPost]
    [Route("upload")]
    [Authorize(Roles = $"{nameof(Whitelabel)}")]
    public Task<IActionResult> HttpPostUploadArchiveAsync(
        [FromServices] IUseCase<SubmitArchiveUseCaseInput, SubmitArchiveUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromForm] IFormFile archive,
        [FromForm] TypeArchive archiveType,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        var clientId = new Guid(HttpContext.User.FindFirst("ClientId")!.Value);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(ArchiveController)}.{nameof(HttpPostUploadArchiveAsync)}",
            activityKind: ActivityKind.Server,
            input: (archive, archiveType),
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: SubmitArchiveUseCaseInput.Factory(
                        archive: await ArchiveValueObject.Factory(
                            type: archiveType,
                            archive: archive),
                        whitelabelId: clientId),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status201Created,
                    value: UploadArchiveSendloadOutput.Factory(
                        archive: useCaseResult.Output.Archive));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: [
                KeyValuePair.Create(
                    key: TraceTitles.TraceClientIdWhiteLabel,
                    value: clientId.ToString())
                ]);
    }
}
