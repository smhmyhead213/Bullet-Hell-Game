using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bullethellwhatever.MainFiles;

namespace bullethellwhatever.Buttons
{
    public class SettingsControlSchemeButton : Button
    {
        public bool WeaponSwitchControl;
        public SettingsControlSchemeButton(Vector2 position, string texture, Vector2 scale, bool weaponSwitchControl) : base(position, texture, null, null, scale)
        {
            Position = position;
            Texture = Main.Assets[texture];
            Scale = scale;
            WeaponSwitchControl = weaponSwitchControl; //true for scroll, false for numbers
        }

        public override void HandleClick()
        {
            GameState.WeaponSwitchControl = WeaponSwitchControl;
            GameState.HasASettingBeenChanged = true;



        }
    }
}
