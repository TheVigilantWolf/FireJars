//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.Mission
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using JetBrains.Annotations;
//using NetworkMessages.FromServer;
//using System;
//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using TaleWorlds.Core;
//using TaleWorlds.DotNet;
//using TaleWorlds.Engine;
//using TaleWorlds.InputSystem;
//using TaleWorlds.Library;
//using TaleWorlds.ModuleManager;
//using TaleWorlds.MountAndBlade.ComponentInterfaces;
//using TaleWorlds.MountAndBlade.Missions;
//using TaleWorlds.MountAndBlade.Network;
//using TaleWorlds.MountAndBlade.Network.Messages;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public sealed class Mission : DotNetObject, IMission
//  {
//    public const int MaxRuntimeMissionObjects = 8191;
//    private int _lastSceneMissionObjectIdCount;
//    private int _lastRuntimeMissionObjectIdCount;
//    private bool _isMainAgentObjectInteractionEnabled = true;
//    private List<Mission.TimeSpeedRequest> _timeSpeedRequests = new List<Mission.TimeSpeedRequest>();
//    private bool _isMainAgentItemInteractionEnabled = true;
//    private readonly MBList<MissionObject> _activeMissionObjects;
//    private readonly MBList<MissionObject> _missionObjects;
//    private readonly List<SpawnedItemEntity> _spawnedItemEntitiesCreatedAtRuntime;
//    private readonly MBList<Mission.DynamicallyCreatedEntity> _addedEntitiesInfo;
//    private readonly Stack<(int, float)> _emptyRuntimeMissionObjectIds;
//    private static bool _isCameraFirstPerson = false;
//    private MissionMode _missionMode;
//    private float _cachedMissionTime;
//    private static readonly object GetNearbyAgentsAuxLock = new object();
//    public const int MaxNavMeshId = 1000000;
//    private const float NavigationMeshHeightLimit = 1.5f;
//    private const float SpeedBonusFactorForSwing = 0.7f;
//    private const float SpeedBonusFactorForThrust = 0.5f;
//    private const float _exitTimeInSeconds = 0.6f;
//    private const int MaxNavMeshPerDynamicObject = 10;
//    private bool? _doesMissionAllowChargeDamageOnFriendly;
//    private bool _missionEnded;
//    private Dictionary<int, Mission.Missile> _missilesDictionary;
//    private MBList<Mission.Missile> _missilesList;
//    private readonly List<Mission.DynamicEntityInfo> _dynamicEntities = new List<Mission.DynamicEntityInfo>();
//    public bool DisableDying;
//    public bool ForceNoFriendlyFire;
//    public const int MaxDamage = 2000;
//    public bool IsFriendlyMission = true;
//    public BasicCultureObject MusicCulture;
//    private int _nextDynamicNavMeshIdStart = 1000050;
//    private MissionState _missionState;
//    private List<IMissionListener> _listeners = new List<IMissionListener>();
//    private BasicMissionTimer _leaveMissionTimer;
//    private MBReadOnlyList<MBSubModuleBase> _cachedSubModuleList;
//    private readonly MBList<KeyValuePair<Agent, MissionTime>> _mountsWithoutRiders;
//    private List<MissionBehavior> _otherMissionBehaviors;
//    private readonly object _lockHelper = new object();
//    private AgentList _activeAgents;
//    private IMissionDeploymentPlan _deploymentPlan;
//    public bool IsOrderMenuOpen;
//    public bool IsTransferMenuOpen;
//    public bool IsInPhotoMode;
//    private Agent _mainAgent;
//    private Action _onLoadingEndedAction;
//    private TaleWorlds.Core.Timer _inMissionLoadingScreenTimer;
//    public bool AllowAiTicking = true;
//    private int _agentCreationIndex;
//    private readonly MBList<FleePosition>[] _fleePositions = new MBList<FleePosition>[3];
//    private bool _doesMissionRequireCivilianEquipment;
//    public IAgentVisualCreator AgentVisualCreator;
//    private readonly int[] _initialAgentCountPerSide = new int[2];
//    private readonly int[] _removedAgentCountPerSide = new int[2];
//    private ConcurrentQueue<CombatLogData> _combatLogsCreated = new ConcurrentQueue<CombatLogData>();
//    private AgentList _allAgents;
//    private MBList<Mission.CorpseAgentInfo> _corpseAgentInfos;
//    private MBList<(Mission.MissionTickAction Action, Agent Agent, int Param1, int Param2)> _tickActions = new MBList<(Mission.MissionTickAction, Agent, int, int)>();
//    private readonly object _tickActionsLock = new object();
//    private List<SiegeWeapon> _attackerWeaponsForFriendlyFirePreventing = new List<SiegeWeapon>();
//    private bool _isFastForward;
//    private float _missionEndTime;
//    public float MissionCloseTimeAfterFinish = 30f;
//    private static Mission _current = (Mission) null;
//    public float NextCheckTimeEndMission = 10f;
//    public int NumOfFormationsSpawnedTeamOne;
//    private SoundEvent _ambientSoundEvent;
//    private readonly BattleSpawnPathSelector _battleSpawnPathSelector;
//    private int _agentCount;
//    public int NumOfFormationsSpawnedTeamTwo;
//    private bool _canPlayerTakeControlOfAnotherAgentWhenDead;
//    private bool tickCompleted = true;

//    internal UIntPtr Pointer { get; private set; }

//    public bool IsFinalized => this.Pointer == UIntPtr.Zero;

//    public static Mission Current
//    {
//      get
//      {
//        Mission current = Mission._current;
//        return Mission._current;
//      }
//      private set
//      {
//        if (value == null)
//        {
//          Mission current = Mission._current;
//        }
//        Mission._current = value;
//      }
//    }

//    private MissionInitializerRecord InitializerRecord { get; set; }

//    public string SceneName => this.InitializerRecord.SceneName;

//    public string SceneLevels => this.InitializerRecord.SceneLevels;

//    public float DamageToPlayerMultiplier => BannerlordConfig.GetDamageToPlayerMultiplier();

//    public float DamageToFriendsMultiplier => this.InitializerRecord.DamageToFriendsMultiplier;

//    public float DamageFromPlayerToFriendsMultiplier
//    {
//      get => this.InitializerRecord.DamageFromPlayerToFriendsMultiplier;
//    }

//    public bool HasValidTerrainType => this.InitializerRecord.TerrainType >= 0;

//    public TerrainType TerrainType
//    {
//      get
//      {
//        return !this.HasValidTerrainType ? TerrainType.Water : (TerrainType) this.InitializerRecord.TerrainType;
//      }
//    }

//    public Scene Scene { get; private set; }

//    public Vec3 CustomCameraTargetLocalOffset { get; private set; }

//    public Vec3 CustomCameraLocalOffset { get; private set; }

//    public Vec3 CustomCameraLocalOffset2 { get; private set; }

//    public Vec3 CustomCameraGlobalOffset { get; private set; }

//    public Vec3 CustomCameraLocalRotationalOffset { get; private set; }

//    public bool CustomCameraIgnoreCollision { get; private set; }

//    public float CustomCameraFovMultiplier { get; private set; } = 1f;

//    public float CustomCameraFixedDistance { get; private set; } = float.MinValue;

//    public float ListenerAndAttenuationPosBlendFactor { get; private set; }

//    public GameEntity IgnoredEntityForCamera { get; private set; }

//    public IEnumerable<WeakGameEntity> GetActiveEntitiesWithScriptComponentOfType<T>()
//    {
//      return this._activeMissionObjects.Where<MissionObject>((Func<MissionObject, bool>) (amo => amo is T)).Select<MissionObject, WeakGameEntity>((Func<MissionObject, WeakGameEntity>) (amo => amo.GameEntity));
//    }

//    public void AddActiveMissionObject(MissionObject missionObject)
//    {
//      this._missionObjects.Add(missionObject);
//      this._activeMissionObjects.Add(missionObject);
//    }

//    public void ActivateMissionObject(MissionObject missionObject)
//    {
//      this._activeMissionObjects.Add(missionObject);
//    }

//    public void DeactivateMissionObject(MissionObject missionObject)
//    {
//      this._activeMissionObjects.Remove(missionObject);
//    }

//    public MBReadOnlyList<MissionObject> ActiveMissionObjects
//    {
//      get => (MBReadOnlyList<MissionObject>) this._activeMissionObjects;
//    }

//    public MBReadOnlyList<MissionObject> MissionObjects
//    {
//      get => (MBReadOnlyList<MissionObject>) this._missionObjects;
//    }

//    public MBReadOnlyList<Mission.DynamicallyCreatedEntity> AddedEntitiesInfo
//    {
//      get => (MBReadOnlyList<Mission.DynamicallyCreatedEntity>) this._addedEntitiesInfo;
//    }

//    public Mission.MBBoundaryCollection Boundaries { get; private set; }

//    public bool IsMainAgentObjectInteractionEnabled
//    {
//      get
//      {
//        switch (this._missionMode)
//        {
//          case MissionMode.Conversation:
//          case MissionMode.Barter:
//          case MissionMode.Deployment:
//          case MissionMode.Replay:
//          case MissionMode.CutScene:
//            return false;
//          default:
//            return (this.IsNavalBattle || !this.MissionEnded) && this._isMainAgentObjectInteractionEnabled;
//        }
//      }
//      set => this._isMainAgentObjectInteractionEnabled = value;
//    }

//    public bool IsMainAgentItemInteractionEnabled
//    {
//      get
//      {
//        switch (this._missionMode)
//        {
//          case MissionMode.Conversation:
//          case MissionMode.Barter:
//          case MissionMode.Deployment:
//          case MissionMode.Replay:
//          case MissionMode.CutScene:
//            return false;
//          default:
//            return this._isMainAgentItemInteractionEnabled;
//        }
//      }
//      set => this._isMainAgentItemInteractionEnabled = value;
//    }

//    public bool IsTeleportingAgents { get; set; }

//    public bool ForceTickOccasionally { get; set; }

//    private void FinalizeMission()
//    {
//      TeamAISiegeComponent.OnMissionFinalize();
//      MBAPI.IMBMission.FinalizeMission(this.Pointer);
//      this.Pointer = UIntPtr.Zero;
//    }

//    public Mission.MissionCombatType CombatType
//    {
//      get => (Mission.MissionCombatType) MBAPI.IMBMission.GetCombatType(this.Pointer);
//      set => MBAPI.IMBMission.SetCombatType(this.Pointer, (int) value);
//    }

//    public void SetMissionCombatType(Mission.MissionCombatType missionCombatType)
//    {
//      MBAPI.IMBMission.SetCombatType(this.Pointer, (int) missionCombatType);
//    }

//    public MissionMode Mode => this._missionMode;

//    public void ConversationCharacterChanged()
//    {
//      foreach (IMissionListener listener in this._listeners)
//        listener.OnConversationCharacterChanged();
//    }

//    public void SetMissionMode(MissionMode newMode, bool atStart)
//    {
//      if (this._missionMode == newMode)
//        return;
//      MissionMode missionMode = this._missionMode;
//      this._missionMode = newMode;
//      if (this.CurrentState == Mission.State.Over)
//        return;
//      for (int index = 0; index < this.MissionBehaviors.Count; ++index)
//        this.MissionBehaviors[index].OnMissionModeChange(missionMode, atStart);
//      foreach (IMissionListener listener in this._listeners)
//        listener.OnMissionModeChange(missionMode, atStart);
//    }

//    private Mission.AgentCreationResult CreateAgentInternal(
//      AgentFlag agentFlags,
//      int forcedAgentIndex,
//      bool isFemale,
//      ref AgentSpawnData spawnData,
//      ref AgentCapsuleData capsuleData,
//      ref AnimationSystemData animationSystemData,
//      int instanceNo)
//    {
//      return MBAPI.IMBMission.CreateAgent(this.Pointer, (ulong) agentFlags, forcedAgentIndex, isFemale, ref spawnData, ref capsuleData.BodyCap, ref capsuleData.CrouchedBodyCap, ref animationSystemData, instanceNo);
//    }

//    public float CurrentTime => this._cachedMissionTime;

//    public bool PauseAITick
//    {
//      get => MBAPI.IMBMission.GetPauseAITick(this.Pointer);
//      set => MBAPI.IMBMission.SetPauseAITick(this.Pointer, value);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void UpdateMissionTimeCache(float curTime) => this._cachedMissionTime = curTime;

//    public float GetAverageFps() => MBAPI.IMBMission.GetAverageFps(this.Pointer);

//    public bool GetFallAvoidSystemActive()
//    {
//      return MBAPI.IMBMission.GetFallAvoidSystemActive(this.Pointer);
//    }

//    public void SetFallAvoidSystemActive(bool fallAvoidActive)
//    {
//      MBAPI.IMBMission.SetFallAvoidSystemActive(this.Pointer, fallAvoidActive);
//    }

//    public bool IsPositionInsideBoundaries(Vec2 position)
//    {
//      return MBAPI.IMBMission.IsPositionInsideBoundaries(this.Pointer, position);
//    }

//    public bool IsPositionInsideHardBoundaries(Vec2 position)
//    {
//      return MBAPI.IMBMission.IsPositionInsideHardBoundaries(this.Pointer, position);
//    }

//    public bool IsPositionInsideAnyBlockerNavMeshFace2D(Vec2 position)
//    {
//      return MBAPI.IMBMission.IsPositionInsideAnyBlockerNavMeshFace2D(this.Pointer, position);
//    }

//    public bool IsPositionOnAnyBlockerNavMeshFace(Vec3 position)
//    {
//      return MBAPI.IMBMission.IsPositionOnAnyBlockerNavMeshFace(this.Pointer, position);
//    }

//    private bool IsFormationUnitPositionAvailableAuxMT(
//      ref WorldPosition formationPosition,
//      ref WorldPosition unitPosition,
//      ref WorldPosition nearestAvailableUnitPosition,
//      float manhattanDistance)
//    {
//      using (new TWSharedMutexReadLock(Scene.PhysicsAndRayCastLock))
//        return MBAPI.IMBMission.IsFormationUnitPositionAvailable(this.Pointer, ref formationPosition, ref unitPosition, ref nearestAvailableUnitPosition, manhattanDistance);
//    }

//    public Agent RayCastForClosestAgent(
//      Vec3 sourcePoint,
//      Vec3 targetPoint,
//      int excludedAgentIndex,
//      float rayThickness,
//      out float collisionDistance)
//    {
//      return MBAPI.IMBMission.RayCastForClosestAgent(this.Pointer, sourcePoint, targetPoint, excludedAgentIndex, rayThickness, out collisionDistance);
//    }

//    public Agent RayCastForClosestAgentsLimbs(
//      Vec3 sourcePoint,
//      Vec3 targetPoint,
//      int excludedAgentIndex,
//      float rayThickness,
//      out float collisionDistance,
//      out sbyte boneIndex)
//    {
//      return MBAPI.IMBMission.RayCastForClosestAgentsLimbs(this.Pointer, sourcePoint, targetPoint, excludedAgentIndex, rayThickness, out collisionDistance, out boneIndex);
//    }

//    public bool RayCastForGivenAgentsLimbs(
//      Vec3 sourcePoint,
//      Vec3 rayFinishPoint,
//      int givenAgentIndex,
//      float rayThickness,
//      out float collisionDistance,
//      out sbyte boneIndex)
//    {
//      return MBAPI.IMBMission.RayCastForGivenAgentsLimbs(this.Pointer, sourcePoint, rayFinishPoint, givenAgentIndex, rayThickness, out collisionDistance, out boneIndex);
//    }

//    internal AgentProximityMap.ProximityMapSearchStructInternal ProximityMapBeginSearch(
//      Vec2 searchPos,
//      float searchRadius)
//    {
//      return MBAPI.IMBMission.ProximityMapBeginSearch(this.Pointer, searchPos, searchRadius);
//    }

//    internal float ProximityMapMaxSearchRadius()
//    {
//      return MBAPI.IMBMission.ProximityMapMaxSearchRadius(this.Pointer);
//    }

//    public float GetBiggestAgentCollisionPadding()
//    {
//      return MBAPI.IMBMission.GetBiggestAgentCollisionPadding(this.Pointer);
//    }

//    public void SetMissionCorpseFadeOutTimeInSeconds(float corpseFadeOutTimeInSeconds)
//    {
//      MBAPI.IMBMission.SetMissionCorpseFadeOutTimeInSeconds(this.Pointer, corpseFadeOutTimeInSeconds);
//    }

//    public void SetOverrideCorpseCount(int overrideCorpseCount)
//    {
//      MBAPI.IMBMission.SetOverrideCorpseCount(this.Pointer, overrideCorpseCount);
//    }

//    public void SetReportStuckAgentsMode(bool value)
//    {
//      MBAPI.IMBMission.SetReportStuckAgentsMode(this.Pointer, value);
//    }

//    internal void BatchFormationUnitPositions(
//      MBArrayList<Vec2i> orderedPositionIndices,
//      MBArrayList<Vec2> orderedLocalPositions,
//      MBList2D<int> availabilityTable,
//      MBList2D<WorldPosition> globalPositionTable,
//      WorldPosition orderPosition,
//      Vec2 direction,
//      int fileCount,
//      int rankCount,
//      bool fastCheckWithSameFaceGroupIdDigit)
//    {
//      MBAPI.IMBMission.BatchFormationUnitPositions(this.Pointer, orderedPositionIndices.RawArray, orderedLocalPositions.RawArray, availabilityTable.RawArray, globalPositionTable.RawArray, orderPosition, direction, fileCount, rankCount, fastCheckWithSameFaceGroupIdDigit);
//    }

//    internal void ProximityMapFindNext(
//      ref AgentProximityMap.ProximityMapSearchStructInternal searchStruct)
//    {
//      MBAPI.IMBMission.ProximityMapFindNext(this.Pointer, ref searchStruct);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    public void ResetMission()
//    {
//      foreach (IMissionListener missionListener in this._listeners.ToArray())
//        missionListener.OnResetMission();
//      foreach (Agent activeAgent in (List<Agent>) this._activeAgents)
//        activeAgent.OnRemove();
//      foreach (Agent allAgent in (List<Agent>) this._allAgents)
//        allAgent.OnDelete();
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnClearScene();
//      this.NumOfFormationsSpawnedTeamOne = 0;
//      this.NumOfFormationsSpawnedTeamTwo = 0;
//      foreach (Team team in (List<Team>) this.Teams)
//        team.Reset();
//      MBAPI.IMBMission.ClearScene(this.Pointer);
//      this._activeAgents.Clear();
//      this._allAgents.Clear();
//      this._mountsWithoutRiders.Clear();
//      this.MainAgent = (Agent) null;
//      this.ClearMissiles();
//      this._missilesList.Clear();
//      this._missilesDictionary.Clear();
//      this._agentCount = 0;
//      for (int index = 0; index < 2; ++index)
//      {
//        this._initialAgentCountPerSide[index] = 0;
//        this._removedAgentCountPerSide[index] = 0;
//      }
//      this.ResetMissionObjects();
//      this.RemoveSpawnedMissionObjects();
//      this._activeMissionObjects.Clear();
//      this._activeMissionObjects.AddRange((IEnumerable<MissionObject>) this.MissionObjects);
//      this._tickActions.Clear();
//      this.Scene.ClearDecals();
//      PropertyChangedEventHandler onMissionReset = this.OnMissionReset;
//      if (onMissionReset == null)
//        return;
//      onMissionReset((object) this, (PropertyChangedEventArgs) null);
//    }

//    public event PropertyChangedEventHandler OnMissionReset;

//    public void Initialize()
//    {
//      Mission.Current = this;
//      this.CurrentState = Mission.State.Initializing;
//      this._deploymentPlan = (IMissionDeploymentPlan) this.GetMissionBehavior<MissionDeploymentPlanningLogic>();
//      if (this._deploymentPlan == null)
//        this._deploymentPlan = (IMissionDeploymentPlan) new DefaultMissionDeploymentPlan(this);
//      MissionInitializerRecord initializerRecord = this.InitializerRecord;
//      MBAPI.IMBMission.InitializeMission(this.Pointer, ref initializerRecord);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void OnSceneCreated(Scene scene) => this.Scene = scene;

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void TickAgentsAndTeams(float dt, bool tickPaused)
//    {
//      this.TickAgentsAndTeamsImp(dt, tickPaused);
//    }

//    public void TickAgentsAndTeamsAsync(float dt)
//    {
//      MBAPI.IMBMission.TickAgentsAndTeamsAsync(this.Pointer, dt);
//    }

//    internal void Tick(float dt) => MBAPI.IMBMission.Tick(this.Pointer, dt);

//    internal void IdleTick(float dt) => MBAPI.IMBMission.IdleTick(this.Pointer, dt);

//    public void MakeSound(
//      int soundIndex,
//      Vec3 position,
//      bool soundCanBePredicted,
//      bool isReliable,
//      int relatedAgent1,
//      int relatedAgent2)
//    {
//      MBAPI.IMBMission.MakeSound(this.Pointer, soundIndex, position, soundCanBePredicted, isReliable, relatedAgent1, relatedAgent2);
//    }

//    public void MakeSound(
//      int soundIndex,
//      Vec3 position,
//      bool soundCanBePredicted,
//      bool isReliable,
//      int relatedAgent1,
//      int relatedAgent2,
//      ref SoundEventParameter parameter)
//    {
//      MBAPI.IMBMission.MakeSoundWithParameter(this.Pointer, soundIndex, position, soundCanBePredicted, isReliable, relatedAgent1, relatedAgent2, parameter);
//    }

//    public void MakeSoundOnlyOnRelatedPeer(int soundIndex, Vec3 position, int relatedAgent)
//    {
//      MBAPI.IMBMission.MakeSoundOnlyOnRelatedPeer(this.Pointer, soundIndex, position, relatedAgent);
//    }

//    public void AddDynamicallySpawnedMissionObjectInfo(Mission.DynamicallyCreatedEntity entityInfo)
//    {
//      this._addedEntitiesInfo.Add(entityInfo);
//    }

//    private void RemoveDynamicallySpawnedMissionObjectInfo(MissionObjectId id)
//    {
//      Mission.DynamicallyCreatedEntity dynamicallyCreatedEntity = this._addedEntitiesInfo.FirstOrDefault<Mission.DynamicallyCreatedEntity>((Func<Mission.DynamicallyCreatedEntity, bool>) (x => x.ObjectId == id));
//      if (dynamicallyCreatedEntity == null)
//        return;
//      this._addedEntitiesInfo.Remove(dynamicallyCreatedEntity);
//    }

//    private int AddMissileAux(
//      int forcedMissileIndex,
//      bool isPrediction,
//      Agent shooterAgent,
//      in WeaponData weaponData,
//      WeaponStatsData[] weaponStatsData,
//      float damageBonus,
//      ref Vec3 position,
//      ref Vec3 direction,
//      ref Mat3 orientation,
//      float baseSpeed,
//      float speed,
//      bool addRigidBody,
//      WeakGameEntity gameEntityToIgnore,
//      bool isPrimaryWeaponShot,
//      out GameEntity missileEntity)
//    {
//      UIntPtr missileEntity1;
//      int num = MBAPI.IMBMission.AddMissile(this.Pointer, isPrediction, shooterAgent.Index, in weaponData, weaponStatsData, weaponStatsData.Length, damageBonus, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, gameEntityToIgnore.Pointer, forcedMissileIndex, isPrimaryWeaponShot, out missileEntity1);
//      missileEntity = isPrediction ? (GameEntity) null : new GameEntity(missileEntity1);
//      return num;
//    }

//    private int AddMissileSingleUsageAux(
//      int forcedMissileIndex,
//      bool isPrediction,
//      Agent shooterAgent,
//      in WeaponData weaponData,
//      in WeaponStatsData weaponStatsData,
//      float damageBonus,
//      ref Vec3 position,
//      ref Vec3 direction,
//      ref Mat3 orientation,
//      float baseSpeed,
//      float speed,
//      bool addRigidBody,
//      WeakGameEntity gameEntityToIgnore,
//      bool isPrimaryWeaponShot,
//      out GameEntity missileEntity)
//    {
//      UIntPtr missileEntity1;
//      int num = MBAPI.IMBMission.AddMissileSingleUsage(this.Pointer, isPrediction, shooterAgent.Index, in weaponData, in weaponStatsData, damageBonus, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, gameEntityToIgnore.Pointer, forcedMissileIndex, isPrimaryWeaponShot, out missileEntity1);
//      missileEntity = isPrediction ? (GameEntity) null : new GameEntity(missileEntity1);
//      return num;
//    }

//    public Vec3 GetMissileCollisionPoint(
//      Vec3 missileStartingPosition,
//      Vec3 missileDirection,
//      float missileSpeed,
//      in WeaponData weaponData)
//    {
//      return MBAPI.IMBMission.GetMissileCollisionPoint(this.Pointer, missileStartingPosition, missileDirection, missileSpeed, in weaponData);
//    }

//    public void RemoveMissileAsClient(int missileIndex)
//    {
//      MBAPI.IMBMission.RemoveMissile(this.Pointer, missileIndex);
//    }

//    public static float GetMissileVerticalAimCorrection(
//      Vec3 vecToTarget,
//      float missileStartingSpeed,
//      ref WeaponStatsData weaponStatsData,
//      float airFrictionConstant)
//    {
//      return MBAPI.IMBMission.GetMissileVerticalAimCorrection(vecToTarget, missileStartingSpeed, ref weaponStatsData, airFrictionConstant);
//    }

//    public static float GetMissileRange(float missileStartingSpeed, float heightDifference)
//    {
//      return MBAPI.IMBMission.GetMissileRange(missileStartingSpeed, heightDifference);
//    }

//    public void PrepareMissileWeaponForDrop(int missileIndex)
//    {
//      MBAPI.IMBMission.PrepareMissileWeaponForDrop(this.Pointer, missileIndex);
//    }

//    public void AddParticleSystemBurstByName(
//      string particleSystem,
//      MatrixFrame frame,
//      bool synchThroughNetwork)
//    {
//      MBAPI.IMBMission.AddParticleSystemBurstByName(this.Pointer, particleSystem, ref frame, synchThroughNetwork);
//    }

//    public bool IsLoadingFinished => MBAPI.IMBMission.GetIsLoadingFinished(this.Pointer);

//    public Vec2 GetClosestBoundaryPosition(Vec2 position)
//    {
//      return MBAPI.IMBMission.GetClosestBoundaryPosition(this.Pointer, position);
//    }

//    private void ResetMissionObjects()
//    {
//      for (int index = this._dynamicEntities.Count - 1; index >= 0; --index)
//      {
//        Mission.DynamicEntityInfo dynamicEntity = this._dynamicEntities[index];
//        dynamicEntity.Entity.RemoveEnginePhysics();
//        dynamicEntity.Entity.Remove(74);
//        this._dynamicEntities.RemoveAt(index);
//      }
//      foreach (MissionObject missionObject in (List<MissionObject>) this.MissionObjects)
//      {
//        if (missionObject.CreatedAtRuntime)
//          break;
//        missionObject.OnMissionReset();
//      }
//    }

//    private void RemoveSpawnedMissionObjects()
//    {
//      MissionObject[] array = this._missionObjects.ToArray();
//      for (int index = array.Length - 1; index >= 0; --index)
//      {
//        MissionObject missionObject = array[index];
//        if (missionObject.CreatedAtRuntime)
//        {
//          WeakGameEntity gameEntity = missionObject.GameEntity;
//          if (gameEntity.IsValid)
//          {
//            gameEntity = missionObject.GameEntity;
//            gameEntity.RemoveAllChildren();
//            gameEntity = missionObject.GameEntity;
//            gameEntity.Remove(75);
//          }
//        }
//        else
//          break;
//      }
//      this._spawnedItemEntitiesCreatedAtRuntime.Clear();
//      this._lastRuntimeMissionObjectIdCount = 0;
//      this._emptyRuntimeMissionObjectIds.Clear();
//      this._addedEntitiesInfo.Clear();
//    }

//    public int GetFreeRuntimeMissionObjectId()
//    {
//      float totalMissionTime = MBCommon.GetTotalMissionTime();
//      int runtimeMissionObjectId = -1;
//      if (this._emptyRuntimeMissionObjectIds.Count > 0)
//      {
//        if ((double) totalMissionTime - (double) this._emptyRuntimeMissionObjectIds.Peek().Item2 > 30.0 || this._lastRuntimeMissionObjectIdCount >= 8191)
//        {
//          runtimeMissionObjectId = this._emptyRuntimeMissionObjectIds.Pop().Item1;
//        }
//        else
//        {
//          runtimeMissionObjectId = this._lastRuntimeMissionObjectIdCount;
//          ++this._lastRuntimeMissionObjectIdCount;
//        }
//      }
//      else if (this._lastRuntimeMissionObjectIdCount < 8191)
//      {
//        runtimeMissionObjectId = this._lastRuntimeMissionObjectIdCount;
//        ++this._lastRuntimeMissionObjectIdCount;
//      }
//      return runtimeMissionObjectId;
//    }

//    private void ReturnRuntimeMissionObjectId(int id)
//    {
//      this._emptyRuntimeMissionObjectIds.Push((id, MBCommon.GetTotalMissionTime()));
//    }

//    public int GetFreeSceneMissionObjectId()
//    {
//      int missionObjectIdCount = this._lastSceneMissionObjectIdCount;
//      ++this._lastSceneMissionObjectIdCount;
//      return missionObjectIdCount;
//    }

//    public void SetCameraFrame(ref MatrixFrame cameraFrame, float zoomFactor)
//    {
//      this.SetCameraFrame(ref cameraFrame, zoomFactor, ref cameraFrame.origin);
//    }

//    public void SetCameraFrame(
//      ref MatrixFrame cameraFrame,
//      float zoomFactor,
//      ref Vec3 attenuationPosition)
//    {
//      cameraFrame.Fill();
//      MBAPI.IMBMission.SetCameraFrame(this.Pointer, ref cameraFrame, zoomFactor, ref attenuationPosition);
//    }

//    public MatrixFrame GetCameraFrame() => MBAPI.IMBMission.GetCameraFrame(this.Pointer);

//    public bool CameraIsFirstPerson
//    {
//      get => Mission._isCameraFirstPerson;
//      set
//      {
//        if (Mission._isCameraFirstPerson == value)
//          return;
//        Mission._isCameraFirstPerson = value;
//        MBAPI.IMBMission.SetCameraIsFirstPerson(value);
//        this.ResetFirstThirdPersonView();
//      }
//    }

//    public static float CameraAddedDistance
//    {
//      get => BannerlordConfig.CombatCameraDistance;
//      set
//      {
//        if ((double) value == (double) BannerlordConfig.CombatCameraDistance)
//          return;
//        BannerlordConfig.CombatCameraDistance = value;
//      }
//    }

//    public float ClearSceneTimerElapsedTime
//    {
//      get => MBAPI.IMBMission.GetClearSceneTimerElapsedTime(this.Pointer);
//    }

//    public void ResetFirstThirdPersonView()
//    {
//      MBAPI.IMBMission.ResetFirstThirdPersonView(this.Pointer);
//    }

//    public void SetCustomCameraLocalOffset(Vec3 newCameraOffset)
//    {
//      this.CustomCameraLocalOffset = newCameraOffset;
//    }

//    public void SetCustomCameraTargetLocalOffset(Vec3 newTargetLocalOffset)
//    {
//      this.CustomCameraTargetLocalOffset = newTargetLocalOffset;
//    }

//    public void SetCustomCameraLocalOffset2(Vec3 newCameraOffset)
//    {
//      this.CustomCameraLocalOffset2 = newCameraOffset;
//    }

//    public void SetCustomCameraLocalRotationalOffset(Vec3 newCameraRotationalOffset)
//    {
//      this.CustomCameraLocalRotationalOffset = newCameraRotationalOffset;
//    }

//    public void SetCustomCameraGlobalOffset(Vec3 newCameraOffset)
//    {
//      this.CustomCameraGlobalOffset = newCameraOffset;
//    }

//    public void SetCustomCameraFovMultiplier(float newFovMultiplier)
//    {
//      this.CustomCameraFovMultiplier = newFovMultiplier;
//    }

//    public void SetCustomCameraFixedDistance(float distance)
//    {
//      this.CustomCameraFixedDistance = distance;
//    }

//    public void SetIgnoredEntityForCamera(GameEntity ignoredEntity)
//    {
//      this.IgnoredEntityForCamera = ignoredEntity;
//    }

//    public void SetCustomCameraIgnoreCollision(bool ignoreCollision)
//    {
//      this.CustomCameraIgnoreCollision = ignoreCollision;
//    }

//    public void SetListenerAndAttenuationPosBlendFactor(float factor)
//    {
//      this.ListenerAndAttenuationPosBlendFactor = factor;
//    }

//    internal void UpdateSceneTimeSpeed()
//    {
//      if (!((NativeObject) this.Scene != (NativeObject) null))
//        return;
//      float comparedValue = 1f;
//      int num = -1;
//      for (int index = 0; index < this._timeSpeedRequests.Count; ++index)
//      {
//        Mission.TimeSpeedRequest timeSpeedRequest = this._timeSpeedRequests[index];
//        if ((double) timeSpeedRequest.RequestedTimeSpeed < (double) comparedValue)
//        {
//          timeSpeedRequest = this._timeSpeedRequests[index];
//          comparedValue = timeSpeedRequest.RequestedTimeSpeed;
//          timeSpeedRequest = this._timeSpeedRequests[index];
//          num = timeSpeedRequest.RequestID;
//        }
//      }
//      if (this.Scene.TimeSpeed.ApproximatelyEqualsTo(comparedValue))
//        return;
//      if (num != -1)
//        TaleWorlds.Library.Debug.Print(string.Format("Updated mission time speed with request ID:{0}, time speed{1}", (object) num, (object) comparedValue));
//      else
//        TaleWorlds.Library.Debug.Print(string.Format("Reverted time speed back to default({0})", (object) comparedValue));
//      this.Scene.TimeSpeed = comparedValue;
//    }

//    public void AddTimeSpeedRequest(Mission.TimeSpeedRequest request)
//    {
//      this._timeSpeedRequests.Add(request);
//    }

//    [Conditional("_RGL_KEEP_ASSERTS")]
//    private void AssertTimeSpeedRequestDoesntExist(Mission.TimeSpeedRequest request)
//    {
//      for (int index = 0; index < this._timeSpeedRequests.Count; ++index)
//      {
//        int requestId1 = this._timeSpeedRequests[index].RequestID;
//        int requestId2 = request.RequestID;
//      }
//    }

//    public void RemoveTimeSpeedRequest(int timeSpeedRequestID)
//    {
//      int index1 = -1;
//      for (int index2 = 0; index2 < this._timeSpeedRequests.Count; ++index2)
//      {
//        if (this._timeSpeedRequests[index2].RequestID == timeSpeedRequestID)
//          index1 = index2;
//      }
//      this._timeSpeedRequests.RemoveAt(index1);
//    }

//    public bool GetRequestedTimeSpeed(int timeSpeedRequestID, out float requestedTime)
//    {
//      for (int index = 0; index < this._timeSpeedRequests.Count; ++index)
//      {
//        if (this._timeSpeedRequests[index].RequestID == timeSpeedRequestID)
//        {
//          requestedTime = this._timeSpeedRequests[index].RequestedTimeSpeed;
//          return true;
//        }
//      }
//      requestedTime = 0.0f;
//      return false;
//    }

//    public void ClearAgentActions() => MBAPI.IMBMission.ClearAgentActions(this.Pointer);

//    public void ClearMissiles() => MBAPI.IMBMission.ClearMissiles(this.Pointer);

//    public void ClearCorpses(bool isMissionReset)
//    {
//      MBAPI.IMBMission.ClearCorpses(this.Pointer, isMissionReset);
//    }

//    private Agent FindAgentWithIndexAux(int index)
//    {
//      return index >= 0 ? MBAPI.IMBMission.FindAgentWithIndex(this.Pointer, index) : (Agent) null;
//    }

//    private Agent GetClosestEnemyAgent(MBTeam team, Vec3 position, float radius)
//    {
//      return MBAPI.IMBMission.GetClosestEnemy(this.Pointer, team.Index, position, radius);
//    }

//    private Agent GetClosestAllyAgent(MBTeam team, Vec3 position, float radius)
//    {
//      return MBAPI.IMBMission.GetClosestAlly(this.Pointer, team.Index, position, radius);
//    }

//    private int GetNearbyEnemyAgentCount(MBTeam team, Vec2 position, float radius)
//    {
//      int allyCount = 0;
//      int enemyCount = 0;
//      MBAPI.IMBMission.GetAgentCountAroundPosition(this.Pointer, team.Index, position, radius, ref allyCount, ref enemyCount);
//      return enemyCount;
//    }

//    public bool IsAgentInProximityMap(Agent agent)
//    {
//      return MBAPI.IMBMission.IsAgentInProximityMap(this.Pointer, agent.Index);
//    }

//    public void OnMissionStateActivate()
//    {
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnMissionStateActivated();
//    }

//    public void OnMissionStateDeactivate()
//    {
//      if (this.MissionBehaviors == null)
//        return;
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnMissionStateDeactivated();
//    }

//    public void OnMissionStateFinalize(bool forceClearGPUResources)
//    {
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnMissionStateFinalized();
//      if (GameNetwork.IsSessionActive && this.GetMissionBehavior<MissionNetworkComponent>() != null)
//        this.RemoveMissionBehavior((MissionBehavior) this.GetMissionBehavior<MissionNetworkComponent>());
//      for (int index = this.MissionBehaviors.Count - 1; index >= 0; --index)
//        this.RemoveMissionBehavior(this.MissionBehaviors[index]);
//      this._deploymentPlan = (IMissionDeploymentPlan) null;
//      this.MissionLogics.Clear();
//      this.Scene = (Scene) null;
//      Mission.Current = (Mission) null;
//      this.ClearUnreferencedResources(forceClearGPUResources);
//    }

//    public void ClearUnreferencedResources(bool forceClearGPUResources)
//    {
//      Common.MemoryCleanupGC();
//      if (!forceClearGPUResources)
//        return;
//      MBAPI.IMBMission.ClearResources(this.Pointer);
//    }

