//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.BehaviorSkirmish
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
//  public class BehaviorSkirmish : BehaviorComponent
//  {
//    private bool _cantShoot;
//    private float _cantShootDistance = float.MaxValue;
//    private bool _alternatePositionUsed;
//    private WorldPosition _alternatePosition = WorldPosition.Invalid;
//    private BehaviorSkirmish.BehaviorState _behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
//    private Timer _cantShootTimer;
//    private Timer _pullBackTimer;

//    public BehaviorSkirmish(Formation formation)
//      : base(formation)
//    {
//      this.BehaviorCoherence = 0.5f;
//      this._cantShootTimer = new Timer(0.0f, 0.0f);
//      this._pullBackTimer = new Timer(0.0f, 0.0f);
//      this.CalculateCurrentOrder();
//    }

//    protected override void CalculateCurrentOrder()
//    {
//      WorldPosition position = this.Formation.CachedMedianPosition;
//      bool flag1 = false;
//      Vec2 vec2;
//      if (this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null)
//      {
//        vec2 = this.Formation.Direction;
//        position.SetVec2(this.Formation.CachedAveragePosition);
//      }
//      else
//      {
//        vec2 = this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedMedianPosition.AsVec2 - this.Formation.CachedAveragePosition;
//        float num1 = vec2.Normalize();
//        float num2 = this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedCurrentVelocity.DotProduct(vec2);
//        float num3 = MBMath.Lerp(5f, 10f, (float) (((double) MBMath.ClampFloat((float) this.Formation.CountOfUnits, 10f, 60f) - 10.0) * 0.019999999552965164)) * num2;
//        float b = num1 + num3;
//        float num4 = MBMath.Lerp(0.1f, 0.33f, (float) (1.0 - (double) MBMath.ClampFloat((float) this.Formation.CountOfUnits, 1f, 50f) * 0.019999999552965164)) * this.Formation.QuerySystem.RangedUnitRatio;
//        switch (this._behaviorState)
//        {
//          case BehaviorSkirmish.BehaviorState.Approaching:
//            if ((double) b < (double) this._cantShootDistance * 0.800000011920929)
//            {
//              this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
//              this._cantShoot = false;
//              flag1 = true;
//              break;
//            }
//            if ((double) this.Formation.QuerySystem.MakingRangedAttackRatio >= (double) num4 * 1.2000000476837158)
//            {
//              this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
//              this._cantShoot = false;
//              flag1 = true;
//              break;
//            }
//            break;
//          case BehaviorSkirmish.BehaviorState.Shooting:
//            if ((double) this.Formation.QuerySystem.MakingRangedAttackRatio <= (double) num4)
//            {
//              if ((double) b > (double) this.Formation.QuerySystem.MaximumMissileRange)
//              {
//                this._behaviorState = BehaviorSkirmish.BehaviorState.Approaching;
//                this._cantShootDistance = MathF.Min(this._cantShootDistance, this.Formation.QuerySystem.MaximumMissileRange * 0.9f);
//                break;
//              }
//              if (!this._cantShoot)
//              {
//                this._cantShoot = true;
//                this._cantShootTimer.Reset(Mission.Current.CurrentTime, MBMath.Lerp(5f, 10f, (float) (((double) MBMath.ClampFloat((float) this.Formation.CountOfUnits, 10f, 60f) - 10.0) * 0.019999999552965164)));
//                break;
//              }
//              if (this._cantShootTimer.Check(Mission.Current.CurrentTime))
//              {
//                this._behaviorState = BehaviorSkirmish.BehaviorState.Approaching;
//                this._cantShootDistance = MathF.Min(this._cantShootDistance, b);
//                break;
//              }
//              break;
//            }
//            this._cantShootDistance = MathF.Max(this._cantShootDistance, b);
//            this._cantShoot = false;
//            if (this._pullBackTimer.Check(Mission.Current.CurrentTime) && this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsInfantryFormation && (double) b < (double) MathF.Min(this.Formation.QuerySystem.MissileRangeAdjusted * 0.4f, this._cantShootDistance * 0.666f))
//            {
//              this._behaviorState = BehaviorSkirmish.BehaviorState.PullingBack;
//              this._pullBackTimer.Reset(Mission.Current.CurrentTime, 10f);
//              break;
//            }
//            break;
//          case BehaviorSkirmish.BehaviorState.PullingBack:
//            if ((double) b > (double) MathF.Min(this._cantShootDistance, this.Formation.QuerySystem.MissileRangeAdjusted) * 0.800000011920929)
//            {
//              this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
//              this._cantShoot = false;
//              flag1 = true;
//              break;
//            }
//            if (this._pullBackTimer.Check(Mission.Current.CurrentTime) && (double) this.Formation.QuerySystem.MakingRangedAttackRatio <= (double) num4 * 0.5)
//            {
//              this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
//              this._cantShoot = false;
//              flag1 = true;
//              this._pullBackTimer.Reset(Mission.Current.CurrentTime, 5f);
//              break;
//            }
//            break;
//        }
//        switch (this._behaviorState)
//        {
//          case BehaviorSkirmish.BehaviorState.Approaching:
//            bool flag2 = false;
//            if (this._alternatePositionUsed)
//            {
//              float num5 = this.Formation.CachedAveragePosition.DistanceSquared(this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedAveragePosition);
//              bool flag3 = (double) this._alternatePosition.AsVec2.DistanceSquared((this.Formation.CachedAveragePosition + this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedAveragePosition) * 0.5f) > (double) num5 * (1.0 / 16.0);
//              if (!flag3)
//              {
//                int faceGroupId;
//                IntPtr navigationMeshForPosition = (IntPtr) Mission.Current.Scene.GetNavigationMeshForPosition(this._alternatePosition.GetNavMeshVec3(), out faceGroupId, 1.5f, false);
//                Agent medianAgent = this.Formation.GetMedianAgent(true, true, this.Formation.CachedAveragePosition);
//                flag3 = (medianAgent != null ? (medianAgent.GetCurrentNavigationFaceId() % 10 == 1 ? 1 : 0) : 0) != (faceGroupId % 10 == 1 ? 1 : 0);
//              }
//              if (flag3)
//              {
//                Agent medianAgent1 = this.Formation.GetMedianAgent(true, true, this.Formation.CachedAveragePosition);
//                int num6 = medianAgent1 != null ? (medianAgent1.GetCurrentNavigationFaceId() % 10 == 1 ? 1 : 0) : 0;
//                Agent medianAgent2 = this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.GetMedianAgent(true, true, this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedAveragePosition);
//                int num7 = medianAgent2 != null ? (medianAgent2.GetCurrentNavigationFaceId() % 10 == 1 ? 1 : 0) : 0;
//                if (num6 == num7)
//                {
//                  this._alternatePositionUsed = false;
//                  this._alternatePosition = WorldPosition.Invalid;
//                }
//                else
//                  flag2 = true;
//              }
//            }
//            else if (Mission.Current.MissionTeamAIType == Mission.MissionTeamAITypeEnum.Siege || Mission.Current.MissionTeamAIType == Mission.MissionTeamAITypeEnum.SallyOut)
//            {
//              Agent medianAgent3 = this.Formation.GetMedianAgent(true, true, this.Formation.CachedAveragePosition);
//              int num8 = medianAgent3 != null ? (medianAgent3.GetCurrentNavigationFaceId() % 10 == 1 ? 1 : 0) : 0;
//              Agent medianAgent4 = this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.GetMedianAgent(true, true, this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedAveragePosition);
//              int num9 = medianAgent4 != null ? (medianAgent4.GetCurrentNavigationFaceId() % 10 == 1 ? 1 : 0) : 0;
//              if (num8 != num9)
//              {
//                this._alternatePositionUsed = true;
//                flag2 = true;
//              }
//            }
//            if (this._alternatePositionUsed)
//            {
//              if (flag2)
//                this._alternatePosition = new WorldPosition(Mission.Current.Scene, new Vec3((this.Formation.CachedAveragePosition + this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedAveragePosition) * 0.5f, this.Formation.CachedMedianPosition.GetNavMeshZ()));
//              position = this._alternatePosition;
//              break;
//            }
//            position = this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedMedianPosition;
//            position.SetVec2(this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedAveragePosition);
//            break;
//          case BehaviorSkirmish.BehaviorState.Shooting:
//            position.SetVec2(this.Formation.CachedAveragePosition + this.Formation.CachedCurrentVelocity.Normalized() * (this.Formation.Depth * 0.5f));
//            break;
//          case BehaviorSkirmish.BehaviorState.PullingBack:
//            position = this.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.CachedMedianPosition;
//            position.SetVec2(position.AsVec2 - vec2 * (float) ((double) this.Formation.QuerySystem.MissileRangeAdjusted - (double) this.Formation.Depth * 0.5 - 10.0));
//            break;
//        }
//      }
//      if (((!this.CurrentOrder.GetPosition(this.Formation).IsValid ? 1 : (this._behaviorState != BehaviorSkirmish.BehaviorState.Shooting ? 1 : 0)) | (flag1 ? 1 : 0)) != 0)
//        this.CurrentOrder = MovementOrder.MovementOrderMove(position);
//      if (((!this.CurrentFacingOrder.GetDirection(this.Formation).IsValid ? 1 : (this._behaviorState != BehaviorSkirmish.BehaviorState.Shooting ? 1 : 0)) | (flag1 ? 1 : 0)) == 0)
//        return;
//      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec2);
//    }

