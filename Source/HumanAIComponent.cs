//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.HumanAIComponent
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System;
//using System.Collections.Generic;
//using TaleWorlds.Core;
//using TaleWorlds.DotNet;
//using TaleWorlds.Engine;
//using TaleWorlds.Library;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public class HumanAIComponent : AgentComponent
//  {
//    private const float AvoidPickUpIfLookAgentIsCloseDistance = 20f;
//    private const float AvoidPickUpIfLookAgentIsCloseDistanceSquared = 400f;
//    private const float ClosestMountSearchRangeSq = 6400f;
//    public static bool FormationSpeedAdjustmentEnabled = true;
//    private readonly HumanAIComponent.BehaviorValues[] _behaviorValues;
//    private bool _hasNewBehaviorValues;
//    private readonly WeakGameEntity[] _tempPickableEntities = new WeakGameEntity[16];
//    private readonly UIntPtr[] _pickableItemsId = new UIntPtr[16];
//    private SpawnedItemEntity _itemToPickUp;
//    private readonly MissionTimer _itemPickUpTickTimer;
//    private bool _disablePickUpForAgent;
//    private readonly MissionTimer _mountSearchTimer;
//    private UsableMissionObject _objectOfInterest;
//    private HumanAIComponent.UsableObjectInterestKind _objectInterestKind;
//    private HumanAIComponent.BehaviorValueSet _lastBehaviorValueSet;
//    private bool _shouldCatchUpWithFormation;
//    private bool _forceDisableItemPickup;
//    private float _scriptedFrameTimer = -1f;

//    public Agent FollowedAgent { get; private set; }

//    public bool ShouldCatchUpWithFormation
//    {
//      get => this._shouldCatchUpWithFormation;
//      private set
//      {
//        if (this._shouldCatchUpWithFormation == value)
//          return;
//        this._shouldCatchUpWithFormation = value;
//        this.Agent.SetShouldCatchUpWithFormation(value);
//      }
//    }

//    public bool IsDefending
//    {
//      get => this._objectInterestKind == HumanAIComponent.UsableObjectInterestKind.Defending;
//    }

//    public bool HasTimedScriptedFrame => (double) this._scriptedFrameTimer > 0.0;

//    public HumanAIComponent(Agent agent)
//      : base(agent)
//    {
//      this._behaviorValues = new HumanAIComponent.BehaviorValues[7];
//      this._lastBehaviorValueSet = HumanAIComponent.BehaviorValueSet.Overriden;
//      this.SetBehaviorValueSet(HumanAIComponent.BehaviorValueSet.Default);
//      this.Agent.SetAllBehaviorParams(this._behaviorValues);
//      this._hasNewBehaviorValues = false;
//      this.Agent.OnAgentWieldedItemChange += new Action(this.DisablePickUpForAgentIfNeeded);
//      this.Agent.OnAgentMountedStateChanged += new Action(this.DisablePickUpForAgentIfNeeded);
//      this._itemPickUpTickTimer = new MissionTimer(2.5f + MBRandom.RandomFloat);
//      this._mountSearchTimer = new MissionTimer(2f + MBRandom.RandomFloat);
//    }

//    public void OverrideBehaviorParams(
//      HumanAIComponent.AISimpleBehaviorKind behavior,
//      float y1,
//      float x2,
//      float y2,
//      float x3,
//      float y3)
//    {
//      this._lastBehaviorValueSet = HumanAIComponent.BehaviorValueSet.Overriden;
//      this._behaviorValues[(int) behavior].y1 = y1;
//      this._behaviorValues[(int) behavior].x2 = x2;
//      this._behaviorValues[(int) behavior].y2 = y2;
//      this._behaviorValues[(int) behavior].x3 = x3;
//      this._behaviorValues[(int) behavior].y3 = y3;
//      this._hasNewBehaviorValues = true;
//    }

//    private void SetBehaviorParams(
//      HumanAIComponent.AISimpleBehaviorKind behavior,
//      float y1,
//      float x2,
//      float y2,
//      float x3,
//      float y3)
//    {
//      this._behaviorValues[(int) behavior].y1 = y1;
//      this._behaviorValues[(int) behavior].x2 = x2;
//      this._behaviorValues[(int) behavior].y2 = y2;
//      this._behaviorValues[(int) behavior].x3 = x3;
//      this._behaviorValues[(int) behavior].y3 = y3;
//      this._hasNewBehaviorValues = true;
//    }

//    public void SyncBehaviorParamsIfNecessary()
//    {
//      if (!this._hasNewBehaviorValues)
//        return;
//      this.Agent.SetAllBehaviorParams(this._behaviorValues);
//      this._hasNewBehaviorValues = false;
//    }

//    public void DisablePickUpForAgentIfNeeded()
//    {
//      this._disablePickUpForAgent = true;
//      if (this.Agent.MountAgent == null)
//      {
//        if (this.Agent.HasLostShield())
//        {
//          this._disablePickUpForAgent = false;
//        }
//        else
//        {
//          for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; ++index)
//          {
//            MissionWeapon missionWeapon = this.Agent.Equipment[index];
//            if (!missionWeapon.IsEmpty && missionWeapon.IsAnyConsumable())
//            {
//              this._disablePickUpForAgent = false;
//              break;
//            }
//          }
//        }
//      }
//      if (!this._disablePickUpForAgent || this.Agent.Formation == null || !MissionGameModels.Current.BattleBannerBearersModel.IsBannerSearchingAgent(this.Agent))
//        return;
//      this._disablePickUpForAgent = false;
//    }