//    internal void OnEntityHit(
//      WeakGameEntity entity,
//      Agent attackerAgent,
//      int inflictedDamage,
//      DamageTypes damageType,
//      Vec3 impactPosition,
//      Vec3 impactDirection,
//      in MissionWeapon weapon,
//      int affectorWeaponSlotOrMissileIndex,
//      ref CombatLogData combatLog)
//    {
//      bool flag = false;
//      float finalDamage = (float) inflictedDamage;
//      MissionObject missionObject = (MissionObject) null;
//      for (; entity.IsValid; entity = entity.Parent)
//      {
//        foreach (MissionObject scriptComponent in entity.GetScriptComponents<MissionObject>())
//        {
//          bool reportDamage;
//          if (scriptComponent.OnHit(attackerAgent, inflictedDamage, impactPosition, impactDirection, in weapon, affectorWeaponSlotOrMissileIndex, (ScriptComponentBehavior) null, out reportDamage, out finalDamage))
//            missionObject = scriptComponent;
//          flag |= reportDamage;
//        }
//        if (missionObject != null)
//          break;
//      }
//      combatLog.MissionObjectHit = missionObject;
//      if (!flag || attackerAgent == null || attackerAgent.IsMount || attackerAgent.IsAIControlled)
//        return;
//      combatLog.DamageType = damageType;
//      combatLog.InflictedDamage = inflictedDamage;
//      combatLog.ModifiedDamage = MathF.Round(finalDamage - (float) inflictedDamage);
//      this.AddCombatLogSafe(attackerAgent, (Agent) null, combatLog);
//    }

//    public float GetMainAgentMaxCameraZoom()
//    {
//      return this.MainAgent != null ? MissionGameModels.Current.AgentStatCalculateModel.GetMaxCameraZoom(this.MainAgent) : 1f;
//    }

//    public WorldPosition GetBestSlopeTowardsDirection(
//      ref WorldPosition centerPosition,
//      float halfSize,
//      ref WorldPosition referencePosition)
//    {
//      return MBAPI.IMBMission.GetBestSlopeTowardsDirection(this.Pointer, ref centerPosition, halfSize, ref referencePosition);
//    }

//    public WorldPosition GetBestSlopeAngleHeightPosForDefending(
//      WorldPosition enemyPosition,
//      WorldPosition defendingPosition,
//      int sampleSize,
//      float distanceRatioAllowedFromDefendedPos,
//      float distanceSqrdAllowedFromBoundary,
//      float cosinusOfBestSlope,
//      float cosinusOfMaxAcceptedSlope,
//      float minSlopeScore,
//      float maxSlopeScore,
//      float excessiveSlopePenalty,
//      float nearConeCenterRatio,
//      float nearConeCenterBonus,
//      float heightDifferenceCeiling,
//      float maxDisplacementPenalty)
//    {
//      return MBAPI.IMBMission.GetBestSlopeAngleHeightPosForDefending(this.Pointer, enemyPosition, defendingPosition, sampleSize, distanceRatioAllowedFromDefendedPos, distanceSqrdAllowedFromBoundary, cosinusOfBestSlope, cosinusOfMaxAcceptedSlope, minSlopeScore, maxSlopeScore, excessiveSlopePenalty, nearConeCenterRatio, nearConeCenterBonus, heightDifferenceCeiling, maxDisplacementPenalty);
//    }

//    public Vec2 GetAveragePositionOfAgents(List<Agent> agents)
//    {
//      int num = 0;
//      Vec2 zero = Vec2.Zero;
//      foreach (Agent agent in agents)
//      {
//        ++num;
//        zero += agent.Position.AsVec2;
//      }
//      return num == 0 ? Vec2.Invalid : zero * (1f / (float) num);
//    }

//    private void GetNearbyAgentsAux(
//      Vec2 center,
//      float radius,
//      MBTeam team,
//      Mission.GetNearbyAgentsAuxType type,
//      MBList<Agent> resultList)
//    {
//      EngineStackArray.StackArray40Int agentIds = new EngineStackArray.StackArray40Int();
//      lock (Mission.GetNearbyAgentsAuxLock)
//      {
//        int agentsArrayOffset = 0;
//        while (true)
//        {
//          int retrievedAgentCount = -1;
//          MBAPI.IMBMission.GetNearbyAgentsAux(this.Pointer, center, radius, team.Index, (int) type, agentsArrayOffset, ref agentIds, ref retrievedAgentCount);
//          for (int index = 0; index < retrievedAgentCount; ++index)
//          {
//            Agent managedObjectWithId = DotNetObject.GetManagedObjectWithId(agentIds[index]) as Agent;
//            resultList.Add(managedObjectWithId);
//          }
//          if (retrievedAgentCount >= 40)
//            agentsArrayOffset += 40;
//          else
//            break;
//        }
//      }
//    }

//    private int GetNearbyAgentsCountAux(
//      Vec2 center,
//      float radius,
//      MBTeam team,
//      Mission.GetNearbyAgentsAuxType type)
//    {
//      int nearbyAgentsCountAux = 0;
//      EngineStackArray.StackArray40Int agentIds = new EngineStackArray.StackArray40Int();
//      lock (Mission.GetNearbyAgentsAuxLock)
//      {
//        int agentsArrayOffset = 0;
//        while (true)
//        {
//          int retrievedAgentCount = -1;
//          MBAPI.IMBMission.GetNearbyAgentsAux(this.Pointer, center, radius, team.Index, (int) type, agentsArrayOffset, ref agentIds, ref retrievedAgentCount);
//          nearbyAgentsCountAux += retrievedAgentCount;
//          if (retrievedAgentCount >= 40)
//            agentsArrayOffset += 40;
//          else
//            break;
//        }
//      }
//      return nearbyAgentsCountAux;
//    }

//    public void SetRandomDecideTimeOfAgentsWithIndices(
//      int[] agentIndices,
//      float? minAIReactionTime = null,
//      float? maxAIReactionTime = null)
//    {
//      if (!minAIReactionTime.HasValue || !maxAIReactionTime.HasValue)
//      {
//        maxAIReactionTime = new float?(-1f);
//        minAIReactionTime = maxAIReactionTime;
//      }
//      MBAPI.IMBMission.SetRandomDecideTimeOfAgents(this.Pointer, agentIndices.Length, agentIndices, minAIReactionTime.Value, maxAIReactionTime.Value);
//    }

//    public void SetBowMissileSpeedModifier(float modifier)
//    {
//      MBAPI.IMBMission.SetBowMissileSpeedModifier(this.Pointer, modifier);
//    }

//    public void SetCrossbowMissileSpeedModifier(float modifier)
//    {
//      MBAPI.IMBMission.SetCrossbowMissileSpeedModifier(this.Pointer, modifier);
//    }

//    public void SetThrowingMissileSpeedModifier(float modifier)
//    {
//      MBAPI.IMBMission.SetThrowingMissileSpeedModifier(this.Pointer, modifier);
//    }

//    public void SetMissileRangeModifier(float modifier)
//    {
//      MBAPI.IMBMission.SetMissileRangeModifier(this.Pointer, modifier);
//    }

//    public void SetLastMovementKeyPressed(Agent.MovementControlFlag lastMovementKeyPressed)
//    {
//      MBAPI.IMBMission.SetLastMovementKeyPressed(this.Pointer, lastMovementKeyPressed);
//    }

//    public Vec2 GetWeightedPointOfEnemies(Agent agent, Vec2 basePoint)
//    {
//      return MBAPI.IMBMission.GetWeightedPointOfEnemies(this.Pointer, agent.Index, basePoint);
//    }

//    public bool GetPathBetweenPositions(ref NavigationData navData)
//    {
//      return MBAPI.IMBMission.GetNavigationPoints(this.Pointer, ref navData);
//    }

//    public void SetNavigationFaceCostWithIdAroundPosition(
//      int navigationFaceId,
//      Vec3 position,
//      float cost)
//    {
//      MBAPI.IMBMission.SetNavigationFaceCostWithIdAroundPosition(this.Pointer, navigationFaceId, position, cost);
//    }

//    public WorldPosition GetStraightPathToTarget(
//      Vec2 targetPosition,
//      WorldPosition startingPosition,
//      float samplingDistance = 1f,
//      bool stopAtObstacle = true)
//    {
//      return MBAPI.IMBMission.GetStraightPathToTarget(this.Pointer, targetPosition, startingPosition, samplingDistance, stopAtObstacle);
//    }

//    public void SkipForwardMissionReplay(float startTime, float endTime)
//    {
//      MBAPI.IMBMission.SkipForwardMissionReplay(this.Pointer, startTime, endTime);
//    }

//    public int GetDebugAgent() => MBAPI.IMBMission.GetDebugAgent(this.Pointer);

//    public void AddAiDebugText(string str) => MBAPI.IMBMission.AddAiDebugText(this.Pointer, str);

//    public void SetDebugAgent(int index) => MBAPI.IMBMission.SetDebugAgent(this.Pointer, index);

//    public static float GetFirstPersonFov() => BannerlordConfig.FirstPersonFov;

//    public float GetWaterLevelAtPosition(Vec2 position, bool useWaterRenderer)
//    {
//      return MBAPI.IMBMission.GetWaterLevelAtPosition(this.Pointer, position, useWaterRenderer);
//    }

//    public float GetWaterLevelAtPositionMT(Vec2 position, bool useWaterRenderer)
//    {
//      return MBAPI.IMBMission.GetWaterLevelAtPosition(this.Pointer, position, useWaterRenderer);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, true)]
//    public bool CanPhysicsCollideBetweenTwoEntities(UIntPtr entity0Ptr, UIntPtr entity1Ptr)
//    {
//      WeakGameEntity weakGameEntity1 = new WeakGameEntity(entity0Ptr);
//      WeakGameEntity weakGameEntity2 = new WeakGameEntity(entity1Ptr);
//      for (WeakGameEntity weakGameEntity3 = weakGameEntity1; weakGameEntity3.IsValid; weakGameEntity3 = weakGameEntity3.Parent)
//      {
//        foreach (ScriptComponentBehavior scriptComponent in weakGameEntity3.GetScriptComponents())
//        {
//          if (scriptComponent != null && !(weakGameEntity1 == weakGameEntity2) && !scriptComponent.CanPhysicsCollideBetweenTwoEntities(weakGameEntity1, weakGameEntity2))
//            return false;
//        }
//      }
//      for (WeakGameEntity weakGameEntity4 = weakGameEntity2; weakGameEntity4.IsValid; weakGameEntity4 = weakGameEntity4.Parent)
//      {
//        foreach (ScriptComponentBehavior scriptComponent in weakGameEntity4.GetScriptComponents())
//        {
//          if (scriptComponent != null && !(weakGameEntity1 == weakGameEntity2) && !scriptComponent.CanPhysicsCollideBetweenTwoEntities(weakGameEntity2, weakGameEntity1))
//            return false;
//        }
//      }
//      return true;
//    }

//    public event Mission.OnBeforeAgentRemovedDelegate OnBeforeAgentRemoved;

//    public event Func<WorldPosition, Team, bool> IsFormationUnitPositionAvailable_AdditionalCondition;

//    public event Func<Agent, bool> CanAgentRout_AdditionalCondition;

//    public event Mission.OnAddSoundAlarmFactorToAgentsDelegate OnAddSoundAlarmFactorToAgents;

//    public event Func<bool> IsAgentInteractionAllowed_AdditionalCondition;

//    public event Mission.OnMainAgentChangedDelegate OnMainAgentChanged;

//    public event Func<BattleSideEnum, BasicCharacterObject, FormationClass> GetAgentTroopClass_Override;

//    public event Action<Agent, SpawnedItemEntity> OnItemPickUp;

//    public MBReadOnlyList<Mission.Missile> MissilesList
//    {
//      get => (MBReadOnlyList<Mission.Missile>) this._missilesList;
//    }

//    public event Action<Agent, SpawnedItemEntity> OnItemDrop;

//    public event Action<Formation> FormationCaptainChanged;

//    public event Func<Agent, WorldPosition?> GetOverriddenFleePositionForAgent;

//    public bool MissionEnded
//    {
//      get => this._missionEnded;
//      private set
//      {
//        if (!this._missionEnded & value)
//        {
//          this.MissionIsEnding = true;
//          foreach (MissionObject missionObject in (List<MissionObject>) this.MissionObjects)
//            missionObject.OnMissionEnded();
//          this.MissionIsEnding = false;
//        }
//        this._missionEnded = value;
//      }
//    }

//    public MBReadOnlyList<KeyValuePair<Agent, MissionTime>> MountsWithoutRiders
//    {
//      get => (MBReadOnlyList<KeyValuePair<Agent, MissionTime>>) this._mountsWithoutRiders;
//    }

//    public event Func<bool> AreOrderGesturesEnabled_AdditionalCondition;

//    public bool MissionIsEnding { get; private set; }

//    public bool IsDeploymentFinished { get; private set; }

//    public event Func<bool> IsBattleInRetreatEvent;

//    public event Action<int> OnMissileRemovedEvent;

//    public BattleSideEnum RetreatSide { get; private set; } = BattleSideEnum.None;

//    public bool IsFastForward
//    {
//      get => this._isFastForward;
//      private set
//      {
//        this._isFastForward = value;
//        MBAPI.IMBMission.OnFastForwardStateChanged(this.Pointer, this._isFastForward);
//      }
//    }

//    public bool FixedDeltaTimeMode { get; set; }

//    public float FixedDeltaTime { get; set; }

//    public Mission.State CurrentState { get; private set; }

//    public Mission.TeamCollection Teams { get; private set; }

//    public Team AttackerTeam => this.Teams.Attacker;

//    public Team DefenderTeam => this.Teams.Defender;

//    public Team AttackerAllyTeam => this.Teams.AttackerAlly;

//    public Team DefenderAllyTeam => this.Teams.DefenderAlly;

//    public Team PlayerTeam
//    {
//      get => this.Teams.Player;
//      set => this.Teams.Player = value;
//    }

//    public Team PlayerEnemyTeam => this.Teams.PlayerEnemy;

//    public Team PlayerAllyTeam => this.Teams.PlayerAlly;

//    public Team SpectatorTeam { get; set; }

//    IMissionTeam IMission.PlayerTeam => (IMissionTeam) this.PlayerTeam;

//    public bool IsMissionEnding => this.CurrentState != Mission.State.Over && this.MissionEnded;

//    public List<MissionLogic> MissionLogics { get; }

//    public List<MissionBehavior> MissionBehaviors { get; }

//    public IInputContext InputManager { get; set; }

//    public bool NeedsMemoryCleanup { get; private set; }

//    public Agent MainAgent
//    {
//      get => this._mainAgent;
//      set
//      {
//        Agent mainAgent = this._mainAgent;
//        this._mainAgent = value;
//        Mission.OnMainAgentChangedDelegate mainAgentChanged = this.OnMainAgentChanged;
//        if (mainAgentChanged != null)
//          mainAgentChanged(mainAgent);
//        if (GameNetwork.IsClient)
//          return;
//        this.MainAgentServer = this._mainAgent;
//      }
//    }

//    public IMissionDeploymentPlan DeploymentPlan => this._deploymentPlan;

//    public bool GetDeploymentPlan<T>(out T deploymentPlan) where T : IMissionDeploymentPlan
//    {
//      deploymentPlan = default (T);
//      IMissionDeploymentPlan deploymentPlan1;
//      if (this._deploymentPlan != null && (deploymentPlan1 = this._deploymentPlan) is T)
//      {
//        T obj = (T) deploymentPlan1;
//        deploymentPlan = obj;
//      }
//      return (object) deploymentPlan != null;
//    }

//    public float GetRemovedAgentRatioForSide(BattleSideEnum side)
//    {
//      float agentRatioForSide = 0.0f;
//      if (side == BattleSideEnum.NumSides)
//        TaleWorlds.Library.Debug.FailedAssert("Cannot get removed agent count for side. Invalid battle side passed!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (GetRemovedAgentRatioForSide), 722);
//      float num = (float) this._initialAgentCountPerSide[(int) side];
//      if ((double) num > 0.0 && this._agentCount > 0)
//        agentRatioForSide = MathF.Min((float) this._removedAgentCountPerSide[(int) side] / num, 1f);
//      return agentRatioForSide;
//    }

//    public Agent MainAgentServer { get; set; }

//    public bool HasSpawnPath => this._battleSpawnPathSelector.IsInitialized;

//    public bool IsFieldBattle
//    {
//      get => this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.FieldBattle;
//    }

//    public bool IsSiegeBattle => this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.Siege;

//    public bool IsSallyOutBattle
//    {
//      get => this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.SallyOut;
//    }

//    public bool IsNavalBattle
//    {
//      get => this.MissionTeamAIType == Mission.MissionTeamAITypeEnum.NavalBattle;
//    }

//    public AgentReadOnlyList AllAgents => (AgentReadOnlyList) this._allAgents;

//    public MBReadOnlyList<Mission.CorpseAgentInfo> CorpseAgentInfos
//    {
//      get => (MBReadOnlyList<Mission.CorpseAgentInfo>) this._corpseAgentInfos;
//    }

//    public AgentReadOnlyList Agents => (AgentReadOnlyList) this._activeAgents;

//    public bool IsInventoryAccessAllowed
//    {
//      get
//      {
//        return (Game.Current.GameType.IsInventoryAccessibleAtMission || this._isScreenAccessAllowed) && this.IsInventoryAccessible;
//      }
//    }

//    public bool IsInventoryAccessible { private get; set; }

//    public MissionResult MissionResult { get; private set; }

//    public MissionFocusableObjectInformationProvider FocusableObjectInformationProvider { get; private set; }

//    public bool IsQuestScreenAccessible { private get; set; }

//    private bool _isScreenAccessAllowed
//    {
//      get
//      {
//        return this.Mode != MissionMode.Battle && this.Mode != MissionMode.Deployment && this.Mode != MissionMode.Duel && this.Mode != MissionMode.CutScene;
//      }
//    }

//    public bool IsQuestScreenAccessAllowed
//    {
//      get
//      {
//        return (Game.Current.GameType.IsQuestScreenAccessibleAtMission || this._isScreenAccessAllowed) && this.IsQuestScreenAccessible;
//      }
//    }

//    public bool IsCharacterWindowAccessible { private get; set; }

//    public bool IsCharacterWindowAccessAllowed
//    {
//      get
//      {
//        return (Game.Current.GameType.IsCharacterWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsCharacterWindowAccessible;
//      }
//    }

//    public bool IsPartyWindowAccessible { private get; set; }

//    public bool IsPartyWindowAccessAllowed
//    {
//      get
//      {
//        return (Game.Current.GameType.IsPartyWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsPartyWindowAccessible;
//      }
//    }

//    public bool IsKingdomWindowAccessible { private get; set; }

//    public bool IsKingdomWindowAccessAllowed
//    {
//      get
//      {
//        return (Game.Current.GameType.IsKingdomWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsKingdomWindowAccessible;
//      }
//    }

//    public bool IsClanWindowAccessible { private get; set; }

//    public bool IsClanWindowAccessAllowed
//    {
//      get
//      {
//        return Game.Current.GameType.IsClanWindowAccessibleAtMission && this._isScreenAccessAllowed && this.IsClanWindowAccessible;
//      }
//    }

//    public bool IsEncyclopediaWindowAccessible { private get; set; }

//    public bool IsEncyclopediaWindowAccessAllowed
//    {
//      get
//      {
//        return (Game.Current.GameType.IsEncyclopediaWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsEncyclopediaWindowAccessible;
//      }
//    }

//    public bool IsBannerWindowAccessible { private get; set; }

//    public bool IsBannerWindowAccessAllowed
//    {
//      get
//      {
//        return (Game.Current.GameType.IsBannerWindowAccessibleAtMission || this._isScreenAccessAllowed) && this.IsBannerWindowAccessible;
//      }
//    }

//    public bool DoesMissionRequireCivilianEquipment
//    {
//      get => this._doesMissionRequireCivilianEquipment;
//      set => this._doesMissionRequireCivilianEquipment = value;
//    }

//    public Mission.MissionTeamAITypeEnum MissionTeamAIType { get; set; }

//    private Lazy<MissionRecorder> _recorder
//    {
//      get => new Lazy<MissionRecorder>((Func<MissionRecorder>) (() => new MissionRecorder(this)));
//    }

//    public MissionRecorder Recorder => this._recorder.Value;

//    public bool CanPlayerTakeControlOfAnotherAgentWhenDead
//    {
//      get => this._canPlayerTakeControlOfAnotherAgentWhenDead;
//    }

//    public ref readonly List<SiegeWeapon> GetAttackerWeaponsForFriendlyFirePreventing()
//    {
//      return ref this._attackerWeaponsForFriendlyFirePreventing;
//    }

//    public void OnDeploymentPlanMade(Team team, bool isFirstPlan)
//    {
//      foreach (IMissionListener listener in this._listeners)
//        listener.OnDeploymentPlanMade(team, isFirstPlan);
//    }

//    public WorldPosition GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(
//      Vec2 directionTowards,
//      WorldPosition originalPosition,
//      ref float positionPenalty)
//    {
//      return MBAPI.IMBMission.GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(this.Pointer, ref directionTowards, ref originalPosition, ref positionPenalty);
//    }

//    public int GetNextDynamicNavMeshIdStart()
//    {
//      int dynamicNavMeshIdStart = this._nextDynamicNavMeshIdStart;
//      this._nextDynamicNavMeshIdStart += 50;
//      return dynamicNavMeshIdStart;
//    }

//    public FormationClass GetAgentTroopClass(
//      BattleSideEnum battleSide,
//      BasicCharacterObject agentCharacter)
//    {
//      if (this.GetAgentTroopClass_Override != null)
//        return this.GetAgentTroopClass_Override(battleSide, agentCharacter);
//      FormationClass troopClass = agentCharacter.GetFormationClass();
//      if ((this.IsSiegeBattle ? 1 : (!this.IsSallyOutBattle ? 0 : (battleSide == BattleSideEnum.Attacker ? 1 : 0))) != 0)
//        troopClass = troopClass.DismountedClass();
//      return troopClass;
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    public WorldPosition GetClosestFleePositionForAgent(Agent agent)
//    {
//      if (this.GetOverriddenFleePositionForAgent != null)
//      {
//        WorldPosition? nullable = this.GetOverriddenFleePositionForAgent(agent);
//        if (nullable.HasValue)
//          return nullable.Value;
//      }
//      WorldPosition worldPosition = agent.GetWorldPosition();
//      float forwardUnlimitedSpeed = agent.GetMaximumForwardUnlimitedSpeed();
//      Team team = agent.Team;
//      BattleSideEnum side = BattleSideEnum.None;
//      bool runnerHasMount = agent.MountAgent != null;
//      if (team != null)
//      {
//        team.UpdateCachedEnemyDataForFleeing();
//        side = team.Side;
//      }
//      return this.GetClosestFleePosition(this.MissionTeamAIType != Mission.MissionTeamAITypeEnum.SallyOut || !agent.IsMount ? this.GetFleePositionsForSide(side) : this.GetFleePositionsForSide(BattleSideEnum.Attacker), worldPosition, forwardUnlimitedSpeed, runnerHasMount, team?.CachedEnemyDataForFleeing);
//    }

//    public WorldPosition GetClosestFleePositionForFormation(Formation formation)
//    {
//      WorldPosition cachedMedianPosition = formation.CachedMedianPosition;
//      float movementSpeedMaximum = formation.QuerySystem.MovementSpeedMaximum;
//      bool runnerHasMount = formation.QuerySystem.IsCavalryFormation || formation.QuerySystem.IsRangedCavalryFormation;
//      Team team = formation.Team;
//      team.UpdateCachedEnemyDataForFleeing();
//      return this.GetClosestFleePosition(this.GetFleePositionsForSide(team.Side), cachedMedianPosition, movementSpeedMaximum, runnerHasMount, team.CachedEnemyDataForFleeing);
//    }

//    private WorldPosition GetClosestFleePosition(
//      MBReadOnlyList<FleePosition> availableFleePositions,
//      WorldPosition runnerPosition,
//      float runnerSpeed,
//      bool runnerHasMount,
//      MBReadOnlyList<(float, WorldPosition, int, Vec2, Vec2, bool)> chaserData)
//    {
//      int count = chaserData != null ? chaserData.Count : 0;
//      Vec2 vec2_1;
//      if (availableFleePositions.Count > 0)
//      {
//        float[] numArray = new float[availableFleePositions.Count];
//        WorldPosition[] worldPositionArray = new WorldPosition[availableFleePositions.Count];
//        for (int index = 0; index < availableFleePositions.Count; ++index)
//        {
//          numArray[index] = 1f;
//          worldPositionArray[index] = new WorldPosition(this.Scene, UIntPtr.Zero, availableFleePositions[index].GetClosestPointToEscape(runnerPosition.AsVec2), false);
//          worldPositionArray[index].SetVec2(worldPositionArray[index].AsVec2 - runnerPosition.AsVec2);
//        }
//        for (int index1 = 0; index1 < count; ++index1)
//        {
//          float num1 = chaserData[index1].Item1;
//          if ((double) num1 > 0.0)
//          {
//            Vec2 asVec2 = chaserData[index1].Item2.AsVec2;
//            int num2 = chaserData[index1].Item3;
//            Vec2 vec2_2;
//            if (num2 > 1)
//            {
//              Vec2 vec2_3 = chaserData[index1].Item4;
//              Vec2 vec2_4 = chaserData[index1].Item5;
//              ref Vec2 local1 = ref vec2_3;
//              ref Vec2 local2 = ref vec2_4;
//              vec2_1 = runnerPosition.AsVec2;
//              ref Vec2 local3 = ref vec2_1;
//              vec2_2 = MBMath.GetClosestPointOnLineSegmentToPoint(in local1, in local2, in local3) - runnerPosition.AsVec2;
//            }
//            else
//              vec2_2 = asVec2 - runnerPosition.AsVec2;
//            for (int index2 = 0; index2 < availableFleePositions.Count; ++index2)
//            {
//              ref Vec2 local4 = ref vec2_2;
//              vec2_1 = worldPositionArray[index2].AsVec2;
//              Vec2 v1 = vec2_1.Normalized();
//              float num3 = local4.DotProduct(v1);
//              if ((double) num3 > 0.0)
//              {
//                ref Vec2 local5 = ref vec2_2;
//                vec2_1 = worldPositionArray[index2].AsVec2;
//                vec2_1 = vec2_1.LeftVec();
//                Vec2 v2 = vec2_1.Normalized();
//                float num4 = MathF.Max(MathF.Abs(local5.DotProduct(v2)) / num1, 1f);
//                float num5 = MathF.Max(num3 / runnerSpeed, 1f);
//                if ((double) num5 > (double) num4)
//                {
//                  float num6 = num5 / num4 / num3;
//                  numArray[index2] += num6 * (float) num2;
//                }
//              }
//            }
//          }
//        }
//        for (int index = 0; index < availableFleePositions.Count; ++index)
//        {
//          WorldPosition point1 = new WorldPosition(this.Scene, UIntPtr.Zero, availableFleePositions[index].GetClosestPointToEscape(runnerPosition.AsVec2), false);
//          float pathDistance;
//          if (this.Scene.GetPathDistanceBetweenPositions(ref runnerPosition, ref point1, 0.0f, out pathDistance))
//            numArray[index] *= pathDistance;
//          else
//            numArray[index] = float.MaxValue;
//        }
//        int index3 = -1;
//        float maxValue = float.MaxValue;
//        for (int index4 = 0; index4 < availableFleePositions.Count; ++index4)
//        {
//          if ((double) maxValue > (double) numArray[index4])
//          {
//            index3 = index4;
//            maxValue = numArray[index4];
//          }
//        }
//        if (index3 >= 0)
//        {
//          Vec3 closestPointToEscape = availableFleePositions[index3].GetClosestPointToEscape(runnerPosition.AsVec2);
//          return new WorldPosition(this.Scene, UIntPtr.Zero, closestPointToEscape, false);
//        }
//      }
//      float[] numArray1 = new float[4];
//      for (int index = 0; index < count; ++index)
//      {
//        Vec2 asVec2 = chaserData[index].Item2.AsVec2;
//        int num7 = chaserData[index].Item3;
//        Vec2 vec2_5;
//        if (num7 > 1)
//        {
//          Vec2 vec2_6 = chaserData[index].Item4;
//          Vec2 vec2_7 = chaserData[index].Item5;
//          ref Vec2 local6 = ref vec2_6;
//          ref Vec2 local7 = ref vec2_7;
//          vec2_1 = runnerPosition.AsVec2;
//          ref Vec2 local8 = ref vec2_1;
//          vec2_5 = MBMath.GetClosestPointOnLineSegmentToPoint(in local6, in local7, in local8) - runnerPosition.AsVec2;
//        }
//        else
//          vec2_5 = asVec2 - runnerPosition.AsVec2;
//        float length = vec2_5.Length;
//        if (chaserData[index].Item6)
//          length *= 0.5f;
//        if (runnerHasMount)
//          length *= 2f;
//        double num8 = (double) MBMath.ClampFloat((float) (1.0 - ((double) length - 40.0) / 40.0), 0.01f, 1f);
//        Vec2 vec2_8 = vec2_5.Normalized();
//        float num9 = 1.2f;
//        double num10 = (double) num7;
//        double num11 = num8 * num10 * (double) num9;
//        float num12 = (float) num11 * MathF.Abs(vec2_8.x);
//        float num13 = (float) num11 * MathF.Abs(vec2_8.y);
//        numArray1[(double) vec2_8.y < 0.0 ? 0 : 1] -= num13;
//        numArray1[(double) vec2_8.x < 0.0 ? 2 : 3] -= num12;
//        numArray1[(double) vec2_8.y < 0.0 ? 1 : 0] += num13;
//        numArray1[(double) vec2_8.x < 0.0 ? 3 : 2] += num12;
//      }
//      float num14 = 0.04f;
//      Vec3 min;
//      Vec3 max;
//      this.Scene.GetBoundingBox(out min, out max);
//      Vec2 boundaryPosition1 = this.GetClosestBoundaryPosition(new Vec2(runnerPosition.X, min.y));
//      Vec2 boundaryPosition2 = this.GetClosestBoundaryPosition(new Vec2(runnerPosition.X, max.y));
//      Vec2 boundaryPosition3 = this.GetClosestBoundaryPosition(new Vec2(min.x, runnerPosition.Y));
//      Vec2 boundaryPosition4 = this.GetClosestBoundaryPosition(new Vec2(max.x, runnerPosition.Y));
//      float num15 = boundaryPosition2.y - boundaryPosition1.y;
//      float num16 = boundaryPosition4.x - boundaryPosition3.x;
//      numArray1[0] += (num15 - (runnerPosition.Y - boundaryPosition1.y)) * num14;
//      numArray1[1] += (num15 - (boundaryPosition2.y - runnerPosition.Y)) * num14;
//      numArray1[2] += (num16 - (runnerPosition.X - boundaryPosition3.x)) * num14;
//      numArray1[3] += (num16 - (boundaryPosition4.x - runnerPosition.X)) * num14;
//      Vec2 xy = (double) numArray1[0] < (double) numArray1[1] || (double) numArray1[0] < (double) numArray1[2] || (double) numArray1[0] < (double) numArray1[3] ? ((double) numArray1[1] < (double) numArray1[2] || (double) numArray1[1] < (double) numArray1[3] ? ((double) numArray1[2] < (double) numArray1[3] ? new Vec2(boundaryPosition4.x, boundaryPosition4.y) : new Vec2(boundaryPosition3.x, boundaryPosition3.y)) : new Vec2(boundaryPosition2.x, boundaryPosition2.y)) : new Vec2(boundaryPosition1.x, boundaryPosition1.y);
//      return new WorldPosition(this.Scene, UIntPtr.Zero, new Vec3(xy, runnerPosition.GetNavMeshZ()), false);
//    }

//    public MissionTimeTracker MissionTimeTracker { get; private set; }

//    public MBReadOnlyList<FleePosition> GetFleePositionsForSide(BattleSideEnum side)
//    {
//      int index;
//      switch (side)
//      {
//        case BattleSideEnum.None:
//          index = 0;
//          break;
//        case BattleSideEnum.NumSides:
//          TaleWorlds.Library.Debug.FailedAssert("Flee position with invalid battle side field found!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (GetFleePositionsForSide), 1283);
//          return (MBReadOnlyList<FleePosition>) null;
//        default:
//          index = (int) (side + 1);
//          break;
//      }
//      return (MBReadOnlyList<FleePosition>) this._fleePositions[index];
//    }

//    public void AddToWeaponListForFriendlyFirePreventing(SiegeWeapon weapon)
//    {
//      this._attackerWeaponsForFriendlyFirePreventing.Add(weapon);
//    }

//    public Mission(
//      MissionInitializerRecord rec,
//      MissionState missionState,
//      bool needsMemoryCleanup)
//    {
//      this.Pointer = MBAPI.IMBMission.CreateMission(this);
//      this._spawnedItemEntitiesCreatedAtRuntime = new List<SpawnedItemEntity>();
//      this._missionObjects = new MBList<MissionObject>();
//      this._activeMissionObjects = new MBList<MissionObject>();
//      this._mountsWithoutRiders = new MBList<KeyValuePair<Agent, MissionTime>>();
//      this._addedEntitiesInfo = new MBList<Mission.DynamicallyCreatedEntity>();
//      this._emptyRuntimeMissionObjectIds = new Stack<(int, float)>();
//      this.Boundaries = new Mission.MBBoundaryCollection(this);
//      this.InitializerRecord = rec;
//      this.CurrentState = Mission.State.NewlyCreated;
//      this.IsInventoryAccessible = false;
//      this.IsQuestScreenAccessible = true;
//      this.IsCharacterWindowAccessible = true;
//      this.IsPartyWindowAccessible = true;
//      this.IsKingdomWindowAccessible = true;
//      this.IsClanWindowAccessible = true;
//      this.IsBannerWindowAccessible = false;
//      this.IsEncyclopediaWindowAccessible = true;
//      this._missilesList = new MBList<Mission.Missile>();
//      this._missilesDictionary = new Dictionary<int, Mission.Missile>();
//      this._activeAgents = new AgentList(256);
//      this._allAgents = new AgentList(256);
//      this._corpseAgentInfos = new MBList<Mission.CorpseAgentInfo>(256);
//      for (int index = 0; index < 3; ++index)
//        this._fleePositions[index] = new MBList<FleePosition>(32);
//      for (int index = 0; index < 2; ++index)
//      {
//        this._initialAgentCountPerSide[index] = 0;
//        this._removedAgentCountPerSide[index] = 0;
//      }
//      this.MissionBehaviors = new List<MissionBehavior>();
//      this.MissionLogics = new List<MissionLogic>();
//      this._otherMissionBehaviors = new List<MissionBehavior>();
//      this._missionState = missionState;
//      this._battleSpawnPathSelector = new BattleSpawnPathSelector(this);
//      this.Teams = new Mission.TeamCollection(this);
//      this.FocusableObjectInformationProvider = new MissionFocusableObjectInformationProvider();
//      this.MissionTimeTracker = new MissionTimeTracker();
//      this.NeedsMemoryCleanup = needsMemoryCleanup;
//    }

//    public void SetCloseProximityWaveSoundsEnabled(bool value)
//    {
//      MBAPI.IMBMission.SetCloseProximityWaveSoundsEnabled(this.Pointer, value);
//    }

//    public void ForceDisableOcclusion(bool value)
//    {
//      MBAPI.IMBMission.ForceDisableOcclusion(this.Pointer, value);
//    }

//    public void AddFleePosition(FleePosition fleePosition)
//    {
//      BattleSideEnum side = fleePosition.GetSide();
//      switch (side)
//      {
//        case BattleSideEnum.None:
//          for (int index = 0; index < this._fleePositions.Length; ++index)
//            this._fleePositions[index].Add(fleePosition);
//          break;
//        case BattleSideEnum.NumSides:
//          TaleWorlds.Library.Debug.FailedAssert("Flee position with invalid battle side field found!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (AddFleePosition), 1374);
//          break;
//        default:
//          this._fleePositions[(int) (side + 1)].Add(fleePosition);
//          break;
//      }
//    }

//    private void FreeResources()
//    {
//      this.MainAgent = (Agent) null;
//      this.Teams.ClearResources();
//      this.SpectatorTeam = (Team) null;
//      this._activeAgents = (AgentList) null;
//      this._allAgents = (AgentList) null;
//      if (GameNetwork.NetworkPeersValid)
//      {
//        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
//        {
//          MissionPeer component1 = networkPeer.GetComponent<MissionPeer>();
//          if (component1 != null)
//          {
//            component1.ClearAllVisuals(true);
//            networkPeer.RemoveComponent((PeerComponent) component1);
//          }
//          MissionRepresentativeBase component2 = networkPeer.GetComponent<MissionRepresentativeBase>();
//          if (component2 != null)
//            networkPeer.RemoveComponent((PeerComponent) component2);
//        }
//      }
//      if (GameNetwork.DisconnectedNetworkPeers != null)
//      {
//        TaleWorlds.Library.Debug.Print("DisconnectedNetworkPeers.Clear()", debugFilter: 17179869184UL);
//        GameNetwork.DisconnectedNetworkPeers.Clear();
//      }
//      this._missionState = (MissionState) null;
//    }

//    public void RetreatMission()
//    {
//      foreach (MissionLogic missionLogic in this.MissionLogics)
//        missionLogic.OnRetreatMission();
//      if (MBEditor.EditModeEnabled && MBEditor.IsEditModeOn)
//        MBEditor.LeaveEditMissionMode();
//      else
//        this.EndMission();
//    }

//    public void SurrenderMission()
//    {
//      foreach (MissionLogic missionLogic in this.MissionLogics)
//        missionLogic.OnSurrenderMission();
//      if (MBEditor.EditModeEnabled && MBEditor.IsEditModeOn)
//        MBEditor.LeaveEditMissionMode();
//      else
//        this.EndMission();
//    }

