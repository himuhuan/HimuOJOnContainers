#region

using Submits.BackgroundTasks.Library;

#endregion

namespace Submits.BackgroundTasks.Services.Sandbox;

public interface ISandboxService
{
    SandboxResult Run(SandboxConfiguration configuration);
}