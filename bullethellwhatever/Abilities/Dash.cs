using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using bullethellwhatever.DrawCode.Particles;

namespace bullethellwhatever.Abilities
{
    public class Dash : Ability
    {
        public Dash(int duration, int cooldown, Keys keyBind, Player owner)
        { 
            Duration = duration;
            KeyBind = keyBind;
            Owner = owner;
            CooldownTime = 40;
        }  
        public override void Execute()
        {        
            if (IsExecuting)
            {
                Owner.MoveSpeed = Owner.MoveSpeed * 4f;
                // make trail fade in and out
                float interpolant = (float)Timer / Duration;
                Owner.GetTrail().Opacity = EasingFunctions.EaseParabolic(interpolant);

                float particleAngleVariance = PI / 6;
                float rotation = Utilities.VectorToAngle(Owner.Velocity) + PI + Utilities.RandomAngle(-particleAngleVariance, particleAngleVariance);

                Particle p = new Particle();

                Vector2 velocity = 10f * Utilities.RotateVectorClockwise(-Vector2.UnitY, rotation);
                int lifetime = 20;
                p.Spawn("box", Owner.Position, velocity, -velocity / 2f / lifetime, Vector2.One * 0.45f, rotation, Owner.Colour, 1f, 20);
            }

            if (JustActivated)
            {
                Owner.IFrames = Duration;
            }

            base.Execute();
        }

        public override void OnAbilityFinish()
        {
            
        }
    }
}
