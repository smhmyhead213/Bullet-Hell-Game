using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles.Player
{
    public class PlayerSharpShooterProjectile : FriendlyProjectile
    {
        public override Color Colour() => Color.Yellow;
    }
}
