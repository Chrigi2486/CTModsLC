using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Audio;
using Unity.Netcode;
using System.Collections;
using System.Security.Cryptography;
using System.ComponentModel;
using DigitalRuby.ThunderAndLightning;

namespace KarmaForBeingAnnoying.Patches
{
    internal class NoisemakerPropPatch
    {

        private static ManualLogSource logger = KarmaForBeingAnnoyingModBase.mls;
        private static bool spawnmine = KarmaForBeingAnnoyingModBase.SpawnmineSetting.Value;
        private static List<NoisemakerProp> noisemakers = new List<NoisemakerProp>();
        private static List<GrabbableObject> remotes = new List<GrabbableObject>();
        private static bool ded;
        private static RoundManager currentRound;
        private static bool server = false;
        private static bool kys = false;
        private static int mine = 0;

        static IEnumerator DelayedExplosion(Vector3 position, bool effect, float killrange, float damagerange, float delay)
        {
            logger.LogInfo("Trying to explode");
            yield return new WaitForSeconds(delay);
            //Landmine.SpawnExplosion(position, effect, killrange, damagerange); //outdated, have to update
            Landmine.SpawnExplosion(position, effect, killrange, damagerange, 50, 0f, (GameObject)null, false);
            logger.LogInfo("Boom");
        }

        //[HarmonyPatch(typeof(NoisemakerProp), "ItemActivate")]
        //[HarmonyPostfix]
        //static void NoiseMakerPropItemActivatePatch(ref PlayerControllerB ___playerHeldBy, ref NoisemakerProp __instance)
        //{
        //    logger.LogInfo("NoiseMakerPropItemActivatePatch ACTIVATED BRUHHHHH: " + __instance.name);
        //    NetworkBehaviour baseplayer = (NetworkBehaviour)___playerHeldBy;
        //    Vector3 itemposition = __instance.transform.position;
        //
        //    if(((KarmaForBeingAnnoyingModBase.AnnoyingItemSetting.Value && baseplayer.IsOwner && ___playerHeldBy.isPlayerControlled && (!baseplayer.IsServer || ___playerHeldBy.isHostPlayerObject)) || ___playerHeldBy.isTestingPlayer))
        //    {
        //        float probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilitySetting.Value;
        //        string itemname = __instance.name.Replace("(Clone)","").ToLower();
        //        switch (itemname)
        //        {
        //            case "airhorn":
        //                probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilityAirhornSetting.Value;
        //                break;
        //            case "clownhorn":
        //                probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilityClownhornSetting.Value;
        //                break;
        //            case "cashregisteritem":
        //                probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilityCashRegisterSetting.Value;
        //                break;
        //            case "hairdryer":
        //                probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilityHairDryerSetting.Value;
        //                break;
        //            default:
        //                break;
        //        }
        //        if (UnityEngine.Random.value < probabilityvar)
        //        {
        //            if (server && spawnmine)
        //            {
        //                GameObject gameObject = UnityEngine.Object.Instantiate(currentRound.currentLevel.spawnableMapObjects[mine].prefabToSpawn, ___playerHeldBy.transform.position, Quaternion.identity, currentRound.mapPropsContainer.transform);
        //                gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
        //                logger.LogInfo("Mine Spawned, Karma!");
        //                return;
        //            } 
        //            else if (server)
        //            {
        //                logger.LogInfo("Server, but no mine");
        //                ___playerHeldBy.KillPlayer(Vector3.up,true,CauseOfDeath.Unknown,0);
        //            }
        //            __instance.StartCoroutine(DelayedExplosion(baseplayer.transform.position, true, KarmaForBeingAnnoyingModBase.KillRangeSetting.Value, KarmaForBeingAnnoyingModBase.DamageRangeSetting.Value, KarmaForBeingAnnoyingModBase.DelaySetting.Value));
        //            logger.LogInfo("Karma");
        //        }
        //        
        //    }
        //
        //}

        //[HarmonyPatch(typeof(RemoteProp), "ItemActivate")]
        //[HarmonyPostfix]
        //static void RemotePropPatch(ref PlayerControllerB ___playerHeldBy, ref RemoteProp __instance)
        //{
        //    logger.LogInfo("RemotePropPatch ACTIVATED BRUHHHHH");
        //
        //    NetworkBehaviour baseplayer = (NetworkBehaviour)__instance;
        //
        //    if (((KarmaForBeingAnnoyingModBase.RemoteSetting.Value && baseplayer.IsOwner && ___playerHeldBy.isPlayerControlled && (!baseplayer.IsServer || ___playerHeldBy.isHostPlayerObject)) || ___playerHeldBy.isTestingPlayer) && UnityEngine.Random.value < KarmaForBeingAnnoyingModBase.ProbabilityRemoteSetting.Value)
        //    {
        //        if(server && spawnmine)
        //        {
        //            GameObject gameObject = UnityEngine.Object.Instantiate(currentRound.currentLevel.spawnableMapObjects[mine].prefabToSpawn, ___playerHeldBy.transform.position, Quaternion.identity, currentRound.mapPropsContainer.transform);
        //            gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
        //        }
        //        else if (server)
        //        {
        //            logger.LogInfo("Server, but no mine");
        //            ___playerHeldBy.KillPlayer(Vector3.up, true, CauseOfDeath.Unknown, 0);
        //        }
        //        __instance.StartCoroutine(DelayedExplosion(baseplayer.transform.position, true, KarmaForBeingAnnoyingModBase.KillRangeSetting.Value, KarmaForBeingAnnoyingModBase.DamageRangeSetting.Value, KarmaForBeingAnnoyingModBase.DelaySetting.Value));
        //        logger.LogInfo("Karma");
        //    }
        //}

