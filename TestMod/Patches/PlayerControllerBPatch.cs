using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using System.Web;
using System.Diagnostics.Eventing.Reader;
using System.Collections;

namespace LCTestModChrigi.Patches
{


    internal class ChrigiGamePatch
    {

        private static RoundManager currentRound;
        private static ManualLogSource logger = TestModBase.mls;
        private static PlayerControllerB gplayer;

        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        static void LoadNewLevelPatch()
        {

            currentRound = RoundManager.Instance;
            HUDManager.Instance.chatTextField.characterLimit = 999;

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
            logger.LogInfo("ItemActivate ACTIVATED BRUHHHHH");

            PlayerControllerB player = __instance.playerHeldBy;

            gplayer = player;

            NetworkBehaviour baseplayer = (NetworkBehaviour)__instance.playerHeldBy;

            /*
            HUDManager.Instance.DisplayTip("Alert!", "CUM ON MY TIDDIES NOW!!", isWarning: true, useSave: false);

            HUDManager.Instance.signalTranslatorAnimator.SetBool("transmitting", value: true);
            HUDManager.Instance.signalTranslatorText.text = "UR MUM GAY";
            */
            /*
            for (int i = 0; i < currentRound.currentLevel.Enemies.Count(); i++)
            {
                logger.LogInfo("Name: " + currentRound.currentLevel.Enemies[i].enemyType.name);
                if (currentRound.currentLevel.Enemies[i].enemyType.name == "Nutcracker")
                {
                    GameObject nutcra = UnityEngine.Object.Instantiate(currentRound.currentLevel.Enemies[i].enemyType.enemyPrefab, baseplayer.transform.position, Quaternion.identity);
                    NutcrackerEnemyAI nutcracomponent = nutcra.GetComponent<NutcrackerEnemyAI>();
                    nutcracomponent.NetworkObject.Spawn();
                    // while (!nutcracomponent) ;
                    // nutcracomponent.KillEnemy();


                }
            }
            */

            /*
            for (int i = 0; i < currentRound.currentLevel.spawnableScrap.Count(); i++)
            {
                GameObject objectInPresent = currentRound.currentLevel.spawnableScrap[i].spawnableItem.spawnPrefab;
                GameObject gameObject = UnityEngine.Object.Instantiate(objectInPresent, baseplayer.transform.position, Quaternion.identity, currentRound.spawnedScrapContainer);
                GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
                logger.LogInfo("Spawned " + component.name + ", " + objectInPresent.name);
                logger.LogInfo("");
                component.startFallingPosition = baseplayer.transform.position;
                component.targetFloorPosition = component.GetItemFloorPosition(baseplayer.transform.position);
                component.SetScrapValue(1000);
                component.NetworkObject.Spawn();
            }
            */
        }

        /*
        [HarmonyPatch(typeof(NutcrackerEnemyAI), "DropGun")]
        [HarmonyPrefix]
        static bool DropGunPatch()
        {
            return false;
        }
        */

        [HarmonyPatch(typeof(ShotgunItem), "ItemActivate")]
        [HarmonyPostfix]
        static void ItemActivateGunPatch(ref ShotgunItem __instance)
        {
            __instance.shellsLoaded = 99;
        }

        /*
        [HarmonyPatch(typeof(NutcrackerEnemyAI), "Start")]
        [HarmonyPostfix]
        static void StartPatch(ref NutcrackerEnemyAI __instance)
        {
            
            logger.LogInfo("Cum on me daddy!");
            /*
            NetworkBehaviour baseplayer = (NetworkBehaviour)gplayer;

            __instance.KillEnemy();

            
            for (int j = 0; j < 3; j++)
            {
                GameObject prefab = __instance.gunPrefab;
                GameObject gameObject = UnityEngine.Object.Instantiate(prefab, baseplayer.transform.position, Quaternion.identity, currentRound.spawnedScrapContainer);
                GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
                component.startFallingPosition = baseplayer.transform.position;
                component.targetFloorPosition = component.GetItemFloorPosition(baseplayer.transform.position);
                component.SetScrapValue(1000);
                component.NetworkObject.Spawn();
                logger.LogInfo("I dropped my hotpocket!");
            }
            
            /*
            __instance.gun.isHeldByEnemy = false;
            __instance.gun.grabbableToEnemies = true;
            __instance.gun.grabbable = true;
            
        }*/

        static void DelayedGunSpawn(Vector3 position, int amount, NutcrackerEnemyAI nutcrack, float delay)
        {
            //yield return new WaitForSeconds(delay);
            logger.LogInfo("Spawning Gun");
            
        }

