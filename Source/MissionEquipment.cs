//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.MissionEquipment
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System.Threading;
//using TaleWorlds.Core;
//using TaleWorlds.Library;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public class MissionEquipment
//  {
//    private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
//    private readonly MissionWeapon[] _weaponSlots;
//    private MissionEquipment.MissionEquipmentCache _cache;

//    public MissionEquipment()
//    {
//      this._weaponSlots = new MissionWeapon[5];
//      this._cache = new MissionEquipment.MissionEquipmentCache();
//      this._cache.Initialize();
//    }

//    public MissionEquipment(Equipment spawnEquipment, Banner banner)
//      : this()
//    {
//      for (EquipmentIndex index1 = EquipmentIndex.WeaponItemBeginSlot; index1 < EquipmentIndex.NumAllWeaponSlots; ++index1)
//      {
//        MissionWeapon[] weaponSlots = this._weaponSlots;
//        int index2 = (int) index1;
//        EquipmentElement equipmentElement = spawnEquipment[index1];
//        ItemObject itemObject = equipmentElement.Item;
//        equipmentElement = spawnEquipment[index1];
//        ItemModifier itemModifier = equipmentElement.ItemModifier;
//        Banner banner1 = banner;
//        MissionWeapon missionWeapon = new MissionWeapon(itemObject, itemModifier, banner1);
//        weaponSlots[index2] = missionWeapon;
//      }
//    }

//    public MissionWeapon this[int index]
//    {
//      get => this._weaponSlots[index];
//      set
//      {
//        this._weaponSlots[index] = value;
//        this._cache.InvalidateOnWeaponSlotUpdated();
//      }
//    }

//    public MissionWeapon this[EquipmentIndex index]
//    {
//      get => this._weaponSlots[(int) index];
//      set => this[(int) index] = value;
//    }

//    public void FillFrom(MissionEquipment sourceEquipment)
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//        this[index] = new MissionWeapon(sourceEquipment[index].Item, sourceEquipment[index].ItemModifier, (Banner) null);
//    }

//    public void FillFrom(Equipment sourceEquipment, Banner banner)
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//        this[index] = new MissionWeapon(sourceEquipment[index].Item, sourceEquipment[index].ItemModifier, banner);
//    }

//    private float CalculateGetTotalWeightOfWeapons()
//    {
//      float totalWeightOfWeapons = 0.0f;
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//      {
//        MissionWeapon missionWeapon = this[index];
//        if (!missionWeapon.IsEmpty)
//        {
//          if (missionWeapon.CurrentUsageItem.IsShield)
//          {
//            if (missionWeapon.HitPoints > (short) 0)
//              totalWeightOfWeapons += missionWeapon.GetWeight();
//          }
//          else
//            totalWeightOfWeapons += missionWeapon.GetWeight();
//        }
//      }
//      return totalWeightOfWeapons;
//    }

//    public float GetTotalWeightOfWeapons()
//    {
//      this._cacheLock.EnterReadLock();
//      try
//      {
//        if (this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons))
//          return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons);
//      }
//      finally
//      {
//        this._cacheLock.ExitReadLock();
//      }
//      this._cacheLock.EnterWriteLock();
//      try
//      {
//        if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons))
//          this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons, this.CalculateGetTotalWeightOfWeapons());
//        return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedFloat.TotalWeightOfWeapons);
//      }
//      finally
//      {
//        this._cacheLock.ExitWriteLock();
//      }
//    }

