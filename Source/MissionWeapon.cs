//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.MissionWeapon
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using TaleWorlds.Core;
//using TaleWorlds.Engine;
//using TaleWorlds.Library;
//using TaleWorlds.Localization;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public struct MissionWeapon
//  {
//    public const short ReloadPhaseCountMax = 10;
//    public static MissionWeapon.OnGetWeaponDataDelegate OnGetWeaponDataHandler;
//    public static readonly MissionWeapon Invalid = new MissionWeapon((ItemObject) null, (ItemModifier) null, (Banner) null);
//    private readonly List<WeaponComponentData> _weapons;
//    public int CurrentUsageIndex;
//    private bool _hasAnyConsumableUsage;
//    private short _dataValue;
//    private short _modifiedMaxDataValue;
//    private MissionWeapon.MissionSubWeapon _ammoWeapon;
//    private List<MissionWeapon.MissionSubWeapon> _attachedWeapons;
//    private List<MatrixFrame> _attachedWeaponFrames;

//    public ItemObject Item { get; private set; }

//    public ItemModifier ItemModifier { get; private set; }

//    public int WeaponsCount => this._weapons.Count;

//    public WeaponComponentData CurrentUsageItem
//    {
//      get
//      {
//        return this._weapons == null || this._weapons.Count == 0 ? (WeaponComponentData) null : this._weapons[this.CurrentUsageIndex];
//      }
//    }

//    public short ReloadPhase { get; set; }

//    public short ReloadPhaseCount
//    {
//      get
//      {
//        short reloadPhaseCount = 1;
//        if (this.CurrentUsageItem != null)
//          reloadPhaseCount = this.CurrentUsageItem.ReloadPhaseCount;
//        return reloadPhaseCount;
//      }
//    }

//    public bool IsReloading => (int) this.ReloadPhase < (int) this.ReloadPhaseCount;

//    public Banner Banner { get; private set; }

//    public float GlossMultiplier { get; private set; }

//    public short RawDataForNetwork => this._dataValue;

//    public short HitPoints
//    {
//      get => this._dataValue;
//      set => this._dataValue = value;
//    }

//    public short Amount
//    {
//      get => this._dataValue;
//      set => this._dataValue = value;
//    }

//    public short Ammo
//    {
//      get
//      {
//        MissionWeapon.MissionSubWeapon ammoWeapon = this._ammoWeapon;
//        return ammoWeapon == null ? (short) 0 : ammoWeapon.Value._dataValue;
//      }
//    }

//    public MissionWeapon AmmoWeapon
//    {
//      get
//      {
//        MissionWeapon.MissionSubWeapon ammoWeapon = this._ammoWeapon;
//        return ammoWeapon == null ? MissionWeapon.Invalid : ammoWeapon.Value;
//      }
//    }

//    public short MaxAmmo => this._modifiedMaxDataValue;

//    public short ModifiedMaxAmount => this._modifiedMaxDataValue;

//    public short ModifiedMaxHitPoints => this._modifiedMaxDataValue;

//    public bool IsEmpty => this.CurrentUsageItem == null;

//    public MissionWeapon(ItemObject item, ItemModifier itemModifier, Banner banner)
//    {
//      this.Item = item;
//      this.ItemModifier = itemModifier;
//      this.Banner = banner;
//      this.CurrentUsageIndex = 0;
//      this._weapons = new List<WeaponComponentData>(1);
//      this._modifiedMaxDataValue = (short) 0;
//      this._hasAnyConsumableUsage = false;
//      if (item != null && item.Weapons != null)
//      {
//        foreach (WeaponComponentData weapon in (List<WeaponComponentData>) item.Weapons)
//        {
//          this._weapons.Add(weapon);
//          bool isConsumable = weapon.IsConsumable;
//          if (isConsumable || weapon.IsRangedWeapon || weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.HasHitPoints))
//          {
//            this._modifiedMaxDataValue = weapon.MaxDataValue;
//            if (itemModifier != null)
//            {
//              if (weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.HasHitPoints))
//                this._modifiedMaxDataValue = weapon.GetModifiedMaximumHitPoints(itemModifier);
//              else if (isConsumable)
//                this._modifiedMaxDataValue = weapon.GetModifiedStackCount(itemModifier);
//            }
//          }
//          if (isConsumable)
//            this._hasAnyConsumableUsage = true;
//        }
//      }
//      this._dataValue = this._modifiedMaxDataValue;
//      this.ReloadPhase = (short) 0;
//      this._ammoWeapon = (MissionWeapon.MissionSubWeapon) null;
//      this._attachedWeapons = (List<MissionWeapon.MissionSubWeapon>) null;
//      this._attachedWeaponFrames = (List<MatrixFrame>) null;
//      this.GlossMultiplier = 1f;
//    }