//    public override void OnTickParallel(float dt)
//    {
//      if (!this.Agent.Mission.AllowAiTicking || !this.Agent.IsAIControlled)
//        return;
//      this.SyncBehaviorParamsIfNecessary();
//    }

//    private void ItemPickupTick()
//    {
//      if (this._itemToPickUp != null)
//      {
//        if (!this._itemToPickUp.IsAIMovingTo(this.Agent) || this.Agent.Mission.MissionEnded)
//          this._itemToPickUp = (SpawnedItemEntity) null;
//        else if (!this._itemToPickUp.GameEntity.IsValid)
//          this.Agent.StopUsingGameObject(false);
//      }
//      if (!this._itemPickUpTickTimer.Check(true) || this.Agent.Mission.MissionEnded)
//        return;
//      EquipmentIndex wieldedItemIndex = this.Agent.GetPrimaryWieldedItemIndex();
//      WeaponComponentData currentUsageItem = wieldedItemIndex == EquipmentIndex.None ? (WeaponComponentData) null : this.Agent.Equipment[wieldedItemIndex].CurrentUsageItem;
//      bool flag = currentUsageItem != null && currentUsageItem.IsRangedWeapon;
//      if (this._disablePickUpForAgent || !MissionGameModels.Current.ItemPickupModel.IsAgentEquipmentSuitableForPickUpAvailability(this.Agent) || !this.Agent.CanBeAssignedForScriptedMovement() || !this.Agent.IsAlarmed() || (this.Agent.GetAgentFlags() & AgentFlag.CanAttack) == AgentFlag.None || this.IsInImportantCombatAction() || this.Agent.IsInWater())
//        return;
//      Agent targetAgent = this.Agent.GetTargetAgent();
//      if (targetAgent != null && ((double) targetAgent.Position.DistanceSquared(this.Agent.Position) <= 400.0 || flag && !this.IsAnyConsumableDepleted() && (double) targetAgent.Position.DistanceSquared(this.Agent.Position) < (double) this.Agent.GetMissileRange() * 1.2000000476837158 && this.Agent.GetLastTargetVisibilityState() == AITargetVisibilityState.TargetIsClear) || this._itemToPickUp != null)
//        return;
//      float forwardUnlimitedSpeed = this.Agent.GetMaximumForwardUnlimitedSpeed();
//      this._itemToPickUp = this.SelectPickableItem(this.Agent.Position - new Vec3(forwardUnlimitedSpeed, forwardUnlimitedSpeed, 1f), this.Agent.Position + new Vec3(forwardUnlimitedSpeed, forwardUnlimitedSpeed, 1.8f));
//      if (this._itemToPickUp == null)
//        return;
//      this.RequestMoveToItem(this._itemToPickUp);
//    }

//    public override void OnTick(float dt)
//    {
//      if (!this.Agent.Mission.AllowAiTicking || !this.Agent.IsAIControlled)
//        return;
//      if (!this._forceDisableItemPickup)
//        this.ItemPickupTick();
//      if (!this._forceDisableItemPickup && this._itemToPickUp != null && !this.Agent.IsRunningAway && this.Agent.AIMoveToGameObjectIsEnabled())
//      {
//        float num = (this._itemToPickUp.IsBanner() ? MissionGameModels.Current.BattleBannerBearersModel.GetBannerInteractionDistance(this.Agent) : MissionGameModels.Current.AgentStatCalculateModel.GetInteractionDistance(this.Agent)) * 3f;
//        if (this.Agent.CanReachAndUseObject((UsableMissionObject) this._itemToPickUp, this._itemToPickUp.GetUserFrameForAgent(this.Agent).Origin.DistanceSquaredWithLimit(this.Agent.Position, (float) ((double) num * (double) num + 9.9999997473787516E-06))))
//          this.Agent.UseGameObject((UsableMissionObject) this._itemToPickUp);
//      }
//      if (this.Agent.CommonAIComponent != null && this.Agent.MountAgent == null && !this.Agent.CommonAIComponent.IsRetreating && this._mountSearchTimer.Check(true) && this.Agent.GetRidingOrder() == 1)
//      {
//        Agent reservedMount = this.FindReservedMount();
//        if ((reservedMount == null || reservedMount.State != AgentState.Active || reservedMount.RiderAgent != null ? 1 : ((double) this.Agent.Position.DistanceSquared(reservedMount.Position) >= 256.0 ? 1 : 0)) != 0)
//        {
//          if (reservedMount != null)
//            this.UnreserveMount(reservedMount);
//          Agent closestMountAvailable = this.FindClosestMountAvailable();
//          if (closestMountAvailable != null)
//            this.ReserveMount(closestMountAvailable);
//        }
//      }
//      if ((double) this._scriptedFrameTimer <= 0.0)
//        return;
//      this._scriptedFrameTimer -= dt;
//      if ((double) this._scriptedFrameTimer >= 0.0)
//        return;
//      this.Agent.DisableScriptedMovement();
//    }

