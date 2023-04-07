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

namespace bullethellwhatever.Projectiles.Player
{
    public class PlayerProjectile : FriendlyProjectile
    {
        public override void AI()
        {
            Position = Position + Velocity;
        }

        public override bool ShouldRemoveOnEdgeTouch() => true;
        public override Color Colour() => Color.LightBlue;
    }
}
