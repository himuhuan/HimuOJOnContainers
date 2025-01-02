namespace HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

public enum JudgeStatus
{
    Unknown = 0,
    Pending = 1,
    
    /// <remarks>
    /// Currently, we never persist this status to the database.
    /// </remarks>
    Running = 2,
    
    Accepted = 3,
    WrongAnswer = 4,
    TimeLimitExceeded = 5,
    MemoryLimitExceeded = 6,
    RuntimeError = 7,
    CompilationError = 8,
    SystemError = 9,
    NotSupported = 10,
    
    /// <summary>
    /// Only used in TestPointResult, which means the test point is skipped since the previous test point failed,
    /// or waiting for the previous test point to finish.
    /// </summary>
    PendingOrSkipped = 11,
}