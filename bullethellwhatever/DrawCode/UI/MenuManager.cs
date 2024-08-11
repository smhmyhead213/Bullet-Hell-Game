using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using bullethellwhatever.DrawCode.UI.Buttons;
using bullethellwhatever.MainFiles;

namespace bullethellwhatever.DrawCode.UI
{
    public static class MenuManager
    {
        public static int MenuCooldown;
        public static int MenuCooldownTimer;

        public static Menu PauseMenu;
        public static void Initialise()
        {
            MenuCooldown = 10;
            MenuCooldownTimer = MenuCooldown;

            PauseMenu = new Menu("MenuBG", new Vector2(IdealScreenWidth / 3, IdealScreenHeight / 6), Utilities.CentreOfScreen());
        }
        public static void ManageMenus()
        {
            if (MenuCooldownTimer > 0)
            {
                MenuCooldownTimer--;
            }

            if (IsKeyPressed(Keys.Escape) && MenuCooldownTimer == 0)
            {
                if (!PauseMenu.IsDisplayed())
                {
                    PauseMenu.SetDraggable(true);
                    PauseMenu.SetImportant(true);

                    ExitButton exitGame = new ExitButton("ExitButton", new Vector2(150, 60));

                    exitGame.AddToMenu(PauseMenu);

                    exitGame.SetPositionInMenu(new Vector2(PauseMenu.Width() / 3f, PauseMenu.Height() / 2f));

                    Button mainMenuButton = new Button("StartButton", 3f);

                    mainMenuButton.AddToMenu(PauseMenu);

                    mainMenuButton.SetPositionInMenu(new Vector2(PauseMenu.Width() / 3f * 2f, PauseMenu.Height() / 2f));

                    mainMenuButton.SetClickEvent(new Action(() =>
                    {
                        UIManager.ClearUI();
                        EntityManager.Clear();
                        GameState.SetGameState(GameState.GameStates.TitleScreen);
                    }));

                    PauseMenu.Display();
                }

                else
                {
                    PauseMenu.Remove();
                }

                MenuCooldownTimer = MenuCooldown;
            }
        }
    }
}
