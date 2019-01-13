using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Classes
{
    public class ApplicationConfig
    {
        public int UpdateCheckInterval { get; set; }

        public string GodotInstallPath { get; set; }

        public DateTime LastUpdateChecked { get; set; }

        public int LastUsedVersion { get; set; }

        public bool ShowMonoVersions { get; set; }
        public bool Show32BitVersions { get; set; }
        public bool Show64BitVersions { get; set; }
        public bool ShowUnstableVersions { get; set; }
    }
}
