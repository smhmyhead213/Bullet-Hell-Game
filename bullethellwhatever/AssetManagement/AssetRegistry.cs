using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using FMOD.Studio;
using FMOD;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.AssetManagement
{
    public static class AssetRegistry
    {
        public static Dictionary<string, ManagedTexture> Textures;
        public static Dictionary<string, ManagedShader> Shaders;
        public static Dictionary<string, ManagedBank> Banks;
        /// <summary>
        /// Dictionary containing the name of a file and its file path.
        /// Can be used as a "map" - given the name of a file, it finds the saved path to it.
        /// The key is the name of the file, and the value is the path leading to it.
        /// </summary>
        public static Dictionary<string, string> FileNameMap;
        public static int TextureUnusedTimeBeforeRemoval => 10800;
        public static void Initialise()
        {
            Textures = new Dictionary<string, ManagedTexture>();
            Shaders = new Dictionary<string, ManagedShader>();
            Banks = new Dictionary<string, ManagedBank>();

            FileNameMap = new Dictionary<string, string>();

            PopulateFileNameMap();
        }

        //public static ManagedTexture GetTexture(string fileName)
        //{
        //    string path = FileNameMap[fileName];

        //    if (Textures.ContainsKey(fileName))
        //    {
        //        return Textures[fileName];
        //    }
        //    else
        //    {
        //        Texture2D texture = MainInstance.Content.Load<Texture2D>(path);
        //        ManagedTexture managedTexture = new ManagedTexture(texture);
        //        managedTexture.TimeSinceLastUse = 0;
        //        Textures.Add(fileName, managedTexture);
        //        return new ManagedTexture(texture);
        //    }
        //}
        public static Texture2D GetTexture2D(string fileName)
        {
            string path = FileNameMap[fileName];

            if (Textures.ContainsKey(fileName))
            {
                return Textures[fileName].Asset;
            }
            else
            {
                Texture2D texture = MainInstance.Content.Load<Texture2D>(path);
                ManagedTexture managedTexture = new ManagedTexture(texture);
                managedTexture.TimeSinceLastUse = 0;
                Textures.Add(fileName, managedTexture);
                return texture;
            }
        }
        //public static ManagedShader GetShader(string fileName)
        //{
        //    string path = FileNameMap[fileName];

        //    if (Shaders.ContainsKey(fileName))
        //    {
        //        return Shaders[fileName];
        //    }
        //    else
        //    {
        //        Effect shader = MainInstance.Content.Load<Effect>(path);
        //        ManagedShader managedShader = new ManagedShader(shader);
        //        managedShader.TimeSinceLastUse = 0;
        //        Shaders.Add(fileName, managedShader);
        //        return new ManagedShader(shader);
        //    }
        //}

        public static Effect GetShader(string fileName)
        {
            string path = FileNameMap[fileName];

            if (Shaders.ContainsKey(fileName))
            {
                return Shaders[fileName].Asset;
            }
            else
            {
                Effect shader = MainInstance.Content.Load<Effect>(path);
                ManagedShader managedShader = new ManagedShader(shader);
                managedShader.TimeSinceLastUse = 0;
                Shaders.Add(fileName, managedShader);
                return shader;
            }
        }
        public static ManagedBank GetBank(string fileName)
        {
            string path = FileNameMap[fileName];

            if (Banks.ContainsKey(fileName))
            {
                return Banks[fileName];
            }
            else
            {
                Bank bank = MainInstance.Content.Load<Bank>(path); // change this to bank loading code
                ManagedBank managedBank = new ManagedBank(bank);
                managedBank.TimeSinceLastUse = 0;
                Banks.Add(fileName, managedBank);
                return new ManagedBank(bank);
            }
        }
        public static void PopulateFileNameMap()
        {
            //File.Delete("Content\\TelegraphLineShader.xnb");

            string[] files = Directory.GetFiles("Content", "", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string toSaveAs = files[i];

                while (toSaveAs.Contains("\\")) //remove all slashes
                {
                    int indexOfSlash = toSaveAs.IndexOf("\\");

                    int startIndex = indexOfSlash + 1; //+ 1 accounts for double slash, there's two slashes in the IndexOf cos the first one is to character escape the second

                    toSaveAs = toSaveAs.Substring(startIndex, toSaveAs.Length - startIndex); //only take everything after the double slash
                }

                int indexOfDot = toSaveAs.IndexOf(".");

                string fileExtension = toSaveAs.Substring(indexOfDot, toSaveAs.Length - indexOfDot); // remove file extension

                toSaveAs = toSaveAs.Substring(0, indexOfDot);

                if (fileExtension == ".xnb") //only do this for .xnbs, when this was written credits.txt would crash it as a .txt doesnt work
                {
                    toSaveAs = toSaveAs.Substring(0, indexOfDot); //remove file extension

                    //if (!FileNameMap.ContainsKey(toSaveAs))
                    FileNameMap.Add(toSaveAs, RemoveContentPrefix(RemoveFileExtenstion(files[i])));
                }
            }
        }

        public static string RemoveFileExtenstion(string path)
        {
            int indexOfDot = path.IndexOf(".");

            return path.Substring(0, indexOfDot);
        }

        public static void Update()
        {
            List<string> toRemove = new List<string>();

            foreach (KeyValuePair<string, ManagedTexture> texture in Textures)
            {
                if (texture.Value.TimeSinceLastUse > TextureUnusedTimeBeforeRemoval)
                {
                    toRemove.Add(texture.Key);
                }

                texture.Value.TimeSinceLastUse++;               
            }

            foreach (string key in toRemove)
            {
                Textures.Remove(key);
            }
        }

        public static string RemoveContentPrefix(string path)
        {
            int contentLength = MainInstance.Content.RootDirectory.Length + 1; // add one to remove /
            // if the string begins with "Content"
            if (path.Substring(0, contentLength - 1) == "Content")
            {
                // remove "Content\"
                return path.Substring(contentLength, path.Length - contentLength);
            }
            else return path;
        }
    }
}