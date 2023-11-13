using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace bullethellwhatever.DrawCode.UI
{
    public class Menu
    {
        public Vector2 Position;
        public Vector2 Size;
        public Texture2D Texture; // background texture
        public Color BackgroundColour;
        public List<UIElement> UIElements;
        public int DefaultButtonCooldown => 25;
        public int ButtonCooldown;

        public Menu(Vector2 position, Vector2 size, Texture2D texture)
        {
            Position = position;
            Size = size;
            Texture = texture;
            UIElements = new List<UIElement>();
            ButtonCooldown = 0;
            BackgroundColour = Color.White; // dont colour if none is specified
        }

        public Menu(Vector2 position, float width, float height, Texture2D texture)
        {
            Position = position;
            Size = new Vector2(width, height);
            Texture = texture;
            UIElements = new List<UIElement>();
            ButtonCooldown = 0;
            BackgroundColour = Color.Black; // dont colour if none is specified
        }

        public void SetBackgroundColour(Color colour)
        {
            BackgroundColour = colour;
        }

        public void Display()
        {
            gameStateHandler.MenusToAdd.Add(this);
        }

        public void Hide()
        {
            gameStateHandler.MenusToRemove.Add(this);
        }

        public void Update()
        {
            if (ButtonCooldown > 0)
            {
                ButtonCooldown--;
            }

            foreach (UIElement uIElement in UIElements)
            {
                if (uIElement.IsClicked() && ButtonCooldown == 0 && !WasMouseDownLastFrame)
                {
                    ButtonCooldown = DefaultButtonCooldown;

                    uIElement.HandleClick();
                }

                uIElement.Update();
            }
        }

        public void AddUIElement(UIElement uiElement)
        {
            uiElement.Owner = this;
            UIElements.Add(uiElement);
        }

        public float Width() => Size.X;

        public float Height() => Size.Y;

        public Vector2 RelativeCentreOfMenu() => new Vector2(Width(), Height()) / 2f;

        public Vector2 TopLeft() => Position - RelativeCentreOfMenu();
        public virtual void Draw(SpriteBatch s)
        {
            Drawing.BetterDraw(Texture, Position, null, BackgroundColour, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1); // scale texture up to required size

            foreach (UIElement uiElement in UIElements)
            {
                uiElement.Draw(s);
            }
        }
    }
}
