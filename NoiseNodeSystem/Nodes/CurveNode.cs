using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
[Serializable]
public class CurveNode : BaseNode
{


    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.Input)]
    public BaseNode input;

    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.InspectOnly)]
    public AnimationCurve curve;

    public CurveNode()
    {
        curve = AnimationCurve.Linear(0,0,1,1);
        nodeColor = NodeColor.blue;
        
    }

    public override float[,,] GetCube(int width, int height, int depth, int seed, Vector3 offset)
    {
        float[,,] map = input.GetCube(width, height,depth, seed, offset);
        float[,,] rArray = new float[width, height,depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    rArray[x, y, z] = Mathf.Clamp01(curve.Evaluate(map[x, y, z]));
                }
            }
        }

        return rArray;
    }

    public override float[,] GetMap(int width, int height, int seed, Vector2 offset)
    {
        float[,] map = input.GetMap(width, height, seed, offset);
        float[,] rArray = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                rArray[x, y] = Mathf.Clamp01(curve.Evaluate(map[x, y]));
            }
        }

        return rArray;
    }

    public override BaseNode[] getInputs()
    {
        BaseNode[] rArray = {input};

        return rArray;
    }

    public override string[] getInputNames()
    {
        string[] rArray = { "input" };

        return rArray;
    }

    public override void setInput(int number, BaseNode i)
    {
        input = i;
    }

    public override void Display(Rect editWindow)
    {
        Rect labelRect = new Rect(editWindow.x + 5, editWindow.y + 5, 200, 20);
        Rect fieldRect = new Rect(editWindow.x + 210, editWindow.y + 5, editWindow.width - 215, 100);

        //curve
        EditorGUI.LabelField(labelRect, "curve:");
        curve = EditorGUI.CurveField(fieldRect,curve);
    }
}
