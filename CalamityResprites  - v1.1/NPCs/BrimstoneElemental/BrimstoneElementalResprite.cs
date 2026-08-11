using System;
using BrimstoneElementalNPC = CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental;

namespace CalamityResprites.NPCs.BrimstoneElemental
{
    public class BrimstoneElementalResprite : BossTextureResprite
    {
        public override Type NPCType => typeof(BrimstoneElementalNPC);
        public override string BodyTexturePath => $"{BossTextureRegistry.BasePath}/BrimstoneElemental/BrimstoneElemental";
        public override string HeadIconTexturePath => $"{BossTextureRegistry.BasePath}/BrimstoneElemental/BrimstoneElemental_Head_Boss";
    }
}
