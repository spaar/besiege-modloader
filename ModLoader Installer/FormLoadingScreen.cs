using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

using SimpleJSON;

namespace spaar.ModLoader.Installer
{
  public partial class FormLoadingScreen : Form
  {
    public FormLoadingScreen()
    {
      InitializeComponent();
    }

    private void FormLoadingScreen_Load(object sender, EventArgs e)
    {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
      var client = new WebClient();
      client.DownloadStringCompleted += DownloadComplete;
      // GitHub does not return correct results if no user agent is set
      client.Headers["user-agent"] =
        "Mozilla / 5.0(Windows NT 6.1; WOW64; rv: 40.0) Gecko / 20100101 Firefox / 40.1";
      try
      {
        client.DownloadStringAsync(
          new Uri("https://api.github.com/repos/spaar/besiege-modloader/releases"));
      }
      catch (WebException)
      {
        MessageBox.Show(
          "Error downloading version information. Check your internet " +
          "connection and make sure the installer is allowed to access " +
          "the internet.");
        Application.Exit();
      }
    }

    private void DownloadComplete(object sender, DownloadStringCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        MessageBox.Show("Error downloading version information. Check your " +
          "internet connection and make sure the installer is allowed to " +
          "access the internet.");
        Application.Exit();
      }

      var versions = new List<ModLoaderVersion>();
      try
      {
        var releases = JSON.Parse(e.Result);

        for (int i = 0; i < releases.AsArray.Count; i++)
        {
          var version = new ModLoaderVersion();
          version.Name = releases[i]["tag_name"];
          version.ID = releases[i]["id"];

          string[] downloads =
          {
            releases[i]["assets"][0]["browser_download_url"],
            releases[i]["assets"][1]["browser_download_url"]
          };

          if (downloads[0].Contains("Developer"))
          {
            version.DeveloperDownload = downloads[0];
            version.NormalDownload = downloads[1];
          }
          else
          {
            version.DeveloperDownload = downloads[1];
            version.NormalDownload = downloads[0];
          }

          versions.Add(version);
        }

      }
      catch (Exception)
      {
        MessageBox.Show("Error parsing version information. Exiting.");
        Application.Exit();
      }

      var form = new FormInstaller();
      form.SetVersionList(versions);
      form.Show();

      Close();
    }
  }
}
