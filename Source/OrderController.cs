//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.OrderController
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using NetworkMessages.FromClient;
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
//  public class OrderController
//  {
//    public const float FormationGapInLine = 1.5f;
//    protected readonly Mission _mission;
//    public readonly Team Team;
//    public Agent Owner;
//    protected readonly MBList<Formation> _selectedFormations;
//    private Dictionary<Formation, float> actualWidths;
//    private Dictionary<Formation, int> actualUnitSpacings;
//    private List<Func<Formation, MovementOrder, MovementOrder>> orderOverrides;
//    private List<(Formation, OrderType)> overridenOrders;
//    protected bool _formationUpdateEnabledAfterSetOrder = true;
//    private bool _gesturesEnabled;
//    private MissionTimer _yellingAfterChargeOrderTimer;

//    public SiegeWeaponController SiegeWeaponController { get; private set; }

//    public MBReadOnlyList<Formation> SelectedFormations
//    {
//      get => (MBReadOnlyList<Formation>) this._selectedFormations;
//    }

//    public bool FormationUpdateEnabledAfterSetOrder => this._formationUpdateEnabledAfterSetOrder;

//    public event OnOrderIssuedDelegate OnOrderIssued;

//    public event Action OnSelectedFormationsChanged;

//    public Dictionary<Formation, Formation> simulationFormations { get; private set; }

//    public OrderController(Mission mission, Team team, Agent owner)
//    {
//      this._mission = mission;
//      this.Team = team;
//      this.Owner = owner;
//      this._gesturesEnabled = true;
//      this._selectedFormations = new MBList<Formation>();
//      this.SiegeWeaponController = new SiegeWeaponController(mission, this.Team);
//      this.simulationFormations = new Dictionary<Formation, Formation>();
//      this.actualWidths = new Dictionary<Formation, float>();
//      this.actualUnitSpacings = new Dictionary<Formation, int>();
//      this._yellingAfterChargeOrderTimer = new MissionTimer(30f);
//      this._yellingAfterChargeOrderTimer.Set(-30f);
//      foreach (Formation formation in (List<Formation>) this.Team.FormationsIncludingEmpty)
//      {
//        formation.OnWidthChanged += new Action<Formation>(this.Formation_OnWidthChanged);
//        formation.OnUnitSpacingChanged += new Action<Formation>(this.Formation_OnUnitSpacingChanged);
//      }
//      if (this.Team.IsPlayerGeneral)
//      {
//        foreach (Formation formation in (List<Formation>) this.Team.FormationsIncludingEmpty)
//          formation.PlayerOwner = owner;
//      }
//      this.CreateDefaultOrderOverrides();
//    }

//    internal void AssignDelegatesToController(OrderController newController)
//    {
//      newController.OnOrderIssued = this.OnOrderIssued;
//      newController.OnSelectedFormationsChanged = this.OnSelectedFormationsChanged;
//    }

//    private void Formation_OnUnitSpacingChanged(Formation formation)
//    {
//      this.actualUnitSpacings.Remove(formation);
//    }

//    private void Formation_OnWidthChanged(Formation formation)
//    {
//      this.actualWidths.Remove(formation);
//    }

//    protected void OnSelectedFormationsCollectionChanged()
//    {
//      Action formationsChanged = this.OnSelectedFormationsChanged;
//      if (formationsChanged != null)
//        formationsChanged();
//      foreach (Formation key in this.SelectedFormations.Except<Formation>((IEnumerable<Formation>) this.simulationFormations.Keys))
//        this.simulationFormations[key] = new Formation((Team) null, -1);
//    }

//    protected virtual void SelectFormation(Formation formation, Agent selectorAgent)
//    {
//      if (!this._selectedFormations.Contains(formation) && this.IsFormationSelectable(formation, selectorAgent))
//      {
//        if (GameNetwork.IsClient)
//        {
//          GameNetwork.BeginModuleEventAsClient();
//          GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.SelectFormation(formation.Index));
//          GameNetwork.EndModuleEventAsClient();
//        }
//        if (selectorAgent != null && this.AreGesturesEnabled())
//          OrderController.PlayFormationSelectedGesture(formation, selectorAgent);
//        MBDebug.Print(formation?.RepresentativeClass.ToString() + " added to selected formations.");
//        this._selectedFormations.Add(formation);
//        this.OnSelectedFormationsCollectionChanged();
//      }
//      else
//        TaleWorlds.Library.Debug.FailedAssert("Formation already selected or is not selectable", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (SelectFormation), 194);
//    }

//    public void SelectFormation(Formation formation) => this.SelectFormation(formation, this.Owner);

//    public void DeselectFormation(Formation formation)
//    {
//      if (this._selectedFormations.Contains(formation))
//      {
//        if (GameNetwork.IsClient)
//        {
//          GameNetwork.BeginModuleEventAsClient();
//          GameNetwork.WriteMessage((GameNetworkMessage) new UnselectFormation(formation.Index));
//          GameNetwork.EndModuleEventAsClient();
//        }
//        MBDebug.Print(formation?.RepresentativeClass.ToString() + " is removed from selected formations.");
//        this._selectedFormations.Remove(formation);
//        this.OnSelectedFormationsCollectionChanged();
//      }
//      else
//        TaleWorlds.Library.Debug.FailedAssert("Trying to deselect an unselected formation", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (DeselectFormation), 220);
//    }

//    public bool IsFormationListening(Formation formation)
//    {
//      return this.SelectedFormations.Contains(formation);
//    }

//    public bool IsFormationSelectable(Formation formation)
//    {
//      return this.IsFormationSelectable(formation, this.Owner);
//    }

//    public bool BackupAndDisableGesturesEnabled()
//    {
//      int num = this._gesturesEnabled ? 1 : 0;
//      this._gesturesEnabled = false;
//      return num != 0;
//    }

//    public void RestoreGesturesEnabled(bool oldValue) => this._gesturesEnabled = oldValue;

//    protected bool IsFormationSelectable(Formation formation, Agent selectorAgent)
//    {
//      return (selectorAgent == null || formation.PlayerOwner == selectorAgent) && formation.CountOfUnits > 0;
//    }

//    protected bool AreGesturesEnabled()
//    {
//      return this._gesturesEnabled && this._mission.IsOrderGesturesEnabled() && !GameNetwork.IsClientOrReplay;
//    }

//    protected virtual void SelectAllFormations(Agent selectorAgent, bool uiFeedback)
//    {
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.SelectAllFormations());
//        GameNetwork.EndModuleEventAsClient();
//      }
//      if (uiFeedback && selectorAgent != null && this.AreGesturesEnabled())
//        selectorAgent.MakeVoice(SkinVoiceManager.VoiceType.Everyone, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//      MBDebug.Print("Selected formations being cleared. Select all formations:");
//      this._selectedFormations.Clear();
//      foreach (Formation formation in this.Team.FormationsIncludingEmpty.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0 && this.IsFormationSelectable(f, selectorAgent))))
//      {
//        MBDebug.Print(formation.RepresentativeClass.ToString() + " added to selected formations.");
//        this._selectedFormations.Add(formation);
//      }
//      this.OnSelectedFormationsCollectionChanged();
//    }

//    public void SelectAllFormations(bool uiFeedback = false)
//    {
//      this.SelectAllFormations(this.Owner, uiFeedback);
//    }

//    public void ClearSelectedFormations()
//    {
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.ClearSelectedFormations());
//        GameNetwork.EndModuleEventAsClient();
//      }
//      MBDebug.Print("Selected formations being cleared.");
//      this._selectedFormations.Clear();
//      this.OnSelectedFormationsCollectionChanged();
//    }

