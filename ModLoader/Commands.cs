using System;
using System.Collections.Generic;
using System.Reflection;
using spaar.ModLoader.Internal;
using UnityEngine;

namespace spaar.ModLoader
{
  /// <summary>
  /// Delegate to use with registering commands.
  /// </summary>
  /// <param name="args">The unnamed arguments passed on the command line,
  /// in the order they appear.</param>
  /// <param name="namedArgs">The named arguments passed on the command line.</param>
  /// <returns>Result of the command, will be printed to the console,
  /// if not null or empty string.</returns>
  public delegate string CommandCallback(string[] args,
    IDictionary<string, string> namedArgs);

  /// <summary>
  /// Commands is used for all things regarding commands.
  /// Mainly, you can register your own commands and help messages here.
  /// </summary>
  /// <remarks>
  /// Commands can be entered in the console by the user.
  /// The command can be entered by itself if it is only registered once,
  /// if multiple mods registered the same command they are differentiated
  /// by entering <![CDATA[<modname>:<command>]]>.
  /// Commands can take two types of arguments: named arguments and unnamed
  /// arguments. Unnamed arguments are simply values entered on the console
  /// after the command, seperated by spaces.
  /// Named arguments follow this format: <c>--name value</c>.
  /// </remarks>
  /// <example>
  /// <code>
  ///   Commands.RegisterCommand("myCommand", MyCallback);
  /// </code>
  /// An example implementation of the callback might looks like this:
  /// <code>
  /// private string MyCallback(string[] args,
  /// IDictionary&lt;string, string&gt; namedArgs)
  /// {
  ///   if (args.Length != 2)
  ///   {
  ///     return "Usage: myCommand &lt;param1&gt; &lt;param2&gt;";
  ///   }
  ///   return "You passed " + args[0] + " and " + args[1];
  /// }
  /// </code>
  /// Of course it is also possible to implement the callback as a lambda or
  /// anonymous delegate instead of a full function:
  /// <code>
  ///   Commands.RegisterCommand("myCommand", (args, namedArgs) =>
  ///   {
  ///     if (args.Length != 2)
  ///     {
  ///       return "Usage: myCommand &lt;param1&gt; &lt;param2&gt;";
  ///     }
  ///     return "You passed " + args[0] + " and " + args[1];
  ///   });
  /// </code>
  /// Registers a new command called myCommand. This can then be used like this:
  /// <code>
  ///   myCommand arg1 "second unnamed arg" --namedArg1 value1 --namedArg2 "second value"
  /// </code>
  /// If called as above, <c>args</c> passed to the callback will contain
  /// <c>["arg1", "second unnamed arg"]</c>
  /// NamedArgs will contain the following mappings:
  /// <c>"namedArg1" -> "value1", "namedArg2" -> "second value"</c>.
  /// </example>
  public static class Commands
  {
    // Simple struct for representing all information needed about a command
    private class Command
    {
      public InternalMod mod;
      public CommandCallback callback;
      public string helpMessage;
    }

    // Map commands the user can enter to all Command's registered with that name.
    // Mapping to multiple Commands is necessary so that multiple mods can
    // register the same command.
    private static Dictionary<string, List<Command>> commands
      = new Dictionary<string, List<Command>>(
        StringComparer.InvariantCultureIgnoreCase);

    private static Dictionary<InternalMod, string> helpMessages
      = new Dictionary<InternalMod, string>();

    // Necessary state for auto-completion
    private static List<string> completedCommands = new List<string>();

    private static List<string> history = new List<string>();
    /// <summary>
    /// List of all executed commands.
    /// This returns a read-only list.
    /// </summary>
    public static IList<string> History { get { return history.AsReadOnly(); } }

    /// <summary>
    /// Registers the general built-in commands.
    /// Specifically help and version.
    /// </summary>
    internal static void Initialize()
    {
      initHelp();
      RegisterCommand("version", (args, namedArgs) =>
        { return string.Format("spaar's Mod Loader version {0}, Besiege {1}",
          Internal.ModLoader.ModLoaderVersion, Internal.ModLoader.BesiegeVersion); });
      RegisterCommand("list", (args, namedArgs) =>
      {
        string output = "";
        foreach (var key in commands.Keys)
        {
          output += key + "\n";
        }
        return output + "\n" + "To get help type help <command>";
      }, "Returns a list of all available commands.");
    }

