using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;
using System;

namespace Assets.Scripts.Spawn_collectable
{
    public class CollectableMissile : Collectable
    {
        protected override void RealUpdate(bool b, int id)
        {
            networkActiveState = b;
            mySprite.enabled = b;
            myCollider.enabled = b;
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                PlayerDataHolder pDH = go.GetComponent<PlayerDataHolder>();
                if (pDH.playerId == id)
                {
                    pDH.CmdAddMissile();
                    break;
                }
            }
        }
    }
}