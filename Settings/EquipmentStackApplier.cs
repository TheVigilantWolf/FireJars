using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace FireJars
{
    internal static class EquipmentStackApplier
    {
        internal static void ApplyToHero(Hero hero)
        {
            if (hero == null)
                return;

            var settings = SettingsUtil.TryGetSettings();
            if (settings == null)
                return;

            var desiredFireJar = MBMath.ClampInt(settings.FireJarStackAmount, 1, 20);
            var desiredSlingAmmo = MBMath.ClampInt(settings.SlingAmmoStackAmount, 1, 20);

            ApplyToEquipment(hero.BattleEquipment, desiredFireJar, desiredSlingAmmo);
            ApplyToEquipment(hero.CivilianEquipment, desiredFireJar, desiredSlingAmmo);
        }

        private static void ApplyToEquipment(Equipment equipment, int desiredFireJar, int desiredSlingAmmo)
        {
            if (equipment == null)
                return;

            foreach (EquipmentIndex idx in Enum.GetValues(typeof(EquipmentIndex)))
            {
                try
                {
                    var elem = equipment[idx];
                    var item = elem.Item;
                    if (item == null)
                        continue;

                    if (item.StringId == "firejar")
                    {
                        SetIntMember(ref elem, desiredFireJar);
                        equipment[idx] = elem;
                    }
                    else if (item.StringId == "sling_firejarammo")
                    {
                        SetIntMember(ref elem, desiredSlingAmmo);
                        equipment[idx] = elem;
                    }
                }
                catch
                {
                    // ignore invalid slots for this enum value
                }
            }
        }

        private static void SetIntMember(ref EquipmentElement elem, int value)
        {
            var names = new[] { "Amount", "StackCount", "MaxAmmo", "Ammo", "_amount", "_stackCount", "_maxDataValue", "_ammo" };
            var type = typeof(EquipmentElement);
            foreach (var name in names)
            {
                var prop = type.GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (prop != null && prop.CanWrite && (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(short)))
                {
                    if (prop.PropertyType == typeof(short))
                        prop.SetValue(elem, (short)value);
                    else
                        prop.SetValue(elem, value);
                    return;
                }

                var field = type.GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (field != null && (field.FieldType == typeof(int) || field.FieldType == typeof(short)))
                {
                    if (field.FieldType == typeof(short))
                        field.SetValue(elem, (short)value);
                    else
                        field.SetValue(elem, value);
                    return;
                }
            }
        }
    }
}
