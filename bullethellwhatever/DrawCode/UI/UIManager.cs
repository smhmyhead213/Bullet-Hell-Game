using Microsoft.Xna.Framework.Graphics;
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

        public static int DefaultButtonCooldown => 25;
        public static int ButtonCooldown;
        public static int NavigationCooldown => 8;

        public static int NavigationCooldownTimer;
        public static void Initialise()
        {
            ActiveUIElements = new List<UIElement>();
            UIElemntsToRemoveNextFrame = new List<UIElement>();
            UIElementsToAddNextFrame = new List<UIElement>();

            ButtonCooldown = 0;

            NavigationCooldownTimer = 0;
        }
        public static void ManageUI()
        {
            foreach (UIElement element in UIElemntsToRemoveNextFrame)
            {
                ActiveUIElements.Remove(element);
            }

            UIElemntsToRemoveNextFrame.Clear();

            foreach (UIElement element in ActiveUIElements)
            {
                element.Update();
            }

            foreach (UIElement element in UIElementsToAddNextFrame)
            {
                //if (element is Menu)
                //{
                //    IndexOfInteractable = ActiveUIElements.Count; // immediately give interaction priority to menus immediately when they spawn
                //}

                ActiveUIElements.Add(element);
            }

            UIElementsToAddNextFrame.Clear();

            if (ButtonCooldown > 0)
            {
                ButtonCooldown--;
            }

            if (NavigationCooldownTimer > 0)
            {
                NavigationCooldownTimer--;
            }

            if (IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter) && ButtonCooldown == 0)
            {
                ActiveUIElements[IndexOfInteractable].HandleEnter();
                ButtonCooldown = DefaultButtonCooldown;
            }

            if (IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Tab) && NavigationCooldownTimer == 0)
            {
                NavigationCooldownTimer = NavigationCooldown;

                if (IndexOfInteractable == -1)
                {
                    IncrementIndexOfInteractable();
                }
                else
                {
                    ActiveUIElements[IndexOfInteractable].HandleTab();
                }
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

            Utilities.drawTextInDrawMethod(IndexOfInteractable.ToString(), Utilities.CentreOfScreen() / 4f, spriteBatch, font, Microsoft.Xna.Framework.Color.White);
            Utilities.drawTextInDrawMethod(NavigationCooldownTimer.ToString(), Utilities.CentreOfScreen() / 4f + new Microsoft.Xna.Framework.Vector2(0f, 100f), spriteBatch, font, Microsoft.Xna.Framework.Color.White);
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
