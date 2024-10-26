using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using bullethellwhatever.DrawCode.UI.Player;
using bullethellwhatever.NPCs;

namespace bullethellwhatever.DrawCode.UI
{
    public class HealthBar : ProgressBar
    {
        public NPC NPCOwner;
        public HealthBar(string texture, Vector2 size, NPC owner, Vector2 position = default, float progress = 1) : base(texture, size, position, progress)
        {
            NPCOwner = owner;
        }

        public override void Update()
        {
            if (NPCOwner != null)
            {
                Progress = NPCOwner.HPRatio();
            }
            else
            {
                Remove();
            }
        }
    }
}
