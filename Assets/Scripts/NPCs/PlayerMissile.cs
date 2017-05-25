using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class PlayerMissile : MonoBehaviour
{
    private Movable myMovable;
    private Movable.CharacterInput myDirection;

    private void Start()
    {
        myMovable = GetComponent<Movable>();
    }

    public void SetDirection(Movable.CharacterInput input)
    {
        myDirection = input;
    }

    // Update is called once per frame
    void Update()
    {
        myMovable.Move(myDirection);
    }

    IEnumerator<WaitForSeconds> VanishCountDown(float vanishTime)
    {
        yield return new WaitForSeconds(vanishTime);
        //NetworkServer.UnSpawn(gameObject);
        //Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerDataHolder player = collider.gameObject.GetComponent<PlayerDataHolder>();

        if (player)
        {
            //Debug.LogError("Player Hit! " + target.gameObject.name);
            //NetworkServer.UnSpawn(gameObject);
            //Destroy(gameObject);
        }

    }
}
