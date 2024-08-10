using bullethellwhatever.BaseClasses;

using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using static bullethellwhatever.MainFiles.GameState;
using bullethellwhatever.DrawCode.UI.Buttons;
using Microsoft.Xna.Framework.Input;
using Microsoft.VisualBasic.Logging;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.UtilitySystems;

namespace bullethellwhatever.DrawCode.UI
{
    public static class UI
    {
        public static void CreateTitleScreenMenu()
        {
            Menu titleMenu = new Menu("box", new Vector2(IdealScreenWidth, IdealScreenHeight), Utilities.CentreOfScreen());

            titleMenu.SetOpacity(0f);

            float titleElementsStartOffsetToRightOfScreen = 300f;
            float titleBannerEndOffsetToRightOfScreen = -300f;
            float[] titleElementEndOffsetToRightOfScreen = new float[3]
                { -250f,
                  -225f,
                  -225f };

            float titleBannerY = 200f;
            float yDistanceBetweenButtonsOnMainMenu = 200f;
            float timeToFadeIn = 60;

            InactiveElement titleBanner = new InactiveElement("TitleBanner", 1f);

            UIElement[] buttons = new UIElement[]
            {
                new Button("PlayButton", 1.5f),
                new Button("SettingsButton", 1.5f),
                new Button("CreditsButton", 1.5f),
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
                }),

                new Action(() => // credits button action
                {
                    SetGameState(GameStates.Credits);
                    titleMenu.Hide();
                }),
            };

            titleBanner.AddToMenu(titleMenu);

            titleBanner.SetPositionInMenu(new Vector2(IdealScreenWidth + titleElementsStartOffsetToRightOfScreen, titleBannerY));

