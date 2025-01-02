using System.Text.Json.Serialization;

namespace HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

public class ResourceUsage : ValueObject
{
    
    /// <summary>
    /// In bytes
    /// </summary>
    public long UsedMemoryByte { get; private set; }
    
    /// <summary>
    /// In milliseconds
    /// </summary>
    public long UsedTimeMs { get; private set; }
    
    public ResourceUsage()
    {
    }

    [JsonConstructor]
    public ResourceUsage(long usedMemoryByte, long usedTimeMs)
    {
        UsedMemoryByte = usedMemoryByte;
        UsedTimeMs = usedTimeMs;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UsedMemoryByte;
        yield return UsedTimeMs;
    }

    public void Update(long usedMemoryByte, long usedTimeMs)
    {
        UsedMemoryByte = Math.Max(usedMemoryByte, UsedMemoryByte);
        UsedTimeMs = Math.Max(usedTimeMs, UsedTimeMs);
    }
}