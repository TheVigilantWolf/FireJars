using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace FireJars.Patches
{
    [HarmonyPatch(typeof(EquipmentElement), nameof(EquipmentElement.GetModifiedStackCountForUsage))]
    internal static class EquipmentElementGetModifiedStackCountForUsagePatch
    {
        private const string FireJarId = "firejar";
        private const string SlingAmmoId = "sling_firejarammo";

        private static void Postfix(EquipmentElement __instance, ref short __result)
        {
            var item = __instance.Item;
            if (item == null)
                return;

            var settings = SettingsUtil.TryGetSettings();
            if (settings == null)
                return;

            int desired;
            if (item.StringId == FireJarId)
                desired = settings.FireJarStackAmount;
            else if (item.StringId == SlingAmmoId)
                desired = settings.SlingAmmoStackAmount;
            else
                return;

            desired = MBMath.ClampInt(desired, 1, 20);
            __result = (short)desired;
        }
    }
}
