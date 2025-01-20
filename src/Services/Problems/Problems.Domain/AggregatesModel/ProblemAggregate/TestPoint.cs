using System.Text.Json.Serialization;

namespace HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

/// <summary>
///     A <see cref="TestPoint" /> is a test case for a <see cref="Problem" />.
/// </summary>
public class TestPoint : Entity
{
    public TestPoint(
        int id,
        int problemId,
        string input,
        string expectedOutput,
        string remarks)
    {
        Id             = id;
        ProblemId      = problemId;
        Input          = input;
        ExpectedOutput = expectedOutput;
        Remarks        = remarks;
    }

    public int ProblemId { get; private set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TestPointResourceType ResourceType { get; init; } = TestPointResourceType.Text;

    /// <summary>
    ///    The input of the test point.
    /// </summary>
    /// <remarks>
    ///    When <see cref="ResourceType" /> is <see cref="TestPointResourceType.File" />,
    ///    this property is the relative path of the input file.
    /// </remarks>
    public string Input { get; private set; }

    /// <summary>
    ///    The expected output of the test point.
    /// </summary>
    /// <remarks>
    ///   When <see cref="ResourceType" /> is <see cref="TestPointResourceType.File" />,
    ///   this property is the relative path of the expected output file.
    /// </remarks>
    public string ExpectedOutput { get; private set; }

    public string Remarks { get; private set; }

    public DateTime LastModifyTime { get; private set; } = DateTime.UtcNow;

    public void Update(string input, string expectedOutput, string remarks)
    {
        Input          = input;
        ExpectedOutput = expectedOutput;
        Remarks        = remarks;
        LastModifyTime = DateTime.UtcNow;
    }
}