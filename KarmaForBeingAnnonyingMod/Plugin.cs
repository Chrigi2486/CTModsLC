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
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);


        private static KarmaForBeingAnnoyingModBase Instance;

        internal static ConfigEntry<bool> AnnoyingItemSetting;
        internal static ConfigEntry<float> ProbabilitySetting;
        public static ManualLogSource mls;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            AnnoyingItemSetting = ((BaseUnityPlugin)this).Config.Bind<bool>("KarmaForBeingAnnoying Settings", "ON OFF switch", true, "Turns functionality on or off");
            ProbabilitySetting = ((BaseUnityPlugin)this).Config.Bind<float>("Probability Settings", "General Probability", 0.1f, "Set probability of exploding: default 0.1f");
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);


            mls.LogInfo("________________\nKarma is a bitch!\n________________");


            harmony.PatchAll(typeof(NoisemakerPropPatch));




        }
    }
}
