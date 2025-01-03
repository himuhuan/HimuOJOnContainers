﻿using HimuOJ.Services.Submits.API.Application.Queries;

namespace HimuOJ.Web.WebSPA.Models;

public class BffProblemListItem
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public int AcceptedSubmissionCount { get; set; }
    public int TotalSubmissionCount { get; set; }
}

public class BffProblemList
{
    public int Total { get; init; }
    public int PageCount { get; init; }
    public IEnumerable<BffProblemListItem> Items { get; init; } = [];
}

public class BffSubmissionListItem : SubmissionListItem
{
    public string? SubmitterName { get; set; }
    public string? SubmitterAvatar { get; set; }
    public string? ProblemTitle { get; set; }
}

public class BffSubmissionList
{
    public int Total { get; init; }
    public int PageCount { get; init; }
    public IEnumerable<BffSubmissionListItem> Items { get; init; } = [];
}

public class BffSubmission : GetSubmissionResult
{
    public string? SubmitterName { get; set; }
    public string? SubmitterAvatar { get; set; }
    public string? ProblemTitle { get; set; }
}