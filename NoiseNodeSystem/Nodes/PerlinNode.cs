using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;



[Serializable]
public class PerlinNode : BaseNode
{

    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.InspectOnly)]
    [Space]
    public float scale = 1;

    [Space]
    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.Property)]
    [Range(1,10)]
    public int octaves = 1;

    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.Property)]
    [Space]
    public float amplitude = 1;

    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.Property)]
    public float frequency = 1;

    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.Property)]
    [Space]
    public float lacunarity = 1;

    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.Property)]
    [Range(0,1)]
    public float persistance = 1;

    [VariableVisibility(VariableVisibilityAttribute.VisibilityType.InspectOnly)]
    [Space]
    public int seedOffset = 0;

    public override float[,] GetMap(int width, int height, int seed, Vector2 offset)
    {
        int oct = octaves;
        
        FastNoise fn = new FastNoise(seed);

        float[,] rArray = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float amp = amplitude;
                float freq = frequency;
                float lac = lacunarity;
                float pers = persistance;

                float noiseHeight = 0;

                for (int o = 0; o < oct; o++)
                {
                    float sampleX = ((x + offset.x) + (float) 800 * o) / scale * freq;
                    float sampleY = (y + offset.y) / scale * freq;

                    float perlinVal = fn.GetPerlin(sampleX,sampleY);       

                    noiseHeight += perlinVal * amp;

                    amp *= pers;
                    freq *= lac;
                }

                noiseHeight += 1;
                noiseHeight /= 2.5f;

                noiseHeight = Mathf.Clamp(noiseHeight,0,1);
                rArray[x, y] = noiseHeight;
            }
        }

        return rArray;        
    }

    public override float[,,] GetCube(int width, int height,int depth, int seed, Vector3 offset)
    {
        int oct = octaves;

        FastNoise fn = new FastNoise(seed);

        float[,,] rArray = new float[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    float amp = amplitude;
                    float freq = frequency;
                    float lac = lacunarity;
                    float pers = persistance;

                    float noiseHeight = 0;

                    for (int o = 0; o < oct; o++)
                    {
                        float sampleX = ((x + offset.x) + (float)800 * o) / scale * freq;
                        float sampleY = (y + offset.x) / scale * freq;
                        float sampleZ = (z + offset.x) / scale * freq;


                        float perlinVal = fn.GetPerlin(sampleX, sampleY, sampleZ);

                        noiseHeight += perlinVal * amp;

                        amp *= pers;
                        freq *= lac;
                    }

                    noiseHeight += 1;
                    noiseHeight /= 2.5f;

                    noiseHeight = Mathf.Clamp(noiseHeight, 0, 1);
                    rArray[x, y, z] = noiseHeight;
                }
            }
        }

        return rArray;
    }

    public PerlinNode()
    {
        nodeColor = NodeColor.orange;
        
    }

    public override BaseNode[] getInputs()
    {
        return null;
    }

    public override string[] getInputNames()
    {
        return null;
    }

    public override void setInput(int number, BaseNode i) { }

    public override void Display(Rect editWindow)
    {
        Rect labelRect = new Rect(editWindow.x + 5, editWindow.y + 5, 200, 20);
        Rect fieldRect = new Rect(editWindow.x + 210, editWindow.y + 5, editWindow.width - 215, 20);

        //scale
        EditorGUI.LabelField(labelRect, "scale:");
        scale = EditorGUI.FloatField(fieldRect, scale);

        labelRect.y += 22;
        fieldRect.y += 22;

        //octaves
        EditorGUI.LabelField(labelRect, "octaves:");
        octaves = EditorGUI.IntSlider(fieldRect, octaves, 1, 10);

        labelRect.y += 22;
        fieldRect.y += 22;

        //amplitude
        EditorGUI.LabelField(labelRect, "amplitude:");
        amplitude = EditorGUI.FloatField(fieldRect, amplitude);

        labelRect.y += 22;
        fieldRect.y += 22;

        //frequency
        EditorGUI.LabelField(labelRect, "frequency:");
        frequency = EditorGUI.FloatField(fieldRect, frequency);

        labelRect.y += 22;
        fieldRect.y += 22;

        //lacunarity
        EditorGUI.LabelField(labelRect, "lacunarity:");
        lacunarity = EditorGUI.FloatField(fieldRect, lacunarity);

        labelRect.y += 22;
        fieldRect.y += 22;

        //persistance
        EditorGUI.LabelField(labelRect, "persistance:");
        persistance = EditorGUI.FloatField(fieldRect, persistance);

        labelRect.y += 22;
        fieldRect.y += 22;

        //seed offset
        EditorGUI.LabelField(labelRect, "seed offset:");
        seedOffset = EditorGUI.IntField(fieldRect, seedOffset);
    }
}
