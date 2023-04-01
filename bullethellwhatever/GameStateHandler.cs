﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace bullethellwhatever
{
    public class GameStateHandler
    {
        public bool isGameStarted;
        public int ButtonCooldown;
        public void HandleGame()
        {
            if (ButtonCooldown > 0)
            {
                ButtonCooldown--;
            }

            if (GameState.State == GameState.GameStates.TitleScreen)
            {
                isGameStarted = false;

                foreach (Button button in Main.activeButtons)
                {
                    if (button.IsButtonClicked() && ButtonCooldown == 0)
                    {
                        ButtonCooldown = 5;
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
                        ButtonCooldown = 5;
                        
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
                        ButtonCooldown = 5;

                        button.HandleClick();
                    }
                }

                Main.activeButtons.RemoveAll(Button => Button.DeleteNextFrame);
            }

            if (GameState.State == GameState.GameStates.InGame)
            {
                if (!isGameStarted)
                {
                    EntityManager.SpawnBoss();
                    Main.player.Spawn(new Vector2(Main._graphics.PreferredBackBufferWidth / 2, Main._graphics.PreferredBackBufferHeight / 2), new Vector2(5, 5), 10, Main.playerTexture);
                    isGameStarted = true;
                }

                EntityManager.RemoveEntities(); //remove all entities queued for deletion
                EntityManager.RunAIs();
            }
        }
    }
}
