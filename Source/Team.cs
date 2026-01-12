//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.Team
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using NetworkMessages.FromServer;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using TaleWorlds.Core;
//using TaleWorlds.Engine;
//using TaleWorlds.Library;
//using TaleWorlds.MountAndBlade.Network.Messages;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public class Team : IMissionTeam
//  {
//    public readonly MBTeam MBTeam;
//    private List<OrderController> _orderControllers;
//    private MBList<Agent> _activeAgents;
//    private MBList<Agent> _teamAgents;
//    private bool _tickDetachments = true;
//    private MBList<(float, WorldPosition, int, Vec2, Vec2, bool)> _cachedEnemyDataForFleeing;
//    private bool _alreadyHasCustomOrderController;
//    private static Team _invalid;

//    public event Action<Team, Formation> OnFormationsChanged;

//    public event OnOrderIssuedDelegate OnOrderIssued;

//    public event Action<Formation> OnFormationAIActiveBehaviorChanged;

//    public BattleSideEnum Side { get; }

//    public Mission Mission { get; }

//    public MBList<Formation> FormationsIncludingEmpty { get; private set; }

//    public MBList<Formation> FormationsIncludingSpecialAndEmpty { get; private set; }

//    public TeamAIComponent TeamAI { get; private set; }

//    public bool IsPlayerTeam => this.Mission != null && this.Mission.PlayerTeam == this;

//    public bool IsPlayerAlly
//    {
//      get
//      {
//        return this.Mission != null && this.Mission.PlayerTeam != null && this.Mission.PlayerTeam.Side == this.Side;
//      }
//    }

//    public TeamSideEnum TeamSide
//    {
//      get
//      {
//        if (this.IsPlayerTeam)
//          return TeamSideEnum.PlayerTeam;
//        return !this.IsPlayerAlly ? TeamSideEnum.EnemyTeam : TeamSideEnum.PlayerAllyTeam;
//      }
//    }

//    public bool IsDefender => this.Side == BattleSideEnum.Defender;

//    public bool IsAttacker => this.Side == BattleSideEnum.Attacker;

//    public uint Color { get; private set; }

//    public uint Color2 { get; private set; }

//    public Banner Banner { get; }

//    public OrderController MasterOrderController => this._orderControllers[0];

//    public OrderController PlayerOrderController => this._orderControllers[1];

//    public TeamQuerySystem QuerySystem { get; private set; }

//    public DetachmentManager DetachmentManager { get; private set; }

//    public bool IsPlayerGeneral { get; private set; }

//    public bool IsPlayerSergeant { get; private set; }

//    public MBReadOnlyList<Agent> ActiveAgents => (MBReadOnlyList<Agent>) this._activeAgents;

//    public MBReadOnlyList<Agent> TeamAgents => (MBReadOnlyList<Agent>) this._teamAgents;

//    public MBReadOnlyList<(float, WorldPosition, int, Vec2, Vec2, bool)> CachedEnemyDataForFleeing
//    {
//      get
//      {
//        return (MBReadOnlyList<(float, WorldPosition, int, Vec2, Vec2, bool)>) this._cachedEnemyDataForFleeing;
//      }
//    }

//    public int TeamIndex => this.MBTeam.Index;

//    public Team(
//      MBTeam mbTeam,
//      BattleSideEnum side,
//      Mission mission,
//      uint color = 4294967295,
//      uint color2 = 4294967295,
//      Banner banner = null)
//    {
//      this.MBTeam = mbTeam;
//      this.Side = side;
//      this.Mission = mission;
//      this.Color = color;
//      this.Color2 = color2;
//      this.Banner = banner;
//      this.IsPlayerGeneral = true;
//      this.IsPlayerSergeant = false;
//      if (this != Team._invalid)
//        this.Initialize();
//      this.MoraleChangeFactor = 1f;
//    }

//    public void SetCustomOrderController(
//      OrderController customMasterOrderController,
//      OrderController customPlayerOrderController)
//    {
//      this._alreadyHasCustomOrderController = true;
//      OrderController orderController1 = this._orderControllers.Count > 0 ? this._orderControllers[0] : (OrderController) null;
//      OrderController orderController2 = this._orderControllers.Count > 1 ? this._orderControllers[1] : (OrderController) null;
//      this._orderControllers.Clear();
//      this._orderControllers.Add(customMasterOrderController);
//      this._orderControllers.Add(customPlayerOrderController);
//      orderController1?.AssignDelegatesToController(customMasterOrderController);
//      orderController2?.AssignDelegatesToController(customPlayerOrderController);
//    }

