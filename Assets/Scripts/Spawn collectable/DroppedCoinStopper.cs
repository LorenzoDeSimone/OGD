using UnityEngine;

public class DroppedCoinStopper : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.CompareTag("Platform"))
        {
            transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
