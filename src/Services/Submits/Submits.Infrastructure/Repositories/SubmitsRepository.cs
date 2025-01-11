#region

using Microsoft.EntityFrameworkCore.Infrastructure;

#endregion

namespace HimuOJ.Services.Submits.Infrastructure.Repositories;

public class SubmitsRepository : ISubmitsRepository
{
    private readonly SubmitsDbContext _context;

    public SubmitsRepository(SubmitsDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public DatabaseFacade Database => _context.Database;

    public Submission Add(Submission entity)
    {
        return _context.Submissions.Add(entity).Entity;
    }

    public void Update(Submission entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task<Submission> GetAsync(int id)
    {
        Submission entity = await _context.Submissions.FindAsync(id);
        if (entity != null)
        {
            await _context.Entry(entity)
                .Collection(p => p.TestPointResults)
                .LoadAsync();
        }

        return entity;
    }
}