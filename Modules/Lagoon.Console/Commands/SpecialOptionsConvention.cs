using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;

namespace Lagoon.Console.Commands;

internal class SpecialOptionsConvention : IConvention
{

    public const string BATCH_COMMAND = "batch";
    public const string LOG_OPTION = "--log";
    public const string NOLOG_OPTION = "--no-log";
    public const string OUT_OPTION = "--out";

    /// <summary>
    /// Apply the convention to each commands.
    /// </summary>
    /// <param name="context">The command context.</param>
    public void Apply(ConventionContext context)
    {
        // We show log option for commands
        if (context.ModelAccessor is CommandLineApplication cmdapp && cmdapp.Parent is not null ) {
            string template;
            string description;
            // the log is activated by default for "batch" command
            if (BATCH_COMMAND.Equals(cmdapp.Name, StringComparison.OrdinalIgnoreCase))
            {
                template = NOLOG_OPTION;
                description = "Disable the trace to the default log file.";
            }
            else
            {
                template = LOG_OPTION;
                description = "Enable the trace to the default log file.";
            }
            CommandOption log = new(template, CommandOptionType.NoValue)
            {
                Description = description,
                // the convention will run on each subcommand automatically.
                // it is better to run the command on each to check for overlap
                // or already set options to avoid conflict
                Inherited = false,
            };
            context.Application.AddOption(log);
        }
        // Redirect the output for the console
        CommandOption option = new($"{OUT_OPTION} <filepath>", CommandOptionType.SingleValue)
        {
            Description = "Write the console output to the specified file.",
            // the convention will run on each subcommand automatically.
            // it is better to run the command on each to check for overlap
            // or already set options to avoid conflict
            Inherited = false,
        };
        context.Application.AddOption(option);
    }

}
