using Microsoft.Xna.Framework.Graphics;
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
