using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;

public class Platform : MonoBehaviour
{
    public float mass = 50;
    public List<int> players;
    private List<GameObject> platforms;

    void Start()
    {
        platforms = new List<GameObject>();
        players = new List<int>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Platform"))
            platforms.Add( go);
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if(collider.gameObject.tag == "Player" && collider.gameObject.GetComponent<PlayerDataHolder>())
        {
            int id = collider.gameObject.GetComponent<PlayerDataHolder>().GetPlayerNetworkId();
            players.Add(id);
            foreach (GameObject go in platforms)
                if (go != gameObject)
                    go.GetComponent<Platform>().removePlayer(id);
        }
    }

    public void removePlayer(int id)
    {
        players.Remove(id);
    }
}
