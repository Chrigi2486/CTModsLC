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

namespace KarmaForBeingAnnoying.Patches
{
    internal class NoisemakerPropPatch
    {

        private static ManualLogSource logger = KarmaForBeingAnnoyingModBase.mls;

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
            logger.LogInfo("NoiseMakerPropItemActivatePacth ACTIVATED BRUHHHHH");

            NetworkBehaviour baseplayer = (NetworkBehaviour)___playerHeldBy;

            if (((KarmaForBeingAnnoyingModBase.AnnoyingItemSetting.Value && baseplayer.IsOwner && ___playerHeldBy.isPlayerControlled && (!baseplayer.IsServer || ___playerHeldBy.isHostPlayerObject)) || ___playerHeldBy.isTestingPlayer) && UnityEngine.Random.value < KarmaForBeingAnnoyingModBase.ProbabilitySetting.Value)
            {
                __instance.StartCoroutine(DelayedExplosion(baseplayer.transform.position, true, 10f, 1f, 0.5f));
                logger.LogInfo("Karma");
            }
            /*else
            {
                Landmine.SpawnExplosion(baseplayer.transform.position, true, 0f, 0f);
            }*/
            


        }
    }
}
