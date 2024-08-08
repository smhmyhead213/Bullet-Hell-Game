using Microsoft.Xna.Framework;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.DrawCode.UI;

using static bullethellwhatever.MainFiles.GameState;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using bullethellwhatever.Projectiles;
using bullethellwhatever.NPCs;

namespace bullethellwhatever.MainFiles
{
    public class GameStateHandler
    {
        public bool isGameStarted
        {
            get;
            set;
        }
        public string activeBoss; //use a swicth statement to spawn a boss in

        public GameStateHandler()
        {

        }
        public void HandleGame()
        {
            switch (State)
            {
                case GameStates.TitleScreen:
                    Credits.ReadInCreditsAlready = false; // i know this is an awful way to do it, in the future make credits reset when opened
                    break;            
            }

            if (!isGameStarted)
            {
                //EntityManager.SpawnBoss();
                player = new Player("box");
                player.Spawn(new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 10 * 9), new Vector2(0, 0), 10, "box");
                activeProjectiles.Clear();
                isGameStarted = true;
            }

            if (!Utilities.ImportantMenusPresent())
            {
                EntityManager.RemoveEntities(); //remove all entities queued for deletion
                EntityManager.RunAIs();
            }
        }
    }
}
