using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HimuOJ.Services.Submits.Infrastructure.Repositories;

public interface ISubmitsRepository : IRepository<Submission, int>
{
    public DatabaseFacade Database { get; }
}