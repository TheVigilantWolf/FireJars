//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.MissionCombatMechanicsHelper
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System;
//using TaleWorlds.Core;
//using TaleWorlds.Engine;
//using TaleWorlds.Library;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public static class MissionCombatMechanicsHelper
//  {
//    private const float SpeedBonusFactorForSwing = 0.7f;
//    private const float SpeedBonusFactorForThrust = 0.5f;

//    public static bool DecideAgentShrugOffBlow(
//      Agent victimAgent,
//      in AttackCollisionData collisionData,
//      in Blow blow)
//    {
//      bool flag = false;
//      if ((double) victimAgent.Health - (double) collisionData.InflictedDamage >= 1.0)
//      {
//        float staggerThresholdDamage = MissionGameModels.Current.AgentApplyDamageModel.CalculateStaggerThresholdDamage(victimAgent, in blow);
//        flag = (double) collisionData.InflictedDamage <= (double) staggerThresholdDamage;
//      }
//      return flag;
//    }

//    public static bool DecideAgentDismountedByBlow(
//      Agent attackerAgent,
//      Agent victimAgent,
//      in AttackCollisionData collisionData,
//      WeaponComponentData attackerWeapon,
//      in Blow blow)
//    {
//      bool flag1 = false;
//      int inflictedDamage = collisionData.InflictedDamage;
//      bool flag2 = (double) victimAgent.Health - (double) inflictedDamage >= 1.0;
//      bool flag3 = (blow.BlowFlag & BlowFlags.ShrugOff) != 0;
//      if (attackerWeapon != null & flag2 && !flag3)
//      {
//        int healthLimit = (int) victimAgent.HealthLimit;
//        if (MissionGameModels.Current.AgentApplyDamageModel.CanWeaponDismount(attackerAgent, attackerWeapon, in blow, in collisionData))
//        {
//          float dismountPenetration = MissionGameModels.Current.AgentApplyDamageModel.GetDismountPenetration(attackerAgent, attackerWeapon, in blow, in collisionData);
//          float dismountResistance = MissionGameModels.Current.AgentStatCalculateModel.GetDismountResistance(victimAgent);
//          flag1 = MissionCombatMechanicsHelper.DecideCombatEffect((float) inflictedDamage, (float) healthLimit, dismountResistance, dismountPenetration);
//        }
//        if (!flag1)
//          flag1 = MissionCombatMechanicsHelper.DecideWeaponKnockDown(attackerAgent, victimAgent, attackerWeapon, in collisionData, in blow);
//      }
//      return flag1;
//    }

//    public static bool DecideAgentKnockedBackByBlow(
//      Agent attackerAgent,
//      Agent victimAgent,
//      in AttackCollisionData collisionData,
//      WeaponComponentData attackerWeapon,
//      in Blow blow)
//    {
//      bool flag1 = false;
//      int healthLimit = (int) victimAgent.HealthLimit;
//      int inflictedDamage = collisionData.InflictedDamage;
//      bool flag2 = (blow.BlowFlag & BlowFlags.ShrugOff) != 0;
//      if (collisionData.IsHorseCharge)
//      {
//        if ((double) MissionCombatMechanicsHelper.ChargeDamageDotProduct(victimAgent.Position, attackerAgent.GetMovementDirection(), collisionData.CollisionGlobalPosition) >= 0.699999988079071)
//          flag1 = true;
//      }
//      else if (collisionData.IsAlternativeAttack)
//        flag1 = true;
//      else if (attackerWeapon != null && !flag2 && MissionGameModels.Current.AgentApplyDamageModel.CanWeaponKnockback(attackerAgent, attackerWeapon, in blow, in collisionData))
//      {
//        float knockBackPenetration = MissionGameModels.Current.AgentApplyDamageModel.GetKnockBackPenetration(attackerAgent, attackerWeapon, in blow, in collisionData);
//        float knockBackResistance = MissionGameModels.Current.AgentStatCalculateModel.GetKnockBackResistance(victimAgent);
//        flag1 = MissionCombatMechanicsHelper.DecideCombatEffect((float) inflictedDamage, (float) healthLimit, knockBackResistance, knockBackPenetration);
//      }
//      return flag1;
//    }

//    public static bool DecideAgentKnockedDownByBlow(
//      Agent attackerAgent,
//      Agent victimAgent,
//      in AttackCollisionData collisionData,
//      WeaponComponentData attackerWeapon,
//      in Blow blow)
//    {
//      bool flag1 = false;
//      if ((blow.BlowFlag & BlowFlags.ShrugOff) == 0)
//      {
//        int healthLimit = (int) victimAgent.HealthLimit;
//        float inflictedDamage = (float) collisionData.InflictedDamage;
//        bool flag2 = (blow.BlowFlag & BlowFlags.KnockBack) != 0;
//        if (collisionData.IsHorseCharge & flag2)
//        {
//          float chargePenetration = MissionGameModels.Current.AgentApplyDamageModel.GetHorseChargePenetration();
//          float knockDownResistance = MissionGameModels.Current.AgentStatCalculateModel.GetKnockDownResistance(victimAgent);
//          flag1 = MissionCombatMechanicsHelper.DecideCombatEffect(inflictedDamage, (float) healthLimit, knockDownResistance, chargePenetration);
//        }
//        else if (attackerWeapon != null)
//          flag1 = MissionCombatMechanicsHelper.DecideWeaponKnockDown(attackerAgent, victimAgent, attackerWeapon, in collisionData, in blow);
//      }
//      return flag1;
//    }

//    public static bool DecideMountRearedByBlow(
//      Agent attackerAgent,
//      Agent victimAgent,
//      in AttackCollisionData collisionData,
//      WeaponComponentData attackerWeapon,
//      in Blow blow)
//    {
//      float combatDifficulty = Mission.Current.GetDamageMultiplierOfCombatDifficulty(victimAgent, attackerAgent);
//      return attackerWeapon != null && attackerWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.WideGrip) && attackerWeapon.WeaponLength > 120 && blow.StrikeType == StrikeType.Thrust && collisionData.ThrustTipHit && attackerAgent != null && !attackerAgent.HasMount && victimAgent.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.CanRear) && (double) victimAgent.MovementVelocity.y > 5.0 && (double) Vec3.DotProduct(blow.Direction, victimAgent.Frame.rotation.f) < -0.34999999403953552 && (double) Vec2.DotProduct(blow.GlobalPosition.AsVec2 - victimAgent.Position.AsVec2, victimAgent.GetMovementDirection()) > 0.0 && (double) collisionData.InflictedDamage >= (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.MakesRearAttackDamageThreshold) * (double) combatDifficulty;
//    }

