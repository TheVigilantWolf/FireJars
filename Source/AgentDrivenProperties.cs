//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.AgentDrivenProperties
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using TaleWorlds.Core;
//using TaleWorlds.Library;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public class AgentDrivenProperties
//  {
//    private readonly float[] _statValues;

//    internal float[] Values => this._statValues;

//    public AgentDrivenProperties() => this._statValues = new float[97];

//    public float GetStat(DrivenProperty propertyEnum) => this._statValues[(int) propertyEnum];

//    public void SetStat(DrivenProperty propertyEnum, float value)
//    {
//      this._statValues[(int) propertyEnum] = value;
//    }

//    public float SwingSpeedMultiplier
//    {
//      get => this.GetStat(DrivenProperty.SwingSpeedMultiplier);
//      set => this.SetStat(DrivenProperty.SwingSpeedMultiplier, value);
//    }

//    public float ThrustOrRangedReadySpeedMultiplier
//    {
//      get => this.GetStat(DrivenProperty.ThrustOrRangedReadySpeedMultiplier);
//      set => this.SetStat(DrivenProperty.ThrustOrRangedReadySpeedMultiplier, value);
//    }

//    public float HandlingMultiplier
//    {
//      get => this.GetStat(DrivenProperty.HandlingMultiplier);
//      set => this.SetStat(DrivenProperty.HandlingMultiplier, value);
//    }

//    public float ReloadSpeed
//    {
//      get => this.GetStat(DrivenProperty.ReloadSpeed);
//      set => this.SetStat(DrivenProperty.ReloadSpeed, value);
//    }

//    public float MissileSpeedMultiplier
//    {
//      get => this.GetStat(DrivenProperty.MissileSpeedMultiplier);
//      set => this.SetStat(DrivenProperty.MissileSpeedMultiplier, value);
//    }

//    public float WeaponInaccuracy
//    {
//      get => this.GetStat(DrivenProperty.WeaponInaccuracy);
//      set => this.SetStat(DrivenProperty.WeaponInaccuracy, value);
//    }

//    public float AiShooterErrorWoRangeUpdate
//    {
//      get => this.GetStat(DrivenProperty.AiShooterErrorWoRangeUpdate);
//      set => this.SetStat(DrivenProperty.AiShooterErrorWoRangeUpdate, value);
//    }

//    public float WeaponMaxMovementAccuracyPenalty
//    {
//      get => this.GetStat(DrivenProperty.WeaponWorstMobileAccuracyPenalty);
//      set => this.SetStat(DrivenProperty.WeaponWorstMobileAccuracyPenalty, value);
//    }

//    public float WeaponMaxUnsteadyAccuracyPenalty
//    {
//      get => this.GetStat(DrivenProperty.WeaponWorstUnsteadyAccuracyPenalty);
//      set => this.SetStat(DrivenProperty.WeaponWorstUnsteadyAccuracyPenalty, value);
//    }

//    public float WeaponBestAccuracyWaitTime
//    {
//      get => this.GetStat(DrivenProperty.WeaponBestAccuracyWaitTime);
//      set => this.SetStat(DrivenProperty.WeaponBestAccuracyWaitTime, value);
//    }

//    public float WeaponUnsteadyBeginTime
//    {
//      get => this.GetStat(DrivenProperty.WeaponUnsteadyBeginTime);
//      set => this.SetStat(DrivenProperty.WeaponUnsteadyBeginTime, value);
//    }

//    public float WeaponUnsteadyEndTime
//    {
//      get => this.GetStat(DrivenProperty.WeaponUnsteadyEndTime);
//      set => this.SetStat(DrivenProperty.WeaponUnsteadyEndTime, value);
//    }

//    public float WeaponRotationalAccuracyPenaltyInRadians
//    {
//      get => this.GetStat(DrivenProperty.WeaponRotationalAccuracyPenaltyInRadians);
//      set => this.SetStat(DrivenProperty.WeaponRotationalAccuracyPenaltyInRadians, value);
//    }

//    public float ArmorEncumbrance
//    {
//      get => this.GetStat(DrivenProperty.ArmorEncumbrance);
//      set => this.SetStat(DrivenProperty.ArmorEncumbrance, value);
//    }

