using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Projectiles.Player
{
    public class PlayerProjectile : Projectile
    {
        public override void AI()
        {
            Position = Position + Velocity;
        }
    }
}
