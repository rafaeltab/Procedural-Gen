using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class InvertNode : BaseNode
{
    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.Input)]
    public BaseNode input;

    public override float[,] GetMap(int width, int height, int seed, Vector2 offset)
    {
        if (input == null)
        {
            return null;
        }

        float[,] rArray = new float[width,height];
        float[,] arrayToBeInverted = input.GetMap(width, height, seed,offset);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                rArray[x, y] = 1 - arrayToBeInverted[x, y];
            }
        }

        return rArray;
    }

    public override float[,,] GetCube(int width, int height, int depth, int seed, Vector3 offset)
    {
        if (input == null)
        {
            return null;
        }

        float[,,] rArray = new float[width, height,depth];
        float[,,] arrayToBeInverted = input.GetCube(width, height,depth, seed,offset);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    rArray[x, y, z] = 1 - arrayToBeInverted[x, y, z];
                }                
            }
        }

        return rArray;
    }

    public InvertNode()
    {
        nodeColor = NodeColor.blue;
        
    }

    public override BaseNode[] getInputs()
    {
        BaseNode[] rArray = { input };

        return rArray;
    }

    public override string[] getInputNames()
    {
        string[] rArray = {"input"};

        return rArray;
    }

    public override void setInput(int number, BaseNode i)
    {
        input = i;
    }

    public override void Display(Rect editWindow)
    {
        
    }
}