//    private Agent FindClosestMountAvailable()
//    {
//      float num1 = 6400f;
//      Agent agent = (Agent) null;
//      float num2 = 6400f;
//      Agent mount = (Agent) null;
//      foreach (KeyValuePair<Agent, MissionTime> mountsWithoutRider in (List<KeyValuePair<Agent, MissionTime>>) Mission.Current.MountsWithoutRiders)
//      {
//        Agent key = mountsWithoutRider.Key;
//        if (key.IsActive() && key.RiderAgent == null && !key.IsRunningAway && MissionGameModels.Current.AgentStatCalculateModel.CanAgentRideMount(this.Agent, key))
//        {
//          float num3 = this.Agent.Position.DistanceSquared(key.Position);
//          if ((double) num2 > (double) num3)
//          {
//            mount = key;
//            num2 = num3;
//            if (mount.CommonAIComponent.ReservedRiderAgentIndex < 0)
//            {
//              num1 = num3;
//              agent = key;
//            }
//          }
//          else if ((double) num1 > (double) num3 && mount.CommonAIComponent.ReservedRiderAgentIndex < 0)
//          {
//            num1 = num3;
//            agent = key;
//          }
//        }
//      }
//      if (mount != agent)
//      {
//        if (agent != null && (double) num1 > 0.0099999997764825821 && (double) num2 / (double) num1 >= 0.40000000596046448)
//        {
//          mount = agent;
//        }
//        else
//        {
//          Agent agentWithIndex = Mission.Current.FindAgentWithIndex(mount.CommonAIComponent.ReservedRiderAgentIndex);
//          float num4 = agentWithIndex.Position.DistanceSquared(mount.Position);
//          if ((double) num4 > 0.0099999997764825821 && (double) num2 / (double) num4 < (agent != null ? 0.40000000596046448 : 0.699999988079071))
//            agentWithIndex.HumanAIComponent.UnreserveMount(mount);
//          else
//            mount = agent;
//        }
//      }
//      return mount;
//    }

//    private Agent FindReservedMount()
//    {
//      Agent reservedMount = (Agent) null;
//      int selectedMountIndex = this.Agent.GetSelectedMountIndex();
//      if (selectedMountIndex >= 0)
//      {
//        foreach (KeyValuePair<Agent, MissionTime> mountsWithoutRider in (List<KeyValuePair<Agent, MissionTime>>) Mission.Current.MountsWithoutRiders)
//        {
//          Agent key = mountsWithoutRider.Key;
//          if (key.Index == selectedMountIndex)
//          {
//            reservedMount = key;
//            break;
//          }
//        }
//      }
//      return reservedMount;
//    }

//    internal void ReserveMount(Agent mount)
//    {
//      this.Agent.SetSelectedMountIndex(mount.Index);
//      mount.CommonAIComponent.OnMountReserved(this.Agent.Index);
//    }

//    internal void UnreserveMount(Agent mount)
//    {
//      this.Agent.SetSelectedMountIndex(-1);
//      mount.CommonAIComponent.OnMountUnreserved();
//    }

//    public override void OnAgentRemoved()
//    {
//      Agent reservedMount = this.FindReservedMount();
//      if (reservedMount == null)
//        return;
//      this.UnreserveMount(reservedMount);
//    }

//    public override void OnComponentRemoved()
//    {
//      Agent reservedMount = this.FindReservedMount();
//      if (reservedMount == null)
//        return;
//      this.UnreserveMount(reservedMount);
//    }

//    public bool IsInImportantCombatAction()
//    {
//      Agent.ActionCodeType currentActionType = this.Agent.GetCurrentActionType(1);
//      switch (currentActionType)
//      {
//        case Agent.ActionCodeType.ReadyRanged:
//        case Agent.ActionCodeType.ReleaseRanged:
//        case Agent.ActionCodeType.ReleaseThrowing:
//        case Agent.ActionCodeType.ReadyMelee:
//        case Agent.ActionCodeType.ReleaseMelee:
//          return true;
//        default:
//          return currentActionType == Agent.ActionCodeType.DefendShield;
//      }
//    }

//    private bool IsAnyConsumableDepleted()
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; ++index)
//      {
//        MissionWeapon missionWeapon = this.Agent.Equipment[index];
//        if (!missionWeapon.IsEmpty && missionWeapon.IsAnyConsumable() && missionWeapon.Amount == (short) 0)
//          return true;
//      }
//      return false;
//    }

