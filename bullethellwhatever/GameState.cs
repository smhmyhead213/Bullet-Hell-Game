namespace bullethellwhatever;

public class GameState
{
    public enum Bosses
    {
        TestBoss
    }

    public enum Difficulties
    {
        Easy, //chloe gamemode
        Normal,
        Hard,
        Insane
    }

    public enum GameStates
    {
        TitleScreen,
        Settings,
        BossSelect,
        DifficultySelect,
        InGame
    }

    public static GameStates State;

    public static Difficulties Difficulty;

    public static Bosses Boss;

    public static bool WeaponSwitchControl; //true for scroll, false for number keys

    public static bool HasASettingBeenChanged;
}