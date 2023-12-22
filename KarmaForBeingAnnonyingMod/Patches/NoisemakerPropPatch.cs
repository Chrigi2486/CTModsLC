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
using Unity.Netcode;
using System.Collections;
using System.Security.Cryptography;
using System.ComponentModel;

namespace KarmaForBeingAnnoying.Patches
{
    internal class NoisemakerPropPatch
    {

        private static ManualLogSource logger = KarmaForBeingAnnoyingModBase.mls;
        private static bool spawnmine = KarmaForBeingAnnoyingModBase.SpawnmineSetting.Value;

        private static bool ded;
        private static RoundManager currentRound;
        private static bool server = false;
        private static bool kys = false;
        private static int mine = 0;

        static IEnumerator DelayedExplosion(Vector3 position, bool effect, float killrange, float damagerange, float delay)
        {
            logger.LogInfo("C'ya");
            yield return new WaitForSeconds(delay);
            Landmine.SpawnExplosion(position, effect, killrange, damagerange);
            logger.LogInfo("Boom");
        }

        [HarmonyPatch(typeof(NoisemakerProp), "ItemActivate")]
        [HarmonyPostfix]
        static void NoiseMakerPropItemActivatePatch(ref PlayerControllerB ___playerHeldBy, ref NoisemakerProp __instance)
        {
            logger.LogInfo("NoiseMakerPropItemActivatePacth ACTIVATED BRUHHHHH: " + __instance.name);
            NetworkBehaviour baseplayer = (NetworkBehaviour)___playerHeldBy;
            

            if(((KarmaForBeingAnnoyingModBase.AnnoyingItemSetting.Value && baseplayer.IsOwner && ___playerHeldBy.isPlayerControlled && (!baseplayer.IsServer || ___playerHeldBy.isHostPlayerObject)) || ___playerHeldBy.isTestingPlayer))
            {
                float probabilityvar = KarmaForBeingAnnoyingModBase.ProbabilitySetting.Value;
                string itemname = __instance.name.Replace("(Clone)","").ToLower();
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
                if (UnityEngine.Random.value < probabilityvar)
                {
                    if (server && spawnmine)
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate(currentRound.currentLevel.spawnableMapObjects[mine].prefabToSpawn, ___playerHeldBy.transform.position, Quaternion.identity, currentRound.mapPropsContainer.transform);
                        gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
                        logger.LogInfo("Mine Spawned, Karma!");
                        return;
                    }
                    __instance.StartCoroutine(DelayedExplosion(baseplayer.transform.position, true, KarmaForBeingAnnoyingModBase.KillRangeSetting.Value, KarmaForBeingAnnoyingModBase.DamageRangeSetting.Value, KarmaForBeingAnnoyingModBase.DelaySetting.Value));
                    logger.LogInfo("Karma");
                }
                
            }

        }

        [HarmonyPatch(typeof(RemoteProp), "ItemActivate")]
        [HarmonyPostfix]
        static void RemotePropPatch(ref PlayerControllerB ___playerHeldBy, ref RemoteProp __instance)
        {
            logger.LogInfo("RemotePropPatch ACTIVATED BRUHHHHH");

            NetworkBehaviour baseplayer = (NetworkBehaviour)__instance;

            if (((KarmaForBeingAnnoyingModBase.RemoteSetting.Value && baseplayer.IsOwner && ___playerHeldBy.isPlayerControlled && (!baseplayer.IsServer || ___playerHeldBy.isHostPlayerObject)) || ___playerHeldBy.isTestingPlayer) && UnityEngine.Random.value < KarmaForBeingAnnoyingModBase.ProbabilityRemoteSetting.Value)
            {
                if(server && spawnmine)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate(currentRound.currentLevel.spawnableMapObjects[mine].prefabToSpawn, ___playerHeldBy.transform.position, Quaternion.identity, currentRound.mapPropsContainer.transform);
                    gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
                }
                __instance.StartCoroutine(DelayedExplosion(baseplayer.transform.position, true, KarmaForBeingAnnoyingModBase.KillRangeSetting.Value, KarmaForBeingAnnoyingModBase.DamageRangeSetting.Value, KarmaForBeingAnnoyingModBase.DelaySetting.Value));
                logger.LogInfo("Karma");
            }
        }

        

        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        static void LoadNewLevelPatch()
        {
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
