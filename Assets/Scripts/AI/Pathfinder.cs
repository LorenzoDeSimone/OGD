using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private AStarStepSolver currentAStarStepSolver;
    private GameObject planetToReach;
    private bool isPathfindingCoroutineRunning;
    private Radar myRadar;

    void Start()
    {
        isPathfindingCoroutineRunning = false;
        myRadar = GetComponentInChildren<Radar>();
        planetToReach = myRadar.GetMyGround().collider.gameObject;
    }

    public GameObject GetPlanetToReach()
    {
        return planetToReach;
    }

    public bool IsPathfindingStillCoroutineRunning()
    {
        return isPathfindingCoroutineRunning;
    }

    public void FindPath(Node start, Node end)
    {
        if(!isPathfindingCoroutineRunning)
        {
            currentAStarStepSolver = new AStarStepSolver(start, end);
            isPathfindingCoroutineRunning = true;
            StartCoroutine(CalculatePath());
        }
    }

    public IEnumerator CalculatePath()
    {
        while (currentAStarStepSolver.Step())
        {
            yield return new WaitForSeconds(0.2f);
        }

        Edge[] path = currentAStarStepSolver.solution;
        planetToReach = path[path.Length - 1].to.sceneObject;
        isPathfindingCoroutineRunning = false;
    }
}
