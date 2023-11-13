using bullethellwhatever.BaseClasses;

using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using static bullethellwhatever.MainFiles.GameState;
using bullethellwhatever.DrawCode.UI.Buttons;
using Microsoft.Xna.Framework.Input;

namespace bullethellwhatever.DrawCode.UI
{
    public static class UI
    {
        public static void CreateTitleScreenMenu()
        {
            Menu titleMenu = new Menu(Utilities.CentreOfScreen(), ScreenWidth, ScreenHeight, Assets["box"]);

            Button[] buttons = new Button[]
            {
                new Button(Assets["StartButton"], new Vector2(3, 3), titleMenu),
                new Button(Assets["SettingsButton"], new Vector2(3, 3), titleMenu),
                new Button(Assets["StartButton"], new Vector2(3, 3), titleMenu),
            };

            Action[] actions = new Action[]
            {
                new Action(() => // start button action
                {
                    SetGameState(GameStates.BossSelect);
                    titleMenu.Hide();
                }),

                new Action(() => // settings button action
                {
                    SetGameState(GameStates.Settings);
                    titleMenu.Hide();
                    //CreateSettingsMenu();
                }),

                new Action(() => // credits button action
                {
                    SetGameState(GameStates.Credits);
                    titleMenu.Hide();
                    //CreateCreditsMenu();
                }),
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].SetPositionInMenu(new Vector2(titleMenu.Width() / (buttons.Length + 1) * (i + 1), titleMenu.Height() / 2f));

                buttons[i].Update();

                buttons[i].SetClickEvent(actions[i]);

                titleMenu.AddUIElement(buttons[i]);
            }

            titleMenu.Display();
        }

        public static void CreateBossSelectMenu()
        {
            Menu bossSelectMenu = new Menu(Utilities.CentreOfScreen(), ScreenWidth, ScreenHeight, Assets["box"]);

            Button[] mainButtons = new Button[] // the buttons all in a row
            {
                new Button(Assets["BossButton"], new Vector2(3, 3), bossSelectMenu),
                new Button(Assets["BossButton"], new Vector2(3, 3), bossSelectMenu),
                new Button(Assets["BossButton"], new Vector2(3, 3), bossSelectMenu)
            };

            for (int i = 0; i < mainButtons.Length; i++)
            {
                mainButtons[i].SetPositionInMenu(new Vector2(bossSelectMenu.Width() / (mainButtons.Length + 1) * (i + 1), bossSelectMenu.Height() / 2f));

                mainButtons[i].Update();

                int locali = i;

                Action startButtonAction = new Action(() =>
                {
                    SetGameState(GameStates.DifficultySelect);

                    GameState.Boss = (GameState.Bosses)locali;

                    bossSelectMenu.Hide();
                    //CreateDifficultySelectMenu();
                });

                mainButtons[i].SetClickEvent(startButtonAction);

                bossSelectMenu.AddUIElement(mainButtons[i]);
            }

            BackButton backButton = new BackButton(Assets["Back"], new Vector2(3, 3), bossSelectMenu);

            backButton.SetPositionInMenu(new Vector2(bossSelectMenu.Width() / 5, bossSelectMenu.Height() / 5));

            bossSelectMenu.AddUIElement(backButton);

            bossSelectMenu.Display();
        }
        public static void CreateDifficultySelectMenu()
        {
            Menu difficultySelectMenu = new Menu(Utilities.CentreOfScreen(), ScreenWidth, ScreenHeight, Assets["box"]);

            string[] buttonTexturesToDraw = { "EasyButton", "NormalButton", "HardButton", "InsaneButton" };

            Button[] mainButtons = new Button[buttonTexturesToDraw.Length]; // the buttons all in a row

            for (int i = 0; i < mainButtons.Length; i++)
            {
                mainButtons[i] = new Button(buttonTexturesToDraw[i], Vector2.One * 3f, difficultySelectMenu);

                mainButtons[i].SetPositionInMenu(new Vector2(difficultySelectMenu.Width() / (mainButtons.Length + 1) * (i + 1), difficultySelectMenu.Height() / 2f));

                mainButtons[i].Update();

                int locali = i;

                Action startButtonAction = new Action(() =>
                {
                    SetGameState(GameStates.InGame);
                    GameState.Difficulty = (Difficulties)locali;
                    difficultySelectMenu.Hide();
                });

                mainButtons[i].SetClickEvent(startButtonAction);

                difficultySelectMenu.AddUIElement(mainButtons[i]);
            }


            BackButton backButton = new BackButton(Assets["Back"], new Vector2(3, 3), difficultySelectMenu);

            backButton.SetPositionInMenu(new Vector2(difficultySelectMenu.Width() / 5, difficultySelectMenu.Height() / 5));

            difficultySelectMenu.AddUIElement(backButton);

            difficultySelectMenu.Display();
        }
        public static void CreateSettingsMenu()
        {
            Menu settingsMenu = new Menu(Utilities.CentreOfScreen(), ScreenWidth, ScreenHeight, Assets["box"]);

            Button[] mainButtons = new Button[]
            {
                new Button("NumberKeys", new Vector2(3, 3), settingsMenu),
                new Button("Scroll", new Vector2(3, 3), settingsMenu),
            };

            for (int i = 0; i < mainButtons.Length; i++)
            {
                mainButtons[i].SetPositionInMenu(new Vector2(settingsMenu.Width() / (mainButtons.Length + 1) * (i + 1), settingsMenu.Height() / 2f));

                mainButtons[i].Update();

                Action startButtonAction = new Action(() =>
                {
                    WeaponSwitchControl = i == 0 ? false : true;
                    Drawing.ConfirmControlSettingsChange(_spriteBatch);
                    settingsMenu.Hide();
                });

                mainButtons[i].SetClickEvent(startButtonAction);

                settingsMenu.AddUIElement(mainButtons[i]);
            }

            BackButton backButton = new BackButton(Assets["Back"], new Vector2(3, 3), settingsMenu);

            backButton.SetPositionInMenu(new Vector2(settingsMenu.Width() / 5, settingsMenu.Height() / 5));

            settingsMenu.AddUIElement(backButton);

            settingsMenu.Display();
        }

