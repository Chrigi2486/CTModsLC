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

            currentRound = RoundManager.Instance;
            if (currentRound.IsServer)
            {
                server = true;
            }

            players = currentRound.playersManager.allPlayerScripts;
            ded = new bool[players.Length];

        }

        [HarmonyPatch(typeof(RoundManager), "LoadNewLevelWait")]
        [HarmonyPostfix]
        static void SpawnMapObjectsPatch()
        {
            kys = true;
        }

        [HarmonyPatch(typeof(Landmine), "Start")]
        [HarmonyPrefix]
        static void LandminePatch(ref Landmine __instance)
        {
            if (kys)
            {
                Debug.Log("Forcing mine explosion");
                __instance.ExplodeMineServerRpc();
                Debug.Log("Mine forcefully activated");
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB))]
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void PlayerControllerBPatch()
        {

            if (server)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].isPlayerDead && !ded[i])
                    {
                        ded[i] = true;
                        Debug.Log("Spawning mine on dead player");
                        GameObject gameObject = UnityEngine.Object.Instantiate(currentRound.currentLevel.spawnableMapObjects[0].prefabToSpawn, players[i].placeOfDeath, Quaternion.identity, currentRound.mapPropsContainer.transform);
                        gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);

                    }
                }
            }

        }


    }
}
