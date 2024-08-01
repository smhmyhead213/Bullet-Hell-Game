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
        public static void DrawDialogues(SpriteBatch spriteBatch)
        {
            foreach (NPC npc in Main.activeNPCs)
            {
                if (npc.dialogueSystem.dialogueObject is not null)
                {
                    Vector2 drawPosition = new Vector2(npc.dialogueSystem.dialogueObject.Position.X - 3.5f * npc.dialogueSystem.dialogueObject.Text.Length, npc.dialogueSystem.dialogueObject.Position.Y);

                    Utilities.drawTextInDrawMethod(npc.dialogueSystem.dialogueObject.Text, drawPosition, spriteBatch, Main.font, Color.White);
                }
            }

            if (player.dialogueSystem.dialogueObject is not null)
            {
                Vector2 drawPosition = new Vector2(player.dialogueSystem.dialogueObject.Position.X - 3.5f * player.dialogueSystem.dialogueObject.Text.Length, player.dialogueSystem.dialogueObject.Position.Y);

                Utilities.drawTextInDrawMethod(player.dialogueSystem.dialogueObject.Text, drawPosition, spriteBatch, Main.font, Color.White);
            }
        }

        public void Dialogue(string dialogueToWrite, int framesBetweenLetters, int duration)
        {
            dialogueObject = new DialogueObject(dialogueToWrite, Owner, framesBetweenLetters, duration);
        }

        public void ClearDialogue()
        {
            dialogueObject.Text = string.Empty;
            dialogueObject.CharactersWritten = 0;
        }
    }
}
