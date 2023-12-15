using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
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


        public static ManualLogSource mls;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);


            mls.LogInfo("________________\nKarma is a bitch!\n________________");


            harmony.PatchAll(typeof(NoisemakerPropPatch));




        }
    }
}
