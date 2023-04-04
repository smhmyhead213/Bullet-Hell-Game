using System;
using Microsoft.Xna.Framework;

namespace bullethellwhatever;

public class OscillatingSpeedProjectile : BasicProjectile
{
    public float OscillationFrequency;
    public float ProjectileSpeed;

    public OscillatingSpeedProjectile(float oscillationFrequency, float projectileSpeed)
    {
        OscillationFrequency = oscillationFrequency;
        ProjectileSpeed = projectileSpeed;
    }

    public override void AI() //and drawing
    {
        TimeAlive++;

        if (Acceleration != 0)
            Velocity = Velocity * Acceleration; //acceleration values must be very very small

        var speed = ProjectileSpeed * (MathF.Sin(TimeAlive / OscillationFrequency) + 1.5f);

        Velocity = speed * Utilities.SafeNormalise(Velocity, Vector2.Zero);

        Position = Position + Velocity;
    }
}