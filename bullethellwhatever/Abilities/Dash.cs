using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Abilities
{
    public class Dash : Ability
    {
        public Dash(int duration, int cooldown, Keys keyBind, Player owner)
        { 
            Duration = duration;
            KeyBind = keyBind;
            Cooldown = new Cooldown(cooldown);
            Owner = owner;
        }  
        public override void Execute()
        {        
            base.Execute();

            if (IsExecuting)
            {
                Owner.MoveSpeed = Owner.MoveSpeed * 4f;
            }

            if (JustActivated)
            {
                Owner.IFrames = Duration;
            }
        }
    }
}
