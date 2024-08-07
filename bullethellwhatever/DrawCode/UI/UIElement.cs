﻿using bullethellwhatever.BaseClasses;
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
using bullethellwhatever.UtilitySystems.SoundSystems;

namespace bullethellwhatever.DrawCode.UI
{
    public class UIElement
    {
        public Vector2 PositionInMenu; // position relative to menu its contained in
        public Vector2 Position; // actual position in game
        public Vector2 Size;
        public Texture2D Texture;
        public Menu Owner;

        public Color Colour;

        public float Opacity;
        public int AITimer;
        public bool IsInMenu => Owner is not null;
        
        public RectangleButGood ClickBox;

        public Action ExtraAI;
        public Action ClickEvent;

        /// <summary>
        /// Constructs a new UIElement.
        /// </summary>
        /// <param name="texture">The name of the texture to used used when drawing the UIElement.</param>
        /// <param name="size">The size of the UIElement.</param>
        /// <param name="position">The position of the UIElement.</param>
        public UIElement(string texture, Vector2 size, Vector2 position = default)
        {
            PositionInMenu = position;
            Texture = AssetRegistry.GetTexture2D(texture);

            Size = size;

            AITimer = 0;

            Colour = Color.White;

            Opacity = 1;
        }

        /// <summary>
        /// Constructs a new UIElement.
        /// </summary>
        /// <param name="texture">The name of the texture to used used when drawing the UIElement.</param>
        /// <param name="size">A scale multiplier of the texture's size to scale it up.</param>
        /// <param name="position">The position of the UIElement.</param>
        public UIElement(string texture, float size, Vector2 position = default)
        {
            PositionInMenu = position;
            Texture = AssetRegistry.GetTexture2D(texture);

            Size = Texture.TextureDimensionsToVector() * size;

            AITimer = 0;

            Colour = Color.White;

            Opacity = 1;
        }

        public virtual void Update()
        {
            if (ExtraAI is not null)
                ExtraAI();

            if (IsInMenu)
            {
                Position = CalculateActualPostion();
            }

            if (CanBeClicked())
            {
                UIManager.ButtonCooldown = UIManager.DefaultButtonCooldown;

                HandleClick();
            }

            ClickBox = new(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Size.X, Size.Y);

            AITimer++;
        }

        public virtual bool CanBeClicked()
        {
            return IsClicked() && UIManager.ButtonCooldown == 0 && !WasMouseDownLastFrame;
        }
        public virtual Vector2 CalculateActualPostion()
        {
            return Owner.TopLeft() + PositionInMenu;
        }
        public virtual void SetClickEvent(Action action)
        {
            ClickEvent = action;
        }
        public virtual void SetExtraAI(Action extraAI)
        {
            ExtraAI = extraAI;
        }
        public virtual void HandleClick()
        {
            if (ClickEvent is not null)
            {
                ClickEvent();
            }

            SoundSystem.PlaySound("testsound");
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

        public void SetOpacity(float opacity)
        {
            Opacity = opacity;
        }
        public virtual Color ColourIfHovered()
        {
            return ClickBox.Contains(MousePosition) ? Color.Red : Colour; // in the future make the colour more red, not just red
        }
        public virtual void Draw(SpriteBatch s)
        {
            Color colour = ColourIfHovered();
            
            Drawing.BetterDraw(Texture, Position, null, colour * Opacity, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1);
        }
    }
}
