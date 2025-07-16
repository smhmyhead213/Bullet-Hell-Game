using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode
{
    // this code is copied and edited from decompiled VertexPositionColor code. CHECK IF THIS IS ALLOWED.
    public struct VertexPositionColourTextureExtra : IVertexType
    {
        public Vector3 Position;

        public Color Color;

        public Vector2 TextureCoordinate;
        public Vector2 ExtraData;

        public static readonly VertexDeclaration VertexDeclaration;
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
        public VertexPositionColourTextureExtra(Vector3 position, Color color, Vector2 textureCoordinate, Vector2 extraData)
        {
            Position = position;
            Color = color;
            TextureCoordinate = textureCoordinate;
            ExtraData = extraData;
        }
        static VertexPositionColourTextureExtra()
        {
            VertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1));
        }
        public override int GetHashCode()
        {
            return (((Position.GetHashCode() * 397) ^ Color.GetHashCode()) * 397) ^ TextureCoordinate.GetHashCode();
        }
    }
}