//    public void UpdateCachedEnemyDataForFleeing()
//    {
//      if (!this._cachedEnemyDataForFleeing.IsEmpty<(float, WorldPosition, int, Vec2, Vec2, bool)>())
//        return;
//      foreach (Team team in (List<Team>) this.Mission.Teams)
//      {
//        if (team.IsEnemyOf(this))
//        {
//          foreach (Formation formation in (List<Formation>) team.FormationsIncludingSpecialAndEmpty)
//          {
//            int countOfUnits = formation.CountOfUnits;
//            if (countOfUnits > 0)
//            {
//              WorldPosition cachedMedianPosition = formation.CachedMedianPosition;
//              float movementSpeedMaximum = formation.QuerySystem.MovementSpeedMaximum;
//              bool flag = (formation.QuerySystem.IsCavalryFormation || formation.QuerySystem.IsRangedCavalryFormation) && formation.HasAnyMountedUnit;
//              if (countOfUnits == 1)
//              {
//                Vec2 asVec2 = cachedMedianPosition.AsVec2;
//                this._cachedEnemyDataForFleeing.Add((movementSpeedMaximum, cachedMedianPosition, countOfUnits, asVec2, asVec2, flag));
//              }
//              else
//              {
//                Vec2 vec2_1 = formation.QuerySystem.EstimatedDirection.LeftVec();
//                float num = formation.Width / 2f;
//                Vec2 vec2_2 = cachedMedianPosition.AsVec2 - vec2_1 * num;
//                Vec2 vec2_3 = cachedMedianPosition.AsVec2 + vec2_1 * num;
//                this._cachedEnemyDataForFleeing.Add((movementSpeedMaximum, cachedMedianPosition, countOfUnits, vec2_2, vec2_3, flag));
//              }
//            }
//          }
//        }
//      }
//    }

//    public float MoraleChangeFactor { get; private set; }

//    private void Initialize()
//    {
//      this._activeAgents = new MBList<Agent>();
//      this._teamAgents = new MBList<Agent>();
//      this._cachedEnemyDataForFleeing = new MBList<(float, WorldPosition, int, Vec2, Vec2, bool)>();
//      if (GameNetwork.IsReplay)
//        return;
//      this.FormationsIncludingSpecialAndEmpty = new MBList<Formation>(10);
//      this.FormationsIncludingEmpty = new MBList<Formation>(8);
//      for (int index = 0; index < 10; ++index)
//      {
//        Formation formation = new Formation(this, index);
//        this.FormationsIncludingSpecialAndEmpty.Add(formation);
//        if (index < 8)
//          this.FormationsIncludingEmpty.Add(formation);
//        formation.AI.OnActiveBehaviorChanged += new Action<Formation>(this.FormationAI_OnActiveBehaviorChanged);
//      }
//      if (this.Mission != null)
//      {
//        this._orderControllers = new List<OrderController>();
//        OrderController orderController1 = new OrderController(this.Mission, this, (Agent) null);
//        this._orderControllers.Add(orderController1);
//        orderController1.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
//        OrderController orderController2 = new OrderController(this.Mission, this, (Agent) null);
//        this._orderControllers.Add(orderController2);
//        orderController2.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
//      }
//      this.QuerySystem = new TeamQuerySystem(this);
//      this.DetachmentManager = new DetachmentManager(this);
//    }

//    public void Reset()
//    {
//      if (!GameNetwork.IsReplay)
//      {
//        foreach (Formation formation in (List<Formation>) this.FormationsIncludingSpecialAndEmpty)
//          formation.Reset();
//        List<OrderController> orderControllers = this._orderControllers;
//        // ISSUE: explicit non-virtual call
//        if ((orderControllers != null ? (__nonvirtual (orderControllers.Count) > 2 ? 1 : 0) : 0) != 0)
//        {
//          for (int index = this._orderControllers.Count - 1; index >= 2; --index)
//          {
//            this._orderControllers[index].OnOrderIssued -= new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
//            this._orderControllers.RemoveAt(index);
//          }
//        }
//        this.QuerySystem = new TeamQuerySystem(this);
//      }
//      this._teamAgents.Clear();
//      this._activeAgents.Clear();
//    }

