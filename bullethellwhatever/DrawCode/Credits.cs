using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using bullethellwhatever.Buttons;
using bullethellwhatever.MainFiles;


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

            Button backButton = new Button(new Vector2(Main._graphics.PreferredBackBufferWidth / 5, Main._graphics.PreferredBackBufferHeight / 5), "Back", GameState.GameStates.TitleScreen, null, new Vector2(3, 3));

            if (!Main.activeButtons.Contains(backButton))
                Main.activeButtons.Add(backButton);

            spriteBatch.Draw(backButton.Texture, backButton.Position, null, Color.White, 0f, new Vector2(backButton.Texture.Width / 2, backButton.Texture.Height / 2), new Vector2(3, 3), SpriteEffects.None, 0f);

            for (int i = 0; i < Credit.Length; i++)
            {
                CreditPositions[i].Y = CreditPositions[i].Y - 1f;
                
                if (CreditPositions[i].Y > 0f && CreditPositions[i].Y < Main._graphics.PreferredBackBufferHeight)
                {
                    Utilities.drawTextInDrawMethod(Credit[i], CreditPositions[i], spriteBatch, Main.font, Color.White);
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
                CreditPositions[i] = new Vector2(Main._graphics.PreferredBackBufferWidth / 2, (i + 1) * 50);
            }
        }
    }
}
