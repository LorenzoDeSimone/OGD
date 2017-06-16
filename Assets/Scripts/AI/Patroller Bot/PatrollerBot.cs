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

    void Start()
    {
        myMovable = GetComponent<Movable>();
        myRadar = GetComponentInChildren<Radar>();
        input = new Movable.CharacterInput();
        input.counterClockwise = input.clockwise = input.jump = false;
        planetToReach = null;
        if (Random.Range(0, 2) == 0)
            input.clockwise = true;
        else
            input.counterClockwise = true;
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

    void Update()
    {
        input.jump = false;

        if (planetToReach == null && myMovable.IsGrounded())
        {
            Graph graph = PathFindingManager.GetGraph();
            Radar targetPlayerRadar = PlayerDataHolder.GetLocalPlayer().GetComponentInChildren<Radar>();

            Node start = GetNodeFromGameObject(myRadar.GetMyGround().collider.gameObject);
            Node end = GetNodeFromGameObject(targetPlayerRadar.GetMyGround().collider.gameObject);

            if (!start.sceneObject.Equals(end.sceneObject))
            {
                currentAStarStepSolver = new AStarStepSolver(start, end);
                planetToReach = null;
                StartCoroutine(CalculatePath());
            }
        }
        else if (planetToReach != null && myMovable.IsGrounded())
        {
            RaycastHit2D planetToReachCheck = Physics2D.Raycast(transform.position, transform.up,
                                                                Mathf.Infinity,
                                                                LayerMask.GetMask("Walkable"));

            if (planetToReachCheck.collider != null && planetToReachCheck.collider.gameObject.Equals(planetToReach))
            {
                input.jump = true;
                planetToReach = null;
            }
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

        planetToReach = path[0].to.sceneObject;
    }
}
