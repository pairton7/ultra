namespace UltraBank.WebApi.Exceptions;

public sealed class ProgramSettingsException : Exception
{
    public ProgramSettingsException(string message) : base(message)
    {
    }

    public static string ThrowExceptionIfTheResourceIsNullOrWhiteSpace(string? resource, string appsettingsKey)
        => string.IsNullOrWhiteSpace(resource) || string.IsNullOrWhiteSpace(appsettingsKey)
        ? throw new ProgramSettingsException($"O recurso a ser obtido no appsettings.json ({appsettingsKey}) não foi configurado.") 
        : resource;
}
