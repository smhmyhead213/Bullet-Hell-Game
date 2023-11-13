using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.DrawCode.UI.Buttons
{
    public class BackButton : Button
    {
        public BackButton(Vector2 position, string texture, Vector2 size) : base(position, Assets[texture], size)
        {
            Texture = Assets[texture];
            Size = size;
        }
        public BackButton(Texture2D texture, Vector2 size, Menu owner) : base(texture, size, owner)
        {
            Texture = texture;
            Size = size;
            Owner = owner;
        }
        public override void HandleClick()
        {
            GameState.RevertToPreviousGameState();
            Owner.Hide();

            base.HandleClick();
        }
        public override void Draw(SpriteBatch s)
        {
            base.Draw(s);

            //debug output, dont worry about this

            Utilities.drawTextInDrawMethod(IsLeftClickDown().ToString(), Utilities.CentreOfScreen() + new Vector2(0, 300), s, font, Color.White);
            Utilities.drawTextInDrawMethod(ClickBox.Contains(MousePosition).ToString(), Utilities.CentreOfScreen() + new Vector2(-200, 300), s, font, Color.White);
            Utilities.drawTextInDrawMethod(WasMouseDownLastFrame.ToString(), Utilities.CentreOfScreen() + new Vector2(-400, 300), s, font, Color.White);
            Utilities.drawTextInDrawMethod(Owner.ButtonCooldown.ToString(), Utilities.CentreOfScreen() + new Vector2(-600, 300), s, font, Color.White);
        }
    }
}
