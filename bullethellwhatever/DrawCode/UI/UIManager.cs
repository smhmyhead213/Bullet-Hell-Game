using bullethellwhatever.DrawCode.UI.Player;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode.UI
{
    public static class UIManager
    {
        public static List<UIElement> ActiveUIElements;
        public static List<UIElement> UIElemntsToRemoveNextFrame;
        public static List<UIElement> UIElementsToAddNextFrame;

        public static PlayerHUD PlayerHUD;
        /// <summary>
        /// The one menu at a time that can be naviagted using the Tab key.
        /// </summary>
        public static int IndexOfInteractable
        {
            get;
            set;
        }

        public static void Initialise()
        {
            ActiveUIElements = new List<UIElement>();
            UIElemntsToRemoveNextFrame = new List<UIElement>();
            UIElementsToAddNextFrame = new List<UIElement>();

            PlayerHUD = new PlayerHUD("HUDBody", new Vector2(260, 128), new Vector2(GameWidth / 10f, GameHeight / 10f));
            PlayerHUD.Display();
        }
        public static void ManageUI()
        {          
            foreach (UIElement element in UIElemntsToRemoveNextFrame)
            {
                ActiveUIElements.Remove(element);
            }

            UIElemntsToRemoveNextFrame.Clear();

            // safety check to ensure that after menus are removed, the interactable index doesnt go over capacity

            if (IndexOfInteractable > ActiveUIElements.Count - 1)
            {
                IndexOfInteractable = -1;
            }

            foreach (UIElement element in UIElementsToAddNextFrame)
            {
                if (element is Menu) // if we add a menu, give it selection priority
                {
                    if (ActiveUIElements.Count != 0 && IndexOfInteractable != -1) // dont try to take selection priority when there is nothing to take from or if nothing is focused
                    {
                        if (ActiveUIElements[IndexOfInteractable] is Menu currentMenu) // if we were focused on a menu, unfocus on its focused element
                        {
                            Menu focusedDeepMenu = DeepestFocusedMenu(currentMenu);

                            if (focusedDeepMenu is not null)
                                focusedDeepMenu.IndexOfSelected = -1; // unfocus
                        }
                    }

                    IndexOfInteractable = ActiveUIElements.Count; // immediately give interaction priority to menus immediately when they spawn
                }

                ActiveUIElements.Add(element);
            }

            UIElementsToAddNextFrame.Clear();

            foreach (UIElement element in ActiveUIElements)
            {
                element.Update();
            }

            if (IsKeyPressed(Keys.Enter) && !WasKeyPressedLastFrame(Keys.Enter) && IndexOfInteractable >= 0)
            {
                ActiveUIElements[IndexOfInteractable].HandleEnter();
            }

            if (IsKeyPressed(Keys.Tab) && !WasKeyPressedLastFrame(Keys.Tab))
            {
                if (IndexOfInteractable == -1)
                {
                    IncrementIndexOfInteractable();
                }
                else
                {
                    ActiveUIElements[IndexOfInteractable].HandleTab();
                }
            }

            if (IsKeyPressed(Keys.Escape) && !WasKeyPressedLastFrame(Keys.Escape) && !UIElementExists("PauseMenu"))
            {
                UI.CreatePauseMenu();
            }
        }

        public static void IncrementIndexOfInteractable()
        {
            do
            {
                MoveIndexToNextInteractable();

                if (IndexOfInteractable == -1)
                {
                    return; // if we get to the end of the ui elements, stop trying
                }
            }
            while (!ActiveUIElements[IndexOfInteractable].Interactable);
        }

        private static void MoveIndexToNextInteractable()
        {
            IndexOfInteractable++;

            if (IndexOfInteractable == ActiveUIElements.Count)
            {
                IndexOfInteractable = 0;
            }
        }

        public static UIElement? InteractableUIElement()
        {
            return IndexOfInteractable == -1 ? null : ActiveUIElements[IndexOfInteractable];
        }

        public static void ResetAllSelections()
        {
            List<UIElement> allMenus = GetListOfActiveMenus();

            foreach (UIElement element in allMenus)
            {
                ((Menu)element).IndexOfSelected = -1;
            }
        }
        public static void DrawUI(SpriteBatch spriteBatch)
        {
            _spriteBatch.Begin();

            PlayerHUD.Draw(spriteBatch);

            foreach (UIElement element in ActiveUIElements)
            {
                element.Draw(spriteBatch);
            }

            Drawing.DrawText("Main Interactable Index = " + IndexOfInteractable.ToString(), new Vector2(GameWidth / 2, GameHeight / 1.5f), spriteBatch, font, Color.White, Vector2.One);
            _spriteBatch.End();
        }

        public static UIElement Focused()
        {
            return ActiveUIElements[IndexOfInteractable];
        }

        public static Menu? DeepestFocusedMenu(Menu menu)
        {
            if (menu.IndexOfSelected == -1)
            {
                return menu;
            }

            UIElement focusedInMenu = menu.UIElements[menu.IndexOfSelected];

            if (focusedInMenu is Menu innerMenu)
            {
                return DeepestFocusedMenu(innerMenu);
            }
            else
            {
                return menu;
            }
        }

        public static void ClearMenus()
        {
            List<UIElement> menus = GetListOfActiveMenus();

            foreach (UIElement menu in menus)
            {
                menu.Remove();
            }
        }

        public static void ClearUI()
        {
            foreach (UIElement element in ActiveUIElements)
            {
                element.Remove();
            }
        }

        public static bool UIElementExists(string menuName) // make this so it can search for buttons within menus
        {
            foreach (UIElement element in ActiveUIElements)
            {
                if (element.Name == menuName)
                {
                    return true;
                }
            }

            return false;
        }
        public static UIElement? GetUIElement(string menuName)
        {
            foreach (UIElement element in ActiveUIElements)
            {
                if (element.Name == menuName)
                {
                    return element;
                }
            }

            return null;
        }

        public static List<UIElement> GetListOfActiveMenus()
        {
            List<UIElement> output = new List<UIElement>();

            foreach (UIElement element in ActiveUIElements)
            {
                if (element is Menu)
                {
                    output.Add((Menu)element);
                }
            }

            return output;
        }

        public static Menu? LatestAddedMenu()
        {
            List<UIElement> activeMenus = GetListOfActiveMenus();

            if (activeMenus.Count > 0)
            {
                return (Menu)activeMenus.Last();
            }
            else return null;
        }
    }
}
