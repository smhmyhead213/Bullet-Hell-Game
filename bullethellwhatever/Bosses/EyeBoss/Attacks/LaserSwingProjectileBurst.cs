using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Bosses.CrabBoss.Projectiles;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class LaserSwingProjectileBurst : EyeBossAttack
    {
        public float AngleToSwing;
        public int SwingPassesComplete;
        public LaserSwingProjectileBurst(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
            AngleToSwing = 0;
            SwingPassesComplete = 0;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int rayTelegraphTime = 60;
            int swingTime = 30;
            int eyeFocusTime = swingTime + 15;
            int cycleTime = (rayTelegraphTime + eyeFocusTime);
            int time = AITimer % cycleTime;

            ChainLink firstLink = EyeOwner.ChainLinks[0];

            float rayAdditionalAngle = SwingPassesComplete * PI; // if an odd number of passes have been completed, a half turn will be added to the ray angle.
            float beamDirection = firstLink.Rotation + PI / 2 + rayAdditionalAngle;

            if (time < rayTelegraphTime)
            {
                Pupil.LookAtPlayer(30f);

                if (time == 0)
                {
                    TelegraphLine t = new TelegraphLine(beamDirection, 0, 0, 50, Utilities.ScreenDiagonalLength(), rayTelegraphTime, Pupil.Position, Color.White, "box", Pupil, true);

                    t.SetExtraAI(new Action(() =>
                    {
                        t.Rotation = beamDirection;
                    }));

                    t.SetOnDeath(new Action(() =>
                    {
                        SwingPassesComplete++;

                        Deathray ray = new Deathray();

                        ray.SetStayWithOwner(true);
                        ray.SetThinOut(true);
                        ray.SpawnDeathray(t.Origin, t.Rotation, 1f, 10, "box", t.Width, t.Length, 0, 0, true, Color.White, "DeathrayShader2", Pupil);

                        int projectiles = 50;
                        int distanceBetweenProjectiles = 150;

                        float additionalRotation = t.Rotation;

                        for (int i = 0; i < projectiles; i++)
                        {
                            Vector2 additionalDistance = Utilities.AngleToVector(t.Rotation) * distanceBetweenProjectiles;

                            for (int j = 0; j < 2; j++)
                            {
                                Projectile p = new Projectile();

                                float projSpeed = 5f;

                                p.SetDrawAfterimages(50, 3);

                                p.Rotation = additionalRotation - PI / 2 + (j * PI);

                                p.SetExtraAI(new Action(() =>
                                {
                                    p.Rotation = Utilities.VectorToAngle(p.Velocity);
                                }));

                                p.Spawn(t.Origin + i * additionalDistance, projSpeed * Utilities.AngleToVector(additionalRotation - PI / 2 + (j * PI)), 1f, 1, "box", 1.01f, Vector2.One * 0.6f, Owner, true, Color.White, true, false);
                            }
                        }
                    }));
                }
            }

            if (time < rayTelegraphTime)
            {
                Pupil.GoTo(30, beamDirection);
            }

            if (time == rayTelegraphTime) // when the ray is fired
            {
                float angleToReachSwingingLeft = Utilities.ToRadians(250f); // im too lazy to convert this myself
                float angleToReachSwingingRight = Utilities.ToRadians(20f);

                if (SwingPassesComplete % 2 == 0) // if we are swinging left this time
                {
                    AngleToSwing = firstLink.Rotation - angleToReachSwingingLeft;
                }
                else
                {
                    AngleToSwing = angleToReachSwingingLeft - firstLink.Rotation;
                }
            }

            if (time > rayTelegraphTime && time <= swingTime + rayTelegraphTime)
            {
                foreach (ChainLink c in EyeOwner.ChainLinks)
                {
                    c.Rotate(-AngleToSwing / swingTime);
                }
            }
        }

        public override void ExtraAttackEnd()
        {
            base.ExtraAttackEnd();

            Pupil.ResetSize();
        }
        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
