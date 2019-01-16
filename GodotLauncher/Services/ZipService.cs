using Ionic.Zip;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Services
{
    public static class ZipService
    {
        public static IEnumerable<string> UnzipFile(string fileName, string targetPath)
        {
            var list = new List<string>();

            using(var zip = ZipFile.Read(fileName))
            {
                Directory.CreateDirectory(targetPath);

                foreach(var entry in zip)
                {
                    entry.Extract(targetPath, ExtractExistingFileAction.OverwriteSilently);

                    if(entry.FileName.EndsWith(".exe"))
                        list.Add($"{targetPath}\\{entry.FileName}");
                }
            }

            return list.Count == 0 ? null : list;
        }
    }
}
