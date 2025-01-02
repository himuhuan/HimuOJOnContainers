using System.ComponentModel.DataAnnotations;

namespace HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

/// <summary>
/// The <see cref="Problem"/> is the basic unit for users to submit test.
/// </summary>
public class Problem : Entity, IAggregateRoot
{
    public Guid? DistributorId { get; private set; }

    [Required]
    public string Title { get; private set; }

    /// <summary>
    /// Content should be MarkDown format.
    /// </summary>
    [Required]
    public string Content { get; private set; }

    public DateTime CreateTime { get; private set; }

    public DateTime LastModifyTime { get; private set; }

    /// <summary>
    /// The default resource limit for each test point in this problem.
    /// </summary>
    [Required]
    public ResourceLimit DefaultResourceLimit { get; private set; }

    [Required]
    public GuestAccessLimit GuestAccessLimit { get; private set; }

    private readonly List<TestPoint> _testPoints;

    public IReadOnlyCollection<TestPoint> TestPoints => _testPoints.AsReadOnly();

    protected Problem()
    {
        _testPoints = new List<TestPoint>();
    }

    public Problem(
        Guid? distributorId,
        string title,
        string content,
        ResourceLimit resourceLimit,
        GuestAccessLimit guestAccessLimit)
        : this()
    {
        DistributorId        = distributorId;
        Title                = title;
        Content              = content;
        CreateTime           = DateTime.UtcNow;
        LastModifyTime       = DateTime.UtcNow;
        DefaultResourceLimit = resourceLimit;
        GuestAccessLimit     = guestAccessLimit;
    }

    public void Update(string title, string content, ResourceLimit resourceLimit, GuestAccessLimit guestAccessLimit)
    {
        Title                = title;
        Content              = content;
        LastModifyTime       = DateTime.UtcNow;
        DefaultResourceLimit = resourceLimit;
        GuestAccessLimit     = guestAccessLimit;
    }

    public void AddTestPoint(string input, string expectedOutput, string remarks)
    {
        _testPoints.Add(new TestPoint(Id, input, expectedOutput, remarks));
    }
    
}