//    public void Clear()
//    {
//      if (!GameNetwork.IsReplay)
//      {
//        foreach (Formation formation in (List<Formation>) this.FormationsIncludingSpecialAndEmpty)
//          formation.AI.OnActiveBehaviorChanged -= new Action<Formation>(this.FormationAI_OnActiveBehaviorChanged);
//      }
//      this.Reset();
//    }

//    private void OrderController_OnOrderIssued(
//      OrderType orderType,
//      MBReadOnlyList<Formation> appliedFormations,
//      OrderController orderController,
//      params object[] delegateParams)
//    {
//      OnOrderIssuedDelegate onOrderIssued = this.OnOrderIssued;
//      if (onOrderIssued == null)
//        return;
//      onOrderIssued(orderType, appliedFormations, orderController, delegateParams);
//    }

//    public static bool DoesFirstFormationClassContainSecond(FormationClass f1, FormationClass f2)
//    {
//      return (f1 & f2) == f2;
//    }

//    public static FormationClass GetFormationFormationClass(Formation f)
//    {
//      if (f.QuerySystem.IsRangedCavalryFormation)
//        return FormationClass.HorseArcher;
//      if (f.QuerySystem.IsCavalryFormation)
//        return FormationClass.Cavalry;
//      return !f.QuerySystem.IsRangedFormation ? FormationClass.Infantry : FormationClass.Ranged;
//    }

//    public static FormationClass GetPlayerTeamFormationClass(Agent mainAgent)
//    {
//      if (mainAgent.IsRangedCached && mainAgent.HasMount)
//        return FormationClass.HorseArcher;
//      if (mainAgent.IsRangedCached)
//        return FormationClass.Ranged;
//      return !mainAgent.HasMount ? FormationClass.Infantry : FormationClass.Cavalry;
//    }

//    public void AssignPlayerAsSergeantOfFormation(MissionPeer peer, FormationClass formationClass)
//    {
//      Formation formation = this.GetFormation(formationClass);
//      formation.PlayerOwner = peer.ControlledAgent;
//      formation.BannerCode = peer.Peer.BannerCode;
//      if (peer.IsMine)
//        this.PlayerOrderController.Owner = peer.ControlledAgent;
//      else
//        this.GetOrderControllerOf(peer.ControlledAgent).Owner = peer.ControlledAgent;
//      formation.SetControlledByAI(false);
//      foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
//        missionBehavior.OnAssignPlayerAsSergeantOfFormation(peer.ControlledAgent);
//      if (peer.IsMine)
//        this.PlayerOrderController.SelectAllFormations();
//      peer.ControlledFormation = formation;
//      if (!GameNetwork.IsServer)
//        return;
//      peer.ControlledAgent.ForceUpdateCachedAndFormationValues(false, false);
//      if (peer.IsMine)
//        return;
//      GameNetwork.BeginModuleEventAsServer(peer.GetNetworkPeer());
//      GameNetwork.WriteMessage((GameNetworkMessage) new AssignFormationToPlayer(peer.GetNetworkPeer(), formationClass));
//      GameNetwork.EndModuleEventAsServer();
//    }

//    private void FormationAI_OnActiveBehaviorChanged(Formation formation)
//    {
//      if (formation.CountOfUnits <= 0)
//        return;
//      Action<Formation> activeBehaviorChanged = this.OnFormationAIActiveBehaviorChanged;
//      if (activeBehaviorChanged == null)
//        return;
//      activeBehaviorChanged(formation);
//    }

//    public void AddTacticOption(TacticComponent tacticOption)
//    {
//      if (!this.HasTeamAi)
//        return;
//      this.TeamAI.AddTacticOption(tacticOption);
//    }

//    public void RemoveTacticOption(System.Type tacticType)
//    {
//      if (!this.HasTeamAi)
//        return;
//      this.TeamAI.RemoveTacticOption(tacticType);
//    }

//    public void ClearTacticOptions()
//    {
//      if (!this.HasTeamAi)
//        return;
//      this.TeamAI.ClearTacticOptions();
//    }

