using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever
{
    public class GameState
    {
        public enum GameStates
        {
            TitleScreen,
            BossSelect,
            DifficultySelect,
            InGame,
        }

        public static GameStates State;
        public enum Difficulties
        {
            Easy, //chloe gamemode
            Normal,
            Hard,
            Insane,
        }

        public static Difficulties Difficulty;
    }
}
