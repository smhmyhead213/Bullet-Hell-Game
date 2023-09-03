using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bullethellwhatever.Bosses;

namespace bullethellwhatever.BaseClasses
{
    public class Boss : NPC
    {
        public int AttackNumber; //position in pattern
        public int FramesPerMusicBeat;
        public int BeatsPerBar;
        public int BarDuration;
        public int CurrentBeat;
        public bool JustStartedBeat;

        public BossAttack[] BossAttacks;

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (BossAttacks is not null)
                BossAttacks[AttackNumber].ExtraDraw(spriteBatch);
        }
    }
}
