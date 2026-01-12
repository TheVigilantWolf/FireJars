//// Decompiled with JetBrains decompiler
//// Type: SandBox.Missions.MissionLogics.BattleAgentLogic
//// Assembly: SandBox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: EF5F436B-43E1-4A13-9290-BD239AA7BA26
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\SandBox.dll

//using TaleWorlds.CampaignSystem;
//using TaleWorlds.CampaignSystem.CharacterDevelopment;
//using TaleWorlds.CampaignSystem.ComponentInterfaces;
//using TaleWorlds.CampaignSystem.MapEvents;
//using TaleWorlds.CampaignSystem.Party;
//using TaleWorlds.Core;
//using TaleWorlds.MountAndBlade;

//#nullable disable
//namespace SandBox.Missions.MissionLogics
//{
//  public class BattleAgentLogic : MissionLogic
//  {
//    private BattleObserverMissionLogic _battleObserverMissionLogic;
//    private const float XpShareForKill = 0.5f;
//    private const float XpShareForDamage = 0.5f;
//    private MissionTime _nextMoraleCheckTime;

//    private TroopUpgradeTracker _troopUpgradeTracker => MapEvent.PlayerMapEvent.TroopUpgradeTracker;

//    public override void AfterStart()
//    {
//      this._battleObserverMissionLogic = Mission.Current.GetMissionBehavior<BattleObserverMissionLogic>();
//      this.CheckPerkEffectsOnTeams();
//    }

//    public override void OnAgentBuild(Agent agent, Banner banner)
//    {
//      if (this._battleObserverMissionLogic == null || agent.Character == null || agent.Origin == null)
//        return;
//      PartyBase battleCombatant = (PartyBase) agent.Origin.BattleCombatant;
//      CharacterObject character = (CharacterObject) agent.Character;
//      if (battleCombatant == null)
//        return;
//      this._troopUpgradeTracker?.AddTrackedTroop(battleCombatant, character);
//    }

//    public override void OnAgentHit(
//      Agent affectedAgent,
//      Agent affectorAgent,
//      in MissionWeapon attackerWeapon,
//      in Blow blow,
//      in AttackCollisionData attackCollisionData)
//    {
//      if (affectedAgent.Character == null || affectorAgent == null || affectorAgent.Character == null || affectedAgent.State != AgentState.Active || affectorAgent == null)
//        return;
//      bool isFatal = (double) affectedAgent.Health - (double) blow.InflictedDamage < 1.0;
//      bool isTeamKill = false;
//      if (affectedAgent.Team != null && affectorAgent.Team != null)
//        isTeamKill = affectedAgent.Team.Side == affectorAgent.Team.Side;
//      affectorAgent.Origin.OnScoreHit(affectedAgent.Character, affectorAgent.Formation?.Captain?.Character, blow.InflictedDamage, isFatal, isTeamKill, attackerWeapon.CurrentUsageItem);
//    }

//    public override void OnAgentTeamChanged(Team prevTeam, Team newTeam, Agent agent)
//    {
//      if (prevTeam == null || prevTeam == Team.Invalid || newTeam == null || prevTeam == newTeam)
//        return;
//      // ISSUE: explicit non-virtual call
//      // ISSUE: explicit non-virtual call
//      this._battleObserverMissionLogic?.BattleObserver?.TroopSideChanged(prevTeam != null ? __nonvirtual (prevTeam.Side) : BattleSideEnum.None, newTeam != null ? __nonvirtual (newTeam.Side) : BattleSideEnum.None, agent.Origin.BattleCombatant, agent.Character);
//    }

//    public override void OnScoreHit(
//      Agent affectedAgent,
//      Agent affectorAgent,
//      WeaponComponentData attackerWeapon,
//      bool isBlocked,
//      bool isSiegeEngineHit,
//      in Blow blow,
//      in AttackCollisionData collisionData,
//      float damagedHp,
//      float hitDistance,
//      float shotDifficulty)
//    {
//      if (affectorAgent == null)
//        return;
//      if (affectorAgent.IsMount && affectorAgent.RiderAgent != null)
//        affectorAgent = affectorAgent.RiderAgent;
//      if (affectorAgent.Character == null || affectedAgent.Character == null)
//        return;
//      float damageAmount = (float) blow.InflictedDamage;
//      if ((double) damageAmount > (double) affectedAgent.HealthLimit)
//        damageAmount = affectedAgent.HealthLimit;
//      float num = damageAmount / affectedAgent.HealthLimit;
//      this.EnemyHitReward(affectedAgent, affectorAgent, blow.MovementSpeedDamageModifier, shotDifficulty, isSiegeEngineHit, attackerWeapon, blow.AttackType, 0.5f * num, damageAmount, collisionData.IsSneakAttack);
//    }

