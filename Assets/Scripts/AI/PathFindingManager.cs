using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PathFindingManager : NetworkBehaviour {

    public static int maxDistanceAdjacencies = 7;
    private static Graph graph;

    void Start ()
    {
        graph = new Graph();
        int i = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Platform"))
            graph.AddNode(new Node(i + "", go));
        connectPlatform();
    }

    public static Graph GetGraph()
    {
        return graph;
    }

    private static void connectPlatform()
    {
        float distance;
        for (int i = 0; i < graph.getNodesLength() - 1; i++)
            for (int j = i + 1; j < graph.getNodesLength(); j++)
            {
                distance = graph.GetNodes()[i].sceneObject.GetComponent<Collider2D>().Distance(graph.GetNodes()[j].sceneObject.GetComponent<Collider2D>()).distance;
                if (distance < maxDistanceAdjacencies)
                {
                    //Debug.DrawLine(graph.GetNodes()[i].sceneObject.transform.position, graph.GetNodes()[j].sceneObject.transform.position, Color.green);
                    graph.AddEdge(new Edge(graph.GetNodes()[i], graph.GetNodes()[j], 1));
                    graph.AddEdge(new Edge(graph.GetNodes()[j], graph.GetNodes()[i], 1));
                }
            }
    }
}
