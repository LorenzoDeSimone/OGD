using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AStarSquareBasic : DijkstraSquare {


	private float EuclideanEstimator(Node from, Node to) {
		return (from.sceneObject.transform.position - to.sceneObject.transform.position).magnitude / gap;
	}
}