        public static void CreateCreditsMenu()
        {
            BackButton backButton = new BackButton(new Vector2(ScreenWidth / 5f, ScreenHeight / 5f), "Back", new Vector2(3, 3));

            backButton.StandaloneUIElement();
        }

        public static void DrawMenus(SpriteBatch spriteBatch)
        {
            foreach (Menu menu in activeMenus)
            {
                menu.Draw(spriteBatch);
            }

        }
        public static void DrawHealthBar(SpriteBatch _spriteBatch, Entity entityToDrawHPBarFor, Vector2 positionOfBar, float BarWidth, float BarHeight) //bar width and height are SCALE FACTORS DO NOT FORGET
        {
            float healthRatio = entityToDrawHPBarFor.Health / entityToDrawHPBarFor.MaxHP;

            float emptySpaceOnLeft = BarWidth * (1 - healthRatio) / 2;

            Texture2D texture = Assets["box"];

            RotatedRectangle HPBar = new RotatedRectangle(0, BarWidth * texture.Width, BarHeight * texture.Height, positionOfBar, entityToDrawHPBarFor);
            HPBar.UpdateVertices();

            float opacity = HPBar.Intersects(player.Hitbox) ? 0.2f : 1f;

            //HP bar background.
            if (entityToDrawHPBarFor is not Player)
            {
                _spriteBatch.Draw(player.Texture, positionOfBar, null, Color.LimeGreen * opacity, 0f, new Vector2(player.Texture.Width / 2, player.Texture.Height / 2), new Vector2(BarWidth, BarHeight), SpriteEffects.None, 0f);

                _spriteBatch.Draw(player.Texture, new Vector2(positionOfBar.X - emptySpaceOnLeft * player.Texture.Width, positionOfBar.Y), null, Color.Red * opacity, 0f, new Vector2(player.Texture.Width / 2, player.Texture.Height / 2), new Vector2(BarWidth * healthRatio, BarHeight), SpriteEffects.None, 0f);
            }

            else
            {
                Drawing.BetterDraw(player.Texture, positionOfBar, null, Color.Red * opacity, 0f, new Vector2(BarWidth, BarHeight), SpriteEffects.None, 0f);

                Drawing.BetterDraw(player.Texture, new Vector2(positionOfBar.X - emptySpaceOnLeft * player.Texture.Width, positionOfBar.Y), null, Color.LimeGreen * opacity, 0f, new Vector2(BarWidth * healthRatio, BarHeight), SpriteEffects.None, 0f);

            }
        }

        public static void DrawTitleScreen(SpriteBatch _spriteBatch)
        {
            Utilities.drawTextInDrawMethod(
                "Controls: WASD to move, Left Click or Enter to shoot, Space to dash, Shift for precision.", new Vector2(ScreenWidth / 2, ScreenHeight / 5 * 2), _spriteBatch, font, Color.White);

        }
    }
}
