//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.Agent
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using NetworkMessages.FromClient;
//using NetworkMessages.FromServer;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using TaleWorlds.Core;
//using TaleWorlds.DotNet;
//using TaleWorlds.Engine;
//using TaleWorlds.Library;
//using TaleWorlds.Localization;
//using TaleWorlds.MountAndBlade.Network.Messages;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public sealed class Agent : 
//    DotNetObject,
//    IAgent,
//    IFocusable,
//    IUsable,
//    IFormationUnit,
//    ITrackableBase
//  {
//    public const float BecomeTeenagerAge = 14f;
//    public const float MaxMountInteractionDistance = 1.75f;
//    public const float DismountVelocityLimit = 0.5f;
//    public const float HealthDyingThreshold = 1f;
//    public const float CachedAndFormationValuesUpdateTime = 0.5f;
//    public const float MaxInteractionDistance = 3f;
//    public const float MaxFocusDistance = 10f;
//    private const float ChainAttackDetectionTimeout = 0.75f;
//    public static readonly ActionIndexCache[] DefaultTauntActions = new ActionIndexCache[4]
//    {
//      ActionIndexCache.act_taunt_cheer_1,
//      ActionIndexCache.act_taunt_cheer_2,
//      ActionIndexCache.act_taunt_cheer_3,
//      ActionIndexCache.act_taunt_cheer_4
//    };
//    private static readonly object _stopUsingGameObjectLock = new object();
//    private static readonly object _pathCheckObjectLock = new object();
//    public Agent.OnMainAgentWieldedItemChangeDelegate OnMainAgentWieldedItemChange;
//    public Action OnAgentMountedStateChanged;
//    public Action OnAgentWieldedItemChange;
//    private readonly MBList<AgentComponent> _components;
//    private readonly Agent.CreationType _creationType;
//    private readonly List<AgentController> _agentControllers;
//    private readonly Timer _cachedAndFormationValuesUpdateTimer;
//    private Agent _cachedMountAgent;
//    private Agent _cachedRiderAgent;
//    private BasicCharacterObject _character;
//    private uint? _clothingColor1;
//    private uint? _clothingColor2;
//    private EquipmentIndex _equipmentOnMainHandBeforeUsingObject;
//    private EquipmentIndex _equipmentOnOffHandBeforeUsingObject;
//    private float _defensiveness;
//    private UIntPtr _positionPointer;
//    private UIntPtr _pointer;
//    private UIntPtr _flagsPointer;
//    private UIntPtr _indexPointer;
//    private UIntPtr _statePointer;
//    private UIntPtr _movementModePointer;
//    private UIntPtr _controllerTypePointer;
//    private UIntPtr _movementDirectionPointer;
//    private UIntPtr _primaryWieldedItemIndexPointer;
//    private float _lastMultiplayerQuickReadyDetectedTime;
//    private UIntPtr _offHandWieldedItemIndexPointer;
//    private UIntPtr _channel0CurrentActionPointer;
//    private UIntPtr _channel1CurrentActionPointer;
//    private UIntPtr _maximumForwardUnlimitedSpeed;
//    private Agent _lookAgentCache;
//    private IDetachment _detachment;
//    private readonly MBList<Agent.Hitter> _hitterList;
//    private List<(MissionWeapon, MatrixFrame, sbyte)> _attachedWeapons;
//    private float _health;
//    private MissionPeer _missionPeer;
//    private TextObject _name;
//    private float _removalTime;
//    private List<CompositeComponent> _synchedBodyComponents;
//    private Formation _formation;
//    private bool _checkIfTargetFrameIsChanged;
//    private Agent.AgentPropertiesModifiers _propertyModifiers;
//    private int _usedObjectPreferenceIndex = -1;
//    private bool _isDeleted;
//    private bool _wantsToYell;
//    private float _yellTimer;
//    private Vec3 _lastSynchedTargetDirection;
//    private Vec2 _lastSynchedTargetPosition;
//    private Agent.AgentLastHitInfo _lastHitInfo;
//    private ClothSimulatorComponent _capeClothSimulator;
//    private bool _isRemoved;
//    private WeakReference<MBAgentVisuals> _visualsWeakRef = new WeakReference<MBAgentVisuals>((MBAgentVisuals) null);
//    private readonly int _creationIndex;
//    private bool _canLeadFormationsRemotely;
//    private bool _isDetachableFromFormation = true;
//    private ItemObject _formationBanner;
//    private bool _isLadderQueueUsing;
//    private WorldPosition _changedFormationPosition;
//    private Vec2 _localPositionError;

//    public static Agent Main => Mission.Current?.MainAgent;

//    public event Agent.OnAgentHealthChangedDelegate OnAgentHealthChanged;

//    public event Agent.OnMountHealthChangedDelegate OnMountHealthChanged;

//    public bool IsPlayerControlled => this.IsMine || this.MissionPeer != null;

//    public bool IsMine => this.Controller == AgentControllerType.Player;

//    public bool IsMainAgent => this == Agent.Main;

//    public bool IsHuman => (this.GetAgentFlags() & AgentFlag.IsHumanoid) > AgentFlag.None;

//    public bool IsMount => (this.GetAgentFlags() & AgentFlag.Mountable) > AgentFlag.None;

//    public bool IsAIControlled
//    {
//      get => this.Controller == AgentControllerType.AI && !GameNetwork.IsClientOrReplay;
//    }

//    public bool IsPlayerTroop
//    {
//      get
//      {
//        return !GameNetwork.IsMultiplayer && this.Origin != null && this.Origin.Troop == Game.Current.PlayerTroop;
//      }
//    }

//    public bool IsUsingGameObject => this.CurrentlyUsedGameObject != null;

//    public bool CanLeadFormationsRemotely => this._canLeadFormationsRemotely;

//    public bool IsDetachableFromFormation => this._isDetachableFromFormation;

//    public float AgentScale => MBAPI.IMBAgent.GetAgentScale(this.GetPtr());

//    public bool CrouchMode => MBAPI.IMBAgent.GetCrouchMode(this.GetPtr());

//    public bool WalkMode => MBAPI.IMBAgent.GetWalkMode(this.GetPtr());

//    public Vec3 Position => AgentHelper.GetAgentPosition(this.PositionPointer);

//    public AgentMovementMode MovementMode
//    {
//      get => AgentHelper.GetAgentMovementMode(this._movementModePointer);
//    }

//    public Vec3 VisualPosition => MBAPI.IMBAgent.GetVisualPosition(this.GetPtr());

//    public Vec2 MovementVelocity => MBAPI.IMBAgent.GetMovementVelocity(this.GetPtr());

//    public Vec3 AverageVelocity => MBAPI.IMBAgent.GetAverageVelocity(this.GetPtr());

//    public float MovementDirectionAsAngle
//    {
//      get => AgentHelper.GetAgentMovementDirectionAsAngle(this._movementDirectionPointer);
//    }

//    public bool IsLookRotationInSlowMotion
//    {
//      get => MBAPI.IMBAgent.IsLookRotationInSlowMotion(this.GetPtr());
//    }

//    public Agent.AgentPropertiesModifiers PropertyModifiers => this._propertyModifiers;

//    public MBActionSet ActionSet => new MBActionSet(MBAPI.IMBAgent.GetActionSetNo(this.GetPtr()));

//    public MBReadOnlyList<AgentComponent> Components
//    {
//      get => (MBReadOnlyList<AgentComponent>) this._components;
//    }

//    public MBReadOnlyList<Agent.Hitter> HitterList
//    {
//      get => (MBReadOnlyList<Agent.Hitter>) this._hitterList;
//    }

//    public Agent.GuardMode CurrentGuardMode => MBAPI.IMBAgent.GetCurrentGuardMode(this.GetPtr());

//    public Agent ImmediateEnemy => MBAPI.IMBAgent.GetImmediateEnemy(this.GetPtr());

//    public bool IsDoingPassiveAttack => MBAPI.IMBAgent.GetIsDoingPassiveAttack(this.GetPtr());

//    public bool IsPassiveUsageConditionsAreMet
//    {
//      get => MBAPI.IMBAgent.GetIsPassiveUsageConditionsAreMet(this.GetPtr());
//    }

//    public float CurrentAimingError => MBAPI.IMBAgent.GetCurrentAimingError(this.GetPtr());

//    public float CurrentAimingTurbulance
//    {
//      get => MBAPI.IMBAgent.GetCurrentAimingTurbulance(this.GetPtr());
//    }

//    public Agent.UsageDirection AttackDirection
//    {
//      get => MBAPI.IMBAgent.GetAttackDirectionUsage(this.GetPtr());
//    }

//    public float WalkingSpeedLimitOfMountable
//    {
//      get => MBAPI.IMBAgent.GetWalkSpeedLimitOfMountable(this.GetPtr());
//    }

//    public Agent RiderAgent => this.GetRiderAgentAux();

//    public bool HasMount => this.MountAgent != null;

//    public bool CanLogCombatFor
//    {
//      get
//      {
//        if (this.RiderAgent != null && !this.RiderAgent.IsAIControlled)
//          return true;
//        return !this.IsMount && !this.IsAIControlled;
//      }
//    }

//    public float MissileRangeAdjusted => this.GetMissileRangeWithHeightDifference();

//    public float MaximumMissileRange => this.GetMissileRange();

//    FocusableObjectType IFocusable.FocusableObjectType
//    {
//      get => !this.IsMount ? FocusableObjectType.Agent : FocusableObjectType.Mount;
//    }

//    bool IFocusable.IsFocusable => true;

//    public string Name => this.MissionPeer == null ? this._name.ToString() : this.MissionPeer.Name;

//    public TextObject NameTextObject
//    {
//      get => this.MissionPeer == null ? this._name : new TextObject("{=!}" + this.MissionPeer.Name);
//    }

//    public AgentMovementLockedState MovementLockedState => this.GetMovementLockedState();

//    public Monster Monster { get; }

//    public bool IsRunningAway { get; private set; }

//    public BodyProperties BodyPropertiesValue { get; private set; }

//    public CommonAIComponent CommonAIComponent { get; private set; }

//    public HumanAIComponent HumanAIComponent { get; private set; }

//    public int BodyPropertiesSeed { get; internal set; }

//    public float LastRangedHitTime { get; private set; } = float.MinValue;

//    public float LastMeleeHitTime { get; private set; } = float.MinValue;

//    public float LastRangedAttackTime { get; private set; } = float.MinValue;

//    public float LastMeleeAttackTime { get; private set; } = float.MinValue;

//    public bool IsFemale { get; set; }

//    public ItemObject Banner => this.Equipment?.GetBanner();

//    public ItemObject FormationBanner => this._formationBanner;

//    public MissionWeapon WieldedWeapon
//    {
//      get
//      {
//        EquipmentIndex wieldedItemIndex = this.GetPrimaryWieldedItemIndex();
//        return wieldedItemIndex < EquipmentIndex.WeaponItemBeginSlot ? MissionWeapon.Invalid : this.Equipment[wieldedItemIndex];
//      }
//    }

//    public bool IsItemUseDisabled { get; set; }

//    public bool SyncHealthToAllClients { get; private set; }

//    public UsableMissionObject CurrentlyUsedGameObject { get; private set; }

//    public bool CombatActionsEnabled
//    {
//      get
//      {
//        return this.CurrentlyUsedGameObject == null || !this.CurrentlyUsedGameObject.DisableCombatActionsOnUse;
//      }
//    }

//    public Mission Mission { get; private set; }

//    public bool IsHero => this.Character != null && this.Character.IsHero;

//    public int Index { get; }

//    public MissionEquipment Equipment { get; private set; }

//    public TextObject AgentRole { get; set; }

//    public bool HasBeenBuilt { get; private set; }

//    public Agent.MortalityState CurrentMortalityState { get; private set; }

//    public TaleWorlds.Core.Equipment SpawnEquipment { get; private set; }

//    public FormationPositionPreference FormationPositionPreference { get; set; }

//    public bool RandomizeColors { get; private set; }

//    public float CharacterPowerCached { get; private set; }

//    public float WalkSpeedCached { get; private set; }

//    public IAgentOriginBase Origin { get; set; }

//    public Team Team { get; private set; }

//    public int KillCount { get; set; }

//    public AgentDrivenProperties AgentDrivenProperties { get; private set; }

//    public float BaseHealthLimit { get; set; }

//    public string HorseCreationKey { get; private set; }

//    public float HealthLimit { get; set; }

//    public bool IsRangedCached => this.Equipment.ContainsNonConsumableRangedWeaponWithAmmo();

//    public bool HasAnyRangedWeaponCached
//    {
//      get => this.IsRangedCached || this.Equipment.ContainsThrownWeapon();
//    }

//    public bool HasMeleeWeaponCached => this.Equipment.ContainsMeleeWeapon();

//    public bool HasShieldCached => this.Equipment.ContainsShield();

//    public bool HasSpearCached => this.Equipment.ContainsSpear();

//    public bool HasThrownCached => this.Equipment.ContainsThrownWeapon();

//    public Agent.AIStateFlag AIStateFlags
//    {
//      get => MBAPI.IMBAgent.GetAIStateFlags(this.GetPtr());
//      set => MBAPI.IMBAgent.SetAIStateFlags(this.GetPtr(), value);
//    }

//    public MatrixFrame Frame
//    {
//      get
//      {
//        MatrixFrame outFrame = new MatrixFrame();
//        MBAPI.IMBAgent.GetRotationFrame(this.GetPtr(), ref outFrame);
//        return outFrame;
//      }
//    }

//    public Agent.MovementControlFlag MovementFlags
//    {
//      get => MBAPI.IMBAgent.GetMovementFlags(this.GetPtr());
//      set => MBAPI.IMBAgent.SetMovementFlags(this.GetPtr(), value);
//    }

//    public Vec2 MovementInputVector
//    {
//      get => MBAPI.IMBAgent.GetMovementInputVector(this.GetPtr());
//      set => MBAPI.IMBAgent.SetMovementInputVector(this.GetPtr(), value);
//    }

//    public CapsuleData CollisionCapsule
//    {
//      get
//      {
//        CapsuleData collisionCapsule = new CapsuleData();
//        MBAPI.IMBAgent.GetCollisionCapsule(this.GetPtr(), ref collisionCapsule);
//        return collisionCapsule;
//      }
//    }

//    public Vec3 CollisionCapsuleCenter
//    {
//      get
//      {
//        (Vec3, Vec3) boxMinMax = this.CollisionCapsule.GetBoxMinMax();
//        return (boxMinMax.Item1 + boxMinMax.Item2) * 0.5f;
//      }
//    }

//    public MBAgentVisuals AgentVisuals
//    {
//      get
//      {
//        MBAgentVisuals target;
//        if (!this._visualsWeakRef.TryGetTarget(out target))
//        {
//          target = MBAPI.IMBAgent.GetAgentVisuals(this.GetPtr());
//          this._visualsWeakRef.SetTarget(target);
//        }
//        return target;
//      }
//    }

//    public bool HeadCameraMode
//    {
//      get => MBAPI.IMBAgent.GetHeadCameraMode(this.GetPtr());
//      set => MBAPI.IMBAgent.SetHeadCameraMode(this.GetPtr(), value);
//    }

//    public Agent MountAgent
//    {
//      get => this.GetMountAgentAux();
//      private set
//      {
//        this.SetMountAgent(value);
//        this.UpdateAgentStats();
//      }
//    }

//    public IDetachment Detachment
//    {
//      get => this._detachment;
//      set
//      {
//        this._detachment = value;
//        if (this._detachment == null)
//          return;
//        this.Formation?.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(this);
//      }
//    }

//    public bool IsPaused
//    {
//      get => this.AIStateFlags.HasAnyFlag<Agent.AIStateFlag>(Agent.AIStateFlag.Paused);
//      set
//      {
//        if (value)
//          this.AIStateFlags |= Agent.AIStateFlag.Paused;
//        else
//          this.AIStateFlags &= ~Agent.AIStateFlag.Paused;
//      }
//    }

//    public bool IsDetachedFromFormation => this._detachment != null;

//    public Agent.WatchState CurrentWatchState
//    {
//      get
//      {
//        switch (this.AIStateFlags & Agent.AIStateFlag.Alarmed)
//        {
//          case Agent.AIStateFlag.Cautious:
//            return Agent.WatchState.Cautious;
//          case Agent.AIStateFlag.Alarmed:
//            return Agent.WatchState.Alarmed;
//          default:
//            return Agent.WatchState.Patrolling;
//        }
//      }
//      private set
//      {
//        switch (value)
//        {
//          case Agent.WatchState.Patrolling:
//            this.SetAlarmState(Agent.AIStateFlag.None);
//            break;
//          case Agent.WatchState.Cautious:
//            this.SetAlarmState(Agent.AIStateFlag.Cautious);
//            break;
//          case Agent.WatchState.Alarmed:
//            this.SetAlarmState(Agent.AIStateFlag.Alarmed);
//            break;
//          default:
//            TaleWorlds.Library.Debug.FailedAssert("false", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Agent.cs", nameof (CurrentWatchState), 929);
//            break;
//        }
//      }
//    }

//    public float Defensiveness
//    {
//      get => this._defensiveness;
//      set
//      {
//        if ((double) MathF.Abs(value - this._defensiveness) <= 9.9999997473787516E-05)
//          return;
//        this._defensiveness = value;
//        this.UpdateAgentProperties();
//      }
//    }

//    public Formation Formation
//    {
//      get => this._formation;
//      set
//      {
//        if (this._formation == value)
//          return;
//        if (GameNetwork.IsServer && this.HasBeenBuilt && this.Mission.GetMissionBehavior<MissionNetworkComponent>() != null)
//        {
//          GameNetwork.BeginBroadcastModuleEvent();
//          GameNetwork.WriteMessage((GameNetworkMessage) new AgentSetFormation(this.Index, value != null ? value.Index : -1));
//          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
//        }
//        this.SetNativeFormationNo(value != null ? value.Index : -1);
//        IDetachment detachment1 = (IDetachment) null;
//        float num = 0.0f;
//        if (this._formation != null)
//        {
//          if (this.IsDetachedFromFormation)
//          {
//            detachment1 = this.Detachment;
//            num = this.DetachmentWeight;
//          }
//          this._formation.RemoveUnit(this);
//          foreach (IDetachment detachment2 in (List<IDetachment>) this._formation.Detachments)
//          {
//            if (!detachment2.IsUsedByFormation(value))
//              this.Team.DetachmentManager.RemoveScoresOfAgentFromDetachment(this, detachment2);
//          }
//        }
//        this._formation = value;
//        if (this._formation != null)
//        {
//          if (!this._formation.HasBeenPositioned)
//            this._formation.SetPositioning(new WorldPosition?(this.GetWorldPosition()), new Vec2?(this.LookDirection.AsVec2));
//          this._formation.AddUnit(this);
//          if (detachment1 != null && this._formation.Detachments.IndexOf(detachment1) >= 0 && detachment1.IsStandingPointAvailableForAgent(this))
//          {
//            detachment1.AddAgent(this);
//            this._formation.DetachUnit(this, detachment1.IsLoose);
//            this.Detachment = detachment1;
//            this.DetachmentWeight = num;
//          }
//        }
//        foreach (AgentComponent component in (List<AgentComponent>) this._components)
//          component.OnFormationSet();
//        this.ForceUpdateCachedAndFormationValues(this._formation != null && this._formation.PostponeCostlyOperations, false);
//      }
//    }

//    IFormationUnit IFormationUnit.FollowedUnit
//    {
//      get
//      {
//        if (!this.IsActive())
//          return (IFormationUnit) null;
//        return this.IsAIControlled ? (IFormationUnit) this.GetFollowedUnit() : (IFormationUnit) null;
//      }
//    }

//    public bool IsShieldUsageEncouraged
//    {
//      get
//      {
//        return this.Formation.FiringOrder.OrderEnum == FiringOrder.RangedWeaponUsageOrderEnum.HoldYourFire || !this.Equipment.HasAnyWeaponWithFlags(WeaponFlags.RangedWeapon | WeaponFlags.NotUsableWithOneHand);
//      }
//    }

//    public bool IsPlayerUnit => this.IsPlayerControlled || this.IsPlayerTroop;

//    public AgentControllerType Controller
//    {
//      get => AgentHelper.GetAgentControllerType(this._controllerTypePointer);
//      set
//      {
//        AgentControllerType controller = this.Controller;
//        if (value == controller)
//          return;
//        if (value == AgentControllerType.Player && this.IsDetachedFromFormation)
//        {
//          this._detachment.RemoveAgent(this);
//          this._formation?.AttachUnit(this);
//        }
//        MBAPI.IMBAgent.SetController(this.GetPtr(), value);
//        bool flag = value == AgentControllerType.Player;
//        if (flag)
//        {
//          this.Mission.MainAgent = this;
//          this.SetAgentFlags(this.GetAgentFlags() | AgentFlag.CanRide);
//        }
//        this.Formation?.OnAgentControllerChanged(this, controller);
//        if (value != AgentControllerType.AI && this.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.IsHumanoid))
//        {
//          this.MountAgent?.SetMaximumSpeedLimit(-1f, false);
//          this.SetMaximumSpeedLimit(-1f, false);
//          if (this.WalkMode)
//            this.EventControlFlags |= Agent.EventControlFlag.Run;
//        }
//        foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
//          missionBehavior.OnAgentControllerChanged(this, controller);
//        if (flag)
//        {
//          foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
//            missionBehavior.OnAgentControllerSetToPlayer(this.Mission.MainAgent);
//        }
//        if (!GameNetwork.IsServer)
//          return;
//        MissionPeer missionPeer = this.MissionPeer;
//        NetworkCommunicator networkPeer = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
//        if (networkPeer == null || networkPeer.IsServerPeer)
//          return;
//        GameNetwork.BeginModuleEventAsServer(networkPeer);
//        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentIsPlayer(this.Index, this.Controller != AgentControllerType.AI));
//        GameNetwork.EndModuleEventAsServer();
//      }
//    }

//    public uint ClothingColor1
//    {
//      get
//      {
//        if (this._clothingColor1.HasValue)
//          return this._clothingColor1.Value;
//        if (this.Team != null)
//          return this.Team.Color;
//        TaleWorlds.Library.Debug.FailedAssert("Clothing color is not set.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Agent.cs", nameof (ClothingColor1), 1146);
//        return uint.MaxValue;
//      }
//    }

//    public uint ClothingColor2 => this._clothingColor2 ?? this.ClothingColor1;

//    public MatrixFrame LookFrame
//    {
//      get
//      {
//        return new MatrixFrame()
//        {
//          origin = this.Position,
//          rotation = this.LookRotation
//        };
//      }
//    }

//    public float LookDirectionAsAngle
//    {
//      get => MBAPI.IMBAgent.GetLookDirectionAsAngle(this.GetPtr());
//      set => MBAPI.IMBAgent.SetLookDirectionAsAngle(this.GetPtr(), value);
//    }

//    public Mat3 LookRotation
//    {
//      get
//      {
//        Mat3 lookRotation;
//        lookRotation.f = this.LookDirection;
//        lookRotation.u = Vec3.Up;
//        lookRotation.s = Vec3.CrossProduct(lookRotation.f, lookRotation.u);
//        double num = (double) lookRotation.s.Normalize();
//        lookRotation.u = Vec3.CrossProduct(lookRotation.s, lookRotation.f);
//        return lookRotation;
//      }
//    }

//    public bool IsLookDirectionLocked
//    {
//      get => MBAPI.IMBAgent.GetIsLookDirectionLocked(this.GetPtr());
//      set => MBAPI.IMBAgent.SetIsLookDirectionLocked(this.GetPtr(), value);
//    }

//    public bool IsCheering
//    {
//      get
//      {
//        ActionIndexCache currentAction = this.GetCurrentAction(1);
//        for (int index = 0; index < Agent.DefaultTauntActions.Length; ++index)
//        {
//          ref ActionIndexCache local = ref Agent.DefaultTauntActions[index];
//          if (Agent.DefaultTauntActions[index] == currentAction)
//            return true;
//        }
//        return false;
//      }
//    }

//    public bool IsInBeingStruckAction
//    {
//      get
//      {
//        return MBMath.IsBetween((int) this.GetCurrentActionType(1), 48, 52) || MBMath.IsBetween((int) this.GetCurrentActionType(0), 48, 52);
//      }
//    }

