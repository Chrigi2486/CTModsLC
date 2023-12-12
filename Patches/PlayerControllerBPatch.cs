using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private static bool[] ded;
        private static RoundManager currentRound;
        private static PlayerControllerB[] players;
        private static bool server = false;
        private enum Enemy
        {
            Centipede,
            Spider,
            LootBug,
            Braken,
            Thumper,
            Blob,
            CoilHead,
            SporeLizard,
            NutCracker,
        };


        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        static void LoadNewLevelPatch()
        {
            server = false;

            Debug.Log("SNATCHED ROUNDMANAGER");
            currentRound = RoundManager.Instance;
            if (currentRound.IsServer)
            {
                server = true;
                Debug.Log("U ARE SERVER BITCH");
            }

            players = currentRound.playersManager.allPlayerScripts;
            ded = new bool[players.Length];
            
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
            
            if (server)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].isPlayerDead && !ded[i])
                    {
                        Debug.Log("SPAWN CAUSE DED");
                        currentRound.SpawnEnemyOnServer(players[i].placeOfDeath, 0f, (System.Int32)Enemy.NutCracker);
                        ded[i] = true;
                    }
                }
            }

        }
        
        
    }


}