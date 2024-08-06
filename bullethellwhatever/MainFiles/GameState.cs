using bullethellwhatever.DrawCode;
using bullethellwhatever.DrawCode.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.MainFiles
{
    public class GameState
    {
        public enum GameStates
        {
            TitleScreen,
            Settings,
            BossSelect,
            DifficultySelect,
            Credits,
            InGame,
        }

        public static GameStates State;
        public static Stack<GameStates> PreviousStates = new Stack<GameStates>();
        public enum Difficulties
        {
            Easy, //chloe gamemode
            Normal,
            Hard,
            Insane,
        }

        public static Difficulties? Difficulty;
        public enum Bosses
        {
            SecondBoss,
            CrabBoss,
            EyeBoss,
        }

        public static Bosses Boss;

        public static bool WeaponSwitchControl; //true for scroll, false for number keys

        public static bool HasASettingBeenChanged;

        public static void SetGameState(GameStates state, bool wasRevertedFromPreviousState = false)
        {
            if (!wasRevertedFromPreviousState)
            {
                PreviousStates.Push(State);
            }

            State = state;

            switch (State)
            {
                case GameStates.TitleScreen: UI.CreateTitleScreenMenu(); break;
                case GameStates.BossSelect : UI.CreateBossSelectMenu(); break;
                case GameStates.Settings: UI.CreateSettingsMenu(); break;
                case GameStates.DifficultySelect : UI.CreateDifficultySelectMenu(); break;
                case GameStates.Credits: UI.CreateCreditsMenu(); break;
                case GameStates.InGame:
                    DrawGame.PlayerHUD.ResetHUD();
                    break; // dont bother doing anything
                default: throw new Exception("if you hit this exception you really messed up"); // no game state is active somehow
            }
        }

        public static void RevertToPreviousGameState()
        {
            SetGameState(PreviousStates.Pop(), true);
        }
    }
}
