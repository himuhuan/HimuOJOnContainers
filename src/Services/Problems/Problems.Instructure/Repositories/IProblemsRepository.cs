namespace HimuOJ.Services.Problems.Infrastructure.Repositories;

public interface IProblemsRepository : IRepository<Problem, int>
{
    /// <summary>
    /// Removes specified test points from a problem.
    /// </summary>
    /// <param name="problemId">The ID of the problem.</param>
    /// <param name="testPointIds">The IDs of the test points to remove.</param>
    /// <returns>The number of test points removed.</returns>
    /// <remarks>
    /// If the test point does not belong to the problem, it will be ignored.
    /// </remarks>
    Task<int> RemoveTestPoints(int problemId, IEnumerable<int> testPointIds);

    /// <summary>
    /// Deletes a problem asynchronously by its ID.
    /// </summary>
    /// <param name="id">The ID of the problem to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous delete operation.
    /// The task result contains a boolean indicating whether the delete operation was successful.
    /// </returns>
    Task<bool> DeleteAsync(int id);
}