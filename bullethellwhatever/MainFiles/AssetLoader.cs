using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FMOD.Studio;

namespace bullethellwhatever.MainFiles
{
    public static class AssetLoader
    {
        public static void LoadIn(string filename)
        {
            string toSaveAs = filename;

            string filePath = string.Empty;

            while (toSaveAs.Contains("\\")) //remove all slashes
            {
                int indexOfSlash = toSaveAs.IndexOf("\\");

                int startIndex = indexOfSlash + 1; //+ 1 accounts for double slash, there's two slashes in the IndexOf cos the first one is to character escape the second

                filePath = toSaveAs.Substring(0, startIndex);
                toSaveAs = toSaveAs.Substring(startIndex, toSaveAs.Length - startIndex); //only take everything after the double slash
            }

            int indexOfDot = toSaveAs.IndexOf(".");

            string fileExtension = toSaveAs.Substring(indexOfDot, toSaveAs.Length - indexOfDot); // remove file extension

            toSaveAs = toSaveAs.Substring(0, indexOfDot);

            if (fileExtension == ".xnb") //only do this for .xnbs, when this was written credits.txt would crash it as a .txt doesnt work
            {
                toSaveAs = toSaveAs.Substring(0, indexOfDot); //remove file extension

                if (toSaveAs.Contains("Shader")) // This system only works if you use a naming convention that matches this
                {
                    if (!(Shaders.ContainsKey(toSaveAs)))
                        Shaders.Add(toSaveAs, MainInstance.Content.Load<Effect>("Shaders/" + toSaveAs));
                }
                else if (toSaveAs.Contains("Music"))
                {
                    if (!(Music.ContainsKey(toSaveAs)))
                        Music.Add(toSaveAs, MainInstance.Content.Load<Bank>("Music/" + toSaveAs));
                }
                else if (toSaveAs.Contains("Sound"))
                {
                    if (!(Sounds.ContainsKey(toSaveAs)))
                        Sounds.Add(toSaveAs, MainInstance.Content.Load<SoundEffect>(toSaveAs));
                }
                else if (toSaveAs != "font")
                {
                    if (!(Assets.ContainsKey(toSaveAs)))
                        try
                        {
                            Assets.Add(toSaveAs, MainInstance.Content.Load<Texture2D>(toSaveAs));
                        }
                        catch // if we fail to load in the texture, try again with a file path this time
                        {
                            Assets.Add(toSaveAs, MainInstance.Content.Load<Texture2D>(filePath + toSaveAs));
                        }
                }
            }
        }

        public static Texture2D LoadTexture(string filename)
        {
            if (Assets.ContainsKey(filename))
                return Assets[filename];
            else
            {
                try
                {
                    Assets.Add(filename, MainInstance.Content.Load<Texture2D>(filename));
                }
                catch // if we fail to load in the texture, try again with a file path this time
                {
                    Assets.Add(filename, MainInstance.Content.Load<Texture2D>(filename));
                }

                return Assets[filename];
            }
        }

        public static Effect LoadShader(string filename)
        {
            if (Shaders.ContainsKey(filename))
                return Shaders[filename];
            else
            {
                Shaders.Add(filename, MainInstance.Content.Load<Effect>(filename));
                return Shaders[filename];
            }
        }

        public static Bank LoadMusic(string filename)
        {
            if (Music.ContainsKey(filename))
                return Music[filename];
            else
            {
                Music.Add(filename, MainInstance.Content.Load<Bank>(filename));
                return Music[filename];
            }
        }

        public static SoundEffect LoadSoundEffect(string filename)
        {
            if (Sounds.ContainsKey(filename))
                return Sounds[filename];
            else
            {
                Sounds.Add(filename, MainInstance.Content.Load<SoundEffect>(filename));
                return Sounds[filename];
            }
}
    }
}