//    public virtual void SetOrder(OrderType orderType)
//    {
//      MBDebug.Print("SetOrder " + (object) orderType + "on team");
//      this.BeforeSetOrder(orderType);
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrder(orderType));
//        GameNetwork.EndModuleEventAsClient();
//      }
//      switch (orderType)
//      {
//        case OrderType.Charge:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetMovementOrder(MovementOrder.MovementOrderCharge);
//            break;
//          }
//        case OrderType.StandYourGround:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetMovementOrder(MovementOrder.MovementOrderStop);
//            break;
//          }
//        case OrderType.Retreat:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetMovementOrder(MovementOrder.MovementOrderRetreat);
//            break;
//          }
//        case OrderType.AdvanceTenPaces:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              if (current.GetReadonlyMovementOrderReference().OrderEnum == MovementOrder.MovementOrderEnum.Move)
//              {
//                MovementOrder input = current.GetReadonlyMovementOrderReference();
//                input.Advance(current, 7f);
//                current.SetMovementOrder(input);
//              }
//            }
//            break;
//          }
//        case OrderType.FallBackTenPaces:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              if (current.GetReadonlyMovementOrderReference().OrderEnum == MovementOrder.MovementOrderEnum.Move)
//              {
//                MovementOrder input = current.GetReadonlyMovementOrderReference();
//                input.FallBack(current, 7f);
//                current.SetMovementOrder(input);
//              }
//            }
//            break;
//          }
//        case OrderType.Advance:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetMovementOrder(MovementOrder.MovementOrderAdvance);
//            break;
//          }
//        case OrderType.FallBack:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetMovementOrder(MovementOrder.MovementOrderFallBack);
//            break;
//          }
//        case OrderType.LookAtEnemy:
//          FacingOrder orderLookAtEnemy = FacingOrder.FacingOrderLookAtEnemy;
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetFacingOrder(orderLookAtEnemy);
//            }
//            break;
//          }
//        case OrderType.ArrangementLine:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);
//            }
//            break;
//          }
//        case OrderType.ArrangementCloseOrder:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);
//            }
//            break;
//          }
//        case OrderType.ArrangementLoose:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetArrangementOrder(ArrangementOrder.ArrangementOrderLoose);
//            }
//            break;
//          }
//        case OrderType.ArrangementCircular:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetArrangementOrder(ArrangementOrder.ArrangementOrderCircle);
//            }
//            break;
//          }
//        case OrderType.ArrangementSchiltron:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetArrangementOrder(ArrangementOrder.ArrangementOrderSquare);
//            }
//            break;
//          }
//        case OrderType.ArrangementVee:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetArrangementOrder(ArrangementOrder.ArrangementOrderSkein);
//            }
//            break;
//          }
//        case OrderType.ArrangementColumn:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetArrangementOrder(ArrangementOrder.ArrangementOrderColumn);
//            }
//            break;
//          }
//        case OrderType.ArrangementScatter:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetArrangementOrder(ArrangementOrder.ArrangementOrderScatter);
//            }
//            break;
//          }
//        case OrderType.FormDeep:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetFormOrder(FormOrder.FormOrderDeep);
//            }
//            break;
//          }
//        case OrderType.FormWide:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetFormOrder(FormOrder.FormOrderWide);
//            }
//            break;
//          }
//        case OrderType.FormWider:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              OrderController.TryCancelStopOrder(current);
//              current.SetFormOrder(FormOrder.FormOrderWider);
//            }
//            break;
//          }
//        case OrderType.HoldFire:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetFiringOrder(FiringOrder.FiringOrderHoldYourFire);
//            break;
//          }
//        case OrderType.FireAtWill:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
//            break;
//          }
//        case OrderType.Mount:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              if (current.PhysicalClass.IsMounted() || current.HasAnyMountedUnit)
//                OrderController.TryCancelStopOrder(current);
//              current.SetRidingOrder(RidingOrder.RidingOrderMount);
//            }
//            break;
//          }
//        case OrderType.Dismount:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              if (current.PhysicalClass.IsMounted() || current.HasAnyMountedUnit)
//                OrderController.TryCancelStopOrder(current);
//              current.SetRidingOrder(RidingOrder.RidingOrderDismount);
//            }
//            break;
//          }
//        case OrderType.AIControlOn:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetControlledByAI(true);
//            break;
//          }
//        case OrderType.AIControlOff:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetControlledByAI(false);
//            break;
//          }
//        default:
//          TaleWorlds.Library.Debug.FailedAssert("[DEBUG]Invalid order type.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (SetOrder), 620);
//          break;
//      }
//      this.AfterSetOrder(orderType);
//      this.FireOnOrderIssued(orderType, this.SelectedFormations, this);
//    }

//    private void PlayOrderGestures(OrderType orderType)
//    {
//      if (!LoadingWindow.IsLoadingWindowActive)
//      {
//        switch (orderType)
//        {
//          case OrderType.Move:
//          case OrderType.MoveToLineSegment:
//          case OrderType.MoveToLineSegmentWithHorizontalLayout:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.Move, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.Charge:
//          case OrderType.ChargeWithTarget:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.Charge, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.StandYourGround:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.Stop, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.FollowMe:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.Follow, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.Retreat:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.Retreat, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.AdvanceTenPaces:
//          case OrderType.Advance:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.Advance, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.FallBackTenPaces:
//          case OrderType.FallBack:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FallBack, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.LookAtEnemy:
//            if (Mission.Current.IsNavalBattle)
//            {
//              this.Owner.MakeVoice(SkinVoiceManager.VoiceType.BoardAtWill, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//              break;
//            }
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FaceEnemy, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.LookAtDirection:
//            if (Mission.Current.IsNavalBattle)
//            {
//              this.Owner.MakeVoice(SkinVoiceManager.VoiceType.AvoidBoarding, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//              break;
//            }
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FaceDirection, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.ArrangementLine:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FormLine, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.ArrangementCloseOrder:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FormShieldWall, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.ArrangementLoose:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FormLoose, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.ArrangementCircular:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FormCircle, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.ArrangementSchiltron:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FormSquare, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.ArrangementVee:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FormSkein, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.ArrangementColumn:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FormColumn, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.ArrangementScatter:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FormScatter, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.HoldFire:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.HoldFire, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.FireAtWill:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.FireAtWill, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.Mount:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.Mount, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.Dismount:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.Dismount, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.AIControlOn:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.CommandDelegate, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case OrderType.AIControlOff:
//            this.Owner.MakeVoice(SkinVoiceManager.VoiceType.CommandUndelegate, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//        }
//      }
//      if (this._selectedFormations.Count > 0 && this.Owner != null && this.Owner.Controller != AgentControllerType.AI)
//      {
//        MissionWeapon wieldedWeapon = this.Owner.WieldedWeapon;
//        switch (wieldedWeapon.IsEmpty ? 0 : (int) wieldedWeapon.Item.PrimaryWeapon.WeaponClass)
//        {
//          case 0:
//          case 18:
//          case 19:
//          case 27:
//            if (this.Owner.MountAgent == null)
//            {
//              this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? (this.Owner.GetIsLeftStance() ? ActionIndexCache.act_command_follow_unarmed_leftstance : ActionIndexCache.act_command_follow_unarmed) : (this.Owner.GetIsLeftStance() ? ActionIndexCache.act_command_unarmed_leftstance : ActionIndexCache.act_command_unarmed));
//              break;
//            }
//            this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? ActionIndexCache.act_horse_command_follow_unarmed : ActionIndexCache.act_horse_command_unarmed);
//            break;
//          case 1:
//          case 2:
//          case 4:
//          case 6:
//          case 7:
//          case 9:
//          case 21:
//          case 22:
//            if (this.Owner.MountAgent == null)
//            {
//              this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? (this.Owner.GetIsLeftStance() ? ActionIndexCache.act_command_follow_leftstance : ActionIndexCache.act_command_follow) : (this.Owner.GetIsLeftStance() ? ActionIndexCache.act_command_leftstance : ActionIndexCache.act_command));
//              break;
//            }
//            this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? ActionIndexCache.act_horse_command_follow : ActionIndexCache.act_horse_command);
//            break;
//          case 3:
//          case 5:
//          case 8:
//          case 10:
//          case 11:
//          case 17:
//          case 23:
//          case 24:
//          case 25:
//            if (this.Owner.MountAgent == null)
//            {
//              this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? (this.Owner.GetIsLeftStance() ? ActionIndexCache.act_command_follow_2h_leftstance : ActionIndexCache.act_command_follow_2h) : (this.Owner.GetIsLeftStance() ? ActionIndexCache.act_command_2h_leftstance : ActionIndexCache.act_command_2h));
//              break;
//            }
//            this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? ActionIndexCache.act_horse_command_follow_2h : ActionIndexCache.act_horse_command_2h);
//            break;
//          case 16:
//            if (this.Owner.MountAgent == null)
//            {
//              this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? ActionIndexCache.act_command_follow_bow : ActionIndexCache.act_command_bow);
//              break;
//            }
//            this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? ActionIndexCache.act_horse_command_follow_bow : ActionIndexCache.act_horse_command_bow);
//            break;
//          case 20:
//          case 26:
//            break;
//          default:
//            TaleWorlds.Library.Debug.FailedAssert("Unexpected weapon class.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (PlayOrderGestures), 819);
//            break;
//        }
//      }
//      foreach (Formation selectedFormation in (List<Formation>) this._selectedFormations)
//      {
//        Agent medianAgent = selectedFormation.GetMedianAgent(false, true, selectedFormation.CachedAveragePosition);
//        if (medianAgent != null)
//        {
//          Vec3 position = medianAgent.Position;
//          switch (orderType)
//          {
//            case OrderType.Move:
//            case OrderType.MoveToLineSegment:
//            case OrderType.MoveToLineSegmentWithHorizontalLayout:
//            case OrderType.FollowMe:
//            case OrderType.AdvanceTenPaces:
//            case OrderType.Advance:
//              MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/nods/move"), position);
//              continue;
//            case OrderType.Charge:
//            case OrderType.ChargeWithTarget:
//            case OrderType.AttackEntity:
//              MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/nods/attack"), position);
//              if (this._yellingAfterChargeOrderTimer.Check(true))
//              {
//                selectedFormation.ApplyActionOnEachUnit((Action<Agent>) (yellingAgent => yellingAgent.YellingBehaviour()));
//                continue;
//              }
//              continue;
//            case OrderType.StandYourGround:
//              MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/nods/stop"), position);
//              continue;
//            case OrderType.ArrangementLine:
//            case OrderType.ArrangementCloseOrder:
//            case OrderType.ArrangementLoose:
//            case OrderType.ArrangementCircular:
//            case OrderType.ArrangementSchiltron:
//            case OrderType.ArrangementVee:
//            case OrderType.ArrangementColumn:
//            case OrderType.ArrangementScatter:
//              MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/nods/formation"), position);
//              continue;
//            default:
//              continue;
//          }
//        }
//      }
//    }

