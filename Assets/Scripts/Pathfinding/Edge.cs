
// Transition between two nodes
public class Edge {
	
	public Node from;
	public Node to;
	public float weight;
	
	public Edge(Node from, Node to, float weight = 1f) {
		this.from = from;
		this.to = to;
		this.weight = weight;
	}

}