//    public static void DecideWeaponCollisionReaction(
//      in Blow registeredBlow,
//      in AttackCollisionData collisionData,
//      Agent attacker,
//      Agent defender,
//      in MissionWeapon attackerWeapon,
//      bool isFatalHit,
//      bool isShruggedOff,
//      float momentumRemaining,
//      out MeleeCollisionReaction colReaction)
//    {
//      if (collisionData.IsColliderAgent && collisionData.StrikeType == 1 && collisionData.CollisionHitResultFlags.HasAnyFlag<CombatHitResultFlags>(CombatHitResultFlags.HitWithStartOfTheAnimation))
//        colReaction = MeleeCollisionReaction.Staggered;
//      else if (!collisionData.IsColliderAgent && collisionData.PhysicsMaterialIndex != -1 && PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex).GetFlags().HasAnyFlag<PhysicsMaterialFlags>(PhysicsMaterialFlags.AttacksCanPassThrough))
//        colReaction = MeleeCollisionReaction.SlicedThrough;
//      else if (!collisionData.IsColliderAgent || registeredBlow.InflictedDamage <= 0)
//        colReaction = MeleeCollisionReaction.Bounced;
//      else if (collisionData.StrikeType == 1 && attacker.IsDoingPassiveAttack)
//        colReaction = MissionGameModels.Current.AgentApplyDamageModel.DecidePassiveAttackCollisionReaction(attacker, defender, isFatalHit);
//      else if (collisionData.IsAlternativeAttack && (double) momentumRemaining > 0.0)
//        colReaction = MeleeCollisionReaction.ContinueChecking;
//      else if (MissionCombatMechanicsHelper.HitWithAnotherBone(in collisionData, attacker, in attackerWeapon))
//      {
//        colReaction = MeleeCollisionReaction.Bounced;
//      }
//      else
//      {
//        WeaponClass weaponClass = !attackerWeapon.IsEmpty ? attackerWeapon.CurrentUsageItem.WeaponClass : WeaponClass.Undefined;
//        int num1 = attackerWeapon.IsEmpty ? 0 : (!isFatalHit ? 1 : 0);
//        int num2 = isShruggedOff ? 1 : 0;
//        colReaction = (num1 & num2) != 0 || attackerWeapon.IsEmpty && defender != null && defender.IsHuman && !collisionData.IsAlternativeAttack && (collisionData.VictimHitBodyPart == BoneBodyPartType.Chest || collisionData.VictimHitBodyPart == BoneBodyPartType.ShoulderLeft || collisionData.VictimHitBodyPart == BoneBodyPartType.ShoulderRight || collisionData.VictimHitBodyPart == BoneBodyPartType.Abdomen || collisionData.VictimHitBodyPart == BoneBodyPartType.Legs) ? MeleeCollisionReaction.Bounced : ((weaponClass == WeaponClass.OneHandedAxe || weaponClass == WeaponClass.TwoHandedAxe) && !isFatalHit && (double) collisionData.InflictedDamage < (double) defender.HealthLimit * 0.5 || attackerWeapon.IsEmpty && !collisionData.IsAlternativeAttack && collisionData.AttackDirection == Agent.UsageDirection.AttackUp || collisionData.ThrustTipHit && collisionData.DamageType == 1 && !attackerWeapon.IsEmpty && defender.CanThrustAttackStickToBone(collisionData.VictimHitBodyPart) ? MeleeCollisionReaction.Stuck : MeleeCollisionReaction.SlicedThrough);
//        if (!collisionData.AttackBlockedWithShield && !collisionData.CollidedWithShieldOnBack || colReaction != MeleeCollisionReaction.SlicedThrough)
//          return;
//        colReaction = MeleeCollisionReaction.Bounced;
//      }
//    }

//    public static bool IsCollisionBoneDifferentThanWeaponAttachBone(
//      in AttackCollisionData collisionData,
//      int weaponAttachBoneIndex)
//    {
//      return collisionData.AttackBoneIndex != (sbyte) -1 && weaponAttachBoneIndex != -1 && weaponAttachBoneIndex != (int) collisionData.AttackBoneIndex;
//    }

//    public static bool DecideSweetSpotCollision(in AttackCollisionData collisionData)
//    {
//      return (double) collisionData.AttackProgress >= 0.2199999988079071 && (double) collisionData.AttackProgress <= 0.550000011920929;
//    }

