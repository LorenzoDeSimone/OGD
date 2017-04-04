using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AStarSquare : DijkstraSquare {

	public bool stopAtFirstHit = false;
	public Material visitedMaterial = null;

	void Start () {
		if (sceneObject != null) {

			// create a x * y matrix of nodes (and scene objects)
			matrix = CreateGrid(sceneObject, x, y, gap);

			// create a graph and put random edges inside
			g = new Graph();
			CreateLabyrinth(g, matrix, edgeProbability);

			// ask A* to solve the problem
			AStarSolver.immediateStop = stopAtFirstHit;
			//Edge[] path = AStarSolver.Solve(g, matrix[0, 0], matrix[x - 1, y - 1], EuclideanEstimator);

			// Outline visited nodes
			//OutlineSet(AStarSolver.visited, visitedMaterial);

			// check if there is a solution
			//if (path.Length == 0) {
			//	Debug.Log ("No solution");
			//} else {
				// if yes, outline it
			//	OutlinePath(path, trackMaterial);
			//}
		}
	}

	protected void OutlineSet(List<Node> set, Material m) {
		if (m == null) return;
		foreach (Node n in set) {
			n.sceneObject.GetComponent<MeshRenderer>().material = m;
		}
	}

	protected float EuclideanEstimator(Node from, Node to) {
		return (from.sceneObject.transform.position - to.sceneObject.transform.position).magnitude / gap;
	}

	protected float ManhattanEstimator(Node from, Node to) {
		return (
				Mathf.Abs(from.sceneObject.transform.position.x - to.sceneObject.transform.position.x) +
				Mathf.Abs(from.sceneObject.transform.position.y - to.sceneObject.transform.position.y)
			) / gap;
	}

	protected float BisectorEstimator(Node from, Node to) {
		Ray r = new Ray (Vector3.zero, to.sceneObject.transform.position);
		return Vector3.Cross(r.direction, from.sceneObject.transform.position - r.origin).magnitude;
	}

	protected float FullBisectorEstimator(Node from, Node to) {
		Ray r = new Ray (Vector3.zero, to.sceneObject.transform.position);
		Vector3 toBisector = Vector3.Cross (r.direction, from.sceneObject.transform.position - r.origin);
		return toBisector.magnitude + (to.sceneObject.transform.position - ( from.sceneObject.transform.position + toBisector  ) ).magnitude ;
	}
}
