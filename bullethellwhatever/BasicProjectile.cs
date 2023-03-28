using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever
{
    public class BasicProjectile : HarmfulProjectile
    {

        public override void AI()
        {
            HandleMovement();
            TimeAlive++;
        }

        public override bool ShouldRemoveOnEdgeTouch() => true;



        public override Color Colour() => Color.Red;

    }
}
