using System.ComponentModel.DataAnnotations;

namespace HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

/// <summary>
/// A <see cref="TestPoint"/> is a test case for a <see cref="Problem"/>.
/// </summary>
public class TestPoint : Entity
{
    public int ProblemId { get; private set; }

    public string Input { get; private set; }

    public string ExpectedOutput { get; private set; }

    public string Remarks { get; private set; }

    public TestPoint(int problemId, string input, string expectedOutput, string remarks)
    {
        ProblemId = problemId;
        Input = input;
        ExpectedOutput = expectedOutput;
        Remarks = remarks;
    }
}
