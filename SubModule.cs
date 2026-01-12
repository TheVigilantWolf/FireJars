using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FireJars
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            try
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=FJ_SubmoduleLoaded}FireJars: SubModule loaded (Harmony PatchAll will run)").ToString(),
                    Colors.Green));
            }
            catch
            {
            }

            new Harmony("FireJars.Mod").PatchAll();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            ItemDefinitionApplier.ApplyAll();

            try
            {
                if (gameStarterObject is TaleWorlds.CampaignSystem.CampaignGameStarter && Hero.MainHero != null)
                    EquipmentStackApplier.ApplyToHero(Hero.MainHero);
            }
            catch
            {
                // ignore in modes without a main hero (e.g., Custom Battle)
            }
        }

        // This is the piece that spawns impact particle in missions (battles/sieges/etc.)
        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            ItemDefinitionApplier.ApplyAll();

            try
            {
                if (Hero.MainHero != null)
                    EquipmentStackApplier.ApplyToHero(Hero.MainHero);
            }
            catch
            {
                // ignore if no main hero in this game mode
            }

            mission.AddMissionBehavior(new FireJarImpactVfxBehavior());
            mission.AddMissionBehavior(new FireJarKnockOffBehavior());
        }
    }
}
