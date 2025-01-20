#region

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#endregion

namespace HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

public class TestPointResult : Entity
{
    protected TestPointResult()
    {
    }

    public TestPointResult(int testPointId)
    {
        TestPointId = testPointId;
        Status      = JudgeStatus.PendingOrSkipped;
    }

    [JsonConstructor]
    public TestPointResult(
        int id,
        int testPointId,
        JudgeStatus status,
        ResourceUsage usage,
        OutputDifference difference,
        int submissionId)
    {
        Id           = id;
        TestPointId  = testPointId;
        Status       = status;
        Usage        = usage;
        Difference   = difference;
        SubmissionId = submissionId;
    }

    [Required]
    public int SubmissionId { get; private set; }

    [Required]
    public int TestPointId { get; private set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public JudgeStatus Status { get; private set; }

    public ResourceUsage Usage { get; private set; }

    public OutputDifference Difference { get; private set; }

    public void UpdateStatus(JudgeStatus status)
    {
        Status = status;
    }

    public void UpdateDifference(string expectedOutput, string actualOutput, int position)
    {
        Difference = new OutputDifference(expectedOutput, actualOutput, position);
        Status     = JudgeStatus.WrongAnswer;
    }

    public void UpdateUsage(ResourceUsage usage)
    {
        Usage = usage;
    }
}