//    public float DamageMultiplierBonus
//    {
//      get => this.GetStat(DrivenProperty.DamageMultiplierBonus);
//      set => this.SetStat(DrivenProperty.DamageMultiplierBonus, value);
//    }

//    public float ThrowingWeaponDamageMultiplierBonus
//    {
//      get => this.GetStat(DrivenProperty.ThrowingWeaponDamageMultiplierBonus);
//      set => this.SetStat(DrivenProperty.ThrowingWeaponDamageMultiplierBonus, value);
//    }

//    public float MeleeWeaponDamageMultiplierBonus
//    {
//      get => this.GetStat(DrivenProperty.MeleeWeaponDamageMultiplierBonus);
//      set => this.SetStat(DrivenProperty.MeleeWeaponDamageMultiplierBonus, value);
//    }

//    public float ArmorPenetrationMultiplierCrossbow
//    {
//      get => this.GetStat(DrivenProperty.ArmorPenetrationMultiplierCrossbow);
//      set => this.SetStat(DrivenProperty.ArmorPenetrationMultiplierCrossbow, value);
//    }

//    public float ArmorPenetrationMultiplierBow
//    {
//      get => this.GetStat(DrivenProperty.ArmorPenetrationMultiplierBow);
//      set => this.SetStat(DrivenProperty.ArmorPenetrationMultiplierBow, value);
//    }

//    public float WeaponsEncumbrance
//    {
//      get => this.GetStat(DrivenProperty.WeaponsEncumbrance);
//      set => this.SetStat(DrivenProperty.WeaponsEncumbrance, value);
//    }

//    public float ArmorHead
//    {
//      get => this.GetStat(DrivenProperty.ArmorHead);
//      set => this.SetStat(DrivenProperty.ArmorHead, value);
//    }

//    public float ArmorTorso
//    {
//      get => this.GetStat(DrivenProperty.ArmorTorso);
//      set => this.SetStat(DrivenProperty.ArmorTorso, value);
//    }

//    public float ArmorLegs
//    {
//      get => this.GetStat(DrivenProperty.ArmorLegs);
//      set => this.SetStat(DrivenProperty.ArmorLegs, value);
//    }

//    public float ArmorArms
//    {
//      get => this.GetStat(DrivenProperty.ArmorArms);
//      set => this.SetStat(DrivenProperty.ArmorArms, value);
//    }

//    public float AttributeRiding
//    {
//      get => this.GetStat(DrivenProperty.AttributeRiding);
//      set => this.SetStat(DrivenProperty.AttributeRiding, value);
//    }

//    public float AttributeShield
//    {
//      get => this.GetStat(DrivenProperty.AttributeShield);
//      set => this.SetStat(DrivenProperty.AttributeShield, value);
//    }

//    public float AttributeShieldMissileCollisionBodySizeAdder
//    {
//      get => this.GetStat(DrivenProperty.AttributeShieldMissileCollisionBodySizeAdder);
//      set => this.SetStat(DrivenProperty.AttributeShieldMissileCollisionBodySizeAdder, value);
//    }

//    public float ShieldBashStunDurationMultiplier
//    {
//      get => this.GetStat(DrivenProperty.ShieldBashStunDurationMultiplier);
//      set => this.SetStat(DrivenProperty.ShieldBashStunDurationMultiplier, value);
//    }

//    public float KickStunDurationMultiplier
//    {
//      get => this.GetStat(DrivenProperty.KickStunDurationMultiplier);
//      set => this.SetStat(DrivenProperty.KickStunDurationMultiplier, value);
//    }

//    public float ReloadMovementPenaltyFactor
//    {
//      get => this.GetStat(DrivenProperty.ReloadMovementPenaltyFactor);
//      set => this.SetStat(DrivenProperty.ReloadMovementPenaltyFactor, value);
//    }

//    public float TopSpeedReachDuration
//    {
//      get => this.GetStat(DrivenProperty.TopSpeedReachDuration);
//      set => this.SetStat(DrivenProperty.TopSpeedReachDuration, value);
//    }

//    public float MaxSpeedMultiplier
//    {
//      get => this.GetStat(DrivenProperty.MaxSpeedMultiplier);
//      set => this.SetStat(DrivenProperty.MaxSpeedMultiplier, value);
//    }

