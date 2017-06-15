using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(Wait(2));
    }

    private IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        DestroyGameObject();
    }

    private void DestroyGameObject()
    {
        // Destroy this gameobject, this can be called from an Animation Event.
        Destroy(gameObject);
    }
}
