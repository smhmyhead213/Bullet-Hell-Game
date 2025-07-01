using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.DrawCode.Particles;
using bullethellwhatever.MainFiles;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

            int floatInTime = 30;

            float availableSectionColumnWidth = sectionColumnWidth - 2 * sectionColumnMarginX;
            float backButtonWidth = 50f;
            float headingWidth = availableSectionColumnWidth - backButtonWidth;

            UIElement backButton = new UIElement("box", new Vector2(backButtonWidth));
            backButton.SetClickEvent(new Action(() =>
            {
                GameState.SetGameState(GameState.GameStates.TitleScreen);
                Remove();
            }));

            TextButton settingsHeading = new TextButton("box", "SETTINGS", 20f, 10f, new Vector2(headingWidth, headingHeight), Vector2.Zero);
            settingsHeading.Interactable = false;
            settingsHeading.ScaleTextToFit();

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

            sidebar.AddUIElementAuto(backButton);
            sidebar.AddUIElementAuto(settingsHeading);


            float leftHandEmptySpace = sectionColumnWidth - buttonWidth - 2 * sectionColumnMarginX - 1; // subtract one because of floating point imprecision when theres exactly enough space
            float paddingBetweenButtons = 40f;

            for (int i = 0; i < sections.Length; i++)
            {
                EmptySpace emptySpaceLeft = new EmptySpace(new Vector2(leftHandEmptySpace, buttonHeight), Vector2.Zero);

                sidebar.AddUIElementAuto(emptySpaceLeft);

                TextButton sectionButton = new TextButton("ButtonBlankRound", sections[i], sectionButtonsTextMarginX, sectionButtonsTextMarginY, new Vector2(buttonWidth, buttonHeight), Vector2.Zero);

                //sectionButton.ScaleTextToFit = false;
                sectionButton.TextScale = new Vector2(textScale);
                sectionButton.Colour = Color.Black;
                sectionButton.TextMarginX = sectionButtonsTextMarginX;
                sectionButton.CentreTextVertically();
                sectionButton.RightAlignText();

                sectionButton.SetExtraAI(new Action(() =>
                {
                    Vector2 objectivePosition = sectionButton.CalculateActualPostion();
                    //sectionButton.Position = objectivePosition + new Vector2(100f * Sin(sectionButton.AITimer / 10f), 0f);
                }));

                sidebar.AddUIElementAuto(sectionButton);

                EmptySpace padding = new EmptySpace(new Vector2(sidebar.AvailableWidth(), paddingBetweenButtons), Vector2.Zero);
                sidebar.AddUIElementAuto(padding);
            }

            StartMenuBuilder(MarginX, MarginY, 0f);
            AddUIElementAuto(sidebar);

            // -- LEFT SIDEBAR END --
            // -- RIGHT SETTINGS STUFF BEGIN --

            Menu mainSettingsColumn = new Menu("box", new Vector2(optionsColumnWidth, Height() - 2 * MarginY), Vector2.Zero);
            mainSettingsColumn.Colour = Color.Black;
            mainSettingsColumn.AddOutline(Color.White, new Vector2(35f));

            // use the automatic adder to add the settings column, then move it up
            bool addedSettingsColumn = AddUIElementAuto(mainSettingsColumn);
            float settingsMenuMarginX = 30f;
            float settingsMenuMarginY = 30f;

            mainSettingsColumn.StartMenuBuilder(settingsMenuMarginX, settingsMenuMarginY, 0f);
            float availableWidth = mainSettingsColumn.AvailableWidth();

            ScrollingButtonColumn settingsScrollColumn = new ScrollingButtonColumn("box", new Vector2(availableWidth, GameHeight - 2 * MarginY), Vector2.Zero, 5f);
            settingsScrollColumn.StartMenuBuilder(0f, 0f, 0f);

            string[] attributes = ["Master Volume", "SFX Volume", "Music Volume"];
            float sliderHeight = 170f;

            for (int i = 0; i < attributes.Length; i++)
            {
                int locali = i;
                Slider tester = new Slider("box", new Vector2(availableWidth, sliderHeight), Vector2.Zero, 0f, 100f, 50f);
                tester.SliderText = (float val) => $"{attributes[locali]}: {Round(val, 0)}%";
                bool addedSuccessfully = settingsScrollColumn.AddUIElementAuto(tester);
            }

            TextButton resetKeybinds = new TextButton("Reset to default keybinds", 40f, 40f, new Vector2(availableWidth, 200f), Vector2.Zero);

            resetKeybinds.AddOutline(Color.White, new Vector2(10f));
            resetKeybinds.ScaleTextToFit();
            resetKeybinds.CentreText();

            resetKeybinds.SetClickEvent(new Action(() =>
            {
                KeybindMap = DefaultKeybinds();
            }));

            settingsScrollColumn.AddUIElementAuto(resetKeybinds);

            // we are going to add keybind changers in columns instead of rows to keep tab ordering nice.
            // keep track of the first row

            settingsScrollColumn.MoveToNextRow();

            int buttonsPerRow = 2;
            int startingRow = settingsScrollColumn.CurrentRow;

            // filter keybinds that we wont allow changing

            Func<KeyValuePair<string, Keybind>, bool> predicate = (KeyValuePair<string, Keybind> bindPair) => !DisallowedBindings().Contains(bindPair.Value);
            Dictionary<string, Keybind> toDisplay = KeybindMap.Where(predicate).ToDictionary();

            int halfwayPoint = startingRow + (toDisplay.Count - 1) / 2;
            
            foreach (KeyValuePair<string, Keybind> keybind in toDisplay)
            {
                if (settingsScrollColumn.CurrentRow > halfwayPoint)
                {
                    settingsScrollColumn.CurrentRow = startingRow;
                }

                float keybindChangerHeight = 70f;

                KeybindChanger keybindChanger = new KeybindChanger("box", new Vector2(availableWidth / buttonsPerRow, keybindChangerHeight), Vector2.Zero, keybind.Key);
                keybindChanger.Colour = Color.Black;
                settingsScrollColumn.AddUIElementAuto(keybindChanger);
                settingsScrollColumn.MoveToNextRow();
            }

            //Slider testSlider = new Slider("box", new Vector2(availableWidth, 300f), Vector2.Zero, 0f, 100f, 30f);

            //settingsScrollColumn.AddUIElementAuto(testSlider);

            bool finishedProperly = mainSettingsColumn.AddUIElementAuto(settingsScrollColumn);
            settingsScrollColumn.Display();
        }

        public override void Draw(SpriteBatch s)
        {
            base.Draw(s);

            bool debugInfo = false;

            if (debugInfo)
            {
                Drawing.DrawText("Main Interactable Index = " + UIManager.IndexOfInteractable.ToString(), new Vector2(GameWidth / 6f, GameHeight * 0.7f), s, font, Color.White, Vector2.One);
                Drawing.DrawText("Settings Interactable Index = " + IndexOfSelected.ToString(), new Vector2(GameWidth / 6f, GameHeight * 0.8f), s, font, Color.White, Vector2.One);
                Drawing.DrawText("Selected Index = " + UIManager.DeepestFocusedMenu(this).IndexOfSelected.ToString(), new Vector2(GameWidth / 6f, GameHeight * 0.9f), s, font, Color.White, Vector2.One);
            }
        }
    }
}
