using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class PlayerMissile : NetworkBehaviour
{ 
    public float despawnTime = 10f;

    private Movable myMovable;
    private Movable.CharacterInput myDirection;
    private Radar myRadar;
    private bool firstHitWithAGravityField = true;
    private bool isDirectionOnGroundClockwise;
    public static int clockwise = 0, counterclockwise = 1;

    private void Start()
    {
        myMovable = GetComponent<Movable>();
        myRadar = GetComponentInChildren<Radar>();
        StartCoroutine(DespawnCountdown(despawnTime));
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D myGround = myRadar.GetMyGround();
        //Debug.LogError("www");   

        if (myGround.collider == null)// if the missile doesn't have a ground during its starts, it follows a straight line until the radar finds something(or the players shoots in air targeting another planet)
            transform.position = transform.position + transform.right * myMovable.speed * Time.deltaTime;
        else if (myGround.collider != null && firstHitWithAGravityField)
        {
            myDirection.counterClockwise = myDirection.clockwise = myDirection.jump = false;
            Vector2 counterClockWiseDirection = new Vector3(-myGround.normal.y, myGround.normal.x);
            Vector2 clockwiseDirection = new Vector3(myGround.normal.y, -myGround.normal.x);

            if (Vector2.Dot(transform.right, counterClockWiseDirection) > Vector2.Dot(transform.right, clockwiseDirection))
                myDirection.counterClockwise = true;
            else
                myDirection.clockwise = true;
            firstHitWithAGravityField = false;
        }

        if(myGround.collider != null && !firstHitWithAGravityField)
            transform.right = myMovable.Move(myDirection);
    }

    public void SetStartDirection(bool isDirectionCounterClockwise)
    {
        myDirection.clockwise = myDirection.counterClockwise = myDirection.jump = false;
        if (isDirectionCounterClockwise)
            myDirection.counterClockwise = true;
        else
            myDirection.clockwise = true;
    }


    public void DestroyMissile()
    {
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
    }

    IEnumerator<WaitForSeconds> DespawnCountdown(float despawnTime)
    {
        yield return new WaitForSeconds(despawnTime);
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
    }
}
