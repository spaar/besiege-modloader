using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Microsoft.Win32;

namespace spaar.ModLoader.Installer
{
  public partial class FormInstaller : Form
  {

    private StreamWriter output;

    public FormInstaller()
    {
      InitializeComponent();
    }

    public void SetVersionList(List<ModLoaderVersion> versions)
    {
      cobVersion.Items.AddRange(versions.ToArray());
      cobVersion.SelectedIndex = 0;
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      var dialog = new FolderBrowserDialog();

      if (txtBesiegeLocation.Text != "")
      {
        dialog.SelectedPath = txtBesiegeLocation.Text;
      }

      dialog.ShowDialog();

      var path = dialog.SelectedPath;

      if (ValidatePath(path))
      {
        SetPath(path);
      }
      else
      {
        MessageBox.Show("Not a Besiege installation!", "Error",
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void SetPath(string path)
    {
      if (path != "")
      {
        var dir = new DirectoryInfo(path);
        if (dir.Name == "Besiege_Data") path = dir.Parent.FullName;
      }

      txtBesiegeLocation.Text = path;
      btnInstall.Enabled = true;
      btnUninstall.Enabled = true;
    }

    private bool ValidatePath(string path)
    {
      if (path == "") return false;
      
      var directory = new DirectoryInfo(path);
      var files = new List<FileInfo>(directory.GetFiles());
      var dirs = new List<DirectoryInfo>(directory.GetDirectories());

      if (files.Find(f => f.Name == "Besiege.exe") != null) return true;
      if (dirs.Find(d => d.Name == "Besiege_Data") != null) return true;

      if (directory.Name == "Besiege_Data") return true;

      return false;
    }

    private string FindSteamInstallation()
    {
      string keypath = @"Software\Valve\Steam";
      var key = Registry.CurrentUser.OpenSubKey(keypath);
      if (key == null) return null; // Key does not exist, Steam not installed
      output.WriteLine("Openend registry key...");
      string registeredFilePath = key.GetValue("SteamPath").ToString();
      output.WriteLine("SteamPath: " + registeredFilePath);

      var reader = new StreamReader(registeredFilePath + "/config/config.vdf");
      string config = reader.ReadToEnd();
      reader.Close();

      var baseInstallFolderPattern = "\"BaseInstallFolder_1\"\\t{2}\"(.+)\"";
      var regex = new Regex(baseInstallFolderPattern);
      var match = regex.Match(config);
      if (!match.Success) output.WriteLine("Regex didn't match!");
      if (!match.Success) return "";
      var capture = match.Groups[1];

      output.WriteLine("Library location: " + capture);

      var path = capture + "\\steamapps\\common\\Besiege";

      output.WriteLine("Raw path: " + path);

      path = path.Replace("\\\\", "\\") + "\\";

      output.WriteLine("Cleaned path: " + path);

      if (ValidatePath(path))
      {
        output.WriteLine("Path valid");
        return path;
      }
      else
      {
        output.WriteLine("Path invalid");
        return "";
      }
    }

    private void FormInstaller_FormClosed(object sender, FormClosedEventArgs e)
    {
      output.Close();
      Application.Exit();
    }

    private void FormInstaller_Load(object sender, EventArgs e)
    {
      output = new StreamWriter("./output.txt");

      // First try the directory the .exe is in as Besiege location
      if (ValidatePath(new FileInfo(Application.ExecutablePath).Directory.FullName))
      {
        SetPath(new FileInfo(Application.ExecutablePath).Directory.FullName);
      }
      else
      {
        // Try to find location via Steam config
        SetPath(FindSteamInstallation());
        if (txtBesiegeLocation.Text == "")
        {
          //Fallback to default Steam location
          if (Directory.Exists(
            @"C:\Program Files (x86)\Steam\steamapps\common\Besiege"))
          {
            if (ValidatePath(
              @"C:\Program Files (x86)\Steam\steamapps\common\Besiege"))
            {
              SetPath(@"C:\Program Files (x86)\Steam\steamapps\common\Besiege");
            }
          }
        }
      }
    }

    private void btnInstall_Click(object sender, EventArgs e)
    {
      SetPath(txtBesiegeLocation.Text);

      tsLblStatus.Text = "Downloading&Installing mod loader. This may take while...";

      Installer.InstallModLoader(txtBesiegeLocation.Text,
        (ModLoaderVersion)cobVersion.SelectedItem, cbDeveloper.Checked);

      tsLblStatus.Text = "Done";

      MessageBox.Show("Done. You can now close the installer and start Besiege.");
    }

    private void btnUninstall_Click(object sender, EventArgs e)
    {
      SetPath(txtBesiegeLocation.Text);
      Installer.UninstallModLoader(txtBesiegeLocation.Text);
    }
  }
}
