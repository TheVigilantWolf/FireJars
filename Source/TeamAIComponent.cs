//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.TeamAIComponent
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using TaleWorlds.Core;
//using TaleWorlds.Engine;
//using TaleWorlds.InputSystem;
//using TaleWorlds.Library;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public abstract class TeamAIComponent
//  {
//    public TeamAIComponent.TacticalDecisionDelegate OnNotifyTacticalDecision;
//    public const int BattleTokenForceSize = 10;
//    private readonly List<TacticComponent> _availableTactics;
//    private static bool _retreatScriptActive;
//    protected readonly Mission Mission;
//    protected readonly Team Team;
//    private readonly Timer _thinkTimer;
//    private readonly Timer _applyTimer;
//    private TacticComponent _currentTactic;
//    public List<TacticalPosition> TacticalPositions;
//    public List<TacticalRegion> TacticalRegions;
//    private readonly MBList<StrategicArea> _strategicAreas;
//    private readonly float _occasionalTickTime;
//    private MissionTime _nextTacticChooseTime;
//    private MissionTime _nextOccasionalTickTime;

//    public MBReadOnlyList<StrategicArea> StrategicAreas
//    {
//      get => (MBReadOnlyList<StrategicArea>) this._strategicAreas;
//    }

//    public bool HasStrategicAreas => !this._strategicAreas.IsEmpty<StrategicArea>();

//    public bool IsDefenseApplicable { get; private set; }

//    public bool GetIsFirstTacticChosen { get; private set; }

//    protected TacticComponent CurrentTactic
//    {
//      get => this._currentTactic;
//      private set
//      {
//        this._currentTactic?.OnCancel();
//        this._currentTactic = value;
//        if (this._currentTactic == null)
//          return;
//        this._currentTactic.OnApply();
//        this._currentTactic.TickOccasionally();
//      }
//    }

//    protected TeamAIComponent(
//      Mission currentMission,
//      Team currentTeam,
//      float thinkTimerTime,
//      float applyTimerTime)
//    {
//      this.Mission = currentMission;
//      this.Team = currentTeam;
//      this._thinkTimer = new Timer(this.Mission.CurrentTime, thinkTimerTime);
//      this._applyTimer = new Timer(this.Mission.CurrentTime, applyTimerTime);
//      this._occasionalTickTime = applyTimerTime;
//      this._availableTactics = new List<TacticComponent>();
//      this.TacticalPositions = currentMission.ActiveMissionObjects.FindAllWithType<TacticalPosition>().ToList<TacticalPosition>();
//      this.TacticalRegions = currentMission.ActiveMissionObjects.FindAllWithType<TacticalRegion>().ToList<TacticalRegion>();
//      this._strategicAreas = currentMission.ActiveMissionObjects.Where<MissionObject>((Func<MissionObject, bool>) (amo => amo is StrategicArea strategicArea && strategicArea.IsActive && strategicArea.IsUsableBy(this.Team.Side))).Select<MissionObject, StrategicArea>((Func<MissionObject, StrategicArea>) (amo => amo as StrategicArea)).ToMBList<StrategicArea>();
//    }

//    public void AddStrategicArea(StrategicArea strategicArea)
//    {
//      this._strategicAreas.Add(strategicArea);
//    }

//    public void RemoveStrategicArea(StrategicArea strategicArea)
//    {
//      if (this.Team.DetachmentManager.ContainsDetachment((IDetachment) strategicArea))
//        this.Team.DetachmentManager.DestroyDetachment((IDetachment) strategicArea);
//      this._strategicAreas.Remove(strategicArea);
//    }

//    public void RemoveAllStrategicAreas()
//    {
//      foreach (StrategicArea strategicArea in (List<StrategicArea>) this._strategicAreas)
//      {
//        if (this.Team.DetachmentManager.ContainsDetachment((IDetachment) strategicArea))
//          this.Team.DetachmentManager.DestroyDetachment((IDetachment) strategicArea);
//      }
//      this._strategicAreas.Clear();
//    }

//    public void AddTacticOption(TacticComponent tacticOption)
//    {
//      this._availableTactics.Add(tacticOption);
//    }

//    public void RemoveTacticOption(System.Type tacticType)
//    {
//      this._availableTactics.RemoveAll((Predicate<TacticComponent>) (at => tacticType == at.GetType()));
//    }

//    public void ClearTacticOptions() => this._availableTactics.Clear();

//    [Conditional("DEBUG")]
//    public void AssertTeam(Team team)
//    {
//    }

//    public void NotifyTacticalDecision(in TacticalDecision decision)
//    {
//      TeamAIComponent.TacticalDecisionDelegate tacticalDecision = this.OnNotifyTacticalDecision;
//      if (tacticalDecision == null)
//        return;
//      tacticalDecision(in decision);
//    }

