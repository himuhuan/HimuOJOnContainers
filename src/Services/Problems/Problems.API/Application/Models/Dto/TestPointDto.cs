using System.Text.Json.Serialization;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

namespace HimuOJ.Services.Problems.API.Application.Models.Dto;

public class TestPointDto
{
    public required int ProblemId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TestPointResourceType ResourceType { get; init; } = TestPointResourceType.Text;
}