//    protected static void PlayFormationSelectedGesture(Formation formation, Agent agent)
//    {
//      if (formation.SecondaryLogicalClasses.Any<FormationClass>())
//      {
//        agent.MakeVoice(SkinVoiceManager.VoiceType.MixedFormation, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//      }
//      else
//      {
//        switch (formation.LogicalClass)
//        {
//          case FormationClass.Infantry:
//            agent.MakeVoice(SkinVoiceManager.VoiceType.Infantry, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case FormationClass.Ranged:
//            agent.MakeVoice(SkinVoiceManager.VoiceType.Archers, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case FormationClass.Cavalry:
//            agent.MakeVoice(SkinVoiceManager.VoiceType.Cavalry, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          case FormationClass.HorseArcher:
//            agent.MakeVoice(SkinVoiceManager.VoiceType.HorseArchers, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            break;
//          default:
//            TaleWorlds.Library.Debug.FailedAssert("false", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (PlayFormationSelectedGesture), 899);
//            break;
//        }
//      }
//    }

//    private void AfterSetOrder(OrderType orderType)
//    {
//      MBDebug.Print("After set order called, number of selected formations: " + (object) this.SelectedFormations.Count);
//      foreach (Formation selectedFormation in (List<Formation>) this.SelectedFormations)
//      {
//        MBDebug.Print(selectedFormation?.FormationIndex.ToString() + " formation being processed.");
//        if (this._formationUpdateEnabledAfterSetOrder)
//        {
//          bool flag = false;
//          if (selectedFormation.IsPlayerTroopInFormation)
//            flag = selectedFormation.GetReadonlyMovementOrderReference().OrderEnum == MovementOrder.MovementOrderEnum.Follow;
//          selectedFormation.ApplyActionOnEachUnit((Action<Agent>) (agent => agent.ForceUpdateCachedAndFormationValues(false, false)), flag ? Mission.Current.MainAgent : (Agent) null);
//          selectedFormation.SetHasPendingUnitPositions(false);
//        }
//        MBDebug.Print("Update cached and formation values on each agent complete, number of selected formations: " + (object) this.SelectedFormations.Count);
//        this._mission.SetRandomDecideTimeOfAgentsWithIndices(selectedFormation.CollectUnitIndices());
//        MBDebug.Print("Set random decide time of agents with indices complete, number of selected formations: " + (object) this.SelectedFormations.Count);
//      }
//      MBDebug.Print("After set order loop complete, number of selected formations: " + (object) this.SelectedFormations.Count);
//      if (this.Owner == null || !this.AreGesturesEnabled())
//        return;
//      this.PlayOrderGestures(orderType);
//    }

//    protected void BeforeSetOrder(OrderType orderType)
//    {
//      foreach (Formation formation in this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => !this.IsFormationSelectable(f, this.Owner))).ToList<Formation>())
//        this.DeselectFormation(formation);
//      if (GameNetwork.IsClientOrReplay || orderType == OrderType.AIControlOff || orderType == OrderType.AIControlOn)
//        return;
//      foreach (Formation selectedFormation in (List<Formation>) this.SelectedFormations)
//      {
//        if (selectedFormation.IsAIControlled && selectedFormation.PlayerOwner != null)
//          selectedFormation.SetControlledByAI(false);
//      }
//    }

//    public virtual void SetOrderWithAgent(OrderType orderType, Agent agent)
//    {
//      MBDebug.Print("SetOrderWithAgent " + (object) orderType + " " + agent.Name + "on team");
//      this.BeforeSetOrder(orderType);
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithAgent(orderType, agent.Index));
//        GameNetwork.EndModuleEventAsClient();
//      }
//      if (orderType == OrderType.FollowMe)
//      {
//        foreach (Formation selectedFormation in (List<Formation>) this.SelectedFormations)
//          selectedFormation.SetMovementOrder(MovementOrder.MovementOrderFollow(agent));
//      }
//      else
//        TaleWorlds.Library.Debug.FailedAssert("[DEBUG]Invalid order type.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (SetOrderWithAgent), 988);
//      this.AfterSetOrder(orderType);
//      this.FireOnOrderIssued(orderType, this.SelectedFormations, this, (object) agent);
//    }

//    public virtual void SetOrderWithPosition(OrderType orderType, WorldPosition orderPosition)
//    {
//      MBDebug.Print("SetOrderWithPosition " + (object) orderType + " " + (object) orderPosition + "on team");
//      this.BeforeSetOrder(orderType);
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithPosition(orderType, orderPosition.GetGroundVec3()));
//        GameNetwork.EndModuleEventAsClient();
//      }
//      switch (orderType)
//      {
//        case OrderType.Move:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetMovementOrder(MovementOrder.MovementOrderMove(orderPosition));
//            break;
//          }
//        case OrderType.LookAtDirection:
//          FacingOrder order = FacingOrder.FacingOrderLookAtDirection(OrderController.GetOrderLookAtDirection((IEnumerable<Formation>) this.SelectedFormations, orderPosition.AsVec2));
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetFacingOrder(order);
//            break;
//          }
//        case OrderType.FormCustom:
//          float orderFormCustomWidth = OrderController.GetOrderFormCustomWidth((IEnumerable<Formation>) this.SelectedFormations, orderPosition.GetGroundVec3());
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetFormOrder(FormOrder.FormOrderCustom(orderFormCustomWidth));
//            break;
//          }
//        default:
//          TaleWorlds.Library.Debug.FailedAssert("[DEBUG]Invalid order type.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (SetOrderWithPosition), 1038);
//          break;
//      }
//      this.AfterSetOrder(orderType);
//      this.FireOnOrderIssued(orderType, this.SelectedFormations, this, (object) orderPosition);
//    }

//    public virtual void SetOrderWithFormation(OrderType orderType, Formation orderFormation)
//    {
//      MBDebug.Print("SetOrderWithFormation " + (object) orderType + " " + (object) orderFormation + "on team");
//      this.BeforeSetOrder(orderType);
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithFormation(orderType, orderFormation.Index));
//        GameNetwork.EndModuleEventAsClient();
//      }
//      switch (orderType)
//      {
//        case OrderType.Charge:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              current.SetMovementOrder(MovementOrder.MovementOrderCharge);
//              current.SetTargetFormation(orderFormation);
//            }
//            break;
//          }
//        case OrderType.Advance:
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              current.SetMovementOrder(MovementOrder.MovementOrderAdvance);
//              current.SetTargetFormation(orderFormation);
//            }
//            break;
//          }
//        default:
//          TaleWorlds.Library.Debug.FailedAssert("Invalid order type", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (SetOrderWithFormation), 1078);
//          break;
//      }
//      this.AfterSetOrder(orderType);
//      this.FireOnOrderIssued(orderType, this.SelectedFormations, this, (object) orderFormation);
//    }

//    public void SetOrderWithFormationAndPercentage(
//      OrderType orderType,
//      Formation orderFormation,
//      float percentage)
//    {
//      int percentage1 = MBMath.ClampInt((int) ((double) percentage * 100.0), 0, 100);
//      this.BeforeSetOrder(orderType);
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithFormationAndPercentage(orderType, orderFormation.Index, percentage1));
//        GameNetwork.EndModuleEventAsClient();
//      }
//      TaleWorlds.Library.Debug.FailedAssert("Invalid order type", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (SetOrderWithFormationAndPercentage), 1116);
//      this.AfterSetOrder(orderType);
//      this.FireOnOrderIssued(orderType, this.SelectedFormations, this, (object) orderFormation, (object) percentage);
//    }

