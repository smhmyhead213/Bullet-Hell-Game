using bullethellwhatever.AssetManagement;
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

        public Dictionary<int, List<UIElement>> Rows;
        public int CurrentRow;

        public float MarginX;
        public float MarginY;

        public float Padding;
        public bool BuildingMode;
        public bool HeldByMouse;
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
            BuildingMode = false;
            HeldByMouse = false;

            PrepareMenu();
        }

        public void PrepareMenu()
        {          
            if (Texture == AssetRegistry.GetTexture2D("box"))
                Colour = Color.Black; // dont colour if none is specified
             
            Important = false;
            Draggable = false;

            IndexOfSelected = -1;

            UIManager.ResetAllSelections();

            UpdateClickBox();
        }

        public void StartMenuBuilder(float marginX, float marginY, float padding)
        {
            MarginX = marginX;
            MarginY = marginY;

            Padding = padding;
            Rows = new Dictionary<int, List<UIElement>>();
            CurrentRow = 1;
            BuildingMode = true;
        }

        public bool RowExists(int row)
        {
            return Rows.ContainsKey(row);
        }

        public bool RowEmpty(int row)
        {
            if (!RowExists(row))
            {
                return false;
            }

            return Rows[row].Count == 0;
        }

        public bool HorizontalSpaceAvailableFor(int row, float requestedWidth)
        {
            float currentRowLength = RowLength(row); // this already includes the padding on the right of the previous element
            float availableHorizontalSpace = Width() - currentRowLength - MarginX;
            return availableHorizontalSpace >= requestedWidth;
        }

        public bool HorizontalSpaceAvailableFor(int row, UIElement uiToAdd)
        {
            return HorizontalSpaceAvailableFor(row, uiToAdd.Size.X);
        }

        public virtual bool VerticalSpaceAvailableFor(int row, float requestedHeight)
        {
            float currentRowHeight = RowHeight(row); // already includes padding on the top of the row above this
            float availableVerticalSpace = Height() - currentRowHeight - MarginY;
            return availableVerticalSpace >= requestedHeight;
        }

        public bool VerticalSpaceAvailableFor(int row, UIElement uiToAdd)
        {
            return VerticalSpaceAvailableFor(row, uiToAdd.Size.Y);
        }

        public bool AddUIElementAuto(UIElement uiElement)
        {
            return AddUIElementToRow(uiElement, CurrentRow);
        }

        /// <summary>
        /// Adds a UIElement to the RIGHT side of a row. Does not (for now) support adding elements anywhere else in the row.
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="row"></param>
        /// <param name="strictRowFilling"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddUIElementToRow(UIElement uiElement, int row, bool strictRowFilling = true)
        {
            if (!BuildingMode)
            {
                throw new Exception("Cannot use menu building without calling StartMenuBuilder first.");
            }
            // check if there's already stuff in this row - if not, initialise list

            if (!RowExists(row))
            {
                Rows.Add(row, new List<UIElement>());
            }

            if (uiElement.Size.X > Width() - 2 * MarginX)
            {
                return false;
            }

            bool horizontalSpaceAvailable = HorizontalSpaceAvailableFor(row, uiElement);
            bool verticalSpaceAvailable = VerticalSpaceAvailableFor(row, uiElement);

            if (horizontalSpaceAvailable && verticalSpaceAvailable)
            {
                uiElement.PositionInMenu = new Vector2(RowLength(row) + uiElement.Size.X / 2, RowHeight(row) + uiElement.Size.Y / 2);

                uiElement.AddToMenu(this);
                uiElement.Position = uiElement.CalculateActualPostion();
                Rows[row].Add(uiElement); // make sure any changes i make after this to the ui element are copied here - should be fine because its a ref

                // for future: if you decide to support adding stuff in between other stuff, make sure a sort is done to keep stuff in order by X position
                return true;
            }
            else if (!horizontalSpaceAvailable && verticalSpaceAvailable)
            {
                MoveToNextRow(); // try and see if theres space on the next row
                
                return AddUIElementToRow(uiElement, row + 1, strictRowFilling);
            }
            else // if theres no vertical space then we've reached the bottom and there isnt anything we can do, so just dont bother (add a log write here when you implement it)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the length from the left-hand side of the menu to the very right hand side of the selected row, including the padding on the right-most element
        /// </summary>
        public float RowLength(int row)
        {
            if (!RowExists(row) || RowEmpty(row))
            {
                return MarginX;
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
                return MarginY;
            }

            if (row == 1)
            {
                return MarginY;
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

        public void MoveToNextRow()
        {
            CurrentRow++;
        }

        public float CalculateTotalHeight()
        {
            // find the row height of the lowest row

            float total = 0;

            foreach (KeyValuePair<int, List<UIElement>> row in Rows)
            {
                total += MaxHeightInRow(row.Key);
            }

            return total;
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
                if (!IsLeftClickDown() || LeftClickReleased())
                {
                    HeldByMouse = false;
                }

                if (ClickBox.Contains(MousePosition) && IsLeftClickDown() && !HoveringOverAnyUI())
                {
                    HeldByMouse = true;
                }

                if (HeldByMouse)
                {
                    // calculate the mouses relative position within the menu

                    Vector2 mouseRelativePosition = MousePosition - TopLeft();

                    Position = TopLeft() + mouseRelativePosition;

                    UIManager.IndexOfInteractable = UIManager.ActiveUIElements.IndexOf(this);
                }
            }

            //if (ExtraAI is not null)
            //{
            //    ExtraAI();
            //}

            AI();

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
            ClickBox = new RectangleF(TopLeft().X, TopLeft().Y, Width(), Height());
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

        /// <summary>
        /// Returns a Rectangle that excludes the margins of the menu.
        /// </summary>
        /// <returns></returns>
        public RectangleF InsideBoundingBox()
        {
            Vector2 topLeft = TopLeft();
            return new RectangleF(topLeft.X + MarginX, topLeft.Y + MarginY, Width() - 2 * MarginX, Height() - 2 * MarginY);
        }
        public override void Draw(SpriteBatch s)
        {
            //Colour = HeldByMouse ? Color.Red : Color.Green;

            Drawing.BetterDraw(Texture, Position, null, Colour * Opacity, 0, new Vector2(Size.X / Texture.Width, Size.Y / Texture.Height), SpriteEffects.None, 1); // scale texture up to required size

            foreach (UIElement uiElement in UIElements)
            {
                uiElement.Draw(s);
            }

            //Utilities.drawTextInDrawMethod("Index of selected = " + IndexOfSelected.ToString(), Utilities.CentreOfScreen() / 4f + new Vector2(0f, 50f), s, font, Color.White);
        }
    }
}
