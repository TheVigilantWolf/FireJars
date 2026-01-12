//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.MissionCombatantsLogic
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TaleWorlds.Core;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public class MissionCombatantsLogic : MissionLogic
//  {
//    protected readonly IEnumerable<IBattleCombatant> BattleCombatants;
//    protected readonly IBattleCombatant PlayerBattleCombatant;
//    protected readonly IBattleCombatant DefenderLeaderBattleCombatant;
//    protected readonly IBattleCombatant AttackerLeaderBattleCombatant;
//    protected readonly Mission.MissionTeamAITypeEnum TeamAIType;
//    protected readonly bool IsPlayerSergeant;

//    public BattleSideEnum PlayerSide
//    {
//      get
//      {
//        if (this.PlayerBattleCombatant == null)
//          return BattleSideEnum.None;
//        return this.PlayerBattleCombatant != this.DefenderLeaderBattleCombatant ? BattleSideEnum.Attacker : BattleSideEnum.Defender;
//      }
//    }

//    public MissionCombatantsLogic(
//      IEnumerable<IBattleCombatant> battleCombatants,
//      IBattleCombatant playerBattleCombatant,
//      IBattleCombatant defenderLeaderBattleCombatant,
//      IBattleCombatant attackerLeaderBattleCombatant,
//      Mission.MissionTeamAITypeEnum teamAIType,
//      bool isPlayerSergeant)
//    {
//      if (battleCombatants == null)
//        battleCombatants = (IEnumerable<IBattleCombatant>) new IBattleCombatant[2]
//        {
//          defenderLeaderBattleCombatant,
//          attackerLeaderBattleCombatant
//        };
//      this.BattleCombatants = battleCombatants;
//      this.PlayerBattleCombatant = playerBattleCombatant;
//      this.DefenderLeaderBattleCombatant = defenderLeaderBattleCombatant;
//      this.AttackerLeaderBattleCombatant = attackerLeaderBattleCombatant;
//      this.TeamAIType = teamAIType;
//      this.IsPlayerSergeant = isPlayerSergeant;
//    }

//    public Banner GetBannerForSide(BattleSideEnum side)
//    {
//      return side != BattleSideEnum.Defender ? this.AttackerLeaderBattleCombatant.Banner : this.DefenderLeaderBattleCombatant.Banner;
//    }

//    public override void OnBehaviorInitialize()
//    {
//      base.OnBehaviorInitialize();
//      if (!this.Mission.Teams.IsEmpty<Team>())
//        throw new MBIllegalValueException("Number of teams is not 0.");
//      BattleSideEnum playerSide = this.PlayerBattleCombatant.Side;
//      BattleSideEnum oppositeSide = playerSide.GetOppositeSide();
//      if (playerSide == BattleSideEnum.Defender)
//        this.AddPlayerTeam(playerSide);
//      else
//        this.AddEnemyTeam(oppositeSide);
//      if (playerSide == BattleSideEnum.Attacker)
//        this.AddPlayerTeam(playerSide);
//      else
//        this.AddEnemyTeam(oppositeSide);
//      IBattleCombatant allyCombatant;
//      if (!MissionCombatantsLogic.SupportsAllyTeamOnPlayerSide(this.BattleCombatants.Where<IBattleCombatant>((Func<IBattleCombatant, bool>) (cmbt => cmbt.Side == playerSide)), this.PlayerBattleCombatant, this.IsPlayerSergeant, out allyCombatant))
//        return;
//      this.AddPlayerAllyTeam(playerSide, allyCombatant);
//    }

