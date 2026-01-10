using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace FireJars
{
    internal static class ItemDefinitionApplier
    {
        private const string FireJarId = "firejar";
        private const string SlingAmmoId = "sling_firejarammo";

        private static readonly Dictionary<string, int> _originalDamage = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> _originalValue = new Dictionary<string, int>();

        internal static void ApplyAll()
        {
            var settings = SettingsUtil.TryGetSettings();
            if (settings == null)
                return;

            ApplyStacks(FireJarId, settings.FireJarStackAmount);
            ApplyStacks(SlingAmmoId, settings.SlingAmmoStackAmount);

            ApplyFireDamage(FireJarId, settings.FireJarDamagePercent);
            ApplyFireDamage(SlingAmmoId, settings.SlingAmmoDamagePercent);

            ApplyValue(FireJarId, settings.FireJarPricePercent);
            ApplyValue(SlingAmmoId, settings.SlingAmmoPricePercent);
        }

        private static void ApplyStacks(string itemId, int desired)
        {
            var item = MBObjectManager.Instance.GetObject<ItemObject>(itemId);
            if (item == null)
                return;

            var clamped = MBMath.ClampInt(desired, 1, 20);
            var weapons = GetWeapons(item);
            foreach (var w in weapons)
            {
                SetIntMember(w, new[] { "StackAmount", "MaxAmmo", "Ammo", "Amount", "MaxDataValue", "_stackAmount", "_maxDataValue", "_ammo", "_amount" }, clamped);
            }
        }

        private static void ApplyFireDamage(string itemId, float percent)
        {
            var item = MBObjectManager.Instance.GetObject<ItemObject>(itemId);
            if (item == null)
                return;

            var weapons = GetWeapons(item);
            foreach (var w in weapons)
            {
                var prop = GetIntProperty(w, "FireDamage");
                if (prop == null)
                    continue;

                var key = itemId + "::FireDamage";
                if (!_originalDamage.TryGetValue(key, out var original))
                {
                    original = (int)prop.GetValue(w);
                    _originalDamage[key] = original;
                }

                var scaled = (int)System.Math.Round(original * (percent / 100f));
                prop.SetValue(w, scaled);
            }
        }

        private static void ApplyValue(string itemId, float percent)
        {
            var item = MBObjectManager.Instance.GetObject<ItemObject>(itemId);
            if (item == null)
                return;

            var key = itemId + "::Value";
            if (!_originalValue.TryGetValue(key, out var original))
            {
                original = item.Value;
                _originalValue[key] = original;
            }

            var scaled = (int)System.Math.Round(original * (percent / 100f));
            if (scaled < 0)
                scaled = 0;

            var valueProp = item.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (valueProp != null && valueProp.CanWrite && valueProp.PropertyType == typeof(int))
            {
                valueProp.SetValue(item, scaled);
            }
            else
            {
                var field = item.GetType().GetField("_value", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null && field.FieldType == typeof(int))
                    field.SetValue(item, scaled);
            }
        }

        private static IEnumerable<object> GetWeapons(ItemObject item)
        {
            if (item.PrimaryWeapon != null)
                yield return item.PrimaryWeapon;

            var weaponsProp = item.GetType().GetProperty("Weapons", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (weaponsProp != null)
            {
                if (weaponsProp.GetValue(item) is System.Collections.IEnumerable list)
                {
                    foreach (var w in list)
                        yield return w;
                }
            }
        }

        private static void SetIntMember(object target, string[] names, int value)
        {
            if (target == null)
                return;

            var type = target.GetType();
            foreach (var name in names)
            {
                var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && prop.CanWrite && (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(short)))
                {
                    if (prop.PropertyType == typeof(short))
                        prop.SetValue(target, (short)value);
                    else
                        prop.SetValue(target, value);
                    return;
                }

                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null && (field.FieldType == typeof(int) || field.FieldType == typeof(short)))
                {
                    if (field.FieldType == typeof(short))
                        field.SetValue(target, (short)value);
                    else
                        field.SetValue(target, value);
                    return;
                }
            }
        }

        private static PropertyInfo GetIntProperty(object target, string name)
        {
            if (target == null)
                return null;

            var prop = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop == null || !prop.CanRead || !prop.CanWrite)
                return null;

            if (prop.PropertyType != typeof(int) && prop.PropertyType != typeof(short))
                return null;

            return prop;
        }
    }
}