//    public void TransferUnitWithPriorityFunction(
//      Formation orderFormation,
//      int number,
//      bool hasShield,
//      bool hasSpear,
//      bool hasThrown,
//      bool isHeavy,
//      bool isRanged,
//      bool isMounted,
//      bool excludeBannerman,
//      List<Agent> excludedAgents)
//    {
//      Func<Agent, int> priorityFunc;
//      TroopFilteringUtilities.GetPriorityFunction(TroopFilteringUtilities.GetFilter(isMounted, isRanged, !isRanged, isHeavy, hasThrown, hasSpear, hasShield), out priorityFunc);
//      this.BeforeSetOrder(OrderType.Transfer);
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithFormationAndNumber(OrderType.Transfer, orderFormation.Index, number));
//        GameNetwork.EndModuleEventAsClient();
//      }
//      List<int> intList = (List<int>) null;
//      int num1 = this.SelectedFormations.Sum<Formation>((Func<Formation, int>) (f => f.CountOfUnits));
//      int num2 = number;
//      int num3 = 0;
//      if (this.SelectedFormations.Count > 1)
//        intList = new List<int>();
//      foreach (Formation selectedFormation in (List<Formation>) this.SelectedFormations)
//      {
//        int countOfUnits = selectedFormation.CountOfUnits;
//        int unitCount = num2 * countOfUnits / num1;
//        if (!GameNetwork.IsClientOrReplay)
//        {
//          selectedFormation.OnMassUnitTransferStart();
//          orderFormation.OnMassUnitTransferStart();
//          selectedFormation.TransferUnitsWithPriorityFunction(orderFormation, unitCount, priorityFunc, excludeBannerman, excludedAgents);
//          selectedFormation.OnMassUnitTransferEnd();
//          orderFormation.OnMassUnitTransferEnd();
//        }
//        intList?.Add(unitCount);
//        num2 -= unitCount;
//        num1 -= countOfUnits;
//        num3 += unitCount;
//      }
//      if (!GameNetwork.IsClientOrReplay)
//        orderFormation.QuerySystem.Expire();
//      this.AfterSetOrder(OrderType.Transfer);
//      if (this.OnOrderIssued == null)
//        return;
//      if (intList != null)
//      {
//        object[] objArray = new object[intList.Count + 1];
//        objArray[0] = (object) number;
//        for (int index = 0; index < intList.Count; ++index)
//          objArray[index + 1] = (object) intList[index];
//        this.FireOnOrderIssued(OrderType.Transfer, this.SelectedFormations, this, (object) orderFormation, (object) objArray);
//      }
//      else
//        this.FireOnOrderIssued(OrderType.Transfer, this.SelectedFormations, this, (object) orderFormation, (object) number);
//    }

//    public void RearrangeFormationsAccordingToFilters(
//      Team team,
//      List<(Formation formation, int troopCount, TroopTraitsMask troopFilter, List<Agent> excludedAgents)> MassTransferData)
//    {
//      team.RearrangeFormationsAccordingToFilter(MassTransferData);
//    }

//    public void SetOrderWithFormationAndNumber(
//      OrderType orderType,
//      Formation orderFormation,
//      int number)
//    {
//      this.BeforeSetOrder(orderType);
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithFormationAndNumber(orderType, orderFormation.Index, number));
//        GameNetwork.EndModuleEventAsClient();
//      }
//      List<int> intList = (List<int>) null;
//      if (orderType == OrderType.Transfer)
//      {
//        int num1 = this.SelectedFormations.Sum<Formation>((Func<Formation, int>) (f => f.CountOfUnits));
//        int num2 = number;
//        int num3 = 0;
//        if (this.SelectedFormations.Count > 1)
//          intList = new List<int>();
//        foreach (Formation selectedFormation in (List<Formation>) this.SelectedFormations)
//        {
//          int countOfUnits = selectedFormation.CountOfUnits;
//          int unitCount = num2 * countOfUnits / num1;
//          if (!GameNetwork.IsClientOrReplay)
//          {
//            selectedFormation.OnMassUnitTransferStart();
//            orderFormation.OnMassUnitTransferStart();
//            selectedFormation.TransferUnitsAux(orderFormation, unitCount, true, unitCount < countOfUnits && orderFormation.CountOfUnits > 0 && orderFormation.OrderPositionIsValid);
//            selectedFormation.OnMassUnitTransferEnd();
//            orderFormation.OnMassUnitTransferEnd();
//          }
//          intList?.Add(unitCount);
//          num2 -= unitCount;
//          num1 -= countOfUnits;
//          num3 += unitCount;
//        }
//        if (!GameNetwork.IsClientOrReplay)
//          orderFormation.QuerySystem.Expire();
//      }
//      else
//        TaleWorlds.Library.Debug.FailedAssert("[DEBUG]Invalid order type.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (SetOrderWithFormationAndNumber), 1358);
//      this.AfterSetOrder(orderType);
//      if (this.OnOrderIssued == null)
//        return;
//      if (intList != null)
//      {
//        object[] objArray = new object[intList.Count + 1];
//        objArray[0] = (object) number;
//        for (int index = 0; index < intList.Count; ++index)
//          objArray[index + 1] = (object) intList[index];
//        this.FireOnOrderIssued(orderType, this.SelectedFormations, this, (object) orderFormation, (object) objArray);
//      }
//      else
//        this.FireOnOrderIssued(orderType, this.SelectedFormations, this, (object) orderFormation, (object) number);
//    }

//    public virtual void SetOrderWithTwoPositions(
//      OrderType orderType,
//      WorldPosition position1,
//      WorldPosition position2)
//    {
//      MBDebug.Print("SetOrderWithTwoPositions " + (object) orderType + " " + (object) position1 + " " + (object) position2 + "on team");
//      this.BeforeSetOrder(orderType);
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithTwoPositions(orderType, position1.GetGroundVec3(), position2.GetGroundVec3()));
//        GameNetwork.EndModuleEventAsClient();
//      }
//      switch (orderType)
//      {
//        case OrderType.MoveToLineSegment:
//        case OrderType.MoveToLineSegmentWithHorizontalLayout:
//          bool isFormationLayoutVertical = orderType == OrderType.MoveToLineSegment;
//          IEnumerable<Formation> formations = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
//          if (formations.Any<Formation>())
//          {
//            this.MoveToLineSegment(formations, position1, position2, isFormationLayoutVertical);
//            break;
//          }
//          break;
//        default:
//          TaleWorlds.Library.Debug.FailedAssert("Invalid order type.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (SetOrderWithTwoPositions), 1412);
//          break;
//      }
//      this.AfterSetOrder(orderType);
//      this.FireOnOrderIssued(orderType, this.SelectedFormations, this, (object) position1, (object) position2);
//    }

//    public virtual void SetOrderWithOrderableObject(IOrderable target)
//    {
//      BattleSideEnum side = this.SelectedFormations[0].Team.Side;
//      OrderType order = target.GetOrder(side);
//      this.BeforeSetOrder(order);
//      MissionObject missionObject = target as MissionObject;
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithMissionObject(missionObject.Id));
//        GameNetwork.EndModuleEventAsClient();
//      }
//      switch (order)
//      {
//        case OrderType.Move:
//          WorldPosition position = new WorldPosition(this._mission.Scene, UIntPtr.Zero, missionObject.GameEntity.GlobalPosition, false);
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetMovementOrder(MovementOrder.MovementOrderMove(position));
//            break;
//          }
//        case OrderType.MoveToLineSegment:
//          IPointDefendable pointDefendable1 = target as IPointDefendable;
//          Vec3 globalPosition1 = pointDefendable1.DefencePoints.Last<DefencePoint>().GameEntity.GlobalPosition;
//          Vec3 globalPosition2 = pointDefendable1.DefencePoints.First<DefencePoint>().GameEntity.GlobalPosition;
//          IEnumerable<Formation> formations1 = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
//          if (formations1.Any<Formation>())
//          {
//            WorldPosition TargetLineSegmentBegin = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition1, false);
//            WorldPosition TargetLineSegmentEnd = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition2, false);
//            this.MoveToLineSegment(formations1, TargetLineSegmentBegin, TargetLineSegmentEnd);
//            break;
//          }
//          break;
//        case OrderType.MoveToLineSegmentWithHorizontalLayout:
//          IPointDefendable pointDefendable2 = target as IPointDefendable;
//          Vec3 globalPosition3 = pointDefendable2.DefencePoints.Last<DefencePoint>().GameEntity.GlobalPosition;
//          Vec3 globalPosition4 = pointDefendable2.DefencePoints.First<DefencePoint>().GameEntity.GlobalPosition;
//          IEnumerable<Formation> formations2 = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
//          if (formations2.Any<Formation>())
//          {
//            WorldPosition TargetLineSegmentBegin = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition3, false);
//            WorldPosition TargetLineSegmentEnd = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition4, false);
//            this.MoveToLineSegment(formations2, TargetLineSegmentBegin, TargetLineSegmentEnd, false);
//            break;
//          }
//          break;
//        case OrderType.FollowEntity:
//          GameEntity waitEntity = (target as UsableMachine).WaitEntity;
//          Vec2 direction = waitEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized();
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Formation current = enumerator.Current;
//              current.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection(direction));
//              current.SetMovementOrder(MovementOrder.MovementOrderFollowEntity(waitEntity));
//            }
//            break;
//          }
//        case OrderType.Use:
//          this.ToggleSideOrderUse((IEnumerable<Formation>) this.SelectedFormations, target as UsableMachine);
//          break;
//        case OrderType.AttackEntity:
//          WeakGameEntity gameEntity = missionObject.GameEntity;
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetMovementOrder(MovementOrder.MovementOrderAttackEntity(GameEntity.CreateFromWeakEntity(gameEntity), !(missionObject is CastleGate)));
//            break;
//          }
//        case OrderType.PointDefence:
//          IPointDefendable pointDefendable3 = target as IPointDefendable;
//          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//              enumerator.Current.SetMovementOrder(MovementOrder.MovementOrderMove(pointDefendable3.MiddleFrame.Origin));
//            break;
//          }
//      }
//      this.AfterSetOrder(order);
//      this.FireOnOrderIssued(order, this.SelectedFormations, this, (object) target);
//    }