//    public MissionPeer MissionPeer
//    {
//      get => this._missionPeer;
//      set
//      {
//        if (this._missionPeer == value)
//          return;
//        MissionPeer missionPeer = this._missionPeer;
//        this._missionPeer = value;
//        if (missionPeer != null && missionPeer.ControlledAgent == this)
//          missionPeer.ControlledAgent = (Agent) null;
//        if (this._missionPeer != null && this._missionPeer.ControlledAgent != this)
//        {
//          this._missionPeer.ControlledAgent = this;
//          if (GameNetwork.IsServerOrRecorder)
//          {
//            this.SyncHealthToClients();
//            Agent.OnAgentHealthChangedDelegate agentHealthChanged = this.OnAgentHealthChanged;
//            if (agentHealthChanged != null)
//              agentHealthChanged(this, this.Health, this.Health);
//          }
//        }
//        if (value != null)
//          this.Controller = value.IsMine ? AgentControllerType.Player : AgentControllerType.None;
//        if (!GameNetwork.IsServer || !this.IsHuman || this._isDeleted)
//          return;
//        NetworkCommunicator networkPeer = value != null ? value.GetNetworkPeer() : (NetworkCommunicator) null;
//        this.SetNetworkPeer(networkPeer);
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentPeer(this.Index, networkPeer));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
//      }
//    }

//    public BasicCharacterObject Character
//    {
//      get => this._character;
//      set
//      {
//        this._character = value;
//        if (value == null)
//          return;
//        this.Health = (float) this._character.HitPoints;
//        this.BaseHealthLimit = (float) this._character.MaxHitPoints();
//        this.HealthLimit = this.BaseHealthLimit;
//        this.CharacterPowerCached = value.GetPower();
//        this._name = value.Name;
//        this.IsFemale = value.IsFemale;
//      }
//    }

//    public float LastDetachmentTickAgentTime { get; private set; }

//    public MissionPeer OwningAgentMissionPeer { get; private set; }

//    public MissionRepresentativeBase MissionRepresentative { get; private set; }

//    public bool IsInLadderQueue { get; private set; }

//    IMissionTeam IAgent.Team => (IMissionTeam) this.Team;

//    IFormationArrangement IFormationUnit.Formation => this._formation.Arrangement;

//    int IFormationUnit.FormationFileIndex { get; set; }

//    int IFormationUnit.FormationRankIndex { get; set; }

//    public Vec2 LocalPositionError
//    {
//      get
//      {
//        return this._localPositionError * this.Formation.Interval * this.Formation.UnitDiameter * 0.75f;
//      }
//      private set => this._localPositionError = value;
//    }

//    public float DetachmentWeight { get; private set; }

//    public int DetachmentIndex { get; private set; } = -1;

//    public bool IsFormationFrameEnabled { get; private set; }

//    private UIntPtr Pointer => this._pointer;

//    private UIntPtr FlagsPointer => this._flagsPointer;

//    private UIntPtr PositionPointer => this._positionPointer;

//    internal Agent(
//      Mission mission,
//      Mission.AgentCreationResult creationResult,
//      Agent.CreationType creationType,
//      Monster monster,
//      int creationIndex)
//    {
//      this.AgentRole = TextObject.GetEmpty();
//      this.Mission = mission;
//      this.Index = creationResult.Index;
//      this._pointer = creationResult.AgentPtr;
//      this._positionPointer = creationResult.PositionPtr;
//      this._flagsPointer = creationResult.FlagsPtr;
//      this._indexPointer = creationResult.IndexPtr;
//      this._statePointer = creationResult.StatePtr;
//      this._movementModePointer = creationResult.MovementModePointer;
//      this._controllerTypePointer = creationResult.ControllerPointer;
//      this._movementDirectionPointer = creationResult.MovementDirectionPointer;
//      this._primaryWieldedItemIndexPointer = creationResult.PrimaryWieldedItemIndexPointer;
//      this._offHandWieldedItemIndexPointer = creationResult.OffHandWieldedItemIndexPointer;
//      this._channel0CurrentActionPointer = creationResult.Channel0CurrentActionPointer;
//      this._channel1CurrentActionPointer = creationResult.Channel1CurrentActionPointer;
//      this._maximumForwardUnlimitedSpeed = creationResult.MaximumForwardUnlimitedSpeed;
//      this._lastHitInfo = new Agent.AgentLastHitInfo();
//      this._lastHitInfo.Initialize();
//      MBAPI.IMBAgent.SetMonoObject(this.GetPtr(), this);
//      this.Monster = monster;
//      this.KillCount = 0;
//      this.HasBeenBuilt = false;
//      this._creationType = creationType;
//      this._agentControllers = new List<AgentController>();
//      this._components = new MBList<AgentComponent>();
//      this._hitterList = new MBList<Agent.Hitter>();
//      ((IFormationUnit) this).FormationFileIndex = -1;
//      ((IFormationUnit) this).FormationRankIndex = -1;
//      this._synchedBodyComponents = (List<CompositeComponent>) null;
//      this._cachedAndFormationValuesUpdateTimer = new Timer(this.Mission.CurrentTime, (float) (0.44999998807907104 + (double) MBRandom.RandomFloat * 0.10000000149011612));
//      this._creationIndex = creationIndex;
//    }

//    bool IAgent.IsEnemyOf(IAgent agent) => this.IsEnemyOf((Agent) agent);

//    bool IAgent.IsFriendOf(IAgent agent) => this.IsFriendOf((Agent) agent);

//    public Vec3 LookDirection
//    {
//      get => MBAPI.IMBAgent.GetLookDirection(this.GetPtr());
//      set => MBAPI.IMBAgent.SetLookDirection(this.GetPtr(), value);
//    }

//    public bool IsLookDirectionLow => (double) this.LookDirection.z < 0.0;

//    public float Health
//    {
//      get => this._health;
//      set
//      {
//        float comparedValue = value.ApproximatelyEqualsTo(0.0f) ? 0.0f : (float) MathF.Ceiling(value);
//        if (this._health.ApproximatelyEqualsTo(comparedValue))
//          return;
//        float health = this._health;
//        this._health = comparedValue;
//        if (GameNetwork.IsServerOrRecorder)
//          this.SyncHealthToClients();
//        Agent.OnAgentHealthChangedDelegate agentHealthChanged = this.OnAgentHealthChanged;
//        if (agentHealthChanged != null)
//          agentHealthChanged(this, health, this._health);
//        if (this.RiderAgent == null)
//          return;
//        Agent.OnMountHealthChangedDelegate mountHealthChanged = this.RiderAgent.OnMountHealthChanged;
//        if (mountHealthChanged == null)
//          return;
//        mountHealthChanged(this.RiderAgent, this, health, this._health);
//      }
//    }

//    public float Age
//    {
//      get => this.BodyPropertiesValue.Age;
//      set
//      {
//        double age = (double) value;
//        BodyProperties bodyPropertiesValue = this.BodyPropertiesValue;
//        double weight = (double) bodyPropertiesValue.Weight;
//        bodyPropertiesValue = this.BodyPropertiesValue;
//        double build = (double) bodyPropertiesValue.Build;
//        this.BodyPropertiesValue = new BodyProperties(new DynamicBodyProperties((float) age, (float) weight, (float) build), this.BodyPropertiesValue.StaticProperties);
//        this.BodyPropertiesValue = this.BodyPropertiesValue;
//      }
//    }

//    public Vec3 Velocity
//    {
//      get
//      {
//        Vec3 v = new Vec3(MBAPI.IMBAgent.GetMovementVelocity(this.GetPtr()));
//        return this.Frame.rotation.TransformToParent(in v);
//      }
//    }

//    [MBCallback(null, false)]
//    internal void SetAgentAIPerformingRetreatBehavior(bool isAgentAIPerformingRetreatBehavior)
//    {
//      if (GameNetwork.IsClientOrReplay || this.Mission == null)
//        return;
//      this.IsRunningAway = isAgentAIPerformingRetreatBehavior;
//    }

//    public Agent.EventControlFlag EventControlFlags
//    {
//      get => MBAPI.IMBAgent.GetEventControlFlags(this.GetPtr());
//      set => MBAPI.IMBAgent.SetEventControlFlags(this.GetPtr(), value);
//    }

//    public bool GetHasOnAiInputSetCallback()
//    {
//      return MBAPI.IMBAgent.GetHasOnAiInputSetCallback(this.GetPtr());
//    }

//    public void SetHasOnAiInputSetCallback(bool value)
//    {
//      MBAPI.IMBAgent.SetHasOnAiInputSetCallback(this.GetPtr(), value);
//    }

//    [MBCallback(null, false)]
//    internal void OnAIInputSet(
//      ref Agent.EventControlFlag eventFlag,
//      ref Agent.MovementControlFlag movementFlag,
//      ref Vec2 inputVector)
//    {
//      foreach (AgentComponent component in (List<AgentComponent>) this._components)
//        component.OnAIInputSet(ref eventFlag, ref movementFlag, ref inputVector);
//    }

//    [MBCallback(null, false)]
//    public float GetMissileRangeWithHeightDifferenceAux(float targetZ)
//    {
//      return MBAPI.IMBAgent.GetMissileRangeWithHeightDifference(this.GetPtr(), targetZ);
//    }

//    [MBCallback(null, false)]
//    internal int GetFormationUnitSpacing() => this.Formation.UnitSpacing;

//    [MBCallback(null, false)]
//    public string GetSoundAndCollisionInfoClassName()
//    {
//      return this.Monster.SoundAndCollisionInfoClassName;
//    }

//    [MBCallback(null, false)]
//    internal bool IsInSameFormationWith(Agent otherAgent)
//    {
//      Formation formation = otherAgent.Formation;
//      return this.Formation != null && formation != null && this.Formation == formation;
//    }

//    [MBCallback(null, false)]
//    internal void OnWeaponSwitchingToAlternativeStart(EquipmentIndex slotIndex, int usageIndex)
//    {
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new StartSwitchingWeaponUsageIndex(this.Index, slotIndex, usageIndex, Agent.MovementFlagToDirection(this.MovementFlags)));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//    }

//    [MBCallback(null, false)]
//    internal void OnWeaponReloadPhaseChange(EquipmentIndex slotIndex, short reloadPhase)
//    {
//      this.Equipment.SetReloadPhaseOfSlot(slotIndex, reloadPhase);
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new SetWeaponReloadPhase(this.Index, slotIndex, reloadPhase));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//    }

//    [MBCallback(null, false)]
//    internal void OnWeaponAmmoReload(
//      EquipmentIndex slotIndex,
//      EquipmentIndex ammoSlotIndex,
//      short totalAmmo)
//    {
//      if (this.Equipment[slotIndex].CurrentUsageItem.IsRangedWeapon)
//      {
//        this.Equipment.SetReloadedAmmoOfSlot(slotIndex, ammoSlotIndex, totalAmmo);
//        if (GameNetwork.IsServerOrRecorder)
//        {
//          GameNetwork.BeginBroadcastModuleEvent();
//          GameNetwork.WriteMessage((GameNetworkMessage) new SetWeaponAmmoData(this.Index, slotIndex, ammoSlotIndex, totalAmmo));
//          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        }
//      }
//      this.UpdateAgentProperties();
//    }

//    [MBCallback(null, false)]
//    internal void OnWeaponAmmoConsume(EquipmentIndex slotIndex, short totalAmmo)
//    {
//      if (this.Equipment[slotIndex].CurrentUsageItem.IsRangedWeapon)
//      {
//        this.Equipment.SetConsumedAmmoOfSlot(slotIndex, totalAmmo);
//        if (GameNetwork.IsServerOrRecorder)
//        {
//          GameNetwork.BeginBroadcastModuleEvent();
//          GameNetwork.WriteMessage((GameNetworkMessage) new SetWeaponAmmoData(this.Index, slotIndex, EquipmentIndex.None, totalAmmo));
//          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        }
//      }
//      this.UpdateAgentProperties();
//    }

//    public AgentState State
//    {
//      get => AgentHelper.GetAgentState(this._statePointer);
//      set
//      {
//        if (this.State == value)
//          return;
//        MBAPI.IMBAgent.SetStateFlags(this.GetPtr(), value);
//      }
//    }

//    [MBCallback(null, false)]
//    internal void OnShieldDamaged(EquipmentIndex slotIndex, int inflictedDamage)
//    {
//      int hitPoints = MathF.Max(0, (int) this.Equipment[slotIndex].HitPoints - inflictedDamage);
//      this.ChangeWeaponHitPoints(slotIndex, (short) hitPoints);
//      if (hitPoints != 0)
//        return;
//      this.RemoveEquippedWeapon(slotIndex);
//    }

//    public MissionWeapon WieldedOffhandWeapon
//    {
//      get
//      {
//        EquipmentIndex wieldedItemIndex = this.GetOffhandWieldedItemIndex();
//        return wieldedItemIndex < EquipmentIndex.WeaponItemBeginSlot ? MissionWeapon.Invalid : this.Equipment[wieldedItemIndex];
//      }
//    }

//    [MBCallback(null, false)]
//    internal void OnWeaponAmmoRemoved(EquipmentIndex slotIndex)
//    {
//      MissionWeapon ammoWeapon = this.Equipment[slotIndex];
//      ammoWeapon = ammoWeapon.AmmoWeapon;
//      if (ammoWeapon.IsEmpty)
//        return;
//      this.Equipment.SetConsumedAmmoOfSlot(slotIndex, (short) 0);
//    }

//    [MBCallback(null, false)]
//    internal void OnMount(Agent mount)
//    {
//      if (!GameNetwork.IsClientOrReplay)
//      {
//        if (mount.IsAIControlled && mount.IsRetreating(false))
//          mount.StopRetreatingMoraleComponent();
//        this.CheckToDropFlaggedItem();
//      }
//      if (this.HasBeenBuilt)
//      {
//        foreach (AgentComponent component in (List<AgentComponent>) this._components)
//          component.OnMount(mount);
//        this.Mission.OnAgentMount(this);
//      }
//      this.UpdateAgentStats();
//      Action mountedStateChanged = this.OnAgentMountedStateChanged;
//      if (mountedStateChanged != null)
//        mountedStateChanged();
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      mount.SyncHealthToClients();
//    }

//    [MBCallback(null, false)]
//    internal void OnDismount(Agent mount)
//    {
//      if (!GameNetwork.IsClientOrReplay)
//      {
//        this.Formation?.OnAgentLostMount(this);
//        this.CheckToDropFlaggedItem();
//      }
//      foreach (AgentComponent component in (List<AgentComponent>) this._components)
//        component.OnDismount(mount);
//      this.Mission.OnAgentDismount(this);
//      if (!this.IsActive())
//        return;
//      this.UpdateAgentStats();
//      Action mountedStateChanged = this.OnAgentMountedStateChanged;
//      if (mountedStateChanged == null)
//        return;
//      mountedStateChanged();
//    }

//    [MBCallback(null, false)]
//    internal void OnAgentAlarmedStateChanged(Agent.AIStateFlag flag)
//    {
//      foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
//        missionBehavior.OnAgentAlarmedStateChanged(this, flag);
//    }

//    [MBCallback(null, false)]
//    internal void OnRetreating()
//    {
//      if (GameNetwork.IsClientOrReplay || this.Mission == null || this.Mission.MissionEnded)
//        return;
//      if (this.IsUsingGameObject && !(this.CurrentlyUsedGameObject is SpawnedItemEntity))
//        this.StopUsingGameObjectMT();
//      foreach (AgentComponent component in (List<AgentComponent>) this._components)
//        component.OnRetreating();
//    }

//    [MBCallback(null, true)]
//    internal void UpdateMountAgentCache(Agent newMountAgent)
//    {
//      this._cachedMountAgent = newMountAgent;
//    }

//    [MBCallback(null, false)]
//    internal void UpdateRiderAgentCache(Agent newRiderAgent)
//    {
//      this._cachedRiderAgent = newRiderAgent;
//      if (newRiderAgent == null)
//        Mission.Current.AddMountWithoutRider(this);
//      else
//        Mission.Current.RemoveMountWithoutRider(this);
//    }

//    [MBCallback(null, false)]
//    public void UpdateAgentStats()
//    {
//      if (!this.IsActive())
//        return;
//      this.UpdateAgentProperties();
//    }

//    [MBCallback(null, true)]
//    public float GetWeaponInaccuracy(EquipmentIndex weaponSlotIndex, int weaponUsageIndex)
//    {
//      WeaponComponentData componentDataForUsage = this.Equipment[weaponSlotIndex].GetWeaponComponentDataForUsage(weaponUsageIndex);
//      int effectiveSkill = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(this, componentDataForUsage.RelevantSkill);
//      return MissionGameModels.Current.AgentStatCalculateModel.GetWeaponInaccuracy(this, componentDataForUsage, effectiveSkill);
//    }

//    [MBCallback(null, false)]
//    public float DebugGetHealth() => this.Health;

//    public void SetTargetPosition(Vec2 value)
//    {
//      MBAPI.IMBAgent.SetTargetPosition(this.GetPtr(), ref value);
//    }

//    public void SetTargetZ(float targetZ) => MBAPI.IMBAgent.SetTargetZ(this.GetPtr(), targetZ);

//    public void SetTargetUp(in Vec3 targetUp)
//    {
//      MBAPI.IMBAgent.SetTargetUp(this.GetPtr(), in targetUp);
//    }

//    public void SetCanLeadFormationsRemotely(bool value) => this._canLeadFormationsRemotely = value;

//    public void SetAveragePingInMilliseconds(double averagePingInMilliseconds)
//    {
//      MBAPI.IMBAgent.SetAveragePingInMilliseconds(this.GetPtr(), averagePingInMilliseconds);
//    }

//    public void SetTargetPositionAndDirection(in Vec2 targetPosition, in Vec3 targetDirection)
//    {
//      MBAPI.IMBAgent.SetTargetPositionAndDirection(this.GetPtr(), in targetPosition, in targetDirection);
//    }

//    public void AddAcceleration(in Vec3 acceleration)
//    {
//      MBAPI.IMBAgent.AddAcceleration(this.GetPtr(), in acceleration);
//    }

//    public void SetWeaponGuard(Agent.UsageDirection direction)
//    {
//      MBAPI.IMBAgent.SetWeaponGuard(this.GetPtr(), direction);
//    }

//    public void SetWatchState(Agent.WatchState watchState) => this.CurrentWatchState = watchState;

//    public bool IsAlarmStateNormal()
//    {
//      return (this.AIStateFlags & Agent.AIStateFlag.Alarmed) == Agent.AIStateFlag.None;
//    }

//    public bool IsCautious()
//    {
//      return (this.AIStateFlags & Agent.AIStateFlag.Alarmed) == Agent.AIStateFlag.Cautious;
//    }

//    public bool IsPatrollingCautious()
//    {
//      return (this.AIStateFlags & Agent.AIStateFlag.Alarmed) == Agent.AIStateFlag.PatrollingCautious;
//    }

//    public bool IsAlarmed()
//    {
//      return (this.AIStateFlags & Agent.AIStateFlag.Alarmed) == Agent.AIStateFlag.Alarmed;
//    }

//    public bool SetAlarmState(Agent.AIStateFlag alarmStateFlag)
//    {
//      if ((this.AIStateFlags & Agent.AIStateFlag.Alarmed) == alarmStateFlag)
//        return false;
//      MBAPI.IMBAgent.SetAIAlarmState(this.GetPtr(), alarmStateFlag);
//      return true;
//    }

//    public void SetTargetFormationIndex(int targetFormationIndex)
//    {
//      MBAPI.IMBAgent.SetTargetFormationIndex(this.GetPtr(), targetFormationIndex);
//    }

//    [MBCallback(null, false)]
//    internal void OnWieldedItemIndexChange(
//      bool isOffHand,
//      bool isWieldedInstantly,
//      bool isWieldedOnSpawn)
//    {
//      if (this.IsMainAgent)
//      {
//        Agent.OnMainAgentWieldedItemChangeDelegate wieldedItemChange = this.OnMainAgentWieldedItemChange;
//        if (wieldedItemChange != null)
//          wieldedItemChange();
//      }
//      Action wieldedItemChange1 = this.OnAgentWieldedItemChange;
//      if (wieldedItemChange1 != null)
//        wieldedItemChange1();
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        int mainHandCurUsageIndex = 0;
//        EquipmentIndex wieldedItemIndex = this.GetPrimaryWieldedItemIndex();
//        if (wieldedItemIndex != EquipmentIndex.None)
//          mainHandCurUsageIndex = this.Equipment[wieldedItemIndex].CurrentUsageIndex;
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new SetWieldedItemIndex(this.Index, isOffHand, isWieldedInstantly, isWieldedOnSpawn, isOffHand ? this.GetOffhandWieldedItemIndex() : this.GetPrimaryWieldedItemIndex(), mainHandCurUsageIndex));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      this.CheckEquipmentForCapeClothSimulationStateChange();
//    }

//    public void StartRagdollAsCorpse() => MBAPI.IMBAgent.StartRagdollAsCorpse(this.GetPtr());

//    public void EndRagdollAsCorpse() => MBAPI.IMBAgent.EndRagdollAsCorpse(this.GetPtr());

//    public bool IsAddedAsCorpse() => MBAPI.IMBAgent.IsAddedAsCorpse(this.GetPtr());

//    public void AddAsCorpse() => MBAPI.IMBAgent.AddAsCorpse(this.GetPtr());

//    public void SetOverridenStrikeAndDeathAction(
//      in ActionIndexCache strikeAction,
//      in ActionIndexCache deathAction)
//    {
//      MBAPI.IMBAgent.SetOverridenStrikeAndDeathAction(this.GetPtr(), strikeAction.Index, deathAction.Index);
//    }

//    public void ApplyForceOnRagdoll(sbyte boneIndex, in Vec3 force)
//    {
//      MBAPI.IMBAgent.ApplyForceOnRagdoll(this.GetPtr(), boneIndex, in force);
//    }

//    public void SetVelocityLimitsOnRagdoll(float linearVelocityLimit, float angularVelocityLimit)
//    {
//      MBAPI.IMBAgent.SetVelocityLimitsOnRagdoll(this.GetPtr(), linearVelocityLimit, angularVelocityLimit);
//    }

//    public WorldPosition GetAILastSuspiciousPosition()
//    {
//      return MBAPI.IMBAgent.GetAILastSuspiciousPosition(this.GetPtr());
//    }

//    public void SetAILastSuspiciousPosition(
//      WorldPosition lastSuspiciousPosition,
//      bool checkNavMeshForCorrection)
//    {
//      Vec3 vec3WithoutValidity = lastSuspiciousPosition.GetVec3WithoutValidity();
//      if (checkNavMeshForCorrection)
//      {
//        int num = 0;
//        Vec2 vec2_1 = this.Position.AsVec2 - lastSuspiciousPosition.AsVec2;
//        Vec2 vec2_2 = vec2_1.Normalized();
//        for (; lastSuspiciousPosition.GetNavMesh() == UIntPtr.Zero && num < 15; ++num)
//        {
//          Vec2 vec2_3 = vec2_2;
//          if (num > 0)
//            vec2_3.RotateCCW((float) ((double) MBRandom.RandomFloat * (double) num * 0.30000001192092896 - (double) num * 0.15000000596046448));
//          lastSuspiciousPosition.SetVec3(UIntPtr.Zero, vec3WithoutValidity, false);
//          lastSuspiciousPosition.SetVec2(lastSuspiciousPosition.AsVec2 + vec2_3 * 0.4f * (float) (num + 2));
//        }
//        if (lastSuspiciousPosition.GetNavMesh() != UIntPtr.Zero)
//        {
//          MBAPI.IMBAgent.SetAILastSuspiciousPosition(this.GetPtr(), in lastSuspiciousPosition);
//        }
//        else
//        {
//          WorldPosition lastSuspiciousPosition1 = this.GetWorldPosition();
//          ref WorldPosition local = ref lastSuspiciousPosition1;
//          Vec2 asVec2 = lastSuspiciousPosition1.AsVec2;
//          vec2_1 = lastSuspiciousPosition.AsVec2 - this.Position.AsVec2;
//          Vec2 vec2_4 = vec2_1.Normalized() * 0.1f;
//          Vec2 vec2_5 = asVec2 + vec2_4;
//          local.SetVec2(vec2_5);
//          MBAPI.IMBAgent.SetAILastSuspiciousPosition(this.GetPtr(), in lastSuspiciousPosition1);
//        }
//      }
//      else
//        MBAPI.IMBAgent.SetAILastSuspiciousPosition(this.GetPtr(), in lastSuspiciousPosition);
//    }

//    public WorldPosition GetAIMoveDestination()
//    {
//      return MBAPI.IMBAgent.GetAIMoveDestination(this.GetPtr());
//    }

//    public Vec2 FindLongestDirectMoveToPosition(
//      Vec2 targetPosition,
//      bool checkBoundaries,
//      bool checkFriendlyAgents,
//      out bool isCollidedWithAgent)
//    {
//      return MBAPI.IMBAgent.FindLongestDirectMoveToPosition(this.GetPtr(), targetPosition, checkBoundaries, checkFriendlyAgents, out isCollidedWithAgent);
//    }

//    public float GetAIMoveStartTolerance()
//    {
//      return MBAPI.IMBAgent.GetAIMoveStopTolerance(this.GetPtr()) * 1.2f;
//    }

//    public float GetAIMoveStopTolerance() => MBAPI.IMBAgent.GetAIMoveStopTolerance(this.GetPtr());

