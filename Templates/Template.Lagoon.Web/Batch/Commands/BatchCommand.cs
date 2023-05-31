namespace TemplateLagoonWeb.Batch.Commands;


/// <summary>
/// Exécute the batch actions.
/// </summary>
[Command(Name = "batch", Description = "Batch actions.")]
public class BatchCommand
{

    /* EXAMPLE

    (Uncomment [Subcommand(typeof(BatchCommand))] in "main.cs")

    /// <summary>
    /// Indicate if the batch must include the foo import.
    /// </summary>
    [Option("--import-foo", "Import the ...", CommandOptionType.NoValue)]
    public bool ImportFooEnabled { get; set; }

    /// <summary>
    /// Execute the specified batch actions.
    /// </summary>
    /// <param name="app">The main application.</param>
    /// <param name="fooService">TODO</param>
    public void OnExecute(Main app, MyFooService fooService)
    {
        // Show the application version
        app.ShowVersion();
        // First task
        if (ImportFooEnabled)
        {
            fooService.ImportFoo();
        }
        // The batch is over
        Console.WriteLine($"Batch successfuly done !");
    }

    */
}