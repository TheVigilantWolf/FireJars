//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.Core.DrivenProperty
//// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: 4282646E-E702-4B98-B76A-B7486FBD2594
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.Core.dll

//#nullable disable
//namespace TaleWorlds.Core
//{
//  public enum DrivenProperty
//  {
//    None = -1, // 0xFFFFFFFF
//    AiRangedHorsebackMissileRange = 0,
//    AiFacingMissileWatch = 1,
//    AiFlyingMissileCheckRadius = 2,
//    AiShootFreq = 3,
//    AiWaitBeforeShootFactor = 4,
//    AIBlockOnDecideAbility = 5,
//    AIParryOnDecideAbility = 6,
//    AiTryChamberAttackOnDecide = 7,
//    AIAttackOnParryChance = 8,
//    AiAttackOnParryTiming = 9,
//    AIDecideOnAttackChance = 10, // 0x0000000A
//    AIParryOnAttackAbility = 11, // 0x0000000B
//    AiKick = 12, // 0x0000000C
//    AiAttackCalculationMaxTimeFactor = 13, // 0x0000000D
//    AiDecideOnAttackWhenReceiveHitTiming = 14, // 0x0000000E
//    AiDecideOnAttackContinueAction = 15, // 0x0000000F
//    AiDecideOnAttackingContinue = 16, // 0x00000010
//    AIParryOnAttackingContinueAbility = 17, // 0x00000011
//    AIDecideOnRealizeEnemyBlockingAttackAbility = 18, // 0x00000012
//    AIRealizeBlockingFromIncorrectSideAbility = 19, // 0x00000013
//    AiAttackingShieldDefenseChance = 20, // 0x00000014
//    AiAttackingShieldDefenseTimer = 21, // 0x00000015
//    AiCheckApplyMovementInterval = 22, // 0x00000016
//    AiCheckCalculateMovementInterval = 23, // 0x00000017
//    AiCheckDecideSimpleBehaviorInterval = 24, // 0x00000018
//    AiCheckDoSimpleBehaviorInterval = 25, // 0x00000019
//    AiMovementDelayFactor = 26, // 0x0000001A
//    AiParryDecisionChangeValue = 27, // 0x0000001B
//    AiDefendWithShieldDecisionChanceValue = 28, // 0x0000001C
//    AiMoveEnemySideTimeValue = 29, // 0x0000001D
//    AiMinimumDistanceToContinueFactor = 30, // 0x0000001E
//    AiChargeHorsebackTargetDistFactor = 31, // 0x0000001F
//    AiRangerLeadErrorMin = 32, // 0x00000020
//    AiRangerLeadErrorMax = 33, // 0x00000021
//    AiRangerVerticalErrorMultiplier = 34, // 0x00000022
//    AiRangerHorizontalErrorMultiplier = 35, // 0x00000023
//    AIAttackOnDecideChance = 36, // 0x00000024
//    AiRaiseShieldDelayTimeBase = 37, // 0x00000025
//    AiUseShieldAgainstEnemyMissileProbability = 38, // 0x00000026
//    AiSpeciesIndex = 39, // 0x00000027
//    AiRandomizedDefendDirectionChance = 40, // 0x00000028
//    AiShooterError = 41, // 0x00000029
//    AiWeaponFavorMultiplierMelee = 42, // 0x0000002A
//    AiWeaponFavorMultiplierRanged = 43, // 0x0000002B
//    AiWeaponFavorMultiplierPolearm = 44, // 0x0000002C
//    AISetNoAttackTimerAfterBeingHitAbility = 45, // 0x0000002D
//    AISetNoAttackTimerAfterBeingParriedAbility = 46, // 0x0000002E
//    AISetNoDefendTimerAfterHittingAbility = 47, // 0x0000002F
//    AISetNoDefendTimerAfterParryingAbility = 48, // 0x00000030
//    AIEstimateStunDurationPrecision = 49, // 0x00000031
//    AIHoldingReadyMaxDuration = 50, // 0x00000032
//    AIHoldingReadyVariationPercentage = 51, // 0x00000033
//    MountChargeDamage = 52, // 0x00000034
//    MountDifficulty = 53, // 0x00000035
//    ArmorEncumbrance = 54, // 0x00000036
//    ArmorHead = 55, // 0x00000037
//    ArmorTorso = 56, // 0x00000038
//    ArmorLegs = 57, // 0x00000039
//    ArmorArms = 58, // 0x0000003A
//    UseRealisticBlocking = 59, // 0x0000003B
//    ThrowingWeaponDamageMultiplierBonus = 60, // 0x0000003C
//    MeleeWeaponDamageMultiplierBonus = 61, // 0x0000003D
//    ArmorPenetrationMultiplierCrossbow = 62, // 0x0000003E
//    ArmorPenetrationMultiplierBow = 63, // 0x0000003F
//    DrivenPropertiesCalculatedAtSpawnEnd = 64, // 0x00000040
//    WeaponsEncumbrance = 64, // 0x00000040
//    DamageMultiplierBonus = 65, // 0x00000041
//    SwingSpeedMultiplier = 66, // 0x00000042
//    ThrustOrRangedReadySpeedMultiplier = 67, // 0x00000043
//    HandlingMultiplier = 68, // 0x00000044
//    ReloadSpeed = 69, // 0x00000045
//    MissileSpeedMultiplier = 70, // 0x00000046
//    WeaponInaccuracy = 71, // 0x00000047
//    AiShooterErrorWoRangeUpdate = 72, // 0x00000048
//    WeaponWorstMobileAccuracyPenalty = 73, // 0x00000049
//    WeaponWorstUnsteadyAccuracyPenalty = 74, // 0x0000004A
//    WeaponBestAccuracyWaitTime = 75, // 0x0000004B
//    WeaponUnsteadyBeginTime = 76, // 0x0000004C
//    WeaponUnsteadyEndTime = 77, // 0x0000004D
//    WeaponRotationalAccuracyPenaltyInRadians = 78, // 0x0000004E
//    AttributeRiding = 79, // 0x0000004F
//    AttributeShield = 80, // 0x00000050
//    AttributeShieldMissileCollisionBodySizeAdder = 81, // 0x00000051
//    ShieldBashStunDurationMultiplier = 82, // 0x00000052
//    KickStunDurationMultiplier = 83, // 0x00000053
//    ReloadMovementPenaltyFactor = 84, // 0x00000054
//    TopSpeedReachDuration = 85, // 0x00000055
//    MaxSpeedMultiplier = 86, // 0x00000056
//    CombatMaxSpeedMultiplier = 87, // 0x00000057
//    CrouchedSpeedMultiplier = 88, // 0x00000058
//    AttributeHorseArchery = 89, // 0x00000059
//    AttributeCourage = 90, // 0x0000005A
//    MountManeuver = 91, // 0x0000005B
//    MountSpeed = 92, // 0x0000005C
//    MountDashAccelerationMultiplier = 93, // 0x0000005D
//    BipedalRangedReadySpeedMultiplier = 94, // 0x0000005E
//    BipedalRangedReloadSpeedMultiplier = 95, // 0x0000005F
//    OffhandWeaponDefendSpeedMultiplier = 96, // 0x00000060
//    Count = 97, // 0x00000061
//  }
//}
