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
using bullethellwhatever.UtilitySystems.Dialogue;

namespace bullethellwhatever.MainFiles
{
    public static class GameStateHandler
    {
        public static bool isGameStarted
        {
            get;
            set;
        }
        public static string activeBoss; //use a swicth statement to spawn a boss in

        public static void HandleGame()
        {
            switch (State)
            {
                case GameStates.TitleScreen:
                    HandleTitleScreen();
                    Credits.ReadInCreditsAlready = false; // i know this is an awful way to do it, in the future make credits reset when opened
                    break;            
            }

            if (!isGameStarted)
            {
                //EntityManager.SpawnBoss();
                player = new Player("box");
                player.Spawn(new Vector2(GameWidth / 2, GameHeight / 10 * 9), new Vector2(0, 0), 10, "box");
                EntityManager.Clear();
                isGameStarted = true;
            }

            if (!Utilities.ImportantMenusPresent())
            {
                EntityManager.RunAIs();
            }

            EntityManager.RemoveEntities(); //remove all entities queued for deletion

            TimeInCurrentGameState++;
        }

        public static void HandleTitleScreen()
        {
            string weaponSwitchInstruction = WeaponSwitchControl == WeaponSwitchControls.NumberKeys ? "Press 1, 2 or 3 to switch weapons." : "Use the scroll wheel to switch weapons.";
            string[] instructions = new string[]
            {
                "Left click to use your weapon.",
                weaponSwitchInstruction,
                "Press space to dash. You are immune to damage while dashing."
            };

            int timeBetweenInstructions = 120;
            int yDistanceBetweenInstructions = 30;
            int index = TimeInCurrentGameState / timeBetweenInstructions;

            if (TimeInCurrentGameState % timeBetweenInstructions == 0 && index < instructions.Length)
            {
                DialogueSystem.Dialogue(instructions[index], 1, new Vector2(GameWidth / 5, GameHeight / 3 + yDistanceBetweenInstructions * index));
            }
        }
    }
}