//    public static void GetAttackCollisionResults(
//      in AttackInformation attackInformation,
//      bool crushedThrough,
//      float momentumRemaining,
//      bool cancelDamage,
//      ref AttackCollisionData attackCollisionData,
//      out CombatLogData combatLog,
//      out int speedBonus)
//    {
//      float distance = 0.0f;
//      Vec3 vec3;
//      if (attackCollisionData.IsMissile)
//      {
//        vec3 = attackCollisionData.MissileStartingPosition - attackCollisionData.CollisionGlobalPosition;
//        distance = vec3.Length;
//      }
//      combatLog = new CombatLogData(attackInformation.IsVictimAgentSameWithAttackerAgent, attackInformation.IsAttackerAgentHuman, attackInformation.IsAttackerAgentMine, attackInformation.DoesAttackerHaveRiderAgent, attackInformation.IsAttackerAgentRiderAgentMine, attackInformation.IsAttackerAgentMount, attackInformation.IsVictimAgentHuman, attackInformation.IsVictimAgentMine, false, attackInformation.DoesVictimHaveRiderAgent, attackInformation.IsVictimAgentRiderAgentMine, attackInformation.IsVictimAgentMount, (MissionObject) null, attackInformation.IsVictimRiderAgentSameAsAttackerAgent, false, false, distance);
//      bool hitWithAnotherBone = MissionCombatMechanicsHelper.IsCollisionBoneDifferentThanWeaponAttachBone(in attackCollisionData, attackInformation.WeaponAttachBoneIndex);
//      Vec2 velocityContribution1 = MissionCombatMechanicsHelper.GetAgentVelocityContribution(attackInformation.DoesAttackerHaveMountAgent, attackInformation.AttackerAgentMovementVelocity, attackInformation.AttackerAgentMountMovementDirection, attackInformation.AttackerMovementDirectionAsAngle);
//      Vec2 velocityContribution2 = MissionCombatMechanicsHelper.GetAgentVelocityContribution(attackInformation.DoesVictimHaveMountAgent, attackInformation.VictimAgentMovementVelocity, attackInformation.VictimAgentMountMovementDirection, attackInformation.VictimMovementDirectionAsAngle);
//      if (attackCollisionData.IsColliderAgent)
//      {
//        combatLog.IsRangedAttack = attackCollisionData.IsMissile;
//        ref CombatLogData local = ref combatLog;
//        double length;
//        if (!attackCollisionData.IsMissile)
//        {
//          length = (double) (velocityContribution1 - velocityContribution2).Length;
//        }
//        else
//        {
//          vec3 = velocityContribution2.ToVec3() - attackCollisionData.MissileVelocity;
//          length = (double) vec3.Length;
//        }
//        local.HitSpeed = (float) length;
//      }
//      float specialMagnitude;
//      MissionCombatMechanicsHelper.ComputeBlowMagnitude(in attackCollisionData, in attackInformation, momentumRemaining, cancelDamage, hitWithAnotherBone, velocityContribution1, velocityContribution2, out attackCollisionData.BaseMagnitude, out specialMagnitude, out attackCollisionData.MovementSpeedDamageModifier, out speedBonus);
//      MissionWeapon attackerWeapon = attackInformation.AttackerWeapon;
//      DamageTypes damageTypes = (attackerWeapon.IsEmpty | hitWithAnotherBone || attackCollisionData.IsAlternativeAttack || attackCollisionData.IsFallDamage ? 1 : (attackCollisionData.IsHorseCharge ? 1 : 0)) != 0 ? DamageTypes.Blunt : (DamageTypes) attackCollisionData.DamageType;
//      combatLog.DamageType = damageTypes;
//      if (!attackCollisionData.IsColliderAgent && attackCollisionData.EntityExists)
//      {
//        bool flag = PhysicsMaterial.GetFromIndex(attackCollisionData.PhysicsMaterialIndex).GetFlags().HasAnyFlag<PhysicsMaterialFlags>(PhysicsMaterialFlags.Flammable);
//        ref float local = ref attackCollisionData.BaseMagnitude;
//        double num1 = (double) local;
//        int num2 = attackInformation.IsAttackerAgentDoingPassiveAttack ? 1 : 0;
//        attackerWeapon = attackInformation.AttackerWeapon;
//        WeaponComponentData currentUsageItem = attackerWeapon.CurrentUsageItem;
//        int damageType = (int) damageTypes;
//        int num3 = flag ? 1 : 0;
//        double damageMultiplier = (double) MissionCombatMechanicsHelper.GetEntityDamageMultiplier(num2 != 0, currentUsageItem, (DamageTypes) damageType, num3 != 0);
//        local = (float) (num1 * damageMultiplier);
//        attackCollisionData.InflictedDamage = MBMath.ClampInt((int) attackCollisionData.BaseMagnitude, 0, 2000);
//        combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
//      }
//      if (!attackCollisionData.IsColliderAgent || attackInformation.IsVictimAgentNull)
//        return;
//      if (attackCollisionData.IsAlternativeAttack)
//        specialMagnitude = attackCollisionData.BaseMagnitude;
//      if (attackCollisionData.AttackBlockedWithShield)
//      {
//        ref readonly AttackInformation local1 = ref attackInformation;
//        ref AttackCollisionData local2 = ref attackCollisionData;
//        attackerWeapon = attackInformation.AttackerWeapon;
//        WeaponComponentData currentUsageItem = attackerWeapon.CurrentUsageItem;
//        double baseMagnitude = (double) attackCollisionData.BaseMagnitude;
//        ref int local3 = ref attackCollisionData.InflictedDamage;
//        MissionCombatMechanicsHelper.ComputeBlowDamageOnShield(in local1, in local2, currentUsageItem, (float) baseMagnitude, out local3);
//        attackCollisionData.AbsorbedByArmor = attackCollisionData.InflictedDamage;
//      }
//      else if (attackCollisionData.MissileBlockedWithWeapon)
//      {
//        attackCollisionData.InflictedDamage = 0;
//        attackCollisionData.AbsorbedByArmor = 0;
//      }
//      else
//      {
//        ref readonly AttackInformation local4 = ref attackInformation;
//        ref AttackCollisionData local5 = ref attackCollisionData;
//        attackerWeapon = attackInformation.AttackerWeapon;
//        WeaponComponentData currentUsageItem = attackerWeapon.CurrentUsageItem;
//        int damageType = (int) damageTypes;
//        double magnitude = (double) specialMagnitude;
//        int speedBonus1 = speedBonus;
//        int num = cancelDamage ? 1 : 0;
//        ref int local6 = ref attackCollisionData.InflictedDamage;
//        ref int local7 = ref attackCollisionData.AbsorbedByArmor;
//        bool flag;
//        ref bool local8 = ref flag;
//        MissionCombatMechanicsHelper.ComputeBlowDamage(in local4, in local5, currentUsageItem, (DamageTypes) damageType, (float) magnitude, speedBonus1, num != 0, out local6, out local7, out local8);
//        attackCollisionData.IsSneakAttack = flag;
//        combatLog.IsSneakAttack = flag;
//      }
//      combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
//      combatLog.AbsorbedDamage = attackCollisionData.AbsorbedByArmor;
//      combatLog.AttackProgress = attackCollisionData.AttackProgress;
//    }

