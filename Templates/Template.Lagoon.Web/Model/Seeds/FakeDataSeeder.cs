//-:cnd:noEmit
#if DEBUG
//+:cnd:noEmit

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter

using Bogus;

namespace TemplateLagoonWeb.Model.Seeds;

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
        /* EXAMPLE :
        // Fill the "Foo" DbSet
        if (!db.Foos.Any())
        {
            // Use "refDate" to have a deterministic dates
            DateTime refDate = new(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            // Foos
            int FooId = 1;
            Faker<Foo> FooFaker = new Faker<Foo>()
                .RuleFor(x => x.Id, f => FooId++)
                .RuleFor(x => x.Name, f => f.Address.Foo());
            List<Foo> Foos = await db.Foos.AddAndGetGeneratedItemsAsync(FooFaker, 20, cancellationToken);
            // Companies
            Faker<Firm> firmFaker = new Faker<Firm>()
                .RuleFor(x => x.Id, f => f.Random.Uuid())
                .RuleFor(x => x.FooId, f => f.PickRandom(Foos).Id)
                .RuleFor(x => x.Name, f => f.Company.CompanyName());
            List<Firm> firms = await db.Firms.AddAndGetGeneratedItemsAsync(firmFaker, 100, cancellationToken);
            // Contacts
            Faker<Contact> contactFaker = new Faker<Contact>()
                .RuleFor(x => x.Id, f => f.Random.Uuid())
                .RuleFor(x => x.BirthDate, f => f.Date.Past(120, refDate).OrNull(f, 0.2f)) 
                .RuleFor(x => x.Comment, f => f.Lorem.Lines(3).OrDefault(f, 0.2f))
                .RuleFor(x => x.FirmId, f => f.PickRandom(firms).Id)
                .RuleFor(x => x.FirstName, f => f.Person.FirstName)
                .RuleFor(x => x.Gender, f => f.PickRandom<Gender>())
                .RuleFor(x => x.LastName, f => f.Person.LastName)
                .RuleFor(x => x.Mail, f => f.Internet.Email().OrNull(f, 0.3f))
                .RuleFor(x => x.HairColor, f => f.PickRandom<HairColor>().OrNull(f, 0.2f))
                .RuleFor(x => x.HasGlasses, f => f.Random.Bool().OrNull(f, 0.3f))
                .RuleFor(x => x.Weight, f => f.Random.Int(40, 110).OrNull(f, 0.2f));
            await db.Contacts.AddGeneratedItemsAsync(contactFaker, 1000, cancellationToken);
            // Apply changes to the database
            await db.SaveChangesAsync(cancellationToken);
        }
        */
        await Task.CompletedTask;
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
//-:cnd:noEmit
#endif
//+:cnd:noEmit
