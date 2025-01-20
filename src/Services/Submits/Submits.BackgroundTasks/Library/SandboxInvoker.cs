#region

using System.Runtime.InteropServices;

#endregion

namespace Submits.BackgroundTasks.Library;

public static partial class SandboxInvoker
{
    /// <summary>
    ///     P/Invoke to call
    ///     <code>
    /// int StartSandbox(const SandboxConfiguration *config, SandboxResult *result);
    /// </code>
    /// </summary>
    /// <remarks>
    ///     See "SandboxRunner/SandboxRunnerCore/Sandbox.h" for more details.
    /// </remarks>
    [LibraryImport("Library/libsandbox.so")]
    private static partial int StartSandbox(
        ref InternalSandboxConfiguration configuration,
        ref SandboxResult result);

    [LibraryImport("Library/libsandbox.so")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static partial bool IsSandboxConfigurationVaild(
        ref InternalSandboxConfiguration configuration);

    public static SandboxResult RunSandbox(SandboxConfiguration configuration)
    {
        var internalConfiguration = new InternalSandboxConfiguration
        {
            TaskName         = Marshal.StringToHGlobalAnsi(configuration.TaskName),
            UserCommand      = Marshal.StringToHGlobalAnsi(configuration.Command),
            WorkingDirectory = Marshal.StringToHGlobalAnsi(configuration.WorkingDirectory),

            // EnvironmentVariables is not used in the current implementation
            EnvironmentVariables      = IntPtr.Zero,
            EnvironmentVariablesCount = 0,

            InputFile        = Marshal.StringToHGlobalAnsi(configuration.InputFile),
            OutputFile       = Marshal.StringToHGlobalAnsi(configuration.OutputFile),
            ErrorFile        = Marshal.StringToHGlobalAnsi(configuration.ErrorFile),
            LogFile          = Marshal.StringToHGlobalAnsi(configuration.LogFile),
            MaxMemoryToCrash = configuration.MaxMemoryToCrash,
            MaxMemory        = configuration.MaxMemory,
            MaxStack         = configuration.MaxStack,
            MaxCpuTime       = configuration.MaxCpuTime,
            MaxRealTime      = configuration.MaxRealTime,
            MaxOutputSize    = configuration.MaxOutputSize,
            MaxProcessCount  = configuration.MaxProcessCount,
            Policy           = configuration.Policy
        };

        if (!IsSandboxConfigurationVaild(ref internalConfiguration))
        {
            throw new ArgumentException("Invalid sandbox configuration");
        }

        var result = new SandboxResult();
        _ = StartSandbox(ref internalConfiguration, ref result);

        Marshal.FreeHGlobal(internalConfiguration.TaskName);
        Marshal.FreeHGlobal(internalConfiguration.UserCommand);
        Marshal.FreeHGlobal(internalConfiguration.WorkingDirectory);
        Marshal.FreeHGlobal(internalConfiguration.EnvironmentVariables);
        Marshal.FreeHGlobal(internalConfiguration.InputFile);
        Marshal.FreeHGlobal(internalConfiguration.OutputFile);
        Marshal.FreeHGlobal(internalConfiguration.ErrorFile);
        Marshal.FreeHGlobal(internalConfiguration.LogFile);

        return result;
    }

    public static bool CheckConfigurationValid(SandboxConfiguration configuration)
    {
        var internalConfiguration = new InternalSandboxConfiguration
        {
            TaskName         = Marshal.StringToHGlobalAnsi(configuration.TaskName),
            UserCommand      = Marshal.StringToHGlobalAnsi(configuration.Command),
            WorkingDirectory = Marshal.StringToHGlobalAnsi(configuration.WorkingDirectory),

            // EnvironmentVariables is not used in the current implementation
            EnvironmentVariables      = IntPtr.Zero,
            EnvironmentVariablesCount = 0,

            InputFile        = Marshal.StringToHGlobalAnsi(configuration.InputFile),
            OutputFile       = Marshal.StringToHGlobalAnsi(configuration.OutputFile),
            ErrorFile        = Marshal.StringToHGlobalAnsi(configuration.ErrorFile),
            LogFile          = Marshal.StringToHGlobalAnsi(configuration.LogFile),
            MaxMemoryToCrash = configuration.MaxMemoryToCrash,
            MaxMemory        = configuration.MaxMemory,
            MaxStack         = configuration.MaxStack,
            MaxCpuTime       = configuration.MaxCpuTime,
            MaxRealTime      = configuration.MaxRealTime,
            MaxOutputSize    = configuration.MaxOutputSize,
            MaxProcessCount  = configuration.MaxProcessCount,
            Policy           = configuration.Policy
        };

        var result = IsSandboxConfigurationVaild(ref internalConfiguration);

        Marshal.FreeHGlobal(internalConfiguration.TaskName);
        Marshal.FreeHGlobal(internalConfiguration.UserCommand);
        Marshal.FreeHGlobal(internalConfiguration.WorkingDirectory);
        Marshal.FreeHGlobal(internalConfiguration.EnvironmentVariables);
        Marshal.FreeHGlobal(internalConfiguration.InputFile);
        Marshal.FreeHGlobal(internalConfiguration.OutputFile);
        Marshal.FreeHGlobal(internalConfiguration.ErrorFile);
        Marshal.FreeHGlobal(internalConfiguration.LogFile);

        return result;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct InternalSandboxConfiguration
    {
        public IntPtr TaskName;
        public IntPtr UserCommand;
        public IntPtr WorkingDirectory;
        public IntPtr EnvironmentVariables;
        public ushort EnvironmentVariablesCount;
        public IntPtr InputFile;
        public IntPtr OutputFile;
        public IntPtr ErrorFile;
        public IntPtr LogFile;
        public ulong MaxMemoryToCrash;
        public ulong MaxMemory;
        public ulong MaxStack;
        public ulong MaxCpuTime;
        public ulong MaxRealTime;
        public ulong MaxOutputSize;
        public int MaxProcessCount;
        public int Policy;
    }
}