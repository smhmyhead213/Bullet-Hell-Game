using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class EyeBoss : Boss
    {
        public List<ChainLink> ChainLinks = new List<ChainLink>();
        public Pupil Pupil;
        public EyeBoss() 
        {
            ChainLinks = new List<ChainLink>();

            Position = Utilities.CentreOfScreen();
            Velocity = Vector2.Zero;
            Texture = Assets["Circle"];
            Size = Vector2.One * 2f;

            MaxHP = 500;

            Colour = Color.White;

            Pupil = new Pupil("Circle", 0, 0, Size / 4);
            Pupil.Spawn(Position, Vector2.Zero, 0f, Pupil.Texture, Pupil.Size, 0, 0, Color.Black, false, false);
            Pupil.SetParticipating(false);

            int numberOfLinks = 15;

            float firstChainInitialAngle = PI / 3;
            float finalChainInitialAngle = PI / 4;
            float totalChainLength = ScreenHeight / 2;
            float linkLength = totalChainLength / numberOfLinks;

            ChainLink c = new ChainLink("box", new Vector2(ScreenWidth / 2, 0), firstChainInitialAngle, linkLength, this);

            float dampFactor = 0.98f;

            c.SetDampingFactor(dampFactor);

            ChainLinks.Add(c);

            float changeBetweenEach = (firstChainInitialAngle - finalChainInitialAngle) / numberOfLinks;

            for (int i = 1; i < numberOfLinks; i++)
            {
                c = new ChainLink("box", ChainLinks[i - 1].End, firstChainInitialAngle - (i * changeBetweenEach) , linkLength, this);
                c.SetDampingFactor(dampFactor);
                ChainLinks.Add(c);
            }

            BossAttacks = new EyeBossAttack[]
            {
                new ProjectileFan(1000),
                new Meteors(300),
                new EyeRay(1600),
                new EnergyBlasts(750),
                new HelixShots(1230),
                new ProjectileFan(1000),
            };
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (ChainLink link in ChainLinks)
            {
                link.Draw(spriteBatch);
            }

            Pupil.Draw();
        }
    }
}