//    public void ResetTactic()
//    {
//      if (!this.HasTeamAi)
//        return;
//      this.TeamAI.ResetTactic();
//    }

//    public void AddTeamAI(TeamAIComponent teamAI, bool forceNotAIControlled = false)
//    {
//      this.TeamAI = teamAI;
//      foreach (Formation formation in (List<Formation>) this.FormationsIncludingSpecialAndEmpty)
//        formation.SetControlledByAI(!forceNotAIControlled && (this != this.Mission.PlayerTeam || !this.IsPlayerGeneral));
//      this.TeamAI.InitializeDetachments(this.Mission);
//      this.TeamAI.CreateMissionSpecificBehaviors();
//      this.TeamAI.ResetTactic();
//      foreach (Formation formation in (List<Formation>) this.FormationsIncludingSpecialAndEmpty)
//      {
//        if (formation.CountOfUnits > 0)
//          formation.AI.Tick();
//      }
//      this.TeamAI.TickOccasionally();
//    }

//    public void DelegateCommandToAI()
//    {
//      foreach (Formation formation in (List<Formation>) this.FormationsIncludingEmpty)
//        formation.SetControlledByAI(true);
//    }

//    public void RearrangeFormationsAccordingToFilter(
//      List<(Formation formation, int troopCount, TroopTraitsMask troopFilter, List<Agent> excludedAgents)> MassTransferData)
//    {
//      List<Formation> formationList = new List<Formation>();
//      foreach ((Formation formation, int troopCount, TroopTraitsMask troopFilter, List<Agent> excludedAgents) tuple in MassTransferData)
//      {
//        tuple.formation.OnMassUnitTransferStart();
//        if (ref tuple.formation.GetReadonlyMovementOrderReference() == MovementOrder.MovementOrderStop && tuple.formation.CountOfUnits > 0)
//        {
//          formationList.Add(tuple.formation);
//          tuple.formation.SetMovementOrder(MovementOrder.MovementOrderMove(tuple.formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None)));
//        }
//      }
//      List<Agent>[] agentListArray = new List<Agent>[MassTransferData.Count];
//      for (int index = 0; index < agentListArray.Length; ++index)
//        agentListArray[index] = new List<Agent>();
//      List<FormationPocket> source = new List<FormationPocket>();
//      for (int index = 0; index < MassTransferData.Count; ++index)
//      {
//        int troopFilter = (int) MassTransferData[index].troopFilter;
//        Func<Agent, int> priorityFunc;
//        TroopFilteringUtilities.GetPriorityFunction((TroopTraitsMask) troopFilter, out priorityFunc);
//        int maxPriority = TroopFilteringUtilities.GetMaxPriority((TroopTraitsMask) troopFilter);
//        source.Add(new FormationPocket(priorityFunc, maxPriority, MassTransferData[index].troopCount, index));
//      }
//      source.RemoveAll((Predicate<FormationPocket>) (pfamv => pfamv.TroopCount <= 0));
//      List<FormationPocket> list1 = source.OrderBy<FormationPocket, int>((Func<FormationPocket, int>) (pfamv => pfamv.TroopCount)).ToList<FormationPocket>().OrderByDescending<FormationPocket, int>((Func<FormationPocket, int>) (pfamv => pfamv.ScoreToSeek)).ToList<FormationPocket>();
//      List<IFormationUnit> formationUnitList = new List<IFormationUnit>();
//      List<IFormationUnit> list2 = MassTransferData.SelectMany<(Formation, int, TroopTraitsMask, List<Agent>), IFormationUnit>((Func<(Formation, int, TroopTraitsMask, List<Agent>), IEnumerable<IFormationUnit>>) (mtd => ((IEnumerable<IFormationUnit>) mtd.formation.DetachedUnits).Concat<IFormationUnit>((IEnumerable<IFormationUnit>) mtd.formation.Arrangement.GetAllUnits()).Except<IFormationUnit>((IEnumerable<IFormationUnit>) mtd.excludedAgents))).ToList<IFormationUnit>();
//      int num = MassTransferData.Sum<(Formation, int, TroopTraitsMask, List<Agent>)>((Func<(Formation, int, TroopTraitsMask, List<Agent>), int>) (mtd => mtd.excludedAgents.Count));
//      int index1 = MassTransferData.Sum<(Formation, int, TroopTraitsMask, List<Agent>)>((Func<(Formation, int, TroopTraitsMask, List<Agent>), int>) (mtd => mtd.formation.CountOfUnits)) - num;
//      int scoreToSeek = list1[0].ScoreToSeek;
//      while (index1 > 0)
//      {
//        for (int index2 = 0; index2 < index1; ++index2)
//        {
//          Agent agent = list2[index2] as Agent;
//          for (int index3 = 0; index3 < list1.Count; ++index3)
//          {
//            FormationPocket formationPocket = list1[index3];
//            int bestScoreSoFar = formationPocket.PriorityFunction(agent);
//            if (scoreToSeek <= formationPocket.ScoreToSeek && bestScoreSoFar >= scoreToSeek)
//            {
//              agentListArray[formationPocket.Index].Add(agent);
//              formationPocket.AddTroop();
//              if (formationPocket.IsFormationPocketFilled())
//                list1.RemoveAt(index3);
//              --index1;
//              list2[index2] = list2[index1];
//              --index2;
//              break;
//            }
//            if (bestScoreSoFar > formationPocket.BestScoreSoFar)
//              formationPocket.SetBestScoreSoFar(bestScoreSoFar);
//          }
//        }
//        if (list1.Count != 0)
//        {
//          for (int index4 = 0; index4 < list1.Count; ++index4)
//            list1[index4].UpdateScoreToSeek();
//          list1.OrderByDescending<FormationPocket, int>((Func<FormationPocket, int>) (pfamv => pfamv.ScoreToSeek));
//          scoreToSeek = list1[0].ScoreToSeek;
//        }
//        else
//          break;
//      }
//      for (int index5 = 0; index5 < agentListArray.Length; ++index5)
//      {
//        foreach (Agent agent in agentListArray[index5])
//          agent.Formation = MassTransferData[index5].formation;
//      }
//      foreach ((Formation formation, int troopCount, TroopTraitsMask troopFilter, List<Agent> excludedAgents) tuple in MassTransferData)
//      {
//        this.TriggerOnFormationsChanged(tuple.formation);
//        tuple.formation.OnMassUnitTransferEnd();
//        if (tuple.formation.CountOfUnits > 0 && !tuple.formation.OrderPositionIsValid)
//        {
//          Vec2 averagePositionOfUnits = tuple.formation.GetAveragePositionOfUnits(false, false);
//          float terrainHeight = this.Mission.Scene.GetTerrainHeight(averagePositionOfUnits);
//          this.Mission.Scene.GetHeightAtPoint(averagePositionOfUnits, BodyFlags.None, ref terrainHeight);
//          Vec3 position = new Vec3(averagePositionOfUnits, terrainHeight);
//          WorldPosition worldPosition = new WorldPosition(this.Mission.Scene, UIntPtr.Zero, position, false);
//          tuple.formation.SetPositioning(new WorldPosition?(worldPosition));
//        }
//      }
//      foreach (Formation formation in formationList)
//        formation.SetMovementOrder(MovementOrder.MovementOrderStop);
//    }

