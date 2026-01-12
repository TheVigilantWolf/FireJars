//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.BehaviorScreenedSkirmish
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System;
//using TaleWorlds.Core;
//using TaleWorlds.Engine;
//using TaleWorlds.Library;
//using TaleWorlds.Localization;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public class BehaviorScreenedSkirmish : BehaviorComponent
//  {
//    private Formation _mainFormation;
//    private bool _isFireAtWill = true;

//    public BehaviorScreenedSkirmish(Formation formation)
//      : base(formation)
//    {
//      this._behaviorSide = formation.AI.Side;
//      this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0 && f.AI.IsMainFormation));
//      this.CalculateCurrentOrder();
//    }

//    protected override void CalculateCurrentOrder()
//    {
//      WorldPosition worldPosition;
//      Vec2 vec2_1;
//      Vec2 vec2_2;
//      if (this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && this._mainFormation != null)
//      {
//        worldPosition = this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedMedianPosition;
//        vec2_1 = worldPosition.AsVec2 - this.Formation.CachedAveragePosition;
//        Vec2 vec2_3 = vec2_1.Normalized();
//        worldPosition = this._mainFormation.CachedMedianPosition;
//        vec2_1 = worldPosition.AsVec2 - this.Formation.CachedAveragePosition;
//        Vec2 v = vec2_1.Normalized();
//        vec2_2 = (double) vec2_3.DotProduct(v) > 0.5 ? this._mainFormation.FacingOrder.GetDirection(this._mainFormation) : vec2_3;
//      }
//      else
//        vec2_2 = this.Formation.Direction;
//      WorldPosition cachedMedianPosition;
//      if (this._mainFormation == null)
//      {
//        cachedMedianPosition = this.Formation.CachedMedianPosition;
//        cachedMedianPosition.SetVec2(this.Formation.CachedAveragePosition);
//      }
//      else
//      {
//        cachedMedianPosition = this._mainFormation.CachedMedianPosition;
//        cachedMedianPosition.SetVec2(cachedMedianPosition.AsVec2 - vec2_2 * (float) (((double) this._mainFormation.Depth + (double) this.Formation.Depth) * 0.5));
//      }
//      worldPosition = this.CurrentOrder.CreateNewOrderWorldPositionMT(this.Formation, WorldPosition.WorldPositionEnforcedCache.None);
//      if (worldPosition.IsValid)
//      {
//        if (this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null)
//        {
//          if (this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation)
//          {
//            vec2_1 = this.Formation.CachedAveragePosition;
//            ref Vec2 local = ref vec2_1;
//            worldPosition = this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedMedianPosition;
//            Vec2 asVec2 = worldPosition.GetNavMeshVec3().AsVec2;
//            if ((double) local.DistanceSquared(asVec2) < (double) this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MissileRangeAdjusted * (double) this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MissileRangeAdjusted)
//            {
//              worldPosition = this.CurrentOrder.CreateNewOrderWorldPositionMT(this.Formation, WorldPosition.WorldPositionEnforcedCache.NavMeshVec3);
//              if ((double) worldPosition.GetNavMeshVec3().DistanceSquared(cachedMedianPosition.GetNavMeshVec3()) < (double) this.Formation.Depth * (double) this.Formation.Depth)
//                goto label_12;
//            }
//          }
//        }
//        else
//          goto label_12;
//      }
//      this.CurrentOrder = MovementOrder.MovementOrderMove(cachedMedianPosition);
//label_12:
//      vec2_1 = this.CurrentFacingOrder.GetDirection(this.Formation);
//      if (vec2_1.IsValid && this.CurrentFacingOrder.OrderEnum != FacingOrder.FacingOrderEnum.LookAtEnemy && this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null)
//      {
//        vec2_1 = this.Formation.CachedAveragePosition;
//        ref Vec2 local = ref vec2_1;
//        worldPosition = this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedMedianPosition;
//        Vec2 asVec2 = worldPosition.GetNavMeshVec3().AsVec2;
//        if ((double) local.DistanceSquared(asVec2) < (double) this.Formation.QuerySystem.MissileRangeAdjusted * (double) this.Formation.QuerySystem.MissileRangeAdjusted)
//        {
//          if (this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation)
//            return;
//          vec2_1 = this.CurrentFacingOrder.GetDirection(this.Formation);
//          if ((double) vec2_1.DotProduct(vec2_2) > (double) MBMath.Lerp(0.5f, 1f, (float) (1.0 - (double) MBMath.ClampFloat(this.Formation.Width, 1f, 20f) * 0.05000000074505806)))
//            return;
//        }
//      }
//      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec2_2);
//    }