//    public bool IsAIAtMoveDestination()
//    {
//      float moveStartTolerance = this.GetAIMoveStartTolerance();
//      return (double) this.GetAIMoveDestination().AsVec2.DistanceSquared(this.Position.AsVec2) <= (double) moveStartTolerance * (double) moveStartTolerance;
//    }

//    public void SetFormationBanner(ItemObject banner) => this._formationBanner = banner;

//    public void SetIsAIPaused(bool isPaused) => this.IsPaused = isPaused;

//    public void ResetEnemyCaches() => MBAPI.IMBAgent.ResetEnemyCaches(this.GetPtr());

//    public void SetTargetPositionSynched(ref Vec2 targetPosition)
//    {
//      if (this.MovementLockedState != AgentMovementLockedState.None && !(this.GetTargetPosition() != targetPosition))
//        return;
//      if (GameNetwork.IsClientOrReplay)
//      {
//        this._lastSynchedTargetPosition = targetPosition;
//        this._checkIfTargetFrameIsChanged = true;
//      }
//      else
//      {
//        this.SetTargetPosition(targetPosition);
//        if (!GameNetwork.IsServerOrRecorder)
//          return;
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentTargetPosition(this.Index, ref targetPosition));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//    }

//    public void SetTargetPositionAndDirectionSynched(
//      ref Vec2 targetPosition,
//      ref Vec3 targetDirection)
//    {
//      if (this.MovementLockedState != AgentMovementLockedState.None && !(this.GetTargetDirection() != targetDirection))
//        return;
//      if (GameNetwork.IsClientOrReplay)
//      {
//        this._lastSynchedTargetDirection = targetDirection;
//        this._checkIfTargetFrameIsChanged = true;
//      }
//      else
//      {
//        this.SetTargetPositionAndDirection(in targetPosition, in targetDirection);
//        if (!GameNetwork.IsServerOrRecorder)
//          return;
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentTargetPositionAndDirection(this.Index, ref targetPosition, ref targetDirection));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//    }

//    public void SetBodyArmorMaterialType(
//      ArmorComponent.ArmorMaterialTypes bodyArmorMaterialType)
//    {
//      MBAPI.IMBAgent.SetBodyArmorMaterialType(this.GetPtr(), bodyArmorMaterialType);
//    }

//    public void SetUsedGameObjectForClient(UsableMissionObject usedObject)
//    {
//      this.CurrentlyUsedGameObject = usedObject;
//      usedObject.OnUse(this, (sbyte) -1);
//      this.Mission.OnObjectUsed(this, usedObject);
//    }

//    public void SetTeam(Team team, bool sync)
//    {
//      if (this.Team == team)
//        return;
//      Team team1 = this.Team;
//      this.Team?.RemoveAgentFromTeam(this);
//      this.Team = team;
//      this.Team?.AddAgentToTeam(this);
//      this.SetTeamInternal(team != null ? team.MBTeam : MBTeam.InvalidTeam);
//      if (sync && GameNetwork.IsServer && this.Mission.HasMissionBehavior<MissionNetworkComponent>())
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new AgentSetTeam(this.Index, team != null ? team.TeamIndex : -1));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
//        missionBehavior.OnAgentTeamChanged(team1, team, this);
//    }

//    public void SetClothingColor1(uint color) => this._clothingColor1 = new uint?(color);

//    public void SetClothingColor2(uint color) => this._clothingColor2 = new uint?(color);

//    public void SetWieldedItemIndexAsClient(
//      Agent.HandIndex handIndex,
//      EquipmentIndex equipmentIndex,
//      bool isWieldedInstantly,
//      bool isWieldedOnSpawn,
//      int mainHandCurrentUsageIndex)
//    {
//      MBAPI.IMBAgent.SetWieldedItemIndexAsClient(this.GetPtr(), (int) handIndex, (int) equipmentIndex, isWieldedInstantly, isWieldedOnSpawn, mainHandCurrentUsageIndex);
//    }

//    public void SetPreciseRangedAimingEnabled(bool set)
//    {
//      if (set)
//        this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.RangerCanMoveForClearTarget);
//      else
//        this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.RangerCanMoveForClearTarget);
//    }

//    public void SetAsConversationAgent(bool set)
//    {
//      if (set)
//      {
//        this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.InConversation);
//        this.DisableLookToPointOfInterest();
//      }
//      else
//        this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.InConversation);
//    }

//    public void SetCrouchMode(bool set)
//    {
//      if (set)
//        this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.Crouch);
//      else
//        this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.Crouch);
//    }

//    public void SetWeaponAmountInSlot(
//      EquipmentIndex equipmentSlot,
//      short amount,
//      bool enforcePrimaryItem)
//    {
//      MBAPI.IMBAgent.SetWeaponAmountInSlot(this.GetPtr(), (int) equipmentSlot, amount, enforcePrimaryItem);
//    }

//    public void SetDraggingMode(bool set)
//    {
//      if (set)
//        this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.Drag);
//      else
//        this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.Drag);
//    }

//    public void SetWeaponAmmoAsClient(
//      EquipmentIndex equipmentIndex,
//      EquipmentIndex ammoEquipmentIndex,
//      short ammo)
//    {
//      MBAPI.IMBAgent.SetWeaponAmmoAsClient(this.GetPtr(), (int) equipmentIndex, (int) ammoEquipmentIndex, ammo);
//    }

//    public void SetWeaponReloadPhaseAsClient(EquipmentIndex equipmentIndex, short reloadState)
//    {
//      MBAPI.IMBAgent.SetWeaponReloadPhaseAsClient(this.GetPtr(), (int) equipmentIndex, reloadState);
//    }

//    public void SetReloadAmmoInSlot(
//      EquipmentIndex equipmentIndex,
//      EquipmentIndex ammoSlotIndex,
//      short reloadedAmmo)
//    {
//      MBAPI.IMBAgent.SetReloadAmmoInSlot(this.GetPtr(), (int) equipmentIndex, (int) ammoSlotIndex, reloadedAmmo);
//    }

//    public void SetUsageIndexOfWeaponInSlotAsClient(EquipmentIndex slotIndex, int usageIndex)
//    {
//      MBAPI.IMBAgent.SetUsageIndexOfWeaponInSlotAsClient(this.GetPtr(), (int) slotIndex, usageIndex);
//    }

//    public void SetRandomizeColors(bool shouldRandomize) => this.RandomizeColors = shouldRandomize;

//    [MBCallback(null, false)]
//    internal void OnRemoveWeapon(EquipmentIndex slotIndex) => this.RemoveEquippedWeapon(slotIndex);

//    public void SetFormationFrameDisabled()
//    {
//      MBAPI.IMBAgent.SetFormationFrameDisabled(this.GetPtr());
//      if (!this.IsFormationFrameEnabled)
//        return;
//      this.IsFormationFrameEnabled = false;
//      this._changedFormationPosition = new WorldPosition(this.Mission.Scene, UIntPtr.Zero, Vec3.Zero, false);
//    }

//    public void SetFormationFrameEnabled(
//      WorldPosition position,
//      Vec2 direction,
//      Vec2 positionVelocity,
//      float formationDirectionEnforcingFactor)
//    {
//      bool flag = MBAPI.IMBAgent.SetFormationFrameEnabled(this.GetPtr(), position, direction, positionVelocity, formationDirectionEnforcingFactor, this.Mission.IsTeleportingAgents);
//      if (!this.IsFormationFrameEnabled)
//      {
//        flag = true;
//        this.IsFormationFrameEnabled = true;
//      }
//      if (!flag)
//        return;
//      this._changedFormationPosition = position;
//    }

//    public void SetShouldCatchUpWithFormation(bool value)
//    {
//      MBAPI.IMBAgent.SetShouldCatchUpWithFormation(this.GetPtr(), value);
//    }

//    public void SetFormationIntegrityData(
//      Vec2 position,
//      Vec2 currentFormationDirection,
//      Vec2 averageVelocityOfCloseAgents,
//      float averageMaxUnlimitedSpeedOfCloseAgents,
//      float deviationOfPositions,
//      bool shouldKeepWithFormationInsteadOfMovingToAgent)
//    {
//      MBAPI.IMBAgent.SetFormationIntegrityData(this.GetPtr(), in position, in currentFormationDirection, in averageVelocityOfCloseAgents, averageMaxUnlimitedSpeedOfCloseAgents, deviationOfPositions, shouldKeepWithFormationInsteadOfMovingToAgent);
//    }

//    public bool IsCrouchingAllowed() => MBAPI.IMBAgent.IsCrouchingAllowed(this.GetPtr());

//    [MBCallback(null, false)]
//    internal void OnWeaponUsageIndexChange(EquipmentIndex slotIndex, int usageIndex)
//    {
//      this.Equipment.SetUsageIndexOfSlot(slotIndex, usageIndex);
//      this.UpdateAgentProperties();
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new WeaponUsageIndexChangeMessage(this.Index, slotIndex, usageIndex));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//    }

//    public void SetCurrentActionProgress(int channelNo, float progress)
//    {
//      MBAPI.IMBAgent.SetCurrentActionProgress(this.GetPtr(), channelNo, progress);
//    }

//    public void SetCurrentActionSpeed(int channelNo, float speed)
//    {
//      MBAPI.IMBAgent.SetCurrentActionSpeed(this.GetPtr(), channelNo, speed);
//    }

//    public bool SetActionChannel(
//      int channelNo,
//      in ActionIndexCache actionIndexCache,
//      bool ignorePriority = false,
//      AnimFlags additionalFlags = (AnimFlags) 0,
//      float blendWithNextActionFactor = 0.0f,
//      float actionSpeed = 1f,
//      float blendInPeriod = -0.2f,
//      float blendOutPeriodToNoAnim = 0.4f,
//      float startProgress = 0.0f,
//      bool useLinearSmoothing = false,
//      float blendOutPeriod = -0.2f,
//      int actionShift = 0,
//      bool forceFaceMorphRestart = true)
//    {
//      int index = actionIndexCache.Index;
//      return MBAPI.IMBAgent.SetActionChannel(this.GetPtr(), channelNo, index + actionShift, (ulong) additionalFlags, ignorePriority, blendWithNextActionFactor, actionSpeed, blendInPeriod, blendOutPeriodToNoAnim, startProgress, useLinearSmoothing, blendOutPeriod, forceFaceMorphRestart);
//    }

//    [MBCallback(null, false)]
//    internal void OnWeaponAmountChange(EquipmentIndex slotIndex, short amount)
//    {
//      this.Equipment.SetAmountOfSlot(slotIndex, amount);
//      this.UpdateAgentProperties();
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new SetWeaponNetworkData(this.Index, slotIndex, amount));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//    }

//    public void SetAttackState(int attackState)
//    {
//      MBAPI.IMBAgent.SetAttackState(this.GetPtr(), attackState);
//    }

//    public void SetAIBehaviorParams(
//      HumanAIComponent.AISimpleBehaviorKind behavior,
//      float y1,
//      float x2,
//      float y2,
//      float x3,
//      float y3)
//    {
//      MBAPI.IMBAgent.SetAIBehaviorParams(this.GetPtr(), (int) behavior, y1, x2, y2, x3, y3);
//    }

//    public void SetAllBehaviorParams(HumanAIComponent.BehaviorValues[] behaviorParams)
//    {
//      MBAPI.IMBAgent.SetAllAIBehaviorParams(this.GetPtr(), behaviorParams);
//    }

//    public void SetMovementDirection(in Vec2 direction)
//    {
//      MBAPI.IMBAgent.SetMovementDirection(this.GetPtr(), in direction);
//    }

//    public void SetScriptedFlags(Agent.AIScriptedFrameFlags flags)
//    {
//      MBAPI.IMBAgent.SetScriptedFlags(this.GetPtr(), (int) flags);
//    }

//    public void SetScriptedCombatFlags(Agent.AISpecialCombatModeFlags flags)
//    {
//      MBAPI.IMBAgent.SetScriptedCombatFlags(this.GetPtr(), (int) flags);
//    }

//    public void SetScriptedPositionAndDirection(
//      ref WorldPosition scriptedPosition,
//      float scriptedDirection,
//      bool addHumanLikeDelay,
//      Agent.AIScriptedFrameFlags additionalFlags = Agent.AIScriptedFrameFlags.None)
//    {
//      MBAPI.IMBAgent.SetScriptedPositionAndDirection(this.GetPtr(), ref scriptedPosition, scriptedDirection, addHumanLikeDelay, (int) additionalFlags);
//      if (!this.Mission.IsTeleportingAgents || !(scriptedPosition.AsVec2 != this.Position.AsVec2))
//        return;
//      this.TeleportToPosition(scriptedPosition.GetGroundVec3());
//    }

//    public void SetScriptedPosition(
//      ref WorldPosition position,
//      bool addHumanLikeDelay,
//      Agent.AIScriptedFrameFlags additionalFlags = Agent.AIScriptedFrameFlags.None)
//    {
//      MBAPI.IMBAgent.SetScriptedPosition(this.GetPtr(), ref position, addHumanLikeDelay, (int) additionalFlags);
//      if (!this.Mission.IsTeleportingAgents || !(position.AsVec2 != this.Position.AsVec2))
//        return;
//      this.TeleportToPosition(position.GetGroundVec3());
//    }

//    public void SetScriptedTargetEntityAndPosition(
//      WeakGameEntity target,
//      WorldPosition position,
//      Agent.AISpecialCombatModeFlags additionalFlags = Agent.AISpecialCombatModeFlags.None,
//      bool ignoreIfAlreadyAttacking = false)
//    {
//      MBAPI.IMBAgent.SetScriptedTargetEntity(this.GetPtr(), target.Pointer, ref position, (int) additionalFlags, ignoreIfAlreadyAttacking);
//    }

//    public void SetAgentExcludeStateForFaceGroupId(int faceGroupId, bool isExcluded)
//    {
//      MBAPI.IMBAgent.SetAgentExcludeStateForFaceGroupId(this.GetPtr(), faceGroupId, isExcluded);
//    }

//    public void SetLookAgent(Agent agent)
//    {
//      this._lookAgentCache = agent;
//      MBAPI.IMBAgent.SetLookAgent(this.GetPtr(), agent != null ? agent.GetPtr() : UIntPtr.Zero);
//    }

//    public void SetInteractionAgent(Agent agent)
//    {
//      MBAPI.IMBAgent.SetInteractionAgent(this.GetPtr(), agent != null ? agent.GetPtr() : UIntPtr.Zero);
//    }

//    public void SetLookToPointOfInterest(Vec3 point)
//    {
//      MBAPI.IMBAgent.SetLookToPointOfInterest(this.GetPtr(), point);
//    }

//    public void SetAgentFlags(AgentFlag agentFlags)
//    {
//      MBAPI.IMBAgent.SetAgentFlags(this.GetPtr(), (uint) agentFlags);
//    }

//    public void SetSelectedMountIndex(int mountIndex)
//    {
//      MBAPI.IMBAgent.SetSelectedMountIndex(this.GetPtr(), mountIndex);
//    }

//    public int GetFiringOrder() => MBAPI.IMBAgent.GetFiringOrder(this.GetPtr());

//    public int GetRidingOrder() => MBAPI.IMBAgent.GetRidingOrder(this.GetPtr());

//    public int GetSelectedMountIndex() => MBAPI.IMBAgent.GetSelectedMountIndex(this.GetPtr());

//    public int GetTargetFormationIndex() => MBAPI.IMBAgent.GetTargetFormationIndex(this.GetPtr());

//    public void SetFiringOrder(FiringOrder.RangedWeaponUsageOrderEnum order)
//    {
//      MBAPI.IMBAgent.SetFiringOrder(this.GetPtr(), (int) order);
//    }

//    public void SetRidingOrder(RidingOrder.RidingOrderEnum order)
//    {
//      MBAPI.IMBAgent.SetRidingOrder(this.GetPtr(), (int) order);
//    }

//    public void SetAgentFacialAnimation(
//      Agent.FacialAnimChannel channel,
//      string animationName,
//      bool loop)
//    {
//      MBAPI.IMBAgent.SetAgentFacialAnimation(this.GetPtr(), (int) channel, animationName, loop);
//    }

//    public bool SetHandInverseKinematicsFrame(
//      in MatrixFrame leftGlobalFrame,
//      in MatrixFrame rightGlobalFrame)
//    {
//      return MBAPI.IMBAgent.SetHandInverseKinematicsFrame(this.GetPtr(), in leftGlobalFrame, in rightGlobalFrame);
//    }

//    public void SetNativeFormationNo(int formationNo)
//    {
//      MBAPI.IMBAgent.SetFormationNo(this.GetPtr(), formationNo);
//    }

//    public void SetDirectionChangeTendency(float tendency)
//    {
//      MBAPI.IMBAgent.SetDirectionChangeTendency(this.GetPtr(), tendency);
//    }

//    public float GetBattleImportance()
//    {
//      BasicCharacterObject character = this.Character;
//      float battleImportance = character != null ? character.GetBattlePower() : 1f;
//      if (this.Team != null && this == this.Team.GeneralAgent)
//        battleImportance *= 2f;
//      else if (this.Formation != null && this == this.Formation.Captain)
//        battleImportance *= 1.2f;
//      return battleImportance;
//    }

//    public TroopTraitsMask GetTraitsMask()
//    {
//      TroopTraitsMask troopTraitsMask = TroopTraitsMask.None;
//      if (this.HasMount)
//        troopTraitsMask |= TroopTraitsMask.Mount;
//      TroopTraitsMask traitsMask = !this.IsRangedCached ? troopTraitsMask | TroopTraitsMask.Melee : troopTraitsMask | TroopTraitsMask.Ranged;
//      if (this.HasShieldCached)
//        traitsMask |= TroopTraitsMask.Shield;
//      if (this.HasSpearCached)
//        traitsMask |= TroopTraitsMask.Spear;
//      if (this.HasThrownCached)
//        traitsMask |= TroopTraitsMask.Thrown;
//      if (MissionGameModels.Current.AgentStatCalculateModel.HasHeavyArmor(this))
//        traitsMask |= TroopTraitsMask.Armor;
//      return traitsMask;
//    }

//    public void SetSynchedPrefabComponentVisibility(int componentIndex, bool visibility)
//    {
//      this._synchedBodyComponents[componentIndex].SetVisible(visibility);
//      this.AgentVisuals.LazyUpdateAgentRendererData();
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentPrefabComponentVisibility(this.Index, componentIndex, visibility));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//    }

//    public void SetActionSet(ref AnimationSystemData animationSystemData)
//    {
//      MBAPI.IMBAgent.SetActionSet(this.GetPtr(), ref animationSystemData);
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentActionSet(this.Index, animationSystemData));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//    }

//    public void SetColumnwiseFollowAgent(Agent followAgent, ref Vec2 followPosition)
//    {
//      if (!this.IsAIControlled)
//        return;
//      int followAgentIndex = followAgent != null ? followAgent.Index : -1;
//      MBAPI.IMBAgent.SetColumnwiseFollowAgent(this.GetPtr(), followAgentIndex, ref followPosition);
//      this.SetFollowedUnit(followAgent);
//    }

//    public void SetHandInverseKinematicsFrameForMissionObjectUsage(
//      in MatrixFrame localIKFrame,
//      in MatrixFrame boundEntityGlobalFrame,
//      float animationHeightDifference = 0.0f)
//    {
//      if (this.GetCurrentAction(1) != ActionIndexCache.act_none && (double) this.GetActionChannelWeight(1) > 0.0)
//        MBAPI.IMBAgent.SetHandInverseKinematicsFrameForMissionObjectUsage(this.GetPtr(), in localIKFrame, in boundEntityGlobalFrame, animationHeightDifference);
//      else
//        this.ClearHandInverseKinematics();
//    }

//    public void SetWantsToYell()
//    {
//      this._wantsToYell = true;
//      this._yellTimer = (float) ((double) MBRandom.RandomFloat * 0.30000001192092896 + 0.10000000149011612);
//    }

//    public void SetCapeClothSimulator(GameEntityComponent clothSimulatorComponent)
//    {
//      this._capeClothSimulator = clothSimulatorComponent as ClothSimulatorComponent;
//    }

//    public Vec2 GetTargetPosition() => MBAPI.IMBAgent.GetTargetPosition(this.GetPtr());

//    public Vec3 GetTargetDirection() => MBAPI.IMBAgent.GetTargetDirection(this.GetPtr());

//    public float GetAimingTimer() => MBAPI.IMBAgent.GetAimingTimer(this.GetPtr());

//    public float GetInteractionDistanceToUsable(IUsable usable)
//    {
//      switch (usable)
//      {
//        case Agent agent when agent.IsActive():
//          return !agent.IsMount ? 3f : 1.75f;
//        case SpawnedItemEntity spawnedItemEntity when spawnedItemEntity.IsBanner():
//          return 3f;
//        case StandingPoint standingPoint:
//          return !this.IsAIControlled ? ((double) standingPoint.CustomPlayerInteractionDistance <= 0.0 ? 2f : standingPoint.CustomPlayerInteractionDistance) : (!this.WalkMode ? 1f : 0.5f);
//        default:
//          return MissionGameModels.Current.AgentStatCalculateModel.GetInteractionDistance(this);
//      }
//    }

//    public TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
//    {
//      return this.IsMount && !userAgent.CheckSkillForMounting(this) ? GameTexts.FindText("str_ui_riding_skill_not_adequate_to_mount") : TextObject.GetEmpty();
//    }

//    public T GetController<T>() where T : AgentController
//    {
//      for (int index = 0; index < this._agentControllers.Count; ++index)
//      {
//        if (this._agentControllers[index] is T)
//          return (T) this._agentControllers[index];
//      }
//      return default (T);
//    }

//    public EquipmentIndex GetPrimaryWieldedItemIndex()
//    {
//      return AgentHelper.GetPrimaryWieldedItemIndex(this._primaryWieldedItemIndexPointer);
//    }

//    public EquipmentIndex GetOffhandWieldedItemIndex()
//    {
//      return AgentHelper.GetOffhandWieldedItemIndex(this._offHandWieldedItemIndexPointer);
//    }

//    public float GetMaximumForwardUnlimitedSpeed()
//    {
//      return AgentHelper.GetMaximumForwardUnlimitedSpeed(this._maximumForwardUnlimitedSpeed);
//    }

//    public TextObject GetDescriptionText(WeakGameEntity gameEntity) => this.NameTextObject;

//    public WeakGameEntity GetWeaponEntityFromEquipmentSlot(EquipmentIndex slotIndex)
//    {
//      return new WeakGameEntity(MBAPI.IMBAgent.GetWeaponEntityFromEquipmentSlot(this.GetPtr(), (int) slotIndex));
//    }

//    public WorldPosition GetRetreatPos() => MBAPI.IMBAgent.GetRetreatPos(this.GetPtr());

//    public Agent.AIScriptedFrameFlags GetScriptedFlags()
//    {
//      return (Agent.AIScriptedFrameFlags) MBAPI.IMBAgent.GetScriptedFlags(this.GetPtr());
//    }

//    public Agent.AISpecialCombatModeFlags GetScriptedCombatFlags()
//    {
//      return (Agent.AISpecialCombatModeFlags) MBAPI.IMBAgent.GetScriptedCombatFlags(this.GetPtr());
//    }

//    public WeakGameEntity GetSteppedEntity()
//    {
//      return new WeakGameEntity(MBAPI.IMBAgent.GetSteppedEntityId(this.GetPtr()));
//    }

//    public WeakGameEntity GetSteppedRootEntity()
//    {
//      return new WeakGameEntity(MBAPI.IMBAgent.GetSteppedRootEntity(this.GetPtr()));
//    }

//    public BodyFlags GetSteppedBodyFlags() => MBAPI.IMBAgent.GetSteppedBodyFlags(this.GetPtr());

//    public AnimFlags GetCurrentAnimationFlag(int channelNo)
//    {
//      return (AnimFlags) MBAPI.IMBAgent.GetCurrentAnimationFlags(this.GetPtr(), channelNo);
//    }

//    public ActionIndexCache GetCurrentAction(int channelNo)
//    {
//      return new ActionIndexCache(channelNo == 0 ? AgentHelper.GetChannel0CurrentActionIndex(this._channel0CurrentActionPointer) : AgentHelper.GetChannel1CurrentActionIndex(this._channel1CurrentActionPointer));
//    }

//    public Agent.ActionCodeType GetCurrentActionType(int channelNo)
//    {
//      return (Agent.ActionCodeType) MBAPI.IMBAgent.GetCurrentActionType(this.GetPtr(), channelNo);
//    }

//    public Agent.ActionStage GetCurrentActionStage(int channelNo)
//    {
//      return (Agent.ActionStage) MBAPI.IMBAgent.GetCurrentActionStage(this.GetPtr(), channelNo);
//    }

