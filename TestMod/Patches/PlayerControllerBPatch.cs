using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace LCTestModChrigi.Patches
{

    
    internal class ChrigiGamePatch
    {

        private static RoundManager currentRound;

        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        static void LoadNewLevelPatch()
        {

            currentRound = RoundManager.Instance;

        }


        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPrefix]
        static void UpdatePatch(ref float ___sprintMeter, ref float ___sprintMultiplier, ref bool ___isSprinting, ref PlayerControllerB __instance)
        {
            ___sprintMeter = 1f;

            if (___isSprinting)
            {
                ___sprintMultiplier = 10f;
            }

        }


        [HarmonyPatch(typeof(FlashlightItem), "ItemActivate")]
        [HarmonyPostfix]
        static void ItemActivatePatch(ref FlashlightItem __instance)
        {
            Debug.Log("ItemActivate ACTIVATED BRUHHHHH");

            PlayerControllerB player = __instance.playerHeldBy;

            NetworkBehaviour baseplayer = (NetworkBehaviour)__instance.playerHeldBy;

            for (int i = 0; i < currentRound.currentLevel.spawnableScrap.Count(); i++)
            {
                GameObject objectInPresent = currentRound.currentLevel.spawnableScrap[i].spawnableItem.spawnPrefab;
                GameObject gameObject = UnityEngine.Object.Instantiate(objectInPresent, baseplayer.transform.position, Quaternion.identity, currentRound.spawnedScrapContainer);
                GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
                component.startFallingPosition = baseplayer.transform.position;
                component.targetFloorPosition = component.GetItemFloorPosition(baseplayer.transform.position);
                component.SetScrapValue(1000);
                component.NetworkObject.Spawn();
            }

        }

    }


}