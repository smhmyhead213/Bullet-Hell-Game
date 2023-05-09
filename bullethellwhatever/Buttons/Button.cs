﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using bullethellwhatever.MainFiles;

namespace bullethellwhatever.Buttons
{
    public class Button
    {
        public Vector2 Position;
        public Vector2 Scale; //how muhc the button is scaled up when drawn
        public Texture2D Texture;
        public GameState.Difficulties? DifficultyChange;
        public GameState.GameStates? Destination;

        public bool DeleteNextFrame;
        public Rectangle ButtonRectangle => new((int)Position.X - (Texture.Width / 2 * (int)Scale.X), (int)Position.Y - (Texture.Height / 2 * (int)Scale.Y), Texture.Width * (int)Scale.X, Texture.Height * (int)Scale.Y);

        public Button(Vector2 position, Texture2D texture, GameState.GameStates? destination, GameState.Difficulties? difficultyChange, Vector2 scale)
        {
            Position = position;
            Texture = texture;
            Scale = scale;
            DifficultyChange = difficultyChange;
            Destination = destination;
        }

        public virtual void HandleClick()
        {
            if (DifficultyChange is not null)
            {
                GameState.Difficulty = DifficultyChange;
            }

            if (Destination is not null)
            {
                GameState.State = Destination;
            }

            foreach (Button button in Main.activeButtons)
            {
                //I HATE YOU I HATE YOU I HATE YOU I HATE UI I HATE UI I HATE UI
                button.DeleteNextFrame = true;
            }
        }

        public bool IsButtonClicked()
        {
            var mouseState = Mouse.GetState();
            var mousePosition = new Point(mouseState.X, mouseState.Y);

            return ButtonRectangle.Contains(mousePosition) && mouseState.LeftButton == ButtonState.Pressed;
        }
    }
}

