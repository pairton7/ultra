using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UltraBank.AlertContext;
using UltraBank.AlertContext.Middleware;
using UltraBank.Application;
using UltraBank.EnvironmentContext;
using UltraBank.Infrascructure;
using UltraBank.Infrascructure.Data;
using UltraBank.ObservabilityContext;
using UltraBank.WebApi.Exceptions;

namespace UltraBank.WebApi;

public partial class Program
{
    protected Program() => Console.WriteLine("Program.cs created.");

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region Environment Configuration

        const string environmentAppsettingsKey = "Environment";

        builder.Services.ApplyEnvironmentDependenciesConfiguration(
            environment: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[environmentAppsettingsKey], environmentAppsettingsKey));

        #endregion

        #region Alert Middleware Configuration

        const string applicationEnvironmentAppsettingsKey = "ApplicationEnvironment";
        const string discordWebhookAppsettingsKey = "Dependencies:Infrascructure:Alerts:DiscordWebhookUrl";
        const string discordBusinessWebhookAppsettingsKey = "Dependencies:Infrascructure:Alerts:DiscordBusinessWebhookUrl";
        const string correlationIdLinkAppsettingsKey = "Dependencies:Infrascructure:Alerts:CorrelationIdLink";

        builder.Services.ApplyAlertDependenciesConfiguration(
            environment: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[applicationEnvironmentAppsettingsKey], applicationEnvironmentAppsettingsKey),
            discordWebhookUrl: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[discordWebhookAppsettingsKey], discordWebhookAppsettingsKey),
            correlationIdLink: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[correlationIdLinkAppsettingsKey], correlationIdLinkAppsettingsKey));

        #endregion

        #region Observability Dependencies Configuration

        const string applicationNameAppsettingsKey = "ApplicationName";
        const string applicationIdAppsettingsKey = "ApplicationId";
        const string applicationVersionAppsettingsKey = "ApplicationVersion";
        const string applicationNamespaceAppsettingsKey = "ApplicationNamespace";
        const string openTelemetryGrpcEndpointAppsettingsKey = "Dependencies:Infrascructure:Observability:OpenTelemetry:Endpoint";

        builder.Services.ApplyObservabilityDependenciesConfiguration(
            applicationName: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[applicationNameAppsettingsKey], applicationNameAppsettingsKey),
            applicationVersion: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[applicationVersionAppsettingsKey], applicationVersionAppsettingsKey),
            applicationId: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[applicationIdAppsettingsKey], applicationIdAppsettingsKey),
            applicationNamespace: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[applicationNamespaceAppsettingsKey], applicationNamespaceAppsettingsKey),
            openTelemetryGrpcEndpoint: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[openTelemetryGrpcEndpointAppsettingsKey], openTelemetryGrpcEndpointAppsettingsKey));

        #endregion

        #region Authentication with JwtBearer Configuration

        const string bearerJwtPrivateKeyAppsettingsKey = "ApplicationBearerPrivateKey";
        const string authenticationExpiresInSeconds = "ApplicationAuthenticationSeconds";

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(
                    key: Encoding.UTF8.GetBytes(ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[bearerJwtPrivateKeyAppsettingsKey], bearerJwtPrivateKeyAppsettingsKey))),
            };
        });

        #endregion

        #region Infrascructure Dependencies Configuration

        const string connectionStringAppsettingsKey = "Dependencies:Infrascructure:Data:MySqlConnectionString";

        Console.WriteLine(builder.Configuration[connectionStringAppsettingsKey]);

        builder.Services.ApplyInfrascructureDependenciesConfiguration(
            connectionString: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[connectionStringAppsettingsKey], connectionStringAppsettingsKey));

        #endregion

        #region Application Dependencies Configuration

        const string environmentArchivesDirectoryAppsettingsKey = "Dependencies:Infrascructure:Environment:Directory";
        const string dellBankBaseUriAppsettingsKey = "Dependencies:Application:DellBank:BaseUri";
        const string dellBankAuthorizationAppsettingsKey = "Dependencies:Application:DellBank:AuthorizationToken";

        const string retryMaxNumberAccountServiceAppsettingsKey = "Dependencies:Application:Internal:AccountService:MaxRetryNumber";
        const string intervalBetweenRetryAccountServiceAppsettingsKey = "Dependencies:Application:Internal:AccountService:IntervalBetweenRetry";

        const string retryMaxNumberPixServiceUpdateChargeDynamicPixAppsettingsKey = "Dependencies:Application:Internal:PixService:DynamicPixChargeWebhook:MaxRetryNumber";
        const string intervalBetweenRetryPixServiceUpdateChargeDynamicPixAppsettingsKey = "Dependencies:Application:Internal:PixService:DynamicPixChargeWebhook:IntervalBetweenRetry";

        const string retryMaxNumberTransferServiceUpdateOutTransferWebhookEventAppsettingsKey = "Dependencies:Application:Internal:TransferService:OutTransferWebhookEvent:MaxRetryNumber";
        const string intervalBetweenRetryTransferServiceUpdateOutTransferWebhookEventAppsettingsKey = "Dependencies:Application:Internal:TransferService:OutTransferWebhookEvent:IntervalBetweenRetry";

        const string pixPaymentUpdatedWebhookUrlAppsettingsKey = "Dependencies:Application:DellBank:PixPaymentUpdatedWebhookUrl";
        const string pixPaymentReceivedWebhookUrlAppsettingsKey = "Dependencies:Application:DellBank:PixReceivedWebhookUrl";

        const string certificateDellBankPassword = "Dependencies:Application:DellBank:CertificateIntegrationPassword";
        const string certificateDellBankArchiveName = "Dependencies:Application:DellBank:CertificateArchiveName";

        builder.Services.ApplyApplicationDependenciesConfiguration(
            directory: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[environmentArchivesDirectoryAppsettingsKey], environmentArchivesDirectoryAppsettingsKey),
            dellBankBaseUri: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[dellBankBaseUriAppsettingsKey], dellBankBaseUriAppsettingsKey),
            dellBankAuthorization: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[dellBankAuthorizationAppsettingsKey], dellBankAuthorizationAppsettingsKey),
            bearerJwtPrivateKey: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[bearerJwtPrivateKeyAppsettingsKey], bearerJwtPrivateKeyAppsettingsKey),
            maxRetryWebhookAccountUpdate: Convert.ToInt32(ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[retryMaxNumberAccountServiceAppsettingsKey], retryMaxNumberAccountServiceAppsettingsKey)),
            intervalBetweenRetriesAccountUpdate: Convert.ToInt32(ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[intervalBetweenRetryAccountServiceAppsettingsKey], intervalBetweenRetryAccountServiceAppsettingsKey)),
            expiresInSecondsAuthentication: Convert.ToInt32(ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[authenticationExpiresInSeconds], authenticationExpiresInSeconds)),
            maxRetryWebhookDynamicChargePixUpdate: Convert.ToInt32(ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[retryMaxNumberPixServiceUpdateChargeDynamicPixAppsettingsKey], retryMaxNumberPixServiceUpdateChargeDynamicPixAppsettingsKey)),
            intervalBetweenRetriesDynamicChargePixUpdate: Convert.ToInt32(ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[intervalBetweenRetryPixServiceUpdateChargeDynamicPixAppsettingsKey], intervalBetweenRetryPixServiceUpdateChargeDynamicPixAppsettingsKey)),
            environmentDescription: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[applicationEnvironmentAppsettingsKey], applicationEnvironmentAppsettingsKey),
            businessAlertWebhook: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[discordBusinessWebhookAppsettingsKey], discordBusinessWebhookAppsettingsKey),
            maxRetryWebhookOutTransferUpdateEvent: Convert.ToInt32(ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[retryMaxNumberTransferServiceUpdateOutTransferWebhookEventAppsettingsKey], retryMaxNumberTransferServiceUpdateOutTransferWebhookEventAppsettingsKey)),
            intervalBetweenRetriesOutTransferUpdateEvent: Convert.ToInt32(ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[intervalBetweenRetryTransferServiceUpdateOutTransferWebhookEventAppsettingsKey], intervalBetweenRetryTransferServiceUpdateOutTransferWebhookEventAppsettingsKey)),
            pixPaymentReceivedWebhookUrl: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[pixPaymentReceivedWebhookUrlAppsettingsKey], pixPaymentReceivedWebhookUrlAppsettingsKey),
            pixPaymentUpdatedWebhookUrl: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[pixPaymentUpdatedWebhookUrlAppsettingsKey], pixPaymentUpdatedWebhookUrlAppsettingsKey),
            certificateDellBankPassword: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[certificateDellBankPassword], certificateDellBankPassword),
            certificateDellBankArchiveName: ProgramSettingsException.ThrowExceptionIfTheResourceIsNullOrWhiteSpace(builder.Configuration[certificateDellBankArchiveName], certificateDellBankArchiveName));

        #endregion

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<AlertErrorHandlingMiddleware>();
        app.MapControllers();
        //app.Services.GetRequiredService<DataContext>().Database.Migrate();
        app.Run();
    }
}
