namespace HimuOJ.Common.WebHostDefaults.Extensions;

public static class UtilityExtensions
{
    /// <summary>
    /// Computes the intersection of two sequences and outputs the difference.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
    /// <param name="first">The first sequence to compare.</param>
    /// <param name="second">The second sequence to compare.</param>
    /// <param name="difference">The elements that are in the first sequence but not in the second.</param>
    /// <returns>A sequence that contains the elements that form the intersection of the two sequences.</returns>
    public static IEnumerable<T> IntersectWithDifference<T>(
        this IEnumerable<T> first,
        IEnumerable<T> second,
        out IEnumerable<T> difference)
    {
        var firstSet  = new HashSet<T>(first);
        var secondSet = new HashSet<T>(second);

        var intersection = firstSet.Intersect(secondSet);
        difference   = firstSet.Except(secondSet);

        return intersection;
    }
}