    /// <summary>
    /// Registers the built-in help command.
    /// </summary>
    private static void initHelp()
    {
      RegisterCommand("help", (args, namedArgs) =>
      {
        if (args.Length == 0)
        {
          return @"List of built-in commands: 
setMessageFilter - Filter console messages by type
setConfigValue - Sets a configuration value
listMods - List all loaded mods, along with their author and version
clear - Clears the Console
version - Prints the current version
list - Prints every command available
help <modname> - Prints help information about the specified mod, if available
help <modname> <command> or help <modname>:<command> or help <command> - Prints help information about the specified command, if available
help - Prints this help message";
        }
        else if (args.Length == 1 && !args[0].Contains(":"))
        {
          var name = args[0].ToLower();
          if (Internal.ModLoader.Instance.LoadedMods
                  .Exists(m => m.Mod.Name.ToLower() == name))
          {
            // help <modname>
            var mod = Internal.ModLoader.Instance.LoadedMods
              .Find(m => m.Mod.Name.ToLower() == name);
            if (helpMessages.ContainsKey(mod))
            {
              return helpMessages[mod];
            }
            else
            {
              return "No help for " + args[0] + " could be found.";
            }
          }
          else if (commands.ContainsKey(args[0]))
          {
            // help <command>
            String output = "";
            foreach (var coms in commands[args[0]])
            {
              output += "[" + coms.mod.Mod.Name + "] " + coms.helpMessage + "\n";
            }
            return output;
          }
          else
          {
            return "No mod or command named " + args[0] + " could be found.";
          }
        }
        else if (args.Length == 2 || args[0].Contains(":"))
        {
          // help <mod> <command> || help <mod>:<command>
          var modName = "";
          var commandName = "";
          if (args[0].Contains(":"))
          {
            modName = args[0].Split(':')[0];
            commandName = args[0].Split(':')[1];
          }
          else
          {
            modName = args[0];
            commandName = args[1];
          }
          if (!commands.ContainsKey(commandName))
          {
            return "No such command: " + commandName;
          }
          var coms = commands[commandName];
          if (!coms.Exists(com => com.mod.Mod.Name.ToLower() == modName.ToLower()))
          {
            return "No command " + commandName + " in mod " + modName;
          }
          var helpMessage = coms.Find(com => com.mod.Mod.Name.ToLower()
            == modName.ToLower()).helpMessage;
          if (helpMessage == "")
          {
            return "No help registered for " + modName + ":" + commandName;
          }
          return helpMessage;
        }
        return "Usage: 'help' or 'help <mod>' or 'help <mod> <command>' or 'help <mod>:<command>'";
      });
    }

    /// <summary>
    /// Register a console command.
    /// The passed callback will be called when the user enters the command in
    /// the console.
    /// If the same command was previously registered by the same mod,
    /// the old one will be updated instead of a new one being registered.
    /// </summary>
    /// <param name="command">The command to register</param>
    /// <param name="callback">The callback to be called when the command is entered</param>
    /// <param name="helpText">Help text for this command.
    ///   Used for <c><![CDATA[help <mod> <command>]]></c></param>
    /// <returns>True if registration succeeded, false otherwise</returns>
    public static bool RegisterCommand(string command, CommandCallback callback,
      string helpText = "")
    {
      var com = new Command();
      com.callback = callback;
      com.helpMessage = helpText;
      var callingAssemblyName = Assembly.GetCallingAssembly().FullName;
      com.mod = Internal.ModLoader.Instance.LoadedMods
        .Find(mod => mod.AssemblyName == callingAssemblyName);
      if (com.mod == null)
      {
        Debug.LogError("Could not identify mod trying to register command "
          + command + "!");
        return false;
      }
      if (commands.ContainsKey(command))
      {
        var oldCommand = commands[command].Find(c => c.mod == com.mod);
        if (oldCommand != default(Command))
        {
          // Command was previously registered by same mod.
          // Update old one instead of registering new one.
          oldCommand.callback = callback;
          oldCommand.helpMessage = helpText;
        }
        else
        {
          commands[command].Add(com);
        }
      }
      else
      {
        var newList = new List<Command>();
        newList.Add(com);
        commands.Add(command, newList);
      }
      return true;
    }

    /// <summary>
    /// Register a help message for your mod to be displayed with the 'help' command.
    /// This should give a general overview, to give specific information about
    /// a command, use the <c>helpText</c> parameter of <c>RegisterCommand</c>.
    /// </summary>
    /// <param name="message">The help message</param>
    public static void RegisterHelpMessage(string message)
    {
      var callingAssembly = Assembly.GetCallingAssembly().FullName;
      var mod = Internal.ModLoader.Instance.LoadedMods.Find(
        m => m.AssemblyName == callingAssembly);

      helpMessages[mod] = message;
    }

