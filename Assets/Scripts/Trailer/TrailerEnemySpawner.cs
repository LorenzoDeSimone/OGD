using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerEnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public float speed;
    private bool spawned, first;
    private float finalY;

    void Start()
    {
        first = false;
        spawned = false;
        finalY = enemy.transform.position.y + 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawned)
        {
            enemy.transform.position = enemy.transform.position + Vector3.up * Time.deltaTime * speed;
            if (enemy.transform.position.y > finalY)
                spawned = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Assets.Scripts.Player.PlayerDataHolder>() && !first)
        {
            spawned = first = true;
        }
    }
}
