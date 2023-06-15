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

        public static GameStates? State;
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
            TestBoss,
            SecondBoss,
            CrabBoss,
        }

        public static Bosses Boss;

        public static bool WeaponSwitchControl; //true for scroll, false for number keys

        public static bool HasASettingBeenChanged;
    }
}
