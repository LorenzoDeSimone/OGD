using UnityEngine;

namespace Assets.Scripts.Spawn_collectable
{
    public class DroppedCoinStopper : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.CompareTag("Platform"))
            {
                transform.parent.gameObject.GetComponent<DroppedCoin>().Stop();
            }
        }
    } 
}
