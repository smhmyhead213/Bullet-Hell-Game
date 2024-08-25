using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode.UI.Buttons
{
    public class MainMenuButton : UIElement
    {
        public MainMenuButton(string texture, Vector2 size, Vector2 position = default) : base(texture, size, position)
        {

        }
        public MainMenuButton(string texture, float size, Vector2 position = default) : base(texture, size, position)
        {

        }
        public override void HandleClick()
        {
            base.HandleClick();

            UIManager.ClearUI();
            EntityManager.Clear();
            GameState.SetGameState(GameState.GameStates.TitleScreen);
        }
    }
}
