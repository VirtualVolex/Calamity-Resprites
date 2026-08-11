using System;
using YharonNPC = CalamityMod.NPCs.Yharon.Yharon;

namespace CalamityResprites.NPCs.Yharon
{
    public class YharonResprite : BossTextureResprite
    {
        public override Type NPCType => typeof(YharonNPC);
        public override string BodyTexturePath => $"{BossTextureRegistry.BasePath}/Yharon/Yharon";
        public override string HeadIconTexturePath => $"{BossTextureRegistry.BasePath}/Yharon/Yharon_Head_Boss";
    }
}