//    public MissionWeapon(
//      ItemObject primaryItem,
//      ItemModifier itemModifier,
//      Banner banner,
//      short dataValue)
//      : this(primaryItem, itemModifier, banner)
//    {
//      this._dataValue = dataValue;
//    }

//    public MissionWeapon(
//      ItemObject primaryItem,
//      ItemModifier itemModifier,
//      Banner banner,
//      short dataValue,
//      short reloadPhase,
//      MissionWeapon? ammoWeapon)
//      : this(primaryItem, itemModifier, banner, dataValue)
//    {
//      this.ReloadPhase = reloadPhase;
//      this._ammoWeapon = ammoWeapon.HasValue ? new MissionWeapon.MissionSubWeapon(ammoWeapon.Value) : (MissionWeapon.MissionSubWeapon) null;
//    }

//    public TextObject GetModifiedItemName()
//    {
//      if (this.ItemModifier == null)
//        return this.Item.Name;
//      TextObject name = this.ItemModifier.Name;
//      name.SetTextVariable("ITEMNAME", this.Item.Name);
//      return name;
//    }

//    public bool IsEqualTo(MissionWeapon other) => this.Item == other.Item;

//    public bool IsSameType(MissionWeapon other)
//    {
//      return this.Item.PrimaryWeapon.WeaponClass == other.Item.PrimaryWeapon.WeaponClass;
//    }

//    public float GetWeight()
//    {
//      double num1 = this.Item.PrimaryWeapon.IsConsumable ? (double) this.GetBaseWeight() * (double) this._dataValue : (double) this.GetBaseWeight();
//      MissionWeapon.MissionSubWeapon ammoWeapon = this._ammoWeapon;
//      double num2 = ammoWeapon != null ? (double) ammoWeapon.Value.GetWeight() : 0.0;
//      return (float) (num1 + num2);
//    }

//    private float GetBaseWeight() => this.Item.Weight;

//    public WeaponComponentData GetWeaponComponentDataForUsage(int usageIndex)
//    {
//      return this._weapons[usageIndex];
//    }

//    public int GetGetModifiedArmorForCurrentUsage()
//    {
//      return this._weapons[this.CurrentUsageIndex].GetModifiedArmor(this.ItemModifier);
//    }

//    public int GetModifiedThrustDamageForCurrentUsage()
//    {
//      return this._weapons[this.CurrentUsageIndex].GetModifiedThrustDamage(this.ItemModifier);
//    }

//    public int GetModifiedSwingDamageForCurrentUsage()
//    {
//      return this._weapons[this.CurrentUsageIndex].GetModifiedSwingDamage(this.ItemModifier);
//    }

//    public int GetModifiedMissileDamageForCurrentUsage()
//    {
//      return this._weapons[this.CurrentUsageIndex].GetModifiedMissileDamage(this.ItemModifier);
//    }

//    public int GetModifiedThrustSpeedForCurrentUsage()
//    {
//      return this._weapons[this.CurrentUsageIndex].GetModifiedThrustSpeed(this.ItemModifier);
//    }

//    public int GetModifiedSwingSpeedForCurrentUsage()
//    {
//      return this._weapons[this.CurrentUsageIndex].GetModifiedSwingSpeed(this.ItemModifier);
//    }

//    public int GetModifiedMissileSpeedForCurrentUsage()
//    {
//      return this._weapons[this.CurrentUsageIndex].GetModifiedMissileSpeed(this.ItemModifier);
//    }

//    public int GetModifiedMissileSpeedForUsage(int usageIndex)
//    {
//      return this._weapons[usageIndex].GetModifiedMissileSpeed(this.ItemModifier);
//    }

//    public int GetModifiedHandlingForCurrentUsage()
//    {
//      return this._weapons[this.CurrentUsageIndex].GetModifiedHandling(this.ItemModifier);
//    }

