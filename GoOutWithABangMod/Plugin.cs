using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GoOutWithABang.Patches;
using HarmonyLib;

namespace GoOutWithABang
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class GoOutWithABangModBase : BaseUnityPlugin
    {

        private const string modGUID = "Chrigi.GoOutWithABangMod";
        private const string modName = "Go Out With A Bang Mod";
        private const string modVersion = "1.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);


        private static GoOutWithABangModBase Instance;
        internal static ConfigEntry<bool> SuffocationSetting;
        internal static ConfigEntry<bool> BlastSetting;
        internal static ConfigEntry<bool> UnknownSetting;
        internal static ConfigEntry<bool> StrangulationSetting;
        internal static ConfigEntry<bool> MaulingSetting;
        internal static ConfigEntry<bool> BludgeoningSetting;
        internal static ConfigEntry<bool> GravitySetting;
        internal static ConfigEntry<bool> GunshotsSetting;
        internal static ConfigEntry<bool> CrushingSetting;
        internal static ConfigEntry<bool> DrowningSetting;
        internal static ConfigEntry<bool> AbandonedSetting;
        internal static ConfigEntry<bool> ElectrocutionSetting;
        internal static ConfigEntry<bool> KickingSetting;






        public static ManualLogSource mls;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);


            mls.LogInfo("Time to go out with a bang!");
            SuffocationSetting = Config.Bind("Settings", "Suffocation", true, "Enable Explosion on Suffocation");
            BlastSetting = Config.Bind("Settings", "Blast", false, "Enable Explosion on Blast");
            UnknownSetting = Config.Bind("Settings", "Unknown", false, "Enable Explosion on Unknown");
            StrangulationSetting = Config.Bind("Settings", "Strangulation", true, "Enable Explosion on Strangulation");
            MaulingSetting = Config.Bind("Settings", "Mauling", true, "Enable Explosion on Mauling");
            BludgeoningSetting = Config.Bind("Settings", "Bludgeoning", true, "Enable Explosion on Bludgeoning");
            GravitySetting = Config.Bind("Settings", "Gravity", true, "Enable Explosion on Gravity");
            GunshotsSetting = Config.Bind("Settings", "Gunshots", true, "Enable Explosion on Gunshots");
            CrushingSetting = Config.Bind("Settings", "Crushing", true, "Enable Explosion on Crushing");
            DrowningSetting = Config.Bind("Settings", "Drowning", true, "Enable Explosion on Drowning");
            AbandonedSetting = Config.Bind("Settings", "Abandoned", true, "Enable Explosion on Abandoned");
            ElectrocutionSetting = Config.Bind("Settings", "Electrocution", true, "Enable Explosion on Electrocution");
            KickingSetting = Config.Bind("Settings", "Kicking", true, "Enable Explosion on Kicking");



            harmony.PatchAll(typeof(LandMinePatch));
            harmony.PatchAll(typeof(GoOutWithABangModBase));






        }

    }
}