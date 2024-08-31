using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        }
        public static void ManageUI()
        {
            foreach (UIElement element in UIElemntsToRemoveNextFrame)
            {
                ActiveUIElements.Remove(element);
            }

            UIElemntsToRemoveNextFrame.Clear();

            foreach (UIElement element in UIElementsToAddNextFrame)
            {
                if (element is Menu)
                {
                    IndexOfInteractable = ActiveUIElements.Count; // immediately give interaction priority to menus immediately when they spawn
                }

                ActiveUIElements.Add(element);
            }

            UIElementsToAddNextFrame.Clear();

            foreach (UIElement element in ActiveUIElements)
            {
                element.Update();
            }

            if (IsKeyPressed(Keys.Enter) && !WasKeyPressedLastFrame(Keys.Enter))
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
            IndexOfInteractable++;

            if (IndexOfInteractable == ActiveUIElements.Count)
            {
                IndexOfInteractable = -1;
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
            Drawing.RestartSpriteBatchForUI(spriteBatch);

            foreach (UIElement element in ActiveUIElements)
            {
                element.Draw(spriteBatch);
            }

            //Utilities.drawTextInDrawMethod("Interactable Index = " + IndexOfInteractable.ToString(), Utilities.CentreOfScreen() / 4f, spriteBatch, font, Microsoft.Xna.Framework.Color.White);
            _spriteBatch.End();
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