//    public Agent.UsageDirection GetCurrentActionDirection(int channelNo)
//    {
//      return (Agent.UsageDirection) MBAPI.IMBAgent.GetCurrentActionDirection(this.GetPtr(), channelNo);
//    }

//    public int GetCurrentActionPriority(int channelNo)
//    {
//      return MBAPI.IMBAgent.GetCurrentActionPriority(this.GetPtr(), channelNo);
//    }

//    public float GetCurrentActionProgress(int channelNo)
//    {
//      return MBAPI.IMBAgent.GetCurrentActionProgress(this.GetPtr(), channelNo);
//    }

//    public float GetActionChannelWeight(int channelNo)
//    {
//      return MBAPI.IMBAgent.GetActionChannelWeight(this.GetPtr(), channelNo);
//    }

//    public float GetActionChannelCurrentActionWeight(int channelNo)
//    {
//      return MBAPI.IMBAgent.GetActionChannelCurrentActionWeight(this.GetPtr(), channelNo);
//    }

//    public WorldFrame GetWorldFrame() => new WorldFrame(this.LookRotation, this.GetWorldPosition());

//    public float GetLookDownLimit() => MBAPI.IMBAgent.GetLookDownLimit(this.GetPtr());

//    public float GetEyeGlobalHeight() => MBAPI.IMBAgent.GetEyeGlobalHeight(this.GetPtr());

//    public float GetMaximumSpeedLimit() => MBAPI.IMBAgent.GetMaximumSpeedLimit(this.GetPtr());

//    public Vec2 GetCurrentVelocity() => MBAPI.IMBAgent.GetCurrentVelocity(this.GetPtr());

//    public float GetTurnSpeed() => MBAPI.IMBAgent.GetTurnSpeed(this.GetPtr());

//    public float GetCurrentSpeedLimit() => MBAPI.IMBAgent.GetCurrentSpeedLimit(this.GetPtr());

//    public Vec3 GetRealGlobalVelocity() => MBAPI.IMBAgent.GetRealGlobalVelocity(this.GetPtr());

//    public Vec3 GetAverageRealGlobalVelocity()
//    {
//      return MBAPI.IMBAgent.GetAverageRealGlobalVelocity(this.GetPtr());
//    }

//    public Vec2 GetMovementDirection() => Vec2.FromRotation(this.MovementDirectionAsAngle);

//    public Vec3 GetCurWeaponOffset() => MBAPI.IMBAgent.GetCurWeaponOffset(this.GetPtr());

//    public bool GetIsLeftStance() => MBAPI.IMBAgent.GetIsLeftStance(this.GetPtr());

//    public float GetPathDistanceToPoint(ref Vec3 point)
//    {
//      return MBAPI.IMBAgent.GetPathDistanceToPoint(this.GetPtr(), ref point);
//    }

//    public int GetCurrentNavigationFaceId()
//    {
//      return MBAPI.IMBAgent.GetCurrentNavigationFaceId(this.GetPtr());
//    }

//    public WorldPosition GetWorldPosition() => MBAPI.IMBAgent.GetWorldPosition(this.GetPtr());

//    public int GetGroundMaterialForCollisionEffect()
//    {
//      return MBAPI.IMBAgent.GetGroundMaterialForCollisionEffect(this.GetPtr());
//    }

//    public Agent GetLookAgent() => this._lookAgentCache;

//    public Agent GetTargetAgent() => MBAPI.IMBAgent.GetTargetAgent(this.GetPtr());

//    public void SetTargetAgent(Agent agent)
//    {
//      MBAPI.IMBAgent.SetTargetAgent(this.GetPtr(), agent != null ? agent.Index : -1);
//    }

//    public void SetAutomaticTargetSelection(bool enable)
//    {
//      MBAPI.IMBAgent.SetAutomaticTargetSelection(this.GetPtr(), enable);
//    }

//    public AgentFlag GetAgentFlags() => AgentHelper.GetAgentFlags(this.FlagsPointer);

//    public string GetAgentFacialAnimation()
//    {
//      return MBAPI.IMBAgent.GetAgentFacialAnimation(this.GetPtr());
//    }

//    public string GetAgentVoiceDefinition()
//    {
//      return MBAPI.IMBAgent.GetAgentVoiceDefinition(this.GetPtr());
//    }

//    public Vec3 GetEyeGlobalPosition() => MBAPI.IMBAgent.GetEyeGlobalPosition(this.GetPtr());

//    public Vec3 GetChestGlobalPosition() => MBAPI.IMBAgent.GetChestGlobalPosition(this.GetPtr());

//    public Agent.MovementControlFlag GetDefendMovementFlag()
//    {
//      return MBAPI.IMBAgent.GetDefendMovementFlag(this.GetPtr());
//    }

//    public Agent.UsageDirection GetAttackDirection()
//    {
//      return MBAPI.IMBAgent.GetAttackDirection(this.GetPtr());
//    }

//    public WeaponInfo GetWieldedWeaponInfo(Agent.HandIndex handIndex)
//    {
//      bool isMeleeWeapon = false;
//      bool isRangedWeapon = false;
//      return MBAPI.IMBAgent.GetWieldedWeaponInfo(this.GetPtr(), (int) handIndex, ref isMeleeWeapon, ref isRangedWeapon) ? new WeaponInfo(true, isMeleeWeapon, isRangedWeapon) : new WeaponInfo(false, false, false);
//    }

//    public Vec2 GetBodyRotationConstraint(int channelIndex = 1)
//    {
//      return MBAPI.IMBAgent.GetBodyRotationConstraint(this.GetPtr(), channelIndex).AsVec2;
//    }

//    public float GetTotalEncumbrance()
//    {
//      return this.AgentDrivenProperties.ArmorEncumbrance + this.AgentDrivenProperties.WeaponsEncumbrance;
//    }

//    public float GetTotalMass() => MBAPI.IMBAgent.GetTotalMass(this.GetPtr());

//    public T GetComponent<T>() where T : AgentComponent
//    {
//      for (int index = 0; index < this._components.Count; ++index)
//      {
//        if (this._components[index] is T)
//          return (T) this._components[index];
//      }
//      return default (T);
//    }

//    public float GetAgentDrivenPropertyValue(DrivenProperty type)
//    {
//      return this.AgentDrivenProperties.GetStat(type);
//    }

//    public UsableMachine GetSteppedMachine()
//    {
//      WeakGameEntity weakGameEntity = this.GetSteppedEntity();
//      while (weakGameEntity.IsValid && !weakGameEntity.HasScriptOfType<UsableMachine>())
//        weakGameEntity = weakGameEntity.Parent;
//      return weakGameEntity.IsValid ? weakGameEntity.GetFirstScriptOfType<UsableMachine>() : (UsableMachine) null;
//    }

//    public int GetAttachedWeaponsCount()
//    {
//      List<(MissionWeapon, MatrixFrame, sbyte)> attachedWeapons = this._attachedWeapons;
//      // ISSUE: explicit non-virtual call
//      return attachedWeapons == null ? 0 : __nonvirtual (attachedWeapons.Count);
//    }

//    public MissionWeapon GetAttachedWeapon(int index) => this._attachedWeapons[index].Item1;

//    public MatrixFrame GetAttachedWeaponFrame(int index) => this._attachedWeapons[index].Item2;

//    public sbyte GetAttachedWeaponBoneIndex(int index) => this._attachedWeapons[index].Item3;

//    public void DeleteAttachedWeapon(int index)
//    {
//      this._attachedWeapons.RemoveAt(index);
//      MBAPI.IMBAgent.DeleteAttachedWeaponFromBone(this.GetPtr(), index);
//    }

//    public bool HasRangedWeapon(bool checkHasAmmo = false)
//    {
//      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
//      {
//        if (!this.Equipment[equipmentIndex].IsEmpty && this.Equipment[equipmentIndex].GetRangedUsageIndex() >= 0 && (!checkHasAmmo || this.Equipment.HasAmmo(equipmentIndex, out int _, out bool _, out bool _)))
//          return true;
//      }
//      return false;
//    }

//    public MatrixFrame GetBoneEntitialFrameAtAnimationProgress(
//      sbyte boneIndex,
//      int animationIndex,
//      float progress)
//    {
//      return MBAPI.IMBAgent.GetBoneEntitialFrameAtAnimationProgress(this.GetPtr(), boneIndex, animationIndex, progress);
//    }

//    public void GetFormationFileAndRankInfo(out int fileIndex, out int rankIndex)
//    {
//      IFormationUnit formationUnit = (IFormationUnit) this;
//      fileIndex = formationUnit.FormationFileIndex;
//      rankIndex = formationUnit.FormationRankIndex;
//    }

//    public void GetFormationFileAndRankInfo(
//      out int fileIndex,
//      out int rankIndex,
//      out int fileCount,
//      out int rankCount)
//    {
//      IFormationUnit formationUnit = (IFormationUnit) this;
//      fileIndex = formationUnit.FormationFileIndex;
//      rankIndex = formationUnit.FormationRankIndex;
//      if (formationUnit.Formation is LineFormation formation)
//      {
//        formation.GetFormationInfo(out fileCount, out rankCount);
//      }
//      else
//      {
//        fileCount = -1;
//        rankCount = -1;
//      }
//    }

//    internal Vec2 GetWallDirectionOfRelativeFormationLocation()
//    {
//      return this.Formation.GetWallDirectionOfRelativeFormationLocation(this);
//    }

//    public void SetMortalityState(Agent.MortalityState newState)
//    {
//      this.CurrentMortalityState = newState;
//    }

//    public void ToggleInvulnerable()
//    {
//      if (this.CurrentMortalityState == Agent.MortalityState.Invulnerable)
//        this.CurrentMortalityState = Agent.MortalityState.Mortal;
//      else
//        this.CurrentMortalityState = Agent.MortalityState.Invulnerable;
//    }

//    public float GetArmLength() => this.Monster.ArmLength * this.AgentScale;

//    public float GetArmWeight() => this.Monster.ArmWeight * this.AgentScale;

//    public void GetRunningSimulationDataUntilMaximumSpeedReached(
//      ref float combatAccelerationTime,
//      ref float maxSpeed,
//      float[] speedValues)
//    {
//      MBAPI.IMBAgent.GetRunningSimulationDataUntilMaximumSpeedReached(this.GetPtr(), ref combatAccelerationTime, ref maxSpeed, speedValues);
//    }

//    public void SetMaximumSpeedLimit(float maximumSpeedLimit, bool isMultiplier)
//    {
//      MBAPI.IMBAgent.SetMaximumSpeedLimit(this.GetPtr(), maximumSpeedLimit, isMultiplier);
//    }

//    public float GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType bodyPart)
//    {
//      if (!this.IsHuman)
//        return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
//      switch (bodyPart)
//      {
//        case BoneBodyPartType.None:
//          return 0.0f;
//        case BoneBodyPartType.Head:
//        case BoneBodyPartType.Neck:
//          return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorHead);
//        case BoneBodyPartType.Chest:
//        case BoneBodyPartType.Abdomen:
//        case BoneBodyPartType.ShoulderLeft:
//        case BoneBodyPartType.ShoulderRight:
//          return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
//        case BoneBodyPartType.ArmLeft:
//        case BoneBodyPartType.ArmRight:
//          return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorArms);
//        case BoneBodyPartType.Legs:
//          return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorLegs);
//        default:
//          TaleWorlds.Library.Debug.FailedAssert("false", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Agent.cs", nameof (GetBaseArmorEffectivenessForBodyPart), 3160);
//          return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
//      }
//    }

//    public AITargetVisibilityState GetLastTargetVisibilityState()
//    {
//      return (AITargetVisibilityState) MBAPI.IMBAgent.GetLastTargetVisibilityState(this.GetPtr());
//    }

//    public float GetMissileRange() => MBAPI.IMBAgent.GetMissileRange(this.GetPtr());

//    public void SetAgentIdleAnimationStatus(bool idleEnabled)
//    {
//      MBAPI.IMBAgent.SetAgentIdleAnimationStatus(this.GetPtr(), idleEnabled);
//    }

//    public ItemObject GetWeaponToReplaceOnQuickAction(
//      SpawnedItemEntity spawnedItem,
//      out EquipmentIndex possibleSlotIndex)
//    {
//      EquipmentIndex index = MissionEquipment.SelectWeaponPickUpSlot(this, spawnedItem.WeaponCopy, spawnedItem.IsStuckMissile());
//      possibleSlotIndex = index;
//      if (index != EquipmentIndex.None && !this.Equipment[index].IsEmpty)
//      {
//        MissionWeapon weaponCopy;
//        if (spawnedItem.IsStuckMissile() || spawnedItem.WeaponCopy.IsAnyConsumable())
//        {
//          int weaponClass1 = (int) this.Equipment[index].Item.PrimaryWeapon.WeaponClass;
//          weaponCopy = spawnedItem.WeaponCopy;
//          int weaponClass2 = (int) weaponCopy.Item.PrimaryWeapon.WeaponClass;
//          if (weaponClass1 == weaponClass2)
//          {
//            weaponCopy = this.Equipment[index];
//            if (weaponCopy.IsAnyConsumable())
//            {
//              weaponCopy = this.Equipment[index];
//              int amount = (int) weaponCopy.Amount;
//              weaponCopy = this.Equipment[index];
//              int modifiedMaxAmount = (int) weaponCopy.ModifiedMaxAmount;
//              if (amount != modifiedMaxAmount)
//                goto label_6;
//            }
//          }
//        }
//        weaponCopy = this.Equipment[index];
//        return weaponCopy.Item;
//      }
//label_6:
//      return (ItemObject) null;
//    }

//    public Agent.Hitter GetAssistingHitter(MissionPeer killerPeer)
//    {
//      Agent.Hitter hitter1 = (Agent.Hitter) null;
//      foreach (Agent.Hitter hitter2 in (List<Agent.Hitter>) this.HitterList)
//      {
//        if (hitter2.HitterPeer != killerPeer && (hitter1 == null || (double) hitter2.Damage > (double) hitter1.Damage))
//          hitter1 = hitter2;
//      }
//      return hitter1 != null && (double) hitter1.Damage >= 35.0 ? hitter1 : (Agent.Hitter) null;
//    }

//    public bool CanReachAgent(Agent otherAgent)
//    {
//      float distanceToUsable = this.GetInteractionDistanceToUsable((IUsable) otherAgent);
//      return (double) this.Position.DistanceSquared(otherAgent.Position) < (double) distanceToUsable * (double) distanceToUsable;
//    }

//    public bool CanInteractWithAgent(Agent otherAgent, float userAgentCameraElevation)
//    {
//      bool flag1 = false;
//      foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
//        flag1 = flag1 || missionBehavior.IsThereAgentAction(this, otherAgent);
//      if (!flag1)
//        return false;
//      bool flag2 = this.CanReachAgent(otherAgent);
//      if (!otherAgent.IsMount)
//        return this.IsAbleToUseMachine() & flag2;
//      if (this.MountAgent == null && this.GetCurrentAction(0) != ActionIndexCache.act_none || this.MountAgent != null && !this.IsAbleToUseMachine())
//        return false;
//      if (otherAgent.RiderAgent == null)
//        return this.MountAgent == null & flag2 && otherAgent.GetCurrentActionType(0) != Agent.ActionCodeType.Rear;
//      if (otherAgent != this.MountAgent)
//        return false;
//      float num = this.GetLookDownLimit() + 0.4f;
//      return flag2 && (double) userAgentCameraElevation < (double) num && (double) this.GetCurrentVelocity().LengthSquared < 0.25 && otherAgent.GetCurrentActionType(0) != Agent.ActionCodeType.Rear;
//    }

//    public bool CanBeAssignedForScriptedMovement()
//    {
//      return this.IsActive() && this.IsAIControlled && !this.IsDetachedFromFormation && !this.IsRunningAway && (this.GetScriptedFlags() & Agent.AIScriptedFrameFlags.GoToPosition) == Agent.AIScriptedFrameFlags.None && !this.InteractingWithAnyGameObject() && !this._isLadderQueueUsing;
//    }

//    public bool CanReachAndUseObject(UsableMissionObject gameObject, float distanceSq)
//    {
//      return this.CanReachObject(gameObject, distanceSq) && this.CanUseObject(gameObject);
//    }

//    public bool CanReachObject(UsableMissionObject gameObject, float distanceSq)
//    {
//      return this.CanReachObjectFromPosition(gameObject, distanceSq, this.Position);
//    }

//    public bool CanReachObjectFromPosition(
//      UsableMissionObject gameObject,
//      float distanceSq,
//      Vec3 position)
//    {
//      if (this.IsItemUseDisabled || this.IsUsingGameObject)
//        return false;
//      float distanceToUsable = this.GetInteractionDistanceToUsable((IUsable) gameObject);
//      return (double) distanceSq <= (double) distanceToUsable * (double) distanceToUsable && (double) MathF.Abs(gameObject.InteractionEntity.GlobalPosition.z - position.z) <= (double) distanceToUsable * 2.0;
//    }

//    public bool CanUseObject(UsableMissionObject gameObject)
//    {
//      return !gameObject.IsDisabledForAgent(this) && gameObject.IsUsableByAgent(this);
//    }

//    public bool CanMoveDirectlyToPosition(in Vec2 position)
//    {
//      return MBAPI.IMBAgent.CanMoveDirectlyToPosition(this.GetPtr(), in position);
//    }

//    public bool CanInteractableWeaponBePickedUp(SpawnedItemEntity spawnedItem)
//    {
//      if (spawnedItem.IsBanner() && !MissionGameModels.Current.BattleBannerBearersModel.IsInteractableFormationBanner(spawnedItem, this))
//        return false;
//      EquipmentIndex possibleSlotIndex;
//      return this.GetWeaponToReplaceOnQuickAction(spawnedItem, out possibleSlotIndex) != null || possibleSlotIndex == EquipmentIndex.None;
//    }

//    public bool CanQuickPickUp(SpawnedItemEntity spawnedItem)
//    {
//      return (!spawnedItem.IsBanner() || MissionGameModels.Current.BattleBannerBearersModel.IsInteractableFormationBanner(spawnedItem, this)) && MissionEquipment.SelectWeaponPickUpSlot(this, spawnedItem.WeaponCopy, spawnedItem.IsStuckMissile()) != EquipmentIndex.None;
//    }

//    public bool CanTeleport()
//    {
//      if (!this.Mission.IsTeleportingAgents)
//        return false;
//      return this.Formation == null || this.Mission.Mode != MissionMode.Deployment || this.Formation.GetReadonlyMovementOrderReference().OrderEnum == MovementOrder.MovementOrderEnum.Move;
//    }

//    public bool IsActive() => this.State == AgentState.Active;

//    public bool IsRetreating() => MBAPI.IMBAgent.IsRetreating(this.GetPtr());

//    public bool IsFadingOut() => MBAPI.IMBAgent.IsFadingOut(this.GetPtr());

//    public void SetAgentDrivenPropertyValueFromConsole(DrivenProperty type, float val)
//    {
//      this.AgentDrivenProperties.SetStat(type, val);
//    }

//    public bool IsOnLand()
//    {
//      return (this.MovementMode & AgentMovementMode.WaterDiving) == AgentMovementMode.Land;
//    }

//    public bool IsInWater()
//    {
//      AgentMovementMode agentMovementMode = this.MovementMode & AgentMovementMode.WaterDiving;
//      return agentMovementMode == AgentMovementMode.WaterSurface || agentMovementMode == AgentMovementMode.WaterDiving;
//    }

//    public bool IsAbleToUseMachine()
//    {
//      return (this.MovementMode & AgentMovementMode.WaterDiving) > AgentMovementMode.None;
//    }

//    public bool IsAgentParentEntitySameAs(GameEntity toBeChecked)
//    {
//      return toBeChecked == MBAPI.IMBAgent.GetAgentParentEntity(this.GetPtr());
//    }

//    public void SetExcludedFromGravity(bool exclude, bool applyAverageGlobalVelocity)
//    {
//      MBAPI.IMBAgent.SetExcludedFromGravity(this.GetPtr(), exclude, applyAverageGlobalVelocity);
//    }

//    public void SetForceAttachedEntity(WeakGameEntity willBeAttached)
//    {
//      MBAPI.IMBAgent.SetForceAttachedEntity(this.GetPtr(), willBeAttached.Pointer);
//    }

//    public bool IsSliding() => MBAPI.IMBAgent.IsSliding(this.GetPtr());

//    public bool IsSitting()
//    {
//      Agent.ActionCodeType currentActionType = this.GetCurrentActionType(0);
//      switch (currentActionType)
//      {
//        case Agent.ActionCodeType.Sit:
//        case Agent.ActionCodeType.SitOnTheFloor:
//          return true;
//        default:
//          return currentActionType == Agent.ActionCodeType.SitOnAThrone;
//      }
//    }

//    public bool IsReleasingChainAttackInMultiplayer()
//    {
//      bool flag = false;
//      if ((double) Mission.Current.CurrentTime - (double) this._lastMultiplayerQuickReadyDetectedTime < 0.75 && this.GetCurrentActionStage(1) == Agent.ActionStage.AttackRelease)
//        flag = true;
//      return flag;
//    }

//    public bool IsCameraAttachable()
//    {
//      if (this._isDeleted || this._isRemoved && (double) this._removalTime + 2.0999999046325684 <= (double) this.Mission.CurrentTime || !this.IsHuman || !((NativeObject) this.AgentVisuals != (NativeObject) null) || !this.AgentVisuals.IsValid())
//        return false;
//      return GameNetwork.IsSessionActive || this.Controller != 0;
//    }

//    public bool IsSynchedPrefabComponentVisible(int componentIndex)
//    {
//      return this._synchedBodyComponents[componentIndex].GetVisible();
//    }

//    public bool IsEnemyOf(Agent otherAgent)
//    {
//      return MBAPI.IMBAgent.IsEnemy(this.GetPtr(), otherAgent.GetPtr());
//    }

//    public bool IsFriendOf(Agent otherAgent)
//    {
//      return MBAPI.IMBAgent.IsFriend(this.GetPtr(), otherAgent.GetPtr());
//    }

//    public void OnFocusGain(Agent userAgent)
//    {
//    }

//    public void OnFocusLose(Agent userAgent)
//    {
//    }

//    public void OnItemRemovedFromScene() => this.StopUsingGameObjectMT(false);

//    public void OnUse(Agent userAgent, sbyte agentBoneIndex)
//    {
//      this.Mission.OnAgentInteraction(userAgent, this, agentBoneIndex);
//    }

//    public void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
//    {
//    }

//    public void OnWeaponDrop(EquipmentIndex equipmentSlot)
//    {
//      MissionWeapon droppedWeapon = this.Equipment[equipmentSlot];
//      this.Equipment[equipmentSlot] = MissionWeapon.Invalid;
//      this.WeaponEquipped(equipmentSlot, in WeaponData.InvalidWeaponData, (WeaponStatsData[]) null, in WeaponData.InvalidWeaponData, (WeaponStatsData[]) null, WeakGameEntity.Invalid, false, false);
//      foreach (AgentComponent component in (List<AgentComponent>) this._components)
//        component.OnWeaponDrop(droppedWeapon);
//    }

