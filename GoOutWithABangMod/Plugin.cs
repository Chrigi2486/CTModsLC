using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
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
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);


        private static GoOutWithABangModBase Instance;


        internal ManualLogSource mls;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);


            mls.LogInfo("Time to go out with a bang!");
            mls.LogInfo("Time to go out with a bang!");
            mls.LogInfo("Time to go out with a bang!");
            mls.LogInfo("Time to go out with a bang!");
            mls.LogInfo("Time to go out with a bang!");


            harmony.PatchAll(typeof(LandMinePatch));




        }

    }
}