//    public static EquipmentIndex SelectWeaponPickUpSlot(
//      Agent agentPickingUp,
//      MissionWeapon weaponBeingPickedUp,
//      bool isStuckMissile)
//    {
//      EquipmentIndex equipmentIndex1 = EquipmentIndex.None;
//      if (weaponBeingPickedUp.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DropOnWeaponChange | ItemFlags.DropOnAnyAction))
//      {
//        equipmentIndex1 = EquipmentIndex.ExtraWeaponSlot;
//      }
//      else
//      {
//        bool flag1 = weaponBeingPickedUp.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.HeldInOffHand);
//        EquipmentIndex index1 = flag1 ? agentPickingUp.GetOffhandWieldedItemIndex() : agentPickingUp.GetPrimaryWieldedItemIndex();
//        MissionWeapon missionWeapon1 = index1 != EquipmentIndex.None ? agentPickingUp.Equipment[index1] : MissionWeapon.Invalid;
//        if (isStuckMissile)
//        {
//          bool flag2 = false;
//          bool flag3 = false;
//          bool isConsumable = weaponBeingPickedUp.Item.PrimaryWeapon.IsConsumable;
//          if (isConsumable)
//          {
//            flag2 = !missionWeapon1.IsEmpty && missionWeapon1.IsEqualTo(weaponBeingPickedUp) && missionWeapon1.HasEnoughSpaceForAmount((int) weaponBeingPickedUp.Amount);
//            flag3 = !missionWeapon1.IsEmpty && missionWeapon1.IsSameType(weaponBeingPickedUp) && missionWeapon1.HasEnoughSpaceForAmount((int) weaponBeingPickedUp.Amount);
//          }
//          EquipmentIndex equipmentIndex2 = EquipmentIndex.None;
//          EquipmentIndex equipmentIndex3 = EquipmentIndex.None;
//          EquipmentIndex equipmentIndex4 = EquipmentIndex.None;
//          for (EquipmentIndex index2 = EquipmentIndex.WeaponItemBeginSlot; index2 < EquipmentIndex.ExtraWeaponSlot; ++index2)
//          {
//            if (isConsumable)
//            {
//              if (equipmentIndex3 != EquipmentIndex.None && !agentPickingUp.Equipment[index2].IsEmpty && agentPickingUp.Equipment[index2].IsEqualTo(weaponBeingPickedUp) && agentPickingUp.Equipment[index2].HasEnoughSpaceForAmount((int) weaponBeingPickedUp.Amount))
//              {
//                equipmentIndex3 = index2;
//                continue;
//              }
//              if (equipmentIndex4 == EquipmentIndex.None && !agentPickingUp.Equipment[index2].IsEmpty && agentPickingUp.Equipment[index2].IsSameType(weaponBeingPickedUp) && agentPickingUp.Equipment[index2].HasEnoughSpaceForAmount((int) weaponBeingPickedUp.Amount))
//              {
//                equipmentIndex4 = index2;
//                continue;
//              }
//            }
//            if (equipmentIndex2 == EquipmentIndex.None && agentPickingUp.Equipment[index2].IsEmpty)
//              equipmentIndex2 = index2;
//          }
//          if (flag2)
//            equipmentIndex1 = index1;
//          else if (equipmentIndex3 != EquipmentIndex.None)
//            equipmentIndex1 = equipmentIndex4;
//          else if (flag3)
//            equipmentIndex1 = index1;
//          else if (equipmentIndex4 != EquipmentIndex.None)
//            equipmentIndex1 = equipmentIndex4;
//          else if (equipmentIndex2 != EquipmentIndex.None)
//            equipmentIndex1 = equipmentIndex2;
//        }
//        else
//        {
//          bool isConsumable = weaponBeingPickedUp.Item.PrimaryWeapon.IsConsumable;
//          if (isConsumable && weaponBeingPickedUp.Amount == (short) 0)
//          {
//            equipmentIndex1 = EquipmentIndex.None;
//          }
//          else
//          {
//            if (flag1 && index1 != EquipmentIndex.None)
//            {
//              for (int index3 = 0; index3 < 4; ++index3)
//              {
//                if ((EquipmentIndex) index3 != index1 && !agentPickingUp.Equipment[index3].IsEmpty && agentPickingUp.Equipment[index3].Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.HeldInOffHand))
//                {
//                  equipmentIndex1 = index1;
//                  break;
//                }
//              }
//            }
//            MissionWeapon missionWeapon2;
//            if (equipmentIndex1 == EquipmentIndex.None && isConsumable)
//            {
//              for (EquipmentIndex index4 = EquipmentIndex.WeaponItemBeginSlot; index4 < EquipmentIndex.ExtraWeaponSlot; ++index4)
//              {
//                missionWeapon2 = agentPickingUp.Equipment[index4];
//                if (!missionWeapon2.IsEmpty)
//                {
//                  missionWeapon2 = agentPickingUp.Equipment[index4];
//                  if (missionWeapon2.IsSameType(weaponBeingPickedUp))
//                  {
//                    missionWeapon2 = agentPickingUp.Equipment[index4];
//                    int amount = (int) missionWeapon2.Amount;
//                    missionWeapon2 = agentPickingUp.Equipment[index4];
//                    int modifiedMaxAmount = (int) missionWeapon2.ModifiedMaxAmount;
//                    if (amount < modifiedMaxAmount)
//                    {
//                      equipmentIndex1 = index4;
//                      break;
//                    }
//                  }
//                }
//              }
//            }
//            if (equipmentIndex1 == EquipmentIndex.None)
//            {
//              for (EquipmentIndex index5 = EquipmentIndex.WeaponItemBeginSlot; index5 < EquipmentIndex.ExtraWeaponSlot; ++index5)
//              {
//                missionWeapon2 = agentPickingUp.Equipment[index5];
//                if (missionWeapon2.IsEmpty)
//                {
//                  equipmentIndex1 = index5;
//                  break;
//                }
//              }
//            }
//            if (equipmentIndex1 == EquipmentIndex.None)
//            {
//              for (EquipmentIndex index6 = EquipmentIndex.WeaponItemBeginSlot; index6 < EquipmentIndex.ExtraWeaponSlot; ++index6)
//              {
//                missionWeapon2 = agentPickingUp.Equipment[index6];
//                if (!missionWeapon2.IsEmpty)
//                {
//                  missionWeapon2 = agentPickingUp.Equipment[index6];
//                  if (missionWeapon2.IsAnyConsumable())
//                  {
//                    missionWeapon2 = agentPickingUp.Equipment[index6];
//                    if (missionWeapon2.Amount == (short) 0)
//                    {
//                      equipmentIndex1 = index6;
//                      break;
//                    }
//                  }
//                }
//              }
//            }
//            if (equipmentIndex1 == EquipmentIndex.None && !missionWeapon1.IsEmpty)
//              equipmentIndex1 = index1;
//            if (equipmentIndex1 == EquipmentIndex.None)
//              equipmentIndex1 = EquipmentIndex.WeaponItemBeginSlot;
//          }
//        }
//      }
//      return equipmentIndex1;
//    }