//    public void OnItemPickup(
//      SpawnedItemEntity spawnedItemEntity,
//      EquipmentIndex weaponPickUpSlotIndex,
//      out bool removeWeapon)
//    {
//      removeWeapon = true;
//      bool flag1 = true;
//      MissionWeapon weaponCopy = spawnedItemEntity.WeaponCopy;
//      if (weaponPickUpSlotIndex == EquipmentIndex.None)
//        weaponPickUpSlotIndex = MissionEquipment.SelectWeaponPickUpSlot(this, weaponCopy, spawnedItemEntity.IsStuckMissile());
//      bool flag2 = false;
//      MissionWeapon missionWeapon;
//      if (weaponPickUpSlotIndex == EquipmentIndex.ExtraWeaponSlot)
//      {
//        if (!GameNetwork.IsClientOrReplay)
//        {
//          flag2 = true;
//          missionWeapon = this.Equipment[weaponPickUpSlotIndex];
//          if (!missionWeapon.IsEmpty)
//          {
//            int itemIndex = (int) weaponPickUpSlotIndex;
//            missionWeapon = this.Equipment[weaponPickUpSlotIndex];
//            int weaponClass = (int) missionWeapon.Item.PrimaryWeapon.WeaponClass;
//            this.DropItem((EquipmentIndex) itemIndex, (WeaponClass) weaponClass);
//          }
//        }
//      }
//      else if (weaponPickUpSlotIndex != EquipmentIndex.None)
//      {
//        int a = 0;
//        if ((spawnedItemEntity.IsStuckMissile() || spawnedItemEntity.WeaponCopy.IsAnyConsumable()) && !this.Equipment[weaponPickUpSlotIndex].IsEmpty && this.Equipment[weaponPickUpSlotIndex].IsSameType(weaponCopy) && this.Equipment[weaponPickUpSlotIndex].IsAnyConsumable())
//          a = (int) this.Equipment[weaponPickUpSlotIndex].ModifiedMaxAmount - (int) this.Equipment[weaponPickUpSlotIndex].Amount;
//        if (a > 0)
//        {
//          short consumedAmount = (short) MathF.Min(a, (int) weaponCopy.Amount);
//          if ((int) consumedAmount != (int) weaponCopy.Amount)
//          {
//            removeWeapon = false;
//            if (!GameNetwork.IsClientOrReplay)
//            {
//              spawnedItemEntity.ConsumeWeaponAmount(consumedAmount);
//              if (GameNetwork.IsServer)
//              {
//                GameNetwork.BeginBroadcastModuleEvent();
//                GameNetwork.WriteMessage((GameNetworkMessage) new ConsumeWeaponAmount(spawnedItemEntity.Id, consumedAmount));
//                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
//              }
//            }
//          }
//          if (!GameNetwork.IsClientOrReplay)
//          {
//            int equipmentSlot = (int) weaponPickUpSlotIndex;
//            missionWeapon = this.Equipment[weaponPickUpSlotIndex];
//            int amount = (int) (short) ((int) missionWeapon.Amount + (int) consumedAmount);
//            this.SetWeaponAmountInSlot((EquipmentIndex) equipmentSlot, (short) amount, true);
//            if (this.GetPrimaryWieldedItemIndex() == EquipmentIndex.None && (weaponCopy.Item.PrimaryWeapon.IsRangedWeapon || weaponCopy.Item.PrimaryWeapon.IsMeleeWeapon))
//              flag2 = true;
//          }
//        }
//        else if (!GameNetwork.IsClientOrReplay)
//        {
//          flag2 = true;
//          missionWeapon = this.Equipment[weaponPickUpSlotIndex];
//          if (!missionWeapon.IsEmpty)
//            this.DropItem(weaponPickUpSlotIndex, weaponCopy.Item.PrimaryWeapon.WeaponClass);
//        }
//      }
//      if (!GameNetwork.IsClientOrReplay)
//      {
//        flag1 = MissionEquipment.DoesWeaponFitToSlot(weaponPickUpSlotIndex, weaponCopy);
//        if (flag1)
//        {
//          this.EquipWeaponFromSpawnedItemEntity(weaponPickUpSlotIndex, spawnedItemEntity, removeWeapon);
//          if (flag2)
//          {
//            EquipmentIndex slotIndex = weaponPickUpSlotIndex;
//            if (weaponCopy.Item.PrimaryWeapon.AmmoClass == weaponCopy.Item.PrimaryWeapon.WeaponClass)
//            {
//              for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < weaponPickUpSlotIndex; ++index)
//              {
//                missionWeapon = this.Equipment[index];
//                if (!missionWeapon.IsEmpty && weaponCopy.IsEqualTo(this.Equipment[index]))
//                {
//                  slotIndex = index;
//                  break;
//                }
//              }
//            }
//            this.TryToWieldWeaponInSlot(slotIndex, Agent.WeaponWieldActionType.InstantAfterPickUp, false);
//          }
//          for (int index = 0; index < this._components.Count; ++index)
//            this._components[index].OnItemPickup(spawnedItemEntity);
//          if (this.Controller == AgentControllerType.AI)
//            this.HumanAIComponent.ItemPickupDone(spawnedItemEntity);
//        }
//      }
//      if (!flag1)
//        return;
//      this.Mission.TriggerOnItemPickUpEvent(this, spawnedItemEntity);
//    }

//    public float GetDistanceTo(Agent other)
//    {
//      if (other != null)
//        return this.Position.Distance(other.Position);
//      TaleWorlds.Library.Debug.FailedAssert("Comparing distance with null agent", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Agent.cs", nameof (GetDistanceTo), 3671);
//      return 0.0f;
//    }

//    public bool CheckPathToAITargetAgentPassesThroughNavigationFaceIdFromDirection(
//      int navigationFaceId,
//      in Vec3 direction,
//      float overridenCostForFaceId)
//    {
//      return MBAPI.IMBAgent.CheckPathToAITargetAgentPassesThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, in direction, overridenCostForFaceId);
//    }

//    public bool IsTargetNavigationFaceIdBetween(int navigationFaceIdStart, int navigationFaceIdEnd)
//    {
//      return MBAPI.IMBAgent.IsTargetNavigationFaceIdBetween(this.GetPtr(), navigationFaceIdStart, navigationFaceIdEnd);
//    }

//    Vec3 ITrackableBase.GetPosition() => this.Position;

//    TextObject ITrackableBase.GetName()
//    {
//      return this.Character != null ? new TextObject(this.Character.Name.ToString()) : TextObject.GetEmpty();
//    }

//    public void CheckEquipmentForCapeClothSimulationStateChange()
//    {
//      if (!((NativeObject) this._capeClothSimulator != (NativeObject) null))
//        return;
//      bool flag = false;
//      EquipmentIndex wieldedItemIndex = this.GetOffhandWieldedItemIndex();
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; ++index)
//      {
//        MissionWeapon missionWeapon = this.Equipment[index];
//        if (!missionWeapon.IsEmpty && missionWeapon.IsShield() && index != wieldedItemIndex)
//        {
//          flag = true;
//          break;
//        }
//      }
//      this._capeClothSimulator.SetMaxDistanceMultiplier(flag ? 0.0f : 1f);
//    }

//    public void CheckToDropFlaggedItem()
//    {
//      if (!this.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.CanWieldWeapon))
//        return;
//      for (int index = 0; index < 2; ++index)
//      {
//        EquipmentIndex equipmentIndex = index == 0 ? this.GetPrimaryWieldedItemIndex() : this.GetOffhandWieldedItemIndex();
//        if (equipmentIndex != EquipmentIndex.None && this.Equipment[equipmentIndex].Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DropOnAnyAction))
//          this.DropItem(equipmentIndex);
//      }
//    }

//    public bool CheckSkillForMounting(Agent mountAgent)
//    {
//      int effectiveSkill = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(this, DefaultSkills.Riding);
//      return (this.GetAgentFlags() & AgentFlag.CanRide) > AgentFlag.None && (double) effectiveSkill >= (double) mountAgent.GetAgentDrivenPropertyValue(DrivenProperty.MountDifficulty);
//    }

//    public void InitializeSpawnEquipment(TaleWorlds.Core.Equipment spawnEquipment)
//    {
//      this.SpawnEquipment = spawnEquipment;
//    }

//    public void InitializeMissionEquipment(MissionEquipment missionEquipment, TaleWorlds.Core.Banner banner)
//    {
//      this.Equipment = missionEquipment ?? new MissionEquipment(this.SpawnEquipment, banner);
//    }

//    public void InitializeAgentProperties(TaleWorlds.Core.Equipment spawnEquipment, AgentBuildData agentBuildData)
//    {
//      this._propertyModifiers = new Agent.AgentPropertiesModifiers();
//      this.AgentDrivenProperties = new AgentDrivenProperties();
//      this.UpdateDrivenProperties(this.AgentDrivenProperties.InitializeDrivenProperties(this, spawnEquipment, agentBuildData));
//      if (!this.IsMount || this.RiderAgent != null)
//        return;
//      Mission.Current.AddMountWithoutRider(this);
//    }

//    public void UpdateFormationOrders()
//    {
//      if (this.Formation == null || this.IsRetreating())
//        return;
//      this.EnforceShieldUsage(ArrangementOrder.GetShieldDirectionOfUnit(this.Formation, this, this.Formation.ArrangementOrder.OrderEnum));
//    }

//    public void UpdateWeapons() => MBAPI.IMBAgent.UpdateWeapons(this.GetPtr());

//    public void UpdateAgentProperties()
//    {
//      if (this.AgentDrivenProperties == null)
//        return;
//      this.UpdateDrivenProperties(this.AgentDrivenProperties.UpdateDrivenProperties(this));
//    }

//    public void UpdateCustomDrivenProperties()
//    {
//      if (this.AgentDrivenProperties == null)
//        return;
//      this.UpdateDrivenProperties(this.AgentDrivenProperties.Values);
//    }

//    public void UpdateBodyProperties(BodyProperties bodyProperties)
//    {
//      this.BodyPropertiesValue = bodyProperties;
//    }

//    public void UpdateSyncHealthToAllClients(bool value) => this.SyncHealthToAllClients = value;

//    public void UpdateSpawnEquipmentAndRefreshVisuals(TaleWorlds.Core.Equipment newSpawnEquipment)
//    {
//      this.SpawnEquipment = newSpawnEquipment;
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new SynchronizeAgentSpawnEquipment(this.Index, this.SpawnEquipment));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      this.AgentVisuals.ClearVisualComponents(false);
//      this.Mission.OnEquipItemsFromSpawnEquipment(this, Agent.CreationType.FromCharacterObj);
//      this.AgentVisuals.ClearAllWeaponMeshes();
//      this.Equipment.FillFrom(this.SpawnEquipment, this.Origin?.Banner);
//      this.CheckEquipmentForCapeClothSimulationStateChange();
//      this.EquipItemsFromSpawnEquipment(true, true);
//      this.UpdateAgentProperties();
//      if (!Mission.Current.DoesMissionRequireCivilianEquipment && !GameNetwork.IsClientOrReplay)
//        this.WieldInitialWeapons();
//      this.PreloadForRendering();
//    }

//    public void ForceUpdateCachedAndFormationValues(
//      bool updateOnlyMovement,
//      bool arrangementChangeAllowed)
//    {
//      this.ParallelUpdateCachedAndFormationValues(updateOnlyMovement);
//      this.UpdateCachedAndFormationValues(updateOnlyMovement, arrangementChangeAllowed);
//    }

//    private void UpdateCachedAndFormationValues(
//      bool updateOnlyMovement,
//      bool arrangementChangeAllowed)
//    {
//      if (!this.IsActive() || GameNetwork.IsClientOrReplay)
//        return;
//      if (!updateOnlyMovement && !this.IsDetachedFromFormation)
//        this.Formation?.Arrangement.OnTickOccasionallyOfUnit((IFormationUnit) this, arrangementChangeAllowed);
//      if (this.IsAIControlled)
//        this.Formation?.SetHasPendingUnitPositions(false);
//      if (!updateOnlyMovement)
//        this.Formation?.Team.DetachmentManager.TickAgent(this);
//      if (updateOnlyMovement || !this.IsAIControlled)
//        return;
//      this.UpdateFormationOrders();
//      if (this.Formation == null)
//        return;
//      int fileIndex;
//      int rankIndex;
//      int fileCount;
//      int rankCount;
//      this.GetFormationFileAndRankInfo(out fileIndex, out rankIndex, out fileCount, out rankCount);
//      Vec2 formationLocation = this.GetWallDirectionOfRelativeFormationLocation();
//      MBAPI.IMBAgent.SetFormationInfo(this.GetPtr(), fileIndex, rankIndex, fileCount, rankCount, this.Formation.CountOfUnits, formationLocation, this.Formation.UnitSpacing);
//    }

//    private void ParallelUpdateCachedAndFormationValues(bool updateOnlyMovement)
//    {
//      if (!this.IsActive())
//        return;
//      if (!updateOnlyMovement)
//      {
//        Agent mountAgent = this.MountAgent;
//        this.WalkSpeedCached = mountAgent != null ? mountAgent.WalkingSpeedLimitOfMountable : this.Monster.WalkingSpeedLimit;
//      }
//      if (GameNetwork.IsClientOrReplay || !this.IsAIControlled)
//        return;
//      this.HumanAIComponent.ParallelUpdateFormationMovement();
//    }

//    public void UpdateLastRangedAttackTimeDueToAnAttack(float newTime)
//    {
//      this.LastRangedAttackTime = newTime;
//    }

//    public void InvalidateTargetAgent() => MBAPI.IMBAgent.InvalidateTargetAgent(this.GetPtr());

//    public void InvalidateAIWeaponSelections()
//    {
//      MBAPI.IMBAgent.InvalidateAIWeaponSelections(this.GetPtr());
//    }

//    public void ResetLookAgent() => this.SetLookAgent((Agent) null);

//    public void ResetGuard() => MBAPI.IMBAgent.ResetGuard(this.GetPtr());

//    public void ResetAgentProperties() => this.AgentDrivenProperties = (AgentDrivenProperties) null;

//    public void ResetAiWaitBeforeShootFactor()
//    {
//      this._propertyModifiers.resetAiWaitBeforeShootFactor = true;
//    }

//    public void ClearTargetFrame()
//    {
//      this._checkIfTargetFrameIsChanged = false;
//      if (this.MovementLockedState == AgentMovementLockedState.None)
//        return;
//      this.ClearTargetFrameAux();
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new ClearAgentTargetFrame(this.Index));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//    }

//    public void ClearEquipment() => MBAPI.IMBAgent.ClearEquipment(this.GetPtr());

//    public void ClearHandInverseKinematics()
//    {
//      MBAPI.IMBAgent.ClearHandInverseKinematics(this.GetPtr());
//    }

//    public void ClearAttachedWeapons() => this._attachedWeapons?.Clear();

//    public void SetDetachableFromFormation(bool value)
//    {
//      bool detachableFromFormation = this._isDetachableFromFormation;
//      if (detachableFromFormation == value)
//        return;
//      if (detachableFromFormation)
//      {
//        if (this.IsDetachedFromFormation)
//          this.TryAttachToFormation();
//        this.TryRemoveAllDetachmentScores();
//      }
//      this._isDetachableFromFormation = value;
//      if (this.IsPlayerControlled)
//        return;
//      if (detachableFromFormation)
//        this._formation?.OnUndetachableNonPlayerUnitAdded(this);
//      else
//        this._formation?.OnUndetachableNonPlayerUnitRemoved(this);
//    }

//    public bool TryAttachToFormation()
//    {
//      if (!this.IsDetachedFromFormation)
//        return false;
//      this._detachment.RemoveAgent(this);
//      this._formation?.AttachUnit(this);
//      return true;
//    }

//    public bool TryRemoveAllDetachmentScores()
//    {
//      if (!this.IsAIControlled)
//        return false;
//      this._formation?.Team?.DetachmentManager.RemoveScoresOfAgentFromDetachments(this);
//      return true;
//    }

//    public void EnforceShieldUsage(Agent.UsageDirection shieldDirection)
//    {
//      MBAPI.IMBAgent.EnforceShieldUsage(this.GetPtr(), shieldDirection);
//    }

//    public bool ObjectHasVacantPosition(UsableMissionObject gameObject)
//    {
//      return !gameObject.HasUser || gameObject.HasAIUser;
//    }

//    public bool InteractingWithAnyGameObject()
//    {
//      if (this.IsUsingGameObject)
//        return true;
//      return this.IsAIControlled && this.AIInterestedInAnyGameObject();
//    }

//    private void StopUsingGameObjectAux(bool isSuccessful, Agent.StopUsingGameObjectFlags flags)
//    {
//      UsableMachine detachmentOrDefault = this.Controller != AgentControllerType.AI || !this.IsDetachableFromFormation || this.Formation == null ? (UsableMachine) null : this.Formation.GetDetachmentOrDefault(this) as UsableMachine;
//      if (detachmentOrDefault == null)
//        flags &= ~Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject;
//      UsableMissionObject currentlyUsedGameObject = this.CurrentlyUsedGameObject;
//      UsableMissionObject movingToOrDefendingObject = (UsableMissionObject) null;
//      if (!this.IsUsingGameObject && this.IsAIControlled)
//        movingToOrDefendingObject = !this.AIMoveToGameObjectIsEnabled() ? this.HumanAIComponent.GetCurrentlyDefendingGameObject() : this.HumanAIComponent.GetCurrentlyMovingGameObject();
//      if (this.IsUsingGameObject)
//      {
//        int num = this.CurrentlyUsedGameObject.LockUserFrames ? 1 : (this.CurrentlyUsedGameObject.LockUserPositions ? 1 : 0);
//        if (GameNetwork.IsServerOrRecorder)
//        {
//          GameNetwork.BeginBroadcastModuleEvent();
//          GameNetwork.WriteMessage((GameNetworkMessage) new StopUsingObject(this.Index, isSuccessful));
//          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        }
//        this.CurrentlyUsedGameObject.OnUseStopped(this, isSuccessful, this._usedObjectPreferenceIndex);
//        this.CurrentlyUsedGameObject = (UsableMissionObject) null;
//        if (this.IsAIControlled)
//          this.AIUseGameObjectDisable();
//        this._usedObjectPreferenceIndex = -1;
//        if (num != 0)
//          this.ClearTargetFrame();
//      }
//      else if (this.IsAIControlled)
//      {
//        if (this.AIDefendGameObjectIsEnabled())
//          this.AIDefendGameObjectDisable();
//        else
//          this.AIMoveToGameObjectDisable();
//      }
//      if (this.State == AgentState.Active)
//      {
//        if (this.IsAIControlled)
//        {
//          this.DisableScriptedMovement();
//          if (detachmentOrDefault != null)
//          {
//            foreach (StandingPoint standingPoint in (List<StandingPoint>) detachmentOrDefault.StandingPoints)
//              standingPoint.FavoredUser = this;
//          }
//        }
//        this.AfterStoppedUsingMissionObject(detachmentOrDefault, currentlyUsedGameObject, movingToOrDefendingObject, isSuccessful, flags);
//      }
//      this.Mission.OnObjectStoppedBeingUsed(this, currentlyUsedGameObject);
//      this._components.ForEach((Action<AgentComponent>) (ac => ac.OnStopUsingGameObject()));
//    }

//    public void StopUsingGameObjectMT(bool isSuccessful = true, Agent.StopUsingGameObjectFlags flags = Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject)
//    {
//      lock (Agent._stopUsingGameObjectLock)
//        this.StopUsingGameObjectAux(isSuccessful, flags);
//    }

//    public void StopUsingGameObject(bool isSuccessful = true, Agent.StopUsingGameObjectFlags flags = Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject)
//    {
//      this.StopUsingGameObjectAux(isSuccessful, flags);
//    }

//    public void HandleStopUsingAction()
//    {
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new RequestStopUsingObject());
//        GameNetwork.EndModuleEventAsClient();
//      }
//      else
//        this.StopUsingGameObject(false);
//    }

//    public void HandleStartUsingAction(UsableMissionObject targetObject, int preferenceIndex)
//    {
//      if (GameNetwork.IsClient)
//      {
//        GameNetwork.BeginModuleEventAsClient();
//        GameNetwork.WriteMessage((GameNetworkMessage) new RequestUseObject(targetObject.Id, preferenceIndex));
//        GameNetwork.EndModuleEventAsClient();
//      }
//      else
//        this.UseGameObject(targetObject, preferenceIndex);
//    }

//    public AgentController AddController(System.Type type)
//    {
//      AgentController agentController = (AgentController) null;
//      if (type.IsSubclassOf(typeof (AgentController)))
//        agentController = Activator.CreateInstance(type) as AgentController;
//      if (agentController != null)
//      {
//        agentController.Owner = this;
//        agentController.Mission = this.Mission;
//        this._agentControllers.Add(agentController);
//        agentController.OnInitialize();
//      }
//      return agentController;
//    }

//    public AgentController RemoveController(System.Type type)
//    {
//      for (int index = 0; index < this._agentControllers.Count; ++index)
//      {
//        if (type.IsInstanceOfType((object) this._agentControllers[index]))
//        {
//          AgentController agentController = this._agentControllers[index];
//          this._agentControllers.RemoveAt(index);
//          return agentController;
//        }
//      }
//      return (AgentController) null;
//    }

//    public bool CanThrustAttackStickToBone(BoneBodyPartType bodyPart)
//    {
//      if (this.IsHuman)
//      {
//        BoneBodyPartType[] boneBodyPartTypeArray = new BoneBodyPartType[8]
//        {
//          BoneBodyPartType.Abdomen,
//          BoneBodyPartType.Legs,
//          BoneBodyPartType.Chest,
//          BoneBodyPartType.Neck,
//          BoneBodyPartType.ShoulderLeft,
//          BoneBodyPartType.ShoulderRight,
//          BoneBodyPartType.ArmLeft,
//          BoneBodyPartType.ArmRight
//        };
//        foreach (BoneBodyPartType boneBodyPartType in boneBodyPartTypeArray)
//        {
//          if (bodyPart == boneBodyPartType)
//            return true;
//        }
//      }
//      return false;
//    }

//    public void GetOldWieldedItemInfo(
//      out int rightHandSlotIndex,
//      out int rightHandUsageIndex,
//      out int leftHandSlotIndex,
//      out int leftHandUsageIndex)
//    {
//      MBAPI.IMBAgent.GetOldWieldedItemInfo(this.GetPtr(), out rightHandSlotIndex, out rightHandUsageIndex, out leftHandSlotIndex, out leftHandUsageIndex);
//    }

//    public void StartSwitchingWeaponUsageIndexAsClient(
//      EquipmentIndex equipmentIndex,
//      int usageIndex,
//      Agent.UsageDirection currentMovementFlagUsageDirection)
//    {
//      MBAPI.IMBAgent.StartSwitchingWeaponUsageIndexAsClient(this.GetPtr(), (int) equipmentIndex, usageIndex, currentMovementFlagUsageDirection);
//    }

//    public void TryToWieldWeaponInSlot(
//      EquipmentIndex slotIndex,
//      Agent.WeaponWieldActionType type,
//      bool isWieldedOnSpawn)
//    {
//      MBAPI.IMBAgent.TryToWieldWeaponInSlot(this.GetPtr(), (int) slotIndex, (int) type, isWieldedOnSpawn);
//    }

//    public void PrepareWeaponForDropInEquipmentSlot(EquipmentIndex slotIndex, bool dropWithHolster)
//    {
//      MBAPI.IMBAgent.PrepareWeaponForDropInEquipmentSlot(this.GetPtr(), (int) slotIndex, dropWithHolster);
//    }

//    public void AddHitter(MissionPeer peer, float damage, bool isFriendlyHit)
//    {
//      Agent.Hitter hitter = this._hitterList.Find((Predicate<Agent.Hitter>) (h => h.HitterPeer == peer && h.IsFriendlyHit == isFriendlyHit));
//      if (hitter == null)
//        this._hitterList.Add(new Agent.Hitter(peer, damage, isFriendlyHit));
//      else
//        hitter.IncreaseDamage(damage);
//    }

//    public void TryToSheathWeaponInHand(Agent.HandIndex handIndex, Agent.WeaponWieldActionType type)
//    {
//      MBAPI.IMBAgent.TryToSheathWeaponInHand(this.GetPtr(), (int) handIndex, (int) type);
//    }

//    public void RemoveHitter(MissionPeer peer, bool isFriendlyHit)
//    {
//      Agent.Hitter hitter = this._hitterList.Find((Predicate<Agent.Hitter>) (h => h.HitterPeer == peer && h.IsFriendlyHit == isFriendlyHit));
//      if (hitter == null)
//        return;
//      this._hitterList.Remove(hitter);
//    }

//    public void Retreat(WorldPosition retreatPos)
//    {
//      MBAPI.IMBAgent.SetRetreatMode(this.GetPtr(), retreatPos, true);
//    }

//    public void StopRetreating()
//    {
//      MBAPI.IMBAgent.SetRetreatMode(this.GetPtr(), WorldPosition.Invalid, false);
//      this.IsRunningAway = false;
//    }

//    public void UseGameObject(UsableMissionObject usedObject, int preferenceIndex = -1)
//    {
//      if (usedObject.LockUserFrames)
//      {
//        WorldFrame userFrameForAgent = usedObject.GetUserFrameForAgent(this);
//        this.SetTargetPositionAndDirection(userFrameForAgent.Origin.AsVec2, in userFrameForAgent.Rotation.f);
//        this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.NoAttack);
//      }
//      else if (usedObject.LockUserPositions)
//      {
//        this.SetTargetPosition(usedObject.GetUserFrameForAgent(this).Origin.AsVec2);
//        this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.NoAttack);
//      }
//      if (this.IsActive() && this.IsAIControlled && this.AIMoveToGameObjectIsEnabled())
//      {
//        this.AIMoveToGameObjectDisable();
//        this.Formation?.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(this);
//      }
//      this.CurrentlyUsedGameObject = usedObject;
//      this._usedObjectPreferenceIndex = preferenceIndex;
//      if (this.IsAIControlled)
//        this.AIUseGameObjectEnable();
//      if (!this.IsInWater() || this.GetPrimaryWieldedItemIndex() != EquipmentIndex.None || this.GetOffhandWieldedItemIndex() != EquipmentIndex.None)
//        this.SaveEquipmentsOnHand();
//      usedObject.OnUse(this, (sbyte) -1);
//      this.Mission.OnObjectUsed(this, usedObject);
//      if (!usedObject.IsInstantUse || GameNetwork.IsClientOrReplay || !this.IsActive() || !this.InteractingWithAnyGameObject())
//        return;
//      this.StopUsingGameObject();
//    }

