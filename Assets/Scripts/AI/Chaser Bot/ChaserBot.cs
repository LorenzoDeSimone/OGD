﻿using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChaserBot : NetworkBehaviour
{
    public float standStillTime = 5f;

    private Movable myMovable;
    private Radar myRadar;
    private Pathfinder myPathfinder;

    private Movable.CharacterInput input;

    public delegate GameObject TargetGetter();
    public delegate void OnHitHandler();
    public delegate void OnCollectableHitHandler(GameObject collectable);

    private TargetGetter MyTargetGetter;
    private OnHitHandler MyOnHit;
    private OnCollectableHitHandler MyOnCollectableHit;

    private bool playerHit;
    //public Dictionary<int, GameObject> PlayersGameObjects;

    public void SetTargetGetter(TargetGetter MyTargetGetter)
    {
        this.MyTargetGetter = MyTargetGetter;
    }

    public void SetOnHitHandler(OnHitHandler MyOnHitHandler)
    {
        this.MyOnHit = MyOnHitHandler;
    }

    public void SetOnCollectableHit(OnCollectableHitHandler MyOnCollectableHit)
    {
        this.MyOnCollectableHit = MyOnCollectableHit;
    }

    void Start()
    {
        if (!isServer)
            return;

        myMovable = GetComponent<Movable>();
        myRadar = GetComponentInChildren<Radar>();
        myPathfinder = GetComponent<Pathfinder>();

        input = new Movable.CharacterInput();
        RandomizeDirection();
        playerHit = false;
    }

    public void SetPlayerHit(bool playerHit)
    {
        this.playerHit = playerHit;
    }

    private Node GetNodeFromGameObject(GameObject go)
    {
        foreach (Node n in PathFindingManager.GetGraph().GetNodes())
        {
            if (n.sceneObject.Equals(go))
                return n;
        }
        return null;
    }

    private void RandomizeDirection()
    {
        input.counterClockwise = input.clockwise = input.jump = false;
        if (Random.Range(0, 2) == 0)
            input.clockwise = true;
        else
            input.counterClockwise = true;
    }

    void Update()
    {
        input.jump = false;

        if (!CanMove())
            return;

        //The agent reached its target planet
        if (GameObject.ReferenceEquals(myPathfinder.GetPlanetToReach(), myRadar.GetMyGround().collider.gameObject))
        {
            if (!myPathfinder.IsPathfindingStillCoroutineRunning())
            {
                Graph graph = PathFindingManager.GetGraph();

                GameObject targetPlayer = MyTargetGetter();
                if (targetPlayer != null)//Edge case: players might not be loaded at start
                {
                    Radar targetPlayerRadar = MyTargetGetter().GetComponentInChildren<Radar>();
                    Node start = GetNodeFromGameObject(myRadar.GetMyGround().collider.gameObject);
                    Node end = GetNodeFromGameObject(targetPlayerRadar.GetMyGround().collider.gameObject);
                    if (!GameObject.ReferenceEquals(start.sceneObject, end.sceneObject))
                    {
                        RandomizeDirection();
                        myPathfinder.FindPath(start, end);
                    }
                }
            }
        }
        else
        {
            RaycastHit2D planetToReachCheck = Physics2D.Raycast(transform.position, transform.up,
                                                                PathFindingManager.maxDistanceAdjacencies,
                                                                LayerMask.GetMask("Walkable"));

            if (planetToReachCheck.collider != null &&
                        GameObject.ReferenceEquals(myPathfinder.GetPlanetToReach(), planetToReachCheck.collider.gameObject))
                input.jump = true;
        }

        myMovable.Move(input);
    }

    private void DrawPath(Edge[] path)
    {
        foreach (Edge currEdge in path)
            Debug.DrawLine(currEdge.from.sceneObject.transform.position, currEdge.to.sceneObject.transform.position, Color.red);
    }

    private bool CanMove()
    {
        return isServer && myMovable.IsGrounded() && !playerHit;
    }

    public void DestroyBot()
    {
        NetworkServer.UnSpawn(gameObject);

        Destroy(gameObject);
    }

    public void OnHit()
    {
        if (MyOnHit != null)
        {
            NpcSpawner.Instance.removeNpc();
            MyOnHit();
        }
    }

    public void OnCollectableHit(GameObject collectable)
    {
        if (MyOnCollectableHit != null)
            MyOnCollectableHit(collectable);
    }
}
