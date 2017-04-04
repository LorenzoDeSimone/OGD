using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarStepSolver : AStarSolver {

	public static Edge[] solution;
	public static Node current;
	
	static Graph graph;
	static Node startNode;
	static Node goalNode;
	static HeuristicFunction estimator;

    public static void Init(Graph g, Node start, Node goal, HeuristicFunction heuristic) {
		
		graph = g;
		startNode = start;
		goalNode = goal;
		estimator = heuristic;
		
		// setup sets (1)
        closedSet = new List<Node>();//Initializing empty closedSet
        openSet = new List<Node>();//Initializing empy openSet
        estimatedDistances = new Dictionary<Node, float>();//estimated distances are valid only for nodes in open set

        distances = new Dictionary<Node, float>();//Initializing distances to Infinity, 0 for start node
        predecessors = new Dictionary<Node, Node>();//Initializing empty predecesor chains

        foreach (Node n in g.getNodes())
        {
            if (n == start)
                distances.Add(n, 0);
            else
                distances.Add(n, float.MaxValue);
        }

        //Add startNode to openSet and sets its estimated values using the heuristic
        openSet.Add(startNode);
        startNode.sceneObject.GetComponent<MeshRenderer>().material = AStarSquareAnimated.greenMat;

        estimatedDistances.Add(startNode, heuristic(startNode,goal));
	}

    private static Node GetCurrentNode()
    {
        Node bestEstimatedNode = null;

        foreach (Node n in openSet)
        {
            if(bestEstimatedNode==null || estimatedDistances[n] < estimatedDistances[bestEstimatedNode])
                bestEstimatedNode = n;
        }
        return bestEstimatedNode;
    }

    protected static float MyEuclideanEstimator(Node from, Node to)
    {
        return Vector3.Distance(from.sceneObject.transform.position, to.sceneObject.transform.position);
    }

    public static bool Step()
    {
        if (openSet.Count > 0)//if there is still a node in the Open set we must continue
        {
            // select the node with the best estimated distance towards goal
            current = GetCurrentNode();
            openSet.Remove(current);
            current.sceneObject.GetComponent<MeshRenderer>().material = AStarSquareAnimated.blueMat;
            closedSet.Add(current);

            if (current.Equals(goalNode))
            {
                FindAndColorPath();
                return false;
            }

            foreach (Edge e in graph.getConnections(current))
            {
                e.to.sceneObject.GetComponent<MeshRenderer>().material = AStarSquareAnimated.greenMat;

                if (!closedSet.Contains(e.to))
                {
                    float newDistance = distances[current] + e.weight;

                    if (!openSet.Contains(e.to))
                    {
                        predecessors[e.to] = current;
                        distances[e.to] = newDistance;
                        openSet.Add(e.to);
                        estimatedDistances.Add(e.to, newDistance + MyEuclideanEstimator(e.to, goalNode));
                    }
                    else if (newDistance < distances[e.to])//Found a better path to e.to with a smaller distance
                    {
                        predecessors[e.to] = current;
                        distances[e.to] = newDistance;
                        estimatedDistances.Remove(e.to);
                        estimatedDistances.Add(e.to, newDistance + MyEuclideanEstimator(e.to, goalNode));
                    }
                }
            }
            return true;
        }
        else
            Debug.Log("No path found =(");
            return false;
    }

    public static bool FindAndColorPath()
    {
        Node curr = goalNode;
        do
        {
            curr.sceneObject.GetComponent<MeshRenderer>().material = AStarSquareAnimated.redMat;
            curr = predecessors[curr];
            if (curr == null)
            {
                return false;
            }
            else if(curr.Equals(startNode))
                curr.sceneObject.GetComponent<MeshRenderer>().material = AStarSquareAnimated.redMat;

        } while (curr != startNode);
        return true;
    }
}
















