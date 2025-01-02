namespace HimuOJ.Services.Submits.API.Application.Objects;

public record CreateSubmitRequest(int ProblemId, string SourceCode, string CompilerName);