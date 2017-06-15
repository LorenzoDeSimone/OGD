using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AStarSquareAnimated : AStarSquare {

	public Material currentMaterial = null;
	public float delay = 0.2f;

	void Start () {
		if (sceneObject != null) {

			// create a x * y matrix of nodes (and scene objects)
			matrix = CreateGrid(sceneObject, x, y, gap);
			
			// create a graph and put random edges inside
			g = new Graph();
			CreateLabyrinth(g, matrix, edgeProbability);
			
			// ask A* to setup the problem
			AStarStepSolver.immediateStop = stopAtFirstHit;
			AStarStepSolver.Init(g, matrix[0, 0], matrix [x - 1, y - 1], EuclideanEstimator);

			// put the solver in a coroutine
			StartCoroutine(AnimateSolution(delay));
		}
	}

	private IEnumerator AnimateSolution(float pause) {
		while (AStarStepSolver.Step()) {
			OutlineSet(AStarStepSolver.visited, visitedMaterial);
			OutlineNode(AStarStepSolver.current, currentMaterial);
			yield return new WaitForSeconds(pause);
		}
		Edge[] path = AStarStepSolver.solution;
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

	// protected float EuclideanEstimator(Node from, Node to);
	// protected float ManhattanEstimator(Node from, Node to);
	// protected float BisectorEstimator(Node from, Node to);
	// protected float FullBisectorEstimator(Node from, Node to);
}
