using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace GodotLauncher.Services
{
    public static class DownloadManagerService
    {

        public static string DownloadFileSync(string url)
        {
            string savedPath;

            savedPath = GetFileNameFromUrl(url);

            return DownloadFileSync(url, savedPath);
        }

        public static string DownloadFileSync(string url, string savedPath, bool isFolder = false)
        {
            if(isFolder)
                savedPath += $"\\{GetFileNameFromUrl(url)}";

            using (var client = new WebClient())
            {
                client.DownloadFile(url, savedPath);
            }

            return savedPath;
        }

        public static string GetFileNameFromUrl(string url)
        {
            return url.Substring(url.LastIndexOf('/') + 1);
        }
    }
}
