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

namespace bullethellwhatever
{
    public class PlayerProjectile : FriendlyProjectile
    {

        public override void HandleMovement()
        {
            Position = Position + Velocity;
        }

        public override void AI()
        {
            base.AI();
        }

        public override bool ShouldRemoveOnEdgeTouch() => true;
        public override Color Colour() => Color.LightBlue;
    }
}
