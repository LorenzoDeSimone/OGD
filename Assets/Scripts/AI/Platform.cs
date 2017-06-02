using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;

public class Platform : MonoBehaviour
{
    public float mass = 50;
    public List<GameObject> adjacencies;
    public int maxDistanceAdjacencies = 10;

    public void connectPlatform()
    {
        adjacencies = new List<GameObject>();
        List<GameObject> platforms = new List<GameObject>();
        float distance;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Platform"))
            platforms.Add(go);
        for (int i = 0; i < platforms.Count - 1; i++)
            for (int j = i + 1; j < platforms.Count; j++)
            {
                distance = platforms[i].GetComponent<Collider2D>().Distance(platforms[j].GetComponent<Collider2D>()).distance;
                if (distance < maxDistanceAdjacencies)
                {
                    Debug.DrawRay(platforms[i].transform.position, platforms[j].transform.position - platforms[i].transform.position, Color.green, maxDistanceAdjacencies);
                    platforms[i].GetComponent<Platform>().adjacencies.Add(platforms[j]);
                    platforms[j].GetComponent<Platform>().adjacencies.Add(platforms[i]);
                }
            }
    }
}
