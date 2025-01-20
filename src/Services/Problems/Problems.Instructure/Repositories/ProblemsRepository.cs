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

    public async Task<int> RemoveTestPoints(int problemId, IEnumerable<int> testPointIds)
    {
        var ids = testPointIds as int[] ?? testPointIds.ToArray();
        if (ids.Length == 0) return 0;
        return await _context.TestPoints
            .Where(tp => tp.ProblemId == problemId && ids.Contains(tp.Id))
            .ExecuteDeleteAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // TODO: use soft delete
        int count = await _context.Problems
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();
        return count > 0;
    }

    public async Task<Problem> GetProblemMinimalAsync(int id)
    {
        return await _context.Problems.FindAsync(id);
    }
}