//    public bool HasAmmo(
//      EquipmentIndex equipmentIndex,
//      out int rangedUsageIndex,
//      out bool hasLoadedAmmo,
//      out bool noAmmoInThisSlot)
//    {
//      hasLoadedAmmo = false;
//      noAmmoInThisSlot = false;
//      MissionWeapon weaponSlot = this._weaponSlots[(int) equipmentIndex];
//      rangedUsageIndex = weaponSlot.GetRangedUsageIndex();
//      if (rangedUsageIndex >= 0)
//      {
//        if (weaponSlot.Ammo > (short) 0)
//        {
//          hasLoadedAmmo = true;
//          return true;
//        }
//        noAmmoInThisSlot = weaponSlot.IsAnyConsumable() && weaponSlot.Amount == (short) 0;
//        for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//        {
//          MissionWeapon missionWeapon = this[(int) index];
//          if (!missionWeapon.IsEmpty && missionWeapon.HasAnyUsageWithWeaponClass(weaponSlot.GetWeaponComponentDataForUsage(rangedUsageIndex).AmmoClass) && this[(int) index].ModifiedMaxAmount > (short) 1 && missionWeapon.Amount > (short) 0)
//            return true;
//        }
//      }
//      return false;
//    }

//    public int GetAmmoAmount(EquipmentIndex weaponIndex)
//    {
//      if (this[weaponIndex].IsAnyConsumable() && this[weaponIndex].ModifiedMaxAmount <= (short) 1)
//        return (int) this[weaponIndex].ModifiedMaxAmount;
//      int ammoAmount = 0;
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//      {
//        if (!this[(int) index].IsEmpty && this[(int) index].CurrentUsageItem.WeaponClass == this[weaponIndex].CurrentUsageItem.AmmoClass && this[(int) index].ModifiedMaxAmount > (short) 1)
//          ammoAmount += (int) this[(int) index].Amount;
//      }
//      return ammoAmount;
//    }

