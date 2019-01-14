using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GodotLauncher.DataClasses;
using GodotLauncher.Services;

namespace GodotLauncher.Services
{
    public class GodotVersionService
    {
        public List<GodotVersion> AllVersions { get; set; }

        public List<GodotVersionInstalled> InstalledVersions { get; set; }
    }
}
