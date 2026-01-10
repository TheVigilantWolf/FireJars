using HarmonyLib;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace FireJars.Patches
{
    [HarmonyPatch(typeof(TownMarketData), "GetPrice", new[] { typeof(EquipmentElement), typeof(TaleWorlds.CampaignSystem.Party.MobileParty), typeof(bool), typeof(TaleWorlds.CampaignSystem.Party.PartyBase) })]
    internal static class TownMarketDataGetPricePatch
    {
        private const string FireJarId = "firejar";
        private const string SlingAmmoId = "sling_firejarammo";

        private static void Postfix(EquipmentElement itemRosterElement, ref int __result)
        {
            var item = itemRosterElement.Item;
            if (item == null)
                return;

            var settings = SettingsUtil.TryGetSettings();
            if (settings == null)
                return;

            var id = item.StringId;
            var percent = -1f;

            if (id == FireJarId)
                percent = settings.FireJarPricePercent;
            else if (id == SlingAmmoId)
                percent = settings.SlingAmmoPricePercent;

            if (percent <= 0f)
                return;

            var multiplier = percent / 100f;
            __result = (int) (0.5f + __result * multiplier);
            if (__result < 1)
                __result = 1;
        }
    }
}
