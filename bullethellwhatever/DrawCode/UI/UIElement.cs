using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode.UI
{
    public class UIElement
    {
        public Vector2 PositionInMenu; // position relative to menu its contained in
        public Vector2 Position; // actual position in game
        public Vector2 Size;
        public Texture2D Texture;
        public Menu Owner;
        public RectangleButGood ClickBox;
        public Action ClickEvent;
        public UIElement(string texture, Vector2 size, Menu owner = null, Vector2 position = default)
        {
            PositionInMenu = position;
            Texture = Assets[texture];
            Size = size;
            Owner = owner;
        }
        public UIElement(Texture2D texture, Vector2 size, Menu owner = null, Vector2 position = default)
        {
            PositionInMenu = position;
            Texture = texture;
            Size = size;
            Owner = owner;
        }
        public virtual void Update()
        {
            Position = CalculateActualPostion();

            ClickBox = new(Position.X - (Texture.Width / 2 * Size.X), Position.Y - (Texture.Height / 2 * Size.Y), Texture.Width * Size.X, Texture.Height * Size.Y);
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
        public virtual void StandaloneUIElement()
        {
            Menu holder = new Menu(Position, new Vector2(Texture.Width * Size.X, Texture.Height * Size.Y), Assets["box"]);
            Owner = holder;
            SetPositionInMenu(holder.RelativeCentreOfMenu());
            Update();
            holder.AddUIElement(this);
            holder.Display();
        }
        public bool IsClicked()
        {
            return ClickBox.Contains(MousePosition) && IsLeftClickDown();
        }
        public void SetPositionInMenu(Vector2 pos)
        {
            PositionInMenu = pos;
            Position = CalculateActualPostion();
        }
        public virtual void Draw(SpriteBatch s)
        {
            Drawing.BetterDraw(Texture, Position, null, Color.White, 0, Size, SpriteEffects.None, 1);
        }
    }
}
