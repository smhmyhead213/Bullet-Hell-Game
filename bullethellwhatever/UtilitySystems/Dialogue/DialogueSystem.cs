using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.NPCs;

namespace bullethellwhatever.UtilitySystems.Dialogue
{
    public static class DialogueSystem
    {      
        public static List<DialogueObject> ActiveDialogues = new List<DialogueObject>();

        public static void Initialise()
        {
            
        }

        public static void DrawDialogues(SpriteBatch spriteBatch)
        {
            foreach (DialogueObject obj in ActiveDialogues)
            {
                obj.Draw(spriteBatch);
            }
        }

        public static void Update()
        {
            foreach (DialogueObject obj in ActiveDialogues)
            {
                obj.DoDialogue();
            }
        }
        public static void Dialogue(string dialogueToWrite, int framesBetweenLetters, Entity owner, int duration = -1, float scale = 1f)
        {
            ActiveDialogues.Add(new DialogueObject(dialogueToWrite, framesBetweenLetters, duration, owner, scale));
        }
        public static void Dialogue(string dialogueToWrite, int framesBetweenLetters, Vector2 position, int duration = -1, float scale = 1f)
        {
            ActiveDialogues.Add(new DialogueObject(dialogueToWrite, framesBetweenLetters, duration, position, scale));
        }
        public static void ClearDialogues()
        {
            ActiveDialogues.Clear();
        }
    }
}
