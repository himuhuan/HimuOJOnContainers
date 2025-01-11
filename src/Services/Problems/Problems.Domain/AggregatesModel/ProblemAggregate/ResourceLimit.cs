using System.Text.Json.Serialization;

namespace HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

public class ResourceLimit : ValueObject
{
    public long MaxMemoryLimitByte { get; private set; }

    public long MaxRealTimeLimitMilliseconds { get; private set; }

    public ResourceLimit()
    {
    }

    [JsonConstructor]
    public ResourceLimit(long maxMemoryLimitByte, long maxRealTimeLimitMilliseconds)
    {
        MaxMemoryLimitByte           = maxMemoryLimitByte;
        MaxRealTimeLimitMilliseconds = maxRealTimeLimitMilliseconds;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MaxMemoryLimitByte;
        yield return MaxRealTimeLimitMilliseconds;
    }
}