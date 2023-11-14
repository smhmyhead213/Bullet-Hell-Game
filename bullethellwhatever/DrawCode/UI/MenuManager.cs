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

        public static bool PauseMenuDisplayed;

        public static Menu PauseMenu;
        public static void Initialise()
        {
            MenuCooldown = 20;
            MenuCooldownTimer = MenuCooldown;
            PauseMenuDisplayed = false;
        }
        public static void ManageMenus()
        {
            if (MenuCooldownTimer > 0)
            {
                MenuCooldownTimer--;
            }

            if (IsKeyPressed(Keys.G) && GameState.State == GameState.GameStates.InGame && MenuCooldownTimer == 0)
            {
                if (!PauseMenuDisplayed)
                {
                    PauseMenu = new Menu(Utilities.CentreOfScreen(), new Vector2(ScreenWidth / 6, ScreenHeight / 6), "MenuBG");

                    PauseMenu.SetDraggable(true);
                    PauseMenu.SetImportant(true);

                    ExitButton exitGame = new ExitButton("StartButton", Vector2.One * 3f, PauseMenu);

                    exitGame.SetPositionInMenu(PauseMenu.RelativeCentreOfMenu());

                    PauseMenu.AddUIElement(exitGame);
                    PauseMenu.Display();

                    PauseMenuDisplayed = true;
                }

                else
                {
                    PauseMenu.Hide();
                    PauseMenuDisplayed = false;
                }

                MenuCooldownTimer = MenuCooldown;
            }
        }
    }
}
