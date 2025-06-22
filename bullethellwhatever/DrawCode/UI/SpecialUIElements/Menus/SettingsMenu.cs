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
            MarginX = 30f;
            MarginY = 30f;

            CreateColumns();

            //float menuWidth = buttonWidth + 2 * marginX;

            //MarginY = 50f;
            //float menuHeight = Height() - 2 * MarginY;

            

            //sidebar.SetOpacity(1f);

            //sidebar.StartMenuBuilder(marginX, marginY, 0);

            //string[] words = "this is good news. we can finally be bees. this isn't your world, but we can be bees. this is good news. you can be a bee. you'll live like a bee. a pet. a pet? a pet, mark. this is good news. you'll live for 30 years. this is insane!".Split(" ");

            //for (int i = 0; i < words.Length; i++)
            //{
            //    TextButton test = new TextButton(words[i], 20, 20, new Vector2(buttonWidth, buttonHeight), Vector2.Zero);
            //    test.Interactable = false;
            //    sidebar.AddUIElementAuto(test);
            //}

            //sidebar.TotalButtonHeight = sidebar.CalculateTotalHeight(); // no plans to add more buttons after creation of menu for now. if this is done, this may need to be updated on button add

            //float sidebarIndentFromLeft = 50f;
            
            //sidebar.PositionInMenu = new Vector2(sidebar.Width() / 2f + sidebarIndentFromLeft, Height() / 2f);
            //AddUIElement(sidebar);
            //sidebar.Display();
        }

        public void CreateColumns()
        {
            float sectionColumnWidth = GameWidth * 0.35f; // the width of the column containing the shortcut button to each section
            float optionsColumnWidth = Width() - 2 * MarginX - sectionColumnWidth; // the width of the column containing the actual settings
            
            float headingHeight = 200f;
            float sectionColumnMarginX = 10f;
            float sectionColumnPaddingY = 10f;
            float sectionButtonsTextMarginX = 20f;
            float sectionButtonsTextMarginY = 20f;

            float textScale = 4f;

            TextButton settingsHeading = new TextButton("box", "SETTINGS", 20f, 10f, new Vector2(sectionColumnWidth - 2 * sectionColumnMarginX, headingHeight), Vector2.Zero);
            settingsHeading.Interactable = false;
            
            string[] sections = ["AUDIO", "CONTROLS", "DISPLAY", "MISC"];

            float longestStringLength = Utilities.LongestStringByPixelLength(sections, font);
            float tallestStringHeight = Utilities.TallestStringByPixelLength(sections, font);
            //float buttonWidth = sectionColumnWidth * 0.5f;
            float buttonWidth = longestStringLength * textScale + 2 * sectionButtonsTextMarginX;
            float buttonHeight = tallestStringHeight * textScale + 2 * sectionButtonsTextMarginX;

            float sidebarHeight = headingHeight + buttonHeight * sections.Length + 2 * sectionColumnPaddingY;
            sidebarHeight = GameHeight;

            Menu sidebar = new Menu("box", new Vector2(sectionColumnWidth, sidebarHeight), Vector2.Zero);
            sidebar.StartMenuBuilder(sectionColumnMarginX, sectionColumnPaddingY, 0f);
            bool addedHeading = sidebar.AddUIElementAuto(settingsHeading);

            float leftHandEmptySpace = sectionColumnWidth - buttonWidth - 2 * sectionColumnMarginX - 1; // subtract one because of floating point imprecision when theres exactly enough space
            float paddingBetweenButtons = 40f;

            for (int i = 0; i < sections.Length; i++)
            {
                EmptySpace emptySpaceLeft = new EmptySpace(new Vector2(leftHandEmptySpace, buttonHeight), Vector2.Zero);

                sidebar.AddUIElementAuto(emptySpaceLeft);

                TextButton sectionButton = new TextButton("ButtonBlankRound", sections[i], sectionButtonsTextMarginX, sectionButtonsTextMarginY, new Vector2(buttonWidth, buttonHeight), Vector2.Zero);

                sectionButton.ScaleTextToFit = false;
                sectionButton.TextScale = new Vector2(textScale);
                sectionButton.Colour = Color.White;
                sectionButton.TextMarginX = sectionButtonsTextMarginX;
                sectionButton.CentreTextVertically();
                sectionButton.RightAlignText();

                sidebar.AddUIElementAuto(sectionButton);

                EmptySpace padding = new EmptySpace(new Vector2(sidebar.AvailableWidth(), paddingBetweenButtons), Vector2.Zero);
                sidebar.AddUIElementAuto(padding);
            }

            StartMenuBuilder(MarginX, MarginY, 0f);
            AddUIElementAuto(sidebar);
            //sidebar.Display();
        }
    }
}
