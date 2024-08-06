using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.AssetManagement;
using System.Windows.Forms;

namespace bullethellwhatever.DrawCode.UI
{
    public class UIElement
    {
        public Vector2 PositionInMenu; // position relative to menu its contained in
        public Vector2 Position; // actual position in game
        public Vector2 Size;
        public Texture2D Texture;
        public Menu Owner;
        public bool IsInMenu => Owner is not null;

        public RectangleButGood ClickBox;
        public Action ClickEvent;
        public UIElement(string texture, Vector2 size, Vector2 position = default)
        {
            PositionInMenu = position;
            Texture = AssetRegistry.GetTexture2D(texture);
            Size = size;
        }

        public virtual void Update()
        {
            if (IsInMenu)
            {
                Position = CalculateActualPostion();
            }

            if (IsClicked() && UIManager.ButtonCooldown == 0 && !WasMouseDownLastFrame)
            {
                UIManager.ButtonCooldown = UIManager.DefaultButtonCooldown;

                HandleClick();
            }

            ClickBox = new(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Size.X, Size.Y);
        }
        public virtual Vector2 CalculateActualPostion()
        {
            return Owner.TopLeft() + PositionInMenu;
        }
        public virtual void SetClickEvent(Action action)
        {
            ClickEvent = action;
        }
        public virtual void HandleClick()
        {
            if (ClickEvent is not null)
            {
                ClickEvent();
            }
        }
        public bool IsClicked()
        {
            return ClickBox.Contains(MousePosition) && IsLeftClickDown();
        }

        public void AddToMenu(Menu menu)
        {
            Owner = menu;
            menu.AddUIElement(this);
        }
        public void SetPositionInMenu(Vector2 pos)
        {
            if (Owner is not null)
            {
                PositionInMenu = pos;
                Position = CalculateActualPostion();
            }
            else
            {
                throw new Exception("cannot set menu position of buttons that arent in a menu. maybe you forgot to add them to the menu?");
            }
        }

        public void AddToActiveUIElements()
        {
            if (Owner is null) // only add to the active ui elements if not part of a menu. the menu will handle updating and drawing the ui element
                UIManager.UIElementsToAddNextFrame.Add(this);
        }

        public void Remove()
        {
            UIManager.UIElemntsToRemoveNextFrame.Add(this);
        }
        public virtual void Draw(SpriteBatch s)
        {
            Color colour = ClickBox.Contains(MousePosition) ? Color.Red : Color.White;

            Drawing.BetterDraw(Texture, Position, null, colour, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1);
        }
    }
}