        [HarmonyPatch(typeof(NoisemakerProp), "Start")]
        [HarmonyPostfix]
        static void NoiseMakerReference(ref NoisemakerProp __instance)
        {
            noisemakers.Add(__instance);
            logger.LogInfo("NoiseMakerReference found: " + __instance.name);

        }

        [HarmonyPatch(typeof(GrabbableObject), "Start")]
        [HarmonyPostfix]
        static void RemoteReference(ref GrabbableObject __instance)
        {
            if (__instance.name.Contains("Remote"))
            remotes.Add(__instance);
            logger.LogInfo("RemoteReference found: " + __instance.name);

        }

        [HarmonyPatch(typeof(RoundManager), "PlayAudibleNoise")]
        [HarmonyPostfix]
        static void SoundManagerPatch(ref Vector3 noisePosition, ref int noiseID)
        {
            logger.LogInfo("Played Sound " + noiseID +" at position:" + noisePosition);
            bool timetodie = false;
            PlayerControllerB player = null;
            if (noisemakers.Count() > 0)
            {
                foreach (NoisemakerProp noisemaker in noisemakers)
                {
                    string itemname = noisemaker.name.Replace("(Clone)", "").ToLower();
                    logger.LogInfo("Noisemaker: " + itemname + " isBeingUsed: " + noisemaker.isBeingUsed + " isBeingHeld: " + noisemaker.isHeld);
                    float probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilitySetting.Value;
                    switch (itemname)
                    {
                        case "airhorn":
                            probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilityAirhornSetting.Value;
                            break;
                        case "clownhorn":
                            probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilityClownhornSetting.Value;
                            break;
                        case "cashregisteritem":
                            probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilityCashRegisterSetting.Value;
                            break;
                        case "hairdryer":
                            probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilityHairDryerSetting.Value;
                            break;
                        default:
                            break;
                    }
                    if (noisemaker.isBeingUsed && noisemaker.isHeld && UnityEngine.Random.value < probabilityvar)
                    {
                        //player = noisemaker.playerHeldBy;
                        float distanceToNoise = float.MaxValue;
                        PlayerControllerB closestPlayer = null;
                        PlayerControllerB[] allPlayerScripts = StartOfRound.Instance.allPlayerScripts;
                        foreach (PlayerControllerB testedPlayer in allPlayerScripts)
                        {
                            distanceToNoise = (Vector3.Distance(testedPlayer.transform.position, noisePosition) < distanceToNoise ? Vector3.Distance(testedPlayer.transform.position, noisePosition) : distanceToNoise);
                            closestPlayer = testedPlayer;
                        }
                        player = closestPlayer;
                        timetodie = true;
                        logger.LogInfo("Noisemaker: " + noisemaker.name + " Used by Player: " + player.playerUsername + " , adding some Karma :)");
                    }
                }
            }
            

            else if (remotes.Count() > 0)
            {
                foreach (GrabbableObject remote in remotes)
                {
                    logger.LogInfo("Remote: " + remote.name + " isBeingUsed: " + remote.isBeingUsed + " isBeingHeld: " + remote.isHeld);
                    if (remote.isBeingUsed && remote.isHeld && UnityEngine.Random.value < KarmaForBeingAnnoyingModBase.ProbabilityRemoteSetting.Value)
                    {
                        float distanceToNoise = float.MaxValue;
                        PlayerControllerB closestPlayer = null;
                        PlayerControllerB[] allPlayerScripts = StartOfRound.Instance.allPlayerScripts;
                        foreach (PlayerControllerB testedPlayer in allPlayerScripts)
                        {
                            distanceToNoise = (Vector3.Distance(testedPlayer.transform.position, noisePosition) < distanceToNoise ? Vector3.Distance(testedPlayer.transform.position, noisePosition) : distanceToNoise);
                            closestPlayer = testedPlayer;
                        }
                        player = closestPlayer;
                        timetodie = true;
                        logger.LogInfo("Remote used by Player: " + player.playerUsername + " , adding some Karma :)");
                    }
                }
            }
            else
            {
                logger.LogInfo("Noisemaker or Remote not found");
                return;
            }

            
            if (timetodie && player != null)
            {
                if (server && spawnmine)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate(currentRound.currentLevel.spawnableMapObjects[mine].prefabToSpawn, player.transform.position, Quaternion.identity, currentRound.mapPropsContainer.transform);
                    gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
                    logger.LogInfo("Mine Spawned, Karma!");
                    return;
                }
                else
                {
                    player.KillPlayer(Vector3.up * 124f, true, CauseOfDeath.Unknown, 0);
                    //player.KillPlayerServerRpc(getPlayerID, true, Vector3.up * 124f, 0, 0, Vector3.Zero); //figure out how to call this
                }
            }
        }
        

        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        static void LoadNewLevelPatch()
        {
            currentRound = RoundManager.Instance;
            if (spawnmine)
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
            
            else if (currentRound.IsServer)
            {
                server = true;
            }

        }

        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingNewLevelClientRpc")]
        [HarmonyPostfix]
        static void FinishGeneratingNewLevelClientRpcPatch()
        {
            if (spawnmine)
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
            
        }

        [HarmonyPatch(typeof(Landmine), "Start")]
        [HarmonyPrefix]
        static void LandminePatch(ref Landmine __instance)
        {
            logger.LogInfo("Landmine Spawned");

            if (kys && spawnmine)
            {
                logger.LogInfo("Forcing mine explosion");
                __instance.ExplodeMineServerRpc();
                logger.LogInfo("Mine forcefully activated");
            }
        }

    }
}