//    internal static void GetDefendCollisionResults(
//      Agent attackerAgent,
//      Agent defenderAgent,
//      CombatCollisionResult collisionResult,
//      int attackerWeaponSlotIndex,
//      bool isAlternativeAttack,
//      StrikeType strikeType,
//      Agent.UsageDirection attackDirection,
//      float collisionDistanceOnWeapon,
//      float attackProgress,
//      bool attackIsParried,
//      bool isPassiveUsageHit,
//      bool isHeavyAttack,
//      ref float defenderStunPeriod,
//      ref float attackerStunPeriod,
//      ref bool crushedThrough,
//      ref bool chamber)
//    {
//      MissionWeapon missionWeapon1 = attackerWeaponSlotIndex >= 0 ? attackerAgent.Equipment[attackerWeaponSlotIndex] : MissionWeapon.Invalid;
//      WeaponComponentData currentUsageItem = missionWeapon1.IsEmpty ? (WeaponComponentData) null : missionWeapon1.CurrentUsageItem;
//      EquipmentIndex wieldedItemIndex = defenderAgent.GetOffhandWieldedItemIndex();
//      if (wieldedItemIndex == EquipmentIndex.None)
//        wieldedItemIndex = defenderAgent.GetPrimaryWieldedItemIndex();
//      MissionWeapon missionWeapon2;
//      ItemObject itemObject1;
//      if (wieldedItemIndex == EquipmentIndex.None)
//      {
//        itemObject1 = (ItemObject) null;
//      }
//      else
//      {
//        missionWeapon2 = defenderAgent.Equipment[wieldedItemIndex];
//        itemObject1 = missionWeapon2.Item;
//      }
//      ItemObject itemObject2 = itemObject1;
//      WeaponComponentData weaponComponentData1;
//      if (wieldedItemIndex == EquipmentIndex.None)
//      {
//        weaponComponentData1 = (WeaponComponentData) null;
//      }
//      else
//      {
//        missionWeapon2 = defenderAgent.Equipment[wieldedItemIndex];
//        weaponComponentData1 = missionWeapon2.CurrentUsageItem;
//      }
//      WeaponComponentData weaponComponentData2 = weaponComponentData1;
//      float totalAttackEnergy = 10f;
//      attackerStunPeriod = strikeType == StrikeType.Thrust ? ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerThrust) : ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerSwing);
//      chamber = false;
//      if (!missionWeapon1.IsEmpty)
//      {
//        float z = attackerAgent.GetCurWeaponOffset().z;
//        float realWeaponLength = currentUsageItem.GetRealWeaponLength();
//        float num1 = realWeaponLength + z;
//        float impactPoint = MBMath.ClampFloat((0.2f + collisionDistanceOnWeapon) / num1, 0.1f, 0.98f);
//        float speedDiffOfAgents = MissionCombatMechanicsHelper.ComputeRelativeSpeedDiffOfAgents(attackerAgent, defenderAgent);
//        float num2 = strikeType != StrikeType.Thrust ? CombatStatCalculator.CalculateBaseBlowMagnitudeForSwing((float) missionWeapon1.GetModifiedSwingSpeedForCurrentUsage() / 4.5454545f * MissionCombatMechanicsHelper.SpeedGraphFunction(attackProgress, strikeType, attackDirection), realWeaponLength, missionWeapon1.Item.Weight, currentUsageItem.TotalInertia, currentUsageItem.CenterOfMass, impactPoint, speedDiffOfAgents) : CombatStatCalculator.CalculateBaseBlowMagnitudeForThrust((float) missionWeapon1.GetModifiedThrustSpeedForCurrentUsage() / 11.7647057f * MissionCombatMechanicsHelper.SpeedGraphFunction(attackProgress, strikeType, attackDirection), missionWeapon1.Item.Weight, speedDiffOfAgents);
//        if (strikeType == StrikeType.Thrust)
//          num2 *= 0.8f;
//        else if (attackDirection == Agent.UsageDirection.AttackUp)
//          num2 *= 1.25f;
//        else if (isHeavyAttack)
//          num2 *= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.HeavyAttackMomentumMultiplier);
//        totalAttackEnergy += num2;
//      }
//      float num = 1f;
//      defenderStunPeriod = totalAttackEnergy * ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunMomentumTransferFactor);
//      if (weaponComponentData2 != null)
//      {
//        if (weaponComponentData2.IsShield)
//        {
//          float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightOffsetShield);
//          num += managedParameter * itemObject2.Weight;
//        }
//        else
//        {
//          num = 0.9f + ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightMultiplierWeaponWeight) * itemObject2.Weight;
//          switch (itemObject2.ItemType)
//          {
//            case ItemObject.ItemTypeEnum.TwoHandedWeapon:
//              num += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusTwoHanded);
//              break;
//            case ItemObject.ItemTypeEnum.Polearm:
//              num += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusPolearm);
//              break;
//          }
//        }
//        switch (collisionResult)
//        {
//          case CombatCollisionResult.Parried:
//            attackerStunPeriod += MathF.Min(0.15f, 0.12f * num);
//            num += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusActiveBlocked);
//            break;
//          case CombatCollisionResult.ChamberBlocked:
//            attackerStunPeriod += MathF.Min(0.25f, 0.25f * num);
//            num += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusChamberBlocked);
//            chamber = true;
//            break;
//        }
//      }
//      if (!defenderAgent.GetIsLeftStance())
//        num += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusRightStance);
//      defenderStunPeriod /= num;
//      MissionGameModels.Current.AgentApplyDamageModel.CalculateDefendedBlowStunMultipliers(attackerAgent, defenderAgent, collisionResult, currentUsageItem, weaponComponentData2, ref attackerStunPeriod, ref defenderStunPeriod);
//      float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodMax);
//      attackerStunPeriod = MathF.Min(attackerStunPeriod, managedParameter1);
//      defenderStunPeriod = MathF.Min(defenderStunPeriod, managedParameter1);
//      crushedThrough = !chamber && MissionGameModels.Current.AgentApplyDamageModel.DecideCrushedThrough(attackerAgent, defenderAgent, totalAttackEnergy, attackDirection, strikeType, weaponComponentData2, isPassiveUsageHit);
//    }

//    public static void UpdateMomentumRemaining(
//      ref float momentumRemaining,
//      in Blow b,
//      in AttackCollisionData collisionData,
//      Agent attacker,
//      Agent victim,
//      in MissionWeapon attackerWeapon,
//      bool isCrushThrough)
//    {
//      momentumRemaining = MissionGameModels.Current.AgentApplyDamageModel.CalculateRemainingMomentum(momentumRemaining, in b, in collisionData, attacker, victim, in attackerWeapon, isCrushThrough);
//    }

//    public static bool HitWithAnotherBone(
//      in AttackCollisionData collisionData,
//      Agent attacker,
//      in MissionWeapon attackerWeapon)
//    {
//      int weaponAttachBoneIndex = attackerWeapon.IsEmpty || attacker == null || !attacker.IsHuman ? -1 : (int) attacker.Monster.GetBoneToAttachForItemFlags(attackerWeapon.Item.ItemFlags);
//      return MissionCombatMechanicsHelper.IsCollisionBoneDifferentThanWeaponAttachBone(in collisionData, weaponAttachBoneIndex);
//    }

//    private static bool DecideWeaponKnockDown(
//      Agent attackerAgent,
//      Agent victimAgent,
//      WeaponComponentData attackerWeapon,
//      in AttackCollisionData collisionData,
//      in Blow blow)
//    {
//      if (!MissionGameModels.Current.AgentApplyDamageModel.CanWeaponKnockDown(attackerAgent, victimAgent, attackerWeapon, in blow, in collisionData))
//        return false;
//      float knockDownPenetration = MissionGameModels.Current.AgentApplyDamageModel.GetKnockDownPenetration(attackerAgent, attackerWeapon, in blow, in collisionData);
//      float knockDownResistance = MissionGameModels.Current.AgentStatCalculateModel.GetKnockDownResistance(victimAgent, blow.StrikeType);
//      return MissionCombatMechanicsHelper.DecideCombatEffect((float) collisionData.InflictedDamage, victimAgent.HealthLimit, knockDownResistance, knockDownPenetration);
//    }

