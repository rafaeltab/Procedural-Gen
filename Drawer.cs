using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Drawer : MonoBehaviour {

    #region vars

    public enum DrawMode {Texture,Terrain,Textureandterrain}
    public DrawMode drawMode;

    public Terrain terrain;
    public NoiseNodeGraph bn;
    public GameObject go;
    [Space]

    public int width = 400;
    public int height = 400;
    [Space]

    public int seed = 1;
    #endregion vars

    public void Show()
    {
        if (bn != null)
        {
            BaseNode o = bn.outputNode;

            if (drawMode == DrawMode.Texture)
            {
                Texture2D texs = new Texture2D(width, height);

                float[,] hm = o.GetMap(width, height, seed, new Vector2(0,0));

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        float val = hm[x, y];
                        texs.SetPixel(x, y, new Color(val, val, val, 1));

                    }
                }

                texs.Apply();
                texs.filterMode = FilterMode.Point;
                texs.wrapMode = TextureWrapMode.Clamp;
                go.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texs;

                return;
            } else if (drawMode == DrawMode.Terrain)
            {


                SplatPrototype[] tex = terrain.terrainData.splatPrototypes;

                TerrainData data = new TerrainData
                {
                    size = new Vector3(32, 100, 32),
                    heightmapResolution = 1024,
                    splatPrototypes = tex
                };

                

                float[,] heightMap = o.GetMap(data.heightmapWidth, data.heightmapHeight,seed, new Vector2(0,0));
                
                data.SetHeights(0, 0, heightMap);

                terrain.terrainData = data;
            }

       


            
        }
        
    }

}
