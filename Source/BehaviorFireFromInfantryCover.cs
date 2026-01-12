//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.BehaviorFireFromInfantryCover
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System;
//using TaleWorlds.Engine;
//using TaleWorlds.Library;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public class BehaviorFireFromInfantryCover : BehaviorComponent
//  {
//    private Formation _mainFormation;
//    private bool _isFireAtWill = true;

//    public BehaviorFireFromInfantryCover(Formation formation)
//      : base(formation)
//    {
//      this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0 && f.AI.IsMainFormation));
//      this.CalculateCurrentOrder();
//    }

//    protected override void CalculateCurrentOrder()
//    {
//      WorldPosition cachedMedianPosition = this.Formation.CachedMedianPosition;
//      Vec2 direction = this.Formation.Direction;
//      if (this._mainFormation == null)
//      {
//        cachedMedianPosition.SetVec2(this.Formation.CachedAveragePosition);
//      }
//      else
//      {
//        Vec2 position = this._mainFormation.GetReadonlyMovementOrderReference().GetPosition(this._mainFormation);
//        if (position.IsValid)
//        {
//          direction = (position - this._mainFormation.CachedAveragePosition).Normalized();
//          Vec2 vec2 = position - direction * this._mainFormation.Depth * 0.33f;
//          cachedMedianPosition.SetVec2(vec2);
//        }
//        else
//          cachedMedianPosition.SetVec2(this.Formation.CachedAveragePosition);
//      }
//      this.CurrentOrder = MovementOrder.MovementOrderMove(cachedMedianPosition);
//      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
//    }

//    public override void TickOccasionally()
//    {
//      this.CalculateCurrentOrder();
//      this.Formation.SetMovementOrder(this.CurrentOrder);
//      this.Formation.SetFacingOrder(this.CurrentFacingOrder);
//      if ((double) this.Formation.CachedAveragePosition.DistanceSquared(this.CurrentOrder.GetPosition(this.Formation)) < 100.0)
//        this.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderSquare);
//      Vec2 position = this.CurrentOrder.GetPosition(this.Formation);
//      bool flag = this.Formation.CachedClosestEnemyFormation == null || (double) this._mainFormation.CachedAveragePosition.DistanceSquared(this.Formation.CachedAveragePosition) <= (double) this.Formation.Depth * (double) this.Formation.Width || (double) this.Formation.CachedAveragePosition.DistanceSquared(position) <= ((double) this._mainFormation.Depth + (double) this.Formation.Depth) * ((double) this._mainFormation.Depth + (double) this.Formation.Depth) * 0.25;
//      if (flag == this._isFireAtWill)
//        return;
//      this._isFireAtWill = flag;
//      this.Formation.SetFiringOrder(this._isFireAtWill ? FiringOrder.FiringOrderFireAtWill : FiringOrder.FiringOrderHoldYourFire);
//    }

//    protected override void OnBehaviorActivatedAux()
//    {
//      this.CalculateCurrentOrder();
//      this.Formation.SetMovementOrder(this.CurrentOrder);
//      this.Formation.SetFacingOrder(this.CurrentFacingOrder);
//      this.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);
//      this.Formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
//      int num = (int) MathF.Sqrt((float) this.Formation.CountOfUnits);
//      this.Formation.SetFormOrder(FormOrder.FormOrderCustom((float) ((double) num * (double) this.Formation.UnitDiameter + (double) (num - 1) * (double) this.Formation.Interval)));
//    }

//    protected override float GetAiWeight()
//    {
//      if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
//        this._mainFormation = this.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0 && f.AI.IsMainFormation));
//      return this._mainFormation == null || this.Formation.AI.IsMainFormation || this.Formation.CachedClosestEnemyFormation == null || !this.Formation.QuerySystem.IsRangedFormation ? 0.0f : 2f;
//    }
//  }
//}
