﻿using bullethellwhatever.BaseClasses;

using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using static bullethellwhatever.MainFiles.GameState;
using bullethellwhatever.DrawCode.UI.Buttons;
using Microsoft.Xna.Framework.Input;
using Microsoft.VisualBasic.Logging;
using bullethellwhatever.BaseClasses.Hitboxes;

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
                    SetGameState(GameStates.Settings);
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
            BackButton backButton = new BackButton("Back", new Vector2(3, 3), null, new Vector2(ScreenWidth / 5f, ScreenHeight / 5f));

            backButton.SetClickEvent(new Action(() =>
            {
                SetGameState(GameStates.TitleScreen);
            }));

            backButton.StandaloneUIElement();
        }

        public static void DrawMenus(SpriteBatch spriteBatch)
        {
            Drawing.RestartSpriteBatchForUI(spriteBatch);

            foreach (Menu menu in activeMenus)
            {
                menu.Draw(spriteBatch);
            }

            // no need to restart again after this because menus are the last thing drawn
        }
        public static void DrawHealthBar(SpriteBatch _spriteBatch, Entity entityToDrawHPBarFor, Vector2 positionOfBar, float BarWidth, float BarHeight)
        {
            float healthRatio = entityToDrawHPBarFor.Health / entityToDrawHPBarFor.MaxHP;

            DrawHealthBar(_spriteBatch, healthRatio, positionOfBar, BarWidth, BarHeight);
        }
        public static void DrawHealthBar(SpriteBatch _spriteBatch, float progress, Vector2 positionOfBar, float BarWidth, float BarHeight)
        {
            progress = MathHelper.Clamp(progress, 0f, 1f);

            float emptySpaceOnLeft = (1 - progress) / 2; // fraction of bar width

            Texture2D texture = Assets["box"];

            RotatedRectangle HPBar = new(0, BarWidth, BarHeight, positionOfBar, player);
            HPBar.UpdateVertices();

            float opacity = HPBar.Intersects(player.Hitbox).Collided ? 0.2f : 1f;

            Vector2 size = new Vector2((float)BarWidth / texture.Width, (float)BarHeight / texture.Height);

            //HP bar background.

            _spriteBatch.Draw(texture, positionOfBar, null, Color.LimeGreen * opacity, 0f, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, 0f);

            _spriteBatch.Draw(texture, new Vector2(positionOfBar.X - emptySpaceOnLeft * BarWidth, positionOfBar.Y), null, Color.Red * opacity, 0f, new Vector2(texture.Width / 2, texture.Height / 2), new Vector2(size.X * progress, size.Y), SpriteEffects.None, 0f);
        }
        public static void DrawTitleScreen(SpriteBatch _spriteBatch)
        {
            //Utilities.drawTextInDrawMethod(
            //    "Controls: WASD to move, Left Click or Enter to shoot, Space to dash, Shift for precision.", new Vector2(ScreenWidth / 2, ScreenHeight / 5 * 2), _spriteBatch, font, Color.White);

            Utilities.drawTextInDrawMethod(
                $"IntPtr size: {IntPtr.Size}", new Vector2(ScreenWidth / 2, ScreenHeight / 5 * 2), _spriteBatch, font, Color.White);
        }
    }
}
