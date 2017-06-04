using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class PlayerMissileHitter : MonoBehaviour
{
    public float despawnTime = 10f;
    PlayerMissile myMissile;

    private void Start()
    {
        myMissile = GetComponentInParent<PlayerMissile>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerDataHolder player = collider.gameObject.GetComponent<PlayerDataHolder>();
        PlayerMissile otherPlayerMissile = collider.gameObject.GetComponent<PlayerMissile>();

        if (myMissile.isServer)//Only server can check missiles collisions
        {
            if (player)
            {
                player.OnHit();
                //Debug.LogError("Player Hit! " + target.gameObject.name);
                myMissile.DestroyMissile();
            }
            else if (otherPlayerMissile)
            {
                myMissile.DestroyMissile();
            }
        }
    }
}


/*

    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class PlayerMissile : MonoBehaviour
{ 
    private Movable myMovable;
    private Movable.CharacterInput myDirection;
    private Radar myRadar;
    private bool firstHitWithAGravityField = true;

    private void Start()
    {
        myMovable = GetComponent<Movable>();
        myRadar = GetComponentInChildren<Radar>();
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

        if(myGround.collider!=null)
            myMovable.Move(myDirection);
        //else//If the missile doesn't have a ground during its starts, it follows a straight line until the radar finds something(or the players shoots in air targeting another planet)
        //transform.position = transform.position + transform.right * myMovable.speed * Time.deltaTime;
    }
}



*/
