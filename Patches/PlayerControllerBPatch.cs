using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LCTestModChrigi.Patches
{

    
    internal class ChrigiGamePatch
    {

        private static bool ded = false;
        private static RoundManager currentRound;


        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        static void LoadNewLevelPatch()
        {
            Debug.Log("SNATCHED ROUNDMANAGER");
            currentRound = RoundManager.Instance;
        }

        [HarmonyPatch(typeof(PlayerControllerB))]
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void infiniteSprintPatch(ref float ___sprintMeter, ref float ___sprintMultiplier, ref float ___drunkness, ref bool ___isSprinting, ref Vector3 ___oldPlayerPosition)
        {
            //___sprintMeter = 1f;

            if (___isSprinting)
            {
                ___sprintMultiplier = 10f;
                //Landmine.SpawnExplosion(___oldPlayerPosition, true);
            }
            // ___drunkness = 10f;
            ded = false;
        }


        [HarmonyPatch(typeof(PlayerControllerB))]
        [HarmonyPatch("KillPlayer")]
        [HarmonyPostfix]
        static void patchDeath(ref Vector3 ___placeOfDeath)
        {

            if (!ded)
            {
                ded = true;
                Debug.Log("SPAWN CAUSE DED");
                currentRound.SpawnEnemyOnServer(___placeOfDeath, 0f, 2);
            }
        }
        
        
    }


}