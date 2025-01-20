using System.Text.Json.Serialization;

namespace HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

public class GuestAccessLimit : ValueObject
{
    
    public bool AllowDownloadInput { get; private set;}

    public bool AllowDownloadOutput { get; private set;}
    
    protected GuestAccessLimit()
    {
    }
    
    [JsonConstructor]
    public GuestAccessLimit(bool allowDownloadInput, bool allowDownloadOutput)
    {
        AllowDownloadInput  = allowDownloadInput;
        AllowDownloadOutput = allowDownloadOutput;
    }
    
    public void Update(bool allowDownloadInput, bool allowDownloadOutput)
    {
        AllowDownloadInput  = allowDownloadInput;
        AllowDownloadOutput = allowDownloadOutput;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AllowDownloadInput;
        yield return AllowDownloadOutput;
    }
}