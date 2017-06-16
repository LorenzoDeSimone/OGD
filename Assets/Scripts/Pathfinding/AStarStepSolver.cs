using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Collections;

public class AStarStepSolver : AStarSolver
{

    public Edge[] solution;
    public Node current;

    private Graph graph;
    Node startNode;
    Node goalNode;
    HeuristicFunction estimator;

    public AStarStepSolver(Node start, Node goal)
    {
        graph = PathFindingManager.GetGraph();
        startNode = start;
        goalNode = goal;
        estimator = EuclideanEstimator;

        // setup sets (1)
        visited = new List<Node>();
        unvisited = new List<Node>(graph.GetNodes());

        // set all node tentative distance (2)
        status = new Dictionary<Node, NodeExtension>();
        foreach (Node n in unvisited)
        {
            NodeExtension ne = new NodeExtension();
            ne.distance = (n == start ? 0f : float.MaxValue); // infinite
            ne.estimate = (n == start ? estimator(start, goal) : float.MaxValue);
            status[n] = ne;
        }
        solution = null;
    }

    public bool Step()
    {
        // if we are not done yet
        if (!CheckSearchComplete(goalNode, unvisited))
        {
            //Debug.LogError("AA");
            // select net current node (3)
            current = GetNextNode();

            // if graph is not partitioned
            if (current != null)
            {
                // assign weight and predecessor to all neighbors (4)
                foreach (Edge e in graph.getConnections(current))
                {
                    if (status[current].distance + e.weight < status[e.to].distance)
                    {
                        NodeExtension ne = new NodeExtension();
                        ne.distance = status[current].distance + e.weight;
                        ne.estimate = ne.distance + estimator(current, goalNode);
                        ne.predecessor = e;
                        status[e.to] = ne;
                        // unlike Dijkstra's, we can discover better paths here
                        if (visited.Contains(e.to))
                        {
                            unvisited.Add(e.to);
                            visited.Remove(e.to);
                        }
                    }
                }

                // mark current node as visited (5)
                visited.Add(current);
                unvisited.Remove(current);
                return true;
            }
        }

        if (status[goalNode].distance == float.MaxValue)
        {
            // goal is unreachable
            solution = new Edge[0];
        }
        else
        {
            // walk back and build the shortest path (7)
            List<Edge> result = new List<Edge>();
            Node walker = goalNode;

            while (walker != startNode)
            {
                result.Add(status[walker].predecessor);
                walker = status[walker].predecessor.from;
            }
            solution = result.ToArray();
        }
        return false;
    }

    protected static float EuclideanEstimator(Node from, Node to)
    {
        return Vector3.Distance(from.sceneObject.transform.position, to.sceneObject.transform.position);
    }
}
















