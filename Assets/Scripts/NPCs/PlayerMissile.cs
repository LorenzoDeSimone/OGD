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
    private int status;
    public static int clockwise = 0, counterclockwise = 1, inAir = 2;

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
        else if (myGround.collider != null && status == inAir)
        {
            myDirection.counterClockwise = myDirection.clockwise = myDirection.jump = false;
            Vector2 counterClockWiseDirection = new Vector3(-myGround.normal.y, myGround.normal.x);
            Vector2 clockwiseDirection = new Vector3(myGround.normal.y, -myGround.normal.x);

            if (Vector2.Dot(transform.right, counterClockWiseDirection) > Vector2.Dot(transform.right, clockwiseDirection))
            {
                status = counterclockwise;
                myDirection.counterClockwise = true;
            }
            else
            {
                status = clockwise;
                myDirection.clockwise = true;
            }
        }

        if (myGround.collider != null)
        {
            myDirection.clockwise = myDirection.counterClockwise = myDirection.jump = false;
            if (status == clockwise)
                myDirection.clockwise=true;
            else if(status == counterclockwise)
                myDirection.counterClockwise = true;

            transform.right = myMovable.Move(myDirection);
        }
    }

    public void SetStatus(int status)
    {
        this.status = status;
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
