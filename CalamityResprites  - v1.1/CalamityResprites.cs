using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityResprites
{
    public class BossTextureRegistry : ModSystem
    {
        public const string BasePath = "CalamityResprites/NPCs";

        private static readonly BossTextureResprite[] Bosses = LoadBossResprites();
        private static readonly Dictionary<Type, string> _pathOverridesByType = new();
        private static readonly HashSet<MethodInfo> _hookedGetters = new();
        private static readonly List<Hook> _hooks = new();

        public override void Load()
        {
            if (Main.dedServ)
                return;

            foreach (BossTextureResprite boss in Bosses)
            {
                boss.BodyTexture = ModContent.Request<Texture2D>(boss.BodyTexturePath);
                if (boss.HeadIconTexturePath != null)
                    boss.HeadIconTexture = ModContent.Request<Texture2D>(boss.HeadIconTexturePath);

                _pathOverridesByType[boss.NPCType] = boss.BodyTexturePath;
                HookTextureGetter(boss.NPCType);
            }
        }

        private void HookTextureGetter(Type npcType)
        {
            MethodInfo getter = npcType
                .GetProperty("Texture", BindingFlags.Public | BindingFlags.Instance)
                ?.GetGetMethod();

            if (getter == null)
            {
                Mod.Logger.Warn($"CalamityResprites: couldn't find a Texture getter on {npcType.Name}; that resprite won't apply.");
                return;
            }

            if (!_hookedGetters.Add(getter))
                return;

            var hook = new Hook(getter, new Func<Func<object, string>, object, string>(RedirectTexturePath));
            _hooks.Add(hook);
        }

        private static string RedirectTexturePath(Func<object, string> orig, object self)
        {
            if (self != null && _pathOverridesByType.TryGetValue(self.GetType(), out string overridePath))
                return overridePath;

            return orig(self);
        }

        public override void PostSetupContent()
        {
            if (Main.dedServ)
                return;

            foreach (BossTextureResprite boss in Bosses)
            {
                int npcId = GetNpcType(boss.NPCType);
                TextureAssets.Npc[npcId] = boss.BodyTexture;

                if (boss.HeadIconTexture != null)
                    TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[npcId]] = boss.HeadIconTexture;
            }
        }

        private static int GetNpcType(Type npcType)
        {
            MethodInfo generic = typeof(ModContent)
                .GetMethods()
                .First(m => m.Name == nameof(ModContent.NPCType) && m.IsGenericMethodDefinition)
                .MakeGenericMethod(npcType);

            return (int)generic.Invoke(null, null);
        }

        public override void Unload()
        {
            foreach (Hook hook in _hooks)
                hook?.Dispose();

            _hooks.Clear();
            _hookedGetters.Clear();
            _pathOverridesByType.Clear();

            foreach (BossTextureResprite boss in Bosses)
            {
                boss.BodyTexture = null;
                boss.HeadIconTexture = null;
            }
        }

        private static BossTextureResprite[] LoadBossResprites()
        {
            IEnumerable<Type> respriteTypes = typeof(BossTextureRegistry)
                .Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(BossTextureResprite).IsAssignableFrom(t) &&
                    t.Namespace?.StartsWith("CalamityResprites.NPCs", StringComparison.Ordinal) == true);

            return respriteTypes
                .Select(t => (BossTextureResprite)Activator.CreateInstance(t)!)
                .ToArray();
        }
    }
}