using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LCTestModChrigi.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCTestModChrigi
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TestModBase : BaseUnityPlugin
    {
        private const string modGUID = "Chrigi.LCTestMod";
        private const string modName = "LC Test Mod";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);


        private static TestModBase Instance;


        public static ManualLogSource mls;


        void Awake()
        {
            if (Instance == null) {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);


            mls.LogInfo("Test mod has arisen!");


            harmony.PatchAll(typeof(ChrigiGamePatch));




        }



    }
}
