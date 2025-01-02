namespace Submits.BackgroundTasks.Library;

public class SandboxConfiguration(string taskName, string command)
{
    public string TaskName { get; init; } = taskName;
    public string Command { get; init; } = command;
    public string? WorkingDirectory { get; init; } = null;
    public string? InputFile { get; init; } = null;
    public string? OutputFile { get; init; } = null;
    public string? ErrorFile { get; init; } = null;
    public string? LogFile { get; set; }
    public ulong MaxMemoryToCrash { get; init; } = 0;
    public ulong MaxMemory { get; init; } = 0;
    public ulong MaxStack { get; init; } = 0;
    public ulong MaxCpuTime { get; init; } = 0;
    public ulong MaxRealTime { get; init; } = 0;
    public ulong MaxOutputSize { get; init; } = 0;
    public int MaxProcessCount { get; init; } = -1;
    public int Policy { get; init; } = 0;
    
    // Only used in client
    public bool OutputLogFileDetail { get; init; } = true;
}