//    public void SaveEquipmentsOnHand()
//    {
//      this._equipmentOnMainHandBeforeUsingObject = this.GetPrimaryWieldedItemIndex();
//      this._equipmentOnOffHandBeforeUsingObject = this.GetOffhandWieldedItemIndex();
//    }

//    public void StartFadingOut() => MBAPI.IMBAgent.StartFadingOut(this.GetPtr());

//    public bool IsWandering() => MBAPI.IMBAgent.IsWandering(this.GetPtr());

//    public void SetRenderCheckEnabled(bool value)
//    {
//      MBAPI.IMBAgent.SetRenderCheckEnabled(this.GetPtr(), value);
//    }

//    public bool GetRenderCheckEnabled() => MBAPI.IMBAgent.GetRenderCheckEnabled(this.GetPtr());

//    public Vec3 ComputeAnimationDisplacement(float dt)
//    {
//      return MBAPI.IMBAgent.ComputeAnimationDisplacement(this.GetPtr(), dt);
//    }

//    public void TickActionChannels(float dt)
//    {
//      MBAPI.IMBAgent.TickActionChannels(this.GetPtr(), dt);
//    }

//    public void SetIsPhysicsForceClosed(bool isPhysicsForceClosed)
//    {
//      MBAPI.IMBAgent.SetIsPhysicsForceClosed(this.GetPtr(), isPhysicsForceClosed);
//    }

//    public void LockAgentReplicationTableDataWithCurrentReliableSequenceNo(NetworkCommunicator peer)
//    {
//      MBDebug.Print("peer: " + peer.UserName + " index: " + (object) this.Index + " name: " + this.Name);
//      MBAPI.IMBAgent.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(this.GetPtr(), peer.Index);
//    }

//    public void TeleportToPosition(Vec3 position)
//    {
//      if (this.MountAgent != null)
//        MBAPI.IMBAgent.SetPosition(this.MountAgent.GetPtr(), ref position);
//      MBAPI.IMBAgent.SetPosition(this.GetPtr(), ref position);
//      if (this.RiderAgent != null)
//        MBAPI.IMBAgent.SetPosition(this.RiderAgent.GetPtr(), ref position);
//      foreach (AgentComponent component in (List<AgentComponent>) this._components)
//        component.OnAgentTeleported();
//    }

//    public void FadeOut(bool hideInstantly, bool hideMount)
//    {
//      MBAPI.IMBAgent.FadeOut(this.GetPtr(), hideInstantly);
//      if (!hideMount || !this.HasMount)
//        return;
//      this.MountAgent.FadeOut(hideMount, false);
//    }

//    public void FadeIn() => MBAPI.IMBAgent.FadeIn(this.GetPtr());

//    public void DisableScriptedMovement() => MBAPI.IMBAgent.DisableScriptedMovement(this.GetPtr());

//    public void DisableScriptedCombatMovement()
//    {
//      MBAPI.IMBAgent.DisableScriptedCombatMovement(this.GetPtr());
//    }

//    public void ForceAiBehaviorSelection()
//    {
//      MBAPI.IMBAgent.ForceAiBehaviorSelection(this.GetPtr());
//    }

//    public bool HasPathThroughNavigationFaceIdFromDirectionMT(int navigationFaceId, Vec2 direction)
//    {
//      lock (Agent._pathCheckObjectLock)
//        return MBAPI.IMBAgent.HasPathThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, ref direction);
//    }

//    public bool HasPathThroughNavigationFaceIdFromDirection(int navigationFaceId, Vec2 direction)
//    {
//      return MBAPI.IMBAgent.HasPathThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, ref direction);
//    }

//    public void DisableLookToPointOfInterest()
//    {
//      MBAPI.IMBAgent.DisableLookToPointOfInterest(this.GetPtr());
//    }

//    public CompositeComponent AddPrefabComponentToBone(string prefabName, sbyte boneIndex)
//    {
//      return MBAPI.IMBAgent.AddPrefabToAgentBone(this.GetPtr(), prefabName, boneIndex);
//    }

//    public void MakeVoice(
//      SkinVoiceManager.SkinVoiceType voiceType,
//      SkinVoiceManager.CombatVoiceNetworkPredictionType predictionType)
//    {
//      MBAPI.IMBAgent.MakeVoice(this.GetPtr(), voiceType.Index, (int) predictionType);
//    }

//    public void YellAfterDelay(float delayTimeInSecond)
//    {
//      MBAPI.IMBAgent.YellAfterDelay(this.GetPtr(), delayTimeInSecond);
//    }

//    public void WieldNextWeapon(
//      Agent.HandIndex weaponIndex,
//      Agent.WeaponWieldActionType wieldActionType = Agent.WeaponWieldActionType.WithAnimation)
//    {
//      MBAPI.IMBAgent.WieldNextWeapon(this.GetPtr(), (int) weaponIndex, (int) wieldActionType);
//    }

//    public Agent.MovementControlFlag AttackDirectionToMovementFlag(Agent.UsageDirection direction)
//    {
//      return MBAPI.IMBAgent.AttackDirectionToMovementFlag(this.GetPtr(), direction);
//    }

//    public Agent.MovementControlFlag DefendDirectionToMovementFlag(Agent.UsageDirection direction)
//    {
//      return MBAPI.IMBAgent.DefendDirectionToMovementFlag(this.GetPtr(), direction);
//    }

//    public bool KickClear() => MBAPI.IMBAgent.KickClear(this.GetPtr());

//    public Agent.UsageDirection PlayerAttackDirection()
//    {
//      return MBAPI.IMBAgent.PlayerAttackDirection(this.GetPtr());
//    }

//    public (sbyte, sbyte) GetRandomPairOfRealBloodBurstBoneIndices()
//    {
//      sbyte num1 = -1;
//      sbyte num2 = -1;
//      if (this.Monster.BloodBurstBoneIndices.Length != 0)
//      {
//        int num3 = MBRandom.RandomInt(this.Monster.BloodBurstBoneIndices.Length / 2);
//        num1 = this.Monster.BloodBurstBoneIndices[num3 * 2];
//        num2 = this.Monster.BloodBurstBoneIndices[num3 * 2 + 1];
//      }
//      return (num1, num2);
//    }

//    public void CreateBloodBurstAtLimb(sbyte realBoneIndex, float scale)
//    {
//      MBAPI.IMBAgent.CreateBloodBurstAtLimb(this.GetPtr(), realBoneIndex, scale);
//    }

//    public void AddComponent(AgentComponent agentComponent)
//    {
//      this._components.Add(agentComponent);
//      switch (agentComponent)
//      {
//        case CommonAIComponent commonAiComponent:
//          this.CommonAIComponent = commonAiComponent;
//          break;
//        case HumanAIComponent humanAiComponent:
//          this.HumanAIComponent = humanAiComponent;
//          break;
//      }
//    }

//    public bool RemoveComponent(AgentComponent agentComponent)
//    {
//      int num = this._components.Remove(agentComponent) ? 1 : 0;
//      if (num == 0)
//        return num != 0;
//      agentComponent.OnComponentRemoved();
//      if (this.CommonAIComponent == agentComponent)
//      {
//        this.CommonAIComponent = (CommonAIComponent) null;
//        return num != 0;
//      }
//      if (this.HumanAIComponent != agentComponent)
//        return num != 0;
//      this.HumanAIComponent = (HumanAIComponent) null;
//      return num != 0;
//    }

//    public void HandleTaunt(int tauntIndex, bool isDefaultTaunt)
//    {
//      if (tauntIndex < 0)
//        return;
//      if (isDefaultTaunt)
//      {
//        ActionIndexCache actionIndexCache = Agent.DefaultTauntActions[tauntIndex];
//        this.SetActionChannel(1, in actionIndexCache);
//        this.MakeVoice(SkinVoiceManager.VoiceType.Victory, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//      }
//      else
//      {
//        if (GameNetwork.IsClientOrReplay)
//          return;
//        ActionIndexCache actionIndexCache = CosmeticsManagerHelper.GetSuitableTauntAction(this, tauntIndex);
//        if (actionIndexCache.Index < 0)
//          return;
//        this.SetActionChannel(1, in actionIndexCache);
//      }
//    }

//    public void HandleBark(int indexOfBark)
//    {
//      if (indexOfBark >= SkinVoiceManager.VoiceType.MpBarks.Length || GameNetwork.IsClientOrReplay)
//        return;
//      this.MakeVoice(SkinVoiceManager.VoiceType.MpBarks[indexOfBark], SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//      if (!GameNetwork.IsMultiplayer)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new BarkAgent(this.Index, indexOfBark));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, this.MissionPeer.GetNetworkPeer());
//    }

//    public void HandleDropWeapon(
//      bool isDefendPressed,
//      EquipmentIndex forcedSlotIndexToDropWeaponFrom)
//    {
//      Agent.ActionCodeType currentActionType = this.GetCurrentActionType(1);
//      if (this.State != AgentState.Active || currentActionType == Agent.ActionCodeType.ReleaseMelee || currentActionType == Agent.ActionCodeType.ReleaseRanged || currentActionType == Agent.ActionCodeType.ReleaseThrowing || currentActionType == Agent.ActionCodeType.WeaponBash)
//        return;
//      EquipmentIndex equipmentIndex = forcedSlotIndexToDropWeaponFrom;
//      if (equipmentIndex == EquipmentIndex.None)
//      {
//        EquipmentIndex wieldedItemIndex1 = this.GetPrimaryWieldedItemIndex();
//        EquipmentIndex wieldedItemIndex2 = this.GetOffhandWieldedItemIndex();
//        if (wieldedItemIndex2 >= EquipmentIndex.WeaponItemBeginSlot & isDefendPressed)
//          equipmentIndex = wieldedItemIndex2;
//        else if (wieldedItemIndex1 >= EquipmentIndex.WeaponItemBeginSlot)
//          equipmentIndex = wieldedItemIndex1;
//        else if (wieldedItemIndex2 >= EquipmentIndex.WeaponItemBeginSlot)
//        {
//          equipmentIndex = wieldedItemIndex2;
//        }
//        else
//        {
//          for (EquipmentIndex index1 = EquipmentIndex.WeaponItemBeginSlot; index1 < EquipmentIndex.ExtraWeaponSlot; ++index1)
//          {
//            if (!this.Equipment[index1].IsEmpty && this.Equipment[index1].Item.PrimaryWeapon.IsConsumable)
//            {
//              if (this.Equipment[index1].Item.PrimaryWeapon.IsRangedWeapon)
//              {
//                if (this.Equipment[index1].Amount == (short) 0)
//                {
//                  equipmentIndex = index1;
//                  break;
//                }
//              }
//              else
//              {
//                bool flag = false;
//                for (EquipmentIndex index2 = EquipmentIndex.WeaponItemBeginSlot; index2 < EquipmentIndex.ExtraWeaponSlot; ++index2)
//                {
//                  if (!this.Equipment[index2].IsEmpty && this.Equipment[index2].HasAnyUsageWithAmmoClass(this.Equipment[index1].Item.PrimaryWeapon.WeaponClass) && this.Equipment[index1].Amount > (short) 0)
//                  {
//                    flag = true;
//                    break;
//                  }
//                }
//                if (!flag)
//                {
//                  equipmentIndex = index1;
//                  break;
//                }
//              }
//            }
//          }
//        }
//      }
//      if (equipmentIndex == EquipmentIndex.None || this.Equipment[equipmentIndex].IsEmpty)
//        return;
//      this.DropItem(equipmentIndex);
//      this.UpdateAgentProperties();
//    }

//    public void DropItem(EquipmentIndex itemIndex, WeaponClass pickedUpItemType = WeaponClass.Undefined)
//    {
//      if (this.Equipment[itemIndex].CurrentUsageItem.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.AffectsArea | WeaponFlags.Burning))
//      {
//        MatrixFrame m = this.AgentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex(this.Monster.MainHandItemBoneIndex);
//        MatrixFrame globalFrame = this.AgentVisuals.GetGlobalFrame();
//        MatrixFrame parent = globalFrame.TransformToParent(in m);
//        Vec3 velocity = globalFrame.origin + globalFrame.rotation.f - parent.origin;
//        double num = (double) velocity.Normalize();
//        Mat3 identity = Mat3.Identity with { f = velocity };
//        identity.Orthonormalize();
//        Mission.Current.OnAgentShootMissile(this, itemIndex, parent.origin, velocity, identity, false, false, -1);
//        this.RemoveEquippedWeapon(itemIndex);
//      }
//      else
//        MBAPI.IMBAgent.DropItem(this.GetPtr(), (int) itemIndex, (int) pickedUpItemType);
//    }

//    public void EquipItemsFromSpawnEquipment(bool neededBatchedItems, bool prepareImmediately)
//    {
//      this.Mission.OnEquipItemsFromSpawnEquipmentBegin(this, this._creationType);
//      switch (this._creationType)
//      {
//        case Agent.CreationType.FromRoster:
//        case Agent.CreationType.FromCharacterObj:
//          for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
//          {
//            WeaponData weaponData = WeaponData.InvalidWeaponData;
//            WeaponStatsData[] weaponStatsData = (WeaponStatsData[]) null;
//            WeaponData ammoWeaponData = WeaponData.InvalidWeaponData;
//            WeaponStatsData[] ammoWeaponStatsData = (WeaponStatsData[]) null;
//            MissionWeapon missionWeapon = this.Equipment[equipmentIndex];
//            if (!missionWeapon.IsEmpty)
//            {
//              missionWeapon = this.Equipment[equipmentIndex];
//              weaponData = missionWeapon.GetWeaponData(neededBatchedItems);
//              missionWeapon = this.Equipment[equipmentIndex];
//              weaponStatsData = missionWeapon.GetWeaponStatsData();
//              missionWeapon = this.Equipment[equipmentIndex];
//              ammoWeaponData = missionWeapon.GetAmmoWeaponData(neededBatchedItems);
//              missionWeapon = this.Equipment[equipmentIndex];
//              ammoWeaponStatsData = missionWeapon.GetAmmoWeaponStatsData();
//            }
//            this.WeaponEquipped(equipmentIndex, in weaponData, weaponStatsData, in ammoWeaponData, ammoWeaponStatsData, WeakGameEntity.Invalid, true, true);
//            weaponData.DeinitializeManagedPointers();
//            ammoWeaponData.DeinitializeManagedPointers();
//            int attachmentIndex = 0;
//            while (true)
//            {
//              int num = attachmentIndex;
//              missionWeapon = this.Equipment[equipmentIndex];
//              int attachedWeaponsCount = missionWeapon.GetAttachedWeaponsCount();
//              if (num < attachedWeaponsCount)
//              {
//                missionWeapon = this.Equipment[equipmentIndex];
//                MatrixFrame attachedWeaponFrame = missionWeapon.GetAttachedWeaponFrame(attachmentIndex);
//                missionWeapon = this.Equipment[equipmentIndex];
//                MissionWeapon attachedWeapon = missionWeapon.GetAttachedWeapon(attachmentIndex);
//                this.AttachWeaponToWeaponAux(equipmentIndex, ref attachedWeapon, (GameEntity) null, ref attachedWeaponFrame);
//                ++attachmentIndex;
//              }
//              else
//                break;
//            }
//          }
//          this.AddSkinMeshes(!neededBatchedItems | prepareImmediately);
//          break;
//      }
//      this.UpdateAgentProperties();
//      this.Mission.OnEquipItemsFromSpawnEquipment(this, this._creationType);
//      this.CheckEquipmentForCapeClothSimulationStateChange();
//    }

//    public void WieldInitialWeapons(
//      Agent.WeaponWieldActionType wieldActionType = Agent.WeaponWieldActionType.InstantAfterPickUp,
//      TaleWorlds.Core.Equipment.InitialWeaponEquipPreference initialWeaponEquipPreference = TaleWorlds.Core.Equipment.InitialWeaponEquipPreference.Any)
//    {
//      EquipmentIndex mainHandWeaponIndex = this.GetPrimaryWieldedItemIndex();
//      EquipmentIndex offHandWeaponIndex = this.GetOffhandWieldedItemIndex();
//      this.SpawnEquipment.GetInitialWeaponIndicesToEquip(out mainHandWeaponIndex, out offHandWeaponIndex, out bool _, initialWeaponEquipPreference);
//      if (offHandWeaponIndex != EquipmentIndex.None)
//        this.TryToWieldWeaponInSlot(offHandWeaponIndex, wieldActionType, true);
//      if (mainHandWeaponIndex == EquipmentIndex.None)
//        return;
//      this.TryToWieldWeaponInSlot(mainHandWeaponIndex, wieldActionType, true);
//      if (this.GetPrimaryWieldedItemIndex() != EquipmentIndex.None)
//        return;
//      this.WieldNextWeapon(Agent.HandIndex.MainHand, wieldActionType);
//    }

//    public void ChangeWeaponHitPoints(EquipmentIndex slotIndex, short hitPoints)
//    {
//      this.Equipment.SetHitPointsOfSlot(slotIndex, hitPoints);
//      this.SetWeaponHitPointsInSlot(slotIndex, hitPoints);
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new SetWeaponNetworkData(this.Index, slotIndex, hitPoints));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      foreach (AgentComponent component in (List<AgentComponent>) this._components)
//        component.OnWeaponHPChanged(this.Equipment[slotIndex].Item, (int) hitPoints);
//    }

//    public bool HasWeapon()
//    {
//      for (int index = 0; index < 5; ++index)
//      {
//        WeaponComponentData currentUsageItem = this.Equipment[index].CurrentUsageItem;
//        if (currentUsageItem != null && currentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.WeaponMask))
//          return true;
//      }
//      return false;
//    }

//    public void AttachWeaponToWeapon(
//      EquipmentIndex slotIndex,
//      MissionWeapon weapon,
//      GameEntity weaponEntity,
//      ref MatrixFrame attachLocalFrame)
//    {
//      this.Equipment.AttachWeaponToWeaponInSlot(slotIndex, ref weapon, ref attachLocalFrame);
//      this.AttachWeaponToWeaponAux(slotIndex, ref weapon, weaponEntity, ref attachLocalFrame);
//    }

//    public void AttachWeaponToBone(
//      MissionWeapon weapon,
//      GameEntity weaponEntity,
//      sbyte boneIndex,
//      ref MatrixFrame attachLocalFrame)
//    {
//      if (this._attachedWeapons == null)
//        this._attachedWeapons = new List<(MissionWeapon, MatrixFrame, sbyte)>();
//      this._attachedWeapons.Add((weapon, attachLocalFrame, boneIndex));
//      this.AttachWeaponToBoneAux(ref weapon, weaponEntity, boneIndex, ref attachLocalFrame);
//    }

//    public void RestoreShieldHitPoints()
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; ++index)
//      {
//        MissionWeapon missionWeapon = this.Equipment[index];
//        if (!missionWeapon.IsEmpty)
//        {
//          missionWeapon = this.Equipment[index];
//          if (missionWeapon.CurrentUsageItem.IsShield)
//          {
//            int slotIndex = (int) index;
//            missionWeapon = this.Equipment[index];
//            int modifiedMaxHitPoints = (int) missionWeapon.ModifiedMaxHitPoints;
//            this.ChangeWeaponHitPoints((EquipmentIndex) slotIndex, (short) modifiedMaxHitPoints);
//          }
//        }
//      }
//    }

//    public void Die(Blow b, Agent.KillInfo overrideKillInfo = Agent.KillInfo.Invalid)
//    {
//      if (this.Formation != null)
//      {
//        this.Formation.Team.QuerySystem.RegisterDeath();
//        if (b.IsMissile)
//          this.Formation.Team.QuerySystem.RegisterDeathByRanged();
//      }
//      this.Health = 0.0f;
//      if (overrideKillInfo != Agent.KillInfo.TeamSwitch && (b.OwnerId == -1 || b.OwnerId == this.Index) && this.IsHuman && this._lastHitInfo.CanOverrideBlow)
//      {
//        b.OwnerId = this._lastHitInfo.LastBlowOwnerId;
//        b.AttackType = this._lastHitInfo.LastBlowAttackType;
//      }
//      MBAPI.IMBAgent.Die(this.GetPtr(), ref b, (sbyte) overrideKillInfo);
//    }

//    public void MakeDead(bool isKilled, ActionIndexCache actionIndex, int corpsesToFadeIndex = -1)
//    {
//      MBAPI.IMBAgent.MakeDead(this.GetPtr(), isKilled, actionIndex.Index, corpsesToFadeIndex);
//    }

//    public void RegisterBlow(Blow blow, in AttackCollisionData collisionData)
//    {
//      this.HandleBlow(ref blow, in collisionData);
//    }

//    public void CreateBlowFromBlowAsReflection(
//      in Blow blow,
//      in AttackCollisionData collisionData,
//      out Blow outBlow,
//      out AttackCollisionData outCollisionData)
//    {
//      outBlow = blow;
//      outBlow.InflictedDamage = blow.SelfInflictedDamage;
//      outBlow.GlobalPosition = this.Position;
//      outBlow.BoneIndex = (sbyte) 0;
//      outBlow.BlowFlag = BlowFlags.None;
//      outCollisionData = collisionData;
//      outCollisionData.UpdateCollisionPositionAndBoneForReflect(collisionData.InflictedDamage, this.Position, (sbyte) 0);
//    }

//    public void TickParallel(float dt)
//    {
//      if (this.IsActive())
//      {
//        if (GameNetwork.IsMultiplayer && this.GetCurrentActionStage(1) == Agent.ActionStage.AttackQuickReady)
//          this._lastMultiplayerQuickReadyDetectedTime = Mission.Current.CurrentTime;
//        if (this._checkIfTargetFrameIsChanged)
//        {
//          Vec2 vecFrom1 = this.MovementLockedState != AgentMovementLockedState.None ? this.GetTargetPosition() : this.LookFrame.origin.AsVec2;
//          Vec3 vecFrom2 = this.MovementLockedState != AgentMovementLockedState.None ? this.GetTargetDirection() : this.LookFrame.rotation.f;
//          switch (this.MovementLockedState)
//          {
//            case AgentMovementLockedState.PositionLocked:
//              this._checkIfTargetFrameIsChanged = this._lastSynchedTargetPosition != vecFrom1;
//              break;
//            case AgentMovementLockedState.FrameLocked:
//              this._checkIfTargetFrameIsChanged = this._lastSynchedTargetPosition != vecFrom1 || this._lastSynchedTargetDirection != vecFrom2;
//              break;
//          }
//          if (this._checkIfTargetFrameIsChanged)
//          {
//            if (this.MovementLockedState == AgentMovementLockedState.FrameLocked)
//              this.SetTargetPositionAndDirection(MBMath.Lerp(vecFrom1, this._lastSynchedTargetPosition, 5f * dt, 0.005f), MBMath.Lerp(vecFrom2, this._lastSynchedTargetDirection, 5f * dt, 0.005f));
//            else
//              this.SetTargetPosition(MBMath.Lerp(vecFrom1, this._lastSynchedTargetPosition, 5f * dt, 0.005f));
//          }
//        }
//        foreach (AgentComponent component in (List<AgentComponent>) this._components)
//          component.OnTickParallel(dt);
//        if (this.Mission.AllowAiTicking && this.IsAIControlled && this._cachedAndFormationValuesUpdateTimer.Check(this.Mission.CurrentTime) && this.Formation != null)
//        {
//          this._cachedAndFormationValuesUpdateTimer.AdjustStartTime(-5f);
//          this.ParallelUpdateCachedAndFormationValues(false);
//        }
//        if (this._wantsToYell)
//        {
//          if ((double) this._yellTimer > 0.0)
//          {
//            this._yellTimer -= dt;
//          }
//          else
//          {
//            this.MakeVoice(this.MountAgent != null ? SkinVoiceManager.VoiceType.HorseRally : SkinVoiceManager.VoiceType.Yell, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//            this._wantsToYell = false;
//          }
//        }
//        if (!this.IsPlayerControlled || !this.IsCheering || !(this.MovementInputVector != Vec2.Zero))
//          return;
//        this.SetActionChannel(1, in ActionIndexCache.act_none);
//      }
//      else
//      {
//        if (this.MissionPeer?.ControlledAgent != this || this.IsCameraAttachable())
//          return;
//        this.MissionPeer.ControlledAgent = (Agent) null;
//      }
//    }

//    public void Tick(float dt)
//    {
//      if (this._changedFormationPosition.IsValid)
//      {
//        if (this.IsFormationFrameEnabled)
//        {
//          this.Formation?.Team?.TeamAI?.OnFormationFrameChanged(this, true, this._changedFormationPosition);
//          if (this.Mission.IsTeleportingAgents)
//            this.TeleportToPosition(this._changedFormationPosition.GetGroundVec3());
//        }
//        else
//          this.Formation?.Team?.TeamAI?.OnFormationFrameChanged(this, false, WorldPosition.Invalid);
//        this._changedFormationPosition = WorldPosition.Invalid;
//      }
//      if (!this.IsActive())
//        return;
//      foreach (AgentComponent component in (List<AgentComponent>) this._components)
//        component.OnTick(dt);
//      if (!this.Mission.AllowAiTicking || !this.IsAIControlled)
//        return;
//      this.TickAsAI();
//    }

