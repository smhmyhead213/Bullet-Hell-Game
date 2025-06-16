using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.DrawCode.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.DrawCode.UI.SpecialUIElements.Menus
{
    public class SettingsMenu : Menu
    {
        public SettingsMenu(string bgtexture, Vector2 size, Vector2 position) : base(bgtexture, size, position)
        {

        }

        public void Construct()
        {
            float buttonWidth = 250f;
            float buttonHeight = 100f;
            float marginX = 0f;
            float marginY = 0f;

            float menuWidth = buttonWidth + 2 * marginX;

            MarginY = 50f;
            float menuHeight = Height() - 2 * MarginY;

            ScrollingButtonColumn sidebar = new ScrollingButtonColumn("box", new Vector2(menuWidth, menuHeight), Vector2.Zero, 5f);

            sidebar.SetOpacity(1f);

            sidebar.StartMenuBuilder(marginX, marginY, 0);

            Action clickEvent = new Action(() =>
            {
                int sparks = 30;

                for (int i = 0; i < sparks; i++)
                {
                    CommonParticles.Spark(MousePositionWithCamera(), 40f, 60, Color.Orange);
                }
            });

            string[] words = "this is good news. we can finally be bees. this isn't your world, but we can be bees. this is good news. you can be a bee. you'll live like a bee. a pet. a pet? a pet, mark. this is good news. you'll live for 30 years. this is insane!".Split(" ");

            for (int i = 0; i < words.Length; i++)
            {
                TextButton test = new TextButton(words[i], 20, 20, new Vector2(buttonWidth, buttonHeight), Vector2.Zero);
                test.Interactable = false;
                test.SetClickEvent(clickEvent);
                sidebar.AddUIElementAuto(test);
            }

            sidebar.TotalButtonHeight = sidebar.CalculateTotalHeight(); // no plans to add more buttons after creation of menu for now. if this is done, this may need to be updated on button add

            float sidebarIndentFromLeft = 50f;
            
            sidebar.PositionInMenu = new Vector2(sidebar.Width() / 2f + sidebarIndentFromLeft, Height() / 2f);
            AddUIElement(sidebar);
            sidebar.Display();
        }
    }
}