//    public override void TickOccasionally()
//    {
//      this.CalculateCurrentOrder();
//      FormationQuerySystem closestEnemyFormation = this.Formation.CachedClosestEnemyFormation;
//      int num1;
//      if (closestEnemyFormation != null)
//      {
//        Vec2 vec2 = this._mainFormation.CachedMedianPosition.AsVec2;
//        double num2 = (double) vec2.DistanceSquared(closestEnemyFormation.Formation.CachedMedianPosition.AsVec2);
//        vec2 = this.Formation.CachedAveragePosition;
//        double num3 = (double) vec2.DistanceSquared(closestEnemyFormation.Formation.CachedMedianPosition.AsVec2);
//        if (num2 > num3)
//        {
//          vec2 = this.Formation.CachedAveragePosition;
//          num1 = (double) vec2.DistanceSquared(this.CurrentOrder.GetPosition(this.Formation)) <= ((double) this._mainFormation.Depth + (double) this.Formation.Depth) * ((double) this._mainFormation.Depth + (double) this.Formation.Depth) * 0.25 ? 1 : 0;
//          goto label_4;
//        }
//      }
//      num1 = 1;
//label_4:
//      bool flag = num1 != 0;
//      if (flag != this._isFireAtWill)
//      {
//        this._isFireAtWill = flag;
//        this.Formation.SetFiringOrder(this._isFireAtWill ? FiringOrder.FiringOrderFireAtWill : FiringOrder.FiringOrderHoldYourFire);
//      }
//      if (this._mainFormation != null && (double) MathF.Abs(this._mainFormation.Width - this.Formation.Width) > 10.0)
//        this.Formation.SetFormOrder(FormOrder.FormOrderCustom(this._mainFormation.Width));
//      this.Formation.SetMovementOrder(this.CurrentOrder);
//      this.Formation.SetFacingOrder(this.CurrentFacingOrder);
//    }

//    protected override void OnBehaviorActivatedAux()
//    {
//      this.CalculateCurrentOrder();
//      this.Formation.SetMovementOrder(this.CurrentOrder);
//      this.Formation.SetFacingOrder(this.CurrentFacingOrder);
//      this.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);
//      this.Formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
//      this.Formation.SetFormOrder(FormOrder.FormOrderWide);
//    }

//    public override TextObject GetBehaviorString()
//    {
//      TextObject behaviorString = base.GetBehaviorString();
//      if (this._mainFormation != null)
//      {
//        behaviorString.SetTextVariable("AI_SIDE", GameTexts.FindText("str_formation_ai_side_strings", this._mainFormation.AI.Side.ToString()));
//        behaviorString.SetTextVariable("CLASS", GameTexts.FindText("str_formation_class_string", this._mainFormation.PhysicalClass.GetName()));
//      }
//      return behaviorString;
//    }

//    protected override float GetAiWeight()
//    {
//      // ISSUE: explicit reference operation
//      if (@this.CurrentOrder == MovementOrder.MovementOrderStop)
//        this.CalculateCurrentOrder();
//      if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
//        this._mainFormation = this.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0 && f.AI.IsMainFormation));
//      if (this._behaviorSide != this.Formation.AI.Side)
//        this._behaviorSide = this.Formation.AI.Side;
//      FormationQuerySystem closestEnemyFormation = this.Formation.CachedClosestEnemyFormation;
//      if (this._mainFormation == null || this.Formation.AI.IsMainFormation || closestEnemyFormation == null)
//        return 0.0f;
//      FormationQuerySystem querySystem = this.Formation.QuerySystem;
//      double num1 = (double) MBMath.Lerp(0.1f, 1f, MBMath.ClampFloat(querySystem.RangedUnitRatio + querySystem.RangedCavalryUnitRatio, 0.0f, 0.5f) * 2f);
//      Vec2 vec2 = this._mainFormation.Direction;
//      vec2 = vec2.Normalized();
//      float num2 = MBMath.LinearExtrapolation(0.5f, 1.1f, (float) (((double) vec2.DotProduct((closestEnemyFormation.Formation.CachedMedianPosition.AsVec2 - this._mainFormation.CachedMedianPosition.AsVec2).Normalized()) + 1.0) / 2.0));
//      float num3 = MBMath.Lerp(0.5f, 1.2f, (float) ((8.0 - (double) MBMath.ClampFloat(this.Formation.CachedAveragePosition.Distance(closestEnemyFormation.Formation.CachedMedianPosition.AsVec2) / closestEnemyFormation.MovementSpeedMaximum, 4f, 8f)) / 4.0));
//      double reliabilityFactor = (double) this.Formation.QuerySystem.MainFormationReliabilityFactor;
//      return (float) (num1 * reliabilityFactor) * num2 * num3;
//    }
//  }
//}
