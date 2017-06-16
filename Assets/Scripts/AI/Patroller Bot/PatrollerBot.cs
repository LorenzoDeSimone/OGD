using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PatrollerBot : NetworkBehaviour
{
    private Movable myMovable;
    private Radar myRadar;

    Movable.CharacterInput input;
    AStarStepSolver currentAStarStepSolver;
    private GameObject planetToReach;
    private bool isPathfindingCoroutineRunning;

    void Start()
    {
        myMovable = GetComponent<Movable>();
        myRadar = GetComponentInChildren<Radar>();
        input = new Movable.CharacterInput();
        RandomizeDirection();
        planetToReach = myRadar.GetMyGround().collider.gameObject;
        isPathfindingCoroutineRunning = false;
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

        if (!myMovable.IsGrounded())
            return;

        //The agent reached its target planet
        if(GameObject.ReferenceEquals(planetToReach, myRadar.GetMyGround().collider.gameObject))
        {
            if(!isPathfindingCoroutineRunning)
            {
                Graph graph = PathFindingManager.GetGraph();
                Radar targetPlayerRadar = PlayerDataHolder.GetLocalPlayer().GetComponentInChildren<Radar>();
                Node start = GetNodeFromGameObject(myRadar.GetMyGround().collider.gameObject);
                Node end = GetNodeFromGameObject(targetPlayerRadar.GetMyGround().collider.gameObject);
                if (!GameObject.ReferenceEquals(start.sceneObject, end.sceneObject))
                {
                    currentAStarStepSolver = new AStarStepSolver(start, end);
                    isPathfindingCoroutineRunning = true;
                    RandomizeDirection();
                    StartCoroutine(CalculatePath());
                }
            }
        }
        else
        {
            RaycastHit2D planetToReachCheck = Physics2D.Raycast(transform.position, transform.up,
                                                                PathFindingManager.maxDistanceAdjacencies,
                                                                LayerMask.GetMask("Walkable"));

            //Debug.DrawRay(transform.position, transform.up * PathFindingManager.maxDistanceAdjacencies, Color.green);

            if (planetToReachCheck.collider != null &&
                        GameObject.ReferenceEquals(planetToReach, planetToReachCheck.collider.gameObject))
                input.jump = true;
        }

        myMovable.Move(input);
    }

    private void DrawPath(Edge[] path)
    {
        foreach (Edge currEdge in path)
            Debug.DrawLine(currEdge.from.sceneObject.transform.position, currEdge.to.sceneObject.transform.position, Color.red);
    }

    /*private void ColorAll(List<Node> nodeList, Color color)
    {
        foreach (Node n in nodeList)
            n.sceneObject.GetComponent<SpriteRenderer>().color = color;
    }*/

    private IEnumerator CalculatePath()
    {

        //foreach (Node n in currentAStarStepSolver.unvisited)
        //    n.sceneObject.GetComponent<SpriteRenderer>().color = Color.magenta;
        //foreach (Node n in currentAStarStepSolver.visited)
        //    n.sceneObject.GetComponent<SpriteRenderer>().color = Color.red;
        while (currentAStarStepSolver.Step())
        {
            //ColorAll(currentAStarStepSolver.visited, Color.red);
            //currentAStarStepSolver.current.sceneObject.GetComponent<SpriteRenderer>().color = Color.green;

            //foreach (Node n in currentAStarStepSolver.unvisited)
            //    n.sceneObject.GetComponent<SpriteRenderer>().color = Color.magenta;
            /*
            foreach (Node n in currentAStarStepSolver.visited)
                Debug.LogWarning("VISITED: " + n.sceneObject.name);
            */

            //Debug.LogError("CCCC");
            //OutlineSet(currentAStarStepSolver.visited, Color.red);
            //OutlineNode(AStarStepSolver.current, Color.green);
            yield return new WaitForSeconds(0f);
        }

        Edge[] path = currentAStarStepSolver.solution;

        // check if there is a solution
        /*if (path.Length == 0)
            Debug.LogError("No solution");
        else
        {
            DrawPath(path);
            Debug.LogError("Path!");
        }*/
        planetToReach = path[path.Length-1].to.sceneObject;
        isPathfindingCoroutineRunning = false;
    }
}