//    public bool HasMissionBehavior<T>() where T : MissionBehavior
//    {
//      return (object) this.GetMissionBehavior<T>() != null;
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void OnCorpseRemoved(int corpsesToFadeIndex)
//    {
//      if (GameNetwork.IsClientOrReplay || this._corpseAgentInfos.Count <= 0)
//        return;
//      this._corpseAgentInfos.RemoveAt(0);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void OnAgentAddedAsCorpse(Agent affectedAgent, int corpsesToFadeIndex)
//    {
//      if (GameNetwork.IsClientOrReplay)
//        return;
//      Mission.CorpseAgentInfo corpseAgentInfo;
//      corpseAgentInfo.AttachedWeapons = new MBList<MissionWeapon>();
//      corpseAgentInfo.AttachedWeaponBoneIndices = new MBList<sbyte>();
//      corpseAgentInfo.AttachedWeaponFrames = new MBList<MatrixFrame>();
//      for (int index = 0; index < affectedAgent.GetAttachedWeaponsCount(); ++index)
//      {
//        MissionWeapon attachedWeapon = affectedAgent.GetAttachedWeapon(index);
//        if (attachedWeapon.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CanBePickedUpFromCorpse))
//        {
//          this.SpawnAttachedWeaponOnCorpse(affectedAgent, index, -1);
//          corpseAgentInfo.AttachedWeapons.Add(attachedWeapon);
//          corpseAgentInfo.AttachedWeaponBoneIndices.Add(affectedAgent.GetAttachedWeaponBoneIndex(index));
//          corpseAgentInfo.AttachedWeaponFrames.Add(affectedAgent.GetAttachedWeaponFrame(index));
//        }
//      }
//      corpseAgentInfo.CorpseBasicCharacterObject = affectedAgent.Character;
//      corpseAgentInfo.CorpseMonster = affectedAgent.Monster;
//      corpseAgentInfo.CorpseSpawnEquipment = affectedAgent.SpawnEquipment;
//      corpseAgentInfo.CorpseMissionEquipment = affectedAgent.Equipment;
//      corpseAgentInfo.CorpseBodyPropertiesValue = affectedAgent.BodyPropertiesValue;
//      corpseAgentInfo.CorpseBodyPropertiesSeed = affectedAgent.BodyPropertiesSeed;
//      corpseAgentInfo.CorpseIsFemale = affectedAgent.IsFemale;
//      ref Mission.CorpseAgentInfo local1 = ref corpseAgentInfo;
//      Team team = affectedAgent.Team;
//      int num1 = team != null ? team.TeamIndex : -1;
//      local1.CorpseTeamIndex = num1;
//      ref Mission.CorpseAgentInfo local2 = ref corpseAgentInfo;
//      Formation formation = affectedAgent.Formation;
//      int num2 = formation != null ? formation.Index : -1;
//      local2.CorpseFormationIndex = num2;
//      corpseAgentInfo.CorpseMissionPeer = affectedAgent.MissionPeer;
//      corpseAgentInfo.CorpseOwningAgentMissionPeer = affectedAgent.OwningAgentMissionPeer;
//      corpseAgentInfo.CorpsePosition = affectedAgent.Position;
//      corpseAgentInfo.CorpseMovementDirection = affectedAgent.GetMovementDirection();
//      corpseAgentInfo.AgentCorpsesToFadeIndex = corpsesToFadeIndex;
//      corpseAgentInfo.IsMount = affectedAgent.IsMount;
//      corpseAgentInfo.CorpseClothingColor1 = affectedAgent.IsHuman ? affectedAgent.ClothingColor1 : uint.MaxValue;
//      corpseAgentInfo.CorpseClothingColor2 = affectedAgent.IsHuman ? affectedAgent.ClothingColor2 : uint.MaxValue;
//      corpseAgentInfo.CorpseDeathActionIndex = affectedAgent.GetCurrentAction(0);
//      this._corpseAgentInfos.Add(corpseAgentInfo);
//      affectedAgent.ClearAttachedWeapons();
//    }

//    public void SpawnAttachedWeaponOnCorpse(
//      Agent agent,
//      int attachedWeaponIndex,
//      int forcedSpawnIndex)
//    {
//      agent.AgentVisuals.GetSkeleton()?.ForceUpdateBoneFrames();
//      MissionWeapon attachedWeapon = agent.GetAttachedWeapon(attachedWeaponIndex);
//      GameEntity attachedWeaponEntity = agent.AgentVisuals.GetAttachedWeaponEntity(attachedWeaponIndex);
//      attachedWeaponEntity.CreateAndAddScriptComponent(typeof (SpawnedItemEntity).Name, true);
//      SpawnedItemEntity firstScriptOfType = attachedWeaponEntity.GetFirstScriptOfType<SpawnedItemEntity>();
//      if (forcedSpawnIndex >= 0)
//        firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.SpawnAttachedWeaponOnCorpse(agent.Index, attachedWeaponIndex, firstScriptOfType.Id.Id));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      this.SpawnWeaponAux(attachedWeaponEntity.WeakEntity, attachedWeapon, Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithStaticPhysics, Vec3.Zero, Vec3.Zero, false);
//    }

//    public void AddMountWithoutRider(Agent mount)
//    {
//      this._mountsWithoutRiders.Add(new KeyValuePair<Agent, MissionTime>(mount, MissionTime.Now));
//    }

//    public void RemoveMountWithoutRider(Agent mount)
//    {
//      for (int index = 0; index < this._mountsWithoutRiders.Count; ++index)
//      {
//        if (this._mountsWithoutRiders[index].Key == mount)
//        {
//          this._mountsWithoutRiders.RemoveAt(index);
//          break;
//        }
//      }
//    }

//    public void UpdateMountReservationsAfterRiderMounts(Agent rider, Agent mount)
//    {
//      int selectedMountIndex = rider.GetSelectedMountIndex();
//      if (selectedMountIndex >= 0 && selectedMountIndex != mount.Index)
//      {
//        Agent agentWithIndex = Mission.Current.FindAgentWithIndex(selectedMountIndex);
//        if (agentWithIndex != null)
//          rider.HumanAIComponent.UnreserveMount(agentWithIndex);
//      }
//      int agentId = mount.CommonAIComponent != null ? mount.CommonAIComponent.ReservedRiderAgentIndex : -1;
//      if (agentId < 0)
//        return;
//      if (agentId == rider.Index)
//        rider.HumanAIComponent.UnreserveMount(mount);
//      else
//        Mission.Current.FindAgentWithIndex(agentId)?.HumanAIComponent.UnreserveMount(mount);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void OnAgentDeleted(Agent affectedAgent)
//    {
//      if (affectedAgent == null)
//        return;
//      affectedAgent.State = AgentState.Deleted;
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnAgentDeleted(affectedAgent);
//      this._allAgents.Remove(affectedAgent);
//      affectedAgent.OnDelete();
//      affectedAgent.SetTeam((Team) null, false);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void OnAgentRemoved(
//      Agent affectedAgent,
//      Agent affectorAgent,
//      AgentState agentState,
//      KillingBlow killingBlow)
//    {
//      Mission.OnBeforeAgentRemovedDelegate beforeAgentRemoved = this.OnBeforeAgentRemoved;
//      if (beforeAgentRemoved != null)
//        beforeAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
//      affectedAgent.State = agentState;
//      if (affectorAgent != null && affectorAgent.Team != affectedAgent.Team)
//        ++affectorAgent.KillCount;
//      affectedAgent.Team?.DeactivateAgent(affectedAgent);
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnEarlyAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
//      int num = this.MainAgent == affectedAgent ? 1 : 0;
//      if (num != 0)
//      {
//        affectedAgent.OnMainAgentWieldedItemChange = (Agent.OnMainAgentWieldedItemChangeDelegate) null;
//        this.MainAgent = (Agent) null;
//      }
//      affectedAgent.OnAgentWieldedItemChange = (Action) null;
//      affectedAgent.OnAgentMountedStateChanged = (Action) null;
//      if (affectedAgent.Team != null && affectedAgent.Team.Side != BattleSideEnum.None)
//        ++this._removedAgentCountPerSide[(int) affectedAgent.Team.Side];
//      this._activeAgents.Remove(affectedAgent);
//      affectedAgent.OnRemove();
//      if (affectedAgent.IsMount && affectedAgent.RiderAgent == null)
//        this.RemoveMountWithoutRider(affectedAgent);
//      if (num != 0)
//        affectedAgent.Team.DelegateCommandToAI();
//      if (GameNetwork.IsClientOrReplay || agentState == AgentState.Routed || !affectedAgent.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.CanWieldWeapon))
//        return;
//      EquipmentIndex wieldedItemIndex = affectedAgent.GetOffhandWieldedItemIndex();
//      if (wieldedItemIndex != EquipmentIndex.ExtraWeaponSlot)
//        return;
//      WeaponComponentData currentUsageItem = affectedAgent.Equipment[wieldedItemIndex].CurrentUsageItem;
//      if (currentUsageItem == null || currentUsageItem.WeaponClass != WeaponClass.Banner)
//        return;
//      affectedAgent.DropItem(EquipmentIndex.ExtraWeaponSlot);
//    }

//    public void OnObjectDisabled(DestructableComponent destructionComponent)
//    {
//      destructionComponent.GameEntity.GetFirstScriptOfType<UsableMachine>()?.Disable();
//      destructionComponent?.SetAbilityOfFaces(false);
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnObjectDisabled(destructionComponent);
//    }

//    public MissionObjectId SpawnWeaponAsDropFromMissile(
//      int missileIndex,
//      MissionObject attachedMissionObject,
//      in MatrixFrame attachLocalFrame,
//      Mission.WeaponSpawnFlags spawnFlags,
//      in Vec3 velocity,
//      in Vec3 angularVelocity,
//      int forcedSpawnIndex)
//    {
//      this.PrepareMissileWeaponForDrop(missileIndex);
//      Mission.Missile missiles = this._missilesDictionary[missileIndex];
//      attachedMissionObject?.AddStuckMissile(missiles.Entity);
//      if (attachedMissionObject != null)
//        missiles.Entity.SetGlobalFrame(attachedMissionObject.GameEntity.GetGlobalFrame().TransformToParent(in attachLocalFrame));
//      else
//        missiles.Entity.SetGlobalFrame(in attachLocalFrame);
//      missiles.Entity.CreateAndAddScriptComponent(typeof (SpawnedItemEntity).Name, true);
//      SpawnedItemEntity firstScriptOfType = missiles.Entity.GetFirstScriptOfType<SpawnedItemEntity>();
//      if (forcedSpawnIndex >= 0)
//        firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
//      this.SpawnWeaponAux(missiles.Entity.WeakEntity, missiles.Weapon, spawnFlags, velocity, angularVelocity, true);
//      return firstScriptOfType.Id;
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void SpawnWeaponAsDropFromAgent(
//      Agent agent,
//      EquipmentIndex equipmentIndex,
//      ref Vec3 globalVelocity,
//      ref Vec3 globalAngularVelocity,
//      Mission.WeaponSpawnFlags spawnFlags)
//    {
//      this.SpawnWeaponAsDropFromAgentAux(agent, equipmentIndex, ref globalVelocity, ref globalAngularVelocity, spawnFlags, -1);
//    }

//    public void SpawnWeaponAsDropFromAgentAux(
//      Agent agent,
//      EquipmentIndex equipmentIndex,
//      ref Vec3 globalVelocity,
//      ref Vec3 globalAngularVelocity,
//      Mission.WeaponSpawnFlags spawnFlags,
//      int forcedSpawnIndex)
//    {
//      agent.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
//      agent.PrepareWeaponForDropInEquipmentSlot(equipmentIndex, (spawnFlags & Mission.WeaponSpawnFlags.WithHolster) > Mission.WeaponSpawnFlags.None);
//      WeakGameEntity fromEquipmentSlot = agent.GetWeaponEntityFromEquipmentSlot(equipmentIndex);
//      fromEquipmentSlot.CreateAndAddScriptComponent(typeof (SpawnedItemEntity).Name, true);
//      SpawnedItemEntity firstScriptOfType = fromEquipmentSlot.GetFirstScriptOfType<SpawnedItemEntity>();
//      if (forcedSpawnIndex >= 0)
//        firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
//      CompressionMission.SpawnedItemVelocityCompressionInfo.ClampValueAccordingToLimits(ref globalVelocity.x);
//      CompressionMission.SpawnedItemVelocityCompressionInfo.ClampValueAccordingToLimits(ref globalVelocity.y);
//      CompressionMission.SpawnedItemVelocityCompressionInfo.ClampValueAccordingToLimits(ref globalVelocity.z);
//      CompressionMission.SpawnedItemAngularVelocityCompressionInfo.ClampValueAccordingToLimits(ref globalAngularVelocity.x);
//      CompressionMission.SpawnedItemAngularVelocityCompressionInfo.ClampValueAccordingToLimits(ref globalAngularVelocity.y);
//      CompressionMission.SpawnedItemAngularVelocityCompressionInfo.ClampValueAccordingToLimits(ref globalAngularVelocity.z);
//      MissionWeapon weapon = agent.Equipment[equipmentIndex];
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.SpawnWeaponAsDropFromAgent(agent.Index, equipmentIndex, globalVelocity, globalAngularVelocity, spawnFlags, firstScriptOfType.Id.Id));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      agent.OnWeaponDrop(equipmentIndex);
//      this.SpawnWeaponAux(fromEquipmentSlot, weapon, spawnFlags, globalVelocity, globalAngularVelocity, true);
//      if (!GameNetwork.IsClientOrReplay)
//      {
//        for (int attachmentIndex = 0; attachmentIndex < weapon.GetAttachedWeaponsCount(); ++attachmentIndex)
//        {
//          if (weapon.GetAttachedWeapon(attachmentIndex).Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CanBePickedUpFromCorpse))
//            this.SpawnAttachedWeaponOnSpawnedWeapon(firstScriptOfType, attachmentIndex, -1);
//        }
//      }
//      Action<Agent, SpawnedItemEntity> onItemDrop = this.OnItemDrop;
//      if (onItemDrop == null)
//        return;
//      onItemDrop(agent, firstScriptOfType);
//    }

//    public void SpawnAttachedWeaponOnSpawnedWeapon(
//      SpawnedItemEntity spawnedWeapon,
//      int attachmentIndex,
//      int forcedSpawnIndex)
//    {
//      WeakGameEntity child = spawnedWeapon.GameEntity.GetChild(attachmentIndex);
//      child.CreateAndAddScriptComponent(typeof (SpawnedItemEntity).Name, true);
//      SpawnedItemEntity firstScriptOfType = child.GetFirstScriptOfType<SpawnedItemEntity>();
//      if (forcedSpawnIndex >= 0)
//        firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
//      this.SpawnWeaponAux(child, spawnedWeapon.WeaponCopy.GetAttachedWeapon(attachmentIndex), Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithStaticPhysics, Vec3.Zero, Vec3.Zero, false);
//      if (!GameNetwork.IsServerOrRecorder)
//        return;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.SpawnAttachedWeaponOnSpawnedWeapon(spawnedWeapon.Id, attachmentIndex, firstScriptOfType.Id.Id));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//    }

//    public GameEntity SpawnWeaponWithNewEntity(
//      ref MissionWeapon weapon,
//      Mission.WeaponSpawnFlags spawnFlags,
//      MatrixFrame frame)
//    {
//      return this.SpawnWeaponWithNewEntityAux(weapon, spawnFlags, frame, -1, (MissionObject) null, false);
//    }

//    public GameEntity SpawnWeaponWithNewEntityAux(
//      MissionWeapon weapon,
//      Mission.WeaponSpawnFlags spawnFlags,
//      MatrixFrame frame,
//      int forcedSpawnIndex,
//      MissionObject attachedMissionObject,
//      bool hasLifeTime)
//    {
//      GameEntity gameEntity = GameEntityExtensions.Instantiate(this.Scene, weapon, spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithHolster), true);
//      gameEntity.CreateAndAddScriptComponent(typeof (SpawnedItemEntity).Name, true);
//      SpawnedItemEntity firstScriptOfType = gameEntity.GetFirstScriptOfType<SpawnedItemEntity>();
//      if (forcedSpawnIndex >= 0)
//        firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
//      attachedMissionObject?.GameEntity.AddChild(gameEntity.WeakEntity);
//      if (attachedMissionObject != null)
//      {
//        MatrixFrame frame1 = attachedMissionObject.GameEntity.GetGlobalFrame().TransformToParent(in frame);
//        if (!frame1.rotation.IsOrthonormal())
//          frame1.rotation.Orthonormalize();
//        gameEntity.SetGlobalFrame(in frame1);
//      }
//      else
//        gameEntity.SetGlobalFrame(in frame);
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.SpawnWeaponWithNewEntity(weapon, spawnFlags, firstScriptOfType.Id.Id, frame, attachedMissionObject != null ? attachedMissionObject.Id : MissionObjectId.Invalid, true, hasLifeTime));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        for (int attachmentIndex = 0; attachmentIndex < weapon.GetAttachedWeaponsCount(); ++attachmentIndex)
//        {
//          GameNetwork.BeginBroadcastModuleEvent();
//          GameNetwork.WriteMessage((GameNetworkMessage) new AttachWeaponToSpawnedWeapon(weapon.GetAttachedWeapon(attachmentIndex), firstScriptOfType.Id, weapon.GetAttachedWeaponFrame(attachmentIndex)));
//          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        }
//      }
//      Vec3 zero = Vec3.Zero;
//      this.SpawnWeaponAux(gameEntity.WeakEntity, weapon, spawnFlags, zero, zero, hasLifeTime);
//      return gameEntity;
//    }

//    public void AttachWeaponWithNewEntityToSpawnedWeapon(
//      MissionWeapon weapon,
//      SpawnedItemEntity spawnedItem,
//      MatrixFrame attachLocalFrame)
//    {
//      GameEntity gameEntity = GameEntityExtensions.Instantiate(this.Scene, weapon, false, true);
//      spawnedItem.GameEntity.AddChild(gameEntity.WeakEntity);
//      gameEntity.SetFrame(ref attachLocalFrame);
//      spawnedItem.AttachWeaponToWeapon(weapon, ref attachLocalFrame);
//    }

//    private void SpawnWeaponAux(
//      WeakGameEntity weaponEntity,
//      MissionWeapon weapon,
//      Mission.WeaponSpawnFlags spawnFlags,
//      Vec3 globalVelocity,
//      Vec3 globalAngularVelocity,
//      bool hasLifeTime)
//    {
//      SpawnedItemEntity firstScriptOfType = weaponEntity.GetFirstScriptOfType<SpawnedItemEntity>();
//      bool flag = weapon.IsBanner();
//      MissionWeapon weapon1 = weapon;
//      int num = !flag & hasLifeTime ? 1 : 0;
//      int spawnFlags1 = (int) spawnFlags;
//      // ISSUE: explicit reference operation
//      ref Vec3 local = @(flag ? globalVelocity : Vec3.Zero);
//      firstScriptOfType.Initialize(weapon1, num != 0, (Mission.WeaponSpawnFlags) spawnFlags1, in local);
//      if (!spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithPhysics | Mission.WeaponSpawnFlags.WithStaticPhysics))
//        return;
//      BodyFlags bodyFlags1 = BodyFlags.OnlyCollideWithRaycast | BodyFlags.DroppedItem;
//      if (weapon.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CannotBePickedUp) || spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.CannotBePickedUp))
//        bodyFlags1 |= BodyFlags.DoNotCollideWithRaycast;
//      BodyFlags bodyFlags2 = bodyFlags1 | BodyFlags.Moveable;
//      weaponEntity.AddBodyFlags(bodyFlags2, false);
//      WeaponData weaponData = weapon.GetWeaponData(true);
//      this.RecalculateBody(ref weaponData, weapon.Item.ItemComponent, weapon.Item.WeaponDesign, ref spawnFlags);
//      int collisionGroupID = -1;
//      if (flag)
//        weaponEntity.AddPhysics(weaponData.BaseWeight, weaponData.CenterOfMassShift, weaponData.Shape, globalVelocity, globalAngularVelocity, PhysicsMaterial.GetFromIndex(weaponData.PhysicsMaterialIndex), true, collisionGroupID);
//      else if (spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithPhysics | Mission.WeaponSpawnFlags.WithStaticPhysics))
//      {
//        float mass = spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithHolster) ? weaponData.BaseWeight * (float) weapon.MaxAmmo : weaponData.BaseWeight;
//        weaponEntity.AddPhysics(mass, weaponData.CenterOfMassShift, weaponData.Shape, globalVelocity, globalAngularVelocity, PhysicsMaterial.GetFromIndex(weaponData.PhysicsMaterialIndex), spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithStaticPhysics), collisionGroupID);
//        if (weaponEntity.Parent != WeakGameEntity.Invalid && spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithStaticPhysics))
//        {
//          weaponEntity.SetPhysicsMoveToBatched(true);
//          weaponEntity.ConvertDynamicBodyToRayCast();
//        }
//        else
//          weaponEntity.SetPhysicsStateOnlyVariable(true, true);
//      }
//      weaponData.DeinitializeManagedPointers();
//    }

//    public void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
//    {
//      foreach (IMissionListener listener in this._listeners)
//        listener.OnEquipItemsFromSpawnEquipmentBegin(agent, creationType);
//    }

//    public void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
//    {
//      foreach (IMissionListener listener in this._listeners)
//        listener.OnEquipItemsFromSpawnEquipment(agent, creationType);
//    }

//    public static int GetCurrentVolumeGeneratorVersion()
//    {
//      return MBAPI.IMBMission.GetCurrentVolumeGeneratorVersion();
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("flee_enemies", "mission")]
//    public static string MakeEnemiesFleeCheat(List<string> strings)
//    {
//      Game current = Game.Current;
//      if ((current != null ? (current.CheatMode ? 1 : 0) : 0) == 0)
//        return "Cheat mode is not enabled.";
//      if (GameNetwork.IsClientOrReplay)
//        return "does not work in multiplayer";
//      if (Mission.Current == null || Mission.Current.Agents == null)
//        return "mission is not available";
//      foreach (Agent agent in Mission.Current.Agents.Where<Agent>((Func<Agent, bool>) (agent => agent.IsHuman && agent.IsActive() && agent.Team.IsEnemyOf(Mission.Current.PlayerTeam))))
//        agent.CommonAIComponent?.Panic();
//      return "enemies are fleeing";
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("flee_team", "mission")]
//    public static string MakeTeamFleeCheat(List<string> strings)
//    {
//      if (GameNetwork.IsClientOrReplay)
//        return "does not work in multiplayer";
//      if (Mission.Current == null || Mission.Current.Agents == null)
//        return "mission is not available";
//      string str1 = "Usage 1: flee_team [ Attacker | AttackerAlly | Defender | DefenderAlly ]\nUsage 2: flee_team [ Attacker | AttackerAlly | Defender | DefenderAlly ] [FormationNo]";
//      if (strings.IsEmpty<string>() || strings[0] == "help")
//        return "makes an entire team or a team's formation flee battle.\n" + str1;
//      if (strings.Count >= 3)
//        return "invalid number of parameters.\n" + str1;
//      string str2 = strings[0];
//      Team targetTeam = (Team) null;
//      switch (str2.ToLower())
//      {
//        case "attacker":
//          targetTeam = Mission.Current.AttackerTeam;
//          break;
//        case "attackerally":
//          targetTeam = Mission.Current.AttackerAllyTeam;
//          break;
//        case "defender":
//          targetTeam = Mission.Current.DefenderTeam;
//          break;
//        case "defenderally":
//          targetTeam = Mission.Current.DefenderAllyTeam;
//          break;
//      }
//      if (targetTeam == null)
//        return "given team is not valid";
//      Formation targetFormation = (Formation) null;
//      if (strings.Count == 2)
//      {
//        int num1 = 8;
//        int num2 = int.Parse(strings[1]);
//        if (num2 < 0 || num2 >= num1)
//          return "invalid formation index. formation index should be between [0, " + (object) (num1 - 1) + "]";
//        FormationClass formationIndex = (FormationClass) num2;
//        targetFormation = targetTeam.GetFormation(formationIndex);
//      }
//      if (targetFormation == null)
//      {
//        foreach (Agent agent in Mission.Current.Agents.Where<Agent>((Func<Agent, bool>) (agent => agent.IsHuman && agent.Team == targetTeam)))
//          agent.CommonAIComponent?.Panic();
//        return "agents in team: " + str2 + " are fleeing";
//      }
//      foreach (Agent agent in Mission.Current.Agents.Where<Agent>((Func<Agent, bool>) (agent => agent.IsHuman && agent.Formation == targetFormation)))
//        agent.CommonAIComponent?.Panic();
//      return "agents in team: " + str2 + " and formation: " + (object) (int) targetFormation.FormationIndex + " (" + targetFormation.FormationIndex.ToString() + ") are fleeing";
//    }

//    public void RecalculateBody(
//      ref WeaponData weaponData,
//      ItemComponent itemComponent,
//      WeaponDesign craftedWeaponData,
//      ref Mission.WeaponSpawnFlags spawnFlags)
//    {
//      WeaponComponent weaponComponent = (WeaponComponent) itemComponent;
//      ItemObject itemObject = weaponComponent.Item;
//      weaponData.Shape = !spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithHolster) ? (string.IsNullOrEmpty(itemObject.BodyName) ? (PhysicsShape) null : PhysicsShape.GetFromResource(itemObject.BodyName)) : (string.IsNullOrEmpty(itemObject.HolsterBodyName) ? (PhysicsShape) null : PhysicsShape.GetFromResource(itemObject.HolsterBodyName));
//      PhysicsShape physicsShape = weaponData.Shape;
//      if ((NativeObject) physicsShape == (NativeObject) null)
//      {
//        TaleWorlds.Library.Debug.FailedAssert("Item has no body! Applying a default body, but this should not happen! Check this!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (RecalculateBody), 2235);
//        physicsShape = PhysicsShape.GetFromResource("bo_axe_short");
//      }
//      if (!weaponComponent.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DoNotScaleBodyAccordingToWeaponLength))
//      {
//        if (spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithHolster) || !itemObject.RecalculateBody)
//        {
//          weaponData.Shape = physicsShape;
//        }
//        else
//        {
//          PhysicsShape copy = physicsShape.CreateCopy();
//          weaponData.Shape = copy;
//          float num1 = (float) weaponComponent.PrimaryWeapon.WeaponLength * 0.01f;
//          if (craftedWeaponData != (WeaponDesign) null)
//          {
//            copy.Clear();
//            copy.InitDescription();
//            float num2 = 0.0f;
//            float num3 = 0.0f;
//            float z1 = 0.0f;
//            for (int index = 0; index < craftedWeaponData.UsedPieces.Length; ++index)
//            {
//              WeaponDesignElement usedPiece = craftedWeaponData.UsedPieces[index];
//              if (usedPiece.IsValid)
//              {
//                float scaledPieceOffset = usedPiece.ScaledPieceOffset;
//                double piecePivotDistance = (double) craftedWeaponData.PiecePivotDistances[index];
//                float a = (float) piecePivotDistance + scaledPieceOffset - usedPiece.ScaledDistanceToPreviousPiece;
//                float num4 = (float) piecePivotDistance - scaledPieceOffset + usedPiece.ScaledDistanceToNextPiece;
//                num2 = MathF.Min(a, num2);
//                if ((double) num4 > (double) num3)
//                {
//                  num3 = num4;
//                  z1 = (float) (((double) num4 + (double) a) * 0.5);
//                }
//              }
//            }
//            WeaponDesignElement usedPiece1 = craftedWeaponData.UsedPieces[2];
//            if (usedPiece1.IsValid)
//            {
//              float scaledPieceOffset = usedPiece1.ScaledPieceOffset;
//              num2 -= scaledPieceOffset;
//            }
//            copy.AddCapsule(new CapsuleData(0.035f, new Vec3(z: craftedWeaponData.CraftedWeaponLength), new Vec3(z: num2)));
//            bool flag = false;
//            if (craftedWeaponData.UsedPieces[1].IsValid)
//            {
//              float piecePivotDistance = craftedWeaponData.PiecePivotDistances[1];
//              copy.AddCapsule(new CapsuleData(0.05f, new Vec3(-0.1f, z: piecePivotDistance), new Vec3(0.1f, z: piecePivotDistance)));
//              flag = true;
//            }
//            if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.OneHandedAxe || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedAxe || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.ThrowingAxe)
//            {
//              WeaponDesignElement usedPiece2 = craftedWeaponData.UsedPieces[0];
//              double piecePivotDistance = (double) craftedWeaponData.PiecePivotDistances[0];
//              float z2 = (float) (piecePivotDistance + (double) usedPiece2.CraftingPiece.Length * 0.800000011920929);
//              float z3 = (float) (piecePivotDistance - (double) usedPiece2.CraftingPiece.Length * 0.800000011920929);
//              float z4 = (float) piecePivotDistance + usedPiece2.CraftingPiece.Length;
//              float z5 = (float) piecePivotDistance - usedPiece2.CraftingPiece.Length;
//              float bladeWidth = usedPiece2.CraftingPiece.BladeData.BladeWidth;
//              copy.AddCapsule(new CapsuleData(0.05f, new Vec3(z: z2), new Vec3(-bladeWidth, z: z4)));
//              copy.AddCapsule(new CapsuleData(0.05f, new Vec3(z: z3), new Vec3(-bladeWidth, z: z5)));
//              copy.AddCapsule(new CapsuleData(0.05f, new Vec3(-bladeWidth, z: z4), new Vec3(-bladeWidth, z: z5)));
//              flag = true;
//            }
//            if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedPolearm || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.Javelin)
//            {
//              float piecePivotDistance = craftedWeaponData.PiecePivotDistances[0];
//              copy.AddCapsule(new CapsuleData(0.025f, new Vec3(-0.05f, z: piecePivotDistance), new Vec3(0.05f, z: piecePivotDistance)));
//              flag = true;
//            }
//            if (!flag)
//              copy.AddCapsule(new CapsuleData(0.025f, new Vec3(-0.05f, z: z1), new Vec3(0.05f, z: z1)));
//          }
//          else
//          {
//            weaponData.Shape.Prepare();
//            int num5 = physicsShape.CapsuleCount();
//            if (num5 == 0)
//            {
//              TaleWorlds.Library.Debug.FailedAssert("Item has 0 body parts. Applying a default body, but this should not happen! Check this!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (RecalculateBody), 2373);
//              return;
//            }
//            switch (weaponComponent.PrimaryWeapon.WeaponClass)
//            {
//              case WeaponClass.Dagger:
//              case WeaponClass.OneHandedSword:
//              case WeaponClass.TwoHandedSword:
//              case WeaponClass.ThrowingKnife:
//                CapsuleData data1 = new CapsuleData();
//                copy.GetCapsule(ref data1, 0);
//                float radius1 = data1.Radius;
//                Vec3 p1_1 = data1.P1;
//                Vec3 p2_1 = data1.P2;
//                copy.SetCapsule(new CapsuleData(radius1, new Vec3(p1_1.x, p1_1.y, p1_1.z * num1), p2_1), 0);
//                break;
//              case WeaponClass.OneHandedAxe:
//              case WeaponClass.TwoHandedAxe:
//              case WeaponClass.Mace:
//              case WeaponClass.TwoHandedMace:
//              case WeaponClass.OneHandedPolearm:
//              case WeaponClass.TwoHandedPolearm:
//              case WeaponClass.LowGripPolearm:
//              case WeaponClass.Arrow:
//              case WeaponClass.Bolt:
//              case WeaponClass.Crossbow:
//              case WeaponClass.ThrowingAxe:
//              case WeaponClass.Javelin:
//              case WeaponClass.Banner:
//                CapsuleData data2 = new CapsuleData();
//                copy.GetCapsule(ref data2, 0);
//                float radius2 = data2.Radius;
//                Vec3 p1_2 = data2.P1;
//                Vec3 p2_2 = data2.P2;
//                copy.SetCapsule(new CapsuleData(radius2, new Vec3(p1_2.x, p1_2.y, p1_2.z * num1), p2_2), 0);
//                for (int index = 1; index < num5; ++index)
//                {
//                  CapsuleData data3 = new CapsuleData();
//                  copy.GetCapsule(ref data3, index);
//                  float radius3 = data3.Radius;
//                  Vec3 p1_3 = data3.P1;
//                  Vec3 p2_3 = data3.P2;
//                  copy.SetCapsule(new CapsuleData(radius3, new Vec3(p1_3.x, p1_3.y, p1_3.z * num1), new Vec3(p2_3.x, p2_3.y, p2_3.z * num1)), index);
//                }
//                break;
//              case WeaponClass.SmallShield:
//              case WeaponClass.LargeShield:
//                TaleWorlds.Library.Debug.FailedAssert("Shields should not have recalculate body flag.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (RecalculateBody), 2447);
//                break;
//            }
//          }
//        }
//      }
//      weaponData.CenterOfMassShift = weaponData.Shape.GetWeaponCenterOfMass();
//    }

//    [UsedImplicitly]
//    [MBCallback(null, true)]
//    internal void OnFixedTick(float fixedDt)
//    {
//      for (int index = this.MissionBehaviors.Count - 1; index >= 0; --index)
//        this.MissionBehaviors[index].OnFixedMissionTick(fixedDt);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void OnPreTick(float dt)
//    {
//      this.WaitTickCompletion();
//      for (int index = this.MissionBehaviors.Count - 1; index >= 0; --index)
//        this.MissionBehaviors[index].OnPreMissionTick(dt);
//      this.TickDebugAgents();
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void ApplySkeletonScaleToAllEquippedItems(string itemName)
//    {
//      int count = this.Agents.Count;
//      for (int index1 = 0; index1 < count; ++index1)
//      {
//        for (int index2 = 0; index2 < 12; ++index2)
//        {
//          EquipmentElement equipmentElement = this.Agents[index1].SpawnEquipment[index2];
//          if (!equipmentElement.IsEmpty && equipmentElement.Item.StringId == itemName && equipmentElement.Item.HorseComponent?.SkeletonScale != null)
//          {
//            this.Agents[index1].AgentVisuals.ApplySkeletonScale(equipmentElement.Item.HorseComponent.SkeletonScale.MountSitBoneScale, equipmentElement.Item.HorseComponent.SkeletonScale.MountRadiusAdder, equipmentElement.Item.HorseComponent.SkeletonScale.BoneIndices, equipmentElement.Item.HorseComponent.SkeletonScale.Scales);
//            break;
//          }
//        }
//      }
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("set_facial_anim_to_agent", "mission")]
//    public static string SetFacialAnimToAgent(List<string> strings)
//    {
//      Mission current = Mission.Current;
//      if (current == null)
//        return "Mission could not be found";
//      if (strings.Count != 2)
//        return "Enter agent index and animation name please";
//      int result;
//      if (int.TryParse(strings[0], out result) && result >= 0)
//      {
//        foreach (Agent agent in (List<Agent>) current.Agents)
//        {
//          if (agent.Index == result)
//          {
//            agent.SetAgentFacialAnimation(Agent.FacialAnimChannel.High, strings[1], true);
//            return "Done";
//          }
//        }
//      }
//      return "Please enter a valid agent index";
//    }

//    private void WaitTickCompletion()
//    {
//      while (!this.tickCompleted)
//        Thread.Sleep(1);
//    }

//    private void AgentTickMT(int startInclusive, int endExclusive, float dt)
//    {
//      for (int index = startInclusive; index < endExclusive; ++index)
//        this.AllAgents[index].TickParallel(dt);
//    }

//    public void TickAgentsAndTeamsImp(float dt, bool tickPaused)
//    {
//      float num = tickPaused ? 0.0f : dt;
//      TWParallel.For(0, this.AllAgents.Count, num, new TWParallel.ParallelForWithDtAuxPredicate(this.AgentTickMT));
//      foreach (Agent allAgent in (List<Agent>) this.AllAgents)
//        allAgent.Tick(num);
//      foreach (Team team in (List<Team>) this.Teams)
//        team.Tick(dt);
//      this.tickCompleted = true;
//      foreach (MBSubModuleBase cachedSubModule in (List<MBSubModuleBase>) this._cachedSubModuleList)
//        cachedSubModule.AfterAsyncTickTick(dt);
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("formation_speed_adjustment_enabled", "ai")]
//    public static string EnableSpeedAdjustmentCommand(List<string> strings)
//    {
//      if (GameNetwork.IsSessionActive)
//        return "Does not work on multiplayer.";
//      HumanAIComponent.FormationSpeedAdjustmentEnabled = !HumanAIComponent.FormationSpeedAdjustmentEnabled;
//      string str = "Speed Adjustment ";
//      return !HumanAIComponent.FormationSpeedAdjustmentEnabled ? str + "disabled" : str + "enabled";
//    }

//    public void OnTick(float dt, float realDt, bool updateCamera, bool doAsyncAITick)
//    {
//      this.ApplyGeneratedCombatLogs();
//      if (this.InputManager == null)
//        this.InputManager = (IInputContext) new EmptyInputContext();
//      for (int index = 0; index < this._tickActions.Count; ++index)
//      {
//        (Mission.MissionTickAction Action, Agent Agent, int Param1, int Param2) tickAction = this._tickActions[index];
//        Agent agent = tickAction.Agent;
//        if (agent.IsActive())
//        {
//          switch (tickAction.Action)
//          {
//            case Mission.MissionTickAction.TryToSheathWeaponInHand:
//              agent.TryToSheathWeaponInHand((Agent.HandIndex) tickAction.Param1, (Agent.WeaponWieldActionType) tickAction.Param2);
//              continue;
//            case Mission.MissionTickAction.RemoveEquippedWeapon:
//              agent.RemoveEquippedWeapon((EquipmentIndex) tickAction.Param1);
//              continue;
//            case Mission.MissionTickAction.TryToWieldWeaponInSlot:
//              agent.TryToWieldWeaponInSlot((EquipmentIndex) tickAction.Param1, (Agent.WeaponWieldActionType) tickAction.Param2, false);
//              continue;
//            case Mission.MissionTickAction.DropItem:
//              if (!agent.Equipment[tickAction.Param1].IsEmpty)
//              {
//                agent.DropItem((EquipmentIndex) tickAction.Param1);
//                continue;
//              }
//              continue;
//            case Mission.MissionTickAction.RegisterDrownBlow:
//              Blow blow = new Blow(agent.Index);
//              blow.DamageType = DamageTypes.Blunt;
//              blow.BoneIndex = agent.Monster.HeadLookDirectionBoneIndex;
//              blow.BaseMagnitude = 10f;
//              blow.GlobalPosition = agent.Position;
//              blow.GlobalPosition.z += agent.GetEyeGlobalHeight();
//              blow.DamagedPercentage = 1f;
//              blow.WeaponRecord.FillAsMeleeBlow((ItemObject) null, (WeaponComponentData) null, -1, (sbyte) -1);
//              blow.SwingDirection = agent.LookDirection;
//              blow.Direction = blow.SwingDirection;
//              blow.InflictedDamage = 10;
//              blow.DamageCalculated = true;
//              sbyte handItemBoneIndex = agent.Monster.MainHandItemBoneIndex;
//              AttackCollisionData collisionData = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Head, handItemBoneIndex, Agent.UsageDirection.AttackLeft, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, Vec3.Up, blow.Direction, blow.GlobalPosition, Vec3.Zero, Vec3.Zero, agent.Velocity, Vec3.Up);
//              agent.RegisterBlow(blow, in collisionData);
//              agent.MakeVoice(SkinVoiceManager.VoiceType.Drown, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//              if (agent.Controller == AgentControllerType.AI)
//              {
//                agent.AddAcceleration(new Vec3(z: -20f));
//                continue;
//              }
//              continue;
//            default:
//              continue;
//          }
//        }
//      }
//      this._tickActions.Clear();
//      this.MissionTimeTracker.Tick(dt);
//      this.CheckMissionEnd(this.CurrentTime);
//      if (this.IsFastForward && this.MissionEnded)
//        this.IsFastForward = false;
//      if (this.CurrentState != Mission.State.Continuing)
//        return;
//      if (this._inMissionLoadingScreenTimer != null && this._inMissionLoadingScreenTimer.Check(this.CurrentTime))
//      {
//        this._inMissionLoadingScreenTimer = (TaleWorlds.Core.Timer) null;
//        Action loadingEndedAction = this._onLoadingEndedAction;
//        if (loadingEndedAction != null)
//          loadingEndedAction();
//        LoadingWindow.DisableGlobalLoadingWindow();
//      }
//      for (int index = this.MissionBehaviors.Count - 1; index >= 0; --index)
//        this.MissionBehaviors[index].OnPreDisplayMissionTick(dt);
//      if (!GameNetwork.IsDedicatedServer & updateCamera)
//        this._missionState.Handler.UpdateCamera(this, realDt);
//      this.tickCompleted = false;
//      for (int index = this.MissionBehaviors.Count - 1; index >= 0; --index)
//        this.MissionBehaviors[index].OnMissionTick(dt);
//      for (int index = this._dynamicEntities.Count - 1; index >= 0; --index)
//      {
//        Mission.DynamicEntityInfo dynamicEntity = this._dynamicEntities[index];
//        if (dynamicEntity.TimerToDisable.Check(this.CurrentTime))
//        {
//          dynamicEntity.Entity.RemoveEnginePhysics();
//          dynamicEntity.Entity.Remove(79);
//          this._dynamicEntities.RemoveAt(index);
//        }
//      }
//      this.HandleSpawnedItems();
//      DebugNetworkEventStatistics.EndTick(dt);
//      if (this.CurrentState == Mission.State.Continuing && this.IsFriendlyMission && !this.IsInPhotoMode)
//      {
//        if (this.InputManager.IsGameKeyDown(4))
//          this.OnEndMissionRequest();
//        else
//          this._leaveMissionTimer = (BasicMissionTimer) null;
//      }
//      if (doAsyncAITick)
//        this.TickAgentsAndTeamsAsync(dt);
//      else
//        this.TickAgentsAndTeamsImp(dt, false);
//    }

