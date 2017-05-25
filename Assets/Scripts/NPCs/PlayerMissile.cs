using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class PlayerMissile : MonoBehaviour
{

    public float despawnTime = 5f;
    public float minimumAttractionDistance = 5.5f;

    private Movable myMovable;
    private Movable.CharacterInput myDirection;
    private Radar myRadar;

    private void Start()
    {
        myMovable = GetComponent<Movable>();
        myRadar = GetComponentInChildren<Radar>();
        StartCoroutine(DespawnCountdown(despawnTime));
    }

    public void SetDirection(Movable.CharacterInput input)
    {
        myDirection = input;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D myGround = myRadar.GetMyGround();
        if (myGround && Vector2.Distance(transform.position,myGround.point) <= minimumAttractionDistance)
            myMovable.Move(myDirection);
        else//If the missile doesn't have a ground during its starts, it follows a straight line until the radar finds something(or the players shoots in air targeting another planet)
            transform.position = transform.position + transform.right * myMovable.speed * Time.deltaTime;
    }

    IEnumerator<WaitForSeconds> DespawnCountdown(float despawnTime)
    {
        yield return new WaitForSeconds(despawnTime);
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
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
