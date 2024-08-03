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

            if (IsKeyPressed(Keys.Escape) && GameState.State == GameState.GameStates.InGame && MenuCooldownTimer == 0)
            {
                if (!PauseMenuDisplayed)
                {
                    PauseMenu = new Menu("MenuBG", new Vector2(IdealScreenWidth / 6, IdealScreenHeight / 6), Utilities.CentreOfScreen());

                    PauseMenu.SetDraggable(true);
                    PauseMenu.SetImportant(true);

                    ExitButton exitGame = new ExitButton("ExitButton", Vector2.One * 3f);

                    exitGame.AddToMenu(PauseMenu);

                    exitGame.SetPositionInMenu(PauseMenu.RelativeCentreOfMenu());

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
