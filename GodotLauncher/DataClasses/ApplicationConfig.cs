using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.DataClasses
{
    public class ApplicationConfig
    {
        public string GodotInstallLocation { get; set; }

        public bool Show32BitVersions { get; set; }
        public bool Show64BitVersions { get; set; }
        public bool ShowMonoVersions { get; set; }
        public bool ShowUnstableVersions { get; set; }

        public bool ShowInstalledVersions { get; set; }
        public bool ShowNotInstalledVersions { get; set; }
        public bool ShowStableVersions { get; set; }
        public bool ShowStandardVersions { get; set; }

        public DateTime LastUpdateChecked { get; set; }
        public int LastSelectedVersion { get; set; }

        public int OnGodotLaunch { get; set; }

        public bool UseProxy { get; set; }
        public string ProxyUrl { get; set; }
        public int ProxyPort { get; set; }
    }
}