//    public void AddTickAction(
//      Mission.MissionTickAction action,
//      Agent agent,
//      int param1,
//      int param2)
//    {
//      this._tickActions.Add((action, agent, param1, param2));
//    }

//    public void AddTickActionMT(
//      Mission.MissionTickAction action,
//      Agent agent,
//      int param1,
//      int param2)
//    {
//      lock (this._tickActionsLock)
//        this._tickActions.Add((action, agent, param1, param2));
//    }

//    public void RemoveSpawnedItemsAndMissiles()
//    {
//      this.ClearMissiles();
//      this._missilesList.Clear();
//      this._missilesDictionary.Clear();
//      this.RemoveSpawnedMissionObjects();
//    }

//    public void AfterStart()
//    {
//      this._activeAgents.Clear();
//      this._allAgents.Clear();
//      this._tickActions.Clear();
//      this._cachedSubModuleList = Module.CurrentModule.CollectSubModules();
//      foreach (MBSubModuleBase cachedSubModule in (List<MBSubModuleBase>) this._cachedSubModuleList)
//        cachedSubModule.OnBeforeMissionBehaviorInitialize(this);
//      for (int index = 0; index < this.MissionBehaviors.Count; ++index)
//        this.MissionBehaviors[index].OnBehaviorInitialize();
//      foreach (MBSubModuleBase cachedSubModule in (List<MBSubModuleBase>) this._cachedSubModuleList)
//        cachedSubModule.OnMissionBehaviorInitialize(this);
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.EarlyStart();
//      this._battleSpawnPathSelector.Initialize();
//      this._deploymentPlan.Initialize();
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.AfterStart();
//      foreach (MissionObject missionObject in (List<MissionObject>) this.MissionObjects)
//        missionObject.AfterMissionStart();
//      if (MissionGameModels.Current.ApplyWeatherEffectsModel != null)
//        MissionGameModels.Current.ApplyWeatherEffectsModel.ApplyWeatherEffects();
//      this.CurrentState = Mission.State.Continuing;
//    }

//    public void OnEndMissionRequest()
//    {
//      foreach (MissionLogic missionLogic in this.MissionLogics)
//      {
//        bool canLeave;
//        InquiryData data = missionLogic.OnEndMissionRequest(out canLeave);
//        if (!canLeave)
//        {
//          this._leaveMissionTimer = (BasicMissionTimer) null;
//          return;
//        }
//        if (data != null)
//        {
//          this._leaveMissionTimer = (BasicMissionTimer) null;
//          InformationManager.ShowInquiry(data, true);
//          return;
//        }
//      }
//      if (this._leaveMissionTimer != null)
//      {
//        if ((double) this._leaveMissionTimer.ElapsedTime <= 0.60000002384185791)
//          return;
//        this._leaveMissionTimer = (BasicMissionTimer) null;
//        this.EndMission();
//      }
//      else
//        this._leaveMissionTimer = new BasicMissionTimer();
//    }

//    public float GetMissionEndTimeInSeconds() => 0.6f;

//    public float GetMissionEndTimerValue()
//    {
//      return this._leaveMissionTimer == null ? -1f : this._leaveMissionTimer.ElapsedTime;
//    }

//    private void ApplyGeneratedCombatLogs()
//    {
//      if (this._combatLogsCreated.IsEmpty)
//        return;
//      CombatLogData result;
//      while (this._combatLogsCreated.TryDequeue(out result))
//        CombatLogManager.GenerateCombatLog(result);
//    }

//    public int GetMemberCountOfSide(BattleSideEnum side)
//    {
//      int memberCountOfSide = 0;
//      foreach (Team team in (List<Team>) this.Teams)
//      {
//        if (team.Side == side)
//          memberCountOfSide += team.ActiveAgents.Count;
//      }
//      return memberCountOfSide;
//    }

//    public Path GetInitialSpawnPath() => this._battleSpawnPathSelector.InitialPath;

//    public SpawnPathData GetInitialSpawnPathData(BattleSideEnum battleSide)
//    {
//      SpawnPathData pathPathData;
//      this._battleSpawnPathSelector.GetInitialPathDataOfSide(battleSide, out pathPathData);
//      return pathPathData;
//    }

//    public MBReadOnlyList<SpawnPathData> GetReinforcementPathsDataOfSide(BattleSideEnum battleSide)
//    {
//      return this._battleSpawnPathSelector.GetReinforcementPathsDataOfSide(battleSide);
//    }

//    public void GetTroopSpawnFrameWithIndex(
//      AgentBuildData buildData,
//      int troopSpawnIndex,
//      int troopSpawnCount,
//      out Vec3 troopSpawnPosition,
//      out Vec2 troopSpawnDirection)
//    {
//      Formation agentFormation = buildData.AgentFormation;
//      BasicCharacterObject agentCharacter = buildData.AgentCharacter;
//      troopSpawnPosition = Vec3.Invalid;
//      WorldPosition formationPosition;
//      Vec2 formationDirection;
//      if (buildData.AgentSpawnsIntoOwnFormation)
//      {
//        formationPosition = agentFormation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.GroundVec3);
//        formationDirection = agentFormation.Direction;
//      }
//      else
//      {
//        IAgentOriginBase agentOrigin = buildData.AgentOrigin;
//        bool agentIsReinforcement = buildData.AgentIsReinforcement;
//        Team agentTeam = buildData.AgentTeam;
//        BattleSideEnum side = agentTeam.Side;
//        if (buildData.AgentSpawnsUsingOwnTroopClass)
//        {
//          FormationClass agentTroopClass = this.GetAgentTroopClass(side, agentCharacter);
//          this.GetFormationSpawnFrame(agentTeam, agentTroopClass, agentIsReinforcement, out formationPosition, out formationDirection);
//        }
//        else if (agentCharacter.IsHero && agentOrigin != null && agentOrigin.BattleCombatant != null && agentCharacter == agentOrigin.BattleCombatant.General && this.GetFormationSpawnClass(agentTeam, FormationClass.NumberOfRegularFormations, agentIsReinforcement) == FormationClass.NumberOfRegularFormations)
//          this.GetFormationSpawnFrame(agentTeam, FormationClass.NumberOfRegularFormations, agentIsReinforcement, out formationPosition, out formationDirection);
//        else
//          this.GetFormationSpawnFrame(agentTeam, agentFormation.FormationIndex, agentIsReinforcement, out formationPosition, out formationDirection);
//      }
//      bool isMountedFormation = !buildData.AgentNoHorses && agentFormation.HasAnyMountedUnit;
//      WorldPosition? unitSpawnPosition;
//      Vec2? unitSpawnDirection;
//      agentFormation.GetUnitSpawnFrameWithIndex(troopSpawnIndex, in formationPosition, in formationDirection, agentFormation.Width, troopSpawnCount, agentFormation.UnitSpacing, isMountedFormation, out unitSpawnPosition, out unitSpawnDirection);
//      if (unitSpawnPosition.HasValue && (double) buildData.MakeUnitStandOutDistance != 0.0)
//        unitSpawnPosition.Value.SetVec2(unitSpawnPosition.Value.AsVec2 + unitSpawnDirection.Value * buildData.MakeUnitStandOutDistance);
//      if (unitSpawnPosition.HasValue)
//        troopSpawnPosition = !(unitSpawnPosition.Value.GetNavMesh() == UIntPtr.Zero) ? unitSpawnPosition.Value.GetGroundVec3() : this.Scene.GetLastPointOnNavigationMeshFromWorldPositionToDestination(ref formationPosition, unitSpawnPosition.Value.AsVec2);
//      if (!troopSpawnPosition.IsValid)
//        troopSpawnPosition = formationPosition.GetGroundVec3();
//      troopSpawnDirection = unitSpawnDirection.HasValue ? unitSpawnDirection.Value : formationDirection;
//    }

//    public void GetFormationSpawnFrame(
//      Team team,
//      FormationClass formationClass,
//      bool isReinforcement,
//      out WorldPosition spawnPosition,
//      out Vec2 spawnDirection)
//    {
//      IFormationDeploymentPlan formationPlan = this._deploymentPlan.GetFormationPlan(team, formationClass, isReinforcement);
//      spawnPosition = formationPlan.CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.GroundVec3);
//      spawnDirection = formationPlan.GetDirection();
//    }

//    public WorldFrame GetSpawnPathFrame(
//      BattleSideEnum battleSide,
//      float pathOffset = 0.0f,
//      float targetOffset = 0.0f)
//    {
//      SpawnPathData initialSpawnPathData = this.GetInitialSpawnPathData(battleSide);
//      if (!initialSpawnPathData.IsValid)
//        return WorldFrame.Invalid;
//      Vec2 spawnPathPosition;
//      Vec2 spawnPathDirection;
//      initialSpawnPathData.GetSpawnPathFrameFacingTarget(pathOffset, targetOffset, false, out spawnPathPosition, out spawnPathDirection);
//      Mat3 identity = Mat3.Identity;
//      identity.RotateAboutUp(spawnPathDirection.RotationInRadians);
//      WorldPosition origin = new WorldPosition(this.Scene, UIntPtr.Zero, spawnPathPosition.ToVec3(), false);
//      return new WorldFrame(identity, origin);
//    }

//    private void BuildAgent(Agent agent, AgentBuildData agentBuildData)
//    {
//      if (agent == null)
//        throw new MBNullParameterException(nameof (agent));
//      agent.Build(agentBuildData);
//      if (!agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty)
//      {
//        EquipmentElement equipmentElement = agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot];
//        if (equipmentElement.Item.HorseComponent.BodyLength != 0)
//          agent.SetInitialAgentScale(0.01f * (float) equipmentElement.Item.HorseComponent.BodyLength);
//      }
//      agent.EquipItemsFromSpawnEquipment(true, agentBuildData != null && agentBuildData.PrepareImmediately || agent == Agent.Main);
//      agent.InitializeAgentRecord();
//      agent.AgentVisuals.BatchLastLodMeshes();
//      agent.PreloadForRendering();
//      ActionIndexCache actionIndexCache = agent.GetCurrentAction(0);
//      if (actionIndexCache != ActionIndexCache.act_none)
//        agent.SetActionChannel(0, in actionIndexCache, startProgress: MBRandom.RandomFloat * 0.8f);
//      agent.InitializeComponents();
//      if (agent.Controller == AgentControllerType.Player)
//        this.ResetFirstThirdPersonView();
//      this._activeAgents.Add(agent);
//      this._allAgents.Add(agent);
//    }

//    private Agent CreateAgent(
//      Monster monster,
//      bool isFemale,
//      int instanceNo,
//      Agent.CreationType creationType,
//      float stepSize,
//      int forcedAgentIndex,
//      int weight,
//      BasicCharacterObject characterObject)
//    {
//      AnimationSystemData animationSystemData = monster.FillAnimationSystemData(stepSize, false, isFemale);
//      AgentCapsuleData capsuleData = monster.FillCapsuleData();
//      AgentSpawnData spawnData = monster.FillSpawnData((ItemObject) null);
//      Agent agent = new Agent(this, this.CreateAgentInternal(monster.Flags, forcedAgentIndex, isFemale, ref spawnData, ref capsuleData, ref animationSystemData, instanceNo), creationType, monster, this._agentCreationIndex);
//      ++this._agentCreationIndex;
//      agent.Character = characterObject;
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnAgentCreated(agent);
//      return agent;
//    }

//    public void SetBattleAgentCount(int agentCount)
//    {
//      if (this._agentCount != 0 && this._agentCount <= agentCount)
//        return;
//      this._agentCount = agentCount;
//    }

//    public Vec2 GetFormationSpawnPosition(Team team, FormationClass formationClass)
//    {
//      return this._deploymentPlan.GetFormationPlan(team, formationClass).CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.None).AsVec2;
//    }

//    public FormationClass GetFormationSpawnClass(
//      Team team,
//      FormationClass formationClass,
//      bool isReinforcement = false)
//    {
//      return this._deploymentPlan.GetFormationPlan(team, formationClass, isReinforcement).SpawnClass;
//    }

//    public Agent SpawnAgent(AgentBuildData agentBuildData, bool spawnFromAgentVisuals = false)
//    {
//      this.Scene.WaitWaterRendererCPUSimulation();
//      BasicCharacterObject agentCharacter = agentBuildData.AgentCharacter;
//      if (agentCharacter == null)
//        throw new MBNullParameterException("npcCharacterObject");
//      int forcedAgentIndex = -1;
//      if (agentBuildData.AgentIndexOverriden)
//        forcedAgentIndex = agentBuildData.AgentIndex;
//      Agent agent1 = this.CreateAgent(agentBuildData.AgentMonster, agentBuildData.GenderOverriden ? agentBuildData.AgentIsFemale : agentCharacter.IsFemale, 0, Agent.CreationType.FromCharacterObj, agentCharacter.GetStepSize(), forcedAgentIndex, agentBuildData.AgentMonster.Weight, agentCharacter);
//      agent1.FormationPositionPreference = agentCharacter.FormationPositionPreference;
//      float age = agentBuildData.AgeOverriden ? (float) agentBuildData.AgentAge : agentCharacter.Age;
//      if ((double) age == 0.0)
//        agentBuildData.Age(29);
//      else if (MBBodyProperties.GetMaturityType(age) < BodyMeshMaturityType.Teenager && (this.Mode == MissionMode.Battle || this.Mode == MissionMode.Duel || this.Mode == MissionMode.Tournament || this.Mode == MissionMode.Stealth))
//        agentBuildData.Age(27);
//      if (agentBuildData.BodyPropertiesOverriden)
//      {
//        agent1.UpdateBodyProperties(agentBuildData.AgentBodyProperties);
//        if (!agentBuildData.AgeOverriden)
//          agent1.Age = agentCharacter.Age;
//      }
//      agent1.BodyPropertiesSeed = agentBuildData.AgentEquipmentSeed;
//      if (agentBuildData.AgeOverriden)
//        agent1.Age = (float) agentBuildData.AgentAge;
//      if (agentBuildData.GenderOverriden)
//        agent1.IsFemale = agentBuildData.AgentIsFemale;
//      agent1.SetTeam(agentBuildData.AgentTeam, false);
//      agent1.SetClothingColor1(agentBuildData.AgentClothingColor1);
//      agent1.SetClothingColor2(agentBuildData.AgentClothingColor2);
//      agent1.SetRandomizeColors(agentBuildData.RandomizeColors);
//      agent1.Origin = agentBuildData.AgentOrigin;
//      Formation agentFormation = agentBuildData.AgentFormation;
//      if (agentFormation != null && !agentFormation.HasBeenPositioned)
//      {
//        if (this._deploymentPlan.IsPlanMade(agentFormation.Team))
//        {
//          this.SetFormationPositioningFromDeploymentPlan(agentFormation);
//        }
//        else
//        {
//          WorldPosition worldPosition = new WorldPosition(this.Scene.Pointer, UIntPtr.Zero, agentBuildData.AgentInitialPosition.Value, false);
//          agentFormation.SetPositioning(new WorldPosition?(worldPosition));
//        }
//      }
//      if (!agentBuildData.AgentInitialPosition.HasValue)
//      {
//        Team agentTeam = agentBuildData.AgentTeam;
//        BattleSideEnum side = agentBuildData.AgentTeam.Side;
//        Vec3 position1 = Vec3.Invalid;
//        Vec2 direction1 = Vec2.Invalid;
//        if (agentCharacter == Game.Current.PlayerTroop && this._deploymentPlan.HasPlayerSpawnFrame(side))
//        {
//          WorldPosition position2;
//          Vec2 direction2;
//          this._deploymentPlan.GetPlayerSpawnFrame(side, out position2, out direction2);
//          position1 = position2.GetGroundVec3();
//          direction1 = direction2;
//        }
//        else if (agentFormation != null)
//        {
//          int troopSpawnIndex;
//          int troopSpawnCount;
//          if (agentBuildData.AgentSpawnsIntoOwnFormation)
//          {
//            troopSpawnIndex = agentFormation.CountOfUnits;
//            troopSpawnCount = troopSpawnIndex + 1;
//          }
//          else if (agentBuildData.AgentFormationTroopSpawnIndex >= 0 && agentBuildData.AgentFormationTroopSpawnCount > 0)
//          {
//            troopSpawnIndex = agentBuildData.AgentFormationTroopSpawnIndex;
//            troopSpawnCount = agentBuildData.AgentFormationTroopSpawnCount;
//          }
//          else
//          {
//            troopSpawnIndex = agentFormation.GetNextSpawnIndex();
//            troopSpawnCount = troopSpawnIndex + 1;
//          }
//          if (troopSpawnIndex >= troopSpawnCount)
//            troopSpawnCount = troopSpawnIndex + 1;
//          this.GetTroopSpawnFrameWithIndex(agentBuildData, troopSpawnIndex, troopSpawnCount, out position1, out direction1);
//        }
//        else
//        {
//          WorldPosition spawnPosition;
//          this.GetFormationSpawnFrame(agentTeam, FormationClass.NumberOfAllFormations, agentBuildData.AgentIsReinforcement, out spawnPosition, out direction1);
//          position1 = spawnPosition.GetGroundVec3();
//        }
//        agentBuildData.InitialPosition(in position1).InitialDirection(in direction1);
//      }
//      Agent agent2 = agent1;
//      Vec3 valueOrDefault1 = agentBuildData.AgentInitialPosition.GetValueOrDefault();
//      ref Vec3 local1 = ref valueOrDefault1;
//      // ISSUE: explicit reference operation
//      ref Vec2 local2 = @agentBuildData.AgentInitialDirection.GetValueOrDefault();
//      int num1 = agentBuildData.AgentCanSpawnOutsideOfMissionBoundary ? 1 : 0;
//      agent2.SetInitialFrame(in local1, in local2, num1 != 0);
//      if (agentCharacter.BattleEquipments == null && agentCharacter.CivilianEquipments == null)
//        TaleWorlds.Library.Debug.Print("characterObject.AllEquipments is null for \"" + agentCharacter.StringId + "\".");
//      if (agentCharacter.BattleEquipments != null && agentCharacter.BattleEquipments.Any<Equipment>((Func<Equipment, bool>) (eq => eq == null)) && agentCharacter.CivilianEquipments != null && agentCharacter.CivilianEquipments.Any<Equipment>((Func<Equipment, bool>) (eq => eq == null)))
//        TaleWorlds.Library.Debug.Print("Character with id \"" + agentCharacter.StringId + "\" has a null equipment in its AllEquipments.");
//      if (agentCharacter.CivilianEquipments == null)
//        agentBuildData.CivilianEquipment(false);
//      if (agentCharacter.IsHero)
//        agentBuildData.FixedEquipment(true);
//      Equipment equipment1 = agentBuildData.AgentOverridenSpawnEquipment == null ? (agentBuildData.AgentFixedEquipment ? (!agentBuildData.AgentCivilianEquipment ? agentCharacter.FirstBattleEquipment.Clone() : agentCharacter.FirstCivilianEquipment.Clone()) : Equipment.GetRandomEquipmentElements(agent1.Character, !Game.Current.GameType.IsCoreOnlyGameMode, agentBuildData.AgentCivilianEquipment ? Equipment.EquipmentType.Civilian : Equipment.EquipmentType.Battle, agentBuildData.AgentEquipmentSeed)) : agentBuildData.AgentOverridenSpawnEquipment.Clone();
//      Agent agent3 = (Agent) null;
//      if (agentBuildData.AgentNoHorses)
//      {
//        equipment1[EquipmentIndex.ArmorItemEndSlot] = new EquipmentElement();
//        equipment1[EquipmentIndex.HorseHarness] = new EquipmentElement();
//      }
//      if (agentBuildData.AgentNoWeapons)
//      {
//        equipment1[EquipmentIndex.WeaponItemBeginSlot] = new EquipmentElement();
//        equipment1[EquipmentIndex.Weapon1] = new EquipmentElement();
//        equipment1[EquipmentIndex.Weapon2] = new EquipmentElement();
//        equipment1[EquipmentIndex.Weapon3] = new EquipmentElement();
//        equipment1[EquipmentIndex.ExtraWeaponSlot] = new EquipmentElement();
//      }
//      if (agentCharacter.IsHero)
//      {
//        ItemObject banner = (ItemObject) null;
//        ItemObject itemObject = equipment1[EquipmentIndex.ExtraWeaponSlot].Item;
//        if (itemObject != null && itemObject.IsBannerItem && itemObject.BannerComponent != null)
//        {
//          banner = itemObject;
//          equipment1[EquipmentIndex.ExtraWeaponSlot] = new EquipmentElement();
//        }
//        else if (agentBuildData.AgentBannerItem != null)
//          banner = agentBuildData.AgentBannerItem;
//        if (banner != null)
//          agent1.SetFormationBanner(banner);
//      }
//      else if (agentBuildData.AgentBannerItem != null)
//      {
//        equipment1[EquipmentIndex.Weapon1] = new EquipmentElement();
//        equipment1[EquipmentIndex.Weapon2] = new EquipmentElement();
//        equipment1[EquipmentIndex.Weapon3] = new EquipmentElement();
//        equipment1[EquipmentIndex.WeaponItemBeginSlot] = agentBuildData.AgentBannerReplacementWeaponItem == null ? new EquipmentElement() : new EquipmentElement(agentBuildData.AgentBannerReplacementWeaponItem);
//        equipment1[EquipmentIndex.ExtraWeaponSlot] = new EquipmentElement(agentBuildData.AgentBannerItem);
//        if (agentBuildData.AgentOverridenSpawnMissionEquipment != null)
//          agentBuildData.AgentOverridenSpawnMissionEquipment[EquipmentIndex.ExtraWeaponSlot] = new MissionWeapon(agentBuildData.AgentBannerItem, (ItemModifier) null, agentBuildData.AgentBanner);
//      }
//      if (agentBuildData.AgentNoArmor)
//      {
//        equipment1[EquipmentIndex.Gloves] = new EquipmentElement();
//        equipment1[EquipmentIndex.Body] = new EquipmentElement();
//        equipment1[EquipmentIndex.Cape] = new EquipmentElement();
//        equipment1[EquipmentIndex.NumAllWeaponSlots] = new EquipmentElement();
//        equipment1[EquipmentIndex.Leg] = new EquipmentElement();
//      }
//      for (int index = 0; index < 5; ++index)
//      {
//        if (!equipment1[(EquipmentIndex) index].IsEmpty && equipment1[(EquipmentIndex) index].Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CannotBePickedUp))
//          equipment1[(EquipmentIndex) index] = new EquipmentElement();
//      }
//      agent1.InitializeSpawnEquipment(equipment1);
//      agent1.InitializeMissionEquipment(agentBuildData.AgentOverridenSpawnMissionEquipment, agentBuildData.AgentBanner);
//      if (agent1.RandomizeColors)
//        agent1.Equipment.SetGlossMultipliersOfWeaponsRandomly(agentBuildData.AgentEquipmentSeed);
//      ItemObject itemObject1 = equipment1[EquipmentIndex.ArmorItemEndSlot].Item;
//      Vec3? agentInitialPosition;
//      if (itemObject1 != null && itemObject1.HasHorseComponent && itemObject1.HorseComponent.IsRideable)
//      {
//        int num2 = -1;
//        if (agentBuildData.AgentMountIndexOverriden)
//          num2 = agentBuildData.AgentMountIndex;
//        EquipmentElement mount = equipment1[EquipmentIndex.ArmorItemEndSlot];
//        EquipmentElement mountHarness = equipment1[EquipmentIndex.HorseHarness];
//        agentInitialPosition = agentBuildData.AgentInitialPosition;
//        valueOrDefault1 = agentInitialPosition.GetValueOrDefault();
//        ref Vec3 local3 = ref valueOrDefault1;
//        // ISSUE: explicit reference operation
//        ref Vec2 local4 = @agentBuildData.AgentInitialDirection.GetValueOrDefault();
//        int forcedAgentMountIndex = num2;
//        string agentMountKey = agentBuildData.AgentMountKey;
//        agent3 = this.CreateHorseAgentFromRosterElements(mount, mountHarness, in local3, in local4, forcedAgentMountIndex, agentMountKey);
//        Equipment spawnEquipment = new Equipment()
//        {
//          [EquipmentIndex.ArmorItemEndSlot] = equipment1[EquipmentIndex.ArmorItemEndSlot],
//          [EquipmentIndex.HorseHarness] = equipment1[EquipmentIndex.HorseHarness]
//        };
//        agent3.InitializeSpawnEquipment(spawnEquipment);
//        agent1.SetMountAgentBeforeBuild(agent3);
//      }
//      if (spawnFromAgentVisuals || !GameNetwork.IsClientOrReplay)
//        agent1.Equipment.CheckLoadedAmmos();
//      if (!agentBuildData.BodyPropertiesOverriden)
//        agent1.UpdateBodyProperties(agentCharacter.GetBodyProperties(equipment1, agentBuildData.AgentEquipmentSeed));
//      if (GameNetwork.IsServerOrRecorder && agent1.RiderAgent == null)
//      {
//        agentInitialPosition = agentBuildData.AgentInitialPosition;
//        Vec3 valueOrDefault2 = agentInitialPosition.GetValueOrDefault();
//        Vec2 valueOrDefault3 = agentBuildData.AgentInitialDirection.GetValueOrDefault();
//        if (agent1.IsMount)
//        {
//          GameNetwork.BeginBroadcastModuleEvent();
//          GameNetwork.WriteMessage((GameNetworkMessage) new CreateFreeMountAgent(agent1, valueOrDefault2, valueOrDefault3));
//          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        }
//        else
//        {
//          bool flag1 = agentBuildData.AgentMissionPeer != null;
//          NetworkCommunicator networkPeer;
//          if (!flag1)
//          {
//            MissionPeer agentMissionPeer = agentBuildData.OwningAgentMissionPeer;
//            networkPeer = agentMissionPeer != null ? agentMissionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
//          }
//          else
//            networkPeer = agentBuildData.AgentMissionPeer.GetNetworkPeer();
//          NetworkCommunicator networkCommunicator = networkPeer;
//          bool flag2 = agent1.MountAgent != null && agent1.MountAgent.RiderAgent == agent1;
//          GameNetwork.BeginBroadcastModuleEvent();
//          int index = agent1.Index;
//          BasicCharacterObject character = agent1.Character;
//          Monster monster = agent1.Monster;
//          Equipment spawnEquipment1 = agent1.SpawnEquipment;
//          MissionEquipment equipment2 = agent1.Equipment;
//          BodyProperties bodyPropertiesValue = agent1.BodyPropertiesValue;
//          int bodyPropertiesSeed = agent1.BodyPropertiesSeed;
//          int num3 = agent1.IsFemale ? 1 : 0;
//          Team team = agent1.Team;
//          int agentTeamIndex = team != null ? team.TeamIndex : -1;
//          Formation formation = agent1.Formation;
//          int agentFormationIndex = formation != null ? formation.Index : -1;
//          int clothingColor1 = (int) agent1.ClothingColor1;
//          int clothingColor2 = (int) agent1.ClothingColor2;
//          int mountAgentIndex = flag2 ? agent1.MountAgent.Index : -1;
//          Equipment spawnEquipment2 = agent1.MountAgent?.SpawnEquipment;
//          int num4 = flag1 ? 1 : 0;
//          Vec3 position = valueOrDefault2;
//          Vec2 direction = valueOrDefault3;
//          NetworkCommunicator peer = networkCommunicator;
//          GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.CreateAgent(index, character, monster, spawnEquipment1, equipment2, bodyPropertiesValue, bodyPropertiesSeed, num3 != 0, agentTeamIndex, agentFormationIndex, (uint) clothingColor1, (uint) clothingColor2, mountAgentIndex, spawnEquipment2, num4 != 0, position, direction, peer));
//          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        }
//      }
//      MultiplayerMissionAgentVisualSpawnComponent missionBehavior1 = this.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
//      if (missionBehavior1 != null && agentBuildData.AgentMissionPeer != null && agentBuildData.AgentMissionPeer.IsMine && agentBuildData.AgentVisualsIndex == 0)
//        missionBehavior1.OnMyAgentSpawned();
//      if (agent3 != null)
//      {
//        this.BuildAgent(agent3, agentBuildData);
//        foreach (MissionBehavior missionBehavior2 in this.MissionBehaviors)
//          missionBehavior2.OnAgentBuild(agent3, (Banner) null);
//      }
//      this.BuildAgent(agent1, agentBuildData);
//      if (agentBuildData.AgentMissionPeer != null)
//        agent1.MissionPeer = agentBuildData.AgentMissionPeer;
//      if (agentBuildData.OwningAgentMissionPeer != null)
//        agent1.SetOwningAgentMissionPeer(agentBuildData.OwningAgentMissionPeer);
//      foreach (MissionBehavior missionBehavior3 in this.MissionBehaviors)
//        missionBehavior3.OnAgentBuild(agent1, agentBuildData.AgentBanner ?? agentBuildData.AgentTeam?.Banner);
//      agent1.AgentVisuals.CheckResources(true);
//      if (agent1.IsAIControlled)
//      {
//        if (agent3 == null)
//        {
//          AgentFlag agentFlags = agent1.GetAgentFlags() & ~AgentFlag.CanRide;
//          agent1.SetAgentFlags(agentFlags);
//        }
//        else if (agent1.Formation == null)
//          agent1.SetRidingOrder(RidingOrder.RidingOrderEnum.Mount);
//      }
//      return agent1;
//    }

//    public void SetInitialAgentCountForSide(BattleSideEnum side, int agentCount)
//    {
//      int num = (int) side;
//      if (num >= 0 && num < 2)
//        this._initialAgentCountPerSide[(int) side] = agentCount;
//      else
//        TaleWorlds.Library.Debug.FailedAssert("Cannot set initial agent count.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (SetInitialAgentCountForSide), 3896);
//    }

//    public void SetFormationPositioningFromDeploymentPlan(Formation formation)
//    {
//      IFormationDeploymentPlan formationPlan = this._deploymentPlan.GetFormationPlan(formation.Team, formation.FormationIndex);
//      if (formationPlan.HasDimensions)
//        formation.SetFormOrder(FormOrder.FormOrderCustom(formationPlan.PlannedWidth));
//      formation.SetPositioning(new WorldPosition?(formationPlan.CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.None)), new Vec2?(formationPlan.GetDirection()));
//    }

//    public Agent SpawnMonster(
//      ItemRosterElement rosterElement,
//      ItemRosterElement harnessRosterElement,
//      in Vec3 initialPosition,
//      in Vec2 initialDirection,
//      int forcedAgentIndex = -1)
//    {
//      return this.SpawnMonster(rosterElement.EquipmentElement, harnessRosterElement.EquipmentElement, in initialPosition, in initialDirection, forcedAgentIndex);
//    }

//    public Agent SpawnMonster(
//      EquipmentElement equipmentElement,
//      EquipmentElement harnessRosterElement,
//      in Vec3 initialPosition,
//      in Vec2 initialDirection,
//      int forcedAgentIndex = -1)
//    {
//      Agent fromRosterElements = this.CreateHorseAgentFromRosterElements(equipmentElement, harnessRosterElement, in initialPosition, in initialDirection, forcedAgentIndex, MountCreationKey.GetRandomMountKeyString(equipmentElement.Item, MBRandom.RandomInt()));
//      Equipment spawnEquipment = new Equipment()
//      {
//        [EquipmentIndex.ArmorItemEndSlot] = equipmentElement,
//        [EquipmentIndex.HorseHarness] = harnessRosterElement
//      };
//      fromRosterElements.InitializeSpawnEquipment(spawnEquipment);
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new CreateFreeMountAgent(fromRosterElements, initialPosition, initialDirection));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      this.BuildAgent(fromRosterElements, (AgentBuildData) null);
//      return fromRosterElements;
//    }

//    public Agent SpawnTroop(
//      IAgentOriginBase troopOrigin,
//      bool isPlayerSide,
//      bool hasFormation,
//      bool spawnWithHorse,
//      bool isReinforcement,
//      int formationTroopCount,
//      int formationTroopIndex,
//      bool isAlarmed,
//      bool wieldInitialWeapons,
//      bool forceDismounted,
//      Vec3? initialPosition,
//      Vec2? initialDirection,
//      string specialActionSetSuffix = null,
//      ItemObject bannerItem = null,
//      FormationClass formationIndex = FormationClass.NumberOfAllFormations,
//      bool useTroopClassForSpawn = false)
//    {
//      BasicCharacterObject troop = troopOrigin.Troop;
//      Team agentTeam = Mission.GetAgentTeam(troopOrigin, isPlayerSide);
//      if (troop.IsPlayerCharacter && !forceDismounted)
//        spawnWithHorse = true;
//      AgentBuildData agentBuildData = new AgentBuildData(troop).Team(agentTeam).Banner(troopOrigin.Banner).ClothingColor1(agentTeam.Color).ClothingColor2(agentTeam.Color2).TroopOrigin(troopOrigin).NoHorses(!spawnWithHorse).CivilianEquipment(this.DoesMissionRequireCivilianEquipment).SpawnsUsingOwnTroopClass(useTroopClassForSpawn);
//      if (hasFormation)
//      {
//        Formation formation = formationIndex != FormationClass.NumberOfAllFormations ? agentTeam.GetFormation(formationIndex) : agentTeam.GetFormation(this.GetAgentTroopClass(agentTeam.Side, troop));
//        agentBuildData.Formation(formation);
//        agentBuildData.FormationTroopSpawnCount(formationTroopCount).FormationTroopSpawnIndex(formationTroopIndex);
//      }
//      if (!troop.IsPlayerCharacter)
//        agentBuildData.IsReinforcement(isReinforcement);
//      if (bannerItem != null)
//      {
//        if (bannerItem.IsBannerItem && bannerItem.BannerComponent != null)
//        {
//          agentBuildData.BannerItem(bannerItem);
//          ItemObject replacementWeapon = MissionGameModels.Current.BattleBannerBearersModel.GetBannerBearerReplacementWeapon(troop);
//          agentBuildData.BannerReplacementWeaponItem(replacementWeapon);
//        }
//        else
//        {
//          TaleWorlds.Library.Debug.FailedAssert("Passed banner item with name: " + (object) bannerItem.Name + " is not a proper banner item", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (SpawnTroop), 4003);
//          TaleWorlds.Library.Debug.Print("Invalid banner item: " + (object) bannerItem.Name + " is passed to a troop to be spawned", color: TaleWorlds.Library.Debug.DebugColor.Yellow);
//        }
//      }
//      if (initialPosition.HasValue)
//      {
//        agentBuildData.InitialPosition(initialPosition.Value);
//        agentBuildData.InitialDirection(initialDirection.Value);
//      }
//      if (spawnWithHorse)
//        agentBuildData.MountKey(MountCreationKey.GetRandomMountKeyString(troop.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, troop.GetMountKeySeed()));
//      if (isPlayerSide && troop == Game.Current.PlayerTroop)
//        agentBuildData.Controller(AgentControllerType.Player);
//      Agent agent = this.SpawnAgent(agentBuildData);
//      if (agent.Character.IsHero)
//        agent.SetAgentFlags(agent.GetAgentFlags() | AgentFlag.IsUnique);
//      if (agent.IsAIControlled & isAlarmed)
//        agent.SetWatchState(Agent.WatchState.Alarmed);
//      if (wieldInitialWeapons)
//        agent.WieldInitialWeapons();
//      if (!string.IsNullOrEmpty(specialActionSetSuffix))
//      {
//        AnimationSystemData animationSystemData = agentBuildData.AgentMonster.FillAnimationSystemData(MBGlobals.GetActionSetWithSuffix(agentBuildData.AgentMonster, agentBuildData.AgentIsFemale, specialActionSetSuffix), agent.Character.GetStepSize(), false);
//        agent.SetActionSet(ref animationSystemData);
//      }
//      return agent;
//    }

