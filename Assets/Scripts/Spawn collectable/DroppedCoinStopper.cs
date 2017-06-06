using System;
using UnityEngine;

namespace Assets.Scripts.Spawn_collectable
{
    public class DroppedCoinStopper : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Platform"))
            {
                transform.parent.GetComponent<DroppedCoin>().Stop();
            }
        }
    } 
}
