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
        private static bool kys = false;
        private static int mine = 0;

        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        static void LoadNewLevelPatch()
        {
            server = false;
            kys = false;

            currentRound = RoundManager.Instance;
            if (currentRound.IsServer)
            {
                server = true;
            }
            ded = false;

        }

        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingNewLevelClientRpc")]
        [HarmonyPostfix]
        static void FinishGeneratingNewLevelClientRpcPatch()
        {
            logger.LogInfo("Level Loaded, any new mine spawned will blow up instantly");
            kys = true;
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

            if (kys)
            {
                logger.LogInfo("Forcing mine explosion");
                __instance.ExplodeMineServerRpc();
                logger.LogInfo("Mine forcefully activated");
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB))]
        [HarmonyPatch("KillPlayerServerRpc")]
        [HarmonyPostfix]
        static void PlayerControllerBPatch(PlayerControllerB __instance)
        {
            NetworkBehaviour baseplayer = (NetworkBehaviour)__instance;
            if (server && __instance.isPlayerDead && (!baseplayer.IsOwnedByServer || !ded) && __instance.causeOfDeath != CauseOfDeath.Blast && __instance.causeOfDeath != CauseOfDeath.Unknown) // && __instance.causeOfDeath != CauseOfDeath.Suffocation  && __instance.causeOfDeath != CauseOfDeath.Strangulation)
            {
                if (baseplayer.IsOwnedByServer)
                {
                    ded = true;
                }
                logger.LogInfo("Spawning mine on dead player");
                GameObject gameObject = UnityEngine.Object.Instantiate(currentRound.currentLevel.spawnableMapObjects[mine].prefabToSpawn, __instance.placeOfDeath, Quaternion.identity, currentRound.mapPropsContainer.transform);
                gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
                

            }

        }


    }
}
