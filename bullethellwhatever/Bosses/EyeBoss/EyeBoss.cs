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
        public EyeBoss() 
        {
            ChainLinks = new List<ChainLink>();

            Position = Utilities.CentreOfScreen();
            Velocity = Vector2.Zero;
            Texture = Assets["box"];
            Size = Vector2.One * 2f;

            MaxHP = 500;
            Health = MaxHP;

            Colour = Color.White;

            int numberOfLinks = 20;

            float firstChainInitialAngle = PI / 3;
            float finalChainInitialAngle = PI / 4;

            ChainLink c = new ChainLink("box", new Vector2(ScreenWidth / 2, 0), firstChainInitialAngle, 25, this);

            c.SetDampingFactor(0.99f);

            ChainLinks.Add(c);
            float changeBetweenEach = (firstChainInitialAngle - finalChainInitialAngle) / numberOfLinks;

            for (int i = 1; i < numberOfLinks; i++)
            {
                c = new ChainLink("box", ChainLinks[i - 1].End, firstChainInitialAngle - (i * changeBetweenEach) , 25, this);
                c.SetDampingFactor(0.99f);
                ChainLinks.Add(c);
            }
            
        }
        public override void Update()
        {
            base.Update();

            ChainLinks[0].Update();

            for (int i = 1; i < ChainLinks.Count; i++)
            {
                ChainLinks[i].Position = ChainLinks[i - 1].End;
                ChainLinks[i].Update();
            }

            Position = ChainLinks.Last().End;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (ChainLink link in ChainLinks)
            {
                link.Draw(spriteBatch);
            }
        }
    }
}
