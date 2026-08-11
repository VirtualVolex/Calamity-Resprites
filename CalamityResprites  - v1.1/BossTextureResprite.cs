using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;

namespace CalamityResprites
{
    public abstract class BossTextureResprite
    {
        public abstract Type NPCType { get; }
        public abstract string BodyTexturePath { get; }
        public virtual string HeadIconTexturePath => null;

        public Asset<Texture2D> BodyTexture { get; set; }
        public Asset<Texture2D> HeadIconTexture { get; set; }
    }
}