//    public Agent ReplaceBotWithPlayer(Agent botAgent, MissionPeer missionPeer)
//    {
//      if (GameNetwork.IsClientOrReplay || botAgent == null)
//        return (Agent) null;
//      if (GameNetwork.IsServer)
//      {
//        NetworkCommunicator networkPeer = missionPeer.GetNetworkPeer();
//        if (!networkPeer.IsServerPeer)
//        {
//          GameNetwork.BeginModuleEventAsServer(networkPeer);
//          NetworkCommunicator peer = networkPeer;
//          int index = botAgent.Index;
//          double health = (double) botAgent.Health;
//          Agent mountAgent = botAgent.MountAgent;
//          double botAgentMountHealth = mountAgent != null ? (double) mountAgent.Health : -1.0;
//          GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.ReplaceBotWithPlayer(peer, index, (float) health, (float) botAgentMountHealth));
//          GameNetwork.EndModuleEventAsServer();
//        }
//      }
//      if (botAgent.Formation != null)
//        botAgent.Formation.PlayerOwner = botAgent;
//      botAgent.SetOwningAgentMissionPeer((MissionPeer) null);
//      botAgent.MissionPeer = missionPeer;
//      botAgent.Formation = missionPeer.ControlledFormation;
//      AgentFlag agentFlags = botAgent.GetAgentFlags();
//      if (!agentFlags.HasAnyFlag<AgentFlag>(AgentFlag.CanRide))
//        botAgent.SetAgentFlags(agentFlags | AgentFlag.CanRide);
//      --missionPeer.BotsUnderControlAlive;
//      GameNetwork.BeginBroadcastModuleEvent();
//      GameNetwork.WriteMessage((GameNetworkMessage) new BotsControlledChange(missionPeer.GetNetworkPeer(), missionPeer.BotsUnderControlAlive, missionPeer.BotsUnderControlTotal));
//      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
//      if (botAgent.Formation != null)
//        missionPeer.Team.AssignPlayerAsSergeantOfFormation(missionPeer, missionPeer.ControlledFormation.FormationIndex);
//      return botAgent;
//    }

//    private Agent CreateHorseAgentFromRosterElements(
//      EquipmentElement mount,
//      EquipmentElement mountHarness,
//      in Vec3 initialPosition,
//      in Vec2 initialDirection,
//      int forcedAgentMountIndex,
//      string horseCreationKey)
//    {
//      Agent agent = this.CreateAgent(mount.Item.HorseComponent.Monster, false, 0, Agent.CreationType.FromHorseObj, 1f, forcedAgentMountIndex, (int) mount.Weight, (BasicCharacterObject) null);
//      agent.SetInitialFrame(in initialPosition, in initialDirection);
//      agent.BaseHealthLimit = (float) mount.GetModifiedMountHitPoints();
//      agent.HealthLimit = agent.BaseHealthLimit;
//      agent.Health = agent.HealthLimit;
//      agent.SetMountInitialValues(mount.GetModifiedItemName(), horseCreationKey);
//      return agent;
//    }

//    public void OnAgentInteraction(Agent requesterAgent, Agent targetAgent, sbyte agentBoneIndex)
//    {
//      if (requesterAgent == Agent.Main && targetAgent.IsMount)
//      {
//        Agent.Main.Mount(targetAgent);
//      }
//      else
//      {
//        foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//          missionBehavior.OnAgentInteraction(requesterAgent, targetAgent, agentBoneIndex);
//      }
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    public void EndMission()
//    {
//      TaleWorlds.Library.Debug.Print("I called EndMission", debugFilter: 17179869184UL);
//      this._missionEndTime = -1f;
//      this.NextCheckTimeEndMission = -1f;
//      this.MissionEnded = true;
//      this.CurrentState = Mission.State.EndingNextFrame;
//    }

//    private void EndMissionInternal()
//    {
//      MBDebug.Print("I called EndMissionInternal", debugFilter: 17179869184UL);
//      this._deploymentPlan.ClearAll();
//      foreach (IMissionListener missionListener in this._listeners.ToArray())
//        missionListener.OnEndMission();
//      this.StopSoundEvents();
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnEndMissionInternal();
//      foreach (Agent agent in (List<Agent>) this.Agents)
//        agent.OnRemove();
//      foreach (Agent allAgent in (List<Agent>) this.AllAgents)
//      {
//        allAgent.OnDelete();
//        allAgent.Clear();
//      }
//      this.Teams.Clear();
//      this.FocusableObjectInformationProvider.OnFinalize();
//      foreach (MissionObject missionObject in (List<MissionObject>) this.MissionObjects)
//        missionObject.OnEndMission();
//      this.CurrentState = Mission.State.Over;
//      this.FreeResources();
//      this.FinalizeMission();
//    }

//    private void StopSoundEvents()
//    {
//      if (this._ambientSoundEvent == null)
//        return;
//      this._ambientSoundEvent.Stop();
//    }

//    public void AddMissionBehavior(MissionBehavior missionBehavior)
//    {
//      this.MissionBehaviors.Add(missionBehavior);
//      missionBehavior.Mission = this;
//      switch (missionBehavior.BehaviorType)
//      {
//        case MissionBehaviorType.Logic:
//          this.MissionLogics.Add(missionBehavior as MissionLogic);
//          break;
//        case MissionBehaviorType.Other:
//          this._otherMissionBehaviors.Add(missionBehavior);
//          break;
//      }
//      missionBehavior.OnCreated();
//    }

//    public T GetMissionBehavior<T>() where T : class, IMissionBehavior
//    {
//      for (int index = 0; index < this.MissionBehaviors.Count; ++index)
//      {
//        if (this.MissionBehaviors[index] is T missionBehavior)
//          return missionBehavior;
//      }
//      return default (T);
//    }

//    public void RemoveMissionBehavior(MissionBehavior missionBehavior)
//    {
//      missionBehavior.OnRemoveBehavior();
//      switch (missionBehavior.BehaviorType)
//      {
//        case MissionBehaviorType.Logic:
//          this.MissionLogics.Remove(missionBehavior as MissionLogic);
//          break;
//        case MissionBehaviorType.Other:
//          this._otherMissionBehaviors.Remove(missionBehavior);
//          break;
//        default:
//          TaleWorlds.Library.Debug.FailedAssert("Invalid behavior type", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (RemoveMissionBehavior), 4299);
//          break;
//      }
//      this.MissionBehaviors.Remove(missionBehavior);
//      missionBehavior.Mission = (Mission) null;
//    }

//    public void JoinEnemyTeam()
//    {
//      if (this.PlayerTeam == this.DefenderTeam)
//      {
//        Agent leader = this.AttackerTeam.Leader;
//        if (leader == null)
//          return;
//        if (this.MainAgent != null && this.MainAgent.IsActive())
//          this.MainAgent.Controller = AgentControllerType.AI;
//        leader.Controller = AgentControllerType.Player;
//        this.PlayerTeam = this.AttackerTeam;
//      }
//      else if (this.PlayerTeam == this.AttackerTeam)
//      {
//        Agent leader = this.DefenderTeam.Leader;
//        if (leader == null)
//          return;
//        if (this.MainAgent != null && this.MainAgent.IsActive())
//          this.MainAgent.Controller = AgentControllerType.AI;
//        leader.Controller = AgentControllerType.Player;
//        this.PlayerTeam = this.DefenderTeam;
//      }
//      else
//        TaleWorlds.Library.Debug.FailedAssert("Player is neither attacker nor defender.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (JoinEnemyTeam), 4343);
//    }

//    public void OnEndMissionResult()
//    {
//      foreach (MissionLogic missionLogic in this.MissionLogics.ToArray())
//        missionLogic.OnBattleEnded();
//      this.RetreatMission();
//    }

//    public bool IsAgentInteractionAllowed()
//    {
//      if (this.IsAgentInteractionAllowed_AdditionalCondition != null)
//      {
//        foreach (Delegate invocation in this.IsAgentInteractionAllowed_AdditionalCondition.GetInvocationList())
//        {
//          object obj;
//          if ((obj = invocation.DynamicInvoke()) is bool && !(bool) obj)
//            return false;
//        }
//      }
//      return true;
//    }

//    public bool IsOrderGesturesEnabled()
//    {
//      if (this.AreOrderGesturesEnabled_AdditionalCondition != null)
//      {
//        foreach (Delegate invocation in this.AreOrderGesturesEnabled_AdditionalCondition.GetInvocationList())
//        {
//          object obj;
//          if ((obj = invocation.DynamicInvoke()) is bool && !(bool) obj)
//            return false;
//        }
//      }
//      return true;
//    }

//    public List<EquipmentElement> GetExtraEquipmentElementsForCharacter(
//      BasicCharacterObject character,
//      bool getAllEquipments = false)
//    {
//      List<EquipmentElement> elementsForCharacter1 = new List<EquipmentElement>();
//      foreach (MissionLogic missionLogic in this.MissionLogics)
//      {
//        List<EquipmentElement> elementsForCharacter2 = missionLogic.GetExtraEquipmentElementsForCharacter(character, getAllEquipments);
//        if (elementsForCharacter2 != null)
//          elementsForCharacter1.AddRange((IEnumerable<EquipmentElement>) elementsForCharacter2);
//      }
//      return elementsForCharacter1;
//    }

//    private bool CheckMissionEnded()
//    {
//      foreach (MissionLogic missionLogic in this.MissionLogics)
//      {
//        MissionResult missionResult = (MissionResult) null;
//        ref MissionResult local = ref missionResult;
//        if (missionLogic.MissionEnded(ref local))
//        {
//          TaleWorlds.Library.Debug.Print("CheckMissionEnded::ended");
//          this.MissionResult = missionResult;
//          this.MissionEnded = true;
//          this.MissionResultReady(missionResult);
//          return true;
//        }
//      }
//      return false;
//    }

//    private void MissionResultReady(MissionResult missionResult)
//    {
//      foreach (MissionLogic missionLogic in this.MissionLogics)
//        missionLogic.OnMissionResultReady(missionResult);
//    }

//    private void CheckMissionEnd(float currentTime)
//    {
//      if (!GameNetwork.IsClient && (double) currentTime > (double) this.NextCheckTimeEndMission)
//      {
//        if (this.CurrentState == Mission.State.Continuing)
//        {
//          if (this.MissionEnded)
//            return;
//          this.NextCheckTimeEndMission += 0.1f;
//          this.CheckMissionEnded();
//          if (!this.MissionEnded)
//            return;
//          this._missionEndTime = currentTime + this.MissionCloseTimeAfterFinish;
//          this.NextCheckTimeEndMission += 5f;
//          foreach (MissionLogic missionLogic in this.MissionLogics)
//            missionLogic.ShowBattleResults();
//        }
//        else if ((double) currentTime > (double) this._missionEndTime)
//          this.EndMissionInternal();
//        else
//          this.NextCheckTimeEndMission += 5f;
//      }
//      else
//      {
//        if (this.CurrentState == Mission.State.Continuing || (double) currentTime <= (double) this.NextCheckTimeEndMission)
//          return;
//        this.EndMissionInternal();
//      }
//    }

//    public bool IsPlayerCloseToAnEnemy(float distance = 5f)
//    {
//      if (this.MainAgent == null)
//        return false;
//      Vec3 position = this.MainAgent.Position;
//      float num = distance * distance;
//      AgentProximityMap.ProximityMapSearchStruct searchStruct = AgentProximityMap.BeginSearch(this, position.AsVec2, distance);
//      while (searchStruct.LastFoundAgent != null)
//      {
//        Agent lastFoundAgent = searchStruct.LastFoundAgent;
//        if (lastFoundAgent != this.MainAgent && lastFoundAgent.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.CanAttack) && (double) lastFoundAgent.Position.DistanceSquared(position) <= (double) num && (!lastFoundAgent.IsAIControlled || lastFoundAgent.IsAlarmed()) && lastFoundAgent.IsEnemyOf(this.MainAgent) && !lastFoundAgent.IsRetreating())
//          return true;
//        AgentProximityMap.FindNext(this, ref searchStruct);
//      }
//      return false;
//    }

//    public Vec3 GetRandomPositionAroundPoint(
//      Vec3 center,
//      float minDistance,
//      float maxDistance,
//      bool nearFirst = false)
//    {
//      Vec3 positionAroundPoint = center;
//      Vec3 vec3 = new Vec3(-1f);
//      vec3.RotateAboutZ(6.28318548f * MBRandom.RandomFloat);
//      float num = maxDistance - minDistance;
//      if (nearFirst)
//      {
//        for (int index1 = 4; index1 > 0; --index1)
//        {
//          for (int index2 = 0; (double) index2 <= 10.0; ++index2)
//          {
//            vec3.RotateAboutZ(1.2566371f);
//            Vec3 position = positionAroundPoint + vec3 * (minDistance + num / (float) index1);
//            if (this.Scene.GetNavigationMeshForPosition(in position) != UIntPtr.Zero)
//              return position;
//          }
//        }
//      }
//      else
//      {
//        for (int index3 = 1; index3 < 5; ++index3)
//        {
//          for (int index4 = 0; (double) index4 <= 10.0; ++index4)
//          {
//            vec3.RotateAboutZ(1.2566371f);
//            Vec3 position = positionAroundPoint + vec3 * (minDistance + num / (float) index3);
//            if (this.Scene.GetNavigationMeshForPosition(in position) != UIntPtr.Zero)
//              return position;
//          }
//        }
//      }
//      return positionAroundPoint;
//    }

//    public WorldPosition FindBestDefendingPosition(
//      WorldPosition enemyPosition,
//      WorldPosition defendedPosition)
//    {
//      return this.GetBestSlopeAngleHeightPosForDefending(enemyPosition, defendedPosition, 10, 0.5f, 4f, 0.5f, 0.707106769f, 0.1f, 1f, 0.7f, 0.5f, 1.2f, 20f, 0.6f);
//    }

//    public WorldPosition FindPositionWithBiggestSlopeTowardsDirectionInSquare(
//      ref WorldPosition center,
//      float halfSize,
//      ref WorldPosition referencePosition)
//    {
//      return this.GetBestSlopeTowardsDirection(ref center, halfSize, ref referencePosition);
//    }

