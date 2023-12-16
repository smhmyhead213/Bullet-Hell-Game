using bullethellwhatever.DrawCode.UI;
using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class EyeBossPhaseTwoMinion : EyeBoss
    {
        public EyeBoss Owner;
        public EyeBossPhaseTwoMinion(EyeBoss owner, int linksInChain, Vector2 chainStartPos)
        {
            Owner = owner;

            ChainStartPosition = chainStartPos;

            Velocity = Vector2.Zero;
            Texture = Assets["Circle"];
            Size = Vector2.One * 2f;

            MaxHP = 20;

            Colour = Color.White;

            //Pupil = new Pupil("Circle", 0, 0, Size / 4);
            //Pupil.Spawn(Position, Vector2.Zero, 0f, Pupil.Texture, Pupil.Size, 0, 0, Color.Black, false, false);
            //Pupil.SetParticipating(false);

            BossAttacks = new EyeBossAttack[]
            {
                
            };

            CreateChain(linksInChain);
        }

        public override void Update()
        {
            base.Update();

            if (Health <= 0)
            {               
                IsDesperationOver = true;
            }
        }

        public override void HandlePhaseChanges()
        {
            // do nothing
        }
        public override void DrawHPBar(SpriteBatch spriteBatch)
        {
            // do the same as NPC, dont draw the large HP bar

            if (Participating)
            {
                UI.DrawHealthBar(spriteBatch, this, Position + new Vector2(0, 10f * DepthFactor()), 50f * DepthFactor(), 10f * DepthFactor());
            }
        }
    }
}
