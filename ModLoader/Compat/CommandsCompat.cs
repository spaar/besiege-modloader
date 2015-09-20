using System.Collections.Generic;
using System.Reflection;

namespace spaar
{
  /// <summary>
  /// Delegate to use with registering commands.
  /// </summary>
  /// <param name="args">The unnamed arguments passed on the command line, in the order they appear</param>
  /// <param name="namedArgs">The named arguments passed on the command line</param>
  /// <returns>Result of the command, will be printed to the console, if not null or empty string</returns>
  public delegate string CommandCallback(string[] args, IDictionary<string, string> namedArgs);

  /// <summary>
  /// Commands is used for all things regarding commands. Mainly, you can register your own commands and help messages here.
  /// </summary>
  /// <remarks>
  /// Commands can be entered in the console by the user. The command can be entered by itself if it is only registered once,
  /// if multiple mods registered the same command they are differentiated by entering <![CDATA[<modname>:<command>]]>.
  /// Commands can take two types of arguments: named arguments and unnamed arguments. Unnamed arguments are simply values entered
  /// on the console after the command, seperated by spaces. Named arguments follow this format: <c>--name value</c>.
  /// </remarks>
  /// <example>
  /// <code>
  ///   Commands.RegisterCommand("myCommand", myCallback);
  /// </code>
  /// Registers a new command called myCommand. This can then be used like this:
  /// <code>
  ///   myCommand arg1 "second unnamed arg" --namedArg1 value1 --namedArg2 "second value"
  /// </code>
  /// If called as above, <c>args</c> passed to the callback will contain <c>["arg1", "second unnamed arg"]</c> and namedArgs
  /// will contain the following mappings: <c>"namedArg1" -> "value1", "namedArg2" -> "second value"</c>.
  /// </example>
  public static class Commands
  {
    /// <summary>
    /// Register a console command. The passed callback will be called when the user enters the command in the console.
    /// </summary>
    /// <param name="command">The command to register</param>
    /// <param name="callback">The callback to be called when the command is entered</param>
    /// <param name="helpText">Help text for this command. Used for <c><![CDATA[help <mod> <command>]]></c></param>
    /// <returns>True if registration succeeded, false otherwise</returns>
    public static bool RegisterCommand(string command, CommandCallback callback, string helpText = "")
    {
      return spaar.ModLoader.Commands.RegisterCommand(command, (a, n) => { return callback(a, n); }, helpText);
    }

    /// <summary>
    /// Register a help message for your mod to be displayed with the 'help' command.
    /// This should give a general overview, to give specific information about a command,
    /// use the <c>helpText</c> parameter of <c>RegisterCommand</c>.
    /// </summary>
    /// <param name="message">The help message</param>
    public static void RegisterHelpMessage(string message)
    {
      spaar.ModLoader.Commands.RegisterHelpMessage(Assembly.GetCallingAssembly(), message);
    }
  }
}