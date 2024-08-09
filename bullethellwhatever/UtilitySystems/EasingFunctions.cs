using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.UtilitySystems
{
    public static class EasingFunctions
    {
        // functions use the same name as on easings.net, add easings as you use them

        public static float EaseOutElastic(float progress)
        {
            float c4 = 2f * PI / 3f;

            if (progress == 0f) return 0f;
            if (progress == 1f) return 1f;
            else return Pow(2, -10 * progress) * Sin((progress * 10 - 0.75f) * c4) + 1;
        }
    }
}