//    public virtual void OnDeploymentFinished()
//    {
//    }

//    public virtual void OnFormationFrameChanged(
//      Agent agent,
//      bool isFrameEnabled,
//      WorldPosition frame)
//    {
//    }

//    public virtual void OnMissionEnded()
//    {
//      MBDebug.Print("Mission end received by teamAI");
//      foreach (Formation formation in (List<Formation>) this.Team.FormationsIncludingSpecialAndEmpty)
//      {
//        if (formation.CountOfUnits > 0)
//        {
//          foreach (UsableMachine usable in formation.GetUsedMachines().ToList<UsableMachine>())
//            formation.StopUsingMachine(usable);
//        }
//      }
//    }

//    public void ResetTacticalPositions()
//    {
//      this.TacticalPositions = this.Mission.ActiveMissionObjects.FindAllWithType<TacticalPosition>().ToList<TacticalPosition>();
//      this.TacticalRegions = this.Mission.ActiveMissionObjects.FindAllWithType<TacticalRegion>().ToList<TacticalRegion>();
//    }

//    public void ResetTactic(bool keepCurrentTactic = true)
//    {
//      if (!keepCurrentTactic)
//        this.CurrentTactic = (TacticComponent) null;
//      this._thinkTimer.Reset(this.Mission.CurrentTime);
//      this._applyTimer.Reset(this.Mission.CurrentTime);
//      this.MakeDecision();
//      this.TickOccasionally();
//    }

//    protected internal virtual void Tick(float dt)
//    {
//      if (this.Team.BodyGuardFormation != null && this.Team.BodyGuardFormation.CountOfUnits > 0 && (this.Team.GeneralsFormation == null || this.Team.GeneralsFormation.CountOfUnits == 0))
//      {
//        this.Team.BodyGuardFormation.AI.ResetBehaviorWeights();
//        this.Team.BodyGuardFormation.AI.SetBehaviorWeight<BehaviorCharge>(1f);
//      }
//      if (this._nextTacticChooseTime.IsPast)
//      {
//        this.MakeDecision();
//        this._nextTacticChooseTime = MissionTime.SecondsFromNow(5f);
//      }
//      if (!this._nextOccasionalTickTime.IsPast)
//        return;
//      this.TickOccasionally();
//      this._nextOccasionalTickTime = MissionTime.SecondsFromNow(this._occasionalTickTime);
//    }

//    public void CheckIsDefenseApplicable()
//    {
//      if (this.Team.Side != BattleSideEnum.Defender)
//      {
//        this.IsDefenseApplicable = false;
//      }
//      else
//      {
//        int memberCount = this.Team.QuerySystem.MemberCount;
//        float rangedAttackRatio = this.Team.QuerySystem.MaxUnderRangedAttackRatio;
//        double num1 = (double) memberCount * (double) rangedAttackRatio;
//        int deathByRangedCount = this.Team.QuerySystem.DeathByRangedCount;
//        int deathCount = this.Team.QuerySystem.DeathCount;
//        double num2 = (double) deathByRangedCount;
//        float num3 = MBMath.ClampFloat((float) (num1 + num2) / (float) (memberCount + deathCount), 0.05f, 1f);
//        int enemyUnitCount = this.Team.QuerySystem.EnemyUnitCount;
//        float num4 = 0.0f;
//        int num5 = 0;
//        int num6 = 0;
//        foreach (Team team in (List<Team>) this.Mission.Teams)
//        {
//          if (this.Team.IsEnemyOf(team))
//          {
//            TeamQuerySystem querySystem = team.QuerySystem;
//            num5 += querySystem.DeathByRangedCount;
//            num6 += querySystem.DeathCount;
//            num4 += enemyUnitCount == 0 ? 0.0f : querySystem.MaxUnderRangedAttackRatio * ((float) querySystem.MemberCount / (enemyUnitCount > 0 ? (float) enemyUnitCount : 1f));
//          }
//        }
//        double num7 = (double) enemyUnitCount * (double) num4;
//        int num8 = enemyUnitCount + num6;
//        double num9 = (double) num5;
//        float num10 = MBMath.ClampFloat((float) ((num7 + num9) / (num8 > 0 ? (double) num8 : 1.0)), 0.05f, 1f);
//        this.IsDefenseApplicable = (double) MathF.Pow(num3 / num10, (float) (3.0 * ((double) this.Team.QuerySystem.EnemyRangedRatio + (double) this.Team.QuerySystem.EnemyRangedCavalryRatio))) <= 1.5;
//      }
//    }