//    public static OrderType GetActiveMovementOrderOf(Formation formation)
//    {
//      switch (formation.GetReadonlyMovementOrderReference().MovementState)
//      {
//        case MovementOrder.MovementStateEnum.Charge:
//          return OrderType.Charge;
//        case MovementOrder.MovementStateEnum.Hold:
//          switch (formation.GetReadonlyMovementOrderReference().OrderType)
//          {
//            case OrderType.ChargeWithTarget:
//              return OrderType.Charge;
//            case OrderType.FollowMe:
//              return OrderType.FollowMe;
//            case OrderType.Advance:
//              return OrderType.Advance;
//            case OrderType.FallBack:
//              return OrderType.FallBack;
//            default:
//              return OrderType.Move;
//          }
//        case MovementOrder.MovementStateEnum.Retreat:
//          return OrderType.Retreat;
//        case MovementOrder.MovementStateEnum.StandGround:
//          return OrderType.StandYourGround;
//        default:
//          TaleWorlds.Library.Debug.FailedAssert("false", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (GetActiveMovementOrderOf), 1565);
//          return OrderType.Move;
//      }
//    }

//    public static OrderType GetActiveFacingOrderOf(Formation formation)
//    {
//      return formation.FacingOrder.OrderType == OrderType.LookAtDirection ? OrderType.LookAtDirection : OrderType.LookAtEnemy;
//    }

//    public static OrderType GetActiveRidingOrderOf(Formation formation)
//    {
//      OrderType orderType = formation.RidingOrder.OrderType;
//      return orderType == OrderType.RideFree ? OrderType.Mount : orderType;
//    }

//    public static OrderType GetActiveArrangementOrderOf(Formation formation)
//    {
//      return formation.ArrangementOrder.OrderType;
//    }

//    public static OrderType GetActiveFormOrderOf(Formation formation)
//    {
//      return formation.FormOrder.OrderType;
//    }

//    public static OrderType GetActiveFiringOrderOf(Formation formation)
//    {
//      return formation.FiringOrder.OrderType;
//    }

//    public static OrderType GetActiveAIControlOrderOf(Formation formation)
//    {
//      return formation.IsAIControlled ? OrderType.AIControlOn : OrderType.AIControlOff;
//    }

//    public void SimulateNewOrderWithPositionAndDirection(
//      WorldPosition formationLineBegin,
//      WorldPosition formationLineEnd,
//      out List<WorldPosition> simulationAgentFrames,
//      bool isFormationLayoutVertical)
//    {
//      IEnumerable<Formation> formations = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
//      if (formations.Any<Formation>())
//        OrderController.SimulateNewOrderWithPositionAndDirection(formations, this.simulationFormations, formationLineBegin, formationLineEnd, out simulationAgentFrames, isFormationLayoutVertical);
//      else
//        simulationAgentFrames = new List<WorldPosition>();
//    }

//    public void SimulateNewFacingOrder(
//      Vec2 direction,
//      out List<WorldPosition> simulationAgentFrames)
//    {
//      IEnumerable<Formation> formations = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
//      if (formations.Any<Formation>())
//        OrderController.SimulateNewFacingOrder(formations, this.simulationFormations, direction, out simulationAgentFrames);
//      else
//        simulationAgentFrames = new List<WorldPosition>();
//    }

//    public void SimulateNewCustomWidthOrder(
//      float width,
//      out List<WorldPosition> simulationAgentFrames)
//    {
//      IEnumerable<Formation> formations = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
//      if (formations.Any<Formation>())
//        OrderController.SimulateNewCustomWidthOrder(formations, this.simulationFormations, width, out simulationAgentFrames);
//      else
//        simulationAgentFrames = new List<WorldPosition>();
//    }

//    private static void SimulateNewOrderWithPositionAndDirectionAux(
//      IEnumerable<Formation> formations,
//      Dictionary<Formation, Formation> simulationFormations,
//      WorldPosition formationLineBegin,
//      WorldPosition formationLineEnd,
//      bool isSimulatingAgentFrames,
//      out List<WorldPosition> simulationAgentFrames,
//      bool isSimulatingFormationChanges,
//      out List<(Formation, int, float, WorldPosition, Vec2)> simulationFormationChanges,
//      out bool isLineShort,
//      bool isFormationLayoutVertical = true)
//    {
//      float length = (formationLineEnd.AsVec2 - formationLineBegin.AsVec2).Length;
//      isLineShort = false;
//      if ((double) length < (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius))
//      {
//        isLineShort = true;
//      }
//      else
//      {
//        float num = !isFormationLayoutVertical ? formations.Max<Formation>((Func<Formation, float>) (f => f.Width)) : formations.Sum<Formation>((Func<Formation, float>) (f => f.MinimumWidth)) + (float) (formations.Count<Formation>() - 1) * 1.5f;
//        if ((double) length < (double) num)
//          isLineShort = true;
//      }
//      if (isLineShort)
//      {
//        float num1 = !isFormationLayoutVertical ? formations.Max<Formation>((Func<Formation, float>) (f => f.Width)) : formations.Sum<Formation>((Func<Formation, float>) (f => f.Width)) + (float) (formations.Count<Formation>() - 1) * 1.5f;
//        Vec2 direction = formations.MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes)).Direction;
//        direction.RotateCCW(-1.57079637f);
//        double num2 = (double) direction.Normalize();
//        formationLineEnd = Mission.Current.GetStraightPathToTarget(formationLineBegin.AsVec2 + num1 / 2f * direction, formationLineBegin);
//        formationLineBegin = Mission.Current.GetStraightPathToTarget(formationLineBegin.AsVec2 - num1 / 2f * direction, formationLineBegin);
//      }
//      else
//        formationLineEnd = Mission.Current.GetStraightPathToTarget(formationLineEnd.AsVec2, formationLineBegin);
//      if (isFormationLayoutVertical)
//        OrderController.SimulateNewOrderWithVerticalLayout(formations, simulationFormations, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, out simulationAgentFrames, isSimulatingFormationChanges, out simulationFormationChanges);
//      else
//        OrderController.SimulateNewOrderWithHorizontalLayout(formations, simulationFormations, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, out simulationAgentFrames, isSimulatingFormationChanges, out simulationFormationChanges);
//    }

//    private static Formation GetSimulationFormation(
//      Formation formation,
//      Dictionary<Formation, Formation> simulationFormations)
//    {
//      return simulationFormations?[formation];
//    }

//    private static void SimulateNewFacingOrder(
//      IEnumerable<Formation> formations,
//      Dictionary<Formation, Formation> simulationFormations,
//      Vec2 direction,
//      out List<WorldPosition> simulationAgentFrames)
//    {
//      simulationAgentFrames = new List<WorldPosition>();
//      foreach (Formation formation in formations)
//      {
//        float width = formation.Width;
//        WorldPosition formationPosition = formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
//        int unitSpacingReduction = 0;
//        OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, OrderController.GetSimulationFormation(formation, simulationFormations), in formationPosition, in direction, ref width, ref unitSpacingReduction);
//        OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, (List<(Formation, int, float, WorldPosition, Vec2)>) null, in formationPosition, in direction, width, unitSpacingReduction, false, out float _);
//      }
//    }

