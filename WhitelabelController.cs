using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Mime;
using UltraBank.Application.UseCases.Interfaces;
using UltraBank.Application.UseCases.WhitelabelContext.CreateWhitelabel.Inputs;
using UltraBank.Application.UseCases.WhitelabelContext.CreateWhitelabel.Outputs;
using UltraBank.Application.UseCases.WhitelabelContext.OAuthWhitelabelAuthentication.Inputs;
using UltraBank.Application.UseCases.WhitelabelContext.OAuthWhitelabelAuthentication.Outputs;
using UltraBank.AuditableInfoContext;
using UltraBank.ObservabilityContext.Metric.Interfaces;
using UltraBank.ObservabilityContext.Trace.Interfaces;
using UltraBank.WebApi.Controllers.WhitelabelContext.Payloads;
using UltraBank.WebApi.Controllers.WhitelabelContext.Sendloads;

namespace UltraBank.WebApi.Controllers.WhitelabelContext;

[Route("api/v1/baas/whitelabel")]
[ApiController]
public class WhitelabelController : CustomizedControllerBase
{
    public WhitelabelController(ITraceManager traceManager, IMetricManager metricManager) : base(traceManager, metricManager)
    {
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("create")]
    [AllowAnonymous]
    public Task<IActionResult> HttpPostCreateWhitelabelAsync(
        [FromServices] IUseCase<CreateWhitelabelUseCaseInput, CreateWhitelabelUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromBody] CreateWhitelabelPayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
                traceName: $"{nameof(WhitelabelController)}.{nameof(HttpPostCreateWhitelabelAsync)}",
                activityKind: ActivityKind.Server,
                input: input,
                handler: async (input, auditableInfo, activity, cancellationToken) =>
                {
                    var useCaseResult = await useCase.ExecuteUseCaseAsync(
                        input: CreateWhitelabelUseCaseInput.Factory(
                            comercialName: input.ComercialName,
                            email: input.Email),
                        auditableInfo: auditableInfo,
                        cancellationToken: cancellationToken);

                    if (useCaseResult.IsError)
                        return StatusCode(
                            statusCode: StatusCodes.Status400BadRequest,
                            value: useCaseResult.Notifications);

                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status201Created,
                        value: CreateWhitelabelSendloadOutput.Factory(
                            clientId: useCaseResult.Output.ClientId,
                            clientSecret: useCaseResult.Output.ClientSecret,
                            comercialName: useCaseResult.Output.ComercialName,
                            email: useCaseResult.Output.Email,
                            createdAt: useCaseResult.Output.CreatedAt,
                            notifications: useCaseResult.Notifications ?? []));
                },
                auditableInfo: auditableInfo,
                cancellationToken: cancellationToken);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("oauth/token")]
    [AllowAnonymous]
    public Task<IActionResult> HttpPostOAuthWhitelabelAuthenticationAsync(
        [FromServices] IUseCase<OAuthWhitelabelAuthenticationUseCaseInput, OAuthWhitelabelAuthenticationUseCaseOutput> useCase,
        [Required][FromHeader(Name = AuditableInfoValueObject.HeaderCorrelationIdKey)] Guid correlationId,
        [FromHeader(Name = AuditableInfoValueObject.HeaderSourcePlatformKey)] string? sourcePlatform,
        [FromHeader(Name = AuditableInfoValueObject.HeaderDeveloperResponsibleKey)] string? developerResponsible,
        [FromHeader(Name = "X-Client-Id")] Guid clientId,
        [FromHeader(Name = "X-Client-Secret")] Guid clientSecret,
        [FromBody] OAuthWhitelabelAuthenticationPayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId,
            developerResponsible: developerResponsible,
            sourcePlatform: sourcePlatform);

        return _traceManager.ExecuteTraceAsync(
                traceName: $"{nameof(WhitelabelController)}.{nameof(HttpPostCreateWhitelabelAsync)}",
                activityKind: ActivityKind.Server,
                input: input,
                handler: async (input, auditableInfo, activity, cancellationToken) =>
                {
                    var useCaseResult = await useCase.ExecuteUseCaseAsync(
                        input: OAuthWhitelabelAuthenticationUseCaseInput.Factory(
                            clientId: clientId,
                            clientSecret: clientSecret,
                            grantType: input.GrantType),
                        auditableInfo: auditableInfo,
                        cancellationToken: cancellationToken);

                    if (useCaseResult.IsError)
                        return StatusCode(
                            statusCode: StatusCodes.Status401Unauthorized,
                            value: useCaseResult.Notifications);

                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status201Created,
                        value: OAuthWhitelabelAuthenticationSendloadOutput.Factory(
                            accessToken: useCaseResult.Output.AccessToken,
                            tokenType: useCaseResult.Output.TokenType.GetTokenType(),
                            expiresIn: useCaseResult.Output.ExpiresIn,
                            grantType: useCaseResult.Output.GrantType.GetGrantType()));
                },
                auditableInfo: auditableInfo,
                cancellationToken: cancellationToken);
    }
}
