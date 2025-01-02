using HimuOJ.Common.WebHostDefaults.Extensions;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;
using HimuOJ.Services.Problems.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace HimuOJ.Services.Problems.API.Infrastructure;

class ProblemsDbContextSeeder : IDbContextSeeder<ProblemsDbContext>
{
    public async Task SeedAsync(ProblemsDbContext context, IServiceProvider serviceProvider)
    {
        using (context)
        {
            if (!await context.Problems.AnyAsync())
            {
                var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                var logger = serviceProvider.GetRequiredService<ILogger<ProblemsDbContextSeeder>>();
                await context.Problems.AddRangeAsync(GetSampleProblems(env.ContentRootPath, logger));
            }

            await context.SaveChangesAsync();
        }
    }

    private IEnumerable<Problem> GetSampleProblems(
        string rootPath,
        ILogger<ProblemsDbContextSeeder> logger
    )
    {
        string samplePath = Path.Combine(rootPath, "Setup", "Problems");
        var xmlFiles = Directory.GetFiles(samplePath, "*.xml");

        logger.LogInformation("Adding sample problems (count: {SampleCount}", xmlFiles.Length);

        List<Problem> problems = [];
        XmlSerializer serializer = new(typeof(ProblemXmlObject));
        var xamlObjs = xmlFiles.Select(file =>
        {
            logger.LogInformation("Parsing data from {SampleFileName}...", file);
            return ParseProblemXmlObject(file, serializer);
        });
        foreach (var xmlObj in xamlObjs)
        {
            Problem problem = new(Guid.Empty, xmlObj.Title, xmlObj.Content,
                                  new(xmlObj.MaxMemoryLimitByte, xmlObj.MaxExecuteTimeLimit),
                                  new(xmlObj.AllowDownloadInput, xmlObj.AllowDownloadAnswer));

            foreach (var testPoint in xmlObj.TestPoints)
            {
                problem.AddTestPoint(testPoint.Input, testPoint.Expected, testPoint.Comment);
            }

            problems.Add(problem);
        }

        return problems;
    }

    private ProblemXmlObject ParseProblemXmlObject(string filePath, XmlSerializer serializer)
    {
        ProblemXmlObject? problem;
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            problem = (ProblemXmlObject?)serializer.Deserialize(stream);
        }

        if (problem == null)
            throw new InvalidDataException("Invalid xml file");
        problem.Content = RemoveIndent(problem.Content);
        foreach (var element in problem.TestPoints)
        {
            element.Expected = RemoveIndent(element.Expected);
            element.Input = RemoveIndent(element.Input);
        }

        return problem;
    }

    static string RemoveIndent(string s)
    {
        // NOT effective but easy way to remove indent
        // Anyway, these code only run once
        return string.Join('\n', s.Split('\n').Select(l => l.TrimStart()).Where(t => t.Length > 0));
    }
}

#nullable disable

public class TestPointXmlObject
{
    [XmlElement("AsSample")]
    public bool AsSample { get; set; }

    [XmlElement("Comment")]
    public string Comment { get; set; }

    [XmlElement("Input")]
    public string Input { get; set; }

    [XmlElement("Expected")]
    public string Expected { get; set; }
}

[XmlRoot("Problem")]
public class ProblemXmlObject
{
    [XmlElement("Title")]
    public string Title { get; set; }

    [XmlElement("MaxMemoryLimitByte")]
    public int MaxMemoryLimitByte { get; set; }

    [XmlElement("MaxExecuteTimeLimit")]
    public int MaxExecuteTimeLimit { get; set; }

    [XmlElement("AllowDownloadInput")]
    public bool AllowDownloadInput { get; set; }

    [XmlElement("AllowDownloadAnswer")]
    public bool AllowDownloadAnswer { get; set; }

    [XmlElement("Content")]
    public string Content { get; set; }

    [XmlArray("TestPoints")]
    [XmlArrayItem("TestPoint")]
    public List<TestPointXmlObject> TestPoints { get; set; }
}