//    public Formation GeneralsFormation { get; set; }

//    public Formation BodyGuardFormation { get; set; }

//    public Agent GeneralAgent { get; set; }

//    public void Tick(float dt)
//    {
//      if (!this._cachedEnemyDataForFleeing.IsEmpty<(float, WorldPosition, int, Vec2, Vec2, bool)>())
//        this._cachedEnemyDataForFleeing.Clear();
//      if (this.Mission.AllowAiTicking)
//      {
//        if (this.Mission.RetreatSide != BattleSideEnum.None && this.Side == this.Mission.RetreatSide)
//        {
//          foreach (Formation formation in (List<Formation>) this.FormationsIncludingSpecialAndEmpty)
//          {
//            if (formation.CountOfUnits > 0)
//              formation.SetMovementOrder(MovementOrder.MovementOrderRetreat);
//          }
//        }
//        else if (this.TeamAI != null && this.HasBots)
//          this.TeamAI.Tick(dt);
//      }
//      if (GameNetwork.IsReplay)
//        return;
//      if (this._tickDetachments)
//        this.DetachmentManager.TickDetachments();
//      foreach (Formation formation in (List<Formation>) this.FormationsIncludingSpecialAndEmpty)
//      {
//        if (formation.CountOfUnits > 0)
//          formation.Tick(dt);
//      }
//    }