//    public override void OnAgentRemoved(
//      Agent affectedAgent,
//      Agent affectorAgent,
//      AgentState agentState,
//      KillingBlow killingBlow)
//    {
//      if (affectorAgent == null && affectedAgent.IsMount && agentState == AgentState.Routed)
//        return;
//      CharacterObject character1 = (CharacterObject) affectedAgent.Character;
//      CharacterObject character2 = (CharacterObject) affectorAgent?.Character;
//      if (affectedAgent.Origin == null)
//        return;
//      PartyBase battleCombatant = (PartyBase) affectedAgent.Origin.BattleCombatant;
//      switch (agentState)
//      {
//        case AgentState.Unconscious:
//          affectedAgent.Origin.SetWounded();
//          break;
//        case AgentState.Killed:
//          affectedAgent.Origin.SetKilled();
//          Hero heroObject1 = affectedAgent.IsHuman ? character1.HeroObject : (Hero) null;
//          Hero heroObject2 = affectorAgent == null ? (Hero) null : (affectorAgent.IsHuman ? character2.HeroObject : (Hero) null);
//          if (heroObject1 != null && heroObject2 != null)
//            CampaignEventDispatcher.Instance.OnCharacterDefeated(heroObject2, heroObject1);
//          if (battleCombatant == null)
//            break;
//          this.CheckUpgrade(affectedAgent.Team.Side, battleCombatant, character1);
//          break;
//        default:
//          bool flag = (double) affectedAgent.GetMorale() < 0.0099999997764825821;
//          affectedAgent.Origin.SetRouted(!flag);
//          break;
//      }
//    }

//    public override void OnAgentFleeing(Agent affectedAgent)
//    {
//    }

//    public override void OnMissionTick(float dt)
//    {
//      this.UpdateMorale();
//      if (!this._nextMoraleCheckTime.IsPast)
//        return;
//      this._nextMoraleCheckTime = MissionTime.SecondsFromNow(10f);
//    }

//    private void CheckPerkEffectsOnTeams()
//    {
//    }

//    private void UpdateMorale()
//    {
//    }

//    private void EnemyHitReward(
//      Agent affectedAgent,
//      Agent affectorAgent,
//      float lastSpeedBonus,
//      float lastShotDifficulty,
//      bool isSiegeEngineHit,
//      WeaponComponentData lastAttackerWeapon,
//      AgentAttackType attackType,
//      float hitpointRatio,
//      float damageAmount,
//      bool isSneakAttack)
//    {
//      CharacterObject character1 = (CharacterObject) affectedAgent.Character;
//      CharacterObject character2 = (CharacterObject) affectorAgent.Character;
//      if (affectedAgent.Origin == null || affectorAgent == null || affectorAgent.Origin == null || affectorAgent.Team == null || !affectorAgent.Team.IsValid || affectedAgent.Team == null || !affectedAgent.Team.IsValid)
//        return;
//      PartyBase battleCombatant = (PartyBase) affectorAgent.Origin.BattleCombatant;
//      Hero captain = BattleAgentLogic.GetCaptain(affectorAgent);
//      Hero heroObject = affectorAgent.Team.Leader == null || !affectorAgent.Team.Leader.Character.IsHero ? (Hero) null : ((CharacterObject) affectorAgent.Team.Leader.Character).HeroObject;
//      bool isTeamKill = affectorAgent.Team.Side == affectedAgent.Team.Side;
//      bool isAffectorMounted = affectorAgent.MountAgent != null;
//      bool isHorseCharge = isAffectorMounted && attackType == AgentAttackType.Collision;
//      SkillLevelingManager.OnCombatHit(character2, character1, captain?.CharacterObject, heroObject, lastSpeedBonus, lastShotDifficulty, lastAttackerWeapon, hitpointRatio, CombatXpModel.MissionTypeEnum.Battle, isAffectorMounted, isTeamKill, heroObject != null && affectorAgent.Character != heroObject.CharacterObject && (heroObject != Hero.MainHero || affectorAgent.Formation == null || !affectorAgent.Formation.IsAIControlled), damageAmount, (double) affectedAgent.Health < 1.0, isSiegeEngineHit, isHorseCharge, isSneakAttack);
//      if (this._battleObserverMissionLogic?.BattleObserver == null || affectorAgent.Character == null)
//        return;
//      if (affectorAgent.Character.IsHero)
//      {
//        foreach (SkillObject checkSkillUpgrade in this._troopUpgradeTracker.CheckSkillUpgrades(character2.HeroObject))
//          this._battleObserverMissionLogic.BattleObserver.HeroSkillIncreased(affectorAgent.Team.Side, (IBattleCombatant) battleCombatant, (BasicCharacterObject) character2, checkSkillUpgrade);
//      }
//      else
//        this.CheckUpgrade(affectorAgent.Team.Side, battleCombatant, character2);
//    }

//    private static Hero GetCaptain(Agent affectorAgent)
//    {
//      Hero captain1 = (Hero) null;
//      if (affectorAgent.Formation != null)
//      {
//        Agent captain2 = affectorAgent.Formation.Captain;
//        if (captain2 != null)
//        {
//          float captainRadius = Campaign.Current.Models.CombatXpModel.CaptainRadius;
//          if ((double) captain2.Position.Distance(affectorAgent.Position) < (double) captainRadius)
//            captain1 = ((CharacterObject) captain2.Character).HeroObject;
//        }
//      }
//      return captain1;
//    }

//    private void CheckUpgrade(BattleSideEnum side, PartyBase party, CharacterObject character)
//    {
//      if (this._battleObserverMissionLogic?.BattleObserver == null)
//        return;
//      int numberReadyToUpgrade = this._troopUpgradeTracker.CheckUpgradedCount(party, character);
//      if (numberReadyToUpgrade == 0)
//        return;
//      this._battleObserverMissionLogic.BattleObserver.TroopNumberChanged(side, (IBattleCombatant) party, (BasicCharacterObject) character, numberReadyToUpgrade: numberReadyToUpgrade);
//    }
//  }
//}
