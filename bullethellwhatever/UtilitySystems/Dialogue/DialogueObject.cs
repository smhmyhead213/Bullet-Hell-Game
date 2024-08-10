﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.UtilitySystems.Dialogue
{
    public class DialogueObject
    {
        public Vector2 Position;
        public string Text;
        public string TextToWrite;
        public Entity Owner;
        public int FramesBetweenLetters;
        public int Duration;
        public int DialogueTimer;

        public bool DeleteNextFrame;
        public int CharactersWritten;

        public DialogueObject(string text, int framesBetweenLetters, int duration, Entity owner)
        {
            Text = string.Empty;
            TextToWrite = text;
            DeleteNextFrame = false;
            FramesBetweenLetters = framesBetweenLetters;
            Duration = duration;

            Owner = owner;

            DialogueTimer = 0;
        }
        public DialogueObject(string text, int framesBetweenLetters, int duration, Vector2 position)
        {
            Text = string.Empty;

            TextToWrite = text;

            DeleteNextFrame = false;

            FramesBetweenLetters = framesBetweenLetters;

            Duration = duration;

            Position = position;

            DialogueTimer = 0;
        }

        public void DoDialogue()
        {
            if (Owner is not null)
            {
                float heightAboveEntity = 50f;

                Position = new Vector2(Owner.Position.X, Owner.Position.Y - (Owner.Texture.Height * Owner.GetSize().Y) - heightAboveEntity);
            }

            // either the owner or the position is given in constructors, so if the owner is null the position is already given

            if (DialogueTimer / FramesBetweenLetters == CharactersWritten && TextToWrite != "")
            {
                if (CharactersWritten <= TextToWrite.Length)
                {           
                    Text = TextToWrite.Substring(0, CharactersWritten);
                }

                CharactersWritten++;             
            }

            DialogueTimer++;

            if (DialogueTimer == Duration)
            {
                Text = string.Empty;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Utilities.drawTextInDrawMethod(Text, Position, spriteBatch, font, Color.White);
        }
    }
}