//    public void OnTacticAppliedForFirstTime() => this.GetIsFirstTacticChosen = false;

//    private void MakeDecision()
//    {
//      List<TacticComponent> availableTactics = this._availableTactics;
//      if (this.Mission.CurrentState != Mission.State.Continuing && availableTactics.Count == 0 || !this.Team.HasAnyFormationsIncludingSpecialThatIsNotEmpty())
//        return;
//      bool flag1 = true;
//      foreach (Team team in (List<Team>) this.Mission.Teams)
//      {
//        if (team.IsEnemyOf(this.Team) && team.HasAnyFormationsIncludingSpecialThatIsNotEmpty())
//        {
//          flag1 = false;
//          break;
//        }
//      }
//      if (flag1)
//      {
//        if (this.Mission.MissionEnded)
//          return;
//        if (!(this.CurrentTactic is TacticCharge))
//        {
//          foreach (TacticComponent tacticComponent in availableTactics)
//          {
//            if (tacticComponent is TacticCharge)
//            {
//              if (this.CurrentTactic == null)
//                this.GetIsFirstTacticChosen = true;
//              this.CurrentTactic = tacticComponent;
//              break;
//            }
//          }
//          if (!(this.CurrentTactic is TacticCharge))
//          {
//            if (this.CurrentTactic == null)
//              this.GetIsFirstTacticChosen = true;
//            this.CurrentTactic = availableTactics.FirstOrDefault<TacticComponent>();
//          }
//        }
//      }
//      this.CheckIsDefenseApplicable();
//      TacticComponent tacticComponent1 = availableTactics.MaxBy<TacticComponent, float>((Func<TacticComponent, float>) (to => to.GetTacticWeight() * (to == this._currentTactic ? 1.5f : 1f)));
//      bool flag2 = false;
//      if (this.CurrentTactic == null)
//        flag2 = true;
//      else if (this.CurrentTactic != tacticComponent1)
//      {
//        if (!this.CurrentTactic.ResetTacticalPositions())
//          flag2 = true;
//        else if ((double) tacticComponent1.GetTacticWeight() > (double) (this.CurrentTactic.GetTacticWeight() * 1.5f))
//          flag2 = true;
//      }
//      if (!flag2)
//        return;
//      if (this.CurrentTactic == null)
//        this.GetIsFirstTacticChosen = true;
//      this.CurrentTactic = tacticComponent1;
//      if (Mission.Current.MainAgent == null || this.Team.GeneralAgent == null || !this.Team.IsPlayerTeam || !this.Team.IsPlayerSergeant)
//        return;
//      MBInformationManager.AddQuickInformation(GameTexts.FindText("str_team_ai_tactic_text", tacticComponent1.GetType().Name), 4000, this.Team.GeneralAgent.Character);
//    }

//    public void TickOccasionally()
//    {
//      if (!Mission.Current.AllowAiTicking || !this.Team.HasBots)
//        return;
//      this.CurrentTactic?.TickOccasionally();
//    }

//    public bool IsCurrentTactic(TacticComponent tactic) => tactic == this.CurrentTactic;

//    [Conditional("DEBUG")]
//    protected virtual void DebugTick(float dt)
//    {
//      if (!MBDebug.IsDisplayingHighLevelAI)
//        return;
//      TacticComponent currentTactic = this.CurrentTactic;
//      if (Input.DebugInput.IsHotKeyPressed("UsableMachineAiBaseHotkeyRetreatScriptActive"))
//        TeamAIComponent._retreatScriptActive = true;
//      else if (Input.DebugInput.IsHotKeyPressed("UsableMachineAiBaseHotkeyRetreatScriptPassive"))
//        TeamAIComponent._retreatScriptActive = false;
//      int num = TeamAIComponent._retreatScriptActive ? 1 : 0;
//    }

//    public abstract void OnUnitAddedToFormationForTheFirstTime(Formation formation);

//    protected internal virtual void CreateMissionSpecificBehaviors()
//    {
//    }

//    protected internal virtual void InitializeDetachments(Mission mission)
//    {
//      this.Mission.GetMissionBehavior<DeploymentHandler>()?.InitializeDeploymentPoints();
//    }

//    protected class TacticOption
//    {
//      public string Id { get; private set; }

//      public Lazy<TacticComponent> Tactic { get; private set; }

//      public float Weight { get; set; }

//      public TacticOption(string id, Lazy<TacticComponent> tactic, float weight)
//      {
//        this.Id = id;
//        this.Tactic = tactic;
//        this.Weight = weight;
//      }
//    }

//    public delegate void TacticalDecisionDelegate(in TacticalDecision decision);
//  }
//}
