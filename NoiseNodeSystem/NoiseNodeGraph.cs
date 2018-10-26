using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new NodeGraph", menuName = "NoiseNodes/NodeGraph")]
public class NoiseNodeGraph : ScriptableObject {
    //All is inGraphlocations
    public string GraphName = "";
    public BaseNode outputNode;
    [SerializeField]
    public List<BaseNode> nodes;
    public Vector2 fiftyFiftylocation; //The in-graph location of the center of the window
    public float zoom;

    public NoiseNodeGraph()
    {
        outputNode = new PerlinNode();
        nodes = new List<BaseNode>();
        nodes.Add(new PerlinNode());
        fiftyFiftylocation = new Vector2(0, 0);
        zoom = 0.5f;
    }
}
