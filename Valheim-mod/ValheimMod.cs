using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;

namespace ValheimMod
{
    [BepInPlugin("kuuhaku.ValheimMod", "Ballista infinite ammo", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class ValheimMod : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("kuuhaku.ValheimMod");
        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("Ballista infinite ammo");

        void Awake()
        {
            ValheimMod.logger.LogInfo("Ballista infinite ammo Mod has been loaded");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Turret), nameof(Turret.ShootProjectile))]
        public static class Patch_Turret_ShootProjectile
        {
            static void Prefix(ref ZNetView ___m_nview, Turret __instance, ref float ___m_attackCooldown)
            {
                ___m_attackCooldown = 0.5f;
                int currentAmmo = ___m_nview.GetZDO().GetInt(ZDOVars.s_ammo);
                
                // This will increment before firing, so u can basically create a delusional infinite ammo.
                ___m_nview.GetZDO().Set(ZDOVars.s_ammo, currentAmmo + 1);

                var getAmmoItemMethod = AccessTools.Method(typeof(Turret), "GetAmmoItem");
                ItemDrop.ItemData ammoItem = getAmmoItemMethod.Invoke(__instance, null) as ItemDrop.ItemData;
                if (ammoItem != null)
                {
                    // Trying to set the accuracy to 100
                    ammoItem.m_shared.m_attack.m_projectileAccuracy = 0f;
                    Debug.Log($"Modified Projectile Accuracy");
                }
            }
        }

    }
}