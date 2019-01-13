using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace GodotLauncher.Classes
{
    public static class DownloadManager
    {

        public static void DownloadFileSync(string url)
        {
            string savedPath;

            savedPath = GetFileNameFromUrl(url);

            DownloadFileSync(url, savedPath);
        }

        public static void DownloadFileSync(string url, string savedPath)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, savedPath);
            }
        }

        public static string GetFileNameFromUrl(string url)
        {
            return url.Substring(url.LastIndexOf('/') + 1);
        }
    }
}
