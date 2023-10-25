using System.Collections.Generic;
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

        public DialogueObject(string text, Entity owner, int framesBetweenLetters, int duration)
        {
            Text = string.Empty;
            TextToWrite = text;
            DeleteNextFrame = false;
            FramesBetweenLetters = framesBetweenLetters;
            Duration = duration;
            Owner = owner;

            DialogueTimer = 0;

        }

        public void DoDialogue()
        {
            Position = new Vector2(Owner.Position.X, Owner.Position.Y - (Owner.Texture.Height * Owner.GetSize().Y) - 50f);

            //fix this drawing every possible string every frame
            if (DialogueTimer / FramesBetweenLetters == CharactersWritten)
            {
                if (CharactersWritten <= TextToWrite.Length)
                {
                    //dialogueObject = new DialogueObject(position, dialogueToWrite.Substring(0, CharactersWritten), Owner);                    
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
    }
}
