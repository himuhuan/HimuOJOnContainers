using Submits.BackgroundTasks.Library;

namespace Submits.BackgroundTasks.Services.Sandbox;

public interface ISandboxService
{ 
    SandboxResult Run(SandboxConfiguration configuration);
}