//    private static bool DecideCombatEffect(
//      float inflictedDamage,
//      float victimMaxHealth,
//      float victimResistance,
//      float attackPenetration)
//    {
//      float num = victimMaxHealth * Math.Max(0.0f, victimResistance - attackPenetration);
//      return (double) inflictedDamage >= (double) num;
//    }

//    private static float ChargeDamageDotProduct(
//      in Vec3 victimPosition,
//      in Vec2 chargerMovementDirection,
//      in Vec3 collisionPoint)
//    {
//      Vec3 vec3 = victimPosition;
//      Vec2 asVec2_1 = vec3.AsVec2;
//      vec3 = collisionPoint;
//      Vec2 asVec2_2 = vec3.AsVec2;
//      return MathF.Max(0.0f, Vec2.DotProduct((asVec2_1 - asVec2_2).Normalized(), chargerMovementDirection));
//    }

//    private static float SpeedGraphFunction(
//      float progress,
//      StrikeType strikeType,
//      Agent.UsageDirection attackDir)
//    {
//      int num = strikeType == StrikeType.Thrust ? 1 : 0;
//      bool flag = attackDir == Agent.UsageDirection.AttackUp;
//      ManagedParametersEnum managedParameterEnum1;
//      ManagedParametersEnum managedParameterEnum2;
//      ManagedParametersEnum managedParameterEnum3;
//      ManagedParametersEnum managedParameterEnum4;
//      if (num != 0)
//      {
//        managedParameterEnum1 = ManagedParametersEnum.ThrustCombatSpeedGraphZeroProgressValue;
//        managedParameterEnum2 = ManagedParametersEnum.ThrustCombatSpeedGraphFirstMaximumPoint;
//        managedParameterEnum3 = ManagedParametersEnum.ThrustCombatSpeedGraphSecondMaximumPoint;
//        managedParameterEnum4 = ManagedParametersEnum.ThrustCombatSpeedGraphOneProgressValue;
//      }
//      else if (flag)
//      {
//        managedParameterEnum1 = ManagedParametersEnum.OverSwingCombatSpeedGraphZeroProgressValue;
//        managedParameterEnum2 = ManagedParametersEnum.OverSwingCombatSpeedGraphFirstMaximumPoint;
//        managedParameterEnum3 = ManagedParametersEnum.OverSwingCombatSpeedGraphSecondMaximumPoint;
//        managedParameterEnum4 = ManagedParametersEnum.OverSwingCombatSpeedGraphOneProgressValue;
//      }
//      else
//      {
//        managedParameterEnum1 = ManagedParametersEnum.SwingCombatSpeedGraphZeroProgressValue;
//        managedParameterEnum2 = ManagedParametersEnum.SwingCombatSpeedGraphFirstMaximumPoint;
//        managedParameterEnum3 = ManagedParametersEnum.SwingCombatSpeedGraphSecondMaximumPoint;
//        managedParameterEnum4 = ManagedParametersEnum.SwingCombatSpeedGraphOneProgressValue;
//      }
//      float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum1);
//      float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum2);
//      float managedParameter3 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum3);
//      float managedParameter4 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum4);
//      return (double) progress >= (double) managedParameter2 ? ((double) managedParameter3 >= (double) progress ? 1f : (float) (((double) managedParameter4 - 1.0) / (1.0 - (double) managedParameter3) * ((double) progress - (double) managedParameter3) + 1.0)) : (1f - managedParameter1) / managedParameter2 * progress + managedParameter1;
//    }

//    private static float ConvertBaseAttackMagnitude(
//      WeaponComponentData weapon,
//      StrikeType strikeType,
//      float baseMagnitude)
//    {
//      return baseMagnitude * (strikeType == StrikeType.Thrust ? weapon.ThrustDamageFactor : weapon.SwingDamageFactor);
//    }

//    private static Vec2 GetAgentVelocityContribution(
//      bool hasAgentMountAgent,
//      Vec2 agentMovementVelocity,
//      Vec2 agentMountMovementDirection,
//      float agentMovementDirectionAsAngle)
//    {
//      Vec2 zero = Vec2.Zero;
//      Vec2 velocityContribution;
//      if (hasAgentMountAgent)
//      {
//        velocityContribution = agentMovementVelocity.y * agentMountMovementDirection;
//      }
//      else
//      {
//        velocityContribution = agentMovementVelocity;
//        velocityContribution.RotateCCW(agentMovementDirectionAsAngle);
//      }
//      return velocityContribution;
//    }

//    private static float GetEntityDamageMultiplier(
//      bool isAttackerAgentDoingPassiveAttack,
//      WeaponComponentData weapon,
//      DamageTypes damageType,
//      bool isFlammable)
//    {
//      float damageMultiplier = 1f;
//      if (isAttackerAgentDoingPassiveAttack)
//        damageMultiplier *= 0.2f;
//      if (weapon != null)
//      {
//        if (weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.BonusAgainstShield))
//          damageMultiplier *= 1.2f;
//        switch (damageType)
//        {
//          case DamageTypes.Cut:
//            damageMultiplier *= 0.8f;
//            break;
//          case DamageTypes.Pierce:
//            damageMultiplier *= 0.1f;
//            break;
//        }
//        if (isFlammable && weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.Burning))
//          damageMultiplier *= 1.5f;
//      }
//      return damageMultiplier;
//    }

//    private static float ComputeSpeedBonus(
//      float baseMagnitude,
//      float baseMagnitudeWithoutSpeedBonus)
//    {
//      return (float) ((double) baseMagnitude / (double) baseMagnitudeWithoutSpeedBonus - 1.0);
//    }

//    private static float ComputeRelativeSpeedDiffOfAgents(Agent agentA, Agent agentB)
//    {
//      Vec2 zero1 = Vec2.Zero;
//      Vec2 vec2_1;
//      if (agentA.MountAgent != null)
//      {
//        vec2_1 = agentA.MountAgent.MovementVelocity.y * agentA.MountAgent.GetMovementDirection();
//      }
//      else
//      {
//        vec2_1 = agentA.MovementVelocity;
//        vec2_1.RotateCCW(agentA.MovementDirectionAsAngle);
//      }
//      Vec2 zero2 = Vec2.Zero;
//      Vec2 vec2_2;
//      if (agentB.MountAgent != null)
//      {
//        vec2_2 = agentB.MountAgent.MovementVelocity.y * agentB.MountAgent.GetMovementDirection();
//      }
//      else
//      {
//        vec2_2 = agentB.MovementVelocity;
//        vec2_2.RotateCCW(agentB.MovementDirectionAsAngle);
//      }
//      return (vec2_1 - vec2_2).Length;
//    }