//    private static void SimulateNewCustomWidthOrder(
//      IEnumerable<Formation> formations,
//      Dictionary<Formation, Formation> simulationFormations,
//      float width,
//      out List<WorldPosition> simulationAgentFrames)
//    {
//      simulationAgentFrames = new List<WorldPosition>();
//      foreach (Formation formation1 in formations)
//      {
//        float num1 = MathF.Min(width, formation1.MaximumWidth);
//        Mat3 identity = Mat3.Identity;
//        ref Mat3 local1 = ref identity;
//        Vec2 direction = formation1.Direction;
//        Vec3 vec3 = direction.ToVec3();
//        local1.f = vec3;
//        identity.Orthonormalize();
//        WorldPosition orderWorldPosition = formation1.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
//        int num2 = 0;
//        Formation formation2 = formation1;
//        Formation simulationFormation1 = OrderController.GetSimulationFormation(formation1, simulationFormations);
//        ref WorldPosition local2 = ref orderWorldPosition;
//        direction = formation1.Direction;
//        ref Vec2 local3 = ref direction;
//        ref float local4 = ref num1;
//        ref int local5 = ref num2;
//        OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation2, simulationFormation1, in local2, in local3, ref local4, ref local5);
//        int count = simulationAgentFrames.Count;
//        Formation formation3 = formation1;
//        Formation simulationFormation2 = OrderController.GetSimulationFormation(formation1, simulationFormations);
//        List<WorldPosition> simulationAgentFrames1 = simulationAgentFrames;
//        ref WorldPosition local6 = ref orderWorldPosition;
//        direction = formation1.Direction;
//        ref Vec2 local7 = ref direction;
//        double formationWidth1 = (double) num1;
//        int unitSpacingReduction1 = num2;
//        float num3;
//        ref float local8 = ref num3;
//        OrderController.SimulateNewOrderWithFrameAndWidth(formation3, simulationFormation2, simulationAgentFrames1, (List<(Formation, int, float, WorldPosition, Vec2)>) null, in local6, in local7, (float) formationWidth1, unitSpacingReduction1, false, out local8);
//        float lesserThanActualWidth = Formation.GetLastSimulatedFormationsOccupationWidthIfLesserThanActualWidth(OrderController.GetSimulationFormation(formation1, simulationFormations));
//        if ((double) lesserThanActualWidth > 0.0)
//        {
//          simulationAgentFrames.RemoveRange(count, simulationAgentFrames.Count - count);
//          Formation formation4 = formation1;
//          Formation simulationFormation3 = OrderController.GetSimulationFormation(formation1, simulationFormations);
//          List<WorldPosition> simulationAgentFrames2 = simulationAgentFrames;
//          ref WorldPosition local9 = ref orderWorldPosition;
//          direction = formation1.Direction;
//          ref Vec2 local10 = ref direction;
//          double formationWidth2 = (double) lesserThanActualWidth;
//          int unitSpacingReduction2 = num2;
//          ref float local11 = ref num3;
//          OrderController.SimulateNewOrderWithFrameAndWidth(formation4, simulationFormation3, simulationAgentFrames2, (List<(Formation, int, float, WorldPosition, Vec2)>) null, in local9, in local10, (float) formationWidth2, unitSpacingReduction2, false, out local11);
//        }
//      }
//    }

//    public static void SimulateNewOrderWithPositionAndDirection(
//      IEnumerable<Formation> formations,
//      Dictionary<Formation, Formation> simulationFormations,
//      WorldPosition formationLineBegin,
//      WorldPosition formationLineEnd,
//      out List<WorldPosition> simulationAgentFrames,
//      bool isFormationLayoutVertical = true)
//    {
//      OrderController.SimulateNewOrderWithPositionAndDirectionAux(formations, simulationFormations, formationLineBegin, formationLineEnd, true, out simulationAgentFrames, false, out List<(Formation, int, float, WorldPosition, Vec2)> _, out bool _, isFormationLayoutVertical);
//    }

//    public static void SimulateNewOrderWithPositionAndDirection(
//      IEnumerable<Formation> formations,
//      Dictionary<Formation, Formation> simulationFormations,
//      WorldPosition formationLineBegin,
//      WorldPosition formationLineEnd,
//      out List<(Formation, int, float, WorldPosition, Vec2)> formationChanges,
//      out bool isLineShort,
//      bool isFormationLayoutVertical = true)
//    {
//      OrderController.SimulateNewOrderWithPositionAndDirectionAux(formations, simulationFormations, formationLineBegin, formationLineEnd, false, out List<WorldPosition> _, true, out formationChanges, out isLineShort, isFormationLayoutVertical);
//    }

//    private static void SimulateNewOrderWithVerticalLayout(
//      IEnumerable<Formation> formations,
//      Dictionary<Formation, Formation> simulationFormations,
//      WorldPosition formationLineBegin,
//      WorldPosition formationLineEnd,
//      bool isSimulatingAgentFrames,
//      out List<WorldPosition> simulationAgentFrames,
//      bool isSimulatingFormationChanges,
//      out List<(Formation, int, float, WorldPosition, Vec2)> simulationFormationChanges)
//    {
//      simulationAgentFrames = !isSimulatingAgentFrames ? (List<WorldPosition>) null : new List<WorldPosition>();
//      simulationFormationChanges = !isSimulatingFormationChanges ? (List<(Formation, int, float, WorldPosition, Vec2)>) null : new List<(Formation, int, float, WorldPosition, Vec2)>();
//      Vec2 vec2 = formationLineEnd.AsVec2 - formationLineBegin.AsVec2;
//      float length = vec2.Length;
//      double num1 = (double) vec2.Normalize();
//      float f1 = MathF.Max(0.0f, length - (float) (formations.Count<Formation>() - 1) * 1.5f);
//      float comparedValue = formations.Sum<Formation>((Func<Formation, float>) (f => f.Width));
//      bool flag = f1.ApproximatelyEqualsTo(comparedValue, 0.1f);
//      float num2 = formations.Sum<Formation>((Func<Formation, float>) (f => f.MinimumWidth));
//      formations.Count<Formation>();
//      Vec2 formationDirection = new Vec2(-vec2.y, vec2.x).Normalized();
//      float num3 = 0.0f;
//      foreach (Formation formation in formations)
//      {
//        float minimumWidth = formation.MinimumWidth;
//        float formationWidth = MathF.Min(flag ? formation.Width : MathF.Min((double) f1 < (double) comparedValue ? formation.Width : float.MaxValue, f1 * (minimumWidth / num2)), formation.MaximumWidth);
//        WorldPosition formationPosition = formationLineBegin;
//        formationPosition.SetVec2(formationPosition.AsVec2 + vec2 * (formationWidth * 0.5f + num3));
//        int unitSpacingReduction = 0;
//        OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, OrderController.GetSimulationFormation(formation, simulationFormations), in formationPosition, in formationDirection, ref formationWidth, ref unitSpacingReduction);
//        OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, in formationPosition, in formationDirection, formationWidth, unitSpacingReduction, false, out float _);
//        num3 += formationWidth + 1.5f;
//      }
//    }

//    private static void DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(
//      Formation formation,
//      Formation simulationFormation,
//      in WorldPosition formationPosition,
//      in Vec2 formationDirection,
//      ref float formationWidth,
//      ref int unitSpacingReduction)
//    {
//      if (simulationFormation.UnitSpacing != formation.UnitSpacing)
//        simulationFormation = new Formation((Team) null, -1);
//      int unitIndex = formation.CountOfUnitsWithoutDetachedOnes - 1;
//      float actualWidth = formationWidth;
//      do
//      {
//        WorldPosition? unitSpawnPosition;
//        formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, in formationPosition, in formationDirection, formationWidth, formation.UnitSpacing - unitSpacingReduction, out unitSpawnPosition, out Vec2? _, out actualWidth);
//        if (!unitSpawnPosition.HasValue)
//          ++unitSpacingReduction;
//        else
//          break;
//      }
//      while (formation.UnitSpacing - unitSpacingReduction >= 0);
//      unitSpacingReduction = MathF.Min(unitSpacingReduction, formation.UnitSpacing);
//      if (unitSpacingReduction <= 0)
//        return;
//      formationWidth = actualWidth;
//    }

//    private static float GetGapBetweenLinesOfFormation(Formation f, float unitSpacing)
//    {
//      float num1 = 0.0f;
//      float num2 = 0.2f;
//      if (f.HasAnyMountedUnit && !(f.RidingOrder == RidingOrder.RidingOrderDismount))
//      {
//        num1 = 2f;
//        num2 = 0.6f;
//      }
//      return num1 + unitSpacing * num2;
//    }