//    public int GetMaxAmmo(EquipmentIndex weaponIndex)
//    {
//      if (this[weaponIndex].IsAnyConsumable() && this[weaponIndex].ModifiedMaxAmount <= (short) 1)
//        return (int) this[weaponIndex].ModifiedMaxAmount;
//      int maxAmmo = 0;
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//      {
//        if (!this[(int) index].IsEmpty && this[(int) index].CurrentUsageItem.WeaponClass == this[weaponIndex].CurrentUsageItem.AmmoClass && this[(int) index].ModifiedMaxAmount > (short) 1)
//          maxAmmo += (int) this[(int) index].ModifiedMaxAmount;
//      }
//      return maxAmmo;
//    }

//    public void GetAmmoCountAndIndexOfType(
//      ItemObject.ItemTypeEnum itemType,
//      out int ammoCount,
//      out EquipmentIndex eIndex,
//      EquipmentIndex equippedIndex = EquipmentIndex.None)
//    {
//      ItemObject.ItemTypeEnum ammoTypeForItemType = ItemObject.GetAmmoTypeForItemType(itemType);
//      ItemObject itemObject;
//      if (equippedIndex != EquipmentIndex.None)
//      {
//        itemObject = this[equippedIndex].Item;
//        ammoCount = 0;
//      }
//      else
//      {
//        itemObject = (ItemObject) null;
//        ammoCount = -1;
//      }
//      eIndex = equippedIndex;
//      if (ammoTypeForItemType == ItemObject.ItemTypeEnum.Invalid)
//        return;
//      for (EquipmentIndex index = EquipmentIndex.Weapon3; index >= EquipmentIndex.WeaponItemBeginSlot; --index)
//      {
//        if (!this[index].IsEmpty && this[index].Item.Type == ammoTypeForItemType)
//        {
//          int amount = (int) this[index].Amount;
//          if (amount > 0)
//          {
//            if (itemObject == null)
//            {
//              eIndex = index;
//              itemObject = this[index].Item;
//              ammoCount = amount;
//            }
//            else if (itemObject.Id == this[index].Item.Id)
//              ammoCount += amount;
//          }
//        }
//      }
//    }

//    public static bool DoesWeaponFitToSlot(EquipmentIndex slotIndex, MissionWeapon weapon)
//    {
//      return weapon.IsEmpty || (!weapon.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DropOnWeaponChange | ItemFlags.DropOnAnyAction) ? slotIndex >= EquipmentIndex.WeaponItemBeginSlot && slotIndex < EquipmentIndex.ExtraWeaponSlot : slotIndex == EquipmentIndex.ExtraWeaponSlot);
//    }

//    public void CheckLoadedAmmos()
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//      {
//        if (!this[index].IsEmpty && this[index].Item.PrimaryWeapon.WeaponClass == WeaponClass.Crossbow)
//        {
//          EquipmentIndex eIndex;
//          this.GetAmmoCountAndIndexOfType(this[index].Item.Type, out int _, out eIndex);
//          if (eIndex != EquipmentIndex.None)
//          {
//            MissionWeapon ammoWeapon = this._weaponSlots[(int) eIndex].Consume(MathF.Min(this[index].MaxAmmo, this._weaponSlots[(int) eIndex].Amount));
//            this._weaponSlots[(int) index].ReloadAmmo(ammoWeapon, this._weaponSlots[(int) index].ReloadPhaseCount);
//          }
//        }
//      }
//      this._cache.InvalidateOnWeaponAmmoUpdated();
//    }

//    public void SetUsageIndexOfSlot(EquipmentIndex slotIndex, int usageIndex)
//    {
//      this._weaponSlots[(int) slotIndex].CurrentUsageIndex = usageIndex;
//      this._cache.InvalidateOnWeaponUsageIndexUpdated();
//    }

//    public void SetReloadPhaseOfSlot(EquipmentIndex slotIndex, short reloadPhase)
//    {
//      this._weaponSlots[(int) slotIndex].ReloadPhase = reloadPhase;
//    }

