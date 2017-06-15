using System;
using System.Collections;
using System.Collections.Generic;

public delegate float HeuristicFunction(Node from, Node to);

public class AStarSolver {

	public static bool immediateStop = false;
	
	// two set of nodes (1)

	public static List<Node> visited;
	public static List<Node> unvisited;

	// data structures to extend nodes (2)

	protected struct NodeExtension {
		public float distance;
		public float estimate;
		public Edge predecessor;
	}

	protected static Dictionary<Node, NodeExtension> status;

	public static Edge[] Solve(Graph g, Node start, Node goal, HeuristicFunction heuristic) {

		// setup sets (1)
		visited = new List<Node>();
		unvisited = new List<Node> (g.getNodes ());

		// set all node tentative distance (2)
		status = new Dictionary<Node, NodeExtension> ();
		foreach (Node n in unvisited) {
			NodeExtension ne = new NodeExtension();
			ne.distance = ( n == start ? 0f : float.MaxValue ); // infinite
			ne.estimate = ( n == start ? heuristic(start, goal) : float.MaxValue );
			status [n] = ne;
		}

		// iterate goal is reached with optimal path (6)
		while (!CheckSearchComplete(goal, unvisited)) {
			// select net current node (3)
			Node current = GetNextNode();

			if (current == null) break; // graph is partitioned

			// assign weight and predecessor to all neighbors (4)
			foreach (Edge e in g.getConnections(current)) {
				if (status[current].distance + e.weight < status[e.to].distance) {
					NodeExtension ne = new NodeExtension();
					ne.distance = status[current].distance + e.weight;
					ne.estimate = ne.distance + heuristic(current, goal);
					ne.predecessor = e;
					status[e.to] = ne;
					// unlike Dijkstra's, we can discover better paths here
					if (visited.Contains(e.to)) {
						unvisited.Add(e.to);
						visited.Remove(e.to);
					}
				}
			}
			// mark current node as visited (5)
			visited.Add(current);
			unvisited.Remove(current);
		}

		if (status [goal].distance == float.MaxValue) return new Edge[0]; // goal is unreachable

		// walk back and build the shortest path (7)
		List<Edge> result = new List<Edge> ();
		Node walker = goal;

		while (walker != start) {
			result.Add(status[walker].predecessor);
			walker = status[walker].predecessor.from;
		}

		return result.ToArray();
	}

	// iterate on the unvisited set and get the lowest weight
	protected static Node GetNextNode() {
		Node candidate = null;
		float cDistance = float.MaxValue;
		foreach (Node n in unvisited) {
			// here it comes why we have three sets in the book
			if (status[n].distance != float.MaxValue) {
				if (candidate == null || cDistance > status[n].estimate) {
					candidate = n;
					cDistance = status[n].estimate;
				}
			}
		}
		return candidate;
	}

	// chek if the goal has been reached in an optimal way
	protected static bool CheckSearchComplete(Node goal, List<Node> nl) {
		// check if we reached the goal
		if (status [goal].distance == float.MaxValue) return false;
		// check if the first hit is ok 
		if (immediateStop) return true;
		// check if all nodes in list have loger or same paths 
		foreach (Node n in nl) {
			if (status[n].distance < status[goal].distance) return false;
		}
		return true;
	}

}
















