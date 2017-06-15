using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PatrollerBot : NetworkBehaviour
{
    private Movable movement;
    Movable.CharacterInput input;
    AStarStepSolver currentAStarStepSolver;
    bool firstTime = true;
    public GameObject startGO, endGO;

    void Start()
    {
        /*movement = GetComponent<Movable>();
        input = new Movable.CharacterInput();
        input.jump = false;
        input.counterClockwise = false;
        input.clockwise = true;*/
    }

    private Node GetNodeFromGameObject(GameObject go)
    {
        foreach(Node n in PathFindingManager.GetGraph().GetNodes())
        {
            if (n.sceneObject.Equals(go))
                return n;
        }
        return null;
    }

    void Update()
    {
        if (firstTime)
        {
            HeuristicFunction g;
            Graph graph = PathFindingManager.GetGraph();

            int i = Random.Range(0, graph.getNodesLength());
            Node start = GetNodeFromGameObject(startGO);
            Node end = GetNodeFromGameObject(endGO);

            currentAStarStepSolver = new AStarStepSolver(start, end);
            //movement.Move(input);
            StartCoroutine(CalculatePath());
            firstTime = false;
        }
    }

    private void DrawPath(Edge[] path)
    {
        foreach(Edge currEdge in path)
            Debug.DrawLine(currEdge.from.sceneObject.transform.position, currEdge.to.sceneObject.transform.position, Color.red);
    }

    private void ColorAll(List<Node> nodeList, Color color)
    {
        foreach (Node n in nodeList)
            n.sceneObject.GetComponent<SpriteRenderer>().color = color;
    }

    private IEnumerator CalculatePath()
    {

        foreach (Node n in currentAStarStepSolver.unvisited)
            n.sceneObject.GetComponent<SpriteRenderer>().color = Color.magenta;
        foreach (Node n in currentAStarStepSolver.visited)
            n.sceneObject.GetComponent<SpriteRenderer>().color = Color.red;

        Debug.LogError("w");

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
            yield return new WaitForSeconds(2f);
        }

        Edge[] path = currentAStarStepSolver.solution;

        // check if there is a solution
        if (path.Length == 0)
            Debug.LogError("No solution");
        else
        {
            DrawPath(path);
            Debug.LogError("Path!");
        }
    }
}
