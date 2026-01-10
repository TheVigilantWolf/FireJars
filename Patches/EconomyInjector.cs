using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace FireJars.Patches
{
    internal static class EconomyInjector
    {
        private const string FireJarId = "firejar";
        private const string SlingAmmoId = "sling_firejarammo";

        // Conservative defaults; can be expanded into MCM later.
        private const int DefaultMaxFireJarsInTown = 6;
        private const int DefaultMaxSlingAmmoInTown = 8;

        // High-tier feel: low daily restock chance.
        private const float DefaultDailyRestockChanceFireJar = 0.18f;
        private const float DefaultDailyRestockChanceSlingAmmo = 0.25f;

        private static ItemObject _fireJar;
        private static ItemObject _slingAmmo;

        private static ItemObject GetItemCached(ref ItemObject cache, string id)
        {
            if (cache != null)
                return cache;

            cache = MBObjectManager.Instance.GetObject<ItemObject>(id);
            return cache;
        }

        internal static void EnsureTownStock(Town town)
        {
            if (town == null || town.Owner == null || town.Owner.ItemRoster == null)
                return;

            if (Campaign.Current == null)
                return;

            if (!SettingsUtil.SettlementMatchesSelectedCulture(town.Settlement))
                return;

            var roster = town.Owner.ItemRoster;

            var fireJar = GetItemCached(ref _fireJar, FireJarId);
            var slingAmmo = GetItemCached(ref _slingAmmo, SlingAmmoId);

            if (fireJar != null)
                EnsureFluctuatingStock(roster, fireJar, DefaultMaxFireJarsInTown, DefaultDailyRestockChanceFireJar, maxAddPerDay: 1);

            if (slingAmmo != null)
                EnsureFluctuatingStock(roster, slingAmmo, DefaultMaxSlingAmmoInTown, DefaultDailyRestockChanceSlingAmmo, maxAddPerDay: 2);
        }

        private static void EnsureFluctuatingStock(ItemRoster roster, ItemObject item, int max, float dailyChance, int maxAddPerDay)
        {
            var current = roster.GetItemNumber(item);

            // Hard cap: never allow absurd amounts.
            if (current > max)
            {
                roster.AddToCounts(item, -(current - max));
                current = max;
            }

            // No restock if already at/near max.
            if (current >= max)
                return;

            // Only sometimes restock, to keep it rare.
            if (MBRandom.RandomFloat > dailyChance)
                return;

            // Add a small random amount to create stock fluctuation.
            // Example: fire jar +1, sling ammo +1..+2.
            var add = 1;
            if (maxAddPerDay > 1)
                add = 1 + MBRandom.RandomInt(System.Math.Min(maxAddPerDay, max - current));

            // Final clamp.
            if (current + add > max)
                add = max - current;

            if (add > 0)
                roster.AddToCounts(item, add);
        }

        internal static void ClampTownStockAfterConsumption(Town town)
        {
            if (town == null || town.Owner == null || town.Owner.ItemRoster == null)
                return;

            var roster = town.Owner.ItemRoster;

            var fireJar = GetItemCached(ref _fireJar, FireJarId);
            var slingAmmo = GetItemCached(ref _slingAmmo, SlingAmmoId);

            if (fireJar != null)
                Clamp(roster, fireJar, DefaultMaxFireJarsInTown);

            if (slingAmmo != null)
                Clamp(roster, slingAmmo, DefaultMaxSlingAmmoInTown);
        }

        private static void Clamp(ItemRoster roster, ItemObject item, int max)
        {
            var current = roster.GetItemNumber(item);
            if (current <= max)
                return;

            roster.AddToCounts(item, -(current - max));
        }
    }

    [HarmonyPatch(typeof(TradeCampaignBehavior), "DailyTickTown")]
    internal static class TradeCampaignBehaviorDailyTickTownPatch
    {
        private static void Postfix(Town town)
        {
            EconomyInjector.EnsureTownStock(town);
        }
    }

    [HarmonyPatch(typeof(ItemConsumptionBehavior), "DailyTickTown")]
    internal static class ItemConsumptionBehaviorDailyTickTownPatch
    {
        private static void Postfix(Town town)
        {
            EconomyInjector.ClampTownStockAfterConsumption(town);
        }
    }
}