//    public WeaponData GetWeaponData(bool needBatchedVersionForMeshes)
//    {
//      if (this.IsEmpty || this.Item.WeaponComponent == null)
//        return WeaponData.InvalidWeaponData;
//      WeaponComponent weaponComponent = this.Item.WeaponComponent;
//      WeaponData weaponData = new WeaponData()
//      {
//        WeaponKind = (int) this.Item.Id.InternalValue,
//        ItemHolsterIndices = this.Item.GetItemHolsterIndices(),
//        ReloadPhase = this.ReloadPhase,
//        Difficulty = this.Item.Difficulty,
//        BaseWeight = this.GetBaseWeight(),
//        HasFlagAnimation = false,
//        WeaponFrame = weaponComponent.PrimaryWeapon.Frame,
//        ScaleFactor = this.Item.ScaleFactor,
//        TotalInertia = weaponComponent.PrimaryWeapon.TotalInertia,
//        CenterOfMass = weaponComponent.PrimaryWeapon.CenterOfMass,
//        CenterOfMass3D = weaponComponent.PrimaryWeapon.CenterOfMass3D,
//        HolsterPositionShift = this.Item.HolsterPositionShift,
//        TrailParticleName = weaponComponent.PrimaryWeapon.TrailParticleName,
//        SkeletonName = this.Item.SkeletonName,
//        StaticAnimationName = this.Item.StaticAnimationName,
//        AmmoOffset = weaponComponent.PrimaryWeapon.AmmoOffset
//      };
//      string physicsMaterial = weaponComponent.PrimaryWeapon.PhysicsMaterial;
//      weaponData.PhysicsMaterialIndex = string.IsNullOrEmpty(physicsMaterial) ? PhysicsMaterial.InvalidPhysicsMaterial.Index : PhysicsMaterial.GetFromName(physicsMaterial).Index;
//      weaponData.FlyingSoundCode = SoundManager.GetEventGlobalIndex(weaponComponent.PrimaryWeapon.FlyingSoundCode);
//      weaponData.PassbySoundCode = SoundManager.GetEventGlobalIndex(weaponComponent.PrimaryWeapon.PassbySoundCode);
//      weaponData.StickingFrame = weaponComponent.PrimaryWeapon.StickingFrame;
//      weaponData.CollisionShape = !needBatchedVersionForMeshes || string.IsNullOrEmpty(this.Item.CollisionBodyName) ? (PhysicsShape) null : PhysicsShape.GetFromResource(this.Item.CollisionBodyName);
//      weaponData.Shape = !needBatchedVersionForMeshes || string.IsNullOrEmpty(this.Item.BodyName) ? (PhysicsShape) null : PhysicsShape.GetFromResource(this.Item.BodyName);
//      weaponData.DataValue = this._dataValue;
//      weaponData.CurrentUsageIndex = this.CurrentUsageIndex;
//      int rangedUsageIndex = this.GetRangedUsageIndex();
//      WeaponComponentData consumableWeapon;
//      if (this.GetConsumableIfAny(out consumableWeapon))
//        weaponData.AirFrictionConstant = ItemObject.GetAirFrictionConstant(consumableWeapon.WeaponClass, consumableWeapon.WeaponFlags);
//      else if (rangedUsageIndex >= 0)
//        weaponData.AirFrictionConstant = ItemObject.GetAirFrictionConstant(this.GetWeaponComponentDataForUsage(rangedUsageIndex).WeaponClass, this.GetWeaponComponentDataForUsage(rangedUsageIndex).WeaponFlags);
//      weaponData.GlossMultiplier = this.GlossMultiplier;
//      weaponData.HasLowerHolsterPriority = this.Item.HasLowerHolsterPriority;
//      MissionWeapon.OnGetWeaponDataDelegate weaponDataHandler = MissionWeapon.OnGetWeaponDataHandler;
//      if (weaponDataHandler != null)
//        weaponDataHandler(ref weaponData, this, false, this.Banner, needBatchedVersionForMeshes);
//      return weaponData;
//    }

//    public WeaponStatsData[] GetWeaponStatsData()
//    {
//      WeaponStatsData[] weaponStatsData = new WeaponStatsData[this._weapons.Count];
//      for (int usageIndex = 0; usageIndex < weaponStatsData.Length; ++usageIndex)
//        weaponStatsData[usageIndex] = this.GetWeaponStatsDataForUsage(usageIndex);
//      return weaponStatsData;
//    }