//    private SpawnedItemEntity SelectPickableItem(Vec3 bMin, Vec3 bMax)
//    {
//      Agent targetAgent = this.Agent.GetTargetAgent();
//      Vec3 v1 = targetAgent == null ? Vec3.Invalid : targetAgent.Position - this.Agent.Position;
//      int num1 = this.Agent.Mission.Scene.SelectEntitiesInBoxWithScriptComponent<SpawnedItemEntity>(ref bMin, ref bMax, this._tempPickableEntities, this._pickableItemsId);
//      float num2 = 0.0f;
//      SpawnedItemEntity spawnedItemEntity = (SpawnedItemEntity) null;
//      for (int index = 0; index < num1; ++index)
//      {
//        SpawnedItemEntity firstScriptOfType = this._tempPickableEntities[index].GetFirstScriptOfType<SpawnedItemEntity>();
//        bool flag = false;
//        if (firstScriptOfType != null)
//        {
//          MissionWeapon weaponCopy = firstScriptOfType.WeaponCopy;
//          flag = !weaponCopy.IsEmpty && (weaponCopy.IsShield() || weaponCopy.IsBanner() || firstScriptOfType.IsStuckMissile() || firstScriptOfType.IsQuiverAndNotEmpty());
//        }
//        if (flag && !firstScriptOfType.HasUser && (!firstScriptOfType.HasAIMovingTo || firstScriptOfType.IsAIMovingTo(this.Agent)) && firstScriptOfType.GameEntityWithWorldPosition.GetNavMesh() != UIntPtr.Zero)
//        {
//          WorldFrame userFrameForAgent = firstScriptOfType.GetUserFrameForAgent(this.Agent);
//          Vec3 v2 = userFrameForAgent.Origin.GetGroundVec3() - this.Agent.Position;
//          float z = v2.z;
//          double num3 = (double) v2.Normalize();
//          if (targetAgent == null || (double) v1.Length - (double) Vec3.DotProduct(v1, v2) > (double) targetAgent.GetMaximumForwardUnlimitedSpeed() * 3.0)
//          {
//            EquipmentIndex slotToPickUp = MissionEquipment.SelectWeaponPickUpSlot(this.Agent, firstScriptOfType.WeaponCopy, firstScriptOfType.IsStuckMissile());
//            if (slotToPickUp != EquipmentIndex.None && firstScriptOfType.GameEntityWithWorldPosition.GetNavMesh() != UIntPtr.Zero)
//            {
//              Agent agent = this.Agent;
//              SpawnedItemEntity gameObject = firstScriptOfType;
//              userFrameForAgent = firstScriptOfType.GetUserFrameForAgent(this.Agent);
//              double distanceSq = (double) userFrameForAgent.Origin.GetGroundVec3().DistanceSquared(firstScriptOfType.GameEntityWithWorldPosition.GetNavMeshVec3());
//              Vec3 navMeshVec3 = firstScriptOfType.GameEntityWithWorldPosition.GetNavMeshVec3();
//              if (agent.CanReachObjectFromPosition((UsableMissionObject) gameObject, (float) distanceSq, navMeshVec3) && MissionGameModels.Current.ItemPickupModel.IsItemAvailableForAgent(firstScriptOfType, this.Agent, slotToPickUp) && this.Agent.CanMoveDirectlyToPosition(firstScriptOfType.GameEntityWithWorldPosition.AsVec2) && (!this.Agent.Mission.IsPositionInsideAnyBlockerNavMeshFace2D(firstScriptOfType.GameEntityWithWorldPosition.AsVec2) || (double) MathF.Abs(z) >= 1.5))
//              {
//                float itemScoreForAgent = MissionGameModels.Current.ItemPickupModel.GetItemScoreForAgent(firstScriptOfType, this.Agent);
//                if ((double) itemScoreForAgent > (double) num2)
//                {
//                  spawnedItemEntity = firstScriptOfType;
//                  num2 = itemScoreForAgent;
//                }
//              }
//            }
//          }
//        }
//      }
//      return spawnedItemEntity;
//    }

//    internal void ItemPickupDone(SpawnedItemEntity spawnedItemEntity)
//    {
//      this._itemToPickUp = (SpawnedItemEntity) null;
//    }

//    private void RequestMoveToItem(SpawnedItemEntity item)
//    {
//      item.MovingAgent?.StopUsingGameObject(false);
//      this.MoveToUsableGameObject((UsableMissionObject) item, (IDetachment) null);
//    }

//    public UsableMissionObject GetCurrentlyMovingGameObject() => this._objectOfInterest;

//    private void SetCurrentlyMovingGameObject(UsableMissionObject objectOfInterest)
//    {
//      this._objectOfInterest = objectOfInterest;
//      this._objectInterestKind = this._objectOfInterest != null ? HumanAIComponent.UsableObjectInterestKind.MovingTo : HumanAIComponent.UsableObjectInterestKind.None;
//    }

//    public UsableMissionObject GetCurrentlyDefendingGameObject() => this._objectOfInterest;

//    private void SetCurrentlyDefendingGameObject(UsableMissionObject objectOfInterest)
//    {
//      this._objectOfInterest = objectOfInterest;
//      this._objectInterestKind = this._objectOfInterest != null ? HumanAIComponent.UsableObjectInterestKind.Defending : HumanAIComponent.UsableObjectInterestKind.None;
//    }

