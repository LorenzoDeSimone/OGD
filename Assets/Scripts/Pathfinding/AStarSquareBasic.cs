using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AStarSquareBasic : DijkstraSquare {

	void Start () {
		if (sceneObject != null) {

			// create a x * y matrix of nodes (and scene objects)
			matrix = CreateGrid(sceneObject, x, y, gap);

			// create a graph and put random edges inside
			g = new Graph();
			CreateLabyrinth(g, matrix, edgeProbability);

			// ask dijkstra to solve the problem
			Edge[] path = AStarSolver.Solve(g, matrix[0, 0], matrix[x - 1, y - 1], EuclideanEstimator);

			// check if there is a solution
			if (path.Length == 0) {
				Debug.Log ("No solution");
			} else {
				// if yes, outline it
				OutlinePath(path, trackMaterial);
			}
		}
	}

	private float EuclideanEstimator(Node from, Node to) {
		return (from.sceneObject.transform.position - to.sceneObject.transform.position).magnitude / gap;
	}
}