//    private static void ComputeBlowDamage(
//      in AttackInformation attackInformation,
//      in AttackCollisionData attackCollisionData,
//      WeaponComponentData attackerWeapon,
//      DamageTypes damageType,
//      float magnitude,
//      int speedBonus,
//      bool cancelDamage,
//      out int inflictedDamage,
//      out int absorbedByArmor,
//      out bool isSneakAttack)
//    {
//      isSneakAttack = false;
//      float armorAmountFloat = attackInformation.ArmorAmountFloat;
//      WeaponComponentData shieldOnBack = attackInformation.ShieldOnBack;
//      AgentFlag victimAgentFlags = attackInformation.VictimAgentFlags;
//      double absorbedDamageRatio1 = (double) attackInformation.VictimAgentAbsorbedDamageRatio;
//      float multiplierOfBone = attackInformation.DamageMultiplierOfBone;
//      float difficultyMultiplier = attackInformation.CombatDifficultyMultiplier;
//      bool blockedWithShield = attackCollisionData.AttackBlockedWithShield;
//      AttackCollisionData attackCollisionData1 = attackCollisionData;
//      int num1 = attackCollisionData1.CollidedWithShieldOnBack ? 1 : 0;
//      attackCollisionData1 = attackCollisionData;
//      bool isFallDamage = attackCollisionData1.IsFallDamage;
//      BasicCharacterObject attackerAgentCharacter = attackInformation.AttackerAgentCharacter;
//      BasicCharacterObject captainCharacter1 = attackInformation.AttackerCaptainCharacter;
//      BasicCharacterObject victimAgentCharacter = attackInformation.VictimAgentCharacter;
//      BasicCharacterObject captainCharacter2 = attackInformation.VictimCaptainCharacter;
//      float armorEffectiveness = 0.0f;
//      if (!isFallDamage)
//        armorEffectiveness = MissionGameModels.Current.StrikeMagnitudeModel.CalculateAdjustedArmorForBlow(in attackInformation, in attackCollisionData, armorAmountFloat, attackerAgentCharacter, captainCharacter1, victimAgentCharacter, captainCharacter2, attackerWeapon);
//      if (num1 != 0 && shieldOnBack != null)
//        armorEffectiveness += 10f;
//      float absorbedDamageRatio2 = (float) absorbedDamageRatio1;
//      float rawDamage = MissionGameModels.Current.StrikeMagnitudeModel.ComputeRawDamage(damageType, magnitude, armorEffectiveness, absorbedDamageRatio2);
//      float num2 = 1f;
//      if (!blockedWithShield && !isFallDamage)
//      {
//        float num3 = num2 * multiplierOfBone;
//        if (MissionGameModels.Current.AgentApplyDamageModel.CanWeaponDealSneakAttack(in attackInformation, attackerWeapon))
//        {
//          float attackMultiplier = MissionGameModels.Current.AgentStatCalculateModel.GetSneakAttackMultiplier(attackInformation.AttackerAgent, attackerWeapon);
//          num3 *= attackMultiplier;
//          isSneakAttack = true;
//        }
//        num2 = num3 * difficultyMultiplier;
//      }
//      float f = rawDamage * num2;
//      inflictedDamage = MBMath.ClampInt(MathF.Ceiling(f), 0, 2000);
//      int num4 = MBMath.ClampInt(MathF.Ceiling(MissionGameModels.Current.StrikeMagnitudeModel.ComputeRawDamage(damageType, magnitude, 0.0f, absorbedDamageRatio2) * num2), 0, 2000);
//      absorbedByArmor = num4 - inflictedDamage;
//    }

//    private static void ComputeBlowDamageOnShield(
//      in AttackInformation attackInformation,
//      in AttackCollisionData attackCollisionData,
//      WeaponComponentData attackerWeapon,
//      float blowMagnitude,
//      out int inflictedDamage)
//    {
//      inflictedDamage = 0;
//      MissionWeapon victimShield = attackInformation.VictimShield;
//      if (!victimShield.CurrentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CanBlockRanged) || !attackInformation.CanGiveDamageToAgentShield)
//        return;
//      DamageTypes damageType = (DamageTypes) attackCollisionData.DamageType;
//      int armorForCurrentUsage = victimShield.GetGetModifiedArmorForCurrentUsage();
//      float absorbedDamageRatio = 1f;
//      float rawDamage = MissionGameModels.Current.StrikeMagnitudeModel.ComputeRawDamage(damageType, blowMagnitude, (float) armorForCurrentUsage, absorbedDamageRatio);
//      if (attackCollisionData.IsMissile)
//      {
//        if (attackerWeapon.WeaponClass == WeaponClass.ThrowingAxe)
//          rawDamage *= 0.3f;
//        else if (attackerWeapon.WeaponClass == WeaponClass.Javelin)
//          rawDamage *= 0.5f;
//        else if (attackerWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CanPenetrateShield) && attackerWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.MultiplePenetration))
//          rawDamage *= 0.5f;
//        else
//          rawDamage *= 0.15f;
//      }
//      else
//      {
//        switch (attackCollisionData.DamageType)
//        {
//          case 0:
//          case 2:
//            rawDamage *= 0.7f;
//            break;
//          case 1:
//            rawDamage *= 0.5f;
//            break;
//        }
//      }
//      if (attackerWeapon != null && attackerWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.BonusAgainstShield))
//        rawDamage *= 2f;
//      if ((double) rawDamage <= 0.0)
//        return;
//      if (!attackInformation.IsVictimAgentLeftStance)
//        rawDamage *= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldRightStanceBlockDamageMultiplier);
//      if (attackCollisionData.CorrectSideShieldBlock)
//        rawDamage *= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldCorrectSideBlockDamageMultiplier);
//      float shieldDamage = MissionGameModels.Current.AgentApplyDamageModel.CalculateShieldDamage(in attackInformation, rawDamage);
//      inflictedDamage = (int) shieldDamage;
//    }

