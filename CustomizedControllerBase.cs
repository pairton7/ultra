using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UltraBank.Domain.BoundedContexts.AccountContext.ENUMs;
using UltraBank.ObservabilityContext.Metric.Interfaces;
using UltraBank.ObservabilityContext.Trace.Interfaces;

namespace UltraBank.WebApi.Controllers;

public abstract class CustomizedControllerBase : ControllerBase
{
    protected readonly ITraceManager _traceManager;
    protected readonly IMetricManager _metricManager;
    protected readonly string _providerDefault;
    protected CustomizedControllerBase(
        ITraceManager traceManager, 
        IMetricManager metricManager)
    {
        _providerDefault = TypeBaasAccountProvider.DELLBANK.ToString();
        _traceManager = traceManager;
        _metricManager = metricManager;
    }

    public string ScopeInvalid = "Não autorizada. A autenticação utilizada não possui escopo válido para efetuar a operação.";

    public bool HasScopeOnAuthentication(
        string scopes, string specifiedScope)
    {
        var scopesOnRequest = scopes.Split(' ');

        var hasSpecifiedScope = false;

        foreach (var scope in scopesOnRequest)
        {
            if (scope == specifiedScope)
            {
                hasSpecifiedScope = true;
                break;
            }
        }

        return hasSpecifiedScope;
    }
}
