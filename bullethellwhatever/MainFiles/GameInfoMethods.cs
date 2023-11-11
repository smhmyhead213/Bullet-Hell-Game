using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.MainFiles
{
    public static class GameInfoMethods
    {
        public static float ScaleFactor() => ScreenWidth / IdealScreenWidth;
        public static GameState.Difficulties? GetDifficulty()
        {
            return GameState.Difficulty;
        }

        public static GameState.Difficulties? Difficulty(int difficultyNumber) // 0 easy 1 normal 2 hard 3 insane
        {
            return (GameState.Difficulties?)difficultyNumber;
        }
    }
}
