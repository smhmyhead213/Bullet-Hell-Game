using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using bullethellwhatever.MainFiles;
using System.Threading.Tasks;
using FMOD.Studio;

namespace bullethellwhatever.UtilitySystems.SoundSystems
{
    public class MusicSystem
    {
        public Bank? ActiveSong;
        public string ActiveSongName;
        public bool IsSongPlaying;
        public int SongTimer;

        public void ManageMusic()
        {

        }
        public void SetMusic(string song, bool loop, float volume) //add fading out / in later
        {
            if (song != ActiveSongName)
            {
                //ActiveSong = Music[song];
                //ActiveSongName = song;

                //MediaPlayer.Volume = volume;
                //MediaPlayer.IsRepeating = loop;

                //IsSongPlaying = false;
            }
        }

        public void PlayMusic()
        {
            if (!IsSongPlaying)
            {
                //MediaPlayer.Play(ActiveSong);
                //FMODSystem.getVCA("vca:/ MyVCA", out VCA vca);
                //vca.setVolume(0.5f);
                var path = "event:/TestBossMusic";
                //FMODSystem.getEvent(path, out EventDescription evDesc);
                //evDesc.createInstance(out EventInstance evInst);
                //evInst.start();
                //evInst.release();
                IsSongPlaying = true;
            }
        }

        public void StopMusic()
        {
            if (ActiveSong is not null) //crash prevention
            {
                MediaPlayer.Stop();
                ActiveSongName = string.Empty;
                ActiveSong = null;
            }
        }
    }
}
