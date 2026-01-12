using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace FireJars
{
    /// <summary>
    /// On FireJar impact, instantly unhorses mounted agents in a radius and plays a fall reaction.
    /// Singleplayer-safe approach. Avoid in MP unless you handle replication.
    /// </summary>
    public sealed class FireJarKnockOffBehavior : MissionLogic
    {
        private const string FireJarItemId = "firejar";
        private const string SlingAmmoItemId = "sling_firejarammo";

        private const float KnockOffRadius = 2.5f;          // tweak
        private const float DedupTimeWindowSeconds = 0.03f;
        private const float DedupPosEpsilonSq = 0.15f * 0.15f;

        private readonly Dictionary<int, ItemObject> _missileItems = new Dictionary<int, ItemObject>();

        private float _lastImpactTime = -999f;
        private Vec3 _lastImpactPos = Vec3.Zero;

        private static readonly TaleWorlds.MountAndBlade.ActionIndexCache KnockOffAction =
            TaleWorlds.MountAndBlade.ActionIndexCache.act_strike_fall_back_back_rise;

        private static readonly MethodInfo MountAgentSetter =
            AccessTools.PropertySetter(typeof(Agent), nameof(Agent.MountAgent));

        public override void OnAgentShootMissile(Agent shooterAgent, EquipmentIndex weaponIndex, Vec3 position, Vec3 velocity,
            Mat3 orientation, bool hasRigidBody, int forcedMissileIndex)
        {
            base.OnAgentShootMissile(shooterAgent, weaponIndex, position, velocity, orientation, hasRigidBody, forcedMissileIndex);

            if (shooterAgent == null || forcedMissileIndex < 0)
                return;

            var item = shooterAgent.Equipment[(int)weaponIndex].Item;
            if (item != null)
                _missileItems[forcedMissileIndex] = item;
        }

        public override void OnMissileRemoved(int missileIndex)
        {
            base.OnMissileRemoved(missileIndex);
            _missileItems.Remove(missileIndex);
        }

        public override void OnMissileHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
        {
            if (isCanceled || attacker == null)
                return;

            var mission = Mission.Current;
            if (mission == null)
                return;

            var item = ResolveImpactItem(attacker, collisionData, mission);
            if (item == null)
                return;

            var settings = SettingsUtil.TryGetSettings();
            if (settings != null)
            {
                if (item.StringId == FireJarItemId && !settings.DismountRiders)
                    return;
                if (item.StringId == SlingAmmoItemId && !settings.DismountSlingRiders)
                    return;
            }

            if (item.StringId != FireJarItemId && item.StringId != SlingAmmoItemId)
                return;

            Vec3 impactPos = collisionData.CollisionGlobalPosition;

            // dedupe
            if (mission.CurrentTime - _lastImpactTime <= DedupTimeWindowSeconds &&
                impactPos.DistanceSquared(_lastImpactPos) <= DedupPosEpsilonSq)
                return;

            _lastImpactTime = mission.CurrentTime;
            _lastImpactPos = impactPos;

            KnockOffMountedAgents(attacker, impactPos, KnockOffRadius);
        }

        private ItemObject ResolveImpactItem(Agent attacker, AttackCollisionData collisionData, Mission mission)
        {
            ItemObject item = null;

            if (collisionData.IsMissile)
            {
                int index = collisionData.AffectorWeaponSlotOrMissileIndex;

                if (index >= 0)
                {
                    var missile = mission.MissilesList?.FirstOrDefault(m => m.Index == index);
                    if (missile != null)
                        item = missile.Weapon.Item;
                }

                if (item == null)
                    _missileItems.TryGetValue(index, out item);

                if (item == null && index >= 0 && index < (int)EquipmentIndex.NumAllWeaponSlots)
                    item = attacker.Equipment[index].Item;
            }

            if (item == null)
                item = attacker.WieldedWeapon.Item;

            return item;
        }

        private void KnockOffMountedAgents(Agent attacker, Vec3 center, float radius)
        {
            if (MountAgentSetter == null)
                return;

            float rSq = radius * radius;

            var agents = Mission.Agents;
            if (agents == null)
                return;

            for (int i = 0; i < agents.Count; i++)
            {
                Agent a = agents[i];
                if (a == null || !a.IsActive() || !a.IsHuman)
                    continue;

                if (!a.HasMount || a.MountAgent == null || !a.MountAgent.IsActive())
                    continue;

                if (a.Position.DistanceSquared(center) > rSq)
                    continue;

                ForceInstantUnhorseAndFall(a);
            }
        }

        private void ForceInstantUnhorseAndFall(Agent rider)
        {
            if (rider?.MountAgent == null)
                return;

            MountAgentSetter.Invoke(rider, new object[] { null });

            var act = KnockOffAction;
            rider.SetActionChannel(0, in act, ignorePriority: true);
        }
    }
}
