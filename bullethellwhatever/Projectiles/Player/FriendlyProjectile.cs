using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;

namespace bullethellwhatever.Projectiles.Player
{
    public class FriendlyProjectile : Projectile
    {
        public int Pierce;
        public override bool IsHarmful() => false;

        public override void Spawn(Vector2 position, Vector2 velocity, float damage, Texture2D texture, float acceleration, Vector2 size, Entity owner) //this is overriden as friendly projectiles add themselves to a different list
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Texture = texture;
            Main.friendlyProjectilesToAddNextFrame.Add(this);
            Size = size;
            Owner = owner;
        }

        
    }
}
