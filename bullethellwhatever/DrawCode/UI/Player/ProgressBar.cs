﻿using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses.Hitboxes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.WIC;

namespace bullethellwhatever.DrawCode.UI.Player
{
    public class ProgressBar : UIElement
    {
        public float Progress;
        public bool DisplayPercentage;
        public ProgressBar(string texture, Vector2 size, Vector2 position = default, float progress = 1) : base(texture, size, position)
        {
            Progress = progress;
            DisplayPercentage = false;
        }
        public override bool CanBeClicked()
        {
            return false;
        }
        public override void Draw(SpriteBatch s)
        {
            float progress = MathHelper.Clamp(Progress, 0f, 1f);

            float emptySpaceOnLeft = (1 - progress) / 2; // fraction of bar width

            Texture2D texture = AssetRegistry.GetTexture2D("box");

            float opacity = 1f;

            Vector2 size = new Vector2(Size.X / texture.Width, Size.Y / texture.Height);

            //HP bar background.

            _spriteBatch.Draw(texture, Position, null, Color.LimeGreen * opacity, 0f, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, 0f);

            _spriteBatch.Draw(texture, new Vector2(Position.X - emptySpaceOnLeft * Size.X, Position.Y), null, Color.Red * opacity, 0f, new Vector2(texture.Width / 2, texture.Height / 2), new Vector2(size.X * progress, size.Y), SpriteEffects.None, 0f);

            if (DisplayPercentage)
            {
                Drawing.DrawText(Round(Progress * 100, 1).ToString() + "%", Position + new Vector2(Size.X / 2 + 100f, 0), s, font, Color.White, Vector2.One);
            }
        }
    }
}
