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

        private static bool[] ded;
        private static RoundManager currentRound;
        private static PlayerControllerB[] players;
        private static bool server = false;
        private static bool kys = false;

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

            players = currentRound.playersManager.allPlayerScripts;
            ded = new bool[players.Length];

        }

        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingNewLevelClientRpc")]
        [HarmonyPostfix]
        static void FinishGeneratingNewLevelClientRpcPatch()
        {
            logger.LogInfo("Level Loaded, any new mine spawned will blow up instantly");
            kys = true;
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

        [HarmonyPatch(typeof(PlayerControllerB))] // Try KillLocalPlayer.KillPlayer and then spawnExplosion on them
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void PlayerControllerBPatch()
        {

            if (server)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].isPlayerDead && !ded[i] && players[i].causeOfDeath != CauseOfDeath.Blast && players[i].causeOfDeath != CauseOfDeath.Suffocation && players[i].causeOfDeath != CauseOfDeath.Unknown)
                    {
                        ded[i] = true;
                        logger.LogInfo("Spawning mine on dead player");
                        GameObject gameObject = UnityEngine.Object.Instantiate(currentRound.currentLevel.spawnableMapObjects[0].prefabToSpawn, players[i].placeOfDeath, Quaternion.identity, currentRound.mapPropsContainer.transform);
                        gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);

                    }
                }
            }

        }


    }
}
