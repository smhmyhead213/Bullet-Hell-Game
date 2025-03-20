using bullethellwhatever.BaseClasses.Entities;
using bullethellwhatever.MainFiles;
using bullethellwhatever.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles
{
    public static class CommonProjectileAIs
    {
        public static void Home(this Entity entity, Vector2 target, float homingStrength)
        {
            entity.Velocity = Utilities.ConserveLengthLerp(entity.Velocity, entity.Position.DirectionTo(target), homingStrength);
        }
    }
}