//    public static float CalculateBaseMeleeBlowMagnitude(
//      in AttackInformation attackInformation,
//      in AttackCollisionData collisionData,
//      StrikeType strikeType,
//      float progressEffect,
//      float impactPointAsPercent,
//      float exraLinearSpeed)
//    {
//      WeaponComponentData currentUsageItem = attackInformation.AttackerWeapon.CurrentUsageItem;
//      float num1 = MathF.Sqrt(progressEffect);
//      float meleeBlowMagnitude;
//      if (strikeType == StrikeType.Thrust)
//      {
//        exraLinearSpeed *= 0.5f;
//        float thrustSpeed = (float) attackInformation.AttackerWeapon.GetModifiedThrustSpeedForCurrentUsage() / 11.7647057f * num1;
//        meleeBlowMagnitude = MissionGameModels.Current.StrikeMagnitudeModel.CalculateStrikeMagnitudeForThrust(in attackInformation, in collisionData, in attackInformation.AttackerWeapon, thrustSpeed, exraLinearSpeed);
//      }
//      else
//      {
//        exraLinearSpeed *= 0.7f;
//        float swingSpeed = (float) attackInformation.AttackerWeapon.GetModifiedSwingSpeedForCurrentUsage() / 4.5454545f * num1;
//        float num2 = MBMath.ClampFloat(0.4f / currentUsageItem.GetRealWeaponLength(), 0.0f, 1f);
//        float num3 = MathF.Min(0.93f, impactPointAsPercent);
//        float num4 = MathF.Min(0.93f, impactPointAsPercent + num2);
//        float num5 = 0.0f;
//        for (int index = 0; index < 5; ++index)
//        {
//          float impactPointAsPercent1 = num3 + (float) ((double) index / 4.0 * ((double) num4 - (double) num3));
//          float magnitudeForSwing = MissionGameModels.Current.StrikeMagnitudeModel.CalculateStrikeMagnitudeForSwing(in attackInformation, in collisionData, in attackInformation.AttackerWeapon, swingSpeed, impactPointAsPercent1, exraLinearSpeed);
//          if ((double) num5 < (double) magnitudeForSwing)
//            num5 = magnitudeForSwing;
//        }
//        meleeBlowMagnitude = num5;
//      }
//      return meleeBlowMagnitude;
//    }

//    private static void ComputeBlowMagnitude(
//      in AttackCollisionData acd,
//      in AttackInformation attackInformation,
//      float momentumRemaining,
//      bool cancelDamage,
//      bool hitWithAnotherBone,
//      Vec2 attackerVelocity,
//      Vec2 victimVelocity,
//      out float baseMagnitude,
//      out float specialMagnitude,
//      out float movementSpeedDamageModifier,
//      out int speedBonusInt)
//    {
//      StrikeType strikeType = (StrikeType) acd.StrikeType;
//      Agent.UsageDirection attackDirection = acd.AttackDirection;
//      bool attackerIsDoingPassiveAttack = !attackInformation.IsAttackerAgentNull && attackInformation.IsAttackerAgentHuman && attackInformation.IsAttackerAgentActive && attackInformation.IsAttackerAgentDoingPassiveAttack;
//      movementSpeedDamageModifier = 0.0f;
//      speedBonusInt = 0;
//      if (acd.IsMissile)
//        MissionCombatMechanicsHelper.ComputeBlowMagnitudeMissile(in attackInformation, in acd, momentumRemaining, in victimVelocity, out baseMagnitude, out specialMagnitude);
//      else if (acd.IsFallDamage)
//        MissionCombatMechanicsHelper.ComputeBlowMagnitudeFromFall(in attackInformation, in acd, out baseMagnitude, out specialMagnitude);
//      else if (acd.IsHorseCharge)
//        MissionCombatMechanicsHelper.ComputeBlowMagnitudeFromHorseCharge(in attackInformation, in acd, attackerVelocity, victimVelocity, out baseMagnitude, out specialMagnitude);
//      else
//        MissionCombatMechanicsHelper.ComputeBlowMagnitudeMelee(in attackInformation, in acd, momentumRemaining, cancelDamage, hitWithAnotherBone, strikeType, attackDirection, attackerIsDoingPassiveAttack, attackerVelocity, victimVelocity, out baseMagnitude, out specialMagnitude, out movementSpeedDamageModifier, out speedBonusInt);
//      specialMagnitude = MBMath.ClampFloat(specialMagnitude, 0.0f, 500f);
//    }

