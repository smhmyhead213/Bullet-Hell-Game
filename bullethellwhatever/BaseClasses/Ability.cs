﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.BaseClasses
{
    public abstract class Ability
    {
        public Cooldown Cooldown;
        public Player Owner;
        public int Duration;
        public int Timer;
        public bool IsExecuting;
        public bool JustActivated;
        public Keys KeyBind;
        public KeyboardState kstate;
        public bool IsKeyDown;
        public virtual void Execute()
        {
            kstate = Keyboard.GetState();

            IsKeyDown = kstate.IsKeyDown(KeyBind);

            if (!IsExecuting)
            {
                if (IsKeyDown && Cooldown.Timer == 0)
                {
                    IsExecuting = true;
                    JustActivated = true;
                    Timer = 0;
                }

                if (Cooldown.Timer > 0)
                {
                    Cooldown.Timer--;
                }
            }
            else
            {
                JustActivated = false;

                if (Timer < Duration)
                {
                    Timer++;
                }

                if (Timer == Duration)
                {
                    IsExecuting = false;
                    Cooldown.Timer = Cooldown.Duration;
                }
            }
        }
    }
}
