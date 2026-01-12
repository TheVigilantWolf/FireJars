//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.BehaviorCharge
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System.Collections.Generic;
//using TaleWorlds.Engine;
//using TaleWorlds.Library;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public class BehaviorCharge : BehaviorComponent
//  {
//    public BehaviorCharge(Formation formation)
//      : base(formation)
//    {
//      this.CalculateCurrentOrder();
//      this.BehaviorCoherence = 0.5f;
//    }

//    protected override void CalculateCurrentOrder()
//    {
//      this.CurrentOrder = this.Formation.CachedClosestEnemyFormation == null ? MovementOrder.MovementOrderCharge : MovementOrder.MovementOrderChargeToTarget(this.Formation.CachedClosestEnemyFormation.Formation);
//    }

//    public override void TickOccasionally()
//    {
//      base.TickOccasionally();
//      if (this.Formation.Team.TeamAI is TeamAISiegeComponent teamAi && teamAi.OuterGate != null && !teamAi.OuterGate.IsGateOpen && teamAi.InnerGate != null && !teamAi.InnerGate.IsGateOpen)
//      {
//        CastleGate usable = teamAi.InnerGate ?? teamAi.OuterGate;
//        if (usable != null && !usable.IsUsedByFormation(this.Formation))
//          this.Formation.StartUsingMachine((UsableMachine) usable);
//      }
//      this.CalculateCurrentOrder();
//      this.Formation.SetMovementOrder(this.CurrentOrder);
//    }

//    public override float NavmeshlessTargetPositionPenalty => 1f;

//    protected override void OnBehaviorActivatedAux()
//    {
//      this.Formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
//      if (this.Formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.ShieldWall)
//        return;
//      this.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);
//    }

