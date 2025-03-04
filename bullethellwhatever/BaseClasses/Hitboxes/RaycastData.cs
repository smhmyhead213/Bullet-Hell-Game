using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.BaseClasses.Hitboxes
{
    public class RaycastData
    {
        public Vector2 DescribingVector;
        public int Direction;

        /// <summary>
        /// -1 is a backwards raycast.
        /// </summary>
        /// <param name="describingVector"></param>
        /// <param name="direction"></param>
        public RaycastData(Vector2 describingVector, int direction)
        {
            DescribingVector = describingVector; // does setting this once work for variable velocity?
            Direction = direction;
        }
    }
}