//    public Formation GetFormation(FormationClass formationIndex)
//    {
//      return this.FormationsIncludingSpecialAndEmpty[(int) formationIndex];
//    }

//    public void SetIsEnemyOf(Team otherTeam, bool isEnemyOf)
//    {
//      this.MBTeam.SetIsEnemyOf(otherTeam.MBTeam, isEnemyOf);
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new TeamSetIsEnemyOf(this.TeamIndex, otherTeam.TeamIndex, isEnemyOf));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//    }

//    public bool IsEnemyOf(Team otherTeam) => this.MBTeam.IsEnemyOf(otherTeam.MBTeam);

//    public bool IsFriendOf(Team otherTeam)
//    {
//      return this == otherTeam || !this.MBTeam.IsEnemyOf(otherTeam.MBTeam);
//    }

//    public IEnumerable<Agent> Heroes
//    {
//      get
//      {
//        Team team = this;
//        Agent main = Agent.Main;
//        if (main != null && main.Team == team)
//          yield return main;
//      }
//    }

//    public void AddAgentToTeam(Agent unit)
//    {
//      this._teamAgents.Add(unit);
//      this._activeAgents.Add(unit);
//    }

//    public void RemoveAgentFromTeam(Agent unit)
//    {
//      this._teamAgents.Remove(unit);
//      this._activeAgents.Remove(unit);
//    }

//    public void DeactivateAgent(Agent agent) => this._activeAgents.Remove(agent);

//    public void OnAgentRemoved(Agent agent)
//    {
//      if (GameNetwork.IsClientOrReplay)
//        return;
//      foreach (Formation formation in (List<Formation>) this.FormationsIncludingSpecialAndEmpty)
//        formation.AI.OnAgentRemoved(agent);
//    }

//    public bool HasBots
//    {
//      get
//      {
//        foreach (Agent activeAgent in (List<Agent>) this.ActiveAgents)
//        {
//          if (!activeAgent.IsMount && !activeAgent.IsPlayerControlled)
//            return true;
//        }
//        return false;
//      }
//    }

//    public Agent Leader
//    {
//      get
//      {
//        if (Agent.Main != null && Agent.Main.Team == this)
//          return Agent.Main;
//        Agent leader = (Agent) null;
//        foreach (Agent activeAgent in (List<Agent>) this.ActiveAgents)
//        {
//          if (leader == null || activeAgent.IsHero)
//          {
//            leader = activeAgent;
//            if (leader.IsHero)
//              break;
//          }
//        }
//        return leader;
//      }
//    }

//    public static Team Invalid
//    {
//      get
//      {
//        return Team._invalid ?? (Team._invalid = new Team(MBTeam.InvalidTeam, BattleSideEnum.None, (Mission) null));
//      }
//      internal set => Team._invalid = value;
//    }

//    public bool IsValid => this.MBTeam.IsValid;

//    public override string ToString() => this.MBTeam.ToString();

//    public bool HasTeamAi => this.TeamAI != null;

//    public void OnMissionEnded()
//    {
//      if (!this.HasTeamAi)
//        return;
//      this.TeamAI.OnMissionEnded();
//    }

//    public void TriggerOnFormationsChanged(Formation formation)
//    {
//      Action<Team, Formation> formationsChanged = this.OnFormationsChanged;
//      if (formationsChanged == null)
//        return;
//      formationsChanged(this, formation);
//    }

//    public OrderController GetOrderControllerOf(Agent agent)
//    {
//      OrderController orderControllerOf = this._orderControllers.FirstOrDefault<OrderController>((Func<OrderController, bool>) (oc => oc.Owner == agent));
//      if (orderControllerOf == null)
//      {
//        orderControllerOf = new OrderController(this.Mission, this, agent);
//        this._orderControllers.Add(orderControllerOf);
//        orderControllerOf.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
//      }
//      return orderControllerOf;
//    }

//    public void SetPlayerRole(bool isPlayerGeneral, bool isPlayerSergeant)
//    {
//      this.IsPlayerGeneral = isPlayerGeneral;
//      this.IsPlayerSergeant = isPlayerSergeant;
//      foreach (Formation formation in (List<Formation>) this.FormationsIncludingSpecialAndEmpty)
//        formation.SetControlledByAI(this != this.Mission.PlayerTeam || !this.IsPlayerGeneral);
//    }

