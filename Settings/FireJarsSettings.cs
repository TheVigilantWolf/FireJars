using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace FireJars
{
    internal sealed class FireJarsSettings : AttributeGlobalSettings<FireJarsSettings>
    {
        public override string Id => "FireJarsSettings";
        public override string DisplayName => "Fire Jars & Sling Ammo Settings";
        public override string FolderName => "FireJars";

        public override string FormatType => "json";

        [SettingPropertyInteger("Sling Ammo Stack Amount", 1, 20, valueFormat: "{0}", Order = 0, RequireRestart = false, HintText = "Sets Sling ammo stack size. Vanilla default: 10.")]
        [SettingPropertyGroup("Sling Ammo")]
        public int SlingAmmoStackAmount { get; set; } = 10;

        [SettingPropertyFloatingInteger("Sling Ammo Damage %", 50f, 200f, valueFormat: "{0:0}%", Order = 1, RequireRestart = false, HintText = "Damage multiplier for Sling ammo. Vanilla damage: 20.")]
        [SettingPropertyGroup("Sling Ammo")]
        public float SlingAmmoDamagePercent { get; set; } = 100f;

        [SettingPropertyFloatingInteger("Sling Ammo Price %", 25f, 300f, valueFormat: "{0:0}%", Order = 2, RequireRestart = false, HintText = "Price multiplier for Sling ammo. Vanilla value: 1500.")]
        [SettingPropertyGroup("Sling Ammo")]
        public float SlingAmmoPricePercent { get; set; } = 100f;

        [SettingPropertyInteger("Fire Jar Stack Amount", 1, 20, valueFormat: "{0}", Order = 10, RequireRestart = false, HintText = "Sets Fire Jar stack size. Vanilla default: 4.")]
        [SettingPropertyGroup("Fire Jar")]
        public int FireJarStackAmount { get; set; } = 4;

        [SettingPropertyFloatingInteger("Fire Jar Damage %", 50f, 200f, valueFormat: "{0:0}%", Order = 11, RequireRestart = false, HintText = "Damage multiplier for Fire Jars. Vanilla damage: 32.")]
        [SettingPropertyGroup("Fire Jar")]
        public float FireJarDamagePercent { get; set; } = 100f;

        [SettingPropertyFloatingInteger("Fire Jar Price %", 25f, 300f, valueFormat: "{0:0}%", Order = 12, RequireRestart = false, HintText = "Price multiplier for Fire Jars. Vanilla value: 1000.")]
        [SettingPropertyGroup("Fire Jar")]
        public float FireJarPricePercent { get; set; } = 100f;

        [SettingPropertyDropdown("Fire Jar Culture", Order = 20, RequireRestart = false, HintText = "Restricts town stock behavior to this culture. 'All' applies to all towns.")]
        [SettingPropertyGroup("Fire Jar")]
        public Dropdown<string> FireJarCulture { get; set; } = new Dropdown<string>(
            new[]
            {
                "All",
                "Empire",
                "Sturgia",
                "Aserai",
                "Vlandia",
                "Battania",
                "Khuzait",
                "Nord"
            },
            selectedIndex: 0);
    }
}
