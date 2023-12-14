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

        static IEnumerator DelayedExplosion(Vector3 position, bool effect, float killrange, float damagerange, float delay)
        {
            yield return new WaitForSeconds(delay);
            Landmine.SpawnExplosion(position, effect, killrange, damagerange);
        }

        [HarmonyPatch(typeof(NoisemakerProp), "ItemActivate")]
        [HarmonyPostfix]
        static void NoiseMakerPropItemActivatePatch(ref PlayerControllerB ___playerHeldBy, ref NoisemakerProp __instance)
        {
            Debug.Log("NoiseMakerPropItemActivatePacth ACTIVATED BRUHHHHH");

            NetworkBehaviour baseplayer = (NetworkBehaviour)___playerHeldBy;

            if (((baseplayer.IsOwner && ___playerHeldBy.isPlayerControlled && (!baseplayer.IsServer || ___playerHeldBy.isHostPlayerObject)) || ___playerHeldBy.isTestingPlayer))
            {
                __instance.StartCoroutine(DelayedExplosion(baseplayer.transform.position, true, 10f, 1f, 0.5f));
            }
            /*else
            {
                Landmine.SpawnExplosion(baseplayer.transform.position, true, 0f, 0f);
            }*/
            


        }
    }
}