//    public void MoveToUsableGameObject(
//      UsableMissionObject usedObject,
//      IDetachment detachment,
//      Agent.AIScriptedFrameFlags scriptedFrameFlags = Agent.AIScriptedFrameFlags.NoAttack)
//    {
//      this.Agent.AIStateFlags |= Agent.AIStateFlag.UseObjectMoving;
//      this.SetCurrentlyMovingGameObject(usedObject);
//      usedObject.OnAIMoveToUse(this.Agent, detachment);
//      WorldFrame userFrameForAgent = usedObject.GetUserFrameForAgent(this.Agent);
//      this.Agent.SetScriptedPositionAndDirection(ref userFrameForAgent.Origin, userFrameForAgent.Rotation.f.AsVec2.RotationInRadians, false, scriptedFrameFlags);
//    }

//    public void MoveToClear()
//    {
//      this.GetCurrentlyMovingGameObject()?.OnMoveToStopped(this.Agent);
//      this.SetCurrentlyMovingGameObject((UsableMissionObject) null);
//      this.Agent.AIStateFlags &= ~Agent.AIStateFlag.UseObjectMoving;
//    }

//    public void StartDefendingGameObject(UsableMissionObject usedObject, IDetachment detachment)
//    {
//      this.SetCurrentlyDefendingGameObject(usedObject);
//      usedObject.OnAIDefendBegin(this.Agent, detachment);
//    }

//    public void StopDefendingGameObject()
//    {
//      this.GetCurrentlyDefendingGameObject().OnAIDefendEnd(this.Agent);
//      this.SetCurrentlyDefendingGameObject((UsableMissionObject) null);
//    }

//    public bool IsInterestedInAnyGameObject() => this._objectInterestKind != 0;

//    public bool IsInterestedInGameObject(UsableMissionObject usableMissionObject)
//    {
//      bool flag = false;
//      switch (this._objectInterestKind)
//      {
//        case HumanAIComponent.UsableObjectInterestKind.None:
//          return flag;
//        case HumanAIComponent.UsableObjectInterestKind.MovingTo:
//          flag = usableMissionObject == this.GetCurrentlyMovingGameObject();
//          goto case HumanAIComponent.UsableObjectInterestKind.None;
//        case HumanAIComponent.UsableObjectInterestKind.Defending:
//          flag = usableMissionObject == this.GetCurrentlyDefendingGameObject();
//          goto case HumanAIComponent.UsableObjectInterestKind.None;
//        default:
//          Debug.FailedAssert("Unexpected object interest kind: " + (object) this._objectInterestKind, "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\AgentComponents\\HumanAIComponent.cs", nameof (IsInterestedInGameObject), 686);
//          goto case HumanAIComponent.UsableObjectInterestKind.None;
//      }
//    }

//    public void FollowAgent(Agent agent) => this.FollowedAgent = agent;

//    public float GetDesiredSpeedInFormation(bool isCharging)
//    {
//      if (!(this.Agent.Formation.Arrangement is ColumnFormation) && this.ShouldCatchUpWithFormation && (!isCharging || !Mission.Current.IsMissionEnding))
//      {
//        Agent mountAgent = this.Agent.MountAgent;
//        float num1 = mountAgent != null ? mountAgent.GetMaximumForwardUnlimitedSpeed() : this.Agent.GetMaximumForwardUnlimitedSpeed();
//        bool flag = !isCharging;
//        Vec3 vec3;
//        if (isCharging)
//        {
//          FormationQuerySystem closestEnemyFormation = this.Agent.Formation.CachedClosestEnemyFormation;
//          float num2 = float.MaxValue;
//          float num3 = 4f * num1 * num1;
//          if (closestEnemyFormation != null)
//          {
//            num2 = this.Agent.Formation.CachedMedianPosition.AsVec2.DistanceSquared(closestEnemyFormation.Formation.CachedMedianPosition.AsVec2);
//            if ((double) num2 <= (double) num3)
//            {
//              WorldPosition cachedMedianPosition = this.Agent.Formation.CachedMedianPosition;
//              vec3 = cachedMedianPosition.GetNavMeshVec3();
//              ref Vec3 local = ref vec3;
//              cachedMedianPosition = closestEnemyFormation.Formation.CachedMedianPosition;
//              Vec3 navMeshVec3 = cachedMedianPosition.GetNavMeshVec3();
//              num2 = local.DistanceSquared(navMeshVec3);
//            }
//          }
//          flag = (double) num2 > (double) num3;
//        }
//        if (flag)
//        {
//          Vec2 globalPositionOfUnit = this.Agent.Formation.GetCurrentGlobalPositionOfUnit(this.Agent, true);
//          vec3 = this.Agent.Position;
//          Vec2 asVec2 = vec3.AsVec2;
//          Vec2 v = globalPositionOfUnit - asVec2;
//          float num4 = MathF.Clamp(-this.Agent.GetMovementDirection().DotProduct(v), 0.0f, 100f);
//          float num5 = this.Agent.MountAgent != null ? 4f : 2f;
//          float num6 = (isCharging ? this.Agent.Formation.CachedFormationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents : this.Agent.Formation.CachedMovementSpeed) / num1;
//          return MathF.Clamp((float) (0.699999988079071 + 0.40000000596046448 * (((double) num1 - (double) num4 * (double) num5) / ((double) num1 + (double) num4 * (double) num5))) * num6, 0.2f, 1f);
//        }
//      }
//      return 1f;
//    }