    internal static void RegisterHelpMessage(Assembly callingAssembly, string message)
    {
      // For CommandsCompat
      var mod = Internal.ModLoader.Instance.LoadedMods.Find(
        m => m.AssemblyName == callingAssembly.FullName);

      helpMessages[mod] = message;
    }

    /// <summary>
    /// Parse the next argument from parts, starting at i.
    /// This respects quotes, so that arguments can contain spaces.
    /// </summary>
    /// <param name="parts">The whole argument string, split by spaces</param>
    /// <param name="i">The index where the argument to get starts</param>
    /// <param name="newI">The first index after the argument</param>
    /// <returns>The parsed argument</returns>
    private static string GetArgument(string[] parts, int i, out int newI)
    {
      if (parts[i].StartsWith("\"", StringComparison.Ordinal))
      {
        var currentArg = parts[i].Substring(1);
        if (parts[i].EndsWith("\"", StringComparison.Ordinal))
        {
          currentArg = currentArg.Substring(0, currentArg.Length - 1);
        }
        else
        {
          i++;
          while (!parts[i].EndsWith("\"", StringComparison.Ordinal))
          {
            currentArg += " " + parts[i];
            i++;
          }
          currentArg += " " + parts[i].Substring(0, parts[i].Length - 1);
        }
        newI = i;
        return currentArg;
      }
      else
      {
        newI = i;
        return parts[i];
      }
    }

    /// <summary>
    /// Handle an entered command.
    /// Arguments are parsed and then the callback corresponding to the command
    /// is called.
    /// </summary>
    /// <param name="input">Complete command line</param>
    public static void HandleCommand(string input)
    {
      history.Add(input);

      var result = "";

      // Input parsing
      var parts = input.Split(' ');
      var command = "";

      if (parts[0].Contains(":"))
        command = parts[0].Split(':')[1];
      else
        command = parts[0];

      var args = new List<string>();
      var namedArgs = new Dictionary<string, string>();
      for (int i = 1; i < parts.Length; i++)
      {
        if (parts[i].StartsWith("--", StringComparison.Ordinal))
        {
          // Named arg
          var name = parts[i].Substring(2);
          var value = "";
          i++;

          if (parts.Length > i)
          {
            value = GetArgument(parts, i, out i);
            namedArgs.Add(name, value);
          }
          else
          {
            // No value passed, use empty string
            namedArgs.Add(name, "");
          }
        }
        else
        {
          // Unnamed arg
          args.Add(GetArgument(parts, i, out i));
        }
      }

      Debug.Log("> " + input);

      if (commands.ContainsKey(command))
      {
        if (commands[command].Count > 1)
        {
          if (!parts[0].Contains(":"))
          {
            result = "Error: Multiple mods have registered " + command
              + ", use <modname>:" + command + " to specify which one to use.\n"
              + "Mods that provide command " + command + ": ";
            foreach (var c in commands[command])
              result += "\n" + c.mod.Mod.Name;
          }
          else
          {
            var modname = parts[0].Split(':')[0];
            result = commands[command].Find(
              c => c.mod.Mod.Name.Equals(
                modname, StringComparison.CurrentCultureIgnoreCase))
              .callback(args.ToArray(), namedArgs);
          }
        }
        else
        {
          result = commands[command][0].callback(args.ToArray(), namedArgs);
        }
      }
      else
      {
        result = "No such command: " + command;
      }

      if (result != null && result != "")
        Debug.Log("=> " + result);
    }

    /// <summary>
    /// Checks whether a command currently exists (is registered).
    /// </summary>
    /// <param name="command">The command to check</param>
    /// <returns>Whether the specifed command exists</returns>
    public static bool DoesCommandExist(string command)
    {
      return commands.ContainsKey(command);
    }

    internal static string AutoCompleteNext(string toComplete)
    {
      var commandList = new List<string>(commands.Keys);
      var matchingCommands = commandList.FindAll(
        c => c.StartsWith(toComplete, StringComparison.InvariantCultureIgnoreCase));

      if (matchingCommands.Count == 0)
        return toComplete;

      foreach (var command in matchingCommands)
      {
        if (!completedCommands.Contains(command))
        {
          completedCommands.Add(command);
          return command;
        }
      }

      // Matching commands were found, however they were all suggested already.
      // Start again at the beginning.
      AutoCompleteReset();
      return AutoCompleteNext(toComplete);
    }

    internal static void AutoCompleteReset()
    {
      completedCommands.Clear();
    }
  }
}