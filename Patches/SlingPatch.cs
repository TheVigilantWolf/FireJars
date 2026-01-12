using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace FireJars.Patches
{
    internal static class SlingAiUtil
    {
        internal static bool HasSlingLauncher(Agent a)
        {
            if (a?.Equipment == null) return false;

            for (int i = (int)EquipmentIndex.WeaponItemBeginSlot; i < (int)EquipmentIndex.NumAllWeaponSlots; i++)
            {
                var w = a.Equipment[(EquipmentIndex)i];
                if (w.IsEmpty) continue;

                var u = w.CurrentUsageItem;
                if (u != null && u.WeaponClass == WeaponClass.Sling)
                    return true;
            }

            return false;
        }

        // Returns true if the agent has a real thrown weapon OTHER than sling ammo.
        internal static bool HasNonSlingThrown(Agent a)
        {
            if (a?.Equipment == null) return false;

            for (int i = (int)EquipmentIndex.WeaponItemBeginSlot; i < (int)EquipmentIndex.NumAllWeaponSlots; i++)
            {
                var w = a.Equipment[(EquipmentIndex)i];
                if (w.IsEmpty) continue;

                var u = w.CurrentUsageItem;
                if (u == null) continue;

                // Engine definition: ranged + consumable => thrown (from MissionWeapon.GatherInformationFromWeapon)
                if (u.IsRangedWeapon && u.IsConsumable)
                {
                    // Ignore sling ammo so slingmen don't become "throwers"
                    if (u.WeaponClass == WeaponClass.SlingStone)
                        continue;

                    // OPTIONAL: if your sling ammo is authored as WeaponClass.Stone, uncomment this.
                    // if (u.WeaponClass == WeaponClass.Stone) continue;

                    return true;
                }

                // Also catch classic thrown classes explicitly
                switch (u.WeaponClass)
                {
                    case WeaponClass.Javelin:
                    case WeaponClass.ThrowingAxe:
                    case WeaponClass.ThrowingKnife:
                    case WeaponClass.Stone:
                        return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Key behavior fix:
    /// If an agent has a sling launcher and no other thrown weapons,
    /// report HasThrownCached=false so the AI doesn't treat them as "throwers/skirmishers".
    /// </summary>
    [HarmonyPatch(typeof(Agent), nameof(Agent.HasThrownCached), MethodType.Getter)]
    internal static class Sling_HasThrownCached_Prefix
    {
        static bool Prefix(Agent __instance, ref bool __result)
        {
            if (!SlingAiUtil.HasSlingLauncher(__instance))
                return true; // vanilla

            // If they also carry actual thrown weapons, keep vanilla behavior.
            if (SlingAiUtil.HasNonSlingThrown(__instance))
                return true; // vanilla

            __result = false;
            return false; // override
        }
    }

    /// <summary>
    /// Classification fix:
    /// Sling carriers count as ranged, even if they spawn holding a sword.
    /// </summary>
    [HarmonyPatch(typeof(Agent), nameof(Agent.IsRangedCached), MethodType.Getter)]
    internal static class Sling_IsRangedCached_Prefix
    {
        static bool Prefix(Agent __instance, ref bool __result)
        {
            if (SlingAiUtil.HasSlingLauncher(__instance))
            {
                __result = true;
                return false; // override
            }

            return true; // vanilla
        }
    }
}