//    private float CalculateAIWeight(bool isSiege, bool isInsideCastle)
//    {
//      FormationQuerySystem querySystem = this.Formation.QuerySystem;
//      FormationQuerySystem closestEnemyFormation = this.Formation.CachedClosestEnemyFormation;
//      float num1 = this.Formation.CachedAveragePosition.Distance(closestEnemyFormation.Formation.CachedMedianPosition.AsVec2) / querySystem.MovementSpeedMaximum;
//      float num2 = querySystem.IsCavalryFormation || querySystem.IsRangedCavalryFormation ? ((double) num1 > 4.0 ? MBMath.Lerp(0.1f, 1.4f, (float) (1.0 - ((double) MBMath.ClampFloat(num1, 4f, 10f) - 4.0) / 6.0)) : MBMath.Lerp(0.1f, 1.4f, MBMath.ClampFloat(num1, 0.0f, 4f) / 4f)) : MBMath.Lerp(0.1f, 1f, (float) (1.0 - ((double) MBMath.ClampFloat(num1, 4f, 10f) - 4.0) / 6.0));
//      float num3 = 0.0f;
//      foreach (Team team1 in (List<Team>) Mission.Current.Teams)
//      {
//        if (team1.IsEnemyOf(this.Formation.Team))
//        {
//          foreach (Formation formation1 in (List<Formation>) team1.FormationsIncludingSpecialAndEmpty)
//          {
//            if (formation1.CountOfUnits > 0 && closestEnemyFormation.Formation != formation1 && (!isSiege || TeamAISiegeComponent.IsFormationInsideCastle(formation1, true) == isInsideCastle))
//            {
//              WorldPosition cachedMedianPosition = formation1.CachedMedianPosition;
//              Vec2 asVec2_1 = cachedMedianPosition.AsVec2;
//              ref Vec2 local = ref asVec2_1;
//              cachedMedianPosition = closestEnemyFormation.Formation.CachedMedianPosition;
//              Vec2 asVec2_2 = cachedMedianPosition.AsVec2;
//              float num4 = local.Distance(asVec2_2) / formation1.QuerySystem.MovementSpeedMaximum;
//              if ((double) num4 <= (double) num1 + 4.0 && ((double) num1 > 8.0 || formation1.CachedClosestEnemyFormation == this.Formation.QuerySystem))
//              {
//                bool flag = false;
//                if ((double) num1 <= 8.0)
//                {
//                  foreach (Team team2 in (List<Team>) this.Formation.Team.Mission.Teams)
//                  {
//                    if (team2.IsFriendOf(this.Formation.Team))
//                    {
//                      foreach (Formation formation2 in (List<Formation>) team2.FormationsIncludingSpecialAndEmpty)
//                      {
//                        if (formation2.CountOfUnits > 0 && formation2 != this.Formation && formation2.CachedClosestEnemyFormation == formation1.QuerySystem)
//                        {
//                          cachedMedianPosition = formation2.CachedMedianPosition;
//                          if ((double) cachedMedianPosition.AsVec2.DistanceSquared(this.Formation.CachedAveragePosition) / (double) formation2.QuerySystem.MovementSpeedMaximum < (double) num4 + 4.0)
//                          {
//                            flag = true;
//                            break;
//                          }
//                        }
//                      }
//                      if (flag)
//                        break;
//                    }
//                  }
//                }
//                if (!flag)
//                  num3 += formation1.QuerySystem.FormationMeleeFightingPower * formation1.QuerySystem.GetClassWeightedFactor(1f, 1f, 1f, 1f);
//              }
//            }
//          }
//        }
//      }
//      float num5 = 0.0f;
//      foreach (Team team in (List<Team>) Mission.Current.Teams)
//      {
//        if (team.IsFriendOf(this.Formation.Team))
//        {
//          foreach (Formation formation in (List<Formation>) team.FormationsIncludingSpecialAndEmpty)
//          {
//            if (formation != this.Formation && formation.CountOfUnits > 0 && closestEnemyFormation == formation.CachedClosestEnemyFormation && (!isSiege || TeamAISiegeComponent.IsFormationInsideCastle(formation, true) == isInsideCastle))
//            {
//              WorldPosition cachedMedianPosition = formation.CachedMedianPosition;
//              Vec2 asVec2_3 = cachedMedianPosition.AsVec2;
//              ref Vec2 local = ref asVec2_3;
//              cachedMedianPosition = formation.CachedClosestEnemyFormation.Formation.CachedMedianPosition;
//              Vec2 asVec2_4 = cachedMedianPosition.AsVec2;
//              if ((double) local.Distance(asVec2_4) / (double) formation.QuerySystem.MovementSpeedMaximum < 4.0)
//                num5 += formation.QuerySystem.FormationMeleeFightingPower * formation.QuerySystem.GetClassWeightedFactor(1f, 1f, 1f, 1f);
//            }
//          }
//        }
//      }
//      float num6 = (float) (((double) this.Formation.QuerySystem.FormationMeleeFightingPower * (double) querySystem.GetClassWeightedFactor(1f, 1f, 1f, 1f) + (double) num5 + 1.0) / (1.0 + (double) num3 + (double) closestEnemyFormation.Formation.QuerySystem.FormationMeleeFightingPower * (double) closestEnemyFormation.GetClassWeightedFactor(1f, 1f, 1f, 1f)) / (!isSiege ? (double) MBMath.ClampFloat(querySystem.Team.RemainingPowerRatio, 0.2f, 3f) : (double) MBMath.ClampFloat(querySystem.Team.RemainingPowerRatio, 0.5f, 3f)));
//      if ((double) num6 > 1.0)
//        num6 = (float) (((double) num6 - 1.0) / 3.0) + 1f;
//      float num7 = MBMath.ClampFloat(num6, 0.1f, 1.25f);
//      float num8 = 1f;
//      if ((double) num1 <= 4.0)
//      {
//        Vec2 cachedAveragePosition = this.Formation.CachedAveragePosition;
//        WorldPosition cachedMedianPosition1 = closestEnemyFormation.Formation.CachedMedianPosition;
//        Vec2 asVec2 = cachedMedianPosition1.AsVec2;
//        float length = (cachedAveragePosition - asVec2).Length;
//        if ((double) length > 1.4012984643248171E-45)
//        {
//          WorldPosition cachedMedianPosition2 = this.Formation.CachedMedianPosition;
//          cachedMedianPosition2.SetVec2(this.Formation.CachedAveragePosition);
//          float navMeshZ1 = cachedMedianPosition2.GetNavMeshZ();
//          if (!float.IsNaN(navMeshZ1))
//          {
//            double num9 = (double) navMeshZ1;
//            cachedMedianPosition1 = closestEnemyFormation.Formation.CachedMedianPosition;
//            double navMeshZ2 = (double) cachedMedianPosition1.GetNavMeshZ();
//            num8 = MBMath.Lerp(0.9f, 1.1f, (float) (((double) MBMath.ClampFloat((float) (num9 - navMeshZ2) / length, -0.58f, 0.58f) + 0.57999998331069946) / 1.1599999666213989));
//          }
//        }
//      }
//      float num10 = 1f;
//      if ((double) num1 <= 4.0 && (double) num1 >= 1.5)
//        num10 = 1.2f;
//      float num11 = 1f;
//      if ((double) num1 <= 4.0 && closestEnemyFormation.Formation.CachedClosestEnemyFormation != querySystem)
//        num11 = 1.2f;
//      float num12 = !isSiege ? querySystem.GetClassWeightedFactor(1f, 1f, 1.5f, 1.5f) * closestEnemyFormation.GetClassWeightedFactor(1f, 1f, 0.5f, 0.5f) : querySystem.GetClassWeightedFactor(1f, 1f, 1.2f, 1.2f) * closestEnemyFormation.GetClassWeightedFactor(1f, 1f, 0.3f, 0.3f);
//      return num2 * num7 * num8 * num10 * num11 * num12;
//    }

//    protected override float GetAiWeight()
//    {
//      bool isSiege = this.Formation.Team.TeamAI is TeamAISiegeComponent;
//      float aiWeight = 0.0f;
//      FormationQuerySystem closestEnemyFormation = this.Formation.CachedClosestEnemyFormation;
//      if (closestEnemyFormation == null)
//      {
//        if (this.Formation.Team.HasAnyEnemyTeamsWithAgents(false))
//          aiWeight = 0.2f;
//      }
//      else
//      {
//        bool isInsideCastle = false;
//        bool flag;
//        if (!isSiege)
//          flag = true;
//        else if ((this.Formation.Team.TeamAI as TeamAISiegeComponent).CalculateIsChargePastWallsApplicable(this.Formation.AI.Side))
//        {
//          flag = true;
//        }
//        else
//        {
//          isInsideCastle = TeamAISiegeComponent.IsFormationInsideCastle(closestEnemyFormation.Formation, true, 0.51f);
//          flag = isInsideCastle == TeamAISiegeComponent.IsFormationInsideCastle(this.Formation, true, isInsideCastle ? 0.9f : 0.1f);
//        }
//        if (flag)
//          aiWeight = this.CalculateAIWeight(isSiege, isInsideCastle);
//      }
//      return aiWeight;
//    }
//  }
//}