//    public float CombatMaxSpeedMultiplier
//    {
//      get => this.GetStat(DrivenProperty.CombatMaxSpeedMultiplier);
//      set => this.SetStat(DrivenProperty.CombatMaxSpeedMultiplier, value);
//    }

//    public float CrouchedSpeedMultiplier
//    {
//      get => this.GetStat(DrivenProperty.CrouchedSpeedMultiplier);
//      set => this.SetStat(DrivenProperty.CrouchedSpeedMultiplier, value);
//    }

//    public float AttributeHorseArchery
//    {
//      get => this.GetStat(DrivenProperty.AttributeHorseArchery);
//      set => this.SetStat(DrivenProperty.AttributeHorseArchery, value);
//    }

//    public float AttributeCourage
//    {
//      get => this.GetStat(DrivenProperty.AttributeCourage);
//      set => this.SetStat(DrivenProperty.AttributeCourage, value);
//    }

//    public float MountManeuver
//    {
//      get => this.GetStat(DrivenProperty.MountManeuver);
//      set => this.SetStat(DrivenProperty.MountManeuver, value);
//    }

//    public float MountSpeed
//    {
//      get => this.GetStat(DrivenProperty.MountSpeed);
//      set => this.SetStat(DrivenProperty.MountSpeed, value);
//    }

//    public float MountDashAccelerationMultiplier
//    {
//      get => this.GetStat(DrivenProperty.MountDashAccelerationMultiplier);
//      set => this.SetStat(DrivenProperty.MountDashAccelerationMultiplier, value);
//    }

//    public float MountChargeDamage
//    {
//      get => this.GetStat(DrivenProperty.MountChargeDamage);
//      set => this.SetStat(DrivenProperty.MountChargeDamage, value);
//    }

//    public float MountDifficulty
//    {
//      get => this.GetStat(DrivenProperty.MountDifficulty);
//      set => this.SetStat(DrivenProperty.MountDifficulty, value);
//    }

//    public float BipedalRangedReadySpeedMultiplier
//    {
//      get => this.GetStat(DrivenProperty.BipedalRangedReadySpeedMultiplier);
//      set => this.SetStat(DrivenProperty.BipedalRangedReadySpeedMultiplier, value);
//    }

//    public float BipedalRangedReloadSpeedMultiplier
//    {
//      get => this.GetStat(DrivenProperty.BipedalRangedReloadSpeedMultiplier);
//      set => this.SetStat(DrivenProperty.BipedalRangedReloadSpeedMultiplier, value);
//    }

//    public float AiRangedHorsebackMissileRange
//    {
//      get => this.GetStat(DrivenProperty.AiRangedHorsebackMissileRange);
//      set => this.SetStat(DrivenProperty.AiRangedHorsebackMissileRange, value);
//    }

//    public float AiFacingMissileWatch
//    {
//      get => this.GetStat(DrivenProperty.AiFacingMissileWatch);
//      set => this.SetStat(DrivenProperty.AiFacingMissileWatch, value);
//    }

//    public float AiFlyingMissileCheckRadius
//    {
//      get => this.GetStat(DrivenProperty.AiFlyingMissileCheckRadius);
//      set => this.SetStat(DrivenProperty.AiFlyingMissileCheckRadius, value);
//    }

//    public float AiShootFreq
//    {
//      get => this.GetStat(DrivenProperty.AiShootFreq);
//      set => this.SetStat(DrivenProperty.AiShootFreq, value);
//    }

//    public float AiWaitBeforeShootFactor
//    {
//      get => this.GetStat(DrivenProperty.AiWaitBeforeShootFactor);
//      set => this.SetStat(DrivenProperty.AiWaitBeforeShootFactor, value);
//    }

//    public float AIBlockOnDecideAbility
//    {
//      get => this.GetStat(DrivenProperty.AIBlockOnDecideAbility);
//      set => this.SetStat(DrivenProperty.AIBlockOnDecideAbility, value);
//    }

//    public float AIParryOnDecideAbility
//    {
//      get => this.GetStat(DrivenProperty.AIParryOnDecideAbility);
//      set => this.SetStat(DrivenProperty.AIParryOnDecideAbility, value);
//    }

