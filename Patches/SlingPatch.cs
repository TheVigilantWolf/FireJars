using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace FireJars
{
    internal static class SlingAi
    {
        // Tune this. Start at 50-60 like you wanted.
        internal const float DesiredRangeMeters = 60f;

        internal static bool HasSlingLauncherAndAmmo(Agent agent)
        {
            if (agent == null) return false;
            var eq = agent.Equipment;
            if (eq == null) return false;

            bool hasLauncher = false;
            bool hasAmmo = false;

            for (EquipmentIndex idx = EquipmentIndex.WeaponItemBeginSlot; idx < EquipmentIndex.NumAllWeaponSlots; idx++)
            {
                MissionWeapon mw = eq[idx];
                if (mw.IsEmpty) continue;

                var item = mw.Item;
                if (item?.Weapons == null) continue;

                foreach (var w in item.Weapons)
                {
                    if (w == null) continue;

                    // Launcher
                    if (w.WeaponClass == WeaponClass.Sling)
                        hasLauncher = true;

                    // Ammo
                    if (w.WeaponClass == WeaponClass.SlingStone && mw.Amount > 0)
                        hasAmmo = true;
                }

                if (hasLauncher && hasAmmo)
                    return true;
            }

            return false;
        }

        internal static bool HasAnyNonSlingThrown(Agent agent)
        {
            if (agent == null) return false;
            var eq = agent.Equipment;
            if (eq == null) return false;

            for (EquipmentIndex idx = EquipmentIndex.WeaponItemBeginSlot; idx < EquipmentIndex.NumAllWeaponSlots; idx++)
            {
                MissionWeapon mw = eq[idx];
                if (mw.IsEmpty) continue;

                var item = mw.Item;
                if (item?.Weapons == null) continue;

                foreach (var w in item.Weapons)
                {
                    if (w == null) continue;

                    // Thrown definition in native is basically: ranged + consumable.
                    // We want to ignore SlingStone here, but keep everything else.
                    if (w.IsRangedWeapon && w.IsConsumable)
                    {
                        if (w.WeaponClass != WeaponClass.SlingStone)
                            return true;
                    }

                    switch (w.WeaponClass)
                    {
                        case WeaponClass.Javelin:
                        case WeaponClass.ThrowingAxe:
                        case WeaponClass.ThrowingKnife:
                        case WeaponClass.Stone:
                            return true;
                    }
                }
            }

            return false;
        }
    }

    // 1) Make sling troops count as ranged (the “bow troop” classification gate).
    [HarmonyPatch(typeof(Agent), "get_IsRangedCached")]
    internal static class Agent_IsRangedCached_SlingPrefix
    {
        private static bool Prefix(Agent __instance, ref bool __result)
        {
            if (SlingAi.HasSlingLauncherAndAmmo(__instance))
            {
                __result = true;
                return false; // skip native
            }
            return true;
        }
    }

    // 2) Prevent sling-only troops from being treated as throwers/skirmishers.
    [HarmonyPatch(typeof(Agent), "get_HasThrownCached")]
    internal static class Agent_HasThrownCached_SlingPrefix
    {
        private static bool Prefix(Agent __instance, ref bool __result)
        {
            if (!SlingAi.HasSlingLauncherAndAmmo(__instance))
                return true;

            // If they ALSO have real thrown weapons, don’t override vanilla.
            if (SlingAi.HasAnyNonSlingThrown(__instance))
                return true;

            __result = false;
            return false; // skip native
        }
    }

    // 3) (Optional but usually necessary) Clamp the AI’s missile range values upward.
    // This only affects what AI *thinks* its range envelope is.
    [HarmonyPatch(typeof(Agent), nameof(Agent.GetMissileRange))]
    internal static class Agent_GetMissileRange_SlingPrefix
    {
        private static bool Prefix(Agent __instance, ref float __result)
        {
            if (SlingAi.HasSlingLauncherAndAmmo(__instance))
            {
                __result = SlingAi.DesiredRangeMeters;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Agent), "get_MissileRangeAdjusted")]
    internal static class Agent_MissileRangeAdjusted_SlingPrefix
    {
        private static bool Prefix(Agent __instance, ref float __result)
        {
            if (SlingAi.HasSlingLauncherAndAmmo(__instance))
            {
                __result = SlingAi.DesiredRangeMeters;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Agent), "get_MaximumMissileRange")]
    internal static class Agent_MaximumMissileRange_SlingPrefix
    {
        private static bool Prefix(Agent __instance, ref float __result)
        {
            if (SlingAi.HasSlingLauncherAndAmmo(__instance))
            {
                __result = SlingAi.DesiredRangeMeters;
                return false;
            }
            return true;
        }
    }
}
