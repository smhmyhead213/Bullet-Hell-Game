using bullethellwhatever.BaseClasses;

using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static bullethellwhatever.MainFiles.GameState;
using bullethellwhatever.DrawCode.UI.Buttons;
using Microsoft.Xna.Framework.Input;
using Microsoft.VisualBasic.Logging;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.UtilitySystems.Dialogue;
using SharpDX.DirectWrite;
using log4net.Core;

namespace bullethellwhatever.DrawCode.UI
{
    public static class UI
    {
        public static void CreateTitleScreenMenu()
        {
            Menu titleMenu = new Menu("box", new Vector2(GameWidth, GameHeight), Utilities.CentreOfScreen());

            titleMenu.SetOpacity(0f);

            float titleElementsStartOffsetToRightOfScreen = 300f;
            float titleBannerEndOffsetToRightOfScreen = -350f;
            float[] titleElementEndOffsetToRightOfScreen = new float[3]
                { -300f,
                  -300f,
                  -300f };

            float titleBannerY = 200f;
            float yDistanceBetweenButtonsOnMainMenu = 200f;
            float timeToFadeIn = 60;

            InactiveElement titleBanner = new InactiveElement("TitleBanner", 1f);

            UIElement[] buttons = new UIElement[]
            {
                new UIElement("PlayButton", 1.5f),
                new UIElement("SettingsButton", 1.5f),
                new UIElement("CreditsButton", 1.5f),
            };

            Action[] actions = new Action[]
            {
                new Action(() => // start button action
                {
                    SetGameState(GameStates.BossSelect);
                    titleMenu.Remove();
                }),

                new Action(() => // settings button action
                {
                    SetGameState(GameStates.Settings);
                    titleMenu.Remove();
                }),

                new Action(() => // credits button action
                {
                    SetGameState(GameStates.Credits);
                    titleMenu.Remove();
                }),
            };

            titleBanner.AddToMenu(titleMenu);

            titleBanner.SetPositionInMenu(new Vector2(GameWidth + titleElementsStartOffsetToRightOfScreen, titleBannerY));

            titleBanner.SetExtraAI(new Action(() =>
            {
                float progress = MathHelper.Clamp(EasingFunctions.EaseOutElastic(titleBanner.AITimer / timeToFadeIn), 0f, 2f);

                titleBanner.Position.X = GameWidth + MathHelper.Lerp(titleElementsStartOffsetToRightOfScreen, titleBannerEndOffsetToRightOfScreen, progress);
            }));

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].AddToMenu(titleMenu);

                buttons[i].SetPositionInMenu(new Vector2(GameWidth + titleElementsStartOffsetToRightOfScreen, titleBannerY + (i + 1) * yDistanceBetweenButtonsOnMainMenu));

                buttons[i].Update();

                buttons[i].SetClickEvent(actions[i]);
            }

            int delayBetweenButtonMovements = 30;

            for (int i = 0; i < buttons.Length; i++)
            {
                int locali = i;

                buttons[i].SetExtraAI(new Action(() =>
                {
                    float timeToUse = MathHelper.Clamp(buttons[locali].AITimer - (locali + 1) * delayBetweenButtonMovements, 0f, timeToFadeIn); // clamp the time value to use to go no less than 0, the easing function does not respond well to inputs < 0

                    float progress = EasingFunctions.EaseOutElastic(timeToUse / timeToFadeIn);

                    //due to the use of idealscreen width, this will break the buttons position if the menu is moved. change GameWidth to right side of main menu if this needs fixed.

                    buttons[locali].PositionInMenu.X = GameWidth + MathHelper.Lerp(titleElementsStartOffsetToRightOfScreen, titleElementEndOffsetToRightOfScreen[locali], progress);
                }));
            }