//    private bool GetFormationFrame(
//      out WorldPosition formationPosition,
//      out Vec2 formationDirection,
//      out float speedLimit,
//      out bool limitIsMultiplier)
//    {
//      Formation formation = this.Agent.Formation;
//      limitIsMultiplier = false;
//      bool formationFrame = false;
//      if (formation != null && (!this.Agent.IsInWater() || this.Agent.Mission.IsTeleportingAgents))
//      {
//        formationPosition = formation.GetOrderPositionOfUnit(this.Agent);
//        formationDirection = formation.GetDirectionOfUnit(this.Agent);
//      }
//      else
//      {
//        formationPosition = WorldPosition.Invalid;
//        formationDirection = Vec2.Invalid;
//      }
//      if (formationPosition.IsValid && formationPosition.GetNavMeshMT() == UIntPtr.Zero)
//      {
//        UIntPtr nearestNavMesh = formationPosition.GetNearestNavMesh();
//        if (nearestNavMesh != UIntPtr.Zero)
//        {
//          Vec2 positionOnAboundaryFace = Mission.Current.Scene.FindClosestExitPositionForPositionOnABoundaryFace(formationPosition.GetVec3WithoutValidity(), nearestNavMesh);
//          if (positionOnAboundaryFace.IsValid)
//            formationPosition.SetVec2(positionOnAboundaryFace);
//        }
//      }
//      if (HumanAIComponent.FormationSpeedAdjustmentEnabled && this.Agent.IsMount)
//      {
//        formationPosition = WorldPosition.Invalid;
//        formationDirection = Vec2.Invalid;
//        if (this.Agent.RiderAgent == null || this.Agent.RiderAgent != null && (!this.Agent.RiderAgent.IsActive() || this.Agent.RiderAgent.Formation == null))
//        {
//          speedLimit = -1f;
//        }
//        else
//        {
//          limitIsMultiplier = true;
//          speedLimit = this.Agent.RiderAgent.HumanAIComponent.GetDesiredSpeedInFormation(formation.GetReadonlyMovementOrderReference().MovementState == MovementOrder.MovementStateEnum.Charge);
//        }
//      }
//      else if (formation == null)
//        speedLimit = -1f;
//      else if (this.Agent.IsDetachedFromFormation)
//      {
//        speedLimit = -1f;
//        WorldFrame? nullable = new WorldFrame?();
//        if (formation.GetReadonlyMovementOrderReference().MovementState != MovementOrder.MovementStateEnum.Charge || this.Agent.Detachment != null && (!this.Agent.Detachment.IsLoose || formationPosition.IsValid))
//          nullable = formation.GetDetachmentFrame(this.Agent);
//        if (nullable.HasValue)
//        {
//          formationDirection = nullable.Value.Rotation.f.AsVec2.Normalized();
//          formationFrame = true;
//        }
//        else
//          formationDirection = Vec2.Invalid;
//      }
//      else
//      {
//        switch (formation.GetReadonlyMovementOrderReference().MovementState)
//        {
//          case MovementOrder.MovementStateEnum.Charge:
//            limitIsMultiplier = true;
//            speedLimit = HumanAIComponent.FormationSpeedAdjustmentEnabled ? this.GetDesiredSpeedInFormation(true) : -1f;
//            formationFrame = formationPosition.IsValid;
//            break;
//          case MovementOrder.MovementStateEnum.Hold:
//            if (HumanAIComponent.FormationSpeedAdjustmentEnabled && this.ShouldCatchUpWithFormation)
//            {
//              limitIsMultiplier = true;
//              speedLimit = this.GetDesiredSpeedInFormation(false);
//            }
//            else
//              speedLimit = -1f;
//            formationFrame = true;
//            break;
//          case MovementOrder.MovementStateEnum.Retreat:
//            speedLimit = -1f;
//            break;
//          case MovementOrder.MovementStateEnum.StandGround:
//            speedLimit = -1f;
//            formationFrame = true;
//            break;
//          default:
//            Debug.FailedAssert("false", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\AgentComponents\\HumanAIComponent.cs", nameof (GetFormationFrame), 878);
//            speedLimit = -1f;
//            break;
//        }
//      }
//      return formationFrame;
//    }

//    public void AdjustSpeedLimit(Agent agent, float desiredSpeed, bool limitIsMultiplier)
//    {
//      if (agent.MissionPeer != null)
//        desiredSpeed = -1f;
//      this.Agent.SetMaximumSpeedLimit(desiredSpeed, limitIsMultiplier);
//      agent.MountAgent?.SetMaximumSpeedLimit(desiredSpeed, limitIsMultiplier);
//    }