//    public Mission.Missile AddCustomMissile(
//      Agent shooterAgent,
//      MissionWeapon missileWeapon,
//      Vec3 position,
//      Vec3 direction,
//      Mat3 orientation,
//      float baseSpeed,
//      float speed,
//      bool addRigidBody,
//      MissionObject missionObjectToIgnore,
//      int forcedMissileIndex = -1)
//    {
//      WeaponData weaponData = missileWeapon.GetWeaponData(true);
//      GameEntity missileEntity;
//      int num;
//      if (missileWeapon.WeaponsCount == 1)
//      {
//        WeaponStatsData weaponStatsData = missileWeapon.GetWeaponStatsDataForUsage(0);
//        num = this.AddMissileSingleUsageAux(forcedMissileIndex, false, shooterAgent, in weaponData, in weaponStatsData, 0.0f, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, missionObjectToIgnore != null ? missionObjectToIgnore.GameEntity : WeakGameEntity.Invalid, false, out missileEntity);
//      }
//      else
//      {
//        WeaponStatsData[] weaponStatsData = missileWeapon.GetWeaponStatsData();
//        num = this.AddMissileAux(forcedMissileIndex, false, shooterAgent, in weaponData, weaponStatsData, 0.0f, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, missionObjectToIgnore != null ? missionObjectToIgnore.GameEntity : WeakGameEntity.Invalid, false, out missileEntity);
//      }
//      weaponData.DeinitializeManagedPointers();
//      Mission.Missile missile = new Mission.Missile(this, num, missileEntity, shooterAgent, missileWeapon, missionObjectToIgnore);
//      this._missilesList.Add(missile);
//      this._missilesDictionary.Add(num, missile);
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new CreateMissile(num, shooterAgent.Index, EquipmentIndex.None, missileWeapon, position, direction, speed, orientation, addRigidBody, missionObjectToIgnore.Id, false));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      return missile;
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void OnAgentShootMissile(
//      Agent shooterAgent,
//      EquipmentIndex weaponIndex,
//      Vec3 position,
//      Vec3 velocity,
//      Mat3 orientation,
//      bool hasRigidBody,
//      bool isPrimaryWeaponShot,
//      int forcedMissileIndex)
//    {
//      bool isPrediction = GameNetwork.IsClient && forcedMissileIndex == -1;
//      float damageBonus = 0.0f;
//      MissionWeapon missionWeapon;
//      MissionWeapon ammoWeapon;
//      if (shooterAgent.Equipment[weaponIndex].CurrentUsageItem != null)
//      {
//        missionWeapon = shooterAgent.Equipment[weaponIndex];
//        if (missionWeapon.CurrentUsageItem.IsRangedWeapon)
//        {
//          missionWeapon = shooterAgent.Equipment[weaponIndex];
//          if (missionWeapon.CurrentUsageItem.IsConsumable)
//          {
//            ammoWeapon = shooterAgent.Equipment[weaponIndex];
//            goto label_6;
//          }
//        }
//      }
//      missionWeapon = shooterAgent.Equipment[weaponIndex];
//      ammoWeapon = missionWeapon.AmmoWeapon;
//      missionWeapon = shooterAgent.Equipment[weaponIndex];
//      if (missionWeapon.CurrentUsageItem != null)
//      {
//        missionWeapon = shooterAgent.Equipment[weaponIndex];
//        damageBonus = (float) missionWeapon.GetModifiedThrustDamageForCurrentUsage();
//      }
//label_6:
//      if (ammoWeapon.IsEmpty)
//        return;
//      ammoWeapon.Amount = (short) 1;
//      WeaponData weaponData = ammoWeapon.GetWeaponData(true);
//      Vec3 direction = velocity;
//      float speed = direction.Normalize();
//      missionWeapon = shooterAgent.Equipment[shooterAgent.GetPrimaryWieldedItemIndex()];
//      float speedForCurrentUsage = (float) missionWeapon.GetModifiedMissileSpeedForCurrentUsage();
//      GameEntity missileEntity;
//      int num;
//      if (ammoWeapon.WeaponsCount == 1)
//      {
//        WeaponStatsData weaponStatsData = ammoWeapon.GetWeaponStatsDataForUsage(0);
//        num = this.AddMissileSingleUsageAux(forcedMissileIndex, isPrediction, shooterAgent, in weaponData, in weaponStatsData, damageBonus, ref position, ref direction, ref orientation, speedForCurrentUsage, speed, hasRigidBody, WeakGameEntity.Invalid, isPrimaryWeaponShot, out missileEntity);
//      }
//      else
//      {
//        WeaponStatsData[] weaponStatsData = ammoWeapon.GetWeaponStatsData();
//        num = this.AddMissileAux(forcedMissileIndex, isPrediction, shooterAgent, in weaponData, weaponStatsData, damageBonus, ref position, ref direction, ref orientation, speedForCurrentUsage, speed, hasRigidBody, WeakGameEntity.Invalid, isPrimaryWeaponShot, out missileEntity);
//      }
//      weaponData.DeinitializeManagedPointers();
//      if (!isPrediction)
//      {
//        Mission.Missile missile = new Mission.Missile(this, num, missileEntity, shooterAgent, ammoWeapon, (MissionObject) null);
//        this._missilesList.Add(missile);
//        this._missilesDictionary.Add(num, missile);
//        if (GameNetwork.IsServerOrRecorder)
//        {
//          GameNetwork.BeginBroadcastModuleEvent();
//          GameNetwork.WriteMessage((GameNetworkMessage) new CreateMissile(num, shooterAgent.Index, weaponIndex, MissionWeapon.Invalid, position, direction, speed, orientation, hasRigidBody, MissionObjectId.Invalid, isPrimaryWeaponShot));
//          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        }
//      }
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnAgentShootMissile(shooterAgent, weaponIndex, position, velocity, orientation, hasRigidBody, forcedMissileIndex);
//      shooterAgent?.UpdateLastRangedAttackTimeDueToAnAttack(MBCommon.GetTotalMissionTime());
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal AgentState GetAgentState(
//      Agent affectorAgent,
//      Agent agent,
//      DamageTypes damageType,
//      WeaponFlags weaponFlags)
//    {
//      float useSurgeryProbability;
//      float stateProbability = MissionGameModels.Current.AgentDecideKilledOrUnconsciousModel.GetAgentStateProbability(affectorAgent, agent, damageType, weaponFlags, out useSurgeryProbability);
//      AgentState agentState = AgentState.None;
//      bool usedSurgery = false;
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//      {
//        if (missionBehavior is IAgentStateDecider agentStateDecider)
//        {
//          agentState = agentStateDecider.GetAgentState(agent, stateProbability, out usedSurgery);
//          break;
//        }
//      }
//      if (agentState == AgentState.None)
//      {
//        float randomFloat = MBRandom.RandomFloat;
//        if ((double) randomFloat < (double) stateProbability)
//        {
//          agentState = AgentState.Killed;
//          usedSurgery = true;
//        }
//        else
//        {
//          agentState = AgentState.Unconscious;
//          if ((double) randomFloat > 1.0 - (double) useSurgeryProbability)
//            usedSurgery = true;
//        }
//      }
//      if (usedSurgery && affectorAgent != null && affectorAgent.Team != null && agent.Team != null && affectorAgent.Team == agent.Team)
//        usedSurgery = false;
//      for (int index = 0; index < this.MissionBehaviors.Count; ++index)
//        this.MissionBehaviors[index].OnGetAgentState(agent, usedSurgery);
//      return agentState;
//    }

//    public void OnAgentMount(Agent agent)
//    {
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnAgentMount(agent);
//    }

//    public void OnAgentDismount(Agent agent)
//    {
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnAgentDismount(agent);
//    }

//    public void OnObjectUsed(Agent userAgent, UsableMissionObject usableGameObject)
//    {
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnObjectUsed(userAgent, usableGameObject);
//    }

//    public void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usableGameObject)
//    {
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnObjectStoppedBeingUsed(userAgent, usableGameObject);
//    }

//    public void InitializeStartingBehaviors(
//      MissionLogic[] logicBehaviors,
//      MissionBehavior[] otherBehaviors,
//      MissionNetwork[] networkBehaviors)
//    {
//      foreach (MissionBehavior logicBehavior in logicBehaviors)
//        this.AddMissionBehavior(logicBehavior);
//      foreach (MissionBehavior networkBehavior in networkBehaviors)
//        this.AddMissionBehavior(networkBehavior);
//      foreach (MissionBehavior otherBehavior in otherBehaviors)
//        this.AddMissionBehavior(otherBehavior);
//    }

//    public Agent GetClosestEnemyAgent(Team team, Vec3 position, float radius)
//    {
//      return this.GetClosestEnemyAgent(team.MBTeam, position, radius);
//    }

//    public Agent GetClosestAllyAgent(Team team, Vec3 position, float radius)
//    {
//      return this.GetClosestAllyAgent(team.MBTeam, position, radius);
//    }

//    public int GetNearbyEnemyAgentCount(Team team, Vec2 position, float radius)
//    {
//      return this.GetNearbyEnemyAgentCount(team.MBTeam, position, radius);
//    }

//    public bool HasAnyAgentsOfSideInRange(Vec3 origin, float radius, BattleSideEnum side)
//    {
//      Team team = side == BattleSideEnum.Attacker ? this.AttackerTeam : this.DefenderTeam;
//      return MBAPI.IMBMission.HasAnyAgentsOfTeamAround(this.Pointer, origin, radius, team.MBTeam.Index);
//    }

//    public void AddSoundAlarmFactorToAgents(
//      Agent alarmCreatorAgent,
//      in Vec3 soundPosition,
//      float soundLevelSquareRoot)
//    {
//      Mission.OnAddSoundAlarmFactorToAgentsDelegate alarmFactorToAgents = this.OnAddSoundAlarmFactorToAgents;
//      if (alarmFactorToAgents == null)
//        return;
//      alarmFactorToAgents(alarmCreatorAgent, in soundPosition, soundLevelSquareRoot);
//    }

//    private void HandleSpawnedItems()
//    {
//      if (GameNetwork.IsClientOrReplay)
//        return;
//      int num = 0;
//      for (int index = this._spawnedItemEntitiesCreatedAtRuntime.Count - 1; index >= 0; --index)
//      {
//        SpawnedItemEntity spawnedItemEntity = this._spawnedItemEntitiesCreatedAtRuntime[index];
//        if (!spawnedItemEntity.IsRemoved)
//        {
//          if (!spawnedItemEntity.IsDeactivated && !spawnedItemEntity.HasUser && spawnedItemEntity.HasLifeTime && !spawnedItemEntity.HasAIMovingTo && (num > 500 || spawnedItemEntity.IsReadyToBeDeleted()))
//            spawnedItemEntity.GameEntity.Remove(80);
//          else
//            ++num;
//        }
//        if (spawnedItemEntity.IsRemoved)
//          this._spawnedItemEntitiesCreatedAtRuntime.RemoveAt(index);
//      }
//    }

//    public bool OnMissionObjectRemoved(MissionObject missionObject, int removeReason)
//    {
//      if (!GameNetwork.IsClientOrReplay && missionObject.CreatedAtRuntime)
//      {
//        this.ReturnRuntimeMissionObjectId(missionObject.Id.Id);
//        if (GameNetwork.IsServerOrRecorder)
//        {
//          this.RemoveDynamicallySpawnedMissionObjectInfo(missionObject.Id);
//          GameNetwork.BeginBroadcastModuleEvent();
//          GameNetwork.WriteMessage((GameNetworkMessage) new RemoveMissionObject(missionObject.Id));
//          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        }
//      }
//      this._activeMissionObjects.Remove(missionObject);
//      return this._missionObjects.Remove(missionObject);
//    }

//    public bool AgentLookingAtAgent(Agent agent1, Agent agent2)
//    {
//      Vec3 v1 = agent2.Position - agent1.Position;
//      float num1 = v1.Normalize();
//      float num2 = Vec3.DotProduct(v1, agent1.LookDirection);
//      return ((double) num2 >= 1.0 ? 0 : ((double) num2 > 0.86000001430511475 ? 1 : 0)) != 0 && (double) num1 < 4.0;
//    }

//    public Agent FindAgentWithIndex(int agentId) => this.FindAgentWithIndexAux(agentId);

//    public static Agent.UnderAttackType GetUnderAttackTypeOfAgents(
//      IEnumerable<Agent> agents,
//      float timeLimit = 3f)
//    {
//      float a1 = float.MinValue;
//      float a2 = float.MinValue;
//      timeLimit += MBCommon.GetTotalMissionTime();
//      foreach (Agent agent in agents)
//      {
//        a1 = MathF.Max(a1, agent.LastMeleeHitTime);
//        a2 = MathF.Max(a2, agent.LastRangedHitTime);
//        if ((double) a2 >= 0.0 && (double) a2 < (double) timeLimit)
//          return Agent.UnderAttackType.UnderRangedAttack;
//        if ((double) a1 >= 0.0 && (double) a1 < (double) timeLimit)
//          return Agent.UnderAttackType.UnderMeleeAttack;
//      }
//      return Agent.UnderAttackType.NotUnderAttack;
//    }

//    public static Team GetAgentTeam(IAgentOriginBase troopOrigin, bool isPlayerSide)
//    {
//      if (Mission.Current != null)
//        return !troopOrigin.IsUnderPlayersCommand ? (!isPlayerSide ? Mission.Current.PlayerEnemyTeam : (Mission.Current.PlayerAllyTeam == null ? Mission.Current.PlayerTeam : Mission.Current.PlayerAllyTeam)) : Mission.Current.PlayerTeam;
//      TaleWorlds.Library.Debug.FailedAssert("Mission current is null", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (GetAgentTeam), 5035);
//      return (Team) null;
//    }

//    public static Team GetTeam(TeamSideEnum teamSide)
//    {
//      if (Mission.Current == null)
//      {
//        TaleWorlds.Library.Debug.FailedAssert("Mission current is null", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (GetTeam), 5068);
//        return (Team) null;
//      }
//      switch (teamSide)
//      {
//        case TeamSideEnum.PlayerTeam:
//          return Mission.Current.PlayerTeam;
//        case TeamSideEnum.PlayerAllyTeam:
//          return Mission.Current.PlayerAllyTeam;
//        case TeamSideEnum.EnemyTeam:
//          return Mission.Current.PlayerEnemyTeam;
//        default:
//          return (Team) null;
//      }
//    }

//    public static IEnumerable<Team> GetTeamsOfSide(BattleSideEnum side)
//    {
//      return Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => t.Side == side));
//    }

//    public static float GetBattleSizeOffset(int battleSize, Path path)
//    {
//      if ((!((NativeObject) path != (NativeObject) null) ? 0 : (path.NumberOfPoints > 1 ? 1 : 0)) == 0)
//        return 0.0f;
//      float normalizationFactor = 800f / path.GetTotalLength();
//      return (float) -(0.43999999761581421 * (double) Mission.GetBattleSizeFactor(battleSize, normalizationFactor));
//    }

//    public static float GetPathOffsetFromDistance(float distance, Path path)
//    {
//      if ((!((NativeObject) path != (NativeObject) null) ? 0 : (path.NumberOfPoints > 1 ? 1 : 0)) == 0)
//        return 0.0f;
//      float totalLength = path.GetTotalLength();
//      return MathF.Clamp(distance / totalLength, 0.0f, 1f);
//    }

//    public void OnRenderingStarted()
//    {
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnRenderingStarted();
//    }

//    public static float GetBattleSizeFactor(int battleSize, float normalizationFactor)
//    {
//      float num = -1f;
//      if (battleSize > 0)
//        num = (float) (0.039999999105930328 + 0.079999998211860657 * (double) MathF.Pow((float) battleSize, 0.4f)) * normalizationFactor;
//      return MBMath.ClampFloat(num, 0.15f, 1f);
//    }

//    public Agent.MovementBehaviorType GetMovementTypeOfAgents(IEnumerable<Agent> agents)
//    {
//      float totalMissionTime = MBCommon.GetTotalMissionTime();
//      int num1 = 0;
//      int num2 = 0;
//      int num3 = 0;
//      foreach (Agent agent in agents)
//      {
//        ++num1;
//        if (agent.IsAIControlled && (agent.IsRetreating() || agent.Formation != null && agent.Formation.GetReadonlyMovementOrderReference().OrderType == OrderType.Retreat))
//          ++num2;
//        if ((double) totalMissionTime - (double) agent.LastMeleeAttackTime < 3.0)
//          ++num3;
//      }
//      if ((double) num2 * 1.0 / (double) num1 > 0.30000001192092896)
//        return Agent.MovementBehaviorType.Flee;
//      return num3 > 0 ? Agent.MovementBehaviorType.Engaged : Agent.MovementBehaviorType.Idle;
//    }

//    public void ShowInMissionLoadingScreen(int durationInSecond, Action onLoadingEndedAction)
//    {
//      this._inMissionLoadingScreenTimer = new TaleWorlds.Core.Timer(this.CurrentTime, (float) durationInSecond);
//      this._onLoadingEndedAction = onLoadingEndedAction;
//      LoadingWindow.EnableGlobalLoadingWindow();
//    }

//    public bool CanAgentRout(Agent agent)
//    {
//      if (!agent.IsRunningAway && (agent.CommonAIComponent == null || !agent.CommonAIComponent.IsRetreating) && (!agent.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.CanWander) || !agent.IsWandering()) || agent.RiderAgent != null)
//        return false;
//      return this.CanAgentRout_AdditionalCondition == null || this.CanAgentRout_AdditionalCondition(agent);
//    }

//    internal bool CanGiveDamageToAgentShield(
//      Agent attacker,
//      WeaponComponentData attackerWeapon,
//      Agent defender)
//    {
//      return MissionGameModels.Current.AgentApplyDamageModel.CanWeaponIgnoreFriendlyFireChecks(attackerWeapon) || !this.CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(attacker, defender);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void MeleeHitCallback(
//      ref AttackCollisionData collisionData,
//      Agent attacker,
//      Agent victim,
//      GameEntity realHitEntity,
//      ref float inOutMomentumRemaining,
//      ref MeleeCollisionReaction colReaction,
//      CrushThroughState crushThroughState,
//      Vec3 blowDir,
//      Vec3 swingDir,
//      ref HitParticleResultData hitParticleResultData,
//      bool crushedThroughWithoutAgentCollision)
//    {
//      hitParticleResultData.Reset();
//      bool flag1 = collisionData.CollisionResult == CombatCollisionResult.Parried || collisionData.CollisionResult == CombatCollisionResult.Blocked || collisionData.CollisionResult == CombatCollisionResult.ChamberBlocked;
//      if (collisionData.IsAlternativeAttack && !flag1 && victim != null && victim.IsHuman && collisionData.CollisionBoneIndex != (sbyte) -1 && (collisionData.VictimHitBodyPart == BoneBodyPartType.ArmLeft || collisionData.VictimHitBodyPart == BoneBodyPartType.ArmRight) && victim.IsHuman)
//        colReaction = MeleeCollisionReaction.ContinueChecking;
//      if (colReaction != MeleeCollisionReaction.ContinueChecking)
//      {
//        int num = this.CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(attacker, victim) ? 1 : 0;
//        bool flag2 = victim != null && victim.CurrentMortalityState == Agent.MortalityState.Invulnerable;
//        bool flag3 = victim == null && realHitEntity == (GameEntity) null;
//        bool flag4;
//        if (num != 0)
//        {
//          collisionData.AttackerStunPeriod = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerFriendlyFire);
//          flag4 = true;
//        }
//        else
//          flag4 = flag2 || flag3 || flag1 && !collisionData.AttackBlockedWithShield;
//        int slotOrMissileIndex = collisionData.AffectorWeaponSlotOrMissileIndex;
//        MissionWeapon attackerWeapon = slotOrMissileIndex >= 0 ? attacker.Equipment[slotOrMissileIndex] : MissionWeapon.Invalid;
//        if (crushThroughState == CrushThroughState.CrushedThisFrame && !collisionData.IsAlternativeAttack)
//          MissionCombatMechanicsHelper.UpdateMomentumRemaining(ref inOutMomentumRemaining, new Blow(), in collisionData, attacker, victim, in attackerWeapon, true);
//        WeaponComponentData shieldOnBack = (WeaponComponentData) null;
//        CombatLogData combatLog = new CombatLogData();
//        if (!flag4)
//        {
//          this.GetAttackCollisionResults(attacker, victim, (object) realHitEntity != null ? realHitEntity.WeakEntity : WeakGameEntity.Invalid, inOutMomentumRemaining, in attackerWeapon, crushThroughState != 0, flag4, crushedThroughWithoutAgentCollision, ref collisionData, out shieldOnBack, out combatLog);
//          if (!collisionData.IsAlternativeAttack && attacker.IsDoingPassiveAttack && !GameNetwork.IsSessionActive && (double) ManagedOptions.GetConfig(ManagedOptions.ManagedOptionsType.ReportDamage) > 0.0)
//          {
//            if (attacker.HasMount)
//            {
//              if (attacker.IsMainAgent)
//                InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_delivered_couched_lance_damage").ToString(), Color.ConvertStringToColor("#AE4AD9FF")));
//              else if (victim != null && victim.IsMainAgent)
//                InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_received_couched_lance_damage").ToString(), Color.ConvertStringToColor("#D65252FF")));
//            }
//            else if (attacker.IsMainAgent)
//              InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_delivered_braced_polearm_damage").ToString(), Color.ConvertStringToColor("#AE4AD9FF")));
//            else if (victim != null && victim.IsMainAgent)
//              InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_received_braced_polearm_damage").ToString(), Color.ConvertStringToColor("#D65252FF")));
//          }
//          if (collisionData.CollidedWithShieldOnBack && shieldOnBack != null && victim != null && victim.IsMainAgent)
//            InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back").ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
//        }
//        else
//        {
//          collisionData.InflictedDamage = 0;
//          collisionData.BaseMagnitude = 0.0f;
//          collisionData.AbsorbedByArmor = 0;
//          collisionData.SelfInflictedDamage = 0;
//        }
//        if (!crushedThroughWithoutAgentCollision)
//        {
//          Blow meleeBlow = this.CreateMeleeBlow(attacker, victim, in collisionData, in attackerWeapon, crushThroughState, blowDir, swingDir, flag4);
//          if (!flag1 && (victim != null && victim.IsActive() || realHitEntity != (GameEntity) null))
//            this.RegisterBlow(attacker, victim, (object) realHitEntity != null ? realHitEntity.WeakEntity : WeakGameEntity.Invalid, meleeBlow, ref collisionData, in attackerWeapon, ref combatLog);
//          MissionCombatMechanicsHelper.UpdateMomentumRemaining(ref inOutMomentumRemaining, in meleeBlow, in collisionData, attacker, victim, in attackerWeapon, false);
//          bool isFatalHit = victim != null && (double) victim.Health <= 0.0;
//          bool isShruggedOff = (meleeBlow.BlowFlag & BlowFlags.ShrugOff) != 0;
//          this.DecideAgentHitParticles(attacker, victim, in meleeBlow, in collisionData, ref hitParticleResultData);
//          MissionGameModels.Current.AgentApplyDamageModel.DecideWeaponCollisionReaction(in meleeBlow, in collisionData, attacker, victim, in attackerWeapon, isFatalHit, isShruggedOff, inOutMomentumRemaining, out colReaction);
//        }
//        else
//          colReaction = MeleeCollisionReaction.ContinueChecking;
//        foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
//          missionBehavior.OnMeleeHit(attacker, victim, flag4, collisionData);
//      }
//      if (collisionData.IsShieldBroken)
//      {
//        this.AddSoundAlarmFactorToAgents(attacker, collisionData.CollisionGlobalPosition, 20f);
//      }
//      else
//      {
//        if (collisionData.IsMissile || collisionData.CollisionResult != CombatCollisionResult.HitWorld && collisionData.CollisionResult != CombatCollisionResult.Blocked && collisionData.CollisionResult != CombatCollisionResult.Parried && collisionData.CollisionResult != CombatCollisionResult.ChamberBlocked)
//          return;
//        this.AddSoundAlarmFactorToAgents(attacker, collisionData.CollisionGlobalPosition, 10f);
//      }
//    }

//    private void DecideAgentHitParticles(
//      Agent attacker,
//      Agent victim,
//      in Blow blow,
//      in AttackCollisionData collisionData,
//      ref HitParticleResultData hprd)
//    {
//      if (victim == null || blow.InflictedDamage <= 0 && (double) victim.Health > 0.0)
//        return;
//      if ((!blow.WeaponRecord.HasWeapon() || blow.WeaponRecord.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.NoBlood) ? 1 : (collisionData.IsAlternativeAttack ? 1 : 0)) != 0)
//        MissionGameModels.Current.DamageParticleModel.GetMeleeAttackSweatParticles(attacker, victim, in blow, in collisionData, out hprd);
//      else
//        MissionGameModels.Current.DamageParticleModel.GetMeleeAttackBloodParticles(attacker, victim, in blow, in collisionData, out hprd);
//    }

//    private void RegisterBlow(
//      Agent attacker,
//      Agent victim,
//      WeakGameEntity realHitEntity,
//      Blow b,
//      ref AttackCollisionData collisionData,
//      in MissionWeapon attackerWeapon,
//      ref CombatLogData combatLogData)
//    {
//      b.VictimBodyPart = collisionData.VictimHitBodyPart;
//      if (!collisionData.AttackBlockedWithShield)
//      {
//        if (collisionData.IsColliderAgent)
//        {
//          if (b.SelfInflictedDamage > 0 && attacker != null && attacker.IsActive() && attacker.IsFriendOf(victim))
//          {
//            Blow outBlow;
//            AttackCollisionData collisionData1;
//            attacker.CreateBlowFromBlowAsReflection(in b, in collisionData, out outBlow, out collisionData1);
//            if (victim.IsMount && attacker.MountAgent != null)
//              attacker.MountAgent.RegisterBlow(outBlow, in collisionData1);
//            else
//              attacker.RegisterBlow(outBlow, in collisionData1);
//          }
//          if (b.InflictedDamage > 0)
//          {
//            combatLogData.IsFatalDamage = victim != null && (double) victim.Health - (double) b.InflictedDamage < 1.0;
//            combatLogData.InflictedDamage = b.InflictedDamage - combatLogData.ModifiedDamage;
//            this.PrintAttackCollisionResults(attacker, victim, (MissionObject) null, ref collisionData, ref combatLogData);
//          }
//          victim.RegisterBlow(b, in collisionData);
//        }
//        else if (collisionData.EntityExists)
//        {
//          MissionWeapon weapon = b.IsMissile ? this._missilesDictionary[b.WeaponRecord.AffectorWeaponSlotOrMissileIndex].Weapon : (attacker == null || !b.WeaponRecord.HasWeapon() ? MissionWeapon.Invalid : attacker.Equipment[b.WeaponRecord.AffectorWeaponSlotOrMissileIndex]);
//          this.OnEntityHit(realHitEntity, attacker, b.InflictedDamage, (DamageTypes) collisionData.DamageType, b.GlobalPosition, b.SwingDirection, in weapon, b.WeaponRecord.AffectorWeaponSlotOrMissileIndex, ref combatLogData);
//          if (attacker != null && b.SelfInflictedDamage > 0 && attacker.IsActive())
//          {
//            Blow outBlow;
//            AttackCollisionData collisionData2;
//            attacker.CreateBlowFromBlowAsReflection(in b, in collisionData, out outBlow, out collisionData2);
//            attacker.RegisterBlow(outBlow, in collisionData2);
//          }
//        }
//      }
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnRegisterBlow(attacker, victim, realHitEntity, b, ref collisionData, in attackerWeapon);
//    }

//    private Blow CreateMissileBlow(
//      Agent attackerAgent,
//      in AttackCollisionData collisionData,
//      in MissionWeapon attackerWeapon,
//      Vec3 missilePosition,
//      Vec3 missileStartingPosition)
//    {
//      Blow missileBlow = new Blow(attackerAgent != null ? attackerAgent.Index : -1)
//      {
//        BlowFlag = attackerWeapon.CurrentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CanKnockDown) ? BlowFlags.KnockDown : BlowFlags.None,
//        Direction = collisionData.MissileVelocity.NormalizedCopy()
//      };
//      missileBlow.SwingDirection = missileBlow.Direction;
//      missileBlow.GlobalPosition = collisionData.CollisionGlobalPosition;
//      ref Blow local1 = ref missileBlow;
//      AttackCollisionData attackCollisionData = collisionData;
//      int collisionBoneIndex = (int) attackCollisionData.CollisionBoneIndex;
//      local1.BoneIndex = (sbyte) collisionBoneIndex;
//      ref Blow local2 = ref missileBlow;
//      attackCollisionData = collisionData;
//      int strikeType = attackCollisionData.StrikeType;
//      local2.StrikeType = (StrikeType) strikeType;
//      missileBlow.DamageType = (DamageTypes) collisionData.DamageType;
//      missileBlow.VictimBodyPart = collisionData.VictimHitBodyPart;
//      sbyte weaponAttachBoneIndex = attackerAgent != null ? attackerAgent.Monster.GetBoneToAttachForItemFlags(attackerWeapon.Item.ItemFlags) : (sbyte) -1;
//      missileBlow.WeaponRecord.FillAsMissileBlow(attackerWeapon.Item, attackerWeapon.CurrentUsageItem, collisionData.AffectorWeaponSlotOrMissileIndex, weaponAttachBoneIndex, missileStartingPosition, missilePosition, collisionData.MissileVelocity);
//      missileBlow.BaseMagnitude = collisionData.BaseMagnitude;
//      missileBlow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
//      missileBlow.AbsorbedByArmor = (float) collisionData.AbsorbedByArmor;
//      missileBlow.InflictedDamage = collisionData.InflictedDamage;
//      missileBlow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
//      missileBlow.DamageCalculated = true;
//      return missileBlow;
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal float OnAgentHitBlocked(
//      Agent affectedAgent,
//      Agent affectorAgent,
//      ref AttackCollisionData collisionData,
//      Vec3 blowDirection,
//      Vec3 swingDirection,
//      bool isMissile)
//    {
//      Blow b;
//      if (isMissile)
//      {
//        Mission.Missile missiles = this._missilesDictionary[collisionData.AffectorWeaponSlotOrMissileIndex];
//        MissionWeapon attackerWeapon = missiles.Weapon;
//        b = this.CreateMissileBlow(affectorAgent, in collisionData, in attackerWeapon, missiles.GetPosition(), collisionData.MissileStartingPosition);
//      }
//      else
//      {
//        int slotOrMissileIndex = collisionData.AffectorWeaponSlotOrMissileIndex;
//        MissionWeapon attackerWeapon = slotOrMissileIndex >= 0 ? affectorAgent.Equipment[slotOrMissileIndex] : MissionWeapon.Invalid;
//        b = this.CreateMeleeBlow(affectorAgent, affectedAgent, in collisionData, in attackerWeapon, CrushThroughState.None, blowDirection, swingDirection, true);
//      }
//      return this.OnAgentHit(affectedAgent, affectorAgent, in b, in collisionData, true, 0.0f);
//    }

//    private Blow CreateMeleeBlow(
//      Agent attackerAgent,
//      Agent victimAgent,
//      in AttackCollisionData collisionData,
//      in MissionWeapon attackerWeapon,
//      CrushThroughState crushThroughState,
//      Vec3 blowDirection,
//      Vec3 swingDirection,
//      bool cancelDamage)
//    {
//      Blow blow = new Blow(attackerAgent.Index);
//      blow.VictimBodyPart = collisionData.VictimHitBodyPart;
//      bool flag = MissionCombatMechanicsHelper.HitWithAnotherBone(in collisionData, attackerAgent, in attackerWeapon);
//      MissionWeapon missionWeapon;
//      if (collisionData.IsAlternativeAttack)
//      {
//        ref Blow local = ref blow;
//        missionWeapon = attackerWeapon;
//        int num = missionWeapon.IsEmpty ? 1 : 2;
//        local.AttackType = (AgentAttackType) num;
//      }
//      else
//        blow.AttackType = AgentAttackType.Standard;
//      missionWeapon = attackerWeapon;
//      int num1;
//      if (!missionWeapon.IsEmpty)
//      {
//        Monster monster = attackerAgent.Monster;
//        missionWeapon = attackerWeapon;
//        int itemFlags = (int) missionWeapon.Item.ItemFlags;
//        num1 = (int) monster.GetBoneToAttachForItemFlags((ItemFlags) itemFlags);
//      }
//      else
//        num1 = -1;
//      sbyte num2 = (sbyte) num1;
//      ref BlowWeaponRecord local1 = ref blow.WeaponRecord;
//      missionWeapon = attackerWeapon;
//      ItemObject itemObject = missionWeapon.Item;
//      missionWeapon = attackerWeapon;
//      WeaponComponentData currentUsageItem1 = missionWeapon.CurrentUsageItem;
//      int slotOrMissileIndex = collisionData.AffectorWeaponSlotOrMissileIndex;
//      int weaponAttachBoneIndex = (int) num2;
//      local1.FillAsMeleeBlow(itemObject, currentUsageItem1, slotOrMissileIndex, (sbyte) weaponAttachBoneIndex);
//      blow.StrikeType = (StrikeType) collisionData.StrikeType;
//      ref Blow local2 = ref blow;
//      missionWeapon = attackerWeapon;
//      AttackCollisionData attackCollisionData;
//      int num3;
//      if (!missionWeapon.IsEmpty && !flag)
//      {
//        attackCollisionData = collisionData;
//        if (!attackCollisionData.IsAlternativeAttack)
//        {
//          attackCollisionData = collisionData;
//          num3 = attackCollisionData.DamageType;
//          goto label_10;
//        }
//      }
//      num3 = 2;
//label_10:
//      local2.DamageType = (DamageTypes) num3;
//      ref Blow local3 = ref blow;
//      attackCollisionData = collisionData;
//      int num4 = attackCollisionData.IsAlternativeAttack ? 1 : 0;
//      local3.NoIgnore = num4 != 0;
//      ref Blow local4 = ref blow;
//      attackCollisionData = collisionData;
//      double attackerStunPeriod = (double) attackCollisionData.AttackerStunPeriod;
//      local4.AttackerStunPeriod = (float) attackerStunPeriod;
//      ref Blow local5 = ref blow;
//      attackCollisionData = collisionData;
//      double defenderStunPeriod = (double) attackCollisionData.DefenderStunPeriod;
//      local5.DefenderStunPeriod = (float) defenderStunPeriod;
//      blow.BlowFlag = BlowFlags.None;
//      ref Blow local6 = ref blow;
//      attackCollisionData = collisionData;
//      Vec3 collisionGlobalPosition = attackCollisionData.CollisionGlobalPosition;
//      local6.GlobalPosition = collisionGlobalPosition;
//      ref Blow local7 = ref blow;
//      attackCollisionData = collisionData;
//      int collisionBoneIndex = (int) attackCollisionData.CollisionBoneIndex;
//      local7.BoneIndex = (sbyte) collisionBoneIndex;
//      blow.Direction = blowDirection;
//      attackCollisionData = collisionData;
//      if (attackCollisionData.CollidedWithLastBoneSegment)
//      {
//        ref Blow local8 = ref blow;
//        attackCollisionData = collisionData;
//        Vec3 boneSegmentSwingDir = attackCollisionData.LastBoneSegmentSwingDir;
//        local8.SwingDirection = boneSegmentSwingDir;
//      }
//      else
//        blow.SwingDirection = swingDirection;
//      if (cancelDamage)
//      {
//        blow.BaseMagnitude = 0.0f;
//        blow.MovementSpeedDamageModifier = 0.0f;
//        blow.InflictedDamage = 0;
//        blow.SelfInflictedDamage = 0;
//        blow.AbsorbedByArmor = 0.0f;
//      }
//      else
//      {
//        blow.BaseMagnitude = collisionData.BaseMagnitude;
//        blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
//        blow.InflictedDamage = collisionData.InflictedDamage;
//        blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
//        blow.AbsorbedByArmor = (float) collisionData.AbsorbedByArmor;
//      }
//      blow.DamageCalculated = true;
//      if (crushThroughState != CrushThroughState.None)
//        blow.BlowFlag |= BlowFlags.CrushThrough;
//      if (blow.StrikeType == StrikeType.Thrust)
//      {
//        attackCollisionData = collisionData;
//        if (!attackCollisionData.ThrustTipHit)
//          blow.BlowFlag |= BlowFlags.NonTipThrust;
//      }
//      attackCollisionData = collisionData;
//      if (attackCollisionData.IsColliderAgent)
//      {
//        if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentShrugOffBlow(victimAgent, in collisionData, in blow))
//          blow.BlowFlag |= BlowFlags.ShrugOff;
//        if (victimAgent.IsHuman)
//        {
//          Agent mountAgent = victimAgent.MountAgent;
//          if (mountAgent != null)
//          {
//            if (mountAgent.RiderAgent == victimAgent)
//            {
//              AgentApplyDamageModel applyDamageModel = MissionGameModels.Current.AgentApplyDamageModel;
//              Agent attackerAgent1 = attackerAgent;
//              Agent victimAgent1 = victimAgent;
//              ref readonly AttackCollisionData local9 = ref collisionData;
//              missionWeapon = attackerWeapon;
//              WeaponComponentData currentUsageItem2 = missionWeapon.CurrentUsageItem;
//              ref Blow local10 = ref blow;
//              if (applyDamageModel.DecideAgentDismountedByBlow(attackerAgent1, victimAgent1, in local9, currentUsageItem2, in local10))
//                blow.BlowFlag |= BlowFlags.CanDismount;
//            }
//          }
//          else
//          {
//            AgentApplyDamageModel applyDamageModel1 = MissionGameModels.Current.AgentApplyDamageModel;
//            Agent attackerAgent2 = attackerAgent;
//            Agent victimAgent2 = victimAgent;
//            ref readonly AttackCollisionData local11 = ref collisionData;
//            missionWeapon = attackerWeapon;
//            WeaponComponentData currentUsageItem3 = missionWeapon.CurrentUsageItem;
//            ref Blow local12 = ref blow;
//            if (applyDamageModel1.DecideAgentKnockedBackByBlow(attackerAgent2, victimAgent2, in local11, currentUsageItem3, in local12))
//              blow.BlowFlag |= BlowFlags.KnockBack;
//            AgentApplyDamageModel applyDamageModel2 = MissionGameModels.Current.AgentApplyDamageModel;
//            Agent attackerAgent3 = attackerAgent;
//            Agent victimAgent3 = victimAgent;
//            ref readonly AttackCollisionData local13 = ref collisionData;
//            missionWeapon = attackerWeapon;
//            WeaponComponentData currentUsageItem4 = missionWeapon.CurrentUsageItem;
//            ref Blow local14 = ref blow;
//            if (applyDamageModel2.DecideAgentKnockedDownByBlow(attackerAgent3, victimAgent3, in local13, currentUsageItem4, in local14))
//              blow.BlowFlag |= BlowFlags.KnockDown;
//          }
//        }
//        else if (victimAgent.IsMount)
//        {
//          AgentApplyDamageModel applyDamageModel = MissionGameModels.Current.AgentApplyDamageModel;
//          Agent attackerAgent4 = attackerAgent;
//          Agent victimAgent4 = victimAgent;
//          ref readonly AttackCollisionData local15 = ref collisionData;
//          missionWeapon = attackerWeapon;
//          WeaponComponentData currentUsageItem5 = missionWeapon.CurrentUsageItem;
//          ref Blow local16 = ref blow;
//          if (applyDamageModel.DecideMountRearedByBlow(attackerAgent4, victimAgent4, in local15, currentUsageItem5, in local16))
//            blow.BlowFlag |= BlowFlags.MakesRear;
//        }
//      }
//      return blow;
//    }

//    internal float OnAgentHit(
//      Agent affectedAgent,
//      Agent affectorAgent,
//      in Blow b,
//      in AttackCollisionData collisionData,
//      bool isBlocked,
//      float damagedHp)
//    {
//      float shotDifficulty = -1f;
//      bool isSiegeEngineHit = false;
//      int slotOrMissileIndex = b.WeaponRecord.AffectorWeaponSlotOrMissileIndex;
//      bool isMissile = b.IsMissile;
//      int inflictedDamage = b.InflictedDamage;
//      float hitDistance = b.IsMissile ? (b.GlobalPosition - b.WeaponRecord.StartingPosition).Length : 0.0f;
//      MissionWeapon affectorWeapon;
//      if (isMissile)
//      {
//        Mission.Missile missiles = this._missilesDictionary[slotOrMissileIndex];
//        affectorWeapon = missiles.Weapon;
//        isSiegeEngineHit = missiles.MissionObjectToIgnore != null;
//      }
//      else
//        affectorWeapon = affectorAgent == null || slotOrMissileIndex < 0 ? MissionWeapon.Invalid : affectorAgent.Equipment[slotOrMissileIndex];
//      if (affectorAgent != null & isMissile)
//        shotDifficulty = this.GetShootDifficulty(affectedAgent, affectorAgent, b.VictimBodyPart == BoneBodyPartType.Head);
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//      {
//        missionBehavior.OnAgentHit(affectedAgent, affectorAgent, in affectorWeapon, in b, in collisionData);
//        missionBehavior.OnScoreHit(affectedAgent, affectorAgent, affectorWeapon.CurrentUsageItem, isBlocked, isSiegeEngineHit, in b, in collisionData, damagedHp, hitDistance, shotDifficulty);
//      }
//      foreach (AgentComponent component in (List<AgentComponent>) affectedAgent.Components)
//        component.OnHit(affectorAgent, inflictedDamage, in affectorWeapon, in b, in collisionData);
//      affectedAgent.CheckToDropFlaggedItem();
//      return (float) inflictedDamage;
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void MissileAreaDamageCallback(
//      ref AttackCollisionData collisionDataInput,
//      ref Blow blowInput,
//      Agent alreadyDamagedAgent,
//      Agent shooterAgent,
//      bool isBigExplosion)
//    {
//      float searchRadius = isBigExplosion ? 2.8f : 1.2f;
//      float num1 = isBigExplosion ? 1.6f : 1f;
//      float num2 = 1f;
//      if ((double) collisionDataInput.MissileVelocity.LengthSquared < 484.0)
//      {
//        num1 *= 0.8f;
//        num2 = 0.5f;
//      }
//      AttackCollisionData attackCollisionData1 = collisionDataInput;
//      blowInput.VictimBodyPart = collisionDataInput.VictimHitBodyPart;
//      List<Agent> agentList = new List<Agent>();
//      AgentProximityMap.ProximityMapSearchStruct searchStruct = AgentProximityMap.BeginSearch(this, blowInput.GlobalPosition.AsVec2, searchRadius, true);
//      while (searchStruct.LastFoundAgent != null)
//      {
//        Agent lastFoundAgent = searchStruct.LastFoundAgent;
//        if (lastFoundAgent.CurrentMortalityState != Agent.MortalityState.Invulnerable && lastFoundAgent != shooterAgent && lastFoundAgent != alreadyDamagedAgent)
//          agentList.Add(lastFoundAgent);
//        AgentProximityMap.FindNext(this, ref searchStruct);
//      }
//      foreach (Agent agent in agentList)
//      {
//        Blow b = blowInput with { DamageCalculated = false };
//        AttackCollisionData attackCollisionData2 = collisionDataInput;
//        float x = float.MaxValue;
//        sbyte boneIndex1 = -1;
//        Skeleton skeleton = agent.AgentVisuals.GetSkeleton();
//        sbyte boneCount = skeleton.GetBoneCount();
//        MatrixFrame globalFrame = agent.AgentVisuals.GetGlobalFrame();
//        for (sbyte boneIndex2 = 0; (int) boneIndex2 < (int) boneCount; ++boneIndex2)
//        {
//          float num3 = globalFrame.TransformToParent(in skeleton.GetBoneEntitialFrame(boneIndex2).origin).DistanceSquared(blowInput.GlobalPosition);
//          if ((double) num3 < (double) x)
//          {
//            boneIndex1 = boneIndex2;
//            x = num3;
//          }
//        }
//        if ((double) x <= (double) searchRadius * (double) searchRadius)
//        {
//          float num4 = MathF.Sqrt(x);
//          float num5 = 1f;
//          if ((double) num4 > (double) num1)
//          {
//            float num6 = MBMath.Lerp(1f, 3f, (float) (((double) num4 - (double) num1) / ((double) searchRadius - (double) num1)));
//            num5 = (float) (1.0 / ((double) num6 * (double) num6));
//          }
//          float num7 = num5 * num2;
//          attackCollisionData2.SetCollisionBoneIndexForAreaDamage(boneIndex1);
//          MissionWeapon attackerWeapon = this._missilesDictionary[attackCollisionData2.AffectorWeaponSlotOrMissileIndex].Weapon;
//          CombatLogData combatLog;
//          this.GetAttackCollisionResults(shooterAgent, agent, WeakGameEntity.Invalid, 1f, in attackerWeapon, false, false, false, ref attackCollisionData2, out WeaponComponentData _, out combatLog);
//          b.BaseMagnitude = attackCollisionData2.BaseMagnitude;
//          b.MovementSpeedDamageModifier = attackCollisionData2.MovementSpeedDamageModifier;
//          b.InflictedDamage = attackCollisionData2.InflictedDamage;
//          b.SelfInflictedDamage = attackCollisionData2.SelfInflictedDamage;
//          b.AbsorbedByArmor = (float) attackCollisionData2.AbsorbedByArmor;
//          b.DamageCalculated = true;
//          b.InflictedDamage = MathF.Round((float) b.InflictedDamage * num7);
//          b.SelfInflictedDamage = MathF.Round((float) b.SelfInflictedDamage * num7);
//          combatLog.ModifiedDamage = MathF.Round((float) combatLog.ModifiedDamage * num7);
//          this.RegisterBlow(shooterAgent, agent, WeakGameEntity.Invalid, b, ref attackCollisionData2, in attackerWeapon, ref combatLog);
//        }
//      }
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void OnMissileRemoved(int missileIndex)
//    {
//      this._missilesDictionary.Remove(missileIndex);
//      for (int index = 0; index < this._missilesList.Count; ++index)
//      {
//        if (this._missilesList[index].Index == missileIndex)
//        {
//          this._missilesList.RemoveAt(index);
//          break;
//        }
//      }
//      Action<int> missileRemovedEvent = this.OnMissileRemovedEvent;
//      if (missileRemovedEvent == null)
//        return;
//      missileRemovedEvent(missileIndex);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal bool MissileHitCallback(
//      out int extraHitParticleIndex,
//      ref AttackCollisionData collisionData,
//      Vec3 missileStartingPosition,
//      Vec3 missilePosition,
//      Vec3 missileAngularVelocity,
//      Vec3 movementVelocity,
//      MatrixFrame attachGlobalFrame,
//      MatrixFrame affectedShieldGlobalFrame,
//      int numDamagedAgents,
//      Agent attacker,
//      Agent victim,
//      GameEntity hitEntity)
//    {
//      WeakGameEntity weakGameEntity1 = (object) hitEntity != null ? hitEntity.WeakEntity : WeakGameEntity.Invalid;
//      Mission.Missile missiles = this._missilesDictionary[collisionData.AffectorWeaponSlotOrMissileIndex];
//      MissionWeapon attackerWeapon = missiles.Weapon;
//      WeaponFlags weaponFlags1 = attackerWeapon.CurrentUsageItem.WeaponFlags;
//      float momentumRemaining = 1f;
//      WeaponComponentData shieldOnBack = (WeaponComponentData) null;
//      MissionGameModels.Current.AgentApplyDamageModel.DecideMissileWeaponFlags(attacker, missiles.Weapon, ref weaponFlags1);
//      extraHitParticleIndex = -1;
//      bool flag1 = !GameNetwork.IsSessionActive;
//      bool missileHasPhysics = collisionData.MissileHasPhysics;
//      PhysicsMaterial fromIndex = PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex);
//      int flags = fromIndex.IsValid ? (int) fromIndex.GetFlags() : 0;
//      bool flag2 = (weaponFlags1 & WeaponFlags.AmmoSticksWhenShot) > (WeaponFlags) 0;
//      bool flag3 = (flags & 1) == 0;
//      bool flag4 = (flags & 8) != 0;
//      MissionObject attachedMissionObject = (MissionObject) null;
//      if (victim == null && weakGameEntity1.IsValid)
//      {
//        WeakGameEntity weakGameEntity2 = weakGameEntity1;
//        do
//        {
//          attachedMissionObject = weakGameEntity2.GetFirstScriptOfType<MissionObject>();
//          weakGameEntity2 = weakGameEntity2.Parent;
//        }
//        while (attachedMissionObject == null && weakGameEntity2.IsValid);
//        weakGameEntity1 = attachedMissionObject != null ? attachedMissionObject.GameEntity : WeakGameEntity.Invalid;
//      }
//      Mission.MissileCollisionReaction collisionReaction1 = !flag4 ? (!weaponFlags1.HasAnyFlag<WeaponFlags>(WeaponFlags.Burning) ? (!flag3 || !flag2 ? Mission.MissileCollisionReaction.BounceBack : Mission.MissileCollisionReaction.Stick) : Mission.MissileCollisionReaction.BecomeInvisible) : Mission.MissileCollisionReaction.PassThrough;
//      bool isCanceled = false;
//      bool flag5 = victim != null && victim.CurrentMortalityState == Agent.MortalityState.Invulnerable;
//      Mission.MissileCollisionReaction collisionReaction2;
//      CombatLogData combatLog1;
//      if (((collisionData.MissileGoneUnderWater ? 1 : (collisionData.MissileGoneOutOfBorder ? 1 : 0)) | (flag5 ? 1 : 0)) != 0)
//        collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
//      else if (victim == null)
//      {
//        if (weakGameEntity1.IsValid)
//        {
//          CombatLogData combatLog2;
//          this.GetAttackCollisionResults(attacker, victim, weakGameEntity1, momentumRemaining, in attackerWeapon, false, false, false, ref collisionData, out shieldOnBack, out combatLog2);
//          Blow missileBlow = this.CreateMissileBlow(attacker, in collisionData, in attackerWeapon, missilePosition, missileStartingPosition);
//          this.RegisterBlow(attacker, (Agent) null, weakGameEntity1, missileBlow, ref collisionData, in attackerWeapon, ref combatLog2);
//        }
//        collisionReaction2 = collisionReaction1;
//      }
//      else if (collisionData.AttackBlockedWithShield)
//      {
//        this.GetAttackCollisionResults(attacker, victim, weakGameEntity1, momentumRemaining, in attackerWeapon, false, false, false, ref collisionData, out shieldOnBack, out combatLog1);
//        if (!collisionData.IsShieldBroken)
//          this.MakeSound(ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeStone, collisionData.CollisionGlobalPosition, false, false, -1, -1);
//        bool flag6 = false;
//        if (weaponFlags1.HasAnyFlag<WeaponFlags>(WeaponFlags.CanPenetrateShield))
//        {
//          if (!collisionData.IsShieldBroken)
//          {
//            EquipmentIndex wieldedItemIndex = victim.GetOffhandWieldedItemIndex();
//            if ((double) collisionData.InflictedDamage > (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldPenetrationOffset) + (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldPenetrationFactor) * (double) victim.Equipment[wieldedItemIndex].GetGetModifiedArmorForCurrentUsage())
//              flag6 = true;
//          }
//          else
//            flag6 = true;
//        }
//        else if (victim.State == AgentState.Active && collisionData.IsShieldBroken && MissionGameModels.Current.AgentApplyDamageModel.ShouldMissilePassThroughAfterShieldBreak(attacker, attackerWeapon.CurrentUsageItem))
//          flag6 = true;
//        if (flag6)
//        {
//          victim.MakeVoice(SkinVoiceManager.VoiceType.Pain, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
//          float num = momentumRemaining * (float) (0.40000000596046448 + (double) MBRandom.RandomFloat * 0.20000000298023224);
//          collisionReaction2 = Mission.MissileCollisionReaction.PassThrough;
//        }
//        else
//          collisionReaction2 = collisionData.IsShieldBroken ? Mission.MissileCollisionReaction.BecomeInvisible : collisionReaction1;
//      }
//      else if (collisionData.MissileBlockedWithWeapon)
//      {
//        this.GetAttackCollisionResults(attacker, victim, weakGameEntity1, momentumRemaining, in attackerWeapon, false, false, false, ref collisionData, out shieldOnBack, out combatLog1);
//        collisionReaction2 = Mission.MissileCollisionReaction.BounceBack;
//      }
//      else
//      {
//        if (attacker != null && attacker.IsFriendOf(victim))
//        {
//          if (this.ForceNoFriendlyFire)
//            isCanceled = true;
//          else if (!missileHasPhysics)
//          {
//            if (flag1)
//            {
//              if (attacker.Controller == AgentControllerType.AI)
//                isCanceled = true;
//            }
//            else if (MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.GetIntValue() <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.GetIntValue() <= 0 || this.Mode == MissionMode.Duel)
//              isCanceled = true;
//          }
//        }
//        else if (victim.IsHuman && attacker != null && !attacker.IsEnemyOf(victim))
//          isCanceled = true;
//        else if (flag1 && attacker != null && attacker.Controller == AgentControllerType.AI && victim.RiderAgent != null && attacker.IsFriendOf(victim.RiderAgent))
//          isCanceled = true;
//        if (isCanceled)
//        {
//          if (flag1 && attacker != null && attacker == Agent.Main && attacker.IsFriendOf(victim))
//            InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_you_hit_a_friendly_troop").ToString(), Color.ConvertStringToColor("#D65252FF")));
//          collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
//        }
//        else
//        {
//          bool flag7 = (weaponFlags1 & WeaponFlags.MultiplePenetration) > (WeaponFlags) 0;
//          CombatLogData combatLog3;
//          this.GetAttackCollisionResults(attacker, victim, WeakGameEntity.Invalid, momentumRemaining, in attackerWeapon, false, false, false, ref collisionData, out shieldOnBack, out combatLog3);
//          Blow blow = this.CreateMissileBlow(attacker, in collisionData, in attackerWeapon, missilePosition, missileStartingPosition);
//          if (collisionData.IsColliderAgent & flag7 && numDamagedAgents > 0)
//          {
//            blow.InflictedDamage /= numDamagedAgents;
//            blow.SelfInflictedDamage /= numDamagedAgents;
//            combatLog3.InflictedDamage = blow.InflictedDamage - combatLog3.ModifiedDamage;
//          }
//          if (collisionData.IsColliderAgent)
//          {
//            if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentShrugOffBlow(victim, in collisionData, in blow))
//              blow.BlowFlag |= BlowFlags.ShrugOff;
//            else if (victim.IsHuman)
//            {
//              Agent mountAgent = victim.MountAgent;
//              if (mountAgent != null)
//              {
//                if (mountAgent.RiderAgent == victim && MissionGameModels.Current.AgentApplyDamageModel.DecideAgentDismountedByBlow(attacker, victim, in collisionData, attackerWeapon.CurrentUsageItem, in blow))
//                  blow.BlowFlag |= BlowFlags.CanDismount;
//              }
//              else
//              {
//                if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentKnockedBackByBlow(attacker, victim, in collisionData, attackerWeapon.CurrentUsageItem, in blow))
//                  blow.BlowFlag |= BlowFlags.KnockBack;
//                if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentKnockedDownByBlow(attacker, victim, in collisionData, attackerWeapon.CurrentUsageItem, in blow))
//                  blow.BlowFlag |= BlowFlags.KnockDown;
//              }
//            }
//          }
//          if (victim.State == AgentState.Active)
//            this.RegisterBlow(attacker, victim, WeakGameEntity.Invalid, blow, ref collisionData, in attackerWeapon, ref combatLog3);
//          extraHitParticleIndex = MissionGameModels.Current.DamageParticleModel.GetMissileAttackParticle(attacker, victim, in blow, in collisionData);
//          if (flag7 && numDamagedAgents < 3)
//          {
//            collisionReaction2 = Mission.MissileCollisionReaction.PassThrough;
//          }
//          else
//          {
//            collisionReaction2 = collisionReaction1;
//            if (collisionReaction1 == Mission.MissileCollisionReaction.Stick && !collisionData.CollidedWithShieldOnBack)
//            {
//              bool flag8 = this.CombatType == Mission.MissionCombatType.Combat;
//              if (flag8)
//              {
//                bool flag9 = victim.IsHuman && collisionData.VictimHitBodyPart == BoneBodyPartType.Head;
//                flag8 = victim.State != AgentState.Active || !flag9;
//              }
//              if (flag8)
//              {
//                float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.MissileMinimumDamageToStick);
//                float num = 2f * managedParameter;
//                if ((double) blow.InflictedDamage < (double) managedParameter && (double) blow.AbsorbedByArmor > (double) num && !GameNetwork.IsClientOrReplay)
//                  collisionReaction2 = Mission.MissileCollisionReaction.BounceBack;
//              }
//              else
//                collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
//            }
//          }
//        }
//      }
//      if (collisionData.CollidedWithShieldOnBack && shieldOnBack != null && victim != null && victim.IsMainAgent)
//        InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back").ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
//      bool isAttachedFrameLocal;
//      MatrixFrame attachLocalFrame;
//      MissionWeapon weapon;
//      if (!collisionData.MissileHasPhysics && collisionReaction2 == Mission.MissileCollisionReaction.Stick)
//      {
//        attachLocalFrame = this.CalculateAttachedLocalFrame(in attachGlobalFrame, collisionData, missiles.Weapon.CurrentUsageItem, victim, weakGameEntity1, movementVelocity, missileAngularVelocity, affectedShieldGlobalFrame, true, out isAttachedFrameLocal);
//      }
//      else
//      {
//        ref MatrixFrame local1 = ref attachGlobalFrame;
//        MatrixFrame missileStartingFrame = missiles.Weapon.CurrentUsageItem.GetMissileStartingFrame();
//        ref MatrixFrame local2 = ref missileStartingFrame;
//        weapon = missiles.Weapon;
//        // ISSUE: explicit reference operation
//        ref MatrixFrame local3 = @weapon.CurrentUsageItem.StickingFrame;
//        // ISSUE: explicit reference operation
//        ref MatrixFrame local4 = @local2.TransformToParent(in local3);
//        MatrixFrame parent = local1.TransformToParent(in local4);
//        ref MatrixFrame local5 = ref parent;
//        weapon = missiles.Weapon;
//        // ISSUE: explicit reference operation
//        ref MatrixFrame local6 = @weapon.CurrentUsageItem.GetMissileStartingFrame();
//        attachLocalFrame = local5.TransformToParent(in local6);
//        attachLocalFrame.origin.z = Math.Max(attachLocalFrame.origin.z, -100f);
//        attachedMissionObject = (MissionObject) null;
//        isAttachedFrameLocal = false;
//      }
//      Vec3 velocity = Vec3.Zero;
//      Vec3 angularVelocity = Vec3.Zero;
//      Vec3 vec3;
//      if (collisionReaction2 == Mission.MissileCollisionReaction.BounceBack)
//      {
//        WeaponFlags weaponFlags2 = weaponFlags1 & WeaponFlags.AmmoBreakOnBounceBackMask;
//        if (weaponFlags2 == WeaponFlags.AmmoCanBreakOnBounceBack)
//        {
//          vec3 = collisionData.MissileVelocity;
//          if ((double) vec3.Length > (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BreakableProjectileMinimumBreakSpeed))
//            goto label_75;
//        }
//        if (weaponFlags2 != WeaponFlags.AmmoBreaksOnBounceBack)
//        {
//          missiles.CalculateBounceBackVelocity(missileAngularVelocity, collisionData, out velocity, out angularVelocity);
//          goto label_78;
//        }
//label_75:
//        collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
//        if (attackerWeapon.Item.ItemType != ItemObject.ItemTypeEnum.SlingStones)
//          extraHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_broken_arrow");
//      }
//label_78:
//      if (missiles.ShooterAgent != null && (collisionReaction2 == Mission.MissileCollisionReaction.Stick || collisionReaction2 == Mission.MissileCollisionReaction.BounceBack) && (victim == null || collisionData.AttackBlockedWithShield || collisionData.MissileBlockedWithWeapon))
//      {
//        weapon = missiles.Weapon;
//        int num1;
//        if (weapon.CurrentUsageItem.WeaponClass != WeaponClass.Stone)
//        {
//          weapon = missiles.Weapon;
//          if (weapon.CurrentUsageItem.WeaponClass != WeaponClass.Boulder)
//          {
//            weapon = missiles.Weapon;
//            if (weapon.CurrentUsageItem.WeaponClass != WeaponClass.BallistaStone)
//            {
//              weapon = missiles.Weapon;
//              num1 = weapon.CurrentUsageItem.WeaponClass == WeaponClass.BallistaBoulder ? 1 : 0;
//              goto label_84;
//            }
//          }
//        }
//        num1 = 1;
//label_84:
//        double num2;
//        if (num1 == 0)
//        {
//          weapon = missiles.Weapon;
//          num2 = weapon.CurrentUsageItem.IsAmmo ? 7.0 : 9.0;
//        }
//        else
//          num2 = 13.100000381469727;
//        float num3 = (float) num2;
//        Agent shooterAgent = missiles.ShooterAgent;
//        vec3 = missiles.GetPosition();
//        ref Vec3 local = ref vec3;
//        double soundLevelSquareRoot = (double) num3;
//        this.AddSoundAlarmFactorToAgents(shooterAgent, in local, (float) soundLevelSquareRoot);
//      }
//      this.HandleMissileCollisionReaction(collisionData.AffectorWeaponSlotOrMissileIndex, collisionReaction2, attachLocalFrame, isAttachedFrameLocal, attacker, victim, collisionData.AttackBlockedWithShield, collisionData.CollisionBoneIndex, attachedMissionObject, velocity, angularVelocity, -1);
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnMissileHit(attacker, victim, isCanceled, collisionData);
//      return collisionReaction2 != Mission.MissileCollisionReaction.PassThrough;
//    }

//    public void HandleMissileCollisionReaction(
//      int missileIndex,
//      Mission.MissileCollisionReaction collisionReaction,
//      MatrixFrame attachLocalFrame,
//      bool isAttachedFrameLocal,
//      Agent attackerAgent,
//      Agent attachedAgent,
//      bool attachedToShield,
//      sbyte attachedBoneIndex,
//      MissionObject attachedMissionObject,
//      Vec3 bounceBackVelocity,
//      Vec3 bounceBackAngularVelocity,
//      int forcedSpawnIndex)
//    {
//      Mission.Missile missiles = this._missilesDictionary[missileIndex];
//      MissionObjectId missionObjectId = new MissionObjectId(-1, true);
//      switch (collisionReaction)
//      {
//        case Mission.MissileCollisionReaction.Stick:
//          missiles.Entity.SetVisibilityExcludeParents(true);
//          if (attachedAgent != null)
//          {
//            this.PrepareMissileWeaponForDrop(missileIndex);
//            if (attachedToShield)
//            {
//              EquipmentIndex wieldedItemIndex = attachedAgent.GetOffhandWieldedItemIndex();
//              attachedAgent.AttachWeaponToWeapon(wieldedItemIndex, missiles.Weapon, missiles.Entity, ref attachLocalFrame);
//              break;
//            }
//            attachedAgent.AttachWeaponToBone(missiles.Weapon, missiles.Entity, attachedBoneIndex, ref attachLocalFrame);
//            break;
//          }
//          Vec3 zero = Vec3.Zero;
//          missionObjectId = this.SpawnWeaponAsDropFromMissile(missileIndex, attachedMissionObject, in attachLocalFrame, Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithStaticPhysics, in zero, in zero, forcedSpawnIndex);
//          break;
//        case Mission.MissileCollisionReaction.BounceBack:
//          missiles.Entity.SetVisibilityExcludeParents(true);
//          missionObjectId = this.SpawnWeaponAsDropFromMissile(missileIndex, (MissionObject) null, in attachLocalFrame, Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithPhysics, in bounceBackVelocity, in bounceBackAngularVelocity, forcedSpawnIndex);
//          break;
//        case Mission.MissileCollisionReaction.BecomeInvisible:
//          missiles.Entity.Remove(81);
//          break;
//      }
//      bool flag = collisionReaction != Mission.MissileCollisionReaction.PassThrough;
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.HandleMissileCollisionReaction(missileIndex, collisionReaction, attachLocalFrame, isAttachedFrameLocal, attackerAgent.Index, attachedAgent != null ? attachedAgent.Index : -1, attachedToShield, attachedBoneIndex, attachedMissionObject != null ? attachedMissionObject.Id : MissionObjectId.Invalid, bounceBackVelocity, bounceBackAngularVelocity, missionObjectId.Id));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//      }
//      else if (GameNetwork.IsClientOrReplay && flag)
//        this.RemoveMissileAsClient(missileIndex);
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnMissileCollisionReaction(collisionReaction, attackerAgent, attachedAgent, attachedBoneIndex);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, true)]
//    internal void MissileCalculatePassbySoundParametersCallbackMT(
//      int missileIndex,
//      ref SoundEventParameter soundEventParameter)
//    {
//      this._missilesDictionary[missileIndex].CalculatePassbySoundParametersMT(ref soundEventParameter);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void ChargeDamageCallback(
//      ref AttackCollisionData collisionData,
//      Blow blow,
//      Agent attacker,
//      Agent victim)
//    {
//      if (victim.CurrentMortalityState == Agent.MortalityState.Invulnerable || attacker.RiderAgent != null && !attacker.IsEnemyOf(victim) && !this.IsFriendlyFireAllowedForChargeDamage())
//        return;
//      WeaponComponentData shieldOnBack;
//      CombatLogData combatLog;
//      this.GetAttackCollisionResults(attacker, victim, WeakGameEntity.Invalid, 1f, in MissionWeapon.Invalid, false, false, false, ref collisionData, out shieldOnBack, out combatLog);
//      if (collisionData.CollidedWithShieldOnBack && shieldOnBack != null && victim != null && victim.IsMainAgent)
//        InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back").ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
//      if ((double) collisionData.InflictedDamage <= 0.0)
//        return;
//      blow.BaseMagnitude = collisionData.BaseMagnitude;
//      blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
//      blow.InflictedDamage = collisionData.InflictedDamage;
//      blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
//      blow.AbsorbedByArmor = (float) collisionData.AbsorbedByArmor;
//      blow.DamageCalculated = true;
//      if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentKnockedBackByBlow(attacker, victim, in collisionData, (WeaponComponentData) null, in blow))
//        blow.BlowFlag |= BlowFlags.KnockBack;
//      else
//        blow.BlowFlag &= ~BlowFlags.KnockBack;
//      if (MissionGameModels.Current.AgentApplyDamageModel.DecideAgentKnockedDownByBlow(attacker, victim, in collisionData, (WeaponComponentData) null, in blow))
//        blow.BlowFlag |= BlowFlags.KnockDown;
//      this.RegisterBlow(attacker, victim, WeakGameEntity.Invalid, blow, ref collisionData, new MissionWeapon(), ref combatLog);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void FallDamageCallback(
//      ref AttackCollisionData collisionData,
//      Blow b,
//      Agent attacker,
//      Agent victim)
//    {
//      if (victim.CurrentMortalityState == Agent.MortalityState.Invulnerable)
//        return;
//      CombatLogData combatLog;
//      this.GetAttackCollisionResults(attacker, victim, WeakGameEntity.Invalid, 1f, in MissionWeapon.Invalid, false, false, false, ref collisionData, out WeaponComponentData _, out combatLog);
//      b.BaseMagnitude = collisionData.BaseMagnitude;
//      b.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
//      b.InflictedDamage = collisionData.InflictedDamage;
//      b.SelfInflictedDamage = collisionData.SelfInflictedDamage;
//      b.AbsorbedByArmor = (float) collisionData.AbsorbedByArmor;
//      b.DamageCalculated = true;
//      if (b.InflictedDamage <= 0)
//        return;
//      Agent riderAgent = victim.RiderAgent;
//      this.RegisterBlow(attacker, victim, WeakGameEntity.Invalid, b, ref collisionData, new MissionWeapon(), ref combatLog);
//      if (riderAgent == null)
//        return;
//      this.FallDamageCallback(ref collisionData, b, riderAgent, riderAgent);
//    }

//    public void KillAgentsOnEntity(GameEntity entity, Agent destroyerAgent, bool burnAgents)
//    {
//      if (entity == (GameEntity) null)
//        return;
//      int ownerId;
//      sbyte AttackBoneIndex;
//      if (destroyerAgent != null)
//      {
//        ownerId = destroyerAgent.Index;
//        AttackBoneIndex = destroyerAgent.Monster.MainHandItemBoneIndex;
//      }
//      else
//      {
//        ownerId = -1;
//        AttackBoneIndex = (sbyte) -1;
//      }
//      Vec3 bbmin;
//      Vec3 bbmax;
//      entity.GetPhysicsMinMax(true, out bbmin, out bbmax, false);
//      Vec2 vec2 = (bbmax.AsVec2 + bbmin.AsVec2) * 0.5f;
//      float searchRadius = (bbmax.AsVec2 - bbmin.AsVec2).Length * 0.5f;
//      Blow blow = new Blow(ownerId);
//      blow.DamageCalculated = true;
//      blow.BaseMagnitude = 2000f;
//      blow.InflictedDamage = 2000;
//      blow.Direction = new Vec3(z: -1f);
//      blow.DamageType = DamageTypes.Blunt;
//      blow.BoneIndex = (sbyte) 0;
//      blow.WeaponRecord.FillAsMeleeBlow((ItemObject) null, (WeaponComponentData) null, -1, (sbyte) 0);
//      if (burnAgents)
//      {
//        blow.WeaponRecord.WeaponFlags |= WeaponFlags.AffectsArea | WeaponFlags.Burning;
//        blow.WeaponRecord.CurrentPosition = blow.GlobalPosition;
//        blow.WeaponRecord.StartingPosition = blow.GlobalPosition;
//      }
//      Vec2 asVec2 = entity.GetGlobalFrame().TransformToParent(vec2.ToVec3()).AsVec2;
//      List<Agent> agentList = new List<Agent>();
//      AgentProximityMap.ProximityMapSearchStruct searchStruct = AgentProximityMap.BeginSearch(this, asVec2, searchRadius);
//      while (searchStruct.LastFoundAgent != null)
//      {
//        Agent lastFoundAgent = searchStruct.LastFoundAgent;
//        WeakGameEntity weakGameEntity = lastFoundAgent.GetSteppedEntity();
//        while (weakGameEntity.IsValid && !(weakGameEntity == entity))
//          weakGameEntity = weakGameEntity.Parent;
//        if (weakGameEntity.IsValid)
//          agentList.Add(lastFoundAgent);
//        AgentProximityMap.FindNext(this, ref searchStruct);
//      }
//      foreach (Agent agent in agentList)
//      {
//        blow.GlobalPosition = agent.Position;
//        AttackCollisionData collisionData = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Abdomen, AttackBoneIndex, Agent.UsageDirection.AttackLeft, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, Vec3.Up, blow.Direction, blow.GlobalPosition, Vec3.Zero, Vec3.Zero, agent.Velocity, Vec3.Up);
//        agent.RegisterBlow(blow, in collisionData);
//      }
//    }

//    public void KillAgentCheat(Agent agent)
//    {
//      if (GameNetwork.IsClientOrReplay)
//        return;
//      Agent agent1 = this.MainAgent ?? agent;
//      Blow blow = new Blow(agent1.Index);
//      blow.DamageType = DamageTypes.Blunt;
//      blow.BoneIndex = agent.Monster.HeadLookDirectionBoneIndex;
//      blow.GlobalPosition = agent.Position;
//      blow.GlobalPosition.z += agent.GetEyeGlobalHeight();
//      blow.BaseMagnitude = 2000f;
//      blow.WeaponRecord.FillAsMeleeBlow((ItemObject) null, (WeaponComponentData) null, -1, (sbyte) -1);
//      blow.InflictedDamage = 2000;
//      blow.SwingDirection = agent.LookDirection;
//      if (this.InputManager.IsGameKeyDown(2))
//      {
//        blow.SwingDirection = agent.Frame.rotation.TransformToParent(new Vec3(-1f));
//        double num = (double) blow.SwingDirection.Normalize();
//      }
//      else if (this.InputManager.IsGameKeyDown(3))
//      {
//        blow.SwingDirection = agent.Frame.rotation.TransformToParent(new Vec3(1f));
//        double num = (double) blow.SwingDirection.Normalize();
//      }
//      else if (this.InputManager.IsGameKeyDown(1))
//      {
//        blow.SwingDirection = agent.Frame.rotation.TransformToParent(new Vec3(y: -1f));
//        double num = (double) blow.SwingDirection.Normalize();
//      }
//      else if (this.InputManager.IsGameKeyDown(0))
//      {
//        blow.SwingDirection = agent.Frame.rotation.TransformToParent(new Vec3(y: 1f));
//        double num = (double) blow.SwingDirection.Normalize();
//      }
//      blow.Direction = blow.SwingDirection;
//      blow.DamageCalculated = true;
//      sbyte handItemBoneIndex = agent1.Monster.MainHandItemBoneIndex;
//      AttackCollisionData collisionData = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Head, handItemBoneIndex, Agent.UsageDirection.AttackLeft, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, Vec3.Up, blow.Direction, blow.GlobalPosition, Vec3.Zero, Vec3.Zero, agent.Velocity, Vec3.Up);
//      agent.RegisterBlow(blow, in collisionData);
//    }

//    public bool KillCheats(bool killAll, bool killEnemy, bool killHorse, bool killYourself)
//    {
//      bool flag1 = false;
//      if (!GameNetwork.IsClientOrReplay)
//      {
//        if (killYourself)
//        {
//          if (this.MainAgent != null)
//          {
//            if (killHorse)
//            {
//              if (this.MainAgent.MountAgent != null)
//              {
//                this.KillAgentCheat(this.MainAgent.MountAgent);
//                flag1 = true;
//              }
//            }
//            else
//            {
//              this.KillAgentCheat(this.MainAgent);
//              flag1 = true;
//            }
//          }
//        }
//        else
//        {
//          bool flag2 = false;
//          for (int index = this.Agents.Count - 1; index >= 0 && !flag2; --index)
//          {
//            Agent agent = this.Agents[index];
//            if (agent != this.MainAgent && agent.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.CanAttack) && this.PlayerTeam != null)
//            {
//              if (killEnemy)
//              {
//                if (agent.Team != null && agent.Team.IsValid && this.PlayerTeam.IsEnemyOf(agent.Team))
//                {
//                  if (killHorse && agent.HasMount)
//                  {
//                    if (agent.MountAgent != null)
//                    {
//                      this.KillAgentCheat(agent.MountAgent);
//                      if (!killAll)
//                        flag2 = true;
//                      flag1 = true;
//                    }
//                  }
//                  else
//                  {
//                    this.KillAgentCheat(agent);
//                    if (!killAll)
//                      flag2 = true;
//                    flag1 = true;
//                  }
//                }
//              }
//              else if (agent.Team != null && agent.Team.IsValid && this.PlayerTeam.IsFriendOf(agent.Team))
//              {
//                if (killHorse)
//                {
//                  if (agent.MountAgent != null)
//                  {
//                    this.KillAgentCheat(agent.MountAgent);
//                    if (!killAll)
//                      flag2 = true;
//                    flag1 = true;
//                  }
//                }
//                else
//                {
//                  this.KillAgentCheat(agent);
//                  if (!killAll)
//                    flag2 = true;
//                  flag1 = true;
//                }
//              }
//            }
//          }
//        }
//      }
//      return flag1;
//    }

//    private bool CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(Agent attacker, Agent victim)
//    {
//      if (victim == null || attacker == null)
//        return false;
//      int num1 = !GameNetwork.IsSessionActive || this.ForceNoFriendlyFire || MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue() <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue() <= 0 || this.Mode == MissionMode.Duel ? 1 : (attacker.Controller == AgentControllerType.AI ? 1 : 0);
//      bool flag = attacker.IsFriendOf(victim);
//      int num2 = flag ? 1 : 0;
//      if ((num1 & num2) != 0)
//        return true;
//      return victim.IsHuman && !flag && !attacker.IsEnemyOf(victim);
//    }

//    private bool IsFriendlyFireAllowedForChargeDamage()
//    {
//      if (!GameNetwork.IsServer)
//        return false;
//      if (!this._doesMissionAllowChargeDamageOnFriendly.HasValue || !this._doesMissionAllowChargeDamageOnFriendly.HasValue)
//        this._doesMissionAllowChargeDamageOnFriendly = new bool?(this.GetMissionBehavior<MissionMultiplayerGameModeBase>().IsGameModeAllowChargeDamageOnFriendly);
//      return this._doesMissionAllowChargeDamageOnFriendly.Value && (MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue() > 0 || MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue() > 0);
//    }

//    public bool CanTakeControlOfAgent(Agent agentToTakeControlOf)
//    {
//      return this._canPlayerTakeControlOfAnotherAgentWhenDead && this.MainAgent == null && agentToTakeControlOf != null && agentToTakeControlOf.IsHuman && agentToTakeControlOf.IsActive() && agentToTakeControlOf.Team != null && agentToTakeControlOf.Team == this.PlayerTeam && !agentToTakeControlOf.IsUsingGameObject && !agentToTakeControlOf.Character.IsHero && (double) agentToTakeControlOf.Health / (double) agentToTakeControlOf.HealthLimit >= 0.25;
//    }

//    public void SetPlayerCanTakeControlOfAnotherAgentWhenDead()
//    {
//      this._canPlayerTakeControlOfAnotherAgentWhenDead = true;
//    }

//    public void TakeControlOfAgent(Agent agentToTakeControlOf)
//    {
//      if (this.IsFastForward)
//        this.IsFastForward = false;
//      agentToTakeControlOf.Controller = AgentControllerType.Player;
//    }

//    public float GetDamageMultiplierOfCombatDifficulty(Agent victimAgent, Agent attackerAgent = null)
//    {
//      return MissionGameModels.Current.MissionDifficultyModel != null ? MissionGameModels.Current.MissionDifficultyModel.GetDamageMultiplierOfCombatDifficulty(victimAgent, attackerAgent) : 1f;
//    }

//    public float GetShootDifficulty(Agent affectedAgent, Agent affectorAgent, bool isHeadShot)
//    {
//      Vec2 vec2 = affectedAgent.MovementVelocity - affectorAgent.MovementVelocity;
//      Vec3 va = new Vec3(vec2.x, vec2.y);
//      Vec3 vb = affectedAgent.Position - affectorAgent.Position;
//      float num1 = vb.Normalize();
//      float num2 = va.Normalize();
//      float length = Vec3.CrossProduct(va, vb).Length;
//      float shootDifficulty = MBMath.ClampFloat((float) (0.30000001192092896 * ((4.0 + (double) num1) / 4.0) * ((4.0 + (double) length * (double) num2) / 4.0)), 1f, 12f);
//      if (isHeadShot)
//        shootDifficulty *= 1.2f;
//      return shootDifficulty;
//    }

//    private MatrixFrame CalculateAttachedLocalFrame(
//      in MatrixFrame attachedGlobalFrame,
//      AttackCollisionData collisionData,
//      WeaponComponentData missileWeapon,
//      Agent affectedAgent,
//      WeakGameEntity hitEntity,
//      Vec3 missileMovementVelocity,
//      Vec3 missileRotationSpeed,
//      MatrixFrame shieldGlobalFrame,
//      bool shouldMissilePenetrate,
//      out bool isAttachedFrameLocal)
//    {
//      isAttachedFrameLocal = false;
//      MatrixFrame attachedLocalFrame = attachedGlobalFrame;
//      bool isNonZero = missileWeapon.RotationSpeed.IsNonZero;
//      bool flag = affectedAgent != null && !collisionData.AttackBlockedWithShield && missileWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.AmmoSticksWhenShot);
//      float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(flag ? (isNonZero ? ManagedParametersEnum.RotatingProjectileMinPenetration : ManagedParametersEnum.ProjectileMinPenetration) : ManagedParametersEnum.ObjectMinPenetration);
//      float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(flag ? (isNonZero ? ManagedParametersEnum.RotatingProjectileMaxPenetration : ManagedParametersEnum.ProjectileMaxPenetration) : ManagedParametersEnum.ObjectMaxPenetration);
//      Vec3 vec3_1 = missileMovementVelocity;
//      float num1 = vec3_1.Normalize();
//      float num2 = MBMath.ClampFloat(flag ? (float) collisionData.InflictedDamage / affectedAgent.HealthLimit : num1 / ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ProjectileMaxPenetrationSpeed), 0.0f, 1f);
//      if (shouldMissilePenetrate)
//      {
//        float num3 = managedParameter1 + (managedParameter2 - managedParameter1) * num2;
//        attachedLocalFrame.origin += vec3_1 * num3;
//      }
//      MatrixFrame matrixFrame1;
//      if (missileRotationSpeed.IsNonZero)
//      {
//        float managedParameter3 = ManagedParameters.Instance.GetManagedParameter(flag ? ManagedParametersEnum.AgentProjectileNormalWeight : ManagedParametersEnum.ProjectileNormalWeight);
//        matrixFrame1 = missileWeapon.GetMissileStartingFrame();
//        Vec3 parent = matrixFrame1.TransformToParent(in missileRotationSpeed);
//        Vec3 vec3_2 = -collisionData.CollisionGlobalNormal;
//        float num4 = parent.x * parent.x;
//        float num5 = parent.y * parent.y;
//        float num6 = parent.z * parent.z;
//        int i = (double) num4 <= (double) num5 || (double) num4 <= (double) num6 ? ((double) num5 > (double) num6 ? 1 : 2) : 0;
//        vec3_2 -= vec3_2.ProjectOnUnitVector(attachedLocalFrame.rotation[i]);
//        Vec3 v = Vec3.CrossProduct(vec3_1, vec3_2.NormalizedCopy());
//        float num7 = v.Normalize();
//        attachedLocalFrame.rotation.RotateAboutAnArbitraryVector(in v, MathF.Asin(MathF.Clamp(num7, 0.0f, 1f)) * managedParameter3);
//      }
//      if (!collisionData.AttackBlockedWithShield && affectedAgent != null)
//      {
//        float num8 = Vec3.DotProduct(collisionData.CollisionGlobalNormal, vec3_1) + 1f;
//        if ((double) num8 > 0.5)
//          attachedLocalFrame.origin -= num8 * 0.1f * collisionData.CollisionGlobalNormal;
//      }
//      ref MatrixFrame local1 = ref attachedLocalFrame;
//      matrixFrame1 = missileWeapon.GetMissileStartingFrame();
//      ref MatrixFrame local2 = ref matrixFrame1;
//      MatrixFrame matrixFrame2 = missileWeapon.StickingFrame;
//      ref MatrixFrame local3 = ref matrixFrame2;
//      matrixFrame1 = local2.TransformToParent(in local3);
//      ref MatrixFrame local4 = ref matrixFrame1;
//      attachedLocalFrame = local1.TransformToParent(in local4);
//      ref MatrixFrame local5 = ref attachedLocalFrame;
//      matrixFrame2 = missileWeapon.GetMissileStartingFrame();
//      ref MatrixFrame local6 = ref matrixFrame2;
//      attachedLocalFrame = local5.TransformToParent(in local6);
//      if (collisionData.AttackBlockedWithShield)
//      {
//        attachedLocalFrame = shieldGlobalFrame.TransformToLocal(in attachedLocalFrame);
//        isAttachedFrameLocal = true;
//      }
//      else if (affectedAgent != null)
//      {
//        if (flag)
//        {
//          MBAgentVisuals agentVisuals = affectedAgent.AgentVisuals;
//          matrixFrame2 = agentVisuals.GetGlobalFrame();
//          ref MatrixFrame local7 = ref matrixFrame2;
//          matrixFrame1 = agentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex(collisionData.CollisionBoneIndex);
//          ref MatrixFrame local8 = ref matrixFrame1;
//          MatrixFrame matrixFrame3 = local7.TransformToParent(in local8);
//          matrixFrame3 = matrixFrame3.GetUnitRotFrame(affectedAgent.AgentScale);
//          attachedLocalFrame = matrixFrame3.TransformToLocalNonOrthogonal(in attachedLocalFrame);
//          isAttachedFrameLocal = true;
//        }
//      }
//      else if (hitEntity.IsValid)
//      {
//        if (collisionData.CollisionBoneIndex >= (sbyte) 0)
//        {
//          attachedLocalFrame = hitEntity.Skeleton.GetBoneEntitialFrameWithIndex(collisionData.CollisionBoneIndex).TransformToLocalNonOrthogonal(in attachedLocalFrame);
//          isAttachedFrameLocal = true;
//        }
//        else
//        {
//          matrixFrame1 = hitEntity.GetGlobalFrame();
//          attachedLocalFrame = matrixFrame1.TransformToLocalNonOrthogonal(in attachedLocalFrame);
//          isAttachedFrameLocal = true;
//        }
//      }
//      else
//        attachedLocalFrame.origin.z = Math.Max(attachedLocalFrame.origin.z, -100f);
//      return attachedLocalFrame;
//    }

//    [UsedImplicitly]
//    [MBCallback(null, true)]
//    internal void GetDefendCollisionResults(
//      Agent attackerAgent,
//      Agent defenderAgent,
//      CombatCollisionResult collisionResult,
//      int attackerWeaponSlotIndex,
//      bool isAlternativeAttack,
//      StrikeType strikeType,
//      Agent.UsageDirection attackDirection,
//      float collisionDistanceOnWeapon,
//      float attackProgress,
//      bool attackIsParried,
//      bool isPassiveUsageHit,
//      bool isHeavyAttack,
//      ref float defenderStunPeriod,
//      ref float attackerStunPeriod,
//      ref bool crushedThrough)
//    {
//      bool chamber = false;
//      MissionCombatMechanicsHelper.GetDefendCollisionResults(attackerAgent, defenderAgent, collisionResult, attackerWeaponSlotIndex, isAlternativeAttack, strikeType, attackDirection, collisionDistanceOnWeapon, attackProgress, attackIsParried, isPassiveUsageHit, isHeavyAttack, ref defenderStunPeriod, ref attackerStunPeriod, ref crushedThrough, ref chamber);
//      if (!(crushedThrough | chamber) || !attackerAgent.CanLogCombatFor && !defenderAgent.CanLogCombatFor)
//        return;
//      CombatLogData combatLog = new CombatLogData(false, attackerAgent.IsHuman, attackerAgent.IsMine, attackerAgent.RiderAgent != null, attackerAgent.RiderAgent != null && attackerAgent.RiderAgent.IsMine, attackerAgent.IsMount, defenderAgent.IsHuman, defenderAgent.IsMine, (double) defenderAgent.Health <= 0.0, defenderAgent.HasMount, defenderAgent.RiderAgent != null && defenderAgent.RiderAgent.IsMine, defenderAgent.IsMount, (MissionObject) null, defenderAgent.RiderAgent == attackerAgent, crushedThrough, chamber, 0.0f);
//      this.AddCombatLogSafe(attackerAgent, defenderAgent, combatLog);
//    }

//    private CombatLogData GetAttackCollisionResults(
//      Agent attackerAgent,
//      Agent victimAgent,
//      WeakGameEntity hitObject,
//      float momentumRemaining,
//      in MissionWeapon attackerWeapon,
//      bool crushedThrough,
//      bool cancelDamage,
//      bool crushedThroughWithoutAgentCollision,
//      ref AttackCollisionData attackCollisionData,
//      out WeaponComponentData shieldOnBack,
//      out CombatLogData combatLog)
//    {
//      AttackInformation attackInformation = new AttackInformation(attackerAgent, victimAgent, hitObject, in attackCollisionData, in attackerWeapon);
//      shieldOnBack = attackInformation.ShieldOnBack;
//      MissionCombatMechanicsHelper.GetAttackCollisionResults(in attackInformation, crushedThrough, momentumRemaining, cancelDamage, ref attackCollisionData, out combatLog, out int _);
//      float inflictedDamage = (float) attackCollisionData.InflictedDamage;
//      if ((double) inflictedDamage > 0.0)
//      {
//        float damage = MissionGameModels.Current.AgentApplyDamageModel.CalculateDamage(in attackInformation, in attackCollisionData, inflictedDamage);
//        combatLog.ModifiedDamage = MathF.Round(damage - inflictedDamage);
//        attackCollisionData.InflictedDamage = MathF.Round(damage);
//      }
//      else
//      {
//        combatLog.ModifiedDamage = 0;
//        attackCollisionData.InflictedDamage = 0;
//      }
//      combatLog.ReflectedDamage = 0;
//      if (!attackCollisionData.IsFallDamage && attackInformation.IsFriendlyFire)
//      {
//        if (!attackInformation.IsAttackerAIControlled && GameNetwork.IsSessionActive)
//        {
//          int num1 = attackCollisionData.IsMissile ? MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.GetIntValue() : MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue();
//          attackCollisionData.SelfInflictedDamage = MathF.Round((float) attackCollisionData.InflictedDamage * ((float) num1 * 0.01f));
//          attackCollisionData.SelfInflictedDamage = MBMath.ClampInt(attackCollisionData.SelfInflictedDamage, 0, 2000);
//          int num2 = attackCollisionData.IsMissile ? MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.GetIntValue() : MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue();
//          attackCollisionData.InflictedDamage = MathF.Round((float) attackCollisionData.InflictedDamage * ((float) num2 * 0.01f));
//          attackCollisionData.InflictedDamage = MBMath.ClampInt(attackCollisionData.InflictedDamage, 0, 2000);
//          combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
//        }
//        combatLog.IsFriendlyFire = true;
//        combatLog.ReflectedDamage = attackCollisionData.SelfInflictedDamage;
//      }
//      if (attackCollisionData.AttackBlockedWithShield && attackCollisionData.InflictedDamage > 0 && (int) attackInformation.VictimShield.HitPoints - attackCollisionData.InflictedDamage <= 0)
//        attackCollisionData.IsShieldBroken = true;
//      if (!crushedThroughWithoutAgentCollision)
//        combatLog.BodyPartHit = attackCollisionData.VictimHitBodyPart;
//      return combatLog;
//    }

//    private void PrintAttackCollisionResults(
//      Agent attackerAgent,
//      Agent victimAgent,
//      MissionObject missionObjectHit,
//      ref AttackCollisionData attackCollisionData,
//      ref CombatLogData combatLog)
//    {
//      if ((!attackCollisionData.IsColliderAgent ? 0 : (!attackCollisionData.AttackBlockedWithShield ? 1 : 0)) == 0 || attackerAgent == null || !attackerAgent.CanLogCombatFor && !victimAgent.CanLogCombatFor || victimAgent.State != AgentState.Active)
//        return;
//      combatLog.MissionObjectHit = missionObjectHit;
//      this.AddCombatLogSafe(attackerAgent, victimAgent, combatLog);
//    }

//    public void AddCombatLogSafe(Agent attackerAgent, Agent victimAgent, CombatLogData combatLog)
//    {
//      MissionObject missionObjectHit = combatLog.MissionObjectHit;
//      combatLog.SetVictimAgent(victimAgent);
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        CombatLogNetworkMessage message = new CombatLogNetworkMessage(attackerAgent.Index, victimAgent != null ? victimAgent.Index : -1, missionObjectHit != null ? missionObjectHit.Id : MissionObjectId.Invalid, combatLog);
//        NetworkCommunicator communicator1 = (attackerAgent == null ? (Agent) null : (attackerAgent.IsHuman ? attackerAgent : attackerAgent.RiderAgent))?.MissionPeer?.Peer.Communicator as NetworkCommunicator;
//        NetworkCommunicator communicator2 = (victimAgent == null ? (Agent) null : (victimAgent.IsHuman ? victimAgent : victimAgent.RiderAgent))?.MissionPeer?.Peer.Communicator as NetworkCommunicator;
//        if (communicator1 != null && !communicator1.IsServerPeer)
//        {
//          GameNetwork.BeginModuleEventAsServer(communicator1);
//          GameNetwork.WriteMessage((GameNetworkMessage) message);
//          GameNetwork.EndModuleEventAsServer();
//        }
//        if (communicator2 != null && !communicator2.IsServerPeer && communicator2 != communicator1)
//        {
//          GameNetwork.BeginModuleEventAsServer(communicator2);
//          GameNetwork.WriteMessage((GameNetworkMessage) message);
//          GameNetwork.EndModuleEventAsServer();
//        }
//      }
//      this._combatLogsCreated.Enqueue(combatLog);
//    }

//    public MissionObject CreateMissionObjectFromPrefab(
//      string prefab,
//      MatrixFrame frame,
//      Action<GameEntity> actionAppliedBeforeScriptInitialization)
//    {
//      if (GameNetwork.IsClientOrReplay)
//        return (MissionObject) null;
//      GameEntity gameEntity = GameEntity.Instantiate(this.Scene, prefab, frame, false);
//      actionAppliedBeforeScriptInitialization(gameEntity);
//      gameEntity.CallScriptCallbacks(true);
//      MissionObject firstScriptOfType1 = gameEntity.GetFirstScriptOfType<MissionObject>();
//      List<MissionObjectId> childObjectIds = new List<MissionObjectId>();
//      foreach (GameEntity child in gameEntity.GetChildren())
//      {
//        MissionObject firstScriptOfType2;
//        if ((firstScriptOfType2 = child.GetFirstScriptOfType<MissionObject>()) != null)
//          childObjectIds.Add(firstScriptOfType2.Id);
//      }
//      if (GameNetwork.IsServerOrRecorder)
//      {
//        GameNetwork.BeginBroadcastModuleEvent();
//        GameNetwork.WriteMessage((GameNetworkMessage) new CreateMissionObject(firstScriptOfType1.Id, prefab, frame, childObjectIds));
//        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
//        this.AddDynamicallySpawnedMissionObjectInfo(new Mission.DynamicallyCreatedEntity(prefab, firstScriptOfType1.Id, frame, ref childObjectIds));
//      }
//      return firstScriptOfType1;
//    }

//    public int GetNearbyAllyAgentsCount(Vec2 center, float radius, Team team)
//    {
//      return this.GetNearbyAgentsCountAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Friend);
//    }

//    public MBList<Agent> GetNearbyAllyAgents(
//      Vec2 center,
//      float radius,
//      Team team,
//      MBList<Agent> agents)
//    {
//      agents.Clear();
//      this.GetNearbyAgentsAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Friend, agents);
//      return agents;
//    }

//    public MBList<Agent> GetNearbyEnemyAgents(
//      Vec2 center,
//      float radius,
//      Team team,
//      MBList<Agent> agents)
//    {
//      agents.Clear();
//      this.GetNearbyAgentsAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Enemy, agents);
//      return agents;
//    }

//    public MBList<Agent> GetNearbyAgents(Vec2 center, float radius, MBList<Agent> agents)
//    {
//      agents.Clear();
//      this.GetNearbyAgentsAux(center, radius, MBTeam.InvalidTeam, Mission.GetNearbyAgentsAuxType.All, agents);
//      return agents;
//    }

//    public bool IsFormationUnitPositionAvailableMT(
//      ref WorldPosition formationPosition,
//      ref WorldPosition unitPosition,
//      ref WorldPosition nearestAvailableUnitPosition,
//      float manhattanDistance,
//      Team team)
//    {
//      return formationPosition.IsValid && !(formationPosition.GetNavMesh() == UIntPtr.Zero) && unitPosition.IsValid && !(unitPosition.GetNavMesh() == UIntPtr.Zero) && (this.IsFormationUnitPositionAvailable_AdditionalCondition == null || this.IsFormationUnitPositionAvailable_AdditionalCondition(unitPosition, team)) && (this.Mode != MissionMode.Deployment || !this.DeploymentPlan.HasDeploymentBoundaries(team) || this.DeploymentPlan.IsPositionInsideDeploymentBoundaries(team, unitPosition.AsVec2)) && this.IsFormationUnitPositionAvailableAuxMT(ref formationPosition, ref unitPosition, ref nearestAvailableUnitPosition, manhattanDistance);
//    }

//    public bool IsOrderPositionAvailable(in WorldPosition orderPosition, Team team)
//    {
//      return orderPosition.IsValid && !(orderPosition.GetNavMesh() == UIntPtr.Zero) && (this.IsFormationUnitPositionAvailable_AdditionalCondition == null || this.IsFormationUnitPositionAvailable_AdditionalCondition(orderPosition, team)) && this.IsPositionInsideBoundaries(orderPosition.AsVec2);
//    }

//    public bool IsFormationUnitPositionAvailable(ref WorldPosition unitPosition, Team team)
//    {
//      WorldPosition formationPosition = unitPosition;
//      float manhattanDistance = 1f;
//      WorldPosition invalid = WorldPosition.Invalid;
//      return this.IsFormationUnitPositionAvailableMT(ref formationPosition, ref unitPosition, ref invalid, manhattanDistance, team);
//    }

//    public bool HasSceneMapPatch() => this.InitializerRecord.SceneHasMapPatch;

//    public bool GetPatchSceneEncounterPosition(out Vec3 position)
//    {
//      if (this.InitializerRecord.SceneHasMapPatch)
//      {
//        Vec2 patchCoordinates = this.InitializerRecord.PatchCoordinates;
//        float northRotation = this.Scene.GetNorthRotation();
//        Vec2 boxMinimum;
//        Vec2 boxMaximum;
//        this.Boundaries.GetOrientedBoundariesBox(out boxMinimum, out boxMaximum, northRotation);
//        Vec2 side = Vec2.Side;
//        side.RotateCCW(northRotation);
//        Vec2 vec2_1 = side.LeftVec();
//        Vec2 vec2_2 = boxMaximum - boxMinimum;
//        Vec2 position1 = boxMinimum.x * side + boxMinimum.y * vec2_1 + vec2_2.x * patchCoordinates.x * side + vec2_2.y * patchCoordinates.y * vec2_1;
//        position = position1.ToVec3(this.Scene.GetTerrainHeight(position1));
//        return true;
//      }
//      position = Vec3.Invalid;
//      return false;
//    }

//    public bool GetPatchSceneEncounterDirection(out Vec2 direction)
//    {
//      if (this.InitializerRecord.SceneHasMapPatch)
//      {
//        float northRotation = this.Scene.GetNorthRotation();
//        direction = this.InitializerRecord.PatchEncounterDir;
//        direction.RotateCCW(northRotation);
//        return true;
//      }
//      direction = Vec2.Invalid;
//      return false;
//    }

//    private void TickDebugAgents()
//    {
//    }

//    public void AddTimerToDynamicEntity(GameEntity gameEntity, float timeToKill = 10f)
//    {
//      this._dynamicEntities.Add(new Mission.DynamicEntityInfo()
//      {
//        Entity = gameEntity,
//        TimerToDisable = new TaleWorlds.Core.Timer(this.CurrentTime, timeToKill)
//      });
//    }

//    public void AddListener(IMissionListener listener) => this._listeners.Add(listener);

//    public void RemoveListener(IMissionListener listener) => this._listeners.Remove(listener);

//    public void OnAgentFleeing(Agent agent)
//    {
//      for (int index = this.MissionBehaviors.Count - 1; index >= 0; --index)
//        this.MissionBehaviors[index].OnAgentFleeing(agent);
//      agent.OnFleeing();
//    }

//    public void OnAgentPanicked(Agent agent)
//    {
//      for (int index = this.MissionBehaviors.Count - 1; index >= 0; --index)
//        this.MissionBehaviors[index].OnAgentPanicked(agent);
//    }

//    public void OnTeamDeployed(Team team)
//    {
//      if (this.MissionBehaviors == null)
//        return;
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnTeamDeployed(team);
//    }

//    public void OnBattleSideDeployed(BattleSideEnum side)
//    {
//      if (this.MissionBehaviors == null)
//        return;
//      foreach (MissionBehavior missionBehavior in this.MissionBehaviors)
//        missionBehavior.OnBattleSideDeployed(side);
//    }

//    public void OnDeploymentFinished()
//    {
//      this.IsDeploymentFinished = true;
//      foreach (Team team in (List<Team>) this.Teams)
//      {
//        if (team.TeamAI != null)
//          team.TeamAI.OnDeploymentFinished();
//      }
//      for (int index = this.MissionBehaviors.Count - 1; index >= 0; --index)
//        this.MissionBehaviors[index].OnDeploymentFinished();
//    }

//    public void OnAfterDeploymentFinished()
//    {
//      for (int index = this.MissionBehaviors.Count - 1; index >= 0; --index)
//        this.MissionBehaviors[index].OnAfterDeploymentFinished();
//    }

//    public void OnFormationCaptainChanged(Formation formation)
//    {
//      Action<Formation> formationCaptainChanged = this.FormationCaptainChanged;
//      if (formationCaptainChanged == null)
//        return;
//      formationCaptainChanged(formation);
//    }

//    public void SetFastForwardingFromUI(bool fastForwarding) => this.IsFastForward = fastForwarding;

//    public bool CheckIfBattleInRetreat()
//    {
//      Func<bool> battleInRetreatEvent = this.IsBattleInRetreatEvent;
//      return battleInRetreatEvent != null && battleInRetreatEvent();
//    }

//    public void AddSpawnedItemEntityCreatedAtRuntime(SpawnedItemEntity spawnedItemEntity)
//    {
//      this._spawnedItemEntitiesCreatedAtRuntime.Add(spawnedItemEntity);
//    }

//    public void TriggerOnItemPickUpEvent(Agent agent, SpawnedItemEntity spawnedItemEntity)
//    {
//      Action<Agent, SpawnedItemEntity> onItemPickUp = this.OnItemPickUp;
//      if (onItemPickUp == null)
//        return;
//      onItemPickUp(agent, spawnedItemEntity);
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal static void DebugLogNativeMissionNetworkEvent(
//      int eventEnum,
//      string eventName,
//      int bitCount)
//    {
//      int eventType = eventEnum + CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo.GetMaximumValue() + 1;
//      DebugNetworkEventStatistics.StartEvent(eventName, eventType);
//      DebugNetworkEventStatistics.AddDataToStatistic(bitCount);
//      DebugNetworkEventStatistics.EndEvent();
//    }

//    [UsedImplicitly]
//    [MBCallback(null, false)]
//    internal void PauseMission() => this._missionState.Paused = true;

//    [CommandLineFunctionality.CommandLineArgumentFunction("kill_n_allies", "mission")]
//    public static string KillNAllies(List<string> strings)
//    {
//      if (GameNetwork.IsSessionActive)
//        return "Does not work on multiplayer.";
//      int result = 0;
//      if (strings.Count > 0 && !int.TryParse(strings[0], out result))
//        return "Please write the arguments in the correct format. Correct format is: 'mission.kill_n_allies [count]";
//      if (Mission.Current == null || result <= 0)
//        return "No active mission found or less than 1 agent to kill.";
//      foreach (Team team in (List<Team>) Mission.Current.Teams)
//      {
//        if (result > 0)
//        {
//          if (team.IsPlayerTeam)
//          {
//            foreach (Agent agent in team.ActiveAgents.ToList<Agent>())
//            {
//              if (agent.IsAIControlled)
//              {
//                Mission.Current.KillAgentCheat(agent);
//                if (--result <= 0)
//                  break;
//              }
//            }
//          }
//        }
//        else
//          break;
//      }
//      return "n allied agents killed.";
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("kill_all_allies", "mission")]
//    public static string KillAllAllies(List<string> strings)
//    {
//      if (GameNetwork.IsSessionActive)
//        return "Does not work on multiplayer.";
//      if (Mission.Current == null)
//        return "No active mission found";
//      foreach (Team team in (List<Team>) Mission.Current.Teams)
//      {
//        if (team.IsPlayerTeam)
//        {
//          foreach (Agent agent in team.ActiveAgents.ToList<Agent>())
//          {
//            if (agent.IsAIControlled)
//              Mission.Current.KillAgentCheat(agent);
//          }
//        }
//      }
//      return "Allied agents killed.";
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("toggleDisableDying", "mission")]
//    public static string ToggleDisableDying(List<string> strings)
//    {
//      if (GameNetwork.IsSessionActive)
//        return "Does not work on multiplayer.";
//      int result = 0;
//      if (strings.Count > 0 && !int.TryParse(strings[0], out result))
//        return "Please write the arguments in the correct format. Correct format is: 'toggleDisableDying [index]' or just 'toggleDisableDying' for making all agents invincible.";
//      if (Mission.Current == null)
//        return "No active mission found";
//      if (strings.Count == 0 || result == -1)
//      {
//        Mission.Current.DisableDying = !Mission.Current.DisableDying;
//        return Mission.Current.DisableDying ? "Dying disabled for all" : "Dying not disabled for all";
//      }
//      Agent agentWithIndex = Mission.Current.FindAgentWithIndex(result);
//      if (agentWithIndex == null)
//        return "Invalid agent index " + result.ToString();
//      agentWithIndex.ToggleInvulnerable();
//      return "Disable Dying for agent " + result.ToString() + ": " + (agentWithIndex.CurrentMortalityState == Agent.MortalityState.Invulnerable).ToString();
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("toggleDisableDyingTeam", "mission")]
//    public static string ToggleDisableDyingTeam(List<string> strings)
//    {
//      if (GameNetwork.IsSessionActive)
//        return "Does not work on multiplayer.";
//      int result = 0;
//      if (strings.Count > 0 && !int.TryParse(strings[0], out result))
//        return "Please write the arguments in the correct format. Correct format is: 'toggleDisableDyingTeam [team_no]' for making all active agents of a team invincible.";
//      int num = 0;
//      foreach (Agent allAgent in (List<Agent>) Mission.Current.AllAgents)
//      {
//        if (allAgent.Team != null && allAgent.Team.MBTeam.Index == result)
//        {
//          allAgent.ToggleInvulnerable();
//          ++num;
//        }
//      }
//      return "Toggled invulnerability for active agents of team " + result.ToString() + ", agent count: " + (object) num;
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("killAgent", "mission")]
//    public static string KillAgent(List<string> strings)
//    {
//      if (GameNetwork.IsSessionActive)
//        return "Does not work on multiplayer.";
//      if (Mission.Current == null)
//        return "Current mission does not exist.";
//      int result;
//      if (strings.Count == 0 || !int.TryParse(strings[0], out result))
//        return "Please write the arguments in the correct format. Correct format is: 'killAgent [index]'";
//      Agent agentWithIndex = Mission.Current.FindAgentWithIndex(result);
//      if (agentWithIndex == null)
//        return "Agent " + result.ToString() + " not found.";
//      if (agentWithIndex.State != AgentState.Active)
//        return "Agent " + result.ToString() + " already dead.";
//      Mission.Current.KillAgentCheat(agentWithIndex);
//      return "Agent " + result.ToString() + " died.";
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("set_battering_ram_speed", "mission")]
//    public static string IncreaseBatteringRamSpeeds(List<string> strings)
//    {
//      if (GameNetwork.IsSessionActive)
//        return "Does not work on multiplayer.";
//      float result;
//      if (strings.Count == 0 || !float.TryParse(strings[0], out result))
//        return "Please enter a speed value";
//      foreach (MissionObject activeMissionObject in (List<MissionObject>) Mission.Current.ActiveMissionObjects)
//      {
//        if (activeMissionObject.GameEntity.HasScriptOfType<BatteringRam>())
//        {
//          activeMissionObject.GameEntity.GetFirstScriptOfType<BatteringRam>().MovementComponent.MaxSpeed = result;
//          activeMissionObject.GameEntity.GetFirstScriptOfType<BatteringRam>().MovementComponent.MinSpeed = result;
//        }
//      }
//      return "Battering ram max speed increased.";
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("set_siege_tower_speed", "mission")]
//    public static string IncreaseSiegeTowerSpeed(List<string> strings)
//    {
//      if (GameNetwork.IsSessionActive)
//        return "Does not work on multiplayer.";
//      float result;
//      if (strings.Count == 0 || !float.TryParse(strings[0], out result))
//        return "Please enter a speed value";
//      foreach (MissionObject activeMissionObject in (List<MissionObject>) Mission.Current.ActiveMissionObjects)
//      {
//        if (activeMissionObject.GameEntity.HasScriptOfType<SiegeTower>())
//        {
//          activeMissionObject.GameEntity.GetFirstScriptOfType<SiegeTower>().MovementComponent.MaxSpeed = result;
//          activeMissionObject.GameEntity.GetFirstScriptOfType<SiegeTower>().MovementComponent.MinSpeed = result;
//        }
//      }
//      return "Siege tower max speed increased.";
//    }

