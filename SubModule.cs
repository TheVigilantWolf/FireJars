using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;

namespace FireJars
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            new Harmony("FireJars.Mod").PatchAll();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            ItemDefinitionApplier.ApplyAll();
            EquipmentStackApplier.ApplyToHero(Hero.MainHero);
        }

        // This is the piece that spawns impact particle in missions (battles/sieges/etc.)
        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            ItemDefinitionApplier.ApplyAll();
            EquipmentStackApplier.ApplyToHero(Hero.MainHero);
            mission.AddMissionBehavior(new FireJarImpactVfxBehavior());
        }
    }
}
