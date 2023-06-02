using Microsoft.Xna.Framework;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.Buttons;
using bullethellwhatever.DrawCode;
using SharpDX.XAudio2;
using static bullethellwhatever.MainFiles.GameState;

namespace bullethellwhatever.MainFiles
{
    public class GameStateHandler
    {
        public bool isGameStarted;
        public int ButtonCooldown;
        public int DefaultButtonCooldown => 25;
        public string activeBoss; //use a swicth statement to spawn a boss in


        public void HandleGame()
        {
            ManageLists();

            if (ButtonCooldown > 0)
            {
                ButtonCooldown--;
            }

            switch (State)
            {
                case GameStates.TitleScreen:
                    isGameStarted = false;
                    CheckButtonClicks();
                    Main.activeButtons.RemoveAll(Button => Button.DeleteNextFrame);
                    Credits.ReadInCreditsAlready = false; // i know this is an awful way to do it, in the future make credits reset when opened
                    break;
                case GameStates.BossSelect:
                    isGameStarted = false;
                    CheckButtonClicks();
                    Main.activeButtons.RemoveAll(Button => Button.DeleteNextFrame);
                    break;
                case GameStates.DifficultySelect:
                    isGameStarted = false;
                    CheckButtonClicks();
                    Main.activeButtons.RemoveAll(Button => Button.DeleteNextFrame);
                    break;
                case GameStates.Settings:
                    isGameStarted = false;
                    CheckButtonClicks();
                    break;
                case GameStates.InGame:                    
                    break;
                case GameStates.Credits:
                    CheckButtonClicks();
                    break;
            }

            if (State == GameStates.InGame) // This needs to be checked every frame independent of the switch statement so it is done the same frame that a difficulty is selected.
            {
                if (!isGameStarted)
                {
                    EntityManager.SpawnBoss();
                    Main.player.Spawn(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 10 * 9), new Vector2(0, 0), 10, "box");
                    Main.activeProjectiles.Clear();
                    isGameStarted = true;
                }

                CheckButtonClicks();

                Main.activeButtons.RemoveAll(Button => Button.DeleteNextFrame);

                EntityManager.RemoveEntities(); //remove all entities queued for deletion
                EntityManager.RunAIs();
            }
        }

        public void ManageLists()
        {
            foreach (Projectile projectile in Main.enemyProjectilesToAddNextFrame)
            {
                Main.activeProjectiles.Add(projectile);
            }

            Main.enemyProjectilesToAddNextFrame.Clear();

            foreach (Projectile projectile in Main.friendlyProjectilesToAddNextFrame)
            {
                Main.activeFriendlyProjectiles.Add(projectile);
            }

            Main.friendlyProjectilesToAddNextFrame.Clear();

            foreach (NPC npc in Main.NPCsToAddNextFrame)
            {
                Main.activeNPCs.Add(npc);
            }

            Main.NPCsToAddNextFrame.Clear();
        }

        public void CheckButtonClicks()
        {
            foreach (Button button in Main.activeButtons)
            {
                if (button.IsButtonClicked() && ButtonCooldown == 0)
                {
                    ButtonCooldown = DefaultButtonCooldown;

                    button.HandleClick();
                }
            }
        }
    }
}
