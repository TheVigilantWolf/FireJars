using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace FireJars
{
    public class FireJarImpactVfxBehavior : MissionLogic
    {
        private const string FireJarItemId = "firejar";
        private const string SlingAmmoItemId = "sling_firejarammo";
        private const string ImpactParticleName = "firejarboom";

        private readonly Dictionary<int, ItemObject> _missileItems = new Dictionary<int, ItemObject>();

        public override void OnAgentShootMissile(Agent shooterAgent, EquipmentIndex weaponIndex, Vec3 position, Vec3 velocity, Mat3 orientation, bool hasRigidBody, int forcedMissileIndex)
        {
            base.OnAgentShootMissile(shooterAgent, weaponIndex, position, velocity, orientation, hasRigidBody, forcedMissileIndex);

            if (shooterAgent == null || forcedMissileIndex < 0)
                return;

            var equipment = shooterAgent.Equipment[(int) weaponIndex];
            var item = equipment.Item;
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

            ItemObject item = null;

            if (collisionData.IsMissile)
            {
                var index = collisionData.AffectorWeaponSlotOrMissileIndex;

                // Try to resolve via Mission.MissilesList (authoritative missile->weapon mapping)
                if (index >= 0)
                {
                    var missile = mission.MissilesList?.FirstOrDefault(m => m.Index == index);
                    if (missile != null)
                        item = missile.Weapon.Item;
                }

                // Fallback: cache keyed by forcedMissileIndex recorded at shoot time
                if (item == null)
                    _missileItems.TryGetValue(index, out item);

                // Fallback: equipment slot when index represents weapon slot
                if (item == null && index >= 0 && index < (int) EquipmentIndex.NumAllWeaponSlots)
                {
                    var equipment = attacker.Equipment[index];
                    item = equipment.Item;
                }
            }

            if (item == null)
                item = attacker.WieldedWeapon.Item;

            if (item == null || (item.StringId != FireJarItemId && item.StringId != SlingAmmoItemId))
                return;

            MatrixFrame frame = MatrixFrame.Identity;
            frame.origin = collisionData.CollisionGlobalPosition;

            mission.AddParticleSystemBurstByName(ImpactParticleName, frame, false);
        }
    }
}