        [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
        [HarmonyPrefix]
        static void SubmitChat_performed(ref HUDManager __instance)
        {
            string text = __instance.chatTextField.text;
            logger.LogInfo(text);
            if (text.StartsWith("/"))
            {
                string[] segments = (text.Substring(1)).Split(' ');
                __instance.chatTextField.text = "";
                string command = segments[0];
                switch (command)
                {
                    case "spawn":
                        if (segments.Length < 3)
                        {
                            logger.LogWarning("Missing Arguments For Spawn\n'/spawn <type> <name> (amount=<amount>) (state=<state>) (position={random, @me, @<playername>})");
                            return;
                        }
                        string type = segments[1];
                        string toSpawn = segments[2];
                        int amount = 1;
                        string vstate = "alive";
                        Vector3 position = Vector3.zero;
                        string sposition = "random";
                        var args = segments.Skip(3);

                        foreach (string arg in args)
                        {
                            string[] darg = arg.Split('=');
                            switch (darg[0])
                            {
                                case "amount":
                                    amount = int.Parse(darg[1]);
                                    logger.LogInfo($"{amount}");
                                    break;
                                case "state":
                                    vstate = darg[1];
                                    logger.LogInfo(vstate);
                                    break;
                                case "position":
                                    sposition = darg[1];
                                    logger.LogInfo(sposition);
                                    break;
                                default:
                                    break;
                            }
                        }


                        if (sposition.StartsWith("@"))
                        {
                            if (sposition == "@me") position = ((NetworkBehaviour)currentRound.playersManager.localPlayerController).transform.position;
                            else
                            {
                                string pstring = sposition.Substring(1);
                                foreach (PlayerControllerB player in currentRound.playersManager.allPlayerScripts)
                                {
                                    if (player.name == pstring)
                                    {
                                        position = ((NetworkBehaviour)player).transform.position;
                                        break;
                                    }
                                }
                            }
                            
                        }
                        else if (sposition != "random")
                        {
                            logger.LogWarning("Position Invalid, Using Default 'random'");
                            sposition = "random";
                        }

                        switch (type)
                        {
                            case "scrap":
                                int len = currentRound.currentLevel.spawnableScrap.Count();
                                for (int i = 0; i < len; i++)
                                {
                                    Item scrap = currentRound.currentLevel.spawnableScrap[i].spawnableItem;
                                    if (scrap.spawnPrefab.name.ToLower() == toSpawn)
                                    {
                                        GameObject objToSpawn = scrap.spawnPrefab;
                                        bool ra = sposition == "random";
                                        RandomScrapSpawn[] source;
                                        List<RandomScrapSpawn> list4 = null;
                                        if (ra)
                                        {
                                            source = UnityEngine.Object.FindObjectsOfType<RandomScrapSpawn>();
                                            list4 = ((scrap.spawnPositionTypes != null && scrap.spawnPositionTypes.Count != 0) ? source.Where((RandomScrapSpawn x) => scrap.spawnPositionTypes.Contains(x.spawnableItems) && !x.spawnUsed).ToList() : source.ToList());
                                            
                                        }

                                        logger.LogInfo("Spawning " + amount + " " + objToSpawn.name + (amount > 1 ? "s" : ""));
                                        for (int j = 0; j < amount; j++)
                                        {
                                            if (ra)
                                            {
                                                RandomScrapSpawn randomScrapSpawn = list4[currentRound.AnomalyRandom.Next(0, list4.Count)];
                                                position = currentRound.GetRandomNavMeshPositionInRadiusSpherical(randomScrapSpawn.transform.position, randomScrapSpawn.itemSpawnRange, currentRound.navHit) + Vector3.up * scrap.verticalOffset;
                                            }
                                            GameObject gameObject = UnityEngine.Object.Instantiate(objToSpawn, position, Quaternion.identity, currentRound.spawnedScrapContainer);
                                            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
                                            component.startFallingPosition = position;
                                            component.targetFloorPosition = component.GetItemFloorPosition(position);
                                            component.SetScrapValue(1000);
                                            component.NetworkObject.Spawn();
                                        }
                                        break;
                                    }
                                }
                                break;

                            case "special":
                                if (toSpawn == "gun")
                                {
                                    for (int i = 0; i < currentRound.currentLevel.Enemies.Count(); i++)
                                    {
                                        if (currentRound.currentLevel.Enemies[i].enemyType.name == "Nutcracker")
                                        {
                                            GameObject nutcra = UnityEngine.Object.Instantiate(currentRound.currentLevel.Enemies[i].enemyType.enemyPrefab, Vector3.zero, Quaternion.identity);
                                            NutcrackerEnemyAI nutcracomponent = nutcra.GetComponent<NutcrackerEnemyAI>();

                                            logger.LogInfo("Spawning " + amount + " gun" + (amount > 1 ? "s" : ""));

                                            for (int j = 0; j < amount; j++)
                                            {
                                                GameObject gameObject = UnityEngine.Object.Instantiate(nutcracomponent.gunPrefab, position, Quaternion.identity, currentRound.spawnedScrapContainer);
                                                GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
                                                component.startFallingPosition = position;
                                                component.targetFloorPosition = component.GetItemFloorPosition(position);
                                                component.SetScrapValue(1000);
                                                component.NetworkObject.Spawn();
                                            }
                                            break;

                                        }
                                    }
                                }
                                break;

                            default:
                                logger.LogWarning("Type Not Found");
                                return;
                        }
                        break;

                    case "list":
                        if (segments.Length < 2)
                        {
                            logger.LogWarning("Missing Arguments For List\n'/list <type>");
                            return;
                        }
                        type = segments[1];
                        switch (type)
                        {
                            case "scrap":
                                int len = currentRound.currentLevel.spawnableScrap.Count();
                                string output = currentRound.currentLevel.spawnableScrap[0].spawnableItem.spawnPrefab.name;
                                for (int i = 1; i < len; i++)
                                {
                                    output += ", ";
                                    output += currentRound.currentLevel.spawnableScrap[i].spawnableItem.spawnPrefab.name;
                                }
                                HUDManager.Instance.DisplayTip("Spawnable Scrap", output);

                                break;

                            case "enemy":
                                len = currentRound.currentLevel.Enemies.Count();
                                output = currentRound.currentLevel.Enemies[0].enemyType.enemyName;
                                for (int i = 1; i < len; i++)
                                {
                                    output += ", ";
                                    output += currentRound.currentLevel.Enemies[i].enemyType.enemyName;
                                }
                                HUDManager.Instance.DisplayTip("Spawnable Scrap", output);

                                break;

                            default:
                                logger.LogWarning("Type Not Found");
                                break;
                        }

                        break;

                    default:
                        logger.LogWarning("Command Not Found");
                        return;
                }
            }
        }
    }


}