            titleMenu.Display();
        }

        public static void CreateBossSelectMenu()
        {
            Menu bossSelectMenu = new Menu("box", new Vector2(GameWidth, GameHeight), Utilities.CentreOfScreen());

            bossSelectMenu.SetOpacity(0f);

            BackButton backButton = new BackButton("Back", new Vector2(150, 60));

            backButton.SetClickEvent(new Action(() =>
            {
                bossSelectMenu.Remove();
            }));

            backButton.AddToMenu(bossSelectMenu);

            backButton.SetPositionInMenu(new Vector2(bossSelectMenu.Width() / 5, bossSelectMenu.Height() / 5));

            UIElement[] mainButtons = new UIElement[] // the buttons all in a row
            {
                new UIElement("BossButton", 3f),
                new UIElement("CrabBoss", 0.5f),
                new UIElement("EyeBoss", 0.5f),
            };

            for (int i = 0; i < mainButtons.Length; i++)
            {
                mainButtons[i].AddToMenu(bossSelectMenu);

                mainButtons[i].SetPositionInMenu(new Vector2(bossSelectMenu.Width() / (mainButtons.Length + 1) * (i + 1), bossSelectMenu.Height() / 2f));

                mainButtons[i].Update();

                int locali = i;

                Action startButtonAction = new Action(() =>
                {
                    SetGameState(GameStates.DifficultySelect);

                    GameState.Boss = (GameState.Bosses)locali;

                    bossSelectMenu.Remove();
                });

                mainButtons[i].SetClickEvent(startButtonAction);
            }

            bossSelectMenu.Display();
        }
        public static void CreateDifficultySelectMenu()
        {
            Menu difficultySelectMenu = new Menu("box", new Vector2(GameWidth, GameHeight), Utilities.CentreOfScreen());

            difficultySelectMenu.SetOpacity(0f);

            BackButton backButton = new BackButton("Back", new Vector2(150, 60));

            backButton.SetClickEvent(new Action(() =>
            {
                difficultySelectMenu.Remove();
            }));

            backButton.AddToMenu(difficultySelectMenu);

            backButton.SetPositionInMenu(new Vector2(difficultySelectMenu.Width() / 5, difficultySelectMenu.Height() / 5));

            string[] buttonTexturesToDraw = { "EasyButton", "NormalButton", "HardButton", "InsaneButton" };

            UIElement[] mainButtons = new UIElement[buttonTexturesToDraw.Length]; // the buttons all in a row

            for (int i = 0; i < mainButtons.Length; i++)
            {
                mainButtons[i] = new UIElement(buttonTexturesToDraw[i], new Vector2(150, 60));

                difficultySelectMenu.AddUIElement(mainButtons[i]);

                mainButtons[i].SetPositionInMenu(new Vector2(difficultySelectMenu.Width() / (mainButtons.Length + 1) * (i + 1), difficultySelectMenu.Height() / 2f));

                mainButtons[i].Update();

                int locali = i;

                Action startButtonAction = new Action(() =>
                {
                    SetGameState(GameStates.InGame);
                    GameState.Difficulty = (Difficulties)locali;
                    EntityManager.SpawnBoss();
                    difficultySelectMenu.Remove();
                });

                mainButtons[i].SetClickEvent(startButtonAction);
            }

            difficultySelectMenu.Display();
        }
        public static void CreateSettingsMenu()
        {
            Menu settingsMenu = new Menu("box", new Vector2(GameWidth, GameHeight), Utilities.CentreOfScreen());

            settingsMenu.SetOpacity(0f);

            BackButton backButton = new BackButton("Back", new Vector2(150, 60));

            backButton.SetClickEvent(new Action(() =>
            {
                settingsMenu.Remove();
                DialogueSystem.ClearDialogues();
            }));

            backButton.AddToMenu(settingsMenu);

            backButton.SetPositionInMenu(new Vector2(settingsMenu.Width() / 5, settingsMenu.Height() / 5));

            //Button fullScreenButton = new Button("StartButton", 3f);

            //fullScreenButton.AddToMenu(settingsMenu);
            //fullScreenButton.PositionInMenu = new Vector2(GameWidth / 2f, GameHeight / 3f);
            //fullScreenButton.SetClickEvent(new Action(() =>
            //{
            //    _graphics.IsFullScreen = !_graphics.IsFullScreen;
            //}));

            UIElement[] mainButtons = new UIElement[]
            {
                new UIElement("NumberKeysOff", 0.5f),
                new UIElement("ScrollWheelOff", 0.5f),
            };

            for (int i = 0; i < mainButtons.Length; i++)
            {
                mainButtons[i].AddToMenu(settingsMenu);

                mainButtons[i].SetPositionInMenu(new Vector2(settingsMenu.Width() / (mainButtons.Length + 1) * (i + 1), settingsMenu.Height() / 2f));

                mainButtons[i].Update();

                int locali = i;

                Action startButtonAction = new Action(() =>
                {
                    ChangeWeaponSwitchControl(locali == 0 ? WeaponSwitchControls.NumberKeys : WeaponSwitchControls.ScrollWheel); // ??
                });

                mainButtons[i].SetClickEvent(startButtonAction);
            }

            float scaleFactorForSelected = 1.4f;

            string[] selectedTextures = new string[] {
                "NumberKeysOn",
                "ScrollWheelOn"
            };

            string[] notSelectedTextures = new string[] {
                "NumberKeysOff",
                "ScrollWheelOff"
            };

            for (int i = 0; i < mainButtons.Length; i++)
            {
                int locali = i;

                mainButtons[locali].SetExtraAI(new Action(() =>
                {
                    if (WeaponSwitchControl == (WeaponSwitchControls)locali)
                    {
                        mainButtons[locali].Texture = AssetRegistry.GetTexture2D(selectedTextures[locali]);
                        mainButtons[locali].Size = mainButtons[locali].InitialSize * scaleFactorForSelected;
                    }
                    else
                    {
                        mainButtons[locali].Texture = AssetRegistry.GetTexture2D(notSelectedTextures[locali]);
                        mainButtons[locali].Size = mainButtons[locali].InitialSize;
                    }
                }));
            }

            settingsMenu.Display();
        }

        public static void CreateCreditsMenu()
        {
            BackButton backButton = new BackButton("Back", new Vector2(150, 60), new Vector2(GameWidth / 5f, GameHeight / 5f));

            backButton.SetClickEvent(new Action(() =>
            {
                backButton.Remove();
            }));

            backButton.Display();
        }

        public static void CreatePauseMenu()
        {
            UIManager.ResetAllSelections();

            Menu pauseMenu = new Menu("MenuBG", new Vector2(GameWidth / 3, GameHeight / 6), Utilities.CentreOfScreen());

            pauseMenu.Name = "PauseMenu";

            pauseMenu.SetDraggable(true);
            pauseMenu.SetImportant(true);

            ExitButton exitGame = new ExitButton("ExitButton", new Vector2(150, 60));

            exitGame.AddToMenu(pauseMenu);

            exitGame.SetPositionInMenu(new Vector2(pauseMenu.Width() / 3f, pauseMenu.Height() / 2f));

            MainMenuButton mainMenuButton = new MainMenuButton("MainMenuButton", new Vector2(150, 60));

            mainMenuButton.AddToMenu(pauseMenu);

            mainMenuButton.SetPositionInMenu(new Vector2(pauseMenu.Width() / 3f * 2f, pauseMenu.Height() / 2f));

            pauseMenu.SetExtraAI(new Action(() =>
            {
                if (IsKeyPressedAndWasntLastFrame(Keys.Escape))
                {
                    pauseMenu.Remove();
                }
            }));

            pauseMenu.Display();
        }

        public static void CreateAfterBossMenu()
        {
            MainMenuButton mainMenuButton = new MainMenuButton("MainMenuButton", new Vector2(150, 60));

            mainMenuButton.Position = Utilities.CentreOfScreen() - new Vector2(GameWidth / 5f, 0);

            mainMenuButton.Display();

            UIElement playButton = new UIElement("PlayButton", new Vector2(150, 60));

            playButton.Position = Utilities.CentreOfScreen() + new Vector2(GameWidth / 5f, 0);

            playButton.SetClickEvent(new Action(() =>
            {
                Utilities.InitialiseGame();
                player.FullHeal();
                EntityManager.SpawnBoss();

                playButton.Remove();
                mainMenuButton.Remove();
            }));

            playButton.Display();
        }
    }
}
