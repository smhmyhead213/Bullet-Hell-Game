using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles._3D
{
    public class Cuboid
    {
        public int Length;
        public int Breadth;
        public int Height;

        public Entity Owner;

        private int NumberOfProjsInHeight;
        private int NumberOfProjsInLength;

        public Vector2 CuboidBottomLeft;

        public int DistanceBetweenProjectiles = 50;

        public LoopingProjectile[] BaseLoopProjectiles;

        public Cuboid(int length, int breadth, int height, Entity owner) 
        { 
            Length = length;
            Breadth = breadth;
            Height = height;

            Owner = owner;

            
            NumberOfProjsInHeight = Height / DistanceBetweenProjectiles; //INTEGER DIVISION THIS IS THE PROBLEM IF THERE IS ONE
            NumberOfProjsInLength = Length / DistanceBetweenProjectiles;
        }

        public virtual void Spawn(Vector2 centre)
        {
            CuboidBottomLeft = new Vector2(centre.X - Length / 2, centre.Y + Height / 2);

            for (int i = 0; i < (NumberOfProjsInHeight - 1); i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    LoopingProjectile proj = new LoopingProjectile(0.25f, Length, MathF.PI / 2 * j, MathF.PI / 2); //these things move upwards by default

                    proj.Spawn(new Vector2(centre.X, centre.Y - ((i - NumberOfProjsInHeight / 2) * DistanceBetweenProjectiles)), Vector2.One, 1f, 1, "box", 1f, Vector2.One, Owner, true, Color.Red, true, false);
                }
            }

            for (int i = 0; i < (NumberOfProjsInLength - 1); i++) //spawn projectiles in row
            {
                for (int j = 0; j < 2; j++)
                {
                    LoopingProjectile proj = new LoopingProjectile(0.25f, Length, MathF.PI / (NumberOfProjsInLength / 2) * i, MathF.PI / 2);

                    // - ((i - NumberOfProjsInHeight / 2) * DistanceBetweenProjectiles)
                    proj.Spawn(new Vector2(centre.X, centre.Y - (Height / 2) + j * Height), Vector2.One, 
                        1f, 1, "box", 1f, Vector2.One, Owner, true, Color.Red, true, false);
                }
            }
        }
    }
}