//    [CommandLineFunctionality.CommandLineArgumentFunction("reload_managed_core_params", "game")]
//    public static string LoadParamsDebug(List<string> strings)
//    {
//      if (GameNetwork.IsSessionActive)
//        return "Does not work on multiplayer.";
//      ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Native", "managed_core_parameters"));
//      return "Managed core parameters reloaded.";
//    }

//    public class MBBoundaryCollection : 
//      IDictionary<string, ICollection<Vec2>>,
//      ICollection<KeyValuePair<string, ICollection<Vec2>>>,
//      IEnumerable<KeyValuePair<string, ICollection<Vec2>>>,
//      IEnumerable,
//      INotifyCollectionChanged
//    {
//      private readonly Mission _mission;

//      IEnumerator IEnumerable.GetEnumerator()
//      {
//        int count = this.Count;
//        for (int i = 0; i < count; ++i)
//        {
//          string boundaryName = MBAPI.IMBMission.GetBoundaryName(this._mission.Pointer, i);
//          List<Vec2> boundaryPoints = this.GetBoundaryPoints(boundaryName);
//          yield return (object) new KeyValuePair<string, ICollection<Vec2>>(boundaryName, (ICollection<Vec2>) boundaryPoints);
//        }
//      }

//      public IEnumerator<KeyValuePair<string, ICollection<Vec2>>> GetEnumerator()
//      {
//        int count = this.Count;
//        for (int i = 0; i < count; ++i)
//        {
//          string boundaryName = MBAPI.IMBMission.GetBoundaryName(this._mission.Pointer, i);
//          List<Vec2> boundaryPoints = this.GetBoundaryPoints(boundaryName);
//          yield return new KeyValuePair<string, ICollection<Vec2>>(boundaryName, (ICollection<Vec2>) boundaryPoints);
//        }
//      }

