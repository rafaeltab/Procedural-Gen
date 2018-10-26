using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseNode
{
    public enum NodeColor {orange, blue}

    [HideInInspector]
    public readonly _UUID uid;
    public string name = "new node";
    //in-graph location
    public int xLocation = 0;
    public int yLocation = 0;
    //in graph size
    public int width = 200;
    public int height = 100;

    public abstract void Display(Rect editWindow);

    [HideInInspector]
    public NodeColor nodeColor;

    public BaseNode()
    {
        uid = new _UUID();
    }

    public abstract float[,] GetMap(int width, int height, int seed, Vector2 offset);

    public abstract float[,,] GetCube(int width, int height, int depth, int seed, Vector3 offset);

    public abstract BaseNode[] getInputs();
    public abstract string[] getInputNames();
    public abstract void setInput(int number, BaseNode i);
}