//    public override void TickOccasionally()
//    {
//      this.CalculateCurrentOrder();
//      this.Formation.SetMovementOrder(this.CurrentOrder);
//      this.Formation.SetFacingOrder(this.CurrentFacingOrder);
//    }

//    protected override void OnBehaviorActivatedAux()
//    {
//      this._cantShoot = false;
//      this._cantShootDistance = float.MaxValue;
//      this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
//      this._cantShootTimer.Reset(Mission.Current.CurrentTime, MBMath.Lerp(5f, 10f, (float) (((double) MBMath.ClampFloat((float) this.Formation.CountOfUnits, 10f, 60f) - 10.0) * 0.019999999552965164)));
//      this._pullBackTimer.Reset(0.0f, 0.0f);
//      this.CalculateCurrentOrder();
//      this.Formation.SetMovementOrder(this.CurrentOrder);
//      this.Formation.SetFacingOrder(this.CurrentFacingOrder);
//      this.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLoose);
//      this.Formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
//      this.Formation.SetFormOrder(FormOrder.FormOrderWide);
//    }

//    protected override float GetAiWeight()
//    {
//      FormationQuerySystem querySystem = this.Formation.QuerySystem;
//      return MBMath.Lerp(0.1f, 1f, MBMath.ClampFloat(querySystem.RangedUnitRatio + querySystem.RangedCavalryUnitRatio, 0.0f, 0.5f) * 2f);
//    }

//    private enum BehaviorState
//    {
//      Approaching,
//      Shooting,
//      PullingBack,
//    }
//  }
//}
