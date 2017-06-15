using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PathFindingManager : NetworkBehaviour {

    public int maxDistanceAdjacencies = 10;
    private Graph graph;

    void Start ()
    {
        graph = new Graph();
        int i = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Platform"))
            graph.AddNode(new Node(i + "", go));
        connectPlatform();
    }

    private void connectPlatform()
    {
        float distance;
        for (int i = 0; i < graph.getNodesLength() - 1; i++)
            for (int j = i + 1; j < graph.getNodesLength(); j++)
            {
                distance = graph.getNodes()[i].sceneObject.GetComponent<Collider2D>().Distance(graph.getNodes()[j].sceneObject.GetComponent<Collider2D>()).distance;
                if (distance < maxDistanceAdjacencies)
                {
                    Debug.DrawLine(graph.getNodes()[i].sceneObject.transform.position, graph.getNodes()[j].sceneObject.transform.position, Color.green);
                    graph.AddEdge(new Edge(graph.getNodes()[i], graph.getNodes()[j], 1));
                }
            }
        //Debug.LogError("ciao");
    }
}
