using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode.UI.Buttons;

namespace bullethellwhatever.DrawCode
{
    public static class Credits
    {
        public static int CreditsTimer;
        public static bool ReadInCreditsAlready;
        public static string[] Credit;
        public static Vector2[] CreditPositions;
        public static void CreditSequence(SpriteBatch spriteBatch)
        {
            if (!ReadInCreditsAlready)
            {
                ReadInCredits();
            }

            for (int i = 0; i < Credit.Length; i++)
            {
                CreditPositions[i].Y = CreditPositions[i].Y - 1f;
                
                if (CreditPositions[i].Y > 0f && CreditPositions[i].Y < _graphics.PreferredBackBufferHeight) // the use of preferredbackbufferheight might cause resolution independence problems
                {
                    Utilities.drawTextInDrawMethod(Credit[i], CreditPositions[i], spriteBatch, font, Color.White);
                }
            }          
        }

        public static void ReadInCredits()
        {
            Credit = File.ReadAllLines(@"Content/credits.txt", Encoding.UTF8);

            ReadInCreditsAlready = true;

            // also initialise positions

            CreditPositions = new Vector2[Credit.Length];

            for (int i = 0; i < Credit.Length; i++)
            {
                CreditPositions[i] = new Vector2(_graphics.PreferredBackBufferWidth / 2, (i + 1) * 50);
            }
        }
    }
}
