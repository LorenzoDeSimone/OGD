using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DijkstraSquare : MonoBehaviour {

	public int x = 10;
	public int y = 10;
	[Range(0f, 1f)] public float edgeProbability = 0.75f;
	public Color edgeColor = Color.red;
	public float gap = 3f;
	public Material trackMaterial = null;

	// what to put on the scene, not really meaningful
	public GameObject sceneObject;

	protected Node[,] matrix;
	protected Graph g;
	
	void Start () {
		if (sceneObject != null) {

			// create a x * y matrix of nodes (and scene objects)
			matrix = CreateGrid(sceneObject, x, y, gap);

			// create a graph and put random edges inside
			g = new Graph();
			CreateLabyrinth(g, matrix, edgeProbability);

			// ask dijkstra to solve the problem
			Edge[] path = DijkstraSolver.Solve(g, matrix[0, 0], matrix [x - 1, y - 1]);

			// check if there is a solution
			if (path.Length == 0) {
				Debug.Log ("No solution");
			} else {
				// if yes, outline it
				OutlinePath(path, trackMaterial);
			}
		}
	}

	protected virtual Node[,] CreateGrid(GameObject o, int x, int y, float gap) {
		Node[,] matrix = new Node[x,y];
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				matrix[i, j] = new Node("" + i + "," +j, Instantiate(o));
				matrix[i, j].sceneObject.name = o.name;
				matrix[i, j].sceneObject.transform.position = 
					transform.position + 
					Vector3.right * gap * (i - ((x - 1) / 2f)) + 
					Vector3.forward * gap * (j - ((y - 1) / 2f));
				matrix[i, j].sceneObject.transform.rotation = transform.rotation;
			}
		}
		return matrix;
	}
	
	protected virtual void CreateLabyrinth(Graph g, Node[,] crossings, float threshold) {
		for (int i = 0; i < crossings.GetLength(0); i += 1) {
			for (int j = 0; j < crossings.GetLength(1); j += 1) {
				g.AddNode(crossings[i, j]);
				foreach (Edge e in RandomEdges(crossings, i, j, threshold)) {
					g.AddEdge(e);
				}
			}
		}
	}

	protected void OutlinePath(Edge[] path, Material m) {
		if (path.Length == 0) return;
		foreach (Edge e in path) {
			e.from.sceneObject.GetComponent<MeshRenderer>().material = m;
		}
		path[0].to.sceneObject.GetComponent<MeshRenderer>().material = m;
	}

	protected virtual Edge[] RandomEdges(Node[,] matrix, int x, int y, float threshold) {        
		List<Edge> result = new List<Edge>();
        if (x != 0 && Random.Range (0f, 1f) <= threshold)//Left
			result.Add(new Edge (matrix [x, y], matrix[x - 1, y], Distance (matrix [x, y], matrix[x - 1, y])));
		
		if (y != 0 && Random.Range (0f, 1f) <= threshold)//Down
			result.Add(new Edge (matrix [x, y], matrix[x, y - 1], Distance (matrix [x, y], matrix[x, y - 1])));
		
		if (x != (matrix.GetLength (0) - 1) && Random.Range (0f, 1f) <= threshold)//Right
			result.Add(new Edge (matrix [x, y], matrix[x + 1, y], Distance (matrix [x, y], matrix[x + 1, y])));
		
		if (y != (matrix.GetLength (1) - 1) && Random.Range (0f, 1f) <= threshold)//Up
			result.Add(new Edge (matrix [x, y], matrix[x, y + 1], Distance (matrix [x, y], matrix[x, y + 1])));
		
		return result.ToArray ();
	}
	
	protected virtual float Distance(Node from, Node to) {
		return 1;
	}

	void OnDrawGizmos() {
		if (matrix != null) {
			Gizmos.color = edgeColor;
			for (int i = 0; i < x; i += 1) {
				for (int j = 0; j < y; j += 1) {
					foreach (Edge e in g.getConnections(matrix[i, j])) {
						Vector3 from = e.from.sceneObject.transform.position;
						Vector3 to = e.to.sceneObject.transform.position;
						Gizmos.DrawCube(from + ((to - from) * .3f), Vector3.one * .3f);
						Gizmos.DrawLine (from, to); 
					}
				}
			}
		}
	}

}
