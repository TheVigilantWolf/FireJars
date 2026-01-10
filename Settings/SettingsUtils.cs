using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.ObjectSystem;

namespace FireJars
{
    internal static class SettingsUtil
    {
        private const string AllCulturesLabel = "All";

        /// <summary>
        /// Safely gets the settings instance. If MCM isn't present or settings fail,
        /// returns null and your mod should fall back to defaults.
        /// </summary>
        public static FireJarsSettings TryGetSettings()
        {
            try { return FireJarsSettings.Instance; }
            catch { return null; }
        }

        /// <summary>
        /// Returns the currently selected culture dropdown value (e.g. "Empire", "All").
        /// Defaults to "All" if settings aren't available.
        /// </summary>
        public static string GetSelectedCultureString()
        {
            var settings = TryGetSettings();
            if (settings == null)
                return AllCulturesLabel;

            try
            {
                return settings.FireJarCulture != null ? (settings.FireJarCulture.SelectedValue ?? AllCulturesLabel) : AllCulturesLabel;
            }
            catch
            {
                return AllCulturesLabel;
            }
        }

        /// <summary>
        /// Checks whether a settlement matches the selected culture rule.
        /// "All" => always true.
        /// </summary>
        public static bool SettlementMatchesSelectedCulture(Settlement settlement)
        {
            if (settlement == null)
                return false;

            var requiredCulture = ResolveCulture(GetSelectedCultureString());
            return requiredCulture == null || settlement.Culture == requiredCulture;
        }

        /// <summary>
        /// Maps your dropdown strings to Bannerlord culture IDs.
        /// NOTE: "Nord" is NOT a native Bannerlord culture. If you don't have a custom "nord"
        /// culture defined in XML, this will resolve to null.
        /// </summary>
        public static TaleWorlds.CampaignSystem.CultureObject ResolveCulture(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Equals(AllCulturesLabel, StringComparison.OrdinalIgnoreCase))
                return null;

            string cultureId;
            switch (value)
            {
                case "Empire":
                    cultureId = "empire";
                    break;
                case "Sturgia":
                    cultureId = "sturgia";
                    break;
                case "Aserai":
                    cultureId = "aserai";
                    break;
                case "Vlandia":
                    cultureId = "vlandia";
                    break;
                case "Battania":
                    cultureId = "battania";
                    break;
                case "Khuzait":
                    cultureId = "khuzait";
                    break;
                case "Nord":
                    cultureId = "nord";
                    break;
                default:
                    cultureId = null;
                    break;
            }

            if (cultureId == null)
                return null;

            return MBObjectManager.Instance.GetObject<TaleWorlds.CampaignSystem.CultureObject>(cultureId);
        }
    }
}