//    public void SetAmountOfSlot(
//      EquipmentIndex slotIndex,
//      short dataValue,
//      bool addOverflowToMaxAmount = false)
//    {
//      if (addOverflowToMaxAmount)
//      {
//        short extraValue = (short) ((int) dataValue - (int) this._weaponSlots[(int) slotIndex].Amount);
//        if (extraValue > (short) 0)
//          this._weaponSlots[(int) slotIndex].AddExtraModifiedMaxValue(extraValue);
//      }
//      short amount = this._weaponSlots[(int) slotIndex].Amount;
//      this._weaponSlots[(int) slotIndex].Amount = dataValue;
//      this._cache.InvalidateOnWeaponAmmoUpdated();
//      if ((amount == (short) 0 || dataValue != (short) 0) && (amount != (short) 0 || dataValue == (short) 0))
//        return;
//      this._cache.InvalidateOnWeaponAmmoAvailabilityChanged();
//    }

//    public void SetHitPointsOfSlot(
//      EquipmentIndex slotIndex,
//      short dataValue,
//      bool addOverflowToMaxHitPoints = false)
//    {
//      if (addOverflowToMaxHitPoints)
//      {
//        short extraValue = (short) ((int) dataValue - (int) this._weaponSlots[(int) slotIndex].HitPoints);
//        if (extraValue > (short) 0)
//          this._weaponSlots[(int) slotIndex].AddExtraModifiedMaxValue(extraValue);
//      }
//      this._weaponSlots[(int) slotIndex].HitPoints = dataValue;
//      this._cache.InvalidateOnWeaponHitPointsUpdated();
//      if (dataValue != (short) 0)
//        return;
//      this._cache.InvalidateOnWeaponDestroyed();
//    }

//    public void SetReloadedAmmoOfSlot(
//      EquipmentIndex slotIndex,
//      EquipmentIndex ammoSlotIndex,
//      short totalAmmo)
//    {
//      if (ammoSlotIndex == EquipmentIndex.None)
//      {
//        this._weaponSlots[(int) slotIndex].SetAmmo(MissionWeapon.Invalid);
//      }
//      else
//      {
//        MissionWeapon weaponSlot = this._weaponSlots[(int) ammoSlotIndex] with
//        {
//          Amount = totalAmmo
//        };
//        this._weaponSlots[(int) slotIndex].SetAmmo(weaponSlot);
//      }
//      this._cache.InvalidateOnWeaponAmmoUpdated();
//    }

//    public void SetConsumedAmmoOfSlot(EquipmentIndex slotIndex, short count)
//    {
//      this._weaponSlots[(int) slotIndex].ConsumeAmmo(count);
//      this._cache.InvalidateOnWeaponAmmoUpdated();
//    }

//    public void AttachWeaponToWeaponInSlot(
//      EquipmentIndex slotIndex,
//      ref MissionWeapon weapon,
//      ref MatrixFrame attachLocalFrame)
//    {
//      this._weaponSlots[(int) slotIndex].AttachWeapon(weapon, ref attachLocalFrame);
//    }

//    public bool HasShield()
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//      {
//        WeaponComponentData currentUsageItem = this._weaponSlots[(int) index].CurrentUsageItem;
//        if (currentUsageItem != null && currentUsageItem.IsShield)
//          return true;
//      }
//      return false;
//    }

//    public bool HasAnyWeapon()
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//      {
//        if (this._weaponSlots[(int) index].CurrentUsageItem != null)
//          return true;
//      }
//      return false;
//    }

//    public bool HasAnyWeaponWithFlags(WeaponFlags flags)
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//      {
//        WeaponComponentData currentUsageItem = this._weaponSlots[(int) index].CurrentUsageItem;
//        if (currentUsageItem != null && currentUsageItem.WeaponFlags.HasAllFlags<WeaponFlags>(flags))
//          return true;
//      }
//      return false;
//    }

//    public ItemObject GetBanner()
//    {
//      ItemObject banner = (ItemObject) null;
//      ItemObject itemObject = this._weaponSlots[4].Item;
//      if (itemObject != null && itemObject.IsBannerItem && itemObject.BannerComponent != null)
//        banner = itemObject;
//      return banner;
//    }

//    public bool HasRangedWeapon(WeaponClass requiredAmmoClass = WeaponClass.Undefined)
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//      {
//        WeaponComponentData currentUsageItem = this._weaponSlots[(int) index].CurrentUsageItem;
//        if (currentUsageItem != null && currentUsageItem.IsRangedWeapon && (requiredAmmoClass == WeaponClass.Undefined || currentUsageItem.AmmoClass == requiredAmmoClass))
//          return true;
//      }
//      return false;
//    }