//    [Conditional("DEBUG")]
//    public void DebugMore() => MBAPI.IMBAgent.DebugMore(this.GetPtr());

//    public void Mount(Agent mountAgent)
//    {
//      bool flag = mountAgent.GetCurrentActionType(0) == Agent.ActionCodeType.Rear;
//      if (this.MountAgent == null && mountAgent.RiderAgent == null)
//      {
//        if (!this.CheckSkillForMounting(mountAgent) || flag || !(this.GetCurrentAction(0) == ActionIndexCache.act_none))
//          return;
//        this.EventControlFlags |= Agent.EventControlFlag.Mount;
//        this.SetInteractionAgent(mountAgent);
//      }
//      else
//      {
//        if (this.MountAgent != mountAgent || flag)
//          return;
//        this.EventControlFlags |= Agent.EventControlFlag.Dismount;
//      }
//    }

//    public void EquipWeaponToExtraSlotAndWield(ref MissionWeapon weapon)
//    {
//      if (!this.Equipment[EquipmentIndex.ExtraWeaponSlot].IsEmpty)
//        this.DropItem(EquipmentIndex.ExtraWeaponSlot);
//      this.EquipWeaponWithNewEntity(EquipmentIndex.ExtraWeaponSlot, ref weapon);
//      this.TryToWieldWeaponInSlot(EquipmentIndex.ExtraWeaponSlot, Agent.WeaponWieldActionType.InstantAfterPickUp, false);
//    }

//    public void RemoveEquippedWeapon(EquipmentIndex slotIndex)
//    {
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.RemoveEquippedWeapon(this.Index, slotIndex));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      this.Equipment[slotIndex] = MissionWeapon.Invalid;
//      this.WeaponEquipped(slotIndex, in WeaponData.InvalidWeaponData, (WeaponStatsData[]) null, in WeaponData.InvalidWeaponData, (WeaponStatsData[]) null, WeakGameEntity.Invalid, true, false);
//      this.UpdateAgentProperties();
//    }

//    public void EquipWeaponWithNewEntity(EquipmentIndex slotIndex, ref MissionWeapon weapon)
//    {
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.EquipWeaponWithNewEntity(this.Index, slotIndex, weapon));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      this.Equipment[slotIndex] = weapon;
//      WeaponData weaponData = WeaponData.InvalidWeaponData;
//      WeaponStatsData[] weaponStatsData = (WeaponStatsData[]) null;
//      WeaponData ammoWeaponData = WeaponData.InvalidWeaponData;
//      WeaponStatsData[] ammoWeaponStatsData = (WeaponStatsData[]) null;
//      if (!weapon.IsEmpty)
//      {
//        weaponData = weapon.GetWeaponData(true);
//        weaponStatsData = weapon.GetWeaponStatsData();
//        ammoWeaponData = weapon.GetAmmoWeaponData(true);
//        ammoWeaponStatsData = weapon.GetAmmoWeaponStatsData();
//      }
//      this.WeaponEquipped(slotIndex, in weaponData, weaponStatsData, in ammoWeaponData, ammoWeaponStatsData, WeakGameEntity.Invalid, true, true);
//      weaponData.DeinitializeManagedPointers();
//      ammoWeaponData.DeinitializeManagedPointers();
//      for (int attachmentIndex = 0; attachmentIndex < weapon.GetAttachedWeaponsCount(); ++attachmentIndex)
//      {
//        MissionWeapon attachedWeapon = weapon.GetAttachedWeapon(attachmentIndex);
//        MatrixFrame attachedWeaponFrame = weapon.GetAttachedWeaponFrame(attachmentIndex);
//        if (GameNetwork.IsServerOrRecorder)
//        {
//          GameNetwork.BeginBroadcastModuleEvent();
//          GameNetwork.WriteMessage((GameNetworkMessage) new AttachWeaponToWeaponInAgentEquipmentSlot(attachedWeapon, this.Index, slotIndex, attachedWeaponFrame));
//          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        }
//        this.AttachWeaponToWeaponAux(slotIndex, ref attachedWeapon, (GameEntity) null, ref attachedWeaponFrame);
//      }
//      this.UpdateAgentProperties();
//    }

//    public void EquipWeaponFromSpawnedItemEntity(
//      EquipmentIndex slotIndex,
//      SpawnedItemEntity spawnedItemEntity,
//      bool removeWeapon)
//    {
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.EquipWeaponFromSpawnedItemEntity(this.Index, slotIndex, spawnedItemEntity.Id, removeWeapon));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      WeakGameEntity weakGameEntity = spawnedItemEntity.GameEntity;
//      weakGameEntity = weakGameEntity.Parent;
//      MissionWeapon weaponCopy1;
//      if (weakGameEntity.IsValid)
//      {
//        weakGameEntity = spawnedItemEntity.GameEntity;
//        weakGameEntity = weakGameEntity.Parent;
//        if (weakGameEntity.HasScriptOfType<SpawnedItemEntity>())
//        {
//          weakGameEntity = spawnedItemEntity.GameEntity;
//          weakGameEntity = weakGameEntity.Parent;
//          SpawnedItemEntity firstScriptOfType = weakGameEntity.GetFirstScriptOfType<SpawnedItemEntity>();
//          int attachmentIndex = -1;
//          int index = 0;
//          while (true)
//          {
//            int num = index;
//            weakGameEntity = firstScriptOfType.GameEntity;
//            int childCount = weakGameEntity.ChildCount;
//            if (num < childCount)
//            {
//              weakGameEntity = firstScriptOfType.GameEntity;
//              if (!(weakGameEntity.GetChild(index) == spawnedItemEntity.GameEntity))
//                ++index;
//              else
//                break;
//            }
//            else
//              goto label_9;
//          }
//          attachmentIndex = index;
//label_9:
//          weaponCopy1 = firstScriptOfType.WeaponCopy;
//          weaponCopy1.RemoveAttachedWeapon(attachmentIndex);
//        }
//      }
//      if (!removeWeapon)
//        return;
//      weaponCopy1 = this.Equipment[slotIndex];
//      if (!weaponCopy1.IsEmpty)
//      {
//        using (new TWSharedMutexWriteLock(Scene.PhysicsAndRayCastLock))
//        {
//          weakGameEntity = spawnedItemEntity.GameEntity;
//          weakGameEntity.Remove(73);
//        }
//      }
//      else
//      {
//        WeakGameEntity gameEntity = spawnedItemEntity.GameEntity;
//        using (new TWSharedMutexWriteLock(Scene.PhysicsAndRayCastLock))
//          gameEntity.RemovePhysics();
//        gameEntity.RemoveScriptComponent(spawnedItemEntity.ScriptComponent.Pointer, 10);
//        gameEntity.SetVisibilityExcludeParents(true);
//        MissionWeapon weaponCopy2 = spawnedItemEntity.WeaponCopy;
//        this.Equipment[slotIndex] = weaponCopy2;
//        WeaponData weaponData = weaponCopy2.GetWeaponData(true);
//        WeaponStatsData[] weaponStatsData = weaponCopy2.GetWeaponStatsData();
//        WeaponData ammoWeaponData = weaponCopy2.GetAmmoWeaponData(true);
//        WeaponStatsData[] ammoWeaponStatsData = weaponCopy2.GetAmmoWeaponStatsData();
//        this.WeaponEquipped(slotIndex, in weaponData, weaponStatsData, in ammoWeaponData, ammoWeaponStatsData, gameEntity, true, false);
//        weaponData.DeinitializeManagedPointers();
//        for (int attachmentIndex = 0; attachmentIndex < weaponCopy2.GetAttachedWeaponsCount(); ++attachmentIndex)
//        {
//          MatrixFrame attachedWeaponFrame = weaponCopy2.GetAttachedWeaponFrame(attachmentIndex);
//          MissionWeapon attachedWeapon = weaponCopy2.GetAttachedWeapon(attachmentIndex);
//          this.AttachWeaponToWeaponAux(slotIndex, ref attachedWeapon, (GameEntity) null, ref attachedWeaponFrame);
//        }
//        this.UpdateAgentProperties();
//      }
//    }

//    public void PreloadForRendering() => this.PreloadForRenderingAux();

//    public int AddSynchedPrefabComponentToBone(string prefabName, sbyte boneIndex)
//    {
//      if (this._synchedBodyComponents == null)
//        this._synchedBodyComponents = new List<CompositeComponent>();
//      if (!GameEntity.PrefabExists(prefabName))
//      {
//        MBDebug.ShowWarning("Missing prefab for agent logic :" + prefabName);
//        prefabName = "rock_001";
//      }
//      CompositeComponent bone = this.AddPrefabComponentToBone(prefabName, boneIndex);
//      int count = this._synchedBodyComponents.Count;
//      this._synchedBodyComponents.Add(bone);
//      if (!GameNetwork.IsServerOrRecorder)
//        return count;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new AddPrefabComponentToAgentBone(this.Index, prefabName, boneIndex));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      return count;
//    }

//    public bool WillDropWieldedShield(SpawnedItemEntity spawnedItem)
//    {
//      EquipmentIndex wieldedItemIndex = this.GetOffhandWieldedItemIndex();
//      if (wieldedItemIndex != EquipmentIndex.None && spawnedItem.WeaponCopy.CurrentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.NotUsableWithOneHand) && spawnedItem.WeaponCopy.HasAllUsagesWithAnyWeaponFlag(WeaponFlags.NotUsableWithOneHand))
//      {
//        bool flag = false;
//        for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; ++index)
//        {
//          if (index != wieldedItemIndex)
//          {
//            MissionWeapon missionWeapon = this.Equipment[index];
//            if (!missionWeapon.IsEmpty)
//            {
//              missionWeapon = this.Equipment[index];
//              if (missionWeapon.IsShield())
//              {
//                flag = true;
//                break;
//              }
//            }
//          }
//        }
//        if (flag)
//          return true;
//      }
//      return false;
//    }

//    public bool HadSameTypeOfConsumableOrShieldOnSpawn(WeaponClass weaponClass)
//    {
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; ++index)
//      {
//        if (!this.SpawnEquipment[index].IsEmpty)
//        {
//          foreach (WeaponComponentData weapon in (List<WeaponComponentData>) this.SpawnEquipment[index].Item.Weapons)
//          {
//            if ((weapon.IsConsumable || weapon.IsShield) && weapon.WeaponClass == weaponClass)
//              return true;
//          }
//        }
//      }
//      return false;
//    }

//    public override int GetHashCode() => this._creationIndex;

//    public bool TryGetImmediateEnemyAgentMovementData(
//      out float maximumForwardUnlimitedSpeed,
//      out Vec3 position)
//    {
//      return MBAPI.IMBAgent.TryGetImmediateEnemyAgentMovementData(this.GetPtr(), out maximumForwardUnlimitedSpeed, out position);
//    }

//    public bool HasLostShield()
//    {
//      bool flag1 = false;
//      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; ++index)
//      {
//        EquipmentElement equipmentElement = this.SpawnEquipment[index];
//        if (equipmentElement.Item != null)
//        {
//          equipmentElement = this.SpawnEquipment[index];
//          if (equipmentElement.Item.PrimaryWeapon.IsShield)
//          {
//            flag1 = true;
//            break;
//          }
//        }
//      }
//      bool flag2 = false;
//      if (flag1)
//      {
//        for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; ++index)
//        {
//          MissionWeapon missionWeapon = this.Equipment[index];
//          if (!missionWeapon.IsEmpty)
//          {
//            missionWeapon = this.Equipment[index];
//            if (missionWeapon.Item != null)
//            {
//              missionWeapon = this.Equipment[index];
//              if (missionWeapon.Item.PrimaryWeapon.IsShield)
//              {
//                flag2 = true;
//                break;
//              }
//            }
//          }
//        }
//      }
//      return flag1 && !flag2;
//    }

//    public void SetLastDetachmentTickAgentTime(float lastDetachmentTickAgentTime)
//    {
//      this.LastDetachmentTickAgentTime = lastDetachmentTickAgentTime;
//    }

//    public void SetDetachmentWeight(float newDetachmentWeight)
//    {
//      this.DetachmentWeight = newDetachmentWeight;
//    }

//    public void SetDetachmentIndex(int newDetachmentIndex)
//    {
//      this.DetachmentIndex = newDetachmentIndex;
//    }

//    public void SetOwningAgentMissionPeer(MissionPeer owningAgentMissionPeer)
//    {
//      this.OwningAgentMissionPeer = owningAgentMissionPeer;
//      if (!GameNetwork.IsServer)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentOwningMissionPeer(this.Index, owningAgentMissionPeer?.Peer));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
//    }

//    public void SetMissionRepresentative(MissionRepresentativeBase missionRepresentative)
//    {
//      this.MissionRepresentative = missionRepresentative;
//    }

//    public void SetIsLadderQueueUsing(bool isLadderQueueUsing)
//    {
//      this._isLadderQueueUsing = isLadderQueueUsing;
//    }

//    public void SetIsInLadderQueue(bool isInLadderQueue) => this.IsInLadderQueue = isInLadderQueue;

//    public void UpdateLocalPositionError()
//    {
//      float num1 = (float) (0.5 - (double) this.Character.SkillFactor * 0.5);
//      float num2 = (float) (0.10000000149011612 + (1.0 - (double) this.Character.SkillFactor));
//      Vec2 forward = Vec2.Forward;
//      forward.RotateCCW(3.14159274f * MBRandom.RandomFloat);
//      this.LocalPositionError = forward * (MBRandom.RandomFloatRanged(num2 - num1) + num1);
//    }

//    public void YellingBehaviour()
//    {
//      if (!this.IsAIControlled || (double) MBRandom.RandomFloat >= (double) this.GetMorale() * 0.0032999999821186066)
//        return;
//      this.YellAfterDelay(1.5f + MBRandom.RandomFloat);
//    }

//    internal void SetMountAgentBeforeBuild(Agent mount) => this.MountAgent = mount;

//    internal void SetMountInitialValues(TextObject name, string horseCreationKey)
//    {
//      this._name = name;
//      this.HorseCreationKey = horseCreationKey;
//    }

//    internal void SetInitialAgentScale(float initialScale)
//    {
//      MBAPI.IMBAgent.SetAgentScale(this.GetPtr(), initialScale);
//    }

//    internal void InitializeAgentRecord() => MBAPI.IMBAgent.InitializeAgentRecord(this.GetPtr());

//    internal void OnDelete()
//    {
//      this._isDeleted = true;
//      this.MissionPeer = (MissionPeer) null;
//    }

//    internal void OnFleeing()
//    {
//      this.RelieveFromCaptaincy();
//      if (this.Formation == null)
//        return;
//      this.Formation.Team.DetachmentManager.OnAgentRemoved(this);
//      this.Formation = (Formation) null;
//    }

//    internal void OnRemove()
//    {
//      this._isRemoved = true;
//      this._removalTime = this.Mission.CurrentTime;
//      this.Origin?.OnAgentRemoved(this.Health);
//      this.RelieveFromCaptaincy();
//      this.Team?.OnAgentRemoved(this);
//      if (this.Formation != null)
//      {
//        this.Formation.Team.DetachmentManager.OnAgentRemoved(this);
//        this.Formation = (Formation) null;
//      }
//      if ((this.HumanAIComponent != null && this.InteractingWithAnyGameObject() || this.IsUsingGameObject) && !GameNetwork.IsClientOrReplay && this.Mission != null && !this.Mission.MissionEnded)
//        this.StopUsingGameObjectMT(false, Agent.StopUsingGameObjectFlags.None);
//      foreach (AgentComponent component in (List<AgentComponent>) this._components)
//        component.OnAgentRemoved();
//    }

//    internal void InitializeComponents()
//    {
//      foreach (AgentComponent component in (List<AgentComponent>) this._components)
//        component.Initialize();
//    }

//    internal void Build(AgentBuildData agentBuildData)
//    {
//      this.BuildAux();
//      this.HasBeenBuilt = true;
//      this.Controller = this.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.IsHumanoid) ? agentBuildData.AgentController : AgentControllerType.AI;
//      this.Formation = !this.IsMount ? agentBuildData?.AgentFormation : (Formation) null;
//      MissionGameModels.Current?.AgentStatCalculateModel.InitializeMissionEquipment(this);
//      this.InitializeAgentProperties(this.SpawnEquipment, agentBuildData);
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
//      {
//        if (!networkPeer.IsMine && networkPeer.IsSynchronized)
//          this.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkPeer);
//      }
//    }

//    private void PreloadForRenderingAux() => MBAPI.IMBAgent.PreloadForRendering(this.GetPtr());

//    internal void Clear()
//    {
//      this.Mission = (Mission) null;
//      this._pointer = UIntPtr.Zero;
//      this._positionPointer = UIntPtr.Zero;
//      this._flagsPointer = UIntPtr.Zero;
//      this._indexPointer = UIntPtr.Zero;
//      this._statePointer = UIntPtr.Zero;
//      this._movementModePointer = UIntPtr.Zero;
//      this._controllerTypePointer = UIntPtr.Zero;
//      this._movementDirectionPointer = UIntPtr.Zero;
//      this._primaryWieldedItemIndexPointer = UIntPtr.Zero;
//      this._offHandWieldedItemIndexPointer = UIntPtr.Zero;
//      this._channel0CurrentActionPointer = UIntPtr.Zero;
//      this._channel1CurrentActionPointer = UIntPtr.Zero;
//      this._maximumForwardUnlimitedSpeed = UIntPtr.Zero;
//    }

//    public bool HasPathThroughNavigationFacesIDFromDirection(
//      int navigationFaceID_1,
//      int navigationFaceID_2,
//      int navigationFaceID_3,
//      Vec2 direction)
//    {
//      return MBAPI.IMBAgent.HasPathThroughNavigationFacesIDFromDirection(this.GetPtr(), navigationFaceID_1, navigationFaceID_2, navigationFaceID_3, ref direction);
//    }

//    public bool HasPathThroughNavigationFacesIDFromDirectionMT(
//      int navigationFaceID_1,
//      int navigationFaceID_2,
//      int navigationFaceID_3,
//      Vec2 direction)
//    {
//      lock (Agent._pathCheckObjectLock)
//        return MBAPI.IMBAgent.HasPathThroughNavigationFacesIDFromDirection(this.GetPtr(), navigationFaceID_1, navigationFaceID_2, navigationFaceID_3, ref direction);
//    }

//    private void AfterStoppedUsingMissionObject(
//      UsableMachine usableMachine,
//      UsableMissionObject usedObject,
//      UsableMissionObject movingToOrDefendingObject,
//      bool isSuccessful,
//      Agent.StopUsingGameObjectFlags flags)
//    {
//      if (this.IsAIControlled)
//      {
//        if (flags.HasAnyFlag<Agent.StopUsingGameObjectFlags>(Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject))
//          this.Formation?.AttachUnit(this);
//        if (flags.HasAnyFlag<Agent.StopUsingGameObjectFlags>(Agent.StopUsingGameObjectFlags.DefendAfterStoppingUsingGameObject))
//          this.AIDefendGameObjectEnable(usedObject ?? movingToOrDefendingObject, (IDetachment) usableMachine);
//      }
//      if (!(usedObject is StandingPoint standingPoint) || !standingPoint.AutoEquipWeaponsOnUseStopped || flags.HasAnyFlag<Agent.StopUsingGameObjectFlags>(Agent.StopUsingGameObjectFlags.DoNotWieldWeaponAfterStoppingUsingGameObject))
//        return;
//      bool flag1 = !isSuccessful;
//      bool flag2 = this._equipmentOnMainHandBeforeUsingObject != EquipmentIndex.None;
//      if (this._equipmentOnOffHandBeforeUsingObject != EquipmentIndex.None)
//        this.Mission.AddTickActionMT(Mission.MissionTickAction.TryToWieldWeaponInSlot, this, (int) this._equipmentOnOffHandBeforeUsingObject, !flag1 || flag2 ? 1 : 0);
//      if (!flag2)
//        return;
//      this.Mission.AddTickActionMT(Mission.MissionTickAction.TryToWieldWeaponInSlot, this, (int) this._equipmentOnMainHandBeforeUsingObject, flag1 ? 0 : 1);
//    }

//    private UIntPtr GetPtr() => this.Pointer;

//    private void SetWeaponHitPointsInSlot(EquipmentIndex equipmentIndex, short hitPoints)
//    {
//      MBAPI.IMBAgent.SetWeaponHitPointsInSlot(this.GetPtr(), (int) equipmentIndex, hitPoints);
//    }

//    private AgentMovementLockedState GetMovementLockedState()
//    {
//      return MBAPI.IMBAgent.GetMovementLockedState(this.GetPtr());
//    }

//    private void AttachWeaponToBoneAux(
//      ref MissionWeapon weapon,
//      GameEntity weaponEntity,
//      sbyte boneIndex,
//      ref MatrixFrame attachLocalFrame)
//    {
//      WeaponData weaponData = weapon.GetWeaponData(true);
//      MBAPI.IMBAgent.AttachWeaponToBone(this.GetPtr(), in weaponData, weapon.GetWeaponStatsData(), weapon.WeaponsCount, (object) weaponEntity != null ? weaponEntity.Pointer : UIntPtr.Zero, boneIndex, ref attachLocalFrame);
//      weaponData.DeinitializeManagedPointers();
//    }

//    private Agent GetRiderAgentAux() => this._cachedRiderAgent;

//    private void AttachWeaponToWeaponAux(
//      EquipmentIndex slotIndex,
//      ref MissionWeapon weapon,
//      GameEntity weaponEntity,
//      ref MatrixFrame attachLocalFrame)
//    {
//      WeaponData weaponData = weapon.GetWeaponData(true);
//      MBAPI.IMBAgent.AttachWeaponToWeaponInSlot(this.GetPtr(), in weaponData, weapon.GetWeaponStatsData(), weapon.WeaponsCount, (object) weaponEntity != null ? weaponEntity.Pointer : UIntPtr.Zero, (int) slotIndex, ref attachLocalFrame);
//      weaponData.DeinitializeManagedPointers();
//    }

//    private Agent GetMountAgentAux() => this._cachedMountAgent;

//    private void SetMountAgent(Agent mountAgent)
//    {
//      int mountAgentIndex = mountAgent == null ? -1 : mountAgent.Index;
//      MBAPI.IMBAgent.SetMountAgent(this.GetPtr(), mountAgentIndex);
//    }

//    private void RelieveFromCaptaincy()
//    {
//      if (this._canLeadFormationsRemotely && this.Team != null)
//      {
//        foreach (Formation formation in (List<Formation>) this.Team.FormationsIncludingSpecialAndEmpty)
//        {
//          if (formation.Captain == this)
//            formation.Captain = (Agent) null;
//        }
//      }
//      else
//      {
//        if (this.Formation == null || this.Formation.Captain != this)
//          return;
//        this.Formation.Captain = (Agent) null;
//      }
//    }

//    private void SetTeamInternal(MBTeam team) => MBAPI.IMBAgent.SetTeam(this.GetPtr(), team.Index);

//    private void WeaponEquipped(
//      EquipmentIndex equipmentSlot,
//      in WeaponData weaponData,
//      WeaponStatsData[] weaponStatsData,
//      in WeaponData ammoWeaponData,
//      WeaponStatsData[] ammoWeaponStatsData,
//      WeakGameEntity weaponEntity,
//      bool removeOldWeaponFromScene,
//      bool isWieldedOnSpawn)
//    {
//      MBAPI.IMBAgent.WeaponEquipped(this.GetPtr(), (int) equipmentSlot, in weaponData, weaponStatsData, weaponStatsData != null ? weaponStatsData.Length : 0, in ammoWeaponData, ammoWeaponStatsData, ammoWeaponStatsData != null ? ammoWeaponStatsData.Length : 0, weaponEntity.Pointer, removeOldWeaponFromScene, isWieldedOnSpawn);
//      this.CheckEquipmentForCapeClothSimulationStateChange();
//    }

//    private Agent GetRiderAgent() => MBAPI.IMBAgent.GetRiderAgent(this.GetPtr());

//    public void SetInitialFrame(
//      in Vec3 initialPosition,
//      in Vec2 initialDirection,
//      bool canSpawnOutsideOfMissionBoundary = false)
//    {
//      MBAPI.IMBAgent.SetInitialFrame(this.GetPtr(), in initialPosition, in initialDirection, canSpawnOutsideOfMissionBoundary);
//    }

//    private void UpdateDrivenProperties(float[] values)
//    {
//      MBAPI.IMBAgent.UpdateDrivenProperties(this.GetPtr(), values);
//    }

