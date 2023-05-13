using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using bullethellwhatever.MainFiles;

namespace bullethellwhatever.UtilitySystems.SoundSystems
{
    public class MusicSystem
    {
        public SoundEffectInstance? ActiveSong;
        public bool IsSongPlaying;
        public int SongTimer;
        public void SetMusic(SoundEffectInstance song, bool loop, float volume) //add fading out / in later
        {
            ActiveSong = song;
            ActiveSong.IsLooped = loop;
            ActiveSong.Volume = volume;
            
            PlayMusic();
        }
        public void PlayMusic()
        {
            if (!(ActiveSong.State == SoundState.Playing))
            {
                ActiveSong.Play();
            }
        }
    }
}