//    public void ParallelUpdateFormationMovement()
//    {
//      WorldPosition formationPosition;
//      Vec2 formationDirection;
//      float speedLimit;
//      bool limitIsMultiplier;
//      int num1 = this.GetFormationFrame(out formationPosition, out formationDirection, out speedLimit, out limitIsMultiplier) ? 1 : 0;
//      this.AdjustSpeedLimit(this.Agent, speedLimit, limitIsMultiplier);
//      if (this.Agent.Controller == AgentControllerType.AI && this.Agent.Formation != null && !(this.Agent.Formation.Arrangement is ColumnFormation) && this.Agent.Formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Stop && this.Agent.Formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Retreat && !this.Agent.IsRetreating())
//      {
//        Formation.FormationIntegrityDataGroup formationIntegrityData = this.Agent.Formation.CachedFormationIntegrityData;
//        float num2 = formationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents * 3f;
//        if ((double) formationIntegrityData.DeviationOfPositionsExcludeFarAgents > (double) num2)
//        {
//          this.ShouldCatchUpWithFormation = false;
//          this.Agent.SetFormationIntegrityData(Vec2.Zero, Vec2.Zero, Vec2.Zero, 0.0f, 0.0f, false);
//        }
//        else
//        {
//          Vec2 globalPositionOfUnit = this.Agent.Formation.GetCurrentGlobalPositionOfUnit(this.Agent, true);
//          this.ShouldCatchUpWithFormation = (double) this.Agent.Position.AsVec2.Distance(globalPositionOfUnit) < (double) num2 * 2.0;
//          bool shouldKeepWithFormationInsteadOfMovingToAgent = this.ShouldCatchUpWithFormation && this.Agent.GetOffhandWieldedItemIndex() == EquipmentIndex.ExtraWeaponSlot && (double) this.Agent.Formation.QuerySystem.RangedUnitRatioReadOnly + (double) this.Agent.Formation.QuerySystem.RangedCavalryUnitRatioReadOnly >= 0.5;
//          this.Agent.SetFormationIntegrityData(this.ShouldCatchUpWithFormation ? globalPositionOfUnit : Vec2.Zero, this.Agent.Formation.CurrentDirection, formationIntegrityData.AverageVelocityExcludeFarAgents, formationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents, formationIntegrityData.DeviationOfPositionsExcludeFarAgents, shouldKeepWithFormationInsteadOfMovingToAgent);
//        }
//      }
//      else
//        this.ShouldCatchUpWithFormation = false;
//      bool flag = formationPosition.IsValid;
//      if (num1 == 0 || !flag)
//      {
//        this.Agent.SetFormationFrameDisabled();
//      }
//      else
//      {
//        if (!GameNetwork.IsMultiplayer && this.Agent.Mission.Mode == MissionMode.Deployment && !this.Agent.Mission.IsNavalBattle)
//        {
//          IMissionDeploymentPlan deploymentPlan = this.Agent.Mission.DeploymentPlan;
//          if (deploymentPlan.SupportsNavmesh())
//            deploymentPlan.ProjectPositionToDeploymentBoundaries(this.Agent.Formation.Team, ref formationPosition);
//          flag = this.Agent.Mission.IsFormationUnitPositionAvailable(ref formationPosition, this.Agent.Team);
//        }
//        if (flag)
//        {
//          this.Agent.SetFormationFrameEnabled(formationPosition, formationDirection, this.Agent.Formation.GetReadonlyMovementOrderReference().GetTargetVelocity(), this.Agent.Formation.CalculateFormationDirectionEnforcingFactorForRank(((IFormationUnit) this.Agent).FormationRankIndex));
//          float tendency = 1f;
//          if (this.Agent.Formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall && !this.Agent.IsDetachedFromFormation)
//            tendency = this.Agent.Formation.Arrangement.GetDirectionChangeTendencyOfUnit((IFormationUnit) this.Agent);
//          this.Agent.SetDirectionChangeTendency(tendency);
//        }
//        else
//          this.Agent.SetFormationFrameDisabled();
//      }
//    }

//    public override void OnRetreating()
//    {
//      base.OnRetreating();
//      this.AdjustSpeedLimit(this.Agent, -1f, false);
//    }

//    public override void OnDismount(Agent mount)
//    {
//      base.OnDismount(mount);
//      mount.SetMaximumSpeedLimit(-1f, false);
//    }

