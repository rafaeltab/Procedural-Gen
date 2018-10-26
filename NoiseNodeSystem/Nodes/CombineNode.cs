using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
[Serializable]
public class CombineNode : BaseNode
{

    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.Input)]
    public BaseNode input1;
    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.Input)]
    public BaseNode input2;    

    public enum CombineMode {Add,Sub,Divide,Multiply, Mix}
    [Space]
    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.InspectOnly)]
    public CombineMode combineMode = CombineMode.Add;
    [Space]
    [Range(0,100)]
    [HideInInspector]
    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.InspectOnly)]
    public float percentage = 1;


    public override float[,] GetMap(int width, int height, int seed, Vector2 offset)
    {
        float[,] rArray = new float[width,height];

        float[,] input1Vals = input1.GetMap(width,height,seed,offset);
        float[,] input2Vals = input2.GetMap(width, height, seed,offset);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                switch (combineMode)
                {
                    case CombineMode.Add:
                        rArray[x, y] = (input1Vals[x, y] + input2Vals[x, y]) / 2;
                        break;
                    case CombineMode.Sub:
                        rArray[x, y] = input1Vals[x, y] - input2Vals[x, y];
                        break;
                    case CombineMode.Divide:
                        rArray[x, y] = input1Vals[x, y] / input2Vals[x,y];
                        break;
                    case CombineMode.Multiply:
                        rArray[x, y] = input1Vals[x, y] * input2Vals[x, y];
                        break;
                    case CombineMode.Mix:
                        rArray[x, y] = input1Vals[x, y] * (percentage / 100) + input2Vals[x, y] * (1-(percentage / 100));
                        break;
                    default:
                        break;
                }
            }
        }

        return rArray;
    }

    public override float[,,] GetCube(int width, int height,int depth, int seed, Vector3 offset)
    {
        float[,,] rArray = new float[width, height,depth];

        float[,,] input1Vals = input1.GetCube(width, height, depth, seed, offset);
        float[,,] input2Vals = input2.GetCube(width, height, depth, seed, offset);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
            {
                for (int z = 0; z < depth; z++)
                {
                    switch (combineMode)
                    {
                        case CombineMode.Add:
                            rArray[x, y, z] = (input1Vals[x, y, z] + input2Vals[x, y, z]) / 2;
                            break;
                        case CombineMode.Sub:
                            rArray[x, y, z] = input1Vals[x, y, z] - input2Vals[x, y, z];
                            break;
                        case CombineMode.Divide:
                            rArray[x, y, z] = input1Vals[x, y, z] / input2Vals[x, y, z];
                            break;
                        case CombineMode.Multiply:
                            rArray[x, y, z] = input1Vals[x, y, z] * input2Vals[x, y, z];
                            break;
                        case CombineMode.Mix:
                            rArray[x, y, z] = input1Vals[x, y, z] * (percentage / 100) + input2Vals[x, y, z] * (1 - (percentage / 100));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        return rArray;
    }

    public CombineNode()
    {
        nodeColor = NodeColor.blue;
        
    }

    public override BaseNode[] getInputs()
    {
        BaseNode[] rArray = { input1,input2 };

        return rArray;
    }

    public override string[] getInputNames()
    {
        string[] rArray = { "input1","input2"};

        return rArray;
    }

    public override void setInput(int number, BaseNode i)
    {
        if (number == 0)
        {
            input1 = i;
        }
        else
        {
            input2 = i;
        }
    }

    public override void Display(Rect editWindow)
    {
        Rect labelRect = new Rect(editWindow.x + 5, editWindow.y + 5, 200, 20);
        Rect fieldRect = new Rect(editWindow.x + 210, editWindow.y + 5, editWindow.width - 215, 20);        

        //combine mode
        EditorGUI.LabelField(labelRect, "combine mode:");
        combineMode = (CombineMode) EditorGUI.EnumPopup(fieldRect, combineMode);

        if (combineMode == CombineMode.Mix)
        {
            labelRect.y += 22;
            fieldRect.y += 22;

            //input2
            EditorGUI.LabelField(labelRect, "percentage:");
            percentage = EditorGUI.Slider(fieldRect,percentage,0,100);
        }
    }
}
