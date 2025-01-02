using System.Text.Json.Serialization;

namespace HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

public class OutputDifference : ValueObject
{
    public string ExpectedOutput { get; private set; }
    
    public string ActualOutput { get; private set; }
    
    public int Position { get; set; }
    
    [JsonConstructor]
    public OutputDifference(string expectedOutput, string actualOutput, int position)
    {
        ExpectedOutput = expectedOutput;
        ActualOutput = actualOutput;
        Position = position;
    }
    
    protected OutputDifference()
    {
        
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ExpectedOutput;
        yield return ActualOutput;
    }
}