            titleBanner.SetExtraAI(new Action(() =>
            {
                float progress = MathHelper.Clamp(EasingFunctions.EaseOutElastic(titleBanner.AITimer / timeToFadeIn), 0f, 2f);

                titleBanner.Position.X = IdealScreenWidth + MathHelper.Lerp(titleElementsStartOffsetToRightOfScreen, titleBannerEndOffsetToRightOfScreen, progress);
            }));

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].AddToMenu(titleMenu);

                buttons[i].SetPositionInMenu(new Vector2(IdealScreenWidth + titleElementsStartOffsetToRightOfScreen, titleBannerY + (i + 1) * yDistanceBetweenButtonsOnMainMenu));

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

                    //due to the use of idealscreen width, this will break the buttons position if the menu is moved. change idealscreenwidth to right side of main menu if this needs fixed.

                    buttons[locali].PositionInMenu.X = IdealScreenWidth + MathHelper.Lerp(titleElementsStartOffsetToRightOfScreen, titleElementEndOffsetToRightOfScreen[locali], progress);
                }));
            }

            titleMenu.Display();
        }

        public static void CreateBossSelectMenu()
        {
            Menu bossSelectMenu = new Menu("box", new Vector2(IdealScreenWidth, IdealScreenHeight), Utilities.CentreOfScreen());

            bossSelectMenu.SetOpacity(0f);

            Button[] mainButtons = new Button[] // the buttons all in a row
            {
                new Button("BossButton", new Vector2(150, 60)),
                new Button("BossButton", new Vector2(150, 60)),
                new Button("BossButton", new Vector2(150, 60)),
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

                    bossSelectMenu.Hide();
                    //CreateDifficultySelectMenu();
                });

                mainButtons[i].SetClickEvent(startButtonAction);
            }

            BackButton backButton = new BackButton("Back", new Vector2(150, 60));

            backButton.SetClickEvent(new Action(() =>
            {
                bossSelectMenu.Hide();
            }));

            backButton.AddToMenu(bossSelectMenu);

            backButton.SetPositionInMenu(new Vector2(bossSelectMenu.Width() / 5, bossSelectMenu.Height() / 5));

            bossSelectMenu.Display();
        }
        public static void CreateDifficultySelectMenu()
        {
            Menu difficultySelectMenu = new Menu("box", new Vector2(IdealScreenWidth, IdealScreenHeight), Utilities.CentreOfScreen());

            difficultySelectMenu.SetOpacity(0f);

            string[] buttonTexturesToDraw = { "EasyButton", "NormalButton", "HardButton", "InsaneButton" };

            Button[] mainButtons = new Button[buttonTexturesToDraw.Length]; // the buttons all in a row

            for (int i = 0; i < mainButtons.Length; i++)
            {
                mainButtons[i] = new Button(buttonTexturesToDraw[i], new Vector2(150, 60));

                difficultySelectMenu.AddUIElement(mainButtons[i]);

                mainButtons[i].SetPositionInMenu(new Vector2(difficultySelectMenu.Width() / (mainButtons.Length + 1) * (i + 1), difficultySelectMenu.Height() / 2f));

                mainButtons[i].Update();

                int locali = i;

                Action startButtonAction = new Action(() =>
                {
                    SetGameState(GameStates.InGame);
                    GameState.Difficulty = (Difficulties)locali;
                    EntityManager.SpawnBoss();
                    difficultySelectMenu.Hide();
                });

                mainButtons[i].SetClickEvent(startButtonAction);
            }

            BackButton backButton = new BackButton("Back", new Vector2(150, 60));

            backButton.SetClickEvent(new Action(() =>
            {
                difficultySelectMenu.Hide();
            }));

            backButton.AddToMenu(difficultySelectMenu);

            backButton.SetPositionInMenu(new Vector2(difficultySelectMenu.Width() / 5, difficultySelectMenu.Height() / 5));

            difficultySelectMenu.Display();
        }
        public static void CreateSettingsMenu()
        {
            Menu settingsMenu = new Menu("box", new Vector2(IdealScreenWidth, IdealScreenHeight), Utilities.CentreOfScreen());

            settingsMenu.SetOpacity(0f);

            Button[] mainButtons = new Button[]
            {
                new Button("NumberKeys", new Vector2(150, 60)),
                new Button("Scroll", new Vector2(150, 60))
            };

            for (int i = 0; i < mainButtons.Length; i++)
            {
                mainButtons[i].AddToMenu(settingsMenu);

                mainButtons[i].SetPositionInMenu(new Vector2(settingsMenu.Width() / (mainButtons.Length + 1) * (i + 1), settingsMenu.Height() / 2f));

                mainButtons[i].Update();

                int locali = i;

                Action startButtonAction = new Action(() =>
                {
                    ChangeWeaponSwitchControl(locali == 0 ? false : true); // ??
                    SetGameState(GameStates.Settings);
                    settingsMenu.Hide();
                });

                mainButtons[i].SetClickEvent(startButtonAction);
            }

            BackButton backButton = new BackButton("Back", new Vector2(150, 60));

            backButton.SetClickEvent(new Action(() =>
            {
                settingsMenu.Hide();
            }));

            backButton.AddToMenu(settingsMenu);

            backButton.SetPositionInMenu(new Vector2(settingsMenu.Width() / 5, settingsMenu.Height() / 5));

            settingsMenu.Display();
        }

        public static void CreateCreditsMenu()
        {
            BackButton backButton = new BackButton("Back", new Vector2(150, 60), new Vector2(IdealScreenWidth / 5f, IdealScreenHeight / 5f));

            backButton.SetClickEvent(new Action(() =>
            {
                backButton.Remove();
            }));

            backButton.AddToActiveUIElements();
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

            Texture2D texture = AssetRegistry.GetTexture2D("box");

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

            //Utilities.drawTextInDrawMethod(
            //    $"IntPtr size: {IntPtr.Size}", new Vector2(ScreenWidth / 2, ScreenHeight / 5 * 2), _spriteBatch, font, Color.White); what was this?
        }
    }
}