//    public bool ContainsNonConsumableRangedWeaponWithAmmo()
//    {
//      this._cacheLock.EnterReadLock();
//      try
//      {
//        if (this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsNonConsumableRangedWeaponWithAmmo))
//          return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsNonConsumableRangedWeaponWithAmmo);
//      }
//      finally
//      {
//        this._cacheLock.ExitReadLock();
//      }
//      this._cacheLock.EnterWriteLock();
//      try
//      {
//        if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsNonConsumableRangedWeaponWithAmmo))
//          this.GatherInformationAndUpdateCache();
//        return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsNonConsumableRangedWeaponWithAmmo);
//      }
//      finally
//      {
//        this._cacheLock.ExitWriteLock();
//      }
//    }

//    public bool ContainsMeleeWeapon()
//    {
//      this._cacheLock.EnterReadLock();
//      try
//      {
//        if (this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsMeleeWeapon))
//          return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsMeleeWeapon);
//      }
//      finally
//      {
//        this._cacheLock.ExitReadLock();
//      }
//      this._cacheLock.EnterWriteLock();
//      try
//      {
//        if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsMeleeWeapon))
//          this.GatherInformationAndUpdateCache();
//        return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsMeleeWeapon);
//      }
//      finally
//      {
//        this._cacheLock.ExitWriteLock();
//      }
//    }

//    public bool ContainsShield()
//    {
//      this._cacheLock.EnterReadLock();
//      try
//      {
//        if (this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsShield))
//          return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsShield);
//      }
//      finally
//      {
//        this._cacheLock.ExitReadLock();
//      }
//      this._cacheLock.EnterWriteLock();
//      try
//      {
//        if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsShield))
//          this.GatherInformationAndUpdateCache();
//        return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsShield);
//      }
//      finally
//      {
//        this._cacheLock.ExitWriteLock();
//      }
//    }

//    public bool ContainsSpear()
//    {
//      this._cacheLock.EnterReadLock();
//      try
//      {
//        if (this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsSpear))
//          return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsSpear);
//      }
//      finally
//      {
//        this._cacheLock.ExitReadLock();
//      }
//      this._cacheLock.EnterWriteLock();
//      try
//      {
//        if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsSpear))
//          this.GatherInformationAndUpdateCache();
//        return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsSpear);
//      }
//      finally
//      {
//        this._cacheLock.ExitWriteLock();
//      }
//    }

//    public bool ContainsThrownWeapon()
//    {
//      this._cacheLock.EnterReadLock();
//      try
//      {
//        if (this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsThrownWeapon))
//          return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsThrownWeapon);
//      }
//      finally
//      {
//        this._cacheLock.ExitReadLock();
//      }
//      this._cacheLock.EnterWriteLock();
//      try
//      {
//        if (!this._cache.IsValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsThrownWeapon))
//          this.GatherInformationAndUpdateCache();
//        return this._cache.GetValue(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsThrownWeapon);
//      }
//      finally
//      {
//        this._cacheLock.ExitWriteLock();
//      }
//    }

//    private void GatherInformationAndUpdateCache()
//    {
//      bool containsMeleeWeapon;
//      bool containsShield;
//      bool containsSpear;
//      bool containsNonConsumableRangedWeaponWithAmmo;
//      bool containsThrownWeapon;
//      this.GatherInformation(out containsMeleeWeapon, out containsShield, out containsSpear, out containsNonConsumableRangedWeaponWithAmmo, out containsThrownWeapon);
//      this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsMeleeWeapon, containsMeleeWeapon);
//      this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsShield, containsShield);
//      this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsSpear, containsSpear);
//      this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsNonConsumableRangedWeaponWithAmmo, containsNonConsumableRangedWeaponWithAmmo);
//      this._cache.UpdateAndMarkValid(MissionEquipment.MissionEquipmentCache.CachedBool.ContainsThrownWeapon, containsThrownWeapon);
//    }