//    public WeaponStatsData GetWeaponStatsDataForUsage(int usageIndex)
//    {
//      WeaponStatsData statsDataForUsage = new WeaponStatsData();
//      WeaponComponentData weapon = this._weapons[usageIndex];
//      statsDataForUsage.WeaponClass = (int) weapon.WeaponClass;
//      statsDataForUsage.AmmoClass = (int) weapon.AmmoClass;
//      statsDataForUsage.Properties = (uint) this.Item.ItemFlags;
//      statsDataForUsage.WeaponFlags = (ulong) weapon.WeaponFlags;
//      statsDataForUsage.ItemUsageIndex = string.IsNullOrEmpty(weapon.ItemUsage) ? -1 : weapon.GetItemUsageIndex();
//      statsDataForUsage.ThrustSpeed = weapon.GetModifiedThrustSpeed(this.ItemModifier);
//      statsDataForUsage.SwingSpeed = weapon.GetModifiedSwingSpeed(this.ItemModifier);
//      statsDataForUsage.MissileSpeed = weapon.GetModifiedMissileSpeed(this.ItemModifier);
//      statsDataForUsage.ShieldArmor = weapon.GetModifiedArmor(this.ItemModifier);
//      statsDataForUsage.Accuracy = weapon.Accuracy;
//      statsDataForUsage.WeaponLength = weapon.WeaponLength;
//      statsDataForUsage.WeaponBalance = weapon.WeaponBalance;
//      statsDataForUsage.ThrustDamage = weapon.GetModifiedThrustDamage(this.ItemModifier);
//      statsDataForUsage.ThrustDamageType = (int) weapon.ThrustDamageType;
//      statsDataForUsage.SwingDamage = weapon.GetModifiedSwingDamage(this.ItemModifier);
//      statsDataForUsage.SwingDamageType = (int) weapon.SwingDamageType;
//      statsDataForUsage.DefendSpeed = weapon.GetModifiedHandling(this.ItemModifier);
//      statsDataForUsage.SweetSpot = weapon.SweetSpotReach;
//      statsDataForUsage.MaxDataValue = this._modifiedMaxDataValue;
//      statsDataForUsage.WeaponFrame = weapon.Frame;
//      statsDataForUsage.RotationSpeed = weapon.RotationSpeed;
//      statsDataForUsage.ReloadPhaseCount = weapon.ReloadPhaseCount;
//      return statsDataForUsage;
//    }

//    public WeaponData GetAmmoWeaponData(bool needBatchedVersion)
//    {
//      return this.AmmoWeapon.GetWeaponData(needBatchedVersion);
//    }

//    public WeaponStatsData[] GetAmmoWeaponStatsData() => this.AmmoWeapon.GetWeaponStatsData();

//    public int GetAttachedWeaponsCount()
//    {
//      List<MissionWeapon.MissionSubWeapon> attachedWeapons = this._attachedWeapons;
//      // ISSUE: explicit non-virtual call
//      return attachedWeapons == null ? 0 : __nonvirtual (attachedWeapons.Count);
//    }

//    public MissionWeapon GetAttachedWeapon(int attachmentIndex)
//    {
//      return this._attachedWeapons[attachmentIndex].Value;
//    }

//    public MatrixFrame GetAttachedWeaponFrame(int attachmentIndex)
//    {
//      return this._attachedWeaponFrames[attachmentIndex];
//    }

//    public bool IsShield() => this._weapons.Count == 1 && this._weapons[0].IsShield;

//    public bool IsBanner()
//    {
//      return this._weapons.Count == 1 && this._weapons[0].WeaponClass == WeaponClass.Banner;
//    }

//    public bool IsAnyAmmo()
//    {
//      foreach (WeaponComponentData weapon in this._weapons)
//      {
//        if (weapon.IsAmmo)
//          return true;
//      }
//      return false;
//    }

//    public bool HasAnyUsageWithWeaponClass(WeaponClass weaponClass)
//    {
//      foreach (WeaponComponentData weapon in this._weapons)
//      {
//        if (weapon.WeaponClass == weaponClass)
//          return true;
//      }
//      return false;
//    }

//    public bool HasAnyUsageWithAmmoClass(WeaponClass ammoClass)
//    {
//      foreach (WeaponComponentData weapon in this._weapons)
//      {
//        if (weapon.AmmoClass == ammoClass)
//          return true;
//      }
//      return false;
//    }

//    public bool HasAllUsagesWithAnyWeaponFlag(WeaponFlags flags)
//    {
//      foreach (WeaponComponentData weapon in this._weapons)
//      {
//        if (!weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(flags))
//          return false;
//      }
//      return true;
//    }

//    public bool HasAnyUsageWithoutWeaponFlag(WeaponFlags flags)
//    {
//      foreach (WeaponComponentData weapon in this._weapons)
//      {
//        if (!weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(flags))
//          return true;
//      }
//      return false;
//    }

//    public void GatherInformationFromWeapon(
//      out bool weaponHasMelee,
//      out bool weaponHasShield,
//      out bool weaponHasPolearm,
//      out bool weaponHasNonConsumableRanged,
//      out bool weaponHasThrown,
//      out WeaponClass rangedAmmoClass)
//    {
//      weaponHasMelee = false;
//      weaponHasShield = false;
//      weaponHasPolearm = false;
//      weaponHasNonConsumableRanged = false;
//      weaponHasThrown = false;
//      rangedAmmoClass = WeaponClass.Undefined;
//      foreach (WeaponComponentData weapon in this._weapons)
//      {
//        weaponHasMelee = weaponHasMelee || weapon.IsMeleeWeapon;
//        weaponHasShield = weaponHasShield || weapon.IsShield;
//        weaponHasPolearm = weapon.IsPolearm;
//        if (weapon.IsRangedWeapon)
//        {
//          weaponHasThrown = weapon.IsConsumable;
//          weaponHasNonConsumableRanged = !weaponHasThrown;
//          rangedAmmoClass = weapon.AmmoClass;
//        }
//      }
//    }