//    private static void ComputeBlowMagnitudeMelee(
//      in AttackInformation attackInformation,
//      in AttackCollisionData collisionData,
//      float momentumRemaining,
//      bool cancelDamage,
//      bool hitWithAnotherBone,
//      StrikeType strikeType,
//      Agent.UsageDirection attackDirection,
//      bool attackerIsDoingPassiveAttack,
//      Vec2 attackerVelocity,
//      Vec2 victimVelocity,
//      out float baseMagnitude,
//      out float specialMagnitude,
//      out float movementSpeedDamageModifier,
//      out int speedBonusInt)
//    {
//      Vec3 currentWeaponOffset = attackInformation.AttackerAgentCurrentWeaponOffset;
//      movementSpeedDamageModifier = 0.0f;
//      speedBonusInt = 0;
//      BasicCharacterObject attackerAgentCharacter = attackInformation.AttackerAgentCharacter;
//      if (collisionData.IsAlternativeAttack)
//      {
//        WeaponComponentData currentUsageItem = attackInformation.AttackerWeapon.CurrentUsageItem;
//        baseMagnitude = MissionGameModels.Current.AgentApplyDamageModel.CalculateAlternativeAttackDamage(in attackInformation, in collisionData, currentUsageItem);
//        baseMagnitude *= momentumRemaining;
//        specialMagnitude = baseMagnitude;
//      }
//      else
//      {
//        Vec3 weaponBlowDir = collisionData.WeaponBlowDir;
//        Vec2 vb = attackerVelocity - victimVelocity;
//        double num1 = (double) vb.Normalize();
//        float num2 = Vec2.DotProduct(weaponBlowDir.AsVec2, vb);
//        if ((double) num2 > 0.0)
//          num2 = MathF.Min(num2 + 0.2f, 1f);
//        double num3 = (double) num2;
//        float num4 = (float) (num1 * num3);
//        if (attackInformation.AttackerWeapon.IsEmpty)
//        {
//          float progressEffect = MissionCombatMechanicsHelper.SpeedGraphFunction(collisionData.AttackProgress, strikeType, attackDirection);
//          baseMagnitude = MissionGameModels.Current.StrikeMagnitudeModel.CalculateStrikeMagnitudeForUnarmedAttack(in attackInformation, in collisionData, progressEffect, momentumRemaining);
//          specialMagnitude = baseMagnitude;
//        }
//        else
//        {
//          float z = currentWeaponOffset.z;
//          WeaponComponentData currentUsageItem = attackInformation.AttackerWeapon.CurrentUsageItem;
//          float maxValue = currentUsageItem.GetRealWeaponLength() + z;
//          AttackCollisionData attackCollisionData = collisionData;
//          float impactPointAsPercent = MBMath.ClampFloat(attackCollisionData.CollisionDistanceOnWeapon, -0.2f, maxValue) / maxValue;
//          if (attackerIsDoingPassiveAttack)
//          {
//            baseMagnitude = attackInformation.DoesAttackerHaveMountAgent || attackInformation.DoesVictimHaveMountAgent || attackInformation.IsVictimAgentMount ? CombatStatCalculator.CalculateBaseBlowMagnitudeForPassiveUsage(attackInformation.AttackerWeapon.Item.Weight, num4) : 0.0f;
//            baseMagnitude = MissionGameModels.Current.AgentApplyDamageModel.CalculatePassiveAttackDamage(attackerAgentCharacter, in collisionData, baseMagnitude);
//          }
//          else
//          {
//            attackCollisionData = collisionData;
//            float progressEffect = MissionCombatMechanicsHelper.SpeedGraphFunction(attackCollisionData.AttackProgress, strikeType, attackDirection);
//            baseMagnitude = MissionCombatMechanicsHelper.CalculateBaseMeleeBlowMagnitude(in attackInformation, in collisionData, strikeType, progressEffect, impactPointAsPercent, num4);
//            if ((double) baseMagnitude >= 0.0 && (double) progressEffect > 0.699999988079071)
//            {
//              float meleeBlowMagnitude = MissionCombatMechanicsHelper.CalculateBaseMeleeBlowMagnitude(in attackInformation, in collisionData, strikeType, progressEffect, impactPointAsPercent, 0.0f);
//              movementSpeedDamageModifier = MissionCombatMechanicsHelper.ComputeSpeedBonus(baseMagnitude, meleeBlowMagnitude);
//              speedBonusInt = MathF.Round(100f * movementSpeedDamageModifier);
//              speedBonusInt = MBMath.ClampInt(speedBonusInt, -1000, 1000);
//            }
//          }
//          baseMagnitude *= momentumRemaining;
//          float num5 = 1f;
//          if (hitWithAnotherBone)
//            num5 = strikeType != StrikeType.Thrust ? ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.SwingHitWithArmDamageMultiplier) : ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ThrustHitWithArmDamageMultiplier);
//          else if (strikeType == StrikeType.Thrust)
//          {
//            attackCollisionData = collisionData;
//            if (!attackCollisionData.ThrustTipHit)
//            {
//              attackCollisionData = collisionData;
//              if (!attackCollisionData.AttackBlockedWithShield)
//                num5 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.NonTipThrustHitDamageMultiplier);
//            }
//          }
//          baseMagnitude *= num5;
//          if (attackInformation.AttackerAgent != null)
//          {
//            float damageMultiplier = MissionGameModels.Current.AgentStatCalculateModel.GetWeaponDamageMultiplier(attackInformation.AttackerAgent, currentUsageItem);
//            baseMagnitude *= damageMultiplier;
//          }
//          specialMagnitude = MissionCombatMechanicsHelper.ConvertBaseAttackMagnitude(currentUsageItem, strikeType, baseMagnitude);
//        }
//      }
//    }

//    private static void ComputeBlowMagnitudeFromHorseCharge(
//      in AttackInformation attackInformation,
//      in AttackCollisionData acd,
//      Vec2 attackerAgentVelocity,
//      Vec2 victimAgentVelocity,
//      out float baseMagnitude,
//      out float specialMagnitude)
//    {
//      Vec2 chargerMovementDirection = attackInformation.AttackerAgentMovementDirection;
//      Vec2 vec2_1 = chargerMovementDirection * Vec2.DotProduct(victimAgentVelocity, chargerMovementDirection);
//      Vec2 vec2_2 = attackerAgentVelocity - vec2_1;
//      float num1 = MissionCombatMechanicsHelper.ChargeDamageDotProduct(in attackInformation.VictimAgentPosition, in chargerMovementDirection, acd.CollisionGlobalPosition);
//      float num2 = vec2_2.Length * num1;
//      baseMagnitude = num2 * num2 * num1 * attackInformation.AttackerAgentMountChargeDamageProperty;
//      specialMagnitude = baseMagnitude;
//    }

//    private static void ComputeBlowMagnitudeMissile(
//      in AttackInformation attackInformation,
//      in AttackCollisionData collisionData,
//      float momentumRemaining,
//      in Vec2 victimVelocity,
//      out float baseMagnitude,
//      out float specialMagnitude)
//    {
//      float missileSpeed = attackInformation.IsVictimAgentNull ? collisionData.MissileVelocity.Length : (victimVelocity.ToVec3() - collisionData.MissileVelocity).Length;
//      baseMagnitude = MissionGameModels.Current.StrikeMagnitudeModel.CalculateStrikeMagnitudeForMissile(in attackInformation, in collisionData, in attackInformation.AttackerWeapon, missileSpeed);
//      baseMagnitude *= momentumRemaining;
//      if (attackInformation.AttackerAgent != null)
//      {
//        float damageMultiplier = MissionGameModels.Current.AgentStatCalculateModel.GetWeaponDamageMultiplier(attackInformation.AttackerAgent, attackInformation.AttackerWeapon.CurrentUsageItem);
//        baseMagnitude *= damageMultiplier;
//      }
//      specialMagnitude = baseMagnitude;
//    }

//    private static void ComputeBlowMagnitudeFromFall(
//      in AttackInformation attackInformation,
//      in AttackCollisionData acd,
//      out float baseMagnitude,
//      out float specialMagnitude)
//    {
//      float victimAgentScale = attackInformation.VictimAgentScale;
//      float num1 = attackInformation.VictimAgentWeight * victimAgentScale * victimAgentScale;
//      float num2 = MathF.Sqrt((float) (1.0 + (double) attackInformation.VictimAgentTotalEncumbrance / (double) num1));
//      float num3 = -acd.VictimAgentCurVelocity.z;
//      if (attackInformation.DoesVictimHaveMountAgent)
//      {
//        float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FallSpeedReductionMultiplierForRiderDamage);
//        num3 *= managedParameter;
//      }
//      float num4 = !attackInformation.IsVictimAgentHuman ? 1.41f : 1f;
//      float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FallDamageMultiplier);
//      float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FallDamageAbsorption);
//      baseMagnitude = (num3 * num3 * managedParameter1 - managedParameter2) * num2 * num4;
//      if ((double) baseMagnitude < 3.0)
//        baseMagnitude = 0.0f;
//      else if ((double) baseMagnitude > 499.89999389648438)
//        baseMagnitude = 499.9f;
//      specialMagnitude = baseMagnitude;
//    }
//  }
//}