//    public override void EarlyStart()
//    {
//      Mission.Current.MissionTeamAIType = this.TeamAIType;
//      switch (this.TeamAIType)
//      {
//        case Mission.MissionTeamAITypeEnum.FieldBattle:
//          using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Team current = enumerator.Current;
//              current.AddTeamAI((TeamAIComponent) new TeamAIGeneral(this.Mission, current));
//            }
//            break;
//          }
//        case Mission.MissionTeamAITypeEnum.Siege:
//          using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Team current = enumerator.Current;
//              if (current.Side == BattleSideEnum.Attacker)
//                current.AddTeamAI((TeamAIComponent) new TeamAISiegeAttacker(this.Mission, current, 5f, 1f));
//              if (current.Side == BattleSideEnum.Defender)
//                current.AddTeamAI((TeamAIComponent) new TeamAISiegeDefender(this.Mission, current, 5f, 1f));
//            }
//            break;
//          }
//        case Mission.MissionTeamAITypeEnum.SallyOut:
//          using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Team current = enumerator.Current;
//              if (current.Side == BattleSideEnum.Attacker)
//                current.AddTeamAI((TeamAIComponent) new TeamAISallyOutDefender(this.Mission, current, 5f, 1f));
//              else
//                current.AddTeamAI((TeamAIComponent) new TeamAISallyOutAttacker(this.Mission, current, 5f, 1f));
//            }
//            break;
//          }
//      }
//      if (Mission.Current.Teams.Count <= 0)
//        return;
//      switch (Mission.Current.MissionTeamAIType)
//      {
//        case Mission.MissionTeamAITypeEnum.NoTeamAI:
//          using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Team current = enumerator.Current;
//              if (current.HasTeamAi)
//                current.AddTacticOption((TacticComponent) new TacticCharge(current));
//            }
//            break;
//          }
//        case Mission.MissionTeamAITypeEnum.FieldBattle:
//          using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Team team = enumerator.Current;
//              if (team.HasTeamAi)
//              {
//                int num = this.BattleCombatants.Where<IBattleCombatant>((Func<IBattleCombatant, bool>) (bc => bc.Side == team.Side)).Max<IBattleCombatant>((Func<IBattleCombatant, int>) (bcs => bcs.GetTacticsSkillAmount()));
//                team.AddTacticOption((TacticComponent) new TacticCharge(team));
//                if ((double) num >= 20.0)
//                {
//                  team.AddTacticOption((TacticComponent) new TacticFullScaleAttack(team));
//                  if (team.Side == BattleSideEnum.Defender)
//                  {
//                    team.AddTacticOption((TacticComponent) new TacticDefensiveEngagement(team));
//                    team.AddTacticOption((TacticComponent) new TacticDefensiveLine(team));
//                  }
//                  if (team.Side == BattleSideEnum.Attacker)
//                    team.AddTacticOption((TacticComponent) new TacticRangedHarrassmentOffensive(team));
//                }
//                if ((double) num >= 50.0)
//                {
//                  team.AddTacticOption((TacticComponent) new TacticFrontalCavalryCharge(team));
//                  if (team.Side == BattleSideEnum.Defender)
//                  {
//                    team.AddTacticOption((TacticComponent) new TacticDefensiveRing(team));
//                    team.AddTacticOption((TacticComponent) new TacticHoldChokePoint(team));
//                  }
//                  if (team.Side == BattleSideEnum.Attacker)
//                    team.AddTacticOption((TacticComponent) new TacticCoordinatedRetreat(team));
//                }
//              }
//            }
//            break;
//          }
//        case Mission.MissionTeamAITypeEnum.Siege:
//          using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Team current = enumerator.Current;
//              if (current.HasTeamAi)
//              {
//                if (current.Side == BattleSideEnum.Attacker)
//                  current.AddTacticOption((TacticComponent) new TacticBreachWalls(current));
//                if (current.Side == BattleSideEnum.Defender)
//                  current.AddTacticOption((TacticComponent) new TacticDefendCastle(current));
//              }
//            }
//            break;
//          }
//        case Mission.MissionTeamAITypeEnum.SallyOut:
//          using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Team current = enumerator.Current;
//              if (current.HasTeamAi)
//              {
//                if (current.Side == BattleSideEnum.Defender)
//                  current.AddTacticOption((TacticComponent) new TacticSallyOutHitAndRun(current));
//                if (current.Side == BattleSideEnum.Attacker)
//                  current.AddTacticOption((TacticComponent) new TacticSallyOutDefense(current));
//                current.AddTacticOption((TacticComponent) new TacticCharge(current));
//              }
//            }
//            break;
//          }
//      }
//      foreach (Team team in (List<Team>) this.Mission.Teams)
//      {
//        team.QuerySystem.Expire();
//        team.ResetTactic();
//      }
//    }

//    public override void AfterStart() => this.Mission.SetMissionMode(MissionMode.Battle, true);

//    public IEnumerable<IBattleCombatant> GetAllCombatants()
//    {
//      foreach (IBattleCombatant battleCombatant in this.BattleCombatants)
//        yield return battleCombatant;
//    }

//    protected void AddPlayerTeam(BattleSideEnum playerSide)
//    {
//      this.Mission.Teams.Add(playerSide, this.PlayerBattleCombatant.PrimaryColorPair.Item1, this.PlayerBattleCombatant.PrimaryColorPair.Item2, this.PlayerBattleCombatant.Banner);
//      this.Mission.PlayerTeam = playerSide == BattleSideEnum.Attacker ? this.Mission.AttackerTeam : this.Mission.DefenderTeam;
//    }

//    protected void AddEnemyTeam(BattleSideEnum enemySide)
//    {
//      IBattleCombatant battleCombatant = enemySide == BattleSideEnum.Attacker ? this.AttackerLeaderBattleCombatant : this.DefenderLeaderBattleCombatant;
//      this.Mission.Teams.Add(enemySide, battleCombatant.PrimaryColorPair.Item1, battleCombatant.PrimaryColorPair.Item2, battleCombatant.Banner);
//    }

//    protected void AddPlayerAllyTeam(BattleSideEnum playerSide, IBattleCombatant allyCombatant)
//    {
//      this.Mission.Teams.Add(playerSide, allyCombatant.PrimaryColorPair.Item1, allyCombatant.PrimaryColorPair.Item2, allyCombatant.Banner);
//      if (playerSide != BattleSideEnum.Attacker)
//      {
//        Team defenderAllyTeam = this.Mission.DefenderAllyTeam;
//      }
//      else
//      {
//        Team attackerAllyTeam = this.Mission.AttackerAllyTeam;
//      }
//    }

//    public static bool SupportsAllyTeamOnPlayerSide(
//      IEnumerable<IBattleCombatant> playerSideBattleCombatants,
//      IBattleCombatant playerBattleCombatant,
//      bool isPlayerSergeant,
//      out IBattleCombatant allyCombatant)
//    {
//      allyCombatant = (IBattleCombatant) null;
//      foreach (IBattleCombatant sideBattleCombatant in playerSideBattleCombatants)
//      {
//        if (sideBattleCombatant != playerBattleCombatant && sideBattleCombatant.Side == playerBattleCombatant.Side && !isPlayerSergeant)
//        {
//          allyCombatant = sideBattleCombatant;
//          return true;
//        }
//      }
//      return false;
//    }
//  }
//}