//    public bool GetConsumableIfAny(out WeaponComponentData consumableWeapon)
//    {
//      consumableWeapon = (WeaponComponentData) null;
//      if (!this._hasAnyConsumableUsage)
//        return false;
//      foreach (WeaponComponentData weapon in this._weapons)
//      {
//        if (weapon.IsConsumable)
//        {
//          consumableWeapon = weapon;
//          break;
//        }
//      }
//      return true;
//    }

//    public bool IsAnyConsumable() => this._hasAnyConsumableUsage;

//    public int GetRangedUsageIndex()
//    {
//      for (int index = 0; index < this._weapons.Count; ++index)
//      {
//        if (this._weapons[index].IsRangedWeapon)
//          return index;
//      }
//      return -1;
//    }

//    public MissionWeapon Consume(short count)
//    {
//      this.Amount -= count;
//      return new MissionWeapon(this.Item, this.ItemModifier, this.Banner, count, (short) 0, new MissionWeapon?());
//    }

//    public void ConsumeAmmo(short count)
//    {
//      if (count > (short) 0)
//        this._ammoWeapon = new MissionWeapon.MissionSubWeapon(this._ammoWeapon.Value with
//        {
//          Amount = count
//        });
//      else
//        this._ammoWeapon = (MissionWeapon.MissionSubWeapon) null;
//    }

//    public void SetAmmo(MissionWeapon ammoWeapon)
//    {
//      this._ammoWeapon = new MissionWeapon.MissionSubWeapon(ammoWeapon);
//    }

//    public void ReloadAmmo(MissionWeapon ammoWeapon, short reloadPhase)
//    {
//      if (this._ammoWeapon != null && this._ammoWeapon.Value.Amount >= (short) 0)
//        ammoWeapon.Amount += this._ammoWeapon.Value.Amount;
//      this._ammoWeapon = new MissionWeapon.MissionSubWeapon(ammoWeapon);
//      this.ReloadPhase = reloadPhase;
//    }

//    public void AttachWeapon(MissionWeapon attachedWeapon, ref MatrixFrame attachFrame)
//    {
//      if (this._attachedWeapons == null)
//      {
//        this._attachedWeapons = new List<MissionWeapon.MissionSubWeapon>();
//        this._attachedWeaponFrames = new List<MatrixFrame>();
//      }
//      this._attachedWeapons.Add(new MissionWeapon.MissionSubWeapon(attachedWeapon));
//      this._attachedWeaponFrames.Add(attachFrame);
//    }

//    public void RemoveAttachedWeapon(int attachmentIndex)
//    {
//      this._attachedWeapons.RemoveAt(attachmentIndex);
//      this._attachedWeaponFrames.RemoveAt(attachmentIndex);
//    }

//    public bool HasEnoughSpaceForAmount(int amount)
//    {
//      return (int) this.ModifiedMaxAmount - (int) this.Amount >= amount;
//    }

//    public void SetRandomGlossMultiplier(int seed)
//    {
//      this.GlossMultiplier = (float) (1.0 + ((double) new Random(seed).NextFloat() * 2.0 - 1.0) * 0.30000001192092896);
//    }

//    public void AddExtraModifiedMaxValue(short extraValue)
//    {
//      this._modifiedMaxDataValue += extraValue;
//    }

//    [StructLayout(LayoutKind.Sequential, Size = 1)]
//    public struct ImpactSoundModifier
//    {
//      public const string ModifierName = "impactModifier";
//      public const float None = 0.0f;
//      public const float ActiveBlock = 0.1f;
//      public const float ChamberBlocked = 0.2f;
//      public const float CrushThrough = 0.3f;
//    }

//    private class MissionSubWeapon
//    {
//      public MissionWeapon Value { get; private set; }

//      public MissionSubWeapon(MissionWeapon subWeapon) => this.Value = subWeapon;
//    }

//    public delegate void OnGetWeaponDataDelegate(
//      ref WeaponData weaponData,
//      MissionWeapon weapon,
//      bool isFemale,
//      Banner banner,
//      bool needBatchedVersion);
//  }
//}