//    public bool HasAnyEnemyTeamsWithAgents(bool ignoreMountedAgents)
//    {
//      foreach (Team team in (List<Team>) this.Mission.Teams)
//      {
//        if (team != this && team.IsEnemyOf(this) && team.ActiveAgents.Count > 0)
//        {
//          if (!ignoreMountedAgents)
//            return true;
//          foreach (Agent activeAgent in (List<Agent>) team.ActiveAgents)
//          {
//            if (!activeAgent.HasMount)
//              return true;
//          }
//        }
//      }
//      return false;
//    }

//    public bool HasAnyFormationsIncludingSpecialThatIsNotEmpty()
//    {
//      foreach (Formation formation in (List<Formation>) this.FormationsIncludingSpecialAndEmpty)
//      {
//        if (formation.CountOfUnits > 0)
//          return true;
//      }
//      return false;
//    }

//    public int GetFormationCount()
//    {
//      int formationCount = 0;
//      foreach (Formation formation in (List<Formation>) this.FormationsIncludingEmpty)
//      {
//        if (formation.CountOfUnits > 0)
//          ++formationCount;
//      }
//      return formationCount;
//    }

//    public int GetAIControlledFormationCount()
//    {
//      int controlledFormationCount = 0;
//      foreach (Formation formation in (List<Formation>) this.FormationsIncludingEmpty)
//      {
//        if (formation.CountOfUnits > 0 && formation.IsAIControlled)
//          ++controlledFormationCount;
//      }
//      return controlledFormationCount;
//    }

//    public Vec2 GetAveragePositionOfEnemies()
//    {
//      Vec2 zero = Vec2.Zero;
//      int num = 0;
//      foreach (Team team in (List<Team>) this.Mission.Teams)
//      {
//        if (team.MBTeam.IsValid && this.IsEnemyOf(team))
//        {
//          foreach (Agent activeAgent in (List<Agent>) team.ActiveAgents)
//          {
//            zero += activeAgent.Position.AsVec2;
//            ++num;
//          }
//        }
//      }
//      return num > 0 ? zero * (1f / (float) num) : Vec2.Invalid;
//    }

//    public Vec2 GetAveragePosition()
//    {
//      Vec2 zero = Vec2.Zero;
//      MBReadOnlyList<Agent> activeAgents = this.ActiveAgents;
//      int num = 0;
//      foreach (Agent agent in (List<Agent>) activeAgents)
//      {
//        zero += agent.Position.AsVec2;
//        ++num;
//      }
//      return num <= 0 ? Vec2.Invalid : zero * (1f / (float) num);
//    }

//    public WorldPosition GetMedianPosition(Vec2 averagePosition)
//    {
//      float num1 = float.MaxValue;
//      Agent agent = (Agent) null;
//      foreach (Agent activeAgent in (List<Agent>) this.ActiveAgents)
//      {
//        float num2 = activeAgent.Position.AsVec2.DistanceSquared(averagePosition);
//        if ((double) num2 <= (double) num1)
//        {
//          agent = activeAgent;
//          num1 = num2;
//        }
//      }
//      return agent == null ? WorldPosition.Invalid : agent.GetWorldPosition();
//    }

//    public Vec2 GetWeightedAverageOfEnemies(Vec2 basePoint)
//    {
//      Vec2 zero = Vec2.Zero;
//      float num1 = 0.0f;
//      foreach (Team team in (List<Team>) this.Mission.Teams)
//      {
//        if (team.MBTeam.IsValid && this.IsEnemyOf(team))
//        {
//          foreach (Agent activeAgent in (List<Agent>) team.ActiveAgents)
//          {
//            Vec2 asVec2 = activeAgent.Position.AsVec2;
//            float num2 = 1f / (basePoint - asVec2).LengthSquared;
//            zero += asVec2 * num2;
//            num1 += num2;
//          }
//        }
//      }
//      return (double) num1 > 0.0 ? zero * (1f / num1) : Vec2.Invalid;
//    }

//    public void DisableDetachmentTicking() => this._tickDetachments = false;

//    [Conditional("DEBUG")]
//    private void TickStandingPointDebug()
//    {
//    }
//  }
//}
