using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Classes
{
    public class GodotVersion
    {
        public int VersionId { get; set; }

        public string VersionName { get; set; }
        public string VersionUrl { get; set; }

        public int BitNum { get; set; }

        public bool IsMono { get; set; }
        public bool IsStable { get; set; }

        public string ChangelogUrl { get; set; }
    }

    public class GodotVersionStorage
    {
        public DateTime LastUpdateChecked { get; set; }

        public List<GodotVersion> AllVersions { get; set; }

        public Dictionary<int, string> InstalledVersions { get; set; }


        public void ReloadInstalledVersions(string manifestFile)
        {
            if (InstalledVersions == null)
                InstalledVersions = new Dictionary<int, string>();

            InstalledVersions = JsonConverter<Dictionary<int, string>>.Deserialize(manifestFile);
        }
    }
}
