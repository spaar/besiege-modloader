using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaar.ModLoader.Installer
{
  public class ModLoaderVersion
  {
    public string Name { get; set; }
    public string ID { get; set; }

    public string NormalDownload { get; set; }
    public string DeveloperDownload { get; set; }

    public override string ToString()
    {
      return Name;
    }

  }
}