//    public float AiTryChamberAttackOnDecide
//    {
//      get => this.GetStat(DrivenProperty.AiTryChamberAttackOnDecide);
//      set => this.SetStat(DrivenProperty.AiTryChamberAttackOnDecide, value);
//    }

//    public float AIAttackOnParryChance
//    {
//      get => this.GetStat(DrivenProperty.AIAttackOnParryChance);
//      set => this.SetStat(DrivenProperty.AIAttackOnParryChance, value);
//    }

//    public float AiAttackOnParryTiming
//    {
//      get => this.GetStat(DrivenProperty.AiAttackOnParryTiming);
//      set => this.SetStat(DrivenProperty.AiAttackOnParryTiming, value);
//    }

//    public float AIDecideOnAttackChance
//    {
//      get => this.GetStat(DrivenProperty.AIDecideOnAttackChance);
//      set => this.SetStat(DrivenProperty.AIDecideOnAttackChance, value);
//    }

//    public float AIParryOnAttackAbility
//    {
//      get => this.GetStat(DrivenProperty.AIParryOnAttackAbility);
//      set => this.SetStat(DrivenProperty.AIParryOnAttackAbility, value);
//    }

//    public float AiKick
//    {
//      get => this.GetStat(DrivenProperty.AiKick);
//      set => this.SetStat(DrivenProperty.AiKick, value);
//    }

//    public float AiAttackCalculationMaxTimeFactor
//    {
//      get => this.GetStat(DrivenProperty.AiAttackCalculationMaxTimeFactor);
//      set => this.SetStat(DrivenProperty.AiAttackCalculationMaxTimeFactor, value);
//    }

//    public float AiDecideOnAttackWhenReceiveHitTiming
//    {
//      get => this.GetStat(DrivenProperty.AiDecideOnAttackWhenReceiveHitTiming);
//      set => this.SetStat(DrivenProperty.AiDecideOnAttackWhenReceiveHitTiming, value);
//    }

//    public float AiDecideOnAttackContinueAction
//    {
//      get => this.GetStat(DrivenProperty.AiDecideOnAttackContinueAction);
//      set => this.SetStat(DrivenProperty.AiDecideOnAttackContinueAction, value);
//    }

//    public float AiDecideOnAttackingContinue
//    {
//      get => this.GetStat(DrivenProperty.AiDecideOnAttackingContinue);
//      set => this.SetStat(DrivenProperty.AiDecideOnAttackingContinue, value);
//    }

//    public float AIParryOnAttackingContinueAbility
//    {
//      get => this.GetStat(DrivenProperty.AIParryOnAttackingContinueAbility);
//      set => this.SetStat(DrivenProperty.AIParryOnAttackingContinueAbility, value);
//    }

//    public float AIDecideOnRealizeEnemyBlockingAttackAbility
//    {
//      get => this.GetStat(DrivenProperty.AIDecideOnRealizeEnemyBlockingAttackAbility);
//      set => this.SetStat(DrivenProperty.AIDecideOnRealizeEnemyBlockingAttackAbility, value);
//    }

//    public float AIRealizeBlockingFromIncorrectSideAbility
//    {
//      get => this.GetStat(DrivenProperty.AIRealizeBlockingFromIncorrectSideAbility);
//      set => this.SetStat(DrivenProperty.AIRealizeBlockingFromIncorrectSideAbility, value);
//    }

//    public float AiAttackingShieldDefenseChance
//    {
//      get => this.GetStat(DrivenProperty.AiAttackingShieldDefenseChance);
//      set => this.SetStat(DrivenProperty.AiAttackingShieldDefenseChance, value);
//    }

//    public float AiAttackingShieldDefenseTimer
//    {
//      get => this.GetStat(DrivenProperty.AiAttackingShieldDefenseTimer);
//      set => this.SetStat(DrivenProperty.AiAttackingShieldDefenseTimer, value);
//    }

//    public float AiCheckApplyMovementInterval
//    {
//      get => this.GetStat(DrivenProperty.AiCheckApplyMovementInterval);
//      set => this.SetStat(DrivenProperty.AiCheckApplyMovementInterval, value);
//    }

//    public float AiCheckCalculateMovementInterval
//    {
//      get => this.GetStat(DrivenProperty.AiCheckCalculateMovementInterval);
//      set => this.SetStat(DrivenProperty.AiCheckCalculateMovementInterval, value);
//    }

