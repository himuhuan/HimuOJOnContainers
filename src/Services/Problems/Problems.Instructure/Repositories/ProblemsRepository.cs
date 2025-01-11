namespace HimuOJ.Services.Problems.Infrastructure.Repositories;

public class ProblemsRepository : IProblemsRepository
{
    private readonly ProblemsDbContext _context;

    public ProblemsRepository(ProblemsDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public Problem Add(Problem problem)
    {
        return _context.Problems.Add(problem).Entity;
    }

    public void Update(Problem problem)
    {
        _context.Entry(problem).State = EntityState.Modified;
    }

    public async Task<Problem> GetAsync(int problemId)
    {
        var problem = await _context.Problems.FindAsync(problemId);
        if (problem != null)
        {
            await _context.Entry(problem)
                .Collection(p => p.TestPoints)
                .LoadAsync();
        }

        return problem;
    }
}