//    private static void SimulateNewOrderWithHorizontalLayout(
//      IEnumerable<Formation> formations,
//      Dictionary<Formation, Formation> simulationFormations,
//      WorldPosition formationLineBegin,
//      WorldPosition formationLineEnd,
//      bool isSimulatingAgentFrames,
//      out List<WorldPosition> simulationAgentFrames,
//      bool isSimulatingFormationChanges,
//      out List<(Formation, int, float, WorldPosition, Vec2)> simulationFormationChanges)
//    {
//      simulationAgentFrames = !isSimulatingAgentFrames ? (List<WorldPosition>) null : new List<WorldPosition>();
//      simulationFormationChanges = !isSimulatingFormationChanges ? (List<(Formation, int, float, WorldPosition, Vec2)>) null : new List<(Formation, int, float, WorldPosition, Vec2)>();
//      Vec2 vec2 = formationLineEnd.AsVec2 - formationLineBegin.AsVec2;
//      float a = vec2.Normalize();
//      float num1 = formations.Max<Formation>((Func<Formation, float>) (f => f.MinimumWidth));
//      if ((double) a < (double) num1)
//        a = num1;
//      Vec2 formationDirection = new Vec2(-vec2.y, vec2.x).Normalized();
//      float num2 = 0.0f;
//      foreach (Formation formation in formations)
//      {
//        float formationWidth = MathF.Min(a, formation.MaximumWidth);
//        WorldPosition formationPosition = formationLineBegin;
//        formationPosition.SetVec2((formationLineEnd.AsVec2 + formationLineBegin.AsVec2) * 0.5f - formationDirection * num2);
//        int unitSpacingReduction = 0;
//        OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, OrderController.GetSimulationFormation(formation, simulationFormations), in formationPosition, in formationDirection, ref formationWidth, ref unitSpacingReduction);
//        float simulatedFormationDepth;
//        OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, in formationPosition, in formationDirection, formationWidth, unitSpacingReduction, true, out simulatedFormationDepth);
//        num2 += simulatedFormationDepth + OrderController.GetGapBetweenLinesOfFormation(formation, (float) (formation.UnitSpacing - unitSpacingReduction));
//      }
//    }

//    private static void SimulateNewOrderWithFrameAndWidth(
//      Formation formation,
//      Formation simulationFormation,
//      List<WorldPosition> simulationAgentFrames,
//      List<(Formation, int, float, WorldPosition, Vec2)> simulationFormationChanges,
//      in WorldPosition formationPosition,
//      in Vec2 formationDirection,
//      float formationWidth,
//      int unitSpacingReduction,
//      bool simulateFormationDepth,
//      out float simulatedFormationDepth)
//    {
//      int unitIndex = 0;
//      float num = simulateFormationDepth ? 0.0f : float.NaN;
//      bool flag = Mission.Current.Mode != MissionMode.Deployment || Mission.Current.IsOrderPositionAvailable(in formationPosition, formation.Team);
//      foreach (Agent agent in (IEnumerable<Agent>) formation.GetUnitsWithoutDetachedOnes().OrderBy<Agent, int>((Func<Agent, int>) (u => MBCommon.Hash(u.Index, (object) u))))
//      {
//        WorldPosition? unitSpawnPosition = new WorldPosition?();
//        Vec2? unitSpawnDirection = new Vec2?();
//        if (flag)
//        {
//          formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, in formationPosition, in formationDirection, formationWidth, formation.UnitSpacing - unitSpacingReduction, out unitSpawnPosition, out unitSpawnDirection);
//        }
//        else
//        {
//          unitSpawnPosition = new WorldPosition?(agent.GetWorldPosition());
//          unitSpawnDirection = new Vec2?(agent.GetMovementDirection());
//        }
//        if (unitSpawnPosition.HasValue)
//        {
//          simulationAgentFrames?.Add(unitSpawnPosition.Value);
//          if (simulateFormationDepth)
//          {
//            WorldPosition worldPosition = formationPosition;
//            Vec2 asVec2_1 = worldPosition.AsVec2;
//            worldPosition = formationPosition;
//            Vec2 line2 = worldPosition.AsVec2 + formationDirection.RightVec();
//            worldPosition = unitSpawnPosition.Value;
//            Vec2 asVec2_2 = worldPosition.AsVec2;
//            float line = Vec2.DistanceToLine(asVec2_1, line2, asVec2_2);
//            if ((double) line > (double) num)
//              num = line;
//          }
//        }
//        ++unitIndex;
//      }
//      if (flag)
//      {
//        simulationFormationChanges?.Add(ValueTuple.Create<Formation, int, float, WorldPosition, Vec2>(formation, unitSpacingReduction, formationWidth, formationPosition, formationDirection));
//      }
//      else
//      {
//        WorldPosition orderWorldPosition = formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
//        simulationFormationChanges?.Add(ValueTuple.Create<Formation, int, float, WorldPosition, Vec2>(formation, unitSpacingReduction, formationWidth, orderWorldPosition, formation.Direction));
//      }
//      simulatedFormationDepth = num + formation.UnitDiameter;
//    }

//    public void SimulateDestinationFrames(
//      out List<WorldPosition> simulationAgentFrames,
//      float minDistance = 3f)
//    {
//      MBReadOnlyList<Formation> selectedFormations = this.SelectedFormations;
//      simulationAgentFrames = new List<WorldPosition>(100);
//      float minDistanceSq = minDistance * minDistance;
//      foreach (Formation formation in (List<Formation>) selectedFormations)
//        formation.ApplyActionOnEachUnit((Action<Agent, List<WorldPosition>>) ((agent, localSimulationAgentFrames) =>
//        {
//          WorldPosition worldPosition = !this._mission.IsTeleportingAgents || agent.CanTeleport() ? agent.Formation.GetOrderPositionOfUnit(agent) : agent.GetWorldPosition();
//          bool flag = worldPosition.IsValid;
//          if (!GameNetwork.IsMultiplayer && this._mission.Mode == MissionMode.Deployment && !this._mission.IsNavalBattle)
//          {
//            IMissionDeploymentPlan deploymentPlan = agent.Mission.DeploymentPlan;
//            if (deploymentPlan.SupportsNavmesh())
//              deploymentPlan.ProjectPositionToDeploymentBoundaries(agent.Formation.Team, ref worldPosition);
//            flag = this._mission.IsFormationUnitPositionAvailable(ref worldPosition, agent.Formation.Team);
//          }
//          if (!flag || (double) agent.Position.AsVec2.DistanceSquared(worldPosition.AsVec2) < (double) minDistanceSq)
//            return;
//          localSimulationAgentFrames.Add(worldPosition);
//        }), simulationAgentFrames);
//    }

//    private void ToggleSideOrderUse(IEnumerable<Formation> formations, UsableMachine usable)
//    {
//      IEnumerable<Formation> source = formations.Where<Formation>(new Func<Formation, bool>(usable.IsUsedByFormation));
//      if (source.IsEmpty<Formation>())
//      {
//        foreach (Formation formation in formations)
//          formation.StartUsingMachine(usable, true);
//        if (!usable.HasWaitFrame)
//          return;
//        foreach (Formation formation in formations)
//          formation.SetMovementOrder(MovementOrder.MovementOrderFollowEntity(usable.WaitEntity));
//      }
//      else
//      {
//        foreach (Formation formation in source)
//          formation.StopUsingMachine(usable, true);
//      }
//    }

//    private static int GetLineOrderByClass(FormationClass formationClass)
//    {
//      return Array.IndexOf<FormationClass>(new FormationClass[8]
//      {
//        FormationClass.HeavyInfantry,
//        FormationClass.Infantry,
//        FormationClass.HeavyCavalry,
//        FormationClass.Cavalry,
//        FormationClass.LightCavalry,
//        FormationClass.NumberOfDefaultFormations,
//        FormationClass.Ranged,
//        FormationClass.HorseArcher
//      }, formationClass);
//    }

//    public static IEnumerable<Formation> SortFormationsForHorizontalLayout(
//      IEnumerable<Formation> formations)
//    {
//      return (IEnumerable<Formation>) formations.OrderBy<Formation, int>((Func<Formation, int>) (f => OrderController.GetLineOrderByClass(f.FormationIndex)));
//    }

//    private static IEnumerable<Formation> GetSortedFormations(
//      IEnumerable<Formation> formations,
//      bool isFormationLayoutVertical)
//    {
//      return isFormationLayoutVertical ? formations : OrderController.SortFormationsForHorizontalLayout(formations);
//    }

