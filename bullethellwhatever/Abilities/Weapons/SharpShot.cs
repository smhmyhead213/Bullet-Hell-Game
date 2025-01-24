using bullethellwhatever.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Abilities.Weapons
{
    public class SharpShot : Projectile
    {
        public List<Projectile> ReflectorsHit;
        public int Reflections;

        public override void Initialise()
        {
            ReflectorsHit = new List<Projectile>();
            Reflections = 0;
        }

        public override void AI()
        {
            
        }
    }
}
