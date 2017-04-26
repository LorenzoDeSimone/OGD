using UnityEngine;
using UnityEngine.Networking;

public class Collectable : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            PointManager.instance.addPoint(other.transform, 1);
        }
    }
}
