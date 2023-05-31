using Bogus;
using Demo.Model.Context;
using System;

namespace Demo.Model.Seeds;

/// <summary>
/// Allows you to add a test data set for the development of the application.
/// </summary>
public static class FakeDataSeeder
{

    #region methods

    /// <summary>
    /// Fill the database for development environnement.
    /// </summary>
    /// <param name="db">The database context of the application.</param>        
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        //// Apply changes to the database
        //await db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Add a set of items to the db set.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="dbSet">The DbSet.</param>
    /// <param name="faker">The item's faker.</param>
    /// <param name="amount">The amount of items to add.</param>
    /// <param name="cancellationToken">The cancellation handler.</param>
    private static async Task<List<T>> AddAndGetGeneratedItemsAsync<T>(this DbSet<T> dbSet, Faker<T> faker, int amount, CancellationToken cancellationToken) where T : class
    {
        List<T> list = new(GenerateItems(faker, amount, cancellationToken));
        await dbSet.AddRangeAsync(list, cancellationToken);
        return list;
    }

    /// <summary>
    /// Add a set of items to the db set.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="dbSet">The DbSet.</param>
    /// <param name="faker">The item's faker.</param>
    /// <param name="amount">The amount of items to add.</param>
    /// <param name="cancellationToken">The cancellation handler.</param>
    private static async Task AddGeneratedItemsAsync<T>(this DbSet<T> dbSet, Faker<T> faker, int amount, CancellationToken cancellationToken) where T : class
    {
        await dbSet.AddRangeAsync(GenerateItems(faker, amount, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Generate a set of items.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="faker">The item's faker.</param>
    /// <param name="amount">The amount of items to add.</param>
    /// <param name="cancellationToken">The cancellation handler.</param>
    private static IEnumerable<T> GenerateItems<T>(Faker<T> faker, int amount, CancellationToken cancellationToken) where T : class
    {
        for (int row = 0; row < amount; row++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Set the row index as seed value to keep deterministict set of data.
            yield return faker.UseSeed(row).Generate();
        }
    }

    #endregion

}
