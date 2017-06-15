using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DijkstraSquareAnimated : DijkstraSquare {

	public Material currentMaterial = null;
	public Material visitedMaterial = null;
	public float delay = 0.2f;

	void Start () {
		if (sceneObject != null) {

			// create a x * y matrix of nodes (and scene objects)
			matrix = CreateGrid(sceneObject, x, y, gap);

			// create a graph and put random edges inside
			g = new Graph();
			CreateLabyrinth(g, matrix, edgeProbability);

			// ask dijkstra to setup the problem
			DijkstraStepSolver.Init(g, matrix[0, 0], matrix [x - 1, y - 1]);

			// put the solver in a coroutine
			StartCoroutine(AnimateSolution(delay));
		}
	}

	private IEnumerator AnimateSolution(float pause) {
		while (DijkstraStepSolver.Step()) {
			OutlineSet(DijkstraStepSolver.visited, visitedMaterial);
			OutlineNode(DijkstraStepSolver.current, currentMaterial);
			yield return new WaitForSeconds(pause);
		}
		Edge[] path = DijkstraStepSolver.solution;
		// check if there is a solution
		if (path.Length == 0) {
			Debug.Log ("No solution");
		} else {
			// if yes, outline it
			OutlinePath(path, trackMaterial);
		}
	}

	protected void OutlineNode(Node n, Material m) {
		if (n != null) 
			n.sceneObject.GetComponent<MeshRenderer>().material = m;
	}

	protected void OutlineSet(List<Node> set, Material m) {
		if (m == null) return;
		foreach (Node n in set) {
			n.sceneObject.GetComponent<MeshRenderer>().material = m;
		}
	}
}
