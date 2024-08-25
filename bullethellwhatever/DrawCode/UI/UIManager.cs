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
        public static int DefaultButtonCooldown => 25;
        public static int ButtonCooldown;

        public static void Initialise()
        {
            ActiveUIElements = new List<UIElement>();
            UIElemntsToRemoveNextFrame = new List<UIElement>();
            UIElementsToAddNextFrame = new List<UIElement>();

            ButtonCooldown = 0;
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
                ActiveUIElements.Add(element);
            }

            UIElementsToAddNextFrame.Clear();

            if (ButtonCooldown > 0)
            {
                ButtonCooldown--;
            }
        }

        public static void DrawUI(SpriteBatch spriteBatch)
        {
            Drawing.RestartSpriteBatchForUI(spriteBatch);

            foreach (UIElement element in ActiveUIElements)
            {
                element.Draw(spriteBatch);
            }

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
    }
}
