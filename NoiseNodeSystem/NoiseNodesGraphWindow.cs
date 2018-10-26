using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NoiseNodesGraphWindow : EditorWindow {
    private BaseNode holdingNode = null;
    private Vector2 offset;
    public List<BaseNode> nodeList;
    private NoiseNodeGraph graph;
    private Vector2 inGraphCenter;
    private float zoom = 1;

    [MenuItem("Window/NoiseNode graph editor")]
    static void Init()
    {
        NoiseNodesGraphWindow window = (NoiseNodesGraphWindow)GetWindow(typeof(NoiseNodesGraphWindow));
        window.Show();
    }

    void OnGUI()
    {
        minSize = new Vector2(1000, 600);       

        DrawWindow();

        if (graph != null)
        {
            EditorUtility.SetDirty(graph);
            ConstructNodeList(graph);            

            Repaint();
            EventHandler(Event.current);
            Save();
            EditorUtility.SetDirty(graph);
        }

        DrawWindowAfter();
    }

    private void DrawWindowAfter()
    {
        Rect fieldBackgroundRect = new Rect(position.width - 300, position.height - 100, 300, 100);
        Rect fieldRect = new Rect(position.width - 290, position.height - 90, 280, 20);

        EditorGUI.DrawRect(fieldBackgroundRect, new Color(0.1f, 0.1f, 0.1f, 1));
        graph = (NoiseNodeGraph)EditorGUI.ObjectField(fieldRect, graph, typeof(NoiseNodeGraph), false);
    }

    private void DrawWindow()
    {
        Rect windowRect = new Rect(0, 0, position.width, position.height);        
       
        EditorGUI.DrawRect(windowRect, new Color(0.2f, 0.2f, 0.2f, 1));        

        int amountOfLinesX = 1 + (int) (position.width / (20 / zoom));
        int amountOfLinesY =1 + (int) (position.height / (20 / zoom));

        for (int x = 0; x < amountOfLinesX; x++)
        {
            Rect lineRect = new Rect(x * (20 / zoom), 0, 1, position.height);
            EditorGUI.DrawRect(lineRect, new Color(0.1f, 0.1f, 0.1f, 1));
        }

        for (int y = 0; y < amountOfLinesY; y++)
        {
            Rect lineRect = new Rect(2, y * (20 / zoom), position.width, 1);
            EditorGUI.DrawRect(lineRect, new Color(0.05f, 0.05f, 0.05f, 1));
        }        
    }

    private void drawConnection(BaseNode n)
    {
        if (n.getInputs() != null)
        {
            int y = 25;
            foreach (var i in n.getInputs())
            {
                BaseNode input = null;
                bool found = false;
                foreach (var ni in nodeList)
                {
                    if (i != null)
                    {
                        if (ni.uid == i.uid)
                        {
                            input = ni;
                            found = true;
                        }
                    }
                }

                if (found)
                {
                    Vector3 start = new Vector3(n.xLocation, y + n.yLocation + 6f, 0);
                    Vector3 end = new Vector3(input.xLocation + input.width, i.yLocation + 25 + 6f, 0);
                    Handles.BeginGUI();
                    Handles.color = Color.white;
                    Handles.DrawAAPolyLine(5, new Vector3[] { start, end });
                    Handles.EndGUI();
                }

                y += 18;
            }
        }
    }

    private void drawNode(BaseNode bn, Rect rect)
    {
        drawConnection(bn);

        if (Event.current.type == EventType.Repaint)
        {
            GUIStyle style = new GUIStyle();
            style.border = new RectOffset(10, 10, 10, 10);
            //EditorGUI.DrawRect(rect, bn.nodeColor);
            switch (bn.nodeColor)
            {
                case BaseNode.NodeColor.blue:
                    style.normal.background = (Texture2D)Resources.Load("BlueNode");

                    break;
                case BaseNode.NodeColor.orange:
                    style.normal.background = (Texture2D)Resources.Load("OrangeNode");

                    break;
                default:
                    Debug.Log("color defaulted");
                    break;
            }
            style.Draw(rect, new GUIContent(), 0);
            
            EditorGUI.LabelField(rect, bn.name);

            string[] inputs = bn.getInputNames();

            int y = 25;

            if (inputs != null)
            {
                foreach (var i in inputs)
                {
                    Rect r = new Rect(bn.xLocation, bn.yLocation + y, 20, 14);
                    style.border = new RectOffset(9, 9, 9, 9);
                    style.normal.background = (Texture2D)Resources.Load("input");
                    style.Draw(r, new GUIContent(), 0);
                    y += 18;
                }
            }
            Rect rs = new Rect(bn.xLocation + bn.width - 20, bn.yLocation + 25, 20, 14);
            style.border = new RectOffset(9, 9, 9, 9);
            style.normal.background = (Texture2D)Resources.Load("output");
            style.Draw(rs, new GUIContent(), 0);

            
        }
    }

    private void Save()
    {
        foreach (var n in nodeList)
        {
            if (n.uid == graph.outputNode.uid)
            {
                graph.outputNode = n;
            }
        }

        graph.nodes = nodeList;      
    }

    private void ConstructNodeList(NoiseNodeGraph graph)
    {
        #region old
        /*List<BaseNode> nodes = new List<BaseNode>();

        nodes.Add(graph.outputNode);

        bool done = false;

        while (!done)
        {
            List<BaseNode> currentNodes = new List<BaseNode>();

            if (graph.outputNode.getInputs() != null) {

                foreach (var n in graph.outputNode.getInputs())
                {
                    currentNodes.Add(n);
                }

                foreach (var n in currentNodes.ToArray())
                {
                    nodes.Add(n);

                    currentNodes.Clear();
                    if (n.getInputs() != null)
                    {
                        foreach (var _n in n.getInputs())
                        {
                            if (_n != null)
                            {
                                currentNodes.Add(_n);
                            }                            
                        }
                    }
                }                
            }

            done = true;
        }

        nodeList = nodes;*/
        #endregion old

        nodeList = graph.nodes;
        inGraphCenter = graph.fiftyFiftylocation;
        zoom = graph.zoom;
    }

    private void MoveNodes()
    {
        //check if a node is being currently moved
        if (holdingNode != null)
        {
            //move node that is being moved
            nodeList[nodeList.IndexOf(holdingNode)].xLocation = (int)Event.current.mousePosition.x - (int)offset.x;
            nodeList[nodeList.IndexOf(holdingNode)].yLocation = (int)Event.current.mousePosition.y - (int)offset.y;

        }

        if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
        {
            //check if a node is being clicked so it can be moved next round
            foreach (var n in nodeList)
            {
                Rect nodeRect = new Rect(n.xLocation, n.yLocation, n.width, n.height);

                if (nodeRect.Contains(Event.current.mousePosition))
                {
                    holdingNode = n;
                    offset = new Vector2(Event.current.mousePosition.x - nodeRect.x, Event.current.mousePosition.y - nodeRect.y);
                }
            }
        }

        if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
        {
            holdingNode = null;
        }

        foreach (var n in nodeList)
        {
            drawNode(n, new Rect(n.xLocation, n.yLocation, n.width, n.height));
        }
    }

    private void EventHandler(Event e)
    {
        if (EditingNode == null) {
            editing = false;
            MoveNodes();
            CreateConnection(e);
            
            CreateNode(e);
            RemoveNode(e);            
        }
        else
        {
            EditNode(e);
        }
    }

    private bool CreatingConn = false;
    private _UUID connCreatingnode;

    private void CreateConnection(Event e)
    {
        if (CreatingConn && connCreatingnode != null)
        {
            if (e.type == EventType.Repaint)
            {
                BaseNode bn = null;

                foreach (var n in nodeList)
                {
                    if (n.uid == connCreatingnode)
                    {
                        bn = n;
                    }
                }

                if (bn != null)
                {
                    Vector3 start = new Vector3(e.mousePosition.x, e.mousePosition.y, 0);
                    Vector3 end = new Vector3(bn.xLocation + bn.width, bn.yLocation + 25 + 6f, 0);
                    Handles.BeginGUI();
                    Handles.color = Color.white;
                    Handles.DrawAAPolyLine(5, new Vector3[] { start, end });
                    Handles.EndGUI();
                }
            }

            else if (e.type == EventType.MouseUp)
            {
                //muis is omhoog gegaan

                //loop through all nodes
                for (int n = 0; n < nodeList.Count; n++)
                {
                    //get inputs
                    var input = nodeList[n].getInputs();

                    //check of de inputs null zijn
                    if (input != null)
                    {
                        //loop through inputs
                        int y = 25;
                        foreach (var i in input)
                        {
                            //maak locatie van de inputs
                            Rect r = new Rect(nodeList[n].xLocation, nodeList[n].yLocation + y, 20, 14);
                            //kijk of de input de muispositie bevat
                            if (r.Contains(e.mousePosition))
                            {
                                //set the input to the new input
                                foreach (var ni in nodeList)
                                {
                                    if (ni.uid == connCreatingnode)
                                    {
                                        nodeList[n].setInput(y == 25 ? 0 : 1, ni);
                                    }
                                }
                            }

                            y += 18;
                        }
                    }
                }
                CreatingConn = false;
                connCreatingnode = null;
            }

        } else
        {
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                foreach (var n in nodeList)
                {
                    Rect rs = new Rect(n.xLocation + n.width - 20, n.yLocation + 25, 20, 14);
                    if (rs.Contains(e.mousePosition))
                    {
                        holdingNode = null;
                        CreatingConn = true;
                        connCreatingnode = n.uid;
                        return;
                    }
                }
            }
        }
    }

    private void CreateNode(Event e)
    {
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            bool isOver = false;
            foreach (var n in nodeList)
            {
                Rect r = new Rect(n.xLocation, n.yLocation, n.width, n.height);
                if (r.Contains(e.mousePosition))
                {
                    isOver = true;
                    break;
                }
            }

            if (!isOver)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Noise/Add Perlin Node"), false, AddPerlinNode);
                menu.AddItem(new GUIContent("Noise/Add Worley Node"), false, AddWorleyNode);
                menu.AddItem(new GUIContent("Modification/Add Invert Node"), false, AddInvertNode);
                menu.AddItem(new GUIContent("Modification/Add Curve Node"), false, AddCurveNode);
                menu.AddItem(new GUIContent("Modification/Add Combine Node"), false, AddCombineNode);

                menu.ShowAsContext();
            }
        }
    }

    #region different nodes
    void AddPerlinNode()
    {
        BaseNode pn = new PerlinNode();
        pn.name = "new Perlin Node";
        
        nodeList.Add(pn);
        Repaint();
    }

    void AddWorleyNode()
    {
        BaseNode pn = new WorleyNode();
        pn.name = "new Worley Node";
        
        nodeList.Add(pn);
        Repaint();
    }

    void AddInvertNode()
    {
        BaseNode pn = new InvertNode();
        pn.name = "new Invert Node";
        
        nodeList.Add(pn);
        Repaint();
    }

    void AddCurveNode()
    {
        BaseNode pn = new CurveNode();
        pn.name = "new Curve Node";
        
        nodeList.Add(pn);
        Repaint();
    }

    void AddCombineNode()
    {
        BaseNode pn = new CombineNode(); ;
        pn.name = "new Combine Node";
        
        nodeList.Add(pn);
        Repaint();
    }
    #endregion different nodes

    _UUID foundNode = null;

    //Also Edit
    private void RemoveNode(Event e)
    {
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            bool found = false;
            foreach (var n in nodeList)
            {
                Rect r = new Rect(n.xLocation, n.yLocation, n.width, n.height);
                if (r.Contains(e.mousePosition))
                {
                    foundNode = n.uid;
                    found = true;
                }
            }
            if (found)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("edit node"), false, Edit);
                menu.AddItem(new GUIContent("remove this node"), false, Remove);
                menu.AddItem(new GUIContent("set as output"), false, SetOutput);

                menu.ShowAsContext();
            }
        }
    }

    void SetOutput()
    {
        for (int n = 0; n < nodeList.Count; n++)
        {
            if (nodeList[n].uid == foundNode)
            {
                graph.outputNode = nodeList[n];
            }
        }
    }

    void Remove()
    {
        for (int n = 0; n < nodeList.Count; n++)
        {
            if (nodeList[n].uid == foundNode)
            {
                nodeList.RemoveAt(n);
            }
        }

        Repaint();
    }

    _UUID EditingNode = null;
    bool editing = false;

    void Edit()
    {
        EditingNode = foundNode;
        editing = true;
        Repaint();
    }    

    private void EditNode(Event e)
    {
        BaseNode editNode = null;
        foreach (var n in nodeList)
        {
            if (EditingNode == n.uid)
            {
                editNode = n;
            }
        }

        if(editNode != null && editing)
        {
            Rect editRect = new Rect(150,131, position.width - 300, position.height-231);
            Rect borderRect = new Rect(148, 98, position.width - 296, position.height - 196);
            Rect closeRect = new Rect(position.width - 177, 102, 25,25);

            EditorGUI.DrawRect(borderRect, Color.black);
            EditorGUI.DrawRect(editRect, Color.gray);
            GUI.DrawTexture(closeRect, (Texture) Resources.Load("close"));

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                if(closeRect.Contains(e.mousePosition)){
                    editing = false;
                    EditingNode = null;
                }
            }

            editNode.Display(editRect);

        }
    }
}