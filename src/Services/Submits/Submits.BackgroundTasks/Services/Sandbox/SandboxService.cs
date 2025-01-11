#region

using Submits.BackgroundTasks.Library;

#endregion

namespace Submits.BackgroundTasks.Services.Sandbox;

public class SandboxService : ISandboxService
{
    private readonly ILogger<SandboxService> _logger;

    public SandboxService(ILogger<SandboxService> logger)
    {
        _logger = logger;
    }

    public SandboxResult Run(SandboxConfiguration configuration)
    {
        _logger.LogInformation("--- Run sandbox-{TaskName} with {@Config}", configuration.TaskName,
            configuration);

        var result = SandboxInvoker.RunSandbox(configuration);

        _logger.LogInformation("--- sandbox-{TaskName} finished with {@Result}",
            configuration.TaskName, result);
        return result;
    }
}