//    private void MoveToLineSegment(
//      IEnumerable<Formation> formations,
//      WorldPosition TargetLineSegmentBegin,
//      WorldPosition TargetLineSegmentEnd,
//      bool isFormationLayoutVertical = true)
//    {
//      foreach (Formation formation in formations)
//      {
//        int num;
//        if (this.actualUnitSpacings.TryGetValue(formation, out num))
//          formation.SetPositioning(unitSpacing: new int?(num));
//        float customWidth;
//        if (this.actualWidths.TryGetValue(formation, out customWidth))
//          formation.SetFormOrder(FormOrder.FormOrderCustom(customWidth));
//      }
//      formations = OrderController.GetSortedFormations(formations, isFormationLayoutVertical);
//      List<(Formation, int, float, WorldPosition, Vec2)> formationChanges;
//      bool isLineShort;
//      OrderController.SimulateNewOrderWithPositionAndDirection(formations, this.simulationFormations, TargetLineSegmentBegin, TargetLineSegmentEnd, out formationChanges, out isLineShort, isFormationLayoutVertical);
//      if (!formations.Any<Formation>())
//        return;
//      foreach ((Formation key, int num1, float customWidth, WorldPosition position, Vec2 direction) in formationChanges)
//      {
//        int unitSpacing = key.UnitSpacing;
//        float width = key.Width;
//        if (num1 > 0)
//        {
//          int num2 = MathF.Max(key.UnitSpacing - num1, 0);
//          key.SetPositioning(unitSpacing: new int?(num2));
//          if (key.UnitSpacing != unitSpacing)
//            this.actualUnitSpacings[key] = unitSpacing;
//        }
//        if ((double) key.Width != (double) customWidth && key.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Column)
//        {
//          key.SetFormOrder(FormOrder.FormOrderCustom(customWidth));
//          if (isLineShort)
//            this.actualWidths[key] = width;
//        }
//        if (!isLineShort)
//        {
//          key.SetMovementOrder(MovementOrder.MovementOrderMove(position));
//          key.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection(direction));
//          key.SetFormOrder(FormOrder.FormOrderCustom(customWidth));
//          MBList<Formation> mbList = new MBList<Formation>();
//          mbList.Add(key);
//          MBList<Formation> appliedFormations = mbList;
//          this.FireOnOrderIssued(OrderType.Move, (MBReadOnlyList<Formation>) appliedFormations, this, (object) position);
//          this.FireOnOrderIssued(OrderType.LookAtDirection, (MBReadOnlyList<Formation>) appliedFormations, this, (object) direction);
//          this.FireOnOrderIssued(OrderType.FormCustom, (MBReadOnlyList<Formation>) appliedFormations, this, (object) customWidth);
//        }
//        else
//        {
//          Formation formation = formations.MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes));
//          switch (OrderController.GetActiveFacingOrderOf(formation))
//          {
//            case OrderType.LookAtEnemy:
//              key.SetMovementOrder(MovementOrder.MovementOrderMove(position));
//              MBList<Formation> mbList1 = new MBList<Formation>();
//              mbList1.Add(key);
//              MBList<Formation> appliedFormations1 = mbList1;
//              this.FireOnOrderIssued(OrderType.Move, (MBReadOnlyList<Formation>) appliedFormations1, this, (object) position);
//              this.FireOnOrderIssued(OrderType.LookAtEnemy, (MBReadOnlyList<Formation>) appliedFormations1, this);
//              continue;
//            case OrderType.LookAtDirection:
//              key.SetMovementOrder(MovementOrder.MovementOrderMove(position));
//              key.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection(formation.Direction));
//              MBList<Formation> mbList2 = new MBList<Formation>();
//              mbList2.Add(key);
//              MBList<Formation> appliedFormations2 = mbList2;
//              this.FireOnOrderIssued(OrderType.Move, (MBReadOnlyList<Formation>) appliedFormations2, this, (object) position);
//              this.FireOnOrderIssued(OrderType.LookAtDirection, (MBReadOnlyList<Formation>) appliedFormations2, this, (object) formation.Direction);
//              continue;
//            default:
//              TaleWorlds.Library.Debug.FailedAssert("false", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (MoveToLineSegment), 2379);
//              continue;
//          }
//        }
//      }
//    }

//    public static Vec2 GetOrderLookAtDirection(IEnumerable<Formation> formations, Vec2 target)
//    {
//      if (!formations.Any<Formation>())
//      {
//        TaleWorlds.Library.Debug.FailedAssert("false", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\OrderController.cs", nameof (GetOrderLookAtDirection), 2399);
//        return Vec2.One;
//      }
//      Formation formation = formations.MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes));
//      return (target - formation.OrderPosition).Normalized();
//    }

//    public static float GetOrderFormCustomWidth(
//      IEnumerable<Formation> formations,
//      Vec3 orderPosition)
//    {
//      return (Agent.Main.Position - orderPosition).Length;
//    }

//    public void TransferUnits(Formation source, Formation target, int count)
//    {
//      source.TransferUnitsAux(target, count, false, count < source.CountOfUnits && target.CountOfUnits > 0);
//      MBList<Formation> appliedFormations = new MBList<Formation>();
//      appliedFormations.Add(source);
//      this.FireOnOrderIssued(OrderType.Transfer, (MBReadOnlyList<Formation>) appliedFormations, this, (object) target, (object) count);
//    }

//    public IEnumerable<Formation> SplitFormation(Formation formation, int count = 2)
//    {
//      if (!formation.IsSplittableByAI || formation.CountOfUnitsWithoutDetachedOnes < count)
//        return (IEnumerable<Formation>) new List<Formation>()
//        {
//          formation
//        };
//      MBDebug.Print((formation.Team.Side == BattleSideEnum.Attacker ? (object) "Attacker team" : (object) "Defender team").ToString() + " formation " + (object) (int) formation.FormationIndex + " split");
//      List<Formation> formationList = new List<Formation>()
//      {
//        formation
//      };
//      for (; count > 1; --count)
//      {
//        int unitCount = formation.CountOfUnits / count;
//        for (int formationIndex = 0; formationIndex < 8; ++formationIndex)
//        {
//          Formation formation1 = formation.Team.GetFormation((FormationClass) formationIndex);
//          if (formation1.CountOfUnits == 0)
//          {
//            formation.TransferUnitsAux(formation1, unitCount, false, false);
//            formationList.Add(formation1);
//            MBList<Formation> appliedFormations = new MBList<Formation>();
//            appliedFormations.Add(formation);
//            this.FireOnOrderIssued(OrderType.Transfer, (MBReadOnlyList<Formation>) appliedFormations, this, (object) formation1, (object) unitCount);
//            break;
//          }
//        }
//      }
//      return (IEnumerable<Formation>) formationList;
//    }

//    protected void FireOnOrderIssued(
//      OrderType orderType,
//      MBReadOnlyList<Formation> appliedFormations,
//      OrderController orderController,
//      params object[] delegateParams)
//    {
//      if (this.OnOrderIssued == null)
//        return;
//      this.OnOrderIssued(orderType, appliedFormations, orderController, delegateParams);
//    }

//    [Conditional("DEBUG")]
//    public void TickDebug()
//    {
//    }

//    public void AddOrderOverride(
//      Func<Formation, MovementOrder, MovementOrder> orderOverride)
//    {
//      if (this.orderOverrides == null)
//      {
//        this.orderOverrides = new List<Func<Formation, MovementOrder, MovementOrder>>();
//        this.overridenOrders = new List<(Formation, OrderType)>();
//      }
//      this.orderOverrides.Add(orderOverride);
//    }

//    public OrderType GetOverridenOrderType(Formation formation)
//    {
//      if (this.overridenOrders == null)
//        return OrderType.None;
//      (Formation, OrderType) tuple = this.overridenOrders.FirstOrDefault<(Formation, OrderType)>((Func<(Formation, OrderType), bool>) (oo => oo.Item1 == formation));
//      return tuple.Item1 != null ? tuple.Item2 : OrderType.None;
//    }

//    public void SetFormationUpdateEnabledAfterSetOrder(bool value)
//    {
//      this._formationUpdateEnabledAfterSetOrder = value;
//    }

//    private void CreateDefaultOrderOverrides()
//    {
//      this.AddOrderOverride((Func<Formation, MovementOrder, MovementOrder>) ((formation, order) =>
//      {
//        if (formation.ArrangementOrder.OrderType != OrderType.ArrangementCloseOrder || order.OrderType != OrderType.StandYourGround)
//          return MovementOrder.MovementOrderStop;
//        Vec2 cachedAveragePosition = formation.CachedAveragePosition;
//        WorldPosition cachedMedianPosition = formation.CachedMedianPosition;
//        cachedMedianPosition.SetVec2(cachedAveragePosition + formation.Direction * formation.Depth * (0.5f + formation.CachedMovementSpeed));
//        return MovementOrder.MovementOrderMove(cachedMedianPosition);
//      }));
//    }

//    private static void TryCancelStopOrder(Formation formation)
//    {
//      if (GameNetwork.IsClientOrReplay || formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Stop)
//        return;
//      WorldPosition orderWorldPosition = formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
//      if (!orderWorldPosition.IsValid)
//        return;
//      formation.SetMovementOrder(MovementOrder.MovementOrderMove(orderWorldPosition));
//    }
//  }
//}
