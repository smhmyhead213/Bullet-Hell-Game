using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.MainFiles
{
    public static class SaveSystem
    {
        public static readonly string SaveFolderName = "SaveData";
        public static readonly string SaveFileName = "Save.json";
        public static void LoadSave()
        {
            bool folderExists = Directory.Exists(SaveFolderName);
            
            if (folderExists)
            {
                bool saveFileExists = File.Exists(SaveFilePath());
            }
            else
            {
                Directory.CreateDirectory(SaveFolderName);
                CreateDefaultSave(SaveFilePath());

            }
        }

        public static string SaveFilePath()
        {
            return $"{SaveFolderName}\\{SaveFileName}";
        }
        public static void CreateDefaultSave(string filepath)
        {
            FileStream saveStream = File.Create(SaveFilePath());

        }
    }
}
