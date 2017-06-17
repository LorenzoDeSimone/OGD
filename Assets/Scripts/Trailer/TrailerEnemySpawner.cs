using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerEnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public float speed;
    private bool first;

    void Start()
    {
        first = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerMissile>() && !first)
        {
            enemy.SetActive(true);
            first = true;
        }
    }
}
