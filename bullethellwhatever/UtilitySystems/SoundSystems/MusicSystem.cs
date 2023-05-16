using Microsoft.Xna.Framework.Audio;
using bullethellwhatever.MainFiles;
namespace bullethellwhatever.UtilitySystems.SoundSystems
{
    public class MusicSystem
    {
        public SoundEffectInstance? ActiveSong;
        public string ActiveSongName;
        public bool IsSongPlaying;
        public int SongTimer;
        public void SetMusic(string song, bool loop, float volume) //add fading out / in later
        {
            if (song != ActiveSongName)
            {
                ActiveSong = Main.Music[song].CreateInstance();
                ActiveSongName = song;
                ActiveSong.IsLooped = loop;
                ActiveSong.Volume = volume;

                PlayMusic();
            }
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