//    private void GatherInformation(
//      out bool containsMeleeWeapon,
//      out bool containsShield,
//      out bool containsSpear,
//      out bool containsNonConsumableRangedWeaponWithAmmo,
//      out bool containsThrownWeapon)
//    {
//      containsMeleeWeapon = false;
//      containsShield = false;
//      containsSpear = false;
//      containsNonConsumableRangedWeaponWithAmmo = false;
//      containsThrownWeapon = false;
//      for (EquipmentIndex weaponIndex = EquipmentIndex.WeaponItemBeginSlot; weaponIndex < EquipmentIndex.NumAllWeaponSlots; ++weaponIndex)
//      {
//        bool weaponHasMelee;
//        bool weaponHasShield;
//        bool weaponHasPolearm;
//        bool weaponHasNonConsumableRanged;
//        bool weaponHasThrown;
//        this._weaponSlots[(int) weaponIndex].GatherInformationFromWeapon(out weaponHasMelee, out weaponHasShield, out weaponHasPolearm, out weaponHasNonConsumableRanged, out weaponHasThrown, out WeaponClass _);
//        containsMeleeWeapon |= weaponHasMelee;
//        containsShield |= weaponHasShield;
//        containsSpear |= weaponHasPolearm;
//        containsThrownWeapon |= weaponHasThrown;
//        if (weaponHasNonConsumableRanged)
//          containsNonConsumableRangedWeaponWithAmmo = containsNonConsumableRangedWeaponWithAmmo || this.GetAmmoAmount(weaponIndex) > 0;
//      }
//    }

//    public void SetGlossMultipliersOfWeaponsRandomly(int seed)
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
//        this._weaponSlots[(int) index].SetRandomGlossMultiplier(seed);
//    }

//    private struct MissionEquipmentCache
//    {
//      private const int CachedBoolCount = 5;
//      private const int CachedFloatCount = 1;
//      private float _cachedFloat;
//      private StackArray.StackArray5Bool _cachedBool;
//      private StackArray.StackArray6Bool _validity;

//      public void Initialize()
//      {
//        this._cachedBool = new StackArray.StackArray5Bool();
//        this._validity = new StackArray.StackArray6Bool();
//      }

//      public bool IsValid(
//        MissionEquipment.MissionEquipmentCache.CachedBool queriedData)
//      {
//        return this._validity[(int) queriedData];
//      }

//      public void UpdateAndMarkValid(
//        MissionEquipment.MissionEquipmentCache.CachedBool data,
//        bool value)
//      {
//        int index = (int) data;
//        this._cachedBool[index] = value;
//        this._validity[index] = true;
//      }

//      public bool GetValue(
//        MissionEquipment.MissionEquipmentCache.CachedBool data)
//      {
//        return this._cachedBool[(int) data];
//      }

//      public bool IsValid(
//        MissionEquipment.MissionEquipmentCache.CachedFloat queriedData)
//      {
//        return this._validity[(int) (5 + queriedData)];
//      }

//      public void UpdateAndMarkValid(
//        MissionEquipment.MissionEquipmentCache.CachedFloat data,
//        float value)
//      {
//        int num = (int) data;
//        this._cachedFloat = value;
//        this._validity[5 + num] = true;
//      }

//      public float GetValue(
//        MissionEquipment.MissionEquipmentCache.CachedFloat data)
//      {
//        return this._cachedFloat;
//      }

//      public void InvalidateOnWeaponSlotUpdated()
//      {
//        this._validity[0] = false;
//        this._validity[1] = false;
//        this._validity[2] = false;
//        this._validity[3] = false;
//        this._validity[4] = false;
//        this._validity[5] = false;
//      }

//      public void InvalidateOnWeaponUsageIndexUpdated()
//      {
//      }

//      public void InvalidateOnWeaponAmmoUpdated() => this._validity[5] = false;

//      public void InvalidateOnWeaponAmmoAvailabilityChanged() => this._validity[3] = false;

//      public void InvalidateOnWeaponHitPointsUpdated() => this._validity[5] = false;

//      public void InvalidateOnWeaponDestroyed() => this._validity[1] = false;

//      public enum CachedBool
//      {
//        ContainsMeleeWeapon,
//        ContainsShield,
//        ContainsSpear,
//        ContainsNonConsumableRangedWeaponWithAmmo,
//        ContainsThrownWeapon,
//        Count,
//      }

//      public enum CachedFloat
//      {
//        TotalWeightOfWeapons,
//        Count,
//      }
//    }
//  }
//}
