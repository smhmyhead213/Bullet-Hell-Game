using bullethellwhatever.Projectiles.Player;
using Microsoft.Xna.Framework;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.Buttons;

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
            foreach (Projectile projectile in Main.enemyProjectilesToAddNextFrame)
            {
                Main.activeProjectiles.Add(projectile);
            }

            Main.enemyProjectilesToAddNextFrame.Clear();

            foreach (FriendlyProjectile projectile in Main.friendlyProjectilesToAddNextFrame)
            {
                Main.activeFriendlyProjectiles.Add(projectile);
            }

            Main.friendlyProjectilesToAddNextFrame.Clear();

            foreach (NPC npc in Main.NPCsToAddNextFrame)
            {
                Main.activeNPCs.Add(npc);
            }

            Main.NPCsToAddNextFrame.Clear();

            if (ButtonCooldown > 0)
            {
                ButtonCooldown--;
            }

            if (GameState.State == GameState.GameStates.TitleScreen) //fix this at some point
            {
                isGameStarted = false;

                foreach (Button button in Main.activeButtons)
                {
                    if (button.IsButtonClicked() && ButtonCooldown == 0)
                    {
                        ButtonCooldown = DefaultButtonCooldown;
                        button.HandleClick();
                    }
                }

                Main.activeButtons.RemoveAll(Button => Button.DeleteNextFrame);
            }

            if (GameState.State == GameState.GameStates.BossSelect)
            {
                isGameStarted = false;

                foreach (Button button in Main.activeButtons)
                {
                    if (button.IsButtonClicked() && ButtonCooldown == 0)
                    {
                        ButtonCooldown = DefaultButtonCooldown;

                        button.HandleClick();
                    }
                }

                Main.activeButtons.RemoveAll(Button => Button.DeleteNextFrame);
            }

            if (GameState.State == GameState.GameStates.DifficultySelect)
            {
                isGameStarted = false;

                foreach (Button button in Main.activeButtons)
                {
                    if (button.IsButtonClicked() && ButtonCooldown == 0)
                    {
                        ButtonCooldown = DefaultButtonCooldown;

                        button.HandleClick();
                    }
                }

                Main.activeButtons.RemoveAll(Button => Button.DeleteNextFrame);
            }

            if (GameState.State == GameState.GameStates.Settings)
            {
                isGameStarted = false;

                foreach (Button button in Main.activeButtons)
                {
                    if (button.IsButtonClicked() && ButtonCooldown == 0)
                    {
                        ButtonCooldown = DefaultButtonCooldown;

                        button.HandleClick();
                    }
                }
            }

            if (GameState.State == GameState.GameStates.InGame)
            {
                if (!isGameStarted)
                {
                    EntityManager.SpawnBoss();
                    Main.player.Spawn(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 10 * 9), new Vector2(0, 0), 10, Main.playerTexture);
                    Main.activeProjectiles.Clear();
                    isGameStarted = true;

                }

                foreach (Button button in Main.activeButtons)
                {
                    if (button.IsButtonClicked() && ButtonCooldown == 0)
                    {
                        ButtonCooldown = DefaultButtonCooldown;

                        button.HandleClick();
                    }
                }

                Main.activeButtons.RemoveAll(Button => Button.DeleteNextFrame);

                EntityManager.RemoveEntities(); //remove all entities queued for deletion
                EntityManager.RunAIs();
            }
        }
    }
}