//    private void UpdateLastAttackAndHitTimes(Agent attackerAgent, bool isMissile)
//    {
//      float currentTime = this.Mission.CurrentTime;
//      if (isMissile)
//        this.LastRangedHitTime = currentTime;
//      else
//        this.LastMeleeHitTime = currentTime;
//      if (attackerAgent == this || attackerAgent == null)
//        return;
//      if (isMissile)
//        attackerAgent.LastRangedAttackTime = currentTime;
//      else
//        attackerAgent.LastMeleeAttackTime = currentTime;
//    }

//    private void SetNetworkPeer(NetworkCommunicator newPeer)
//    {
//      MBAPI.IMBAgent.SetNetworkPeer(this.GetPtr(), newPeer != null ? newPeer.Index : -1);
//    }

//    private void ClearTargetFrameAux() => MBAPI.IMBAgent.ClearTargetFrame(this.GetPtr());

//    [Conditional("_RGL_KEEP_ASSERTS")]
//    private void CheckUnmanagedAgentValid() => AgentHelper.GetAgentIndex(this._indexPointer);

//    private void BuildAux() => MBAPI.IMBAgent.Build(this.GetPtr(), this.Monster.EyeOffsetWrtHead);

//    public void ClearTargetZ() => MBAPI.IMBAgent.ClearTargetZ(this.GetPtr());

//    private float GetMissileRangeWithHeightDifference()
//    {
//      return this.IsMount || !this.IsRangedCached && !this.HasThrownCached || this.Formation?.CachedClosestEnemyFormation == null ? 0.0f : this.GetMissileRangeWithHeightDifferenceAux(this.Formation.CachedClosestEnemyFormation.Formation.CachedMedianPosition.GetNavMeshZ());
//    }

//    private void AddSkinMeshes(bool prepareImmediately)
//    {
//      this.AgentVisuals.AddSkinMeshes(new SkinGenerationParams((int) this.SpawnEquipment.GetSkinMeshesMask(), this.SpawnEquipment.GetUnderwearType(this.IsFemale && (double) this.BodyPropertiesValue.Age >= 14.0), (int) this.SpawnEquipment.BodyMeshType, (int) this.SpawnEquipment.HairCoverType, (int) this.SpawnEquipment.BeardCoverType, (int) this.SpawnEquipment.BodyDeformType, prepareImmediately, this.Character.FaceDirtAmount, this.IsFemale ? 1 : 0, this.Character.Race, false, false), this.BodyPropertiesValue, prepareImmediately, prepareImmediately);
//    }

//    private void HandleBlow(ref Blow b, in AttackCollisionData collisionData)
//    {
//      b.BaseMagnitude = MathF.Min(b.BaseMagnitude, 1000f);
//      b.DamagedPercentage = (float) b.InflictedDamage / this.HealthLimit;
//      Agent agentWithIndex = b.OwnerId != -1 ? this.Mission.FindAgentWithIndex(b.OwnerId) : (Agent) null;
//      if (!b.BlowFlag.HasAnyFlag<BlowFlags>(BlowFlags.NoSound))
//      {
//        bool isCriticalBlow = b.IsBlowCrit(this.Monster.HitPoints * 4);
//        bool isLowBlow = b.IsBlowLow(this.Monster.HitPoints);
//        bool isOwnerHumanoid = agentWithIndex == null || agentWithIndex.IsHuman;
//        bool isNonTipThrust = b.BlowFlag.HasAnyFlag<BlowFlags>(BlowFlags.NonTipThrust);
//        int hitSound = b.WeaponRecord.GetHitSound(isOwnerHumanoid, isCriticalBlow, isLowBlow, isNonTipThrust, b.AttackType, b.DamageType);
//        SoundEventParameter parameter = new SoundEventParameter("Armor Type", Agent.GetSoundParameterForArmorType(this.GetProtectorArmorMaterialOfBone(b.BoneIndex)));
//        this.Mission.MakeSound(hitSound, b.GlobalPosition, false, true, b.OwnerId, this.Index, ref parameter);
//        if (b.IsMissile && agentWithIndex != null)
//          this.Mission.MakeSoundOnlyOnRelatedPeer(CombatSoundContainer.SoundCodeMissionCombatPlayerhit, b.GlobalPosition, agentWithIndex.Index);
//        if (!collisionData.IsSneakAttack)
//          this.Mission.AddSoundAlarmFactorToAgents(agentWithIndex, in b.GlobalPosition, 15f);
//      }
//      if (b.InflictedDamage <= 0)
//        return;
//      this.UpdateLastAttackAndHitTimes(agentWithIndex, b.IsMissile);
//      float health = this.Health;
//      float damagedHp = (double) b.InflictedDamage > (double) health ? health : (float) b.InflictedDamage;
//      if (this.CurrentMortalityState == Agent.MortalityState.Immortal || this.Mission.DisableDying)
//        damagedHp = 0.0f;
//      float num1 = health - damagedHp;
//      if ((double) num1 < 0.0)
//        num1 = 0.0f;
//      this.Health = num1;
//      if (agentWithIndex != null && agentWithIndex != this && this.IsHuman)
//      {
//        if (agentWithIndex.IsMount && agentWithIndex.RiderAgent != null)
//          this._lastHitInfo.RegisterLastBlow(agentWithIndex.RiderAgent.Index, b.AttackType);
//        else if (agentWithIndex.IsHuman)
//          this._lastHitInfo.RegisterLastBlow(b.OwnerId, b.AttackType);
//      }
//      if (!this.Mission.DisableDying)
//      {
//        double num2 = (double) this.Mission.OnAgentHit(this, agentWithIndex, in b, in collisionData, false, damagedHp);
//      }
//      if ((double) this.Health < 1.0)
//      {
//        Agent.KillInfo overrideKillInfo = b.IsFallDamage ? Agent.KillInfo.Gravity : Agent.KillInfo.Invalid;
//        this.Die(b, overrideKillInfo);
//      }
//      this.HandleBlowAux(ref b);
//    }

//    private void HandleBlowAux(ref Blow b) => MBAPI.IMBAgent.HandleBlowAux(this.GetPtr(), ref b);

//    private ArmorComponent.ArmorMaterialTypes GetProtectorArmorMaterialOfBone(sbyte boneIndex)
//    {
//      if (boneIndex >= (sbyte) 0)
//      {
//        EquipmentIndex index = EquipmentIndex.None;
//        switch (this.AgentVisuals.GetBoneTypeData(boneIndex).BodyPartType)
//        {
//          case BoneBodyPartType.Head:
//          case BoneBodyPartType.Neck:
//            index = EquipmentIndex.NumAllWeaponSlots;
//            break;
//          case BoneBodyPartType.Chest:
//          case BoneBodyPartType.Abdomen:
//          case BoneBodyPartType.ShoulderLeft:
//          case BoneBodyPartType.ShoulderRight:
//            index = EquipmentIndex.Body;
//            break;
//          case BoneBodyPartType.ArmLeft:
//          case BoneBodyPartType.ArmRight:
//            index = EquipmentIndex.Gloves;
//            break;
//          case BoneBodyPartType.Legs:
//            index = EquipmentIndex.Leg;
//            break;
//        }
//        if (index != EquipmentIndex.None)
//        {
//          EquipmentElement equipmentElement = this.SpawnEquipment[index];
//          if (equipmentElement.Item != null)
//          {
//            equipmentElement = this.SpawnEquipment[index];
//            return equipmentElement.Item.ArmorComponent.MaterialType;
//          }
//        }
//      }
//      return ArmorComponent.ArmorMaterialTypes.None;
//    }

//    private void TickAsAI()
//    {
//      if (!this._cachedAndFormationValuesUpdateTimer.Check(this.Mission.CurrentTime) || this.Formation == null)
//        return;
//      this.UpdateCachedAndFormationValues(false, true);
//    }

//    private void SyncHealthToClients()
//    {
//      if (this.SyncHealthToAllClients && (!this.IsMount || this.RiderAgent != null))
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentHealth(this.Index, (int) this.Health));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
//      }
//      else
//      {
//        NetworkCommunicator networkCommunicator;
//        if (!this.IsMount)
//        {
//          MissionPeer missionPeer = this.MissionPeer;
//          networkCommunicator = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
//        }
//        else
//        {
//          Agent riderAgent = this.RiderAgent;
//          if (riderAgent == null)
//          {
//            networkCommunicator = (NetworkCommunicator) null;
//          }
//          else
//          {
//            MissionPeer missionPeer = riderAgent.MissionPeer;
//            networkCommunicator = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
//          }
//        }
//        NetworkCommunicator communicator = networkCommunicator;
//        if (communicator == null || communicator.IsServerPeer)
//          return;
//        GameNetwork.BeginModuleEventAsServer(communicator);
//        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentHealth(this.Index, (int) this.Health));
//        GameNetwork.EndModuleEventAsServer();
//      }
//    }

//    public static Agent.UsageDirection MovementFlagToDirection(Agent.MovementControlFlag flag)
//    {
//      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.AttackDown))
//        return Agent.UsageDirection.AttackDown;
//      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.AttackUp))
//        return Agent.UsageDirection.AttackUp;
//      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.AttackLeft))
//        return Agent.UsageDirection.AttackLeft;
//      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.AttackRight))
//        return Agent.UsageDirection.AttackRight;
//      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.DefendDown))
//        return Agent.UsageDirection.DefendDown;
//      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.DefendUp))
//        return Agent.UsageDirection.AttackEnd;
//      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.DefendLeft))
//        return Agent.UsageDirection.DefendLeft;
//      return flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.DefendRight) ? Agent.UsageDirection.DefendRight : Agent.UsageDirection.None;
//    }

//    public static Agent.UsageDirection GetActionDirection(int actionIndex)
//    {
//      return MBAPI.IMBAgent.GetActionDirection(actionIndex);
//    }

//    public static int GetMonsterUsageIndex(string monsterUsage)
//    {
//      return MBAPI.IMBAgent.GetMonsterUsageIndex(monsterUsage);
//    }

//    public static float GetSoundParameterForArmorType(
//      ArmorComponent.ArmorMaterialTypes armorMaterialType)
//    {
//      return (float) armorMaterialType * 0.1f;
//    }

//    public class Hitter
//    {
//      public const float AssistMinDamage = 35f;
//      public readonly MissionPeer HitterPeer;
//      public readonly bool IsFriendlyHit;

//      public float Damage { get; private set; }

//      public Hitter(MissionPeer peer, float damage, bool isFriendlyHit)
//      {
//        this.HitterPeer = peer;
//        this.Damage = damage;
//        this.IsFriendlyHit = isFriendlyHit;
//      }

//      public void IncreaseDamage(float amount) => this.Damage += amount;
//    }

//    public struct AgentLastHitInfo
//    {
//      private BasicMissionTimer _lastBlowTimer;

//      public int LastBlowOwnerId { get; private set; }

//      public AgentAttackType LastBlowAttackType { get; private set; }

//      public bool CanOverrideBlow
//      {
//        get => this.LastBlowOwnerId >= 0 && (double) this._lastBlowTimer.ElapsedTime <= 5.0;
//      }

//      public void Initialize()
//      {
//        this.LastBlowOwnerId = -1;
//        this.LastBlowAttackType = AgentAttackType.Standard;
//        this._lastBlowTimer = new BasicMissionTimer();
//      }

//      public void RegisterLastBlow(int ownerId, AgentAttackType attackType)
//      {
//        this._lastBlowTimer.Reset();
//        this.LastBlowOwnerId = ownerId;
//        this.LastBlowAttackType = attackType;
//      }
//    }

//    public struct AgentPropertiesModifiers
//    {
//      public bool resetAiWaitBeforeShootFactor;
//    }

//    public struct StackArray8Agent
//    {
//      private Agent _element0;
//      private Agent _element1;
//      private Agent _element2;
//      private Agent _element3;
//      private Agent _element4;
//      private Agent _element5;
//      private Agent _element6;
//      private Agent _element7;
//      public const int Length = 8;

//      public Agent this[int index]
//      {
//        get
//        {
//          switch (index)
//          {
//            case 0:
//              return this._element0;
//            case 1:
//              return this._element1;
//            case 2:
//              return this._element2;
//            case 3:
//              return this._element3;
//            case 4:
//              return this._element4;
//            case 5:
//              return this._element5;
//            case 6:
//              return this._element6;
//            case 7:
//              return this._element7;
//            default:
//              return (Agent) null;
//          }
//        }
//        set
//        {
//          switch (index)
//          {
//            case 0:
//              this._element0 = value;
//              break;
//            case 1:
//              this._element1 = value;
//              break;
//            case 2:
//              this._element2 = value;
//              break;
//            case 3:
//              this._element3 = value;
//              break;
//            case 4:
//              this._element4 = value;
//              break;
//            case 5:
//              this._element5 = value;
//              break;
//            case 6:
//              this._element6 = value;
//              break;
//            case 7:
//              this._element7 = value;
//              break;
//          }
//        }
//      }
//    }

//    public enum ActionStage
//    {
//      None = -1, // 0xFFFFFFFF
//      AttackReady = 0,
//      AttackQuickReady = 1,
//      AttackRelease = 2,
//      ReloadMidPhase = 3,
//      ReloadLastPhase = 4,
//      Defend = 5,
//      DefendParry = 6,
//      NumActionStages = 7,
//    }

//    [Flags]
//    public enum AIScriptedFrameFlags
//    {
//      None = 0,
//      GoToPosition = 1,
//      NoAttack = 2,
//      ConsiderRotation = 4,
//      NeverSlowDown = 8,
//      DoNotRun = 16, // 0x00000010
//      GoWithoutMount = 32, // 0x00000020
//      RangerCanMoveForClearTarget = 128, // 0x00000080
//      InConversation = 256, // 0x00000100
//      Crouch = 512, // 0x00000200
//      Drag = 1024, // 0x00000400
//    }

//    [Flags]
//    public enum AISpecialCombatModeFlags
//    {
//      None = 0,
//      AttackEntity = 1,
//      SurroundAttackEntity = 2,
//      IgnoreAmmoLimitForRangeCalculation = 1024, // 0x00000400
//    }

//    [Flags]
//    [EngineStruct("Ai_state_flag", true, "aisf", false)]
//    public enum AIStateFlag : uint
//    {
//      None = 0,
//      Cautious = 1,
//      PatrollingCautious = 2,
//      Alarmed = PatrollingCautious | Cautious, // 0x00000003
//      Paused = 8,
//      UseObjectMoving = 16, // 0x00000010
//      UseObjectUsing = 32, // 0x00000020
//      UseObjectWaiting = 64, // 0x00000040
//      ColumnwiseFollow = 256, // 0x00000100
//      AlarmStateMask = Alarmed, // 0x00000003
//    }

//    public enum WatchState
//    {
//      Patrolling,
//      Cautious,
//      Alarmed,
//    }

//    public enum MortalityState
//    {
//      Mortal,
//      Invulnerable,
//      Immortal,
//    }

//    public enum CreationType
//    {
//      Invalid,
//      FromRoster,
//      FromHorseObj,
//      FromCharacterObj,
//    }

//    [Flags]
//    [EngineStruct("Agent_event_control_flags", true, "agce", false)]
//    public enum EventControlFlag : uint
//    {
//      None = 0,
//      Dismount = 1,
//      Mount = 2,
//      Rear = 4,
//      Jump = 8,
//      Wield0 = 16, // 0x00000010
//      Wield1 = 32, // 0x00000020
//      Wield2 = 64, // 0x00000040
//      Wield3 = 128, // 0x00000080
//      Sheath0 = 256, // 0x00000100
//      Sheath1 = 512, // 0x00000200
//      ToggleAlternativeWeapon = 1024, // 0x00000400
//      Walk = 2048, // 0x00000800
//      Run = 4096, // 0x00001000
//      Crouch = 8192, // 0x00002000
//      Stand = 16384, // 0x00004000
//      Kick = 32768, // 0x00008000
//      DoubleTapToDirectionUp = 65536, // 0x00010000
//      DoubleTapToDirectionDown = 131072, // 0x00020000
//      DoubleTapToDirectionLeft = DoubleTapToDirectionDown | DoubleTapToDirectionUp, // 0x00030000
//      DoubleTapToDirectionRight = 262144, // 0x00040000
//      DoubleTapToDirectionMask = DoubleTapToDirectionRight | DoubleTapToDirectionLeft, // 0x00070000
//    }

//    public enum FacialAnimChannel
//    {
//      High,
//      Mid,
//      Low,
//      num_facial_anim_channels,
//    }

//    [EngineStruct("Action_code_type", true, "actt", false)]
//    public enum ActionCodeType
//    {
//      Other = 0,
//      CombatAllBegin = 1,
//      DefendAllBegin = 1,
//      DefendFist = 1,
//      DefendShield = 2,
//      DefendForward2h = 3,
//      DefendUp2h = 4,
//      DefendRight2h = 5,
//      DefendLeft2h = 6,
//      DefendForward1h = 7,
//      DefendUp1h = 8,
//      DefendRight1h = 9,
//      DefendLeft1h = 10, // 0x0000000A
//      DefendForwardStaff = 11, // 0x0000000B
//      DefendUpStaff = 12, // 0x0000000C
//      DefendRightStaff = 13, // 0x0000000D
//      DefendLeftStaff = 14, // 0x0000000E
//      AttackMeleeAndRangedAllBegin = 15, // 0x0000000F
//      DefendAllEnd = 15, // 0x0000000F
//      ReadyRanged = 15, // 0x0000000F
//      ReleaseRanged = 16, // 0x00000010
//      ReleaseThrowing = 17, // 0x00000011
//      Reload = 18, // 0x00000012
//      AttackMeleeAllBegin = 19, // 0x00000013
//      ReadyMelee = 19, // 0x00000013
//      ReleaseMelee = 20, // 0x00000014
//      ParriedMelee = 21, // 0x00000015
//      BlockedMelee = 22, // 0x00000016
//      AttackMeleeAllEnd = 23, // 0x00000017
//      AttackMeleeAndRangedAllEnd = 23, // 0x00000017
//      CombatAllEnd = 23, // 0x00000017
//      Fall = 23, // 0x00000017
//      JumpAllBegin = 24, // 0x00000018
//      JumpStart = 24, // 0x00000018
//      FallAllBegin = 25, // 0x00000019
//      Jump = 25, // 0x00000019
//      JumpEnd = 26, // 0x0000001A
//      JumpEndHard = 27, // 0x0000001B
//      AlternativeAttackAllBegin = 28, // 0x0000001C
//      FallAllEnd = 28, // 0x0000001C
//      JumpAllEnd = 28, // 0x0000001C
//      Kick = 28, // 0x0000001C
//      KickAllBegin = 28, // 0x0000001C
//      KickContinue = 29, // 0x0000001D
//      KickHit = 30, // 0x0000001E
//      KickAllEnd = 31, // 0x0000001F
//      WeaponBash = 31, // 0x0000001F
//      AlternativeAttackAllEnd = 32, // 0x00000020
//      PassiveUsage = 32, // 0x00000020
//      EquipUnequip = 33, // 0x00000021
//      SwitchAlternative = 34, // 0x00000022
//      Idle = 35, // 0x00000023
//      Guard = 36, // 0x00000024
//      Mount = 37, // 0x00000025
//      Dismount = 38, // 0x00000026
//      Dash = 39, // 0x00000027
//      MountQuickStop = 40, // 0x00000028
//      HitObject = 41, // 0x00000029
//      Sit = 42, // 0x0000002A
//      SitOnTheFloor = 43, // 0x0000002B
//      SitOnAThrone = 44, // 0x0000002C
//      LadderRaise = 45, // 0x0000002D
//      LadderRaiseEnd = 46, // 0x0000002E
//      Rear = 47, // 0x0000002F
//      StrikeBegin = 48, // 0x00000030
//      StrikeLight = 48, // 0x00000030
//      StrikeMedium = 49, // 0x00000031
//      StrikeHeavy = 50, // 0x00000032
//      StrikeKnockBack = 51, // 0x00000033
//      MountStrike = 52, // 0x00000034
//      StrikeEnd = 52, // 0x00000034
//      Count = 53, // 0x00000035
//    }

//    [EngineStruct("Agent_guard_mode", true, "guard_mode", false)]
//    public enum GuardMode
//    {
//      MarkForDeletion = -2, // 0xFFFFFFFE
//      None = -1, // 0xFFFFFFFF
//      Up = 0,
//      Down = 1,
//      Left = 2,
//      Right = 3,
//    }

//    public enum HandIndex
//    {
//      MainHand,
//      OffHand,
//    }

//    [EngineStruct("rglInt8", false, null)]
//    public enum KillInfo : sbyte
//    {
//      Invalid = -1, // 0xFF
//      Headshot = 0,
//      CouchedLance = 1,
//      Punch = 2,
//      MountHit = 3,
//      Bow = 4,
//      Crossbow = 5,
//      ThrowingAxe = 6,
//      ThrowingKnife = 7,
//      Javelin = 8,
//      Stone = 9,
//      Pistol = 10, // 0x0A
//      Musket = 11, // 0x0B
//      OneHandedSword = 12, // 0x0C
//      TwoHandedSword = 13, // 0x0D
//      OneHandedAxe = 14, // 0x0E
//      TwoHandedAxe = 15, // 0x0F
//      Mace = 16, // 0x10
//      Spear = 17, // 0x11
//      Morningstar = 18, // 0x12
//      Maul = 19, // 0x13
//      Backstabbed = 20, // 0x14
//      Gravity = 21, // 0x15
//      ShieldBash = 22, // 0x16
//      WeaponBash = 23, // 0x17
//      Kick = 24, // 0x18
//      TeamSwitch = 25, // 0x19
//    }

//    public enum MovementBehaviorType
//    {
//      Engaged,
//      Idle,
//      Flee,
//    }

//    [Flags]
//    [EngineStruct("Agent_movement_control_flags", true, "agcm", false)]
//    public enum MovementControlFlag : uint
//    {
//      None = 0,
//      Forward = 1,
//      Backward = 2,
//      StrafeRight = 4,
//      StrafeLeft = 8,
//      TurnRight = 16, // 0x00000010
//      TurnLeft = 32, // 0x00000020
//      AttackLeft = 64, // 0x00000040
//      AttackRight = 128, // 0x00000080
//      AttackUp = 256, // 0x00000100
//      AttackDown = 512, // 0x00000200
//      DefendLeft = 1024, // 0x00000400
//      DefendRight = 2048, // 0x00000800
//      DefendUp = 4096, // 0x00001000
//      DefendDown = 8192, // 0x00002000
//      DefendAuto = 16384, // 0x00004000
//      DefendBlock = 32768, // 0x00008000
//      Action = 65536, // 0x00010000
//      AttackMask = AttackDown | AttackUp | AttackRight | AttackLeft, // 0x000003C0
//      DefendMask = DefendAuto | DefendDown | DefendUp | DefendRight | DefendLeft, // 0x00007C00
//      DefendDirMask = DefendDown | DefendUp | DefendRight | DefendLeft, // 0x00003C00
//      MoveMask = TurnLeft | TurnRight | StrafeLeft | StrafeRight | Backward | Forward, // 0x0000003F
//      MaxValue = MoveMask | DefendDirMask | AttackMask | Action | DefendBlock | DefendAuto, // 0x0001FFFF
//    }

//    public enum UnderAttackType
//    {
//      NotUnderAttack,
//      UnderMeleeAttack,
//      UnderRangedAttack,
//    }

//    [EngineStruct("Usage_direction", true, "ud", false)]
//    public enum UsageDirection
//    {
//      None = -1, // 0xFFFFFFFF
//      AttackBegin = 0,
//      AttackUp = 0,
//      AttackDown = 1,
//      AttackLeft = 2,
//      AttackRight = 3,
//      AttackEnd = 4,
//      DefendBegin = 4,
//      DefendUp = 4,
//      DefendDown = 5,
//      DefendLeft = 6,
//      DefendRight = 7,
//      DefendAny = 8,
//      AttackAny = 9,
//      DefendEnd = 9,
//    }

//    [EngineStruct("Weapon_wield_action_type", false, null)]
//    public enum WeaponWieldActionType
//    {
//      WithAnimation,
//      Instant,
//      InstantAfterPickUp,
//      WithAnimationUninterruptible,
//    }

//    [Flags]
//    public enum StopUsingGameObjectFlags : byte
//    {
//      None = 0,
//      AutoAttachAfterStoppingUsingGameObject = 1,
//      DoNotWieldWeaponAfterStoppingUsingGameObject = 2,
//      DefendAfterStoppingUsingGameObject = 4,
//    }

//    public delegate void OnAgentHealthChangedDelegate(
//      Agent agent,
//      float oldHealth,
//      float newHealth);

//    public delegate void OnMountHealthChangedDelegate(
//      Agent agent,
//      Agent mount,
//      float oldHealth,
//      float newHealth);

//    public delegate void OnMainAgentWieldedItemChangeDelegate();
//  }
//}
