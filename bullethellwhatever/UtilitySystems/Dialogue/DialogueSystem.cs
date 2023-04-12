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
        public DialogueSystem(Entity owner)
        {
            //DialogueTimer = 0;
            CharactersWritten = 0;
            Owner = owner;
            
        }

        public void Dialogue(Vector2 position, string dialogueToWrite, int framesBetweenLetters, int duration)
        {
            dialogueObject.Position = position;
            dialogueObject.Owner = Owner;

            if (MainFiles.Main.activeDialogues.Contains(dialogueObject))
                MainFiles.Main.activeDialogues.Remove(dialogueObject);

            if (DialogueTimer % framesBetweenLetters == 0)
            {
                CharactersWritten = DialogueTimer / framesBetweenLetters; //increment CharactersWritten

                if (dialogueToWrite.Length >= CharactersWritten)
                    dialogueObject = new DialogueObject(position, dialogueToWrite.Substring(0, CharactersWritten), Owner);                   
                else
                    dialogueObject.DeleteNextFrame = true;
            }

            MainFiles.Main.activeDialogues.Add(dialogueObject);

            DialogueTimer++;
        }

        public void ClearDialogue()
        {
            if (MainFiles.Main.activeDialogues.Contains(dialogueObject))
                MainFiles.Main.activeDialogues.Remove(dialogueObject);

            dialogueObject.Text = string.Empty;
            DialogueTimer = 0;

            //for (int i = 0; i < MainFiles.Main.activeDialogues.Count; i++)
            //{
            //    if (MainFiles.Main.activeDialogues[i].Owner == Owner)
            //    {
            //        MainFiles.Main.activeDialogues[i].Owner.DeleteNextFrame = true;
            //    }
            //}
        }
    }
}