//    public void SetBehaviorValueSet(HumanAIComponent.BehaviorValueSet behaviorValueSet)
//    {
//      if (this._lastBehaviorValueSet == behaviorValueSet)
//        return;
//      this._lastBehaviorValueSet = behaviorValueSet;
//      switch (behaviorValueSet)
//      {
//        case HumanAIComponent.BehaviorValueSet.Default:
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Melee, 8f, 7f, 4f, 20f, 1f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Ranged, 2f, 7f, 4f, 20f, 5f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 2f, 25f, 5f, 30f, 5f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 2f, 15f, 6.5f, 30f, 5.5f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 4f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 5.5f, 12f, 8f, 30f, 4.5f);
//          break;
//        case HumanAIComponent.BehaviorValueSet.DefensiveArrangementMove:
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 8f, 5f, 20f, 6f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Melee, 4f, 5f, 0.0f, 20f, 0.0f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Ranged, 0.0f, 7f, 0.0f, 20f, 0.0f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 0.0f, 7f, 0.0f, 30f, 0.0f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 0.0f, 15f, 0.0f, 30f, 0.0f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 9f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
//          break;
//        case HumanAIComponent.BehaviorValueSet.Follow:
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Melee, 6f, 7f, 4f, 20f, 0.0f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Ranged, 0.0f, 7f, 0.0f, 20f, 0.0f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 0.0f, 7f, 0.0f, 30f, 0.0f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 0.0f, 15f, 0.0f, 30f, 0.0f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 9f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
//          break;
//        case HumanAIComponent.BehaviorValueSet.DefaultMove:
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Melee, 8f, 7f, 5f, 20f, 0.01f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Ranged, 0.02f, 7f, 0.04f, 20f, 0.03f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 100f, 3f, 10f, 15f, 0.1f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 0.02f, 15f, 0.065f, 30f, 0.055f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 9f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
//          break;
//        case HumanAIComponent.BehaviorValueSet.Charge:
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Melee, 8f, 7f, 4f, 20f, 1f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Ranged, 2f, 7f, 4f, 20f, 5f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 2f, 25f, 5f, 30f, 5f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 0.0f, 10f, 3f, 20f, 6f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 9f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
//          break;
//        case HumanAIComponent.BehaviorValueSet.DefaultDetached:
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Melee, 8f, 7f, 4f, 20f, 1f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.Ranged, 0.02f, 7f, 0.04f, 20f, 0.03f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 2f, 25f, 5f, 30f, 5f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 0.02f, 15f, 0.065f, 30f, 0.055f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 3.5f, 7f, 5.5f, 20f, 6.5f);
//          this.SetBehaviorParams(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 0.25f, 7f, 0.45f, 20f, 0.55f);
//          break;
//      }
//    }

//    public void RefreshBehaviorValues(
//      MovementOrder.MovementOrderEnum movementOrder,
//      ArrangementOrder.ArrangementOrderEnum arrangementOrder)
//    {
//      if (this.Agent.IsDetachedFromFormation)
//      {
//        this.SetBehaviorValueSet(HumanAIComponent.BehaviorValueSet.DefaultDetached);
//      }
//      else
//      {
//        switch (movementOrder)
//        {
//          case MovementOrder.MovementOrderEnum.Charge:
//          case MovementOrder.MovementOrderEnum.ChargeToTarget:
//            this.SetBehaviorValueSet(HumanAIComponent.BehaviorValueSet.Charge);
//            break;
//          case MovementOrder.MovementOrderEnum.Follow:
//label_5:
//            this.SetBehaviorValueSet(HumanAIComponent.BehaviorValueSet.Follow);
//            break;
//          default:
//            switch (arrangementOrder)
//            {
//              case ArrangementOrder.ArrangementOrderEnum.Circle:
//              case ArrangementOrder.ArrangementOrderEnum.ShieldWall:
//              case ArrangementOrder.ArrangementOrderEnum.Square:
//                this.SetBehaviorValueSet(HumanAIComponent.BehaviorValueSet.DefensiveArrangementMove);
//                return;
//              case ArrangementOrder.ArrangementOrderEnum.Column:
//                goto label_5;
//              default:
//                this.SetBehaviorValueSet(HumanAIComponent.BehaviorValueSet.DefaultMove);
//                return;
//            }
//        }
//      }
//    }

//    public void ForceDisablePickUpForAgent() => this._forceDisableItemPickup = true;

//    public void SetScriptedPositionAndDirectionTimed(
//      Vec2 position,
//      float directionAsRotationInRadians,
//      float duration)
//    {
//      this._scriptedFrameTimer = duration;
//      WorldPosition worldPosition = this.Agent.GetWorldPosition();
//      worldPosition.SetVec2(position);
//      this.Agent.SetScriptedPositionAndDirection(ref worldPosition, directionAsRotationInRadians, false);
//    }

//    public void DisableTimedScriptedMovement()
//    {
//      this._scriptedFrameTimer = -1f;
//      this.Agent.DisableScriptedMovement();
//    }

//    [EngineStruct("behavior_values_struct", false, null)]
//    public struct BehaviorValues
//    {
//      public float y1;
//      public float x2;
//      public float y2;
//      public float x3;
//      public float y3;

//      public float GetValueAt(float x)
//      {
//        if ((double) x <= (double) this.x2)
//          return (this.y2 - this.y1) * x / this.x2 + this.y1;
//        return (double) x <= (double) this.x3 ? (float) (((double) this.y3 - (double) this.y2) * ((double) x - (double) this.x2) / ((double) this.x3 - (double) this.x2)) + this.y2 : this.y3;
//      }
//    }

//    public enum AISimpleBehaviorKind
//    {
//      GoToPos,
//      Melee,
//      Ranged,
//      ChargeHorseback,
//      RangedHorseback,
//      AttackEntityMelee,
//      AttackEntityRanged,
//      Count,
//    }

//    public enum BehaviorValueSet
//    {
//      Default,
//      DefensiveArrangementMove,
//      Follow,
//      DefaultMove,
//      Charge,
//      DefaultDetached,
//      Overriden,
//    }

//    public enum UsableObjectInterestKind
//    {
//      None,
//      MovingTo,
//      Defending,
//      Count,
//    }
//  }
//}
