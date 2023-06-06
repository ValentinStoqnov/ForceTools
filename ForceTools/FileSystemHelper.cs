using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceTools
{
    public static class FileSystemHelper
    {
        public static string DataFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Database";
        public static string TempFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Temp";

        public static void CheckAndCreateDatabaseFolder()
        {
            if (!Directory.Exists(FileSystemHelper.DataFolderPath))
            {
                Directory.CreateDirectory(FileSystemHelper.DataFolderPath);
            }
        }

        public static void CheckAndCreateTempFolder()
        {
            if (!Directory.Exists(FileSystemHelper.TempFolderPath))
            {
                Directory.CreateDirectory(FileSystemHelper.TempFolderPath);
            }
        }
    }
}