//    public float AiCheckDecideSimpleBehaviorInterval
//    {
//      get => this.GetStat(DrivenProperty.AiCheckDecideSimpleBehaviorInterval);
//      set => this.SetStat(DrivenProperty.AiCheckDecideSimpleBehaviorInterval, value);
//    }

//    public float AiCheckDoSimpleBehaviorInterval
//    {
//      get => this.GetStat(DrivenProperty.AiCheckDoSimpleBehaviorInterval);
//      set => this.SetStat(DrivenProperty.AiCheckDoSimpleBehaviorInterval, value);
//    }

//    public float AiMovementDelayFactor
//    {
//      get => this.GetStat(DrivenProperty.AiMovementDelayFactor);
//      set => this.SetStat(DrivenProperty.AiMovementDelayFactor, value);
//    }

//    public float AiParryDecisionChangeValue
//    {
//      get => this.GetStat(DrivenProperty.AiParryDecisionChangeValue);
//      set => this.SetStat(DrivenProperty.AiParryDecisionChangeValue, value);
//    }

//    public float AiDefendWithShieldDecisionChanceValue
//    {
//      get => this.GetStat(DrivenProperty.AiDefendWithShieldDecisionChanceValue);
//      set => this.SetStat(DrivenProperty.AiDefendWithShieldDecisionChanceValue, value);
//    }

//    public float AiMoveEnemySideTimeValue
//    {
//      get => this.GetStat(DrivenProperty.AiMoveEnemySideTimeValue);
//      set => this.SetStat(DrivenProperty.AiMoveEnemySideTimeValue, value);
//    }

//    public float AiMinimumDistanceToContinueFactor
//    {
//      get => this.GetStat(DrivenProperty.AiMinimumDistanceToContinueFactor);
//      set => this.SetStat(DrivenProperty.AiMinimumDistanceToContinueFactor, value);
//    }

//    public float AiChargeHorsebackTargetDistFactor
//    {
//      get => this.GetStat(DrivenProperty.AiChargeHorsebackTargetDistFactor);
//      set => this.SetStat(DrivenProperty.AiChargeHorsebackTargetDistFactor, value);
//    }

//    public float AiRangerLeadErrorMin
//    {
//      get => this.GetStat(DrivenProperty.AiRangerLeadErrorMin);
//      set => this.SetStat(DrivenProperty.AiRangerLeadErrorMin, value);
//    }

//    public float AiRangerLeadErrorMax
//    {
//      get => this.GetStat(DrivenProperty.AiRangerLeadErrorMax);
//      set => this.SetStat(DrivenProperty.AiRangerLeadErrorMax, value);
//    }

//    public float AiRangerVerticalErrorMultiplier
//    {
//      get => this.GetStat(DrivenProperty.AiRangerVerticalErrorMultiplier);
//      set => this.SetStat(DrivenProperty.AiRangerVerticalErrorMultiplier, value);
//    }

//    public float AiRangerHorizontalErrorMultiplier
//    {
//      get => this.GetStat(DrivenProperty.AiRangerHorizontalErrorMultiplier);
//      set => this.SetStat(DrivenProperty.AiRangerHorizontalErrorMultiplier, value);
//    }

//    public float AIAttackOnDecideChance
//    {
//      get => this.GetStat(DrivenProperty.AIAttackOnDecideChance);
//      set => this.SetStat(DrivenProperty.AIAttackOnDecideChance, value);
//    }

//    public float AiRaiseShieldDelayTimeBase
//    {
//      get => this.GetStat(DrivenProperty.AiRaiseShieldDelayTimeBase);
//      set => this.SetStat(DrivenProperty.AiRaiseShieldDelayTimeBase, value);
//    }

//    public float AiUseShieldAgainstEnemyMissileProbability
//    {
//      get => this.GetStat(DrivenProperty.AiUseShieldAgainstEnemyMissileProbability);
//      set => this.SetStat(DrivenProperty.AiUseShieldAgainstEnemyMissileProbability, value);
//    }

//    public int AiSpeciesIndex
//    {
//      get => MathF.Round(this.GetStat(DrivenProperty.AiSpeciesIndex));
//      set => this.SetStat(DrivenProperty.AiSpeciesIndex, (float) value);
//    }

