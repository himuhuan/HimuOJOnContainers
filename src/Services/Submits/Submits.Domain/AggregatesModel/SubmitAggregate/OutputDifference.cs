#region

using System.Text.Json.Serialization;

#endregion

namespace HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

public class OutputDifference : ValueObject
{
    [JsonConstructor]
    public OutputDifference(string expectedOutput, string actualOutput, int position)
    {
        ExpectedOutput = expectedOutput;
        ActualOutput   = actualOutput;
        Position       = position;
    }

    protected OutputDifference()
    {
    }

    public string ExpectedOutput { get; }

    public string ActualOutput { get; }

    public int Position { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ExpectedOutput;
        yield return ActualOutput;
    }
}