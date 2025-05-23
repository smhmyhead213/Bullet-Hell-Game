﻿using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace bullethellwhatever.DrawCode.UI
{
    public class Menu : UIElement
    {
        public List<UIElement> UIElements;

        public bool Important; // whether or not stuff in game can happen while this menu is up

        public bool Draggable;

        public int TimeSinceLastDrag;

        public Dictionary<int, List<UIElement>> Rows;

        public float Margin; // consider changing these to vector2s to allow different x and y paddings
        public float Padding;
        public int IndexOfSelected
        {
            get;
            set;
        }

        public Menu(string texture, Vector2 size, Vector2 position) : base(texture, size, position)
        {
            Position = position;
            Size = size;
            Texture = AssetRegistry.GetTexture2D(texture);

            UIElements = new List<UIElement>();

            Name = "";

            PrepareMenu();
        }

        public void PrepareMenu()
        {          
            if (Texture == AssetRegistry.GetTexture2D("box"))
                Colour = Color.Black; // dont colour if none is specified
             
            Important = false;
            Draggable = false;

            TimeSinceLastDrag = 0;

            IndexOfSelected = -1;

            UIManager.ResetAllSelections();

            UpdateClickBox();
        }

        public void StartMenuBuilder(float margin, float padding)
        {
            Margin = margin;
            Padding = padding;
            Rows = new Dictionary<int, List<UIElement>>();
        }

        public bool RowExists(int row)
        {
            return Rows.ContainsKey(row);
        }

        public void AddUIElementToRow(UIElement uiElement, int row, bool strictRowFilling = true)
        {
            // check if there's already stuff in this row - if not, initialise list

            if (!RowExists(row))
            {
                Rows.Add(row, new List<UIElement>());
            }
            else
            {
                Rows[row].Add(uiElement); // make sure any changes i make after this to the ui element are copied here - should be fine because its a ref
            }

            float currentRowLength = RowLength(row); // this already includes the padding on the right of the previous element
            float currentRowHeight = RowHeight(row); // already includes padding on the top of the row above this
                                                     
            uiElement.PositionInMenu = new Vector2(currentRowLength + uiElement.Size.X / 2, currentRowHeight + uiElement.Size.Y / 2);
        }

        /// <summary>
        /// Returns the length from the left-hand side of the menu to the very right hand side of the selected row, including the padding on the right-most element
        /// </summary>
        public float RowLength(int row)
        {
            if (!RowExists(row))
            {
                return Margin;
            }

            float rightMostElementPosition = 0; // not sure if each row will be ordered, so do a quick find max
            float rightMostElementWidth = 0;

            foreach (UIElement uiElement in Rows[row])
            {
                if (uiElement.PositionInMenu.X > rightMostElementPosition)
                {
                    rightMostElementPosition = uiElement.PositionInMenu.X;
                    rightMostElementWidth = uiElement.Size.X;
                }
            }

            return rightMostElementPosition + (rightMostElementWidth / 2) + Padding;
        }

        /// <summary>
        /// Returns the top-most Y co-ordinate encompassed by the given row, including the padding at the bottom.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public float RowHeight(int row)
        {
            if (!RowExists(row))
            {
                return Margin;
            }

            if (row == 1)
            {
                return Margin;
            }
            else
            {
                return RowHeight(row - 1) + MaxHeightInRow(row - 1) + Padding; // unironic recursion application
            }
        }

        public float MaxHeightInRow(int row)
        {
            if (!RowExists(row))
            {
                return 0;
            }

            float maxHeight = 0;

            foreach (UIElement uIElement in Rows[row])
            {
                if (uIElement.Size.Y > maxHeight)
                {
                    maxHeight = uIElement.Size.Y;
                }
            }

            return maxHeight;
        }
        public void SetImportant(bool important)
        {
            Important = important;
        }

        public void SetDraggable(bool draggable)
        {
            Draggable = draggable;
        }

        public bool IsDisplayed()
        {
            return UIManager.ActiveUIElements.Contains(this);
        }

        public override void Display()
        {
            UIManager.UIElementsToAddNextFrame.Add(this);
        }

        public override void Remove()
        {
            UIManager.IndexOfInteractable = UIManager.ActiveUIElements.IndexOf(this) - 1;
            base.Remove();
        }

        public override void Update()
        {
            base.Update();

            if (Draggable)
            {
                TimeSinceLastDrag++;

                if ((ClickBox.Contains(MousePosition) || TimeSinceLastDrag < 10) && !HoveringOverAnyUI() && IsLeftClickDown())
                {
                    TimeSinceLastDrag = 0;

                    // calculate the mouses relative position within the menu

                    Vector2 mouseRelativePosition = MousePosition - TopLeft();

                    Position = TopLeft() + mouseRelativePosition;

                    UIManager.IndexOfInteractable = UIManager.ActiveUIElements.IndexOf(this);
                }
            }

            UpdateClickBox();

            foreach (UIElement uIElement in UIElements)
            {
                uIElement.Update();
            }
        }

        public bool HoveringOverAnyUI()
        {
            foreach (UIElement uIElement in UIElements)
            {
                if (uIElement.IsHovered())
                {
                    return true;
                }
            }

            return false;
        }
        public void UpdateClickBox()
        {
            ClickBox = new RectangleButGood(TopLeft().X, TopLeft().Y, Width(), Height());
        }

        public void IncrementSelected()
        {
            if (IndexOfSelected == UIElements.Count - 1)
            {
                IndexOfSelected = -1;
            }
            else
            {
                IndexOfSelected++;
            }
        }
        public void UpdateSelectedElement()
        {
            // try to increment

            int startIndex = IndexOfSelected; // track start index so if we get back to it that means theres no other selectable one

            IncrementSelected();

            if (IndexOfSelected == -1)
            {
                return;
            }

            while (!UIElements[IndexOfSelected].Interactable)
            {
                IncrementSelected();

                if (IndexOfSelected == startIndex)
                {
                    return; // give up.
                }
            }
        }

        public override void HandleEnter()
        {
            if (IndexOfSelected >= 0)
            {
                UIElements[IndexOfSelected].HandleClick();
            }
        }
        public override void HandleTab()
        {
            UpdateSelectedElement();
        }
        public UIElement? GetSelectedElement()
        {
            if (IndexOfSelected >= 0)
            {
                return UIElements[IndexOfSelected];
            }
            else return null;
        }
        public void AddUIElements(UIElement[] uIElements)
        {
            foreach (UIElement uIElement in uIElements)
            {
                AddUIElement(uIElement);
            }
        }
        public void AddUIElement(UIElement uiElement)
        {
            uiElement.Owner = this;
            UIElements.Add(uiElement);
        }

        public float Width() => Size.X;

        public float Height() => Size.Y;

        public Vector2 RelativeCentreOfMenu() => new Vector2(Width(), Height()) / 2f;

        public Vector2 TopLeft() => Position - RelativeCentreOfMenu();
        public override void Draw(SpriteBatch s)
        {
            Drawing.BetterDraw(Texture, Position, null, Colour * Opacity, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1); // scale texture up to required size

            foreach (UIElement uiElement in UIElements)
            {
                uiElement.Draw(s);
            }

            //Utilities.drawTextInDrawMethod("Index of selected = " + IndexOfSelected.ToString(), Utilities.CentreOfScreen() / 4f + new Vector2(0f, 50f), s, font, Color.White);
        }
    }
}