//    public float AiRandomizedDefendDirectionChance
//    {
//      get => this.GetStat(DrivenProperty.AiRandomizedDefendDirectionChance);
//      set => this.SetStat(DrivenProperty.AiRandomizedDefendDirectionChance, value);
//    }

//    public float AiShooterError
//    {
//      get => this.GetStat(DrivenProperty.AiShooterError);
//      set => this.SetStat(DrivenProperty.AiShooterError, value);
//    }

//    public float AiWeaponFavorMultiplierMelee
//    {
//      get => this.GetStat(DrivenProperty.AiWeaponFavorMultiplierMelee);
//      set => this.SetStat(DrivenProperty.AiWeaponFavorMultiplierMelee, value);
//    }

//    public float AiWeaponFavorMultiplierRanged
//    {
//      get => this.GetStat(DrivenProperty.AiWeaponFavorMultiplierRanged);
//      set => this.SetStat(DrivenProperty.AiWeaponFavorMultiplierRanged, value);
//    }

//    public float AiWeaponFavorMultiplierPolearm
//    {
//      get => this.GetStat(DrivenProperty.AiWeaponFavorMultiplierPolearm);
//      set => this.SetStat(DrivenProperty.AiWeaponFavorMultiplierPolearm, value);
//    }

//    public float AISetNoAttackTimerAfterBeingHitAbility
//    {
//      get => this.GetStat(DrivenProperty.AISetNoAttackTimerAfterBeingHitAbility);
//      set => this.SetStat(DrivenProperty.AISetNoAttackTimerAfterBeingHitAbility, value);
//    }

//    public float AISetNoAttackTimerAfterBeingParriedAbility
//    {
//      get => this.GetStat(DrivenProperty.AISetNoAttackTimerAfterBeingParriedAbility);
//      set => this.SetStat(DrivenProperty.AISetNoAttackTimerAfterBeingParriedAbility, value);
//    }

//    public float AISetNoDefendTimerAfterHittingAbility
//    {
//      get => this.GetStat(DrivenProperty.AISetNoDefendTimerAfterHittingAbility);
//      set => this.SetStat(DrivenProperty.AISetNoDefendTimerAfterHittingAbility, value);
//    }

//    public float AISetNoDefendTimerAfterParryingAbility
//    {
//      get => this.GetStat(DrivenProperty.AISetNoDefendTimerAfterParryingAbility);
//      set => this.SetStat(DrivenProperty.AISetNoDefendTimerAfterParryingAbility, value);
//    }

//    public float AIEstimateStunDurationPrecision
//    {
//      get => this.GetStat(DrivenProperty.AIEstimateStunDurationPrecision);
//      set => this.SetStat(DrivenProperty.AIEstimateStunDurationPrecision, value);
//    }

//    public float AIHoldingReadyMaxDuration
//    {
//      get => this.GetStat(DrivenProperty.AIHoldingReadyMaxDuration);
//      set => this.SetStat(DrivenProperty.AIHoldingReadyMaxDuration, value);
//    }

//    public float AIHoldingReadyVariationPercentage
//    {
//      get => this.GetStat(DrivenProperty.AIHoldingReadyVariationPercentage);
//      set => this.SetStat(DrivenProperty.AIHoldingReadyVariationPercentage, value);
//    }

//    public float OffhandWeaponDefendSpeedMultiplier
//    {
//      get => this.GetStat(DrivenProperty.OffhandWeaponDefendSpeedMultiplier);
//      set => this.SetStat(DrivenProperty.OffhandWeaponDefendSpeedMultiplier, value);
//    }

//    internal float[] InitializeDrivenProperties(
//      Agent agent,
//      Equipment spawnEquipment,
//      AgentBuildData agentBuildData)
//    {
//      MissionGameModels.Current.AgentStatCalculateModel.InitializeAgentStats(agent, spawnEquipment, this, agentBuildData);
//      MissionGameModels.Current.AgentStatCalculateModel.UpdateAgentStats(agent, this);
//      return this._statValues;
//    }

//    internal float[] UpdateDrivenProperties(Agent agent)
//    {
//      MissionGameModels.Current.AgentStatCalculateModel.UpdateAgentStats(agent, this);
//      return this._statValues;
//    }
//  }
//}
