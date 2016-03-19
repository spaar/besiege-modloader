using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace spaar.ModLoader.Installer
{
  public class Installer
  {
    public static void InstallModLoader(string besiegeLocation,
      ModLoaderVersion version, bool developer)
    {
      UninstallModLoader(besiegeLocation);

      var besiegeDir = new DirectoryInfo(besiegeLocation);
      besiegeDir.CreateSubdirectory("Installer");

      // Download and extract ZIP archive

      string url = developer ? version.DeveloperDownload
                             : version.NormalDownload;
      string zipFile = besiegeDir.FullName + "/Installer/modloader-"
        + version.Name + ".zip";
      string extractDir = besiegeDir.FullName + "/Installer/extracted";

      var client = new WebClient();
      client.Headers["user-agent"] = "Mozilla / 5.0(Windows NT 6.1; WOW64; rv: 40.0) Gecko / 20100101 Firefox / 40.1";
      client.DownloadFile(new Uri(url), zipFile);


      if (Directory.Exists(extractDir))
      {
        Directory.Delete(extractDir, true);
      }

      ZipFile.ExtractToDirectory(zipFile, extractDir);

      // Replace Assembly-UnityScript.dll, keeping the original around
      File.Move(besiegeDir + "/Besiege_Data/Managed/Assembly-UnityScript.dll",
        besiegeDir + "/Besiege_Data/Managed/Assembly-UnityScript.dll.original");
      File.Copy(extractDir + "/Assembly-UnityScript.dll",
        besiegeDir + "/Besiege_Data/Managed/Assembly-UnityScript.dll");

      // Replace Assembly-CSharp-firstpass.dll, if it exists in the archive
      if (File.Exists(extractDir + "/Assembly-CSharp-firstpass.dll"))
      {
        File.Move(besiegeDir + "/Besiege_Data/Managed/Assembly-CSharp-firstpass.dll",
          besiegeDir + "/Besiege_Data/Managed/Assembly-CSharp-firstpass.dll.original");
        File.Copy(extractDir + "/Assembly-CSharp-firstpass.dll",
          besiegeDir + "/Besiege_Data/Managed/Assembly-CSharp-firstpass.dll");
      }

      // Create Mods directory and copy SpaarModLoader.dll
      Directory.CreateDirectory(besiegeDir + "/Besiege_Data/Mods/");
      File.Copy(extractDir + "/SpaarModLoader.dll",
        besiegeDir + "/Besiege_Data/Mods/SpaarModLoader.dll");

      if (Directory.Exists(extractDir + "/Resources/"))
      {
        Directory.CreateDirectory(
          besiegeDir + "/Besiege_Data/Mods/Resources/ModLoader/");
        Directory.Delete(
          besiegeDir + "/Besiege_Data/Mods/Resources/ModLoader/", true);

        CopyFilesRecursively(
          new DirectoryInfo(extractDir + "/Resources/"),
          new DirectoryInfo(besiegeDir + "/Besiege_Data/Mods/Resources/"));
      }
    }

    public static void UninstallModLoader(string besiegeLocation)
    {
      var besiegeDir = new DirectoryInfo(besiegeLocation);

      if (File.Exists(
        besiegeDir + "/Besiege_Data/Managed/Assembly-UnityScript.dll.original"))
      {
        // Restore original Assembly-UnityScript.dll
        File.Delete(besiegeDir + "/Besiege_Data/Managed/Assembly-UnityScript.dll");
        File.Move(
          besiegeDir + "/Besiege_Data/Managed/Assembly-UnityScript.dll.original",
          besiegeDir + "/Besiege_Data/Managed/Assembly-UnityScript.dll");

        // Delete SpaarModLoader.dll
        File.Delete(besiegeDir + "/Besiege_Data/Mods/SpaarModLoader.dll");

        if (Directory.Exists(
          besiegeDir + "/Besiege_Data/Mods/Resources/ModLoader/"))
        {
          Directory.Delete(
            besiegeDir + "/Besiege_Data/Mods/Resources/ModLoader/", true);
        }
      }
    }

    // Somehow not a thing in the .NET framework
    private static void CopyFilesRecursively(DirectoryInfo source,
      DirectoryInfo target)
    {
      foreach (DirectoryInfo dir in source.GetDirectories())
        CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
      foreach (FileInfo file in source.GetFiles())
        file.CopyTo(Path.Combine(target.FullName, file.Name));
    }
  }
}
