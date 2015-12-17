using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Mono.Cecil;
using SimpleJSON;

namespace DebugHelper
{
  public partial class Form1 : Form
  {
    private int count = 0;

    private string modsFolder, baseModFileName;
    
    public Form1()
    {
      InitializeComponent();
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
      modsFolder = (new FileInfo(txtBesiegeLocation.Text).Directory)
        + "\\Besiege_Data\\Mods\\";
      baseModFileName = new FileInfo(txtModLocation.Text).Name.Replace(".dll", "");

      if (txtModLocation.Text != (modsFolder + baseModFileName + ".dll"))
      {
        File.Copy(txtModLocation.Text, modsFolder + baseModFileName + ".dll",
          true);
      }

      Process.Start(txtBesiegeLocation.Text, "-popupwindow -enable-debug-server");

      count = 1;
    }

    private void btnBrowseBesiege_Click(object sender, EventArgs e)
    {
      var dialog = new OpenFileDialog();
      dialog.Filter = "Besiege|Besiege.exe";
      dialog.ShowDialog();
      txtBesiegeLocation.Text = dialog.FileName;
    }

    private void btnBrowseMod_Click(object sender, EventArgs e)
    {
      var dialog = new OpenFileDialog();
      dialog.Filter = "Mod|*.dll";
      dialog.ShowDialog();
      txtModLocation.Text = dialog.FileName;
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
      for (; count > 0; count--)
      {
        var path = modsFolder + baseModFileName + "-" + count + ".dll";
        if (File.Exists(path)) File.Delete(path);
      }
      if (File.Exists(modsFolder + baseModFileName + ".dll"))
        File.Delete(modsFolder + baseModFileName + ".dll");

      var configFile = JSON.Parse(
        File.ReadAllText(modsFolder + "Config/modLoader.json"));

      for (int i = 0; i < configFile.Count; i++)
      {
        var item = configFile[i];
        if (item["key"].Value.StartsWith("modStatus:" + txtModName.Text))
        {
          configFile.Remove(i);
          i--;
        }
      }

      File.WriteAllText(modsFolder + "Config/modLoader.json",
        configFile.ToJSON(0));
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      var besiegeLocation = Environment.ExpandEnvironmentVariables("%BESIEGE_LOCATION%");
      if (besiegeLocation != "")
      {
        txtBesiegeLocation.Text = besiegeLocation + "Besiege.exe";
      }

      var args = Environment.GetCommandLineArgs();

      if (args.Length > 1)
      {
        txtModLocation.Text = args[1];
      }
      if (args.Length > 2)
      {
        txtModName.Text = args[2];
      }
    }

    private void btnReload_Click(object sender, EventArgs e)
    {
      if (count == 0) return;

      var newModPath = modsFolder + baseModFileName + "-" + count + ".dll";
      File.Copy(txtModLocation.Text, newModPath);

      // Rewrite 'Name' and assembly version of new file
      var modDefinition = AssemblyDefinition.ReadAssembly(newModPath);
      var ver = modDefinition.Name.Version;
      var newVersion = new Version(ver.Major, ver.Minor, ver.Build, count);
      modDefinition.Name.Version = newVersion;
      modDefinition.Write(newModPath);

      // Connect to mod loader, disable old mod, load new mod, enable new mod
      var disableMessage = Encoding.UTF8.GetBytes(
        "disableMod " +
        (count == 1 ? txtModName.Text : txtModName.Text + "-" + (count-1)) + "\n");
      var loadMessage = Encoding.UTF8.GetBytes(
        "loadMod " + newModPath + " " + txtModName.Text + "-" + count + "\n");
      var enableMessage = Encoding.UTF8.GetBytes(
        "enableMod " + txtModName.Text + "-" + count + "\n");

      var client = new TcpClient();
      client.Connect("localhost", 5000);
      client.GetStream().Write(disableMessage, 0, disableMessage.Length);
      Thread.Sleep(200);
      client.GetStream().Write(loadMessage, 0, loadMessage.Length);
      Thread.Sleep(200);
      client.GetStream().Write(enableMessage, 0, enableMessage.Length);
      Thread.Sleep(200);
      client.Close();

      count++;
    }
  }
}
