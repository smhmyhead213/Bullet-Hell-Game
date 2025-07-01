using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework.Input;

namespace bullethellwhatever.DrawCode.UI.SpecialUIElements
{
    public class KeybindChanger : UIElement
    {
        public string KeybindName;
        public Func<string> TextFunction;
        public bool Rebinding;
        public int RebindTimeOut => 300;

        public KeybindChanger(string texture, Vector2 size, Vector2 position, string keybindName) : base(texture, size, position)
        {
            KeybindName = keybindName;
            TextFunction = DefaultMessage();
        }
        public Func<string> DefaultMessage()
        {
            return () => $"{KeybindName} is currently bound to {KeybindMap[KeybindName]}. Select to rebind.";
        }

        public Func<string> RebindingMessage()
        {
            return () => $"Press the button you wish to bind {KeybindName} to.";
        }

        public Func<string> DuplicateBindingMessage(string badBind, string control)
        {
            return () => $"{badBind} is already bound to {control}. Reselect to try again.";
        }

        public Func<string> DisallowedBindingMessage(string badBind)
        {
            return () => $"You may not bind {badBind}. Reselect and choose a different key.";
        }
        public void BeginRebinding()
        {
            Rebinding = true;
            AITimer = 0;
            TextFunction = RebindingMessage();
        }

        public void StopRebinding()
        {
            Rebinding = false;
            TextFunction = DefaultMessage();
        }

        public override void Update()
        {
            if (Rebinding)
            {
                if (AITimer == RebindTimeOut)
                {
                    StopRebinding();
                }

                Keybind newKey = new Keybind(Keys.None);

                if (AnyKeyNewlyPressed(out newKey))
                {
                    if (!(newKey.MouseButton != MouseButtons.None && newKey.Key != Keys.None))
                    {
                        if (KeybindMap.Values.Contains(newKey) && !(newKey == KeybindMap[KeybindName]))
                        {
                            Rebinding = false;
                            TextFunction = DuplicateBindingMessage(newKey.ToString(), ControlBoundToKey(newKey.ToString()));
                        }
                        else if (DisallowedBindings().Contains(newKey))
                        {
                            Rebinding = false;
                            TextFunction = DisallowedBindingMessage(newKey.ToString());
                        }
                        else
                        {
                            KeybindMap[KeybindName] = newKey;
                            StopRebinding();
                        }
                    }
                }
            }

            if (ClickedButNotLastFrame() && !Rebinding)
            {
                BeginRebinding();
            }

            base.Update();
        }

        public override void DrawAtPosition(SpriteBatch s, Vector2 position)
        {
            base.DrawAtPosition(s, position);

            Vector2 textSize = font.MeasureString(TextFunction());

            float textPadding = 20f;

            Drawing.DrawText(TextFunction(), position - Size / 2f + new Vector2(textPadding), s, font, Color.White, new Vector2((Size.X - 2 * textPadding) / textSize.X, (Size.Y - 2 * textPadding) / textSize.Y));
        }
    }
}
