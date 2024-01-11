using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;
using Unity.Netcode;

namespace GoOutWithABang.Patches
{
    internal class LandMinePatch
    {

        private static ManualLogSource logger = GoOutWithABangModBase.mls;

        private static bool ded;
        private static RoundManager currentRound;
        private static bool server = false;
        private static bool isLevelLoaded = false;
        private static bool deathMineSpawned = false;
        private static int mine = 0;
        private static bool suff;
        private static bool blast;
        private static bool unknown;
        private static bool strang;
        private static bool maul;
        private static bool bludg;
        private static bool grav;
        private static bool gun;
        private static bool crush;
        private static bool drown;
        private static bool aban;
        private static bool electro;
        private static bool kick;


        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        static void LoadNewLevelPatch()
        {
            server = false;
            isLevelLoaded = false;

            currentRound = RoundManager.Instance;
            if (currentRound.IsServer)
            {
                server = true;
            }
            ded = false;
            suff = GoOutWithABangModBase.SuffocationSetting.Value;
            blast = GoOutWithABangModBase.BlastSetting.Value;
            unknown = GoOutWithABangModBase.UnknownSetting.Value;
            strang = GoOutWithABangModBase.StrangulationSetting.Value;
            maul = GoOutWithABangModBase.MaulingSetting.Value;
            bludg = GoOutWithABangModBase.BludgeoningSetting.Value;
            grav = GoOutWithABangModBase.GravitySetting.Value;
            gun = GoOutWithABangModBase.GunshotsSetting.Value;
            crush = GoOutWithABangModBase.CrushingSetting.Value;
            drown = GoOutWithABangModBase.DrowningSetting.Value;
            aban = GoOutWithABangModBase.AbandonedSetting.Value;
            electro = GoOutWithABangModBase.ElectrocutionSetting.Value;
            kick = GoOutWithABangModBase.KickingSetting.Value;

        }

        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingNewLevelClientRpc")]
        [HarmonyPostfix]
        static void FinishGeneratingNewLevelClientRpcPatch()
        {
            isLevelLoaded = true;
            logger.LogInfo("Level Loaded, any new mine spawned will blow up instantly");
            int len = currentRound.currentLevel.spawnableMapObjects.Count();
            for (int i = 0; i < len; i++)
            {
                if (currentRound.currentLevel.spawnableMapObjects[i].prefabToSpawn.name == "Landmine")
                {
                    logger.LogInfo("Found Mine Index: " + i);
                    mine = i;
                    break;
                }
            }


        }

        [HarmonyPatch(typeof(Landmine), "Start")]
        [HarmonyPrefix]
        static void LandminePatch(ref Landmine __instance)
        {
            logger.LogInfo("Landmine Spawned");

            if (isLevelLoaded && deathMineSpawned)
            {
                logger.LogInfo("Forcing mine explosion");
                __instance.ExplodeMineServerRpc();
                deathMineSpawned = false;
                logger.LogInfo("Mine forcefully activated");
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB))]
        [HarmonyPatch("KillPlayerServerRpc")]
        [HarmonyPostfix]
        static void PlayerControllerBPatch(PlayerControllerB __instance)
        {
            NetworkBehaviour baseplayer = (NetworkBehaviour)__instance;

            if (server && __instance.isPlayerDead && (!baseplayer.IsOwnedByServer || !ded)) // && __instance.causeOfDeath != CauseOfDeath.Suffocation  && __instance.causeOfDeath != CauseOfDeath.Strangulation)
            {
                if (__instance.causeOfDeath == CauseOfDeath.Suffocation && !suff || __instance.causeOfDeath == CauseOfDeath.Mauling && !maul || __instance.causeOfDeath == CauseOfDeath.Bludgeoning && !bludg || __instance.causeOfDeath == CauseOfDeath.Gravity && !grav || __instance.causeOfDeath == CauseOfDeath.Gunshots && !gun || __instance.causeOfDeath == CauseOfDeath.Crushing && !crush || __instance.causeOfDeath == CauseOfDeath.Drowning && !drown || __instance.causeOfDeath == CauseOfDeath.Abandoned && !aban || __instance.causeOfDeath == CauseOfDeath.Electrocution && !electro || __instance.causeOfDeath == CauseOfDeath.Kicking && !kick || __instance.causeOfDeath == CauseOfDeath.Strangulation && !strang || __instance.causeOfDeath == CauseOfDeath.Unknown && !unknown || __instance.causeOfDeath == CauseOfDeath.Blast && !blast)
                {
                    return;
                }
                if (baseplayer.IsOwnedByServer)
                {
                    ded = true;
                }
                logger.LogInfo("Spawning mine on dead player");
                deathMineSpawned = true;
                GameObject gameObject = UnityEngine.Object.Instantiate(currentRound.currentLevel.spawnableMapObjects[mine].prefabToSpawn, __instance.placeOfDeath, Quaternion.identity, currentRound.mapPropsContainer.transform);
                gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);


            }

        }


    }
}