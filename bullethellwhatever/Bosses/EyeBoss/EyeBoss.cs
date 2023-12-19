using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.MainFiles;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.DrawCode;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class EyeBoss : Boss
    {
        public List<ChainLink> ChainLinks = new List<ChainLink>();
        public BossAttack[] OriginalAttacks;
        public Pupil Pupil;
        public int Phase;

        public int PhaseTwoMinionCount = 4;

        public float InitialChainDampingFactor;
        public Vector2 ChainStartPosition;
        public EyeBoss()
        {        
            //Position = Utilities.CentreOfScreen();
            Velocity = Vector2.Zero;
            Texture = Assets["Circle"];
            Size = Vector2.One * 2f;

            ChainStartPosition = new Vector2(ScreenWidth / 2, 0);

            Phase = 1;
            MaxHP = 5;

            Colour = Color.White;

            Pupil = new Pupil("Circle", 0, 0, Size / 4);
            Pupil.Spawn(Position, Vector2.Zero, 0f, Pupil.Texture, Pupil.Size, 0, 0, Color.Black, false, false);
            Pupil.SetParticipating(false);
            Pupil.TargetableByHoming = false;

            CreateChain(ScreenHeight / 2f);

            BossAttacks = new EyeBossAttack[]
            {
                new ProjectileRows(300),
                new LaserSwingProjectileBurst(850),               
                new Meteors(300),
                new EyeRay(1600),
                new EnergyBlasts(750),
                new HelixShots(1270),
                new ProjectileFan(600),
            };

            OriginalAttacks = BossAttacks;

            //RandomlyArrangeAttacks();
        }

        public virtual void CreateChain(float chainLength)
        {
            ChainLinks = new List<ChainLink>();

            float firstChainInitialAngle = PI / 3;
            float finalChainInitialAngle = PI / 4;

            int numberOfLinks = 15;

            float linkLength = chainLength / numberOfLinks;

            ChainLink c = new ChainLink("box", ChainStartPosition, firstChainInitialAngle, linkLength, this);

            InitialChainDampingFactor = 0.98f;

            c.SetDampingFactor(InitialChainDampingFactor);

            ChainLinks.Add(c);

            float changeBetweenEach = (firstChainInitialAngle - finalChainInitialAngle) / numberOfLinks;

            for (int i = 1; i < numberOfLinks; i++)
            {
                c = new ChainLink("box", ChainLinks[i - 1].End, firstChainInitialAngle - (i * changeBetweenEach), linkLength, this);
                c.SetDampingFactor(InitialChainDampingFactor);
                ChainLinks.Add(c);
            }
        }
        public override void Update()
        {
            ChainLinks[0].Update();

            for (int i = 1; i < ChainLinks.Count; i++)
            {
                ChainLinks[i].Position = ChainLinks[i - 1].End;
                ChainLinks[i].Update();
            }

            Position = ChainLinks.Last().End;

            Pupil.Update(Position);

            Pupil.CalculatePosition();

            base.Update();
        }

        public override void AI()
        {
            HandlePhaseChanges();

            base.AI();
        }

        public virtual void HandlePhaseChanges()
        {
            if (Health <= 0)
            {
                if (Phase == 1)
                {
                    Drawing.StopScreenShake();

                    BossAttack[] phaseTwoAttacks = new BossAttack[]
                    {
                        new PhaseTwoBulletHellBoss(900000),
                    };

                    Phase = 2;

                    //SetParticipating(false);

                    TargetableByHoming = false;
                    IsInvincible = true;
                   
                    ReplaceAttackPattern(phaseTwoAttacks);
                }
            }
        }
        public override void Die()
        {
            base.Die();

            Pupil.Die();
        }
        public void SetChainDampingFactor(float factor)
        {
            foreach (ChainLink link in ChainLinks)
            {
                link.SetDampingFactor(factor);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Utilities.drawTextInDrawMethod(Main.activeProjectiles.Count.ToString(), new Vector2(ScreenWidth / 4f * 3f, ScreenHeight / 4f * 3f), spriteBatch, font, Color.White);

            base.Draw(spriteBatch);

            foreach (ChainLink link in ChainLinks)
            {
                link.Draw(spriteBatch);
            }

            Pupil.Draw();
        }
    }
}
