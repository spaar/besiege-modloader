using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace spaar.ModLoader.Installer
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      // The installer can be launched without GUI by directly specifying all necessary arguments.
      // This is mostly used by the installer itself to attempt an install with elevated privileges.
      var args = Environment.GetCommandLineArgs();
      if (args.Length != 1)
      {
        for (int i = 0; i < args.Length; i++)
        {
          args[i] = args[i].Replace("'", "");
        }

        var path = args[1];

        var version = new ModLoaderVersion();
        version.Name = args[2];
        version.NormalDownload = args[3];
        version.DeveloperDownload = args[3];

        var dev = bool.Parse(args[4]);

        try
        {
          Installer.InstallModLoader(path, version, dev);
        }
        catch (Exception ex) when (ex is SecurityException || ex is UnauthorizedAccessException)
        {
          Environment.Exit(-2);
        }
        catch (Exception)
        {
          Environment.Exit(-3);
        }

        Environment.Exit(0);
      }
      else
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        var form = new FormLoadingScreen();
        form.Show();
        Application.Run();
      }
    }
  }
}
