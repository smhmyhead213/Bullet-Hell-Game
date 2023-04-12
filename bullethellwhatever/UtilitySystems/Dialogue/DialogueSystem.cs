using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.UtilitySystems.Dialogue
{
    public class DialogueSystem
    {
        public int DialogueTimer;
        public int CharactersWritten;
        public string StringWritten;
        public DialogueObject dialogueObject;
        public Entity Owner;
        public bool HasWritingStarted;
        public DialogueSystem(Entity owner)
        {
            DialogueTimer = 0;
            CharactersWritten = 0;
            Owner = owner;
            
        }

        public void Dialogue(Vector2 position, string dialogueToWrite, int framesBetweenLetters, int duration)
        {
            dialogueObject.Position = new Vector2(position.X, position.Y - 50f);

            //fix this drawing every possible string every frame
            if (DialogueTimer / framesBetweenLetters == CharactersWritten + 1)
            {
                if (CharactersWritten <= dialogueToWrite.Length)
                {
                    //dialogueObject = new DialogueObject(position, dialogueToWrite.Substring(0, CharactersWritten), Owner);                    
                    dialogueObject.Text = dialogueToWrite.Substring(0, CharactersWritten);
                }

                else
                {
                    dialogueObject.DeleteNextFrame = true;
                }

                CharactersWritten++;

                HasWritingStarted = true;             
            }
            DialogueTimer++;
        }

        public void ClearDialogue()
        {
            dialogueObject.Text = string.Empty;
            DialogueTimer = 0;
            CharactersWritten = 0;
        }

        public void MakeSureThisIsTheOnlyActiveDialogue()
        {
            for (int i = 0; i < MainFiles.Main.activeDialogues.Count; i++) //Delete all previoous instances of itself; each boss only has 1 dialogue active at once.
            {
                if (MainFiles.Main.activeDialogues[i].Owner == Owner)
                {
                    MainFiles.Main.activeDialogues[i].Owner.DeleteNextFrame = true;
                }
            }
        }
    }
}