//      public int Count => MBAPI.IMBMission.GetBoundaryCount(this._mission.Pointer);

//      public float GetBoundaryRadius(string name)
//      {
//        return MBAPI.IMBMission.GetBoundaryRadius(this._mission.Pointer, name);
//      }

//      public bool IsReadOnly => false;

//      public void GetOrientedBoundariesBox(
//        out Vec2 boxMinimum,
//        out Vec2 boxMaximum,
//        float rotationInRadians = 0.0f)
//      {
//        Vec2 side = Vec2.Side;
//        side.RotateCCW(rotationInRadians);
//        Vec2 vb = side.LeftVec();
//        boxMinimum = new Vec2(float.MaxValue, float.MaxValue);
//        boxMaximum = new Vec2(float.MinValue, float.MinValue);
//        foreach (IEnumerable<Vec2> vec2s in (IEnumerable<ICollection<Vec2>>) this.Values)
//        {
//          foreach (Vec2 va in vec2s)
//          {
//            float num1 = Vec2.DotProduct(va, side);
//            float num2 = Vec2.DotProduct(va, vb);
//            boxMinimum.x = (double) num1 < (double) boxMinimum.x ? num1 : boxMinimum.x;
//            boxMinimum.y = (double) num2 < (double) boxMinimum.y ? num2 : boxMinimum.y;
//            boxMaximum.x = (double) num1 > (double) boxMaximum.x ? num1 : boxMaximum.x;
//            boxMaximum.y = (double) num2 > (double) boxMaximum.y ? num2 : boxMaximum.y;
//          }
//        }
//      }

//      internal MBBoundaryCollection(Mission mission) => this._mission = mission;

//      public void Add(KeyValuePair<string, ICollection<Vec2>> item)
//      {
//        this.Add(item.Key, item.Value);
//      }

//      public void Clear()
//      {
//        foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
//          this.Remove(keyValuePair.Key);
//      }

//      public bool Contains(KeyValuePair<string, ICollection<Vec2>> item)
//      {
//        return this.ContainsKey(item.Key);
//      }

//      public void CopyTo(KeyValuePair<string, ICollection<Vec2>>[] array, int arrayIndex)
//      {
//        if (array == null)
//          throw new ArgumentNullException(nameof (array));
//        if (arrayIndex < 0)
//          throw new ArgumentOutOfRangeException(nameof (arrayIndex));
//        foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
//        {
//          array[arrayIndex] = keyValuePair;
//          ++arrayIndex;
//          if (arrayIndex >= array.Length)
//            throw new ArgumentException("Not enough size in array.");
//        }
//      }

//      public bool Remove(KeyValuePair<string, ICollection<Vec2>> item) => this.Remove(item.Key);

//      public ICollection<string> Keys
//      {
//        get
//        {
//          List<string> keys = new List<string>();
//          foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
//            keys.Add(keyValuePair.Key);
//          return (ICollection<string>) keys;
//        }
//      }

//      public ICollection<ICollection<Vec2>> Values
//      {
//        get
//        {
//          List<ICollection<Vec2>> values = new List<ICollection<Vec2>>();
//          foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
//            values.Add(keyValuePair.Value);
//          return (ICollection<ICollection<Vec2>>) values;
//        }
//      }

//      public ICollection<Vec2> this[string name]
//      {
//        get
//        {
//          List<Vec2> vec2List = name != null ? this.GetBoundaryPoints(name) : throw new ArgumentNullException(nameof (name));
//          return vec2List.Count != 0 ? (ICollection<Vec2>) vec2List : throw new KeyNotFoundException();
//        }
//        set
//        {
//          if (name == null)
//            throw new ArgumentNullException(nameof (name));
//          this.Add(name, value);
//        }
//      }

//      public void Add(string name, ICollection<Vec2> points) => this.Add(name, points, true);

//      public void Add(string name, ICollection<Vec2> points, bool isAllowanceInside)
//      {
//        if (name == null)
//          throw new ArgumentNullException(nameof (name));
//        if (points == null)
//          throw new ArgumentNullException(nameof (points));
//        if (points.Count < 3)
//          throw new ArgumentException("At least three points are required.");
//        int num = MBAPI.IMBMission.AddBoundary(this._mission.Pointer, name, points.ToArray<Vec2>(), points.Count, isAllowanceInside) ? 1 : 0;
//        if (num == 0)
//          throw new ArgumentException("An element with the same name already exists.");
//        if (num != 0)
//        {
//          NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;
//          if (collectionChanged != null)
//            collectionChanged((object) this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object) name));
//        }
//        foreach (Team team in (List<Team>) Mission.Current.Teams)
//        {
//          foreach (Formation formation in (List<Formation>) team.FormationsIncludingSpecialAndEmpty)
//            formation.ResetMovementOrderPositionCache();
//        }
//      }

//      public bool ContainsKey(string name)
//      {
//        if (name == null)
//          throw new ArgumentNullException(nameof (name));
//        return this.GetBoundaryPoints(name).Count > 0;
//      }

//      public bool Remove(string name)
//      {
//        if (name == null)
//          throw new ArgumentNullException(nameof (name));
//        bool flag = MBAPI.IMBMission.RemoveBoundary(this._mission.Pointer, name);
//        if (flag)
//        {
//          NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;
//          if (collectionChanged != null)
//            collectionChanged((object) this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object) name));
//        }
//        foreach (Team team in (List<Team>) Mission.Current.Teams)
//        {
//          foreach (Formation formation in (List<Formation>) team.FormationsIncludingSpecialAndEmpty)
//            formation.ResetMovementOrderPositionCache();
//        }
//        return flag;
//      }

//      public bool TryGetValue(string name, out ICollection<Vec2> points)
//      {
//        points = name != null ? (ICollection<Vec2>) this.GetBoundaryPoints(name) : throw new ArgumentNullException(nameof (name));
//        return points.Count > 0;
//      }

//      private List<Vec2> GetBoundaryPoints(string name)
//      {
//        List<Vec2> boundaryPoints = new List<Vec2>();
//        Vec2[] vec2Array = new Vec2[10];
//        for (int boundaryPointOffset = 0; boundaryPointOffset < 1000; boundaryPointOffset += 10)
//        {
//          int retrievedPointCount = -1;
//          MBAPI.IMBMission.GetBoundaryPoints(this._mission.Pointer, name, boundaryPointOffset, vec2Array, 10, ref retrievedPointCount);
//          boundaryPoints.AddRange(((IEnumerable<Vec2>) vec2Array).Take<Vec2>(retrievedPointCount));
//          if (retrievedPointCount < 10)
//            break;
//        }
//        return boundaryPoints;
//      }

//      public event NotifyCollectionChangedEventHandler CollectionChanged;
//    }

//    public class DynamicallyCreatedEntity
//    {
//      public string Prefab;
//      public MissionObjectId ObjectId;
//      public MatrixFrame Frame;
//      public List<MissionObjectId> ChildObjectIds;

//      public DynamicallyCreatedEntity(
//        string prefab,
//        MissionObjectId objectId,
//        MatrixFrame frame,
//        ref List<MissionObjectId> childObjectIds)
//      {
//        this.Prefab = prefab;
//        this.ObjectId = objectId;
//        this.Frame = frame;
//        this.ChildObjectIds = childObjectIds;
//      }
//    }

//    [Flags]
//    [EngineStruct("Weapon_spawn_flag", true, "wsf", false)]
//    public enum WeaponSpawnFlags : uint
//    {
//      None = 0,
//      WithHolster = 1,
//      WithoutHolster = 2,
//      AsMissile = 4,
//      WithPhysics = 8,
//      WithStaticPhysics = 16, // 0x00000010
//      UseAnimationSpeed = 32, // 0x00000020
//      CannotBePickedUp = 64, // 0x00000040
//    }

//    [EngineStruct("Mission_combat_type", false, null)]
//    public enum MissionCombatType
//    {
//      Combat,
//      ArenaCombat,
//      NoCombat,
//    }

//    public enum BattleSizeType
//    {
//      Battle,
//      Siege,
//      SallyOut,
//    }

//    [EngineStruct("Agent_creation_result", false, null)]
//    internal struct AgentCreationResult
//    {
//      internal int Index;
//      internal UIntPtr AgentPtr;
//      internal UIntPtr PositionPtr;
//      internal UIntPtr IndexPtr;
//      internal UIntPtr FlagsPtr;
//      internal UIntPtr StatePtr;
//      internal UIntPtr MovementModePointer;
//      internal UIntPtr ControllerPointer;
//      internal UIntPtr MovementDirectionPointer;
//      internal UIntPtr PrimaryWieldedItemIndexPointer;
//      internal UIntPtr OffHandWieldedItemIndexPointer;
//      internal UIntPtr Channel0CurrentActionPointer;
//      internal UIntPtr Channel1CurrentActionPointer;
//      internal UIntPtr MaximumForwardUnlimitedSpeed;
//    }

//    public struct TimeSpeedRequest
//    {
//      public float RequestedTimeSpeed { get; private set; }

//      public int RequestID { get; private set; }

//      public TimeSpeedRequest(float requestedTime, int requestID)
//      {
//        this.RequestedTimeSpeed = requestedTime;
//        this.RequestID = requestID;
//      }
//    }

//    private enum GetNearbyAgentsAuxType
//    {
//      Friend = 1,
//      Enemy = 2,
//      All = 3,
//    }

//    public struct CorpseAgentInfo
//    {
//      public BasicCharacterObject CorpseBasicCharacterObject;
//      public Monster CorpseMonster;
//      public Equipment CorpseSpawnEquipment;
//      public MissionEquipment CorpseMissionEquipment;
//      public BodyProperties CorpseBodyPropertiesValue;
//      public int CorpseBodyPropertiesSeed;
//      public bool CorpseIsFemale;
//      public int CorpseTeamIndex;
//      public int CorpseFormationIndex;
//      public uint CorpseClothingColor1;
//      public uint CorpseClothingColor2;
//      public MissionPeer CorpseMissionPeer;
//      public MissionPeer CorpseOwningAgentMissionPeer;
//      public Vec3 CorpsePosition;
//      public Vec2 CorpseMovementDirection;
//      public int AgentCorpsesToFadeIndex;
//      public bool IsMount;
//      public MBList<MissionWeapon> AttachedWeapons;
//      public MBList<sbyte> AttachedWeaponBoneIndices;
//      public MBList<MatrixFrame> AttachedWeaponFrames;
//      public ActionIndexCache CorpseDeathActionIndex;
//    }

//    public static class MissionNetworkHelper
//    {
//      public static Agent GetAgentFromIndex(int agentIndex, bool canBeNull = false)
//      {
//        Agent agentWithIndex = Mission.Current.FindAgentWithIndex(agentIndex);
//        if (!canBeNull && agentWithIndex == null && agentIndex >= 0)
//        {
//          TaleWorlds.Library.Debug.Print("Agent with index: " + (object) agentIndex + " could not be found while reading reference from packet.");
//          throw new MBNotFoundException("Agent with index: " + (object) agentIndex + " could not be found while reading reference from packet.");
//        }
//        return agentWithIndex;
//      }

//      public static MBTeam GetMBTeamFromTeamIndex(int teamIndex)
//      {
//        if (Mission.Current == null)
//          throw new Exception("Mission.Current is null!");
//        return teamIndex < 0 ? MBTeam.InvalidTeam : new MBTeam(Mission.Current, teamIndex);
//      }

//      public static Team GetTeamFromTeamIndex(int teamIndex)
//      {
//        if (Mission.Current == null)
//          throw new Exception("Mission.Current is null!");
//        return teamIndex < 0 ? Team.Invalid : Mission.Current.Teams.Find(Mission.MissionNetworkHelper.GetMBTeamFromTeamIndex(teamIndex));
//      }

//      public static MissionObject GetMissionObjectFromMissionObjectId(
//        MissionObjectId missionObjectId)
//      {
//        if (Mission.Current == null)
//          throw new Exception("Mission.Current is null!");
//        if (missionObjectId.Id < 0)
//          return (MissionObject) null;
//        MissionObject fromMissionObjectId = Mission.Current.MissionObjects.FirstOrDefault<MissionObject>((Func<MissionObject, bool>) (mo => mo.Id == missionObjectId));
//        if (fromMissionObjectId == null)
//          MBDebug.Print("MissionObject with ID: " + (object) missionObjectId.Id + " runtime: " + missionObjectId.CreatedAtRuntime.ToString() + " could not be found.");
//        return fromMissionObjectId;
//      }

//      public static CombatLogData GetCombatLogDataForCombatLogNetworkMessage(
//        CombatLogNetworkMessage message)
//      {
//        if (Mission.Current == null)
//          throw new Exception("Mission.Current is null!");
//        Agent agentFromIndex1 = Mission.MissionNetworkHelper.GetAgentFromIndex(message.AttackerAgentIndex);
//        Agent agentFromIndex2 = Mission.MissionNetworkHelper.GetAgentFromIndex(message.VictimAgentIndex, true);
//        int num = agentFromIndex1 != null ? 1 : 0;
//        bool isAttackerAgentHuman = num != 0 && agentFromIndex1.IsHuman;
//        bool isAttackerAgentMine = num != 0 && agentFromIndex1.IsMine;
//        bool doesAttackerAgentHaveRiderAgent = num != 0 && agentFromIndex1.RiderAgent != null;
//        bool isAttackerAgentRiderAgentMine = doesAttackerAgentHaveRiderAgent && agentFromIndex1.RiderAgent.IsMine;
//        bool isAttackerAgentMount = num != 0 && agentFromIndex1.IsMount;
//        bool isVictimAgentDead = agentFromIndex2 != null && (double) agentFromIndex2.Health <= 0.0;
//        bool isVictimRiderAgentSameAsAttackerAgent = agentFromIndex1 != null && agentFromIndex2?.RiderAgent == agentFromIndex1;
//        return new CombatLogData(agentFromIndex1 == agentFromIndex2, isAttackerAgentHuman, isAttackerAgentMine, doesAttackerAgentHaveRiderAgent, isAttackerAgentRiderAgentMine, isAttackerAgentMount, agentFromIndex2 != null && agentFromIndex2.IsHuman, agentFromIndex2 != null && agentFromIndex2.IsMine, isVictimAgentDead, agentFromIndex2?.RiderAgent != null, ((int) agentFromIndex2?.RiderAgent?.IsMine ?? 0) != 0, agentFromIndex2 != null && agentFromIndex2.IsMount, Mission.MissionNetworkHelper.GetMissionObjectFromMissionObjectId(message.MissionObjectHitId), isVictimRiderAgentSameAsAttackerAgent, message.CrushedThrough, message.Chamber, message.Distance)
//        {
//          DamageType = message.DamageType,
//          IsRangedAttack = message.IsRangedAttack,
//          IsFriendlyFire = message.IsFriendlyFire,
//          IsFatalDamage = message.IsFatalDamage,
//          BodyPartHit = message.BodyPartHit,
//          HitSpeed = message.HitSpeed,
//          InflictedDamage = message.InflictedDamage,
//          AbsorbedDamage = message.AbsorbedDamage,
//          ModifiedDamage = message.ModifiedDamage,
//          ReflectedDamage = message.ReflectedDamage,
//          VictimAgentName = agentFromIndex2?.MissionPeer?.DisplayedName ?? agentFromIndex2?.Name ?? ""
//        };
//      }
//    }

//    public class Missile : MBMissile
//    {
//      public GameEntity Entity { get; private set; }

//      public MissionWeapon Weapon { get; private set; }

//      public Agent ShooterAgent { get; private set; }

//      public MissionObject MissionObjectToIgnore { get; private set; }

//      public GameEntity AlreadyHitEntityToIgnore { get; private set; }

//      public Missile(
//        Mission mission,
//        int index,
//        GameEntity entity,
//        Agent shooterAgent,
//        MissionWeapon weapon,
//        MissionObject missionObjectToIgnore)
//        : base(mission)
//      {
//        this.Index = index;
//        this.Entity = entity;
//        this.Weapon = weapon;
//        this.ShooterAgent = shooterAgent;
//        this.MissionObjectToIgnore = missionObjectToIgnore;
//      }

//      public void CalculatePassbySoundParametersMT(ref SoundEventParameter soundEventParameter)
//      {
//        if (!this.Weapon.CurrentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CanPenetrateShield))
//          return;
//        soundEventParameter.Update("impactModifier", 0.3f);
//      }

//      public void CalculateBounceBackVelocity(
//        Vec3 rotationSpeed,
//        AttackCollisionData collisionData,
//        out Vec3 velocity,
//        out Vec3 angularVelocity)
//      {
//        Vec3 missileVelocity = collisionData.MissileVelocity;
//        MissionWeapon weapon = this.Weapon;
//        double num1 = (double) weapon.CurrentUsageItem.WeaponLength * 0.0099999997764825821;
//        weapon = this.Weapon;
//        double scaleFactor = (double) weapon.Item.ScaleFactor;
//        float num2 = (float) (num1 * scaleFactor);
//        PhysicsMaterial fromIndex = PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex);
//        float num3;
//        float num4;
//        if (fromIndex.IsValid)
//        {
//          num3 = fromIndex.GetDynamicFriction();
//          num4 = fromIndex.GetRestitution();
//        }
//        else
//        {
//          num3 = 0.3f;
//          num4 = 0.4f;
//        }
//        PhysicsMaterial fromName = PhysicsMaterial.GetFromName(this.Weapon.Item.PrimaryWeapon.PhysicsMaterial);
//        float num5;
//        float num6;
//        if (fromName.IsValid)
//        {
//          num5 = fromName.GetDynamicFriction();
//          num6 = fromName.GetRestitution();
//        }
//        else
//        {
//          num5 = 0.3f;
//          num6 = 0.4f;
//        }
//        float num7 = (float) (((double) num3 + (double) num5) * 0.5);
//        float num8 = (float) (((double) num4 + (double) num6) * 0.5);
//        Vec3 vec3_1 = missileVelocity.Reflect(collisionData.CollisionGlobalNormal);
//        float num9 = Vec3.DotProduct(vec3_1, collisionData.CollisionGlobalNormal);
//        Vec3 v2 = collisionData.CollisionGlobalNormal.RotateAboutAnArbitraryVector(Vec3.CrossProduct(vec3_1, collisionData.CollisionGlobalNormal).NormalizedCopy(), 1.57079637f);
//        float num10 = Vec3.DotProduct(vec3_1, v2);
//        velocity = collisionData.CollisionGlobalNormal * (num8 * num9) + v2 * (num10 * num7);
//        velocity += collisionData.CollisionGlobalNormal;
//        angularVelocity = -Vec3.CrossProduct(collisionData.CollisionGlobalNormal, velocity);
//        float lengthSquared = angularVelocity.LengthSquared;
//        float weight = this.Weapon.GetWeight();
//        float num11;
//        switch (this.Weapon.CurrentUsageItem.WeaponClass)
//        {
//          case WeaponClass.Arrow:
//          case WeaponClass.Bolt:
//            num11 = (float) (0.25 * (double) weight * 0.054999999701976776 * 0.054999999701976776 + 0.0833333283662796 * (double) weight * (double) num2 * (double) num2);
//            break;
//          case WeaponClass.SlingStone:
//          case WeaponClass.Stone:
//          case WeaponClass.BallistaStone:
//            num11 = (float) (0.40000000596046448 * (double) weight * 0.10000000149011612 * 0.10000000149011612);
//            break;
//          case WeaponClass.Boulder:
//          case WeaponClass.BallistaBoulder:
//            num11 = (float) (0.40000000596046448 * (double) weight * 0.40000000596046448 * 0.40000000596046448);
//            break;
//          case WeaponClass.ThrowingAxe:
//            num11 = (float) (0.25 * (double) weight * 0.20000000298023224 * 0.20000000298023224 + 0.0833333283662796 * (double) weight * (double) num2 * (double) num2) + (float) (0.5 * (double) weight * 0.20000000298023224 * 0.20000000298023224);
//            Vec3 vec3_2 = rotationSpeed * num4;
//            angularVelocity = this.Entity.GetGlobalFrame().rotation.TransformToParent(rotationSpeed * num4);
//            break;
//          case WeaponClass.ThrowingKnife:
//            num11 = (float) (0.25 * (double) weight * 0.20000000298023224 * 0.20000000298023224 + 0.0833333283662796 * (double) weight * (double) num2 * (double) num2) + (float) (0.5 * (double) weight * 0.20000000298023224 * 0.20000000298023224);
//            Vec3 vec3_3 = rotationSpeed * num4;
//            angularVelocity = this.Entity.GetGlobalFrame().rotation.TransformToParent(rotationSpeed * num4);
//            break;
//          case WeaponClass.Javelin:
//            num11 = (float) (0.25 * (double) weight * 0.1550000011920929 * 0.1550000011920929 + 0.0833333283662796 * (double) weight * (double) num2 * (double) num2);
//            break;
//          default:
//            TaleWorlds.Library.Debug.FailedAssert("Unknown missile type!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Mission.cs", nameof (CalculateBounceBackVelocity), 298);
//            num11 = 0.0f;
//            break;
//        }
//        float num12 = 0.5f * num11 * lengthSquared;
//        float length = missileVelocity.Length;
//        float num13 = MathF.Sqrt((float) ((0.5 * (double) weight * (double) length * (double) length - (double) num12) * 2.0) / weight);
//        velocity *= num13 / length;
//        float maximumValue1 = CompressionMission.SpawnedItemVelocityCompressionInfo.GetMaximumValue();
//        float maximumValue2 = CompressionMission.SpawnedItemAngularVelocityCompressionInfo.GetMaximumValue();
//        if ((double) velocity.LengthSquared > (double) maximumValue1 * (double) maximumValue1)
//          velocity = velocity.NormalizedCopy() * maximumValue1;
//        if ((double) angularVelocity.LengthSquared <= (double) maximumValue2 * (double) maximumValue2)
//          return;
//        angularVelocity = angularVelocity.NormalizedCopy() * maximumValue2;
//      }

//      public void PassThroughEntity(GameEntity entity)
//      {
//        this.AlreadyHitEntityToIgnore = entity;
//        this.SetVelocity(this.GetVelocity() * 0.8f);
//      }
//    }

//    public struct SpectatorData
//    {
//      public Agent AgentToFollow { get; private set; }

//      public IAgentVisual AgentVisualToFollow { get; private set; }

//      public SpectatorCameraTypes CameraType { get; private set; }

//      public SpectatorData(
//        Agent agentToFollow,
//        IAgentVisual agentVisualToFollow,
//        SpectatorCameraTypes cameraType)
//      {
//        this.AgentToFollow = agentToFollow;
//        this.CameraType = cameraType;
//        this.AgentVisualToFollow = agentVisualToFollow;
//      }
//    }

//    private class DynamicEntityInfo
//    {
//      public GameEntity Entity;
//      public TaleWorlds.Core.Timer TimerToDisable;
//    }

//    public enum State
//    {
//      NewlyCreated,
//      Initializing,
//      Continuing,
//      EndingNextFrame,
//      Over,
//    }

//    public enum BattleSizeQualifier
//    {
//      Small,
//      Medium,
//    }

//    public enum MissionTeamAITypeEnum
//    {
//      NoTeamAI,
//      FieldBattle,
//      Siege,
//      SallyOut,
//      NavalBattle,
//    }

//    public enum MissileCollisionReaction
//    {
//      Invalid = -1, // 0xFFFFFFFF
//      Stick = 0,
//      PassThrough = 1,
//      BounceBack = 2,
//      BecomeInvisible = 3,
//      Count = 4,
//    }

//    public enum MissionTickAction
//    {
//      TryToSheathWeaponInHand,
//      RemoveEquippedWeapon,
//      TryToWieldWeaponInSlot,
//      DropItem,
//      RegisterDrownBlow,
//    }

//    public delegate void OnBeforeAgentRemovedDelegate(
//      Agent affectedAgent,
//      Agent affectorAgent,
//      AgentState agentState,
//      KillingBlow killingBlow);

//    public delegate void OnAddSoundAlarmFactorToAgentsDelegate(
//      Agent alarmCreatorAgent,
//      in Vec3 soundPosition,
//      float soundLevelSquareRoot);

//    public delegate void OnMainAgentChangedDelegate(Agent oldAgent);

//    public sealed class TeamCollection : List<Team>
//    {
//      private Mission _mission;
//      private Team _playerTeam;

//      public event Action<Team, Team> OnPlayerTeamChanged;

//      public Team Attacker { get; private set; }

//      public Team Defender { get; private set; }

//      public Team AttackerAlly { get; private set; }

//      public Team DefenderAlly { get; private set; }

//      public Team Player
//      {
//        get => this._playerTeam;
//        set
//        {
//          if (this._playerTeam == value)
//            return;
//          this.SetPlayerTeamAux(value == null ? -1 : this.IndexOf(value));
//        }
//      }

//      public Team PlayerEnemy { get; private set; }

//      public Team PlayerAlly { get; private set; }

//      public TeamCollection(Mission mission)
//        : base((IEnumerable<Team>) new List<Team>())
//      {
//        this._mission = mission;
//      }

//      private MBTeam AddNative()
//      {
//        return new MBTeam(this._mission, MBAPI.IMBMission.AddTeam(this._mission.Pointer));
//      }

//      public new void Add(Team t)
//      {
//        MBDebug.ShowWarning("Pre-created Team can not be added to TeamCollection!");
//      }

//      public Team Add(
//        BattleSideEnum side,
//        uint color = 4294967295,
//        uint color2 = 4294967295,
//        Banner banner = null,
//        bool isPlayerGeneral = true,
//        bool isPlayerSergeant = false,
//        bool isSettingRelations = true)
//      {
//        MBDebug.Print("----------Mission-AddTeam-" + (object) side);
//        Team team = new Team(this.AddNative(), side, this._mission, color, color2, banner);
//        if (!GameNetwork.IsClientOrReplay)
//          team.SetPlayerRole(isPlayerGeneral, isPlayerSergeant);
//        base.Add(team);
//        foreach (MissionBehavior missionBehavior in this._mission.MissionBehaviors)
//          missionBehavior.OnAddTeam(team);
//        if (isSettingRelations)
//          this.SetRelations(team);
//        switch (side)
//        {
//          case BattleSideEnum.Defender:
//            if (this.Defender == null)
//            {
//              this.Defender = team;
//              break;
//            }
//            if (this.DefenderAlly == null)
//            {
//              this.DefenderAlly = team;
//              break;
//            }
//            break;
//          case BattleSideEnum.Attacker:
//            if (this.Attacker == null)
//            {
//              this.Attacker = team;
//              break;
//            }
//            if (this.AttackerAlly == null)
//            {
//              this.AttackerAlly = team;
//              break;
//            }
//            break;
//        }
//        this.AdjustPlayerTeams();
//        foreach (MissionBehavior missionBehavior in this._mission.MissionBehaviors)
//          missionBehavior.AfterAddTeam(team);
//        return team;
//      }

//      public Team Find(MBTeam mbTeam)
//      {
//        if (mbTeam.IsValid)
//        {
//          for (int index = 0; index < this.Count; ++index)
//          {
//            Team team = this[index];
//            if (team.MBTeam == mbTeam)
//              return team;
//          }
//        }
//        return Team.Invalid;
//      }

//      public void ClearResources()
//      {
//        this.Attacker = (Team) null;
//        this.AttackerAlly = (Team) null;
//        this.Defender = (Team) null;
//        this.DefenderAlly = (Team) null;
//        this._playerTeam = (Team) null;
//        this.PlayerEnemy = (Team) null;
//        this.PlayerAlly = (Team) null;
//        Team.Invalid = (Team) null;
//      }

//      public new void Clear()
//      {
//        foreach (Team team in (List<Team>) this)
//          team.Clear();
//        base.Clear();
//        this.ClearResources();
//        MBAPI.IMBMission.ResetTeams(this._mission.Pointer);
//      }

//      private void SetRelations(Team team)
//      {
//        BattleSideEnum side = team.Side;
//        for (int index = 0; index < this.Count; ++index)
//        {
//          Team otherTeam = this[index];
//          if (side.IsOpponentOf(otherTeam.Side))
//            team.SetIsEnemyOf(otherTeam, true);
//        }
//      }

//      private void SetPlayerTeamAux(int index)
//      {
//        Team playerTeam = this._playerTeam;
//        this._playerTeam = index == -1 ? (Team) null : this[index];
//        this.AdjustPlayerTeams();
//        Action<Team, Team> playerTeamChanged = this.OnPlayerTeamChanged;
//        if (playerTeamChanged == null)
//          return;
//        playerTeamChanged(playerTeam, this._playerTeam);
//      }

//      private void AdjustPlayerTeams()
//      {
//        if (this.Player == null)
//        {
//          this.PlayerEnemy = (Team) null;
//          this.PlayerAlly = (Team) null;
//        }
//        else if (this.Player == this.Attacker)
//        {
//          this.PlayerEnemy = this.Defender == null || !this.Player.IsEnemyOf(this.Defender) ? (Team) null : this.Defender;
//          if (this.AttackerAlly != null && this.Player.IsFriendOf(this.AttackerAlly))
//            this.PlayerAlly = this.AttackerAlly;
//          else
//            this.PlayerAlly = (Team) null;
//        }
//        else
//        {
//          if (this.Player != this.Defender)
//            return;
//          this.PlayerEnemy = this.Attacker == null || !this.Player.IsEnemyOf(this.Attacker) ? (Team) null : this.Attacker;
//          if (this.DefenderAlly != null && this.Player.IsFriendOf(this.DefenderAlly))
//            this.PlayerAlly = this.DefenderAlly;
//          else
//            this.PlayerAlly = (Team) null;
//        }
//      }

//      private int TeamCountNative => MBAPI.IMBMission.GetNumberOfTeams(this._mission.Pointer);
//    }
//  }
//}
