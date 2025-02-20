using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.DrawCode.UI;
using bullethellwhatever.DrawCode;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.UtilitySystems.SoundSystems;
using Microsoft.Xna.Framework.Media;

namespace bullethellwhatever.MainFiles
{
    public static class UpdateLoopManager
    {
        public static bool Frozen;
        public static bool FreezeNextFrame;
        public static void Update()
        {
            SoundSystem.Update();

            GameTime++;

            UpdateInputSystem();

            AssetRegistry.Update();

            // to do: move this to a new camera Update method if something else needs updated as well
            MainCamera.UpdateVisibleArea();

            if (MainInstance.IsActive && !Frozen)
            {
                // call me Odie the way i bark at that garfeild

                GameStateHandler.HandleGame();
                UIManager.ManageUI();

                if (musicSystem.ActiveSong is not null)
                    musicSystem.PlayMusic();
            }
            else
            {
                if (musicSystem.ActiveSong is not null)
                    MediaPlayer.Pause();
            }

            DialogueSystem.Update();

            Drawing.UpdateDrawer();

            if (FreezeNextFrame)
            {
                Frozen = true;
                FreezeNextFrame = false;
            }

            if (IsKeyPressedAndWasntLastFrame(Microsoft.Xna.Framework.Input.Keys.P))
            {
                Frozen = !Frozen;
            }

            if (IsKeyPressedAndWasntLastFrame(Microsoft.Xna.Framework.Input.Keys.O))
            {
                Frozen = false;
                FreezeNextFrame = true;
            }
        }
    }
}
