using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using KarmaForBeingAnnoying.Patches;

namespace KarmaForBeingAnnoying
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class KarmaForBeingAnnoyingModBase : BaseUnityPlugin
    {
        private const string modGUID = "Chrigi.KarmaForBeingAnnoyingMod";
        private const string modName = "Karma For Being Annoying Mod";
        private const string modVersion = "1.1.0";

        private readonly Harmony harmony = new Harmony(modGUID);


        private static KarmaForBeingAnnoyingModBase Instance;

        internal static ConfigEntry<bool> AnnoyingItemSetting;
        internal static ConfigEntry<float> ProbabilitySetting;
        internal static ConfigEntry<float> ProbabilityRemoteSetting;
        internal static ConfigEntry<float> ProbabilityAirhornSetting;
        internal static ConfigEntry<float> ProbabilityClownhornSetting;
        internal static ConfigEntry<float> ProbabilityCashRegisterSetting;
        internal static ConfigEntry<float> ProbabilityHairDryerSetting;
        internal static ConfigEntry<float> DelaySetting;
        internal static ConfigEntry<float> KillRangeSetting;
        internal static ConfigEntry<float> DamageRangeSetting;
        internal static ConfigEntry<bool> RemoteSetting;
        internal static ConfigEntry<bool> SpawnmineSetting;
        public static ManualLogSource mls;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);


            mls.LogInfo("________________\nKarma is a bitch!\n________________");
            SetCFG();

            harmony.PatchAll(typeof(NoisemakerPropPatch));




        }
        private static void SetCFG()
        {
            AnnoyingItemSetting = Instance.Config.Bind<bool>("KarmaForBeingAnnoying Settings", "ON OFF switch", true, "Turns functionality on or off");
            ProbabilitySetting = Instance.Config.Bind<float>("Probability Settings", "General Probability", 0.1f, "Set probability of exploding");
            ProbabilityRemoteSetting = Instance.Config.Bind<float>("Probability Settings", "Remote Probability", 0.1f, "Set probability of exploding when using Remote");
            ProbabilityAirhornSetting = Instance.Config.Bind<float>("Probability Settings", "Airhorn Probability", 0.1f, "Set probability of exploding when using Airhorn");
            ProbabilityClownhornSetting = Instance.Config.Bind<float>("Probability Settings", "Clownhorn Probability", 0.1f, "Set probability of exploding when using Clownhorn");
            ProbabilityCashRegisterSetting = Instance.Config.Bind<float>("Probability Settings", "Cashregister Probability", 0.1f, "Set probability of exploding when using Cashregister");
            ProbabilityHairDryerSetting = Instance.Config.Bind<float>("Probability Settings", "Hairdryer Probability", 0.1f, "Set probability of exploding when using Hairdryer");
            DelaySetting = Instance.Config.Bind<float>("Delay Settings", "General Delay", 0.5f, "Set delay of explosion");
            KillRangeSetting = Instance.Config.Bind<float>("Kill Range Settings", "General Kill Range", 10f, "Set kill range of explosion");
            DamageRangeSetting = Instance.Config.Bind<float>("Damage Range Settings", "General Damage Range", 1f, "Set damage range of explosion");
            RemoteSetting = Instance.Config.Bind<bool>("KarmaForBeingAnnoying Settings", "UseOnRemote", true, "Defines if Remote sets off explosion based on params");
            SpawnmineSetting = Instance.Config.Bind<bool>("KarmaForBeingAnnoying Settings", "SpawnMine", true, "Defines if a mine gets spawned (only works for server host)");
        } 
    }
}
