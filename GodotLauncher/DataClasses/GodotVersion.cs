using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.DataClasses
{
    public abstract class GodotVersionBase
    {
        public int VersionId { get; set; }
        public string VersionName { get; set; }

        public int BitNum { get; set; }

        public bool IsMono { get; set; }
        public bool IsStable { get; set; }
    } 
    
    public class GodotVersion : GodotVersionBase
    {
        public string VersionUrl { get; set; }
        public string ChangelogUrl { get; set; }
    }

    public class GodotVersionInstalled: GodotVersionBase
    {
        public string InstallPath { get; set; }
    }
}
