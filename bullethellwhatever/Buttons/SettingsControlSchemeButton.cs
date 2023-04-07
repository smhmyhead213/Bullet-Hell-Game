using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bullethellwhatever.MainFiles;

namespace bullethellwhatever.Buttons
{
    public class SettingsControlSchemeButton : Button
    {
        public bool WeaponSwitchControl;
        public SettingsControlSchemeButton(Vector2 position, Texture2D texture, Vector2 scale, bool weaponSwitchControl)
        {
            Position = position;
            Texture = texture;
            Scale = scale;
            WeaponSwitchControl = weaponSwitchControl; //true for scroll false for numbers
        }

        public override void HandleClick()
        {
            GameState.WeaponSwitchControl = WeaponSwitchControl;
            GameState.HasASettingBeenChanged = true;



        }
    }
}
