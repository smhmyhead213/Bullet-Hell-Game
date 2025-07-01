using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Abilities
{
    public abstract class Ability
    {
        public int Cooldown;
        public int CooldownTime;
        public Player Owner;
        public int Duration;
        public int Timer;
        public bool IsExecuting;
        public bool JustActivated;
        public string KeyBind;
        public bool IsKeyDown;
        public virtual void Execute()
        {
            IsKeyDown = KeybindPressed(KeyBind);

            if (!IsExecuting)
            {
                if (IsKeyDown && Cooldown == 0)
                {
                    IsExecuting = true;
                    JustActivated = true;
                    Timer = 0;
                }

                if (Cooldown > 0)
                {
                    Cooldown--;
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
                    OnAbilityFinish();
                    IsExecuting = false;
                    Cooldown = CooldownTime;
                }
            }
        }

        public virtual void OnAbilityFinish()
        {
            
        }
    }
}
