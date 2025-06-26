using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using bullethellwhatever.UtilitySystems;

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
                CreateDefaultSave(SaveFilePath());
            }
            else
            {
                Directory.CreateDirectory(SaveFolderName);
                CreateDefaultSave(SaveFilePath());
            }

            string jsonString = File.ReadAllText(SaveFilePath());
            Dictionary<string, string> readData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
            
            KeybindMap = ReadSave(readData);
        }

        public static string SaveFilePath()
        {
            return $"{SaveFolderName}\\{SaveFileName}";
        }

        public static void CreateDefaultSave(string filepath)
        {
            FileStream saveStream = File.Create(SaveFilePath());
            saveStream.Dispose();

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(KeybindMapToStrings(DefaultKeybinds()), options);

            File.WriteAllText(SaveFilePath(), jsonString);
        }

        public static void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(KeybindMapToStrings(KeybindMap), options);

            File.WriteAllText(SaveFilePath(), jsonString);
        }
    }
}
