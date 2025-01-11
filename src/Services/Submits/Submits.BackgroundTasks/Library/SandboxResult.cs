#region

using System.Runtime.InteropServices;

#endregion

namespace Submits.BackgroundTasks.Library;

[StructLayout(LayoutKind.Sequential)]
public struct SandboxResult
{
    public int Status;
    public int ExitCode;
    public int Signal;
    public ulong CpuTimeUsage;
    public ulong RealTimeUsage;
    public ulong MemoryUsage;
}

// From Sandbox.h
public enum SandboxStatus
{
    Success = 0,
    MemoryLimitExceeded,
    RuntimeError,
    CpuTimeLimitExceeded,
    RealTimeLimitExceeded,
    ProcessLimitExceeded,
    OutputLimitExceeded,
    IllegalOperation,

    InternalError = 0xFFFF
}