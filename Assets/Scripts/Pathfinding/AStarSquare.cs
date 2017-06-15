using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AStarSquare : DijkstraSquare {

	public bool stopAtFirstHit = false;
	public Material visitedMaterial = null;

	protected void OutlineSet(List<Node> set, Material m) {
		if (m == null) return;
		foreach (Node n in set) {
			n.sceneObject.GetComponent<MeshRenderer>().material = m;
		}
	}

	protected static float EuclideanEstimator(Node from, Node to) {
		return Vector3.Distance(from.sceneObject.transform.position,to.sceneObject.transform.position);
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
