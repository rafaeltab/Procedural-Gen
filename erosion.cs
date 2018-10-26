using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class erosion{
    Terrain ter;


    public enum ErosionMode { Filter = 0, Brush = 1 }
    public enum ErosionType { Thermal = 0, Hydraulic = 1, Tidal = 2, Wind = 3, Glacial = 4 }
    public enum HydraulicType { Fast = 0, Full = 1, Velocity = 2 }
    public enum Neighbourhood { Moore = 0, VonNeumann = 1 }


    private ErosionMode p_erosionMode = ErosionMode.Filter;
    private ErosionType p_erosionType = ErosionType.Thermal;
    private Neighbourhood p_neighbourhood = Neighbourhood.Moore;
    private HydraulicType p_hydraulicType = HydraulicType.Fast;
    
    // Settings...
    public bool useDifferenceMaps = true;

    // Thermal...
    public int thermalIterations = 25;
    public float thermalMinSlope = 1.0f;
    public float thermalFalloff = 0.5f;

    // Hydraulic...
    public int hydraulicTypeInt = 0;
    public int hydraulicIterations = 25;

    // Fast...
    public float hydraulicMaxSlope = 60.0f;
    public float hydraulicFalloff = 0.5f;

    // Full...
    public float hydraulicRainfall = 0.01f;
    public float hydraulicEvaporation = 0.5f;
    public float hydraulicSedimentSolubility = 0.01f;
    public float hydraulicSedimentSaturation = 0.1f;

    // Velocity...
    public float hydraulicVelocityRainfall = 0.01f;
    public float hydraulicVelocityEvaporation = 0.5f;
    public float hydraulicVelocitySedimentSolubility = 0.01f;
    public float hydraulicVelocitySedimentSaturation = 0.1f;
    public float hydraulicVelocity = 20.0f;
    public float hydraulicMomentum = 1.0f;
    public float hydraulicEntropy = 0.0f;
    public float hydraulicDowncutting = 0.1f;

    // Tidal...
    public int tidalIterations = 25;
    public float tidalSeaLevel = 0.3f;
    public float tidalRangeAmount = 0.03f;
    public float tidalCliffLimit = 0.4f;

    // Wind...
    public int windIterations = 25;
    public float windDirection = 0.0f;
    public float windForce = 0.5f;
    public float windLift = 0.01f;
    public float windGravity = 0.5f;
    public float windCapacity = 0.01f;
    public float windEntropy = 0.1f;
    public float windSmoothing = 0.25f;

    public float[,] fastErosion(float[,] heightMap, Vector2 arraySize, int iterations,float width,float height,float length)
    {
        int Tw = (int)arraySize.y;
        int Th = (int)arraySize.x;
        float[,] heightDiffMap = new float[Tw, Th];
        Vector3 terSize = new Vector3(width,height,length);
        float minSlopeBlendMin = 0.0f;
        float minSlopeBlendMax = 0.0f;
        float maxSlopeBlendMin = 0.0f;
        float maxSlopeBlendMax = 0.0f;
        float seaLevel = 0.0f;
        float lowerTidalLimit = 0.0f;
        float upperTidalLimit = 0.0f;
        float tidalRange = 0.0f;
        float cliffLimit = 0.0f;
        switch (p_erosionType)
        {
            case ErosionType.Thermal:
                minSlopeBlendMin = ((terSize.x / Tw) * Mathf.Tan(thermalMinSlope * Mathf.Deg2Rad)) / terSize.y;
                if (minSlopeBlendMin > 1.0f)
                {
                    minSlopeBlendMin = 1.0f;
                }
                if (thermalFalloff == 1.0f)
                {
                    thermalFalloff = 0.999f;
                }
                float thermalMaxSlope = thermalMinSlope + ((90 - thermalMinSlope) * thermalFalloff);
                minSlopeBlendMax = ((terSize.x / Tw) * Mathf.Tan(thermalMaxSlope * Mathf.Deg2Rad)) / terSize.y;
                if (minSlopeBlendMax > 1.0f)
                {
                    minSlopeBlendMax = 1.0f;
                }
                break;
            case ErosionType.Hydraulic:
                maxSlopeBlendMax = ((terSize.x / Tw) * Mathf.Tan(hydraulicMaxSlope * Mathf.Deg2Rad)) / terSize.y;
                if (hydraulicFalloff == 0.0f)
                {
                    hydraulicFalloff = 0.001f;
                }
                float hydraulicMinSlope = hydraulicMaxSlope * (1 - hydraulicFalloff);
                maxSlopeBlendMin = ((terSize.x / Tw) * Mathf.Tan(hydraulicMinSlope * Mathf.Deg2Rad)) / terSize.y;
                break;
            case ErosionType.Tidal:
                seaLevel = tidalSeaLevel;
                lowerTidalLimit = (tidalSeaLevel - tidalRangeAmount) / (terSize.y);
                upperTidalLimit = (tidalSeaLevel + tidalRangeAmount) / (terSize.y);
                tidalRange = upperTidalLimit - seaLevel;
                cliffLimit = ((terSize.x / Tw) * Mathf.Tan(tidalCliffLimit * Mathf.Deg2Rad)) / terSize.y;
                break;
            default:
                return heightMap;
        }
        int xNeighbours;
        int yNeighbours;
        int xShift;
        int yShift;
        int xIndex;
        int yIndex;
        int Tx;
        int Ty;
        // Start iterations...
        for (int iter = 0; iter < iterations; iter++)
        {
            for (Ty = 0; Ty < Th; Ty++)
            {
                // y...
                if (Ty == 0)
                {
                    yNeighbours = 2;
                    yShift = 0;
                    yIndex = 0;
                }
                else if (Ty == Th - 1)
                {
                    yNeighbours = 2;
                    yShift = -1;
                    yIndex = 1;
                }
                else
                {
                    yNeighbours = 3;
                    yShift = -1;
                    yIndex = 1;
                }
                for (Tx = 0; Tx < Tw; Tx++)
                {
                    // x...
                    if (Tx == 0)
                    {
                        xNeighbours = 2;
                        xShift = 0;
                        xIndex = 0;
                    }
                    else if (Tx == Tw - 1)
                    {
                        xNeighbours = 2;
                        xShift = -1;
                        xIndex = 1;
                    }
                    else
                    {
                        xNeighbours = 3;
                        xShift = -1;
                        xIndex = 1;
                    }
                    // Calculate slope...
                    float tMin = 1.0f;
                    float tMax = 0.0f;
                    float tCumulative = 0.0f;
                    int Ny;
                    int Nx;
                    float t;
                    float heightAtIndex = heightMap[Tx + xIndex + xShift, Ty + yIndex + yShift]; // Get height at index
                    float hCumulative = heightAtIndex;
                    int nNeighbours = 0;
                    for (Ny = 0; Ny < yNeighbours; Ny++)
                    {
                        for (Nx = 0; Nx < xNeighbours; Nx++)
                        {
                            if (Nx != xIndex || Ny != yIndex)
                            {
                                if (p_neighbourhood == Neighbourhood.Moore || (p_neighbourhood == Neighbourhood.VonNeumann && (Nx == xIndex || Ny == yIndex)))
                                {
                                    float heightAtPoint = heightMap[Tx + Nx + xShift, Ty + Ny + yShift]; // Get height at point
                                                                                                         // Tidal...
                                    hCumulative += heightAtPoint;
                                    // Others...
                                    t = heightAtIndex - heightAtPoint;
                                    if (t > 0)
                                    {
                                        tCumulative += t;
                                        if (t < tMin) tMin = t;
                                        if (t > tMax) tMax = t;
                                    }
                                    nNeighbours++;
                                }
                            }
                        }
                    }
                    float tAverage = tCumulative / nNeighbours;
                    // float tAverage = tMax;
                    // Erosion type...
                    bool doErode = false;
                    switch (p_erosionType)
                    {
                        case ErosionType.Thermal:
                            if (tAverage >= minSlopeBlendMin)
                            {
                                doErode = true;
                            }
                            break;
                        case ErosionType.Hydraulic:
                            if (tAverage > 0 && tAverage <= maxSlopeBlendMax)
                            {
                                doErode = true;
                            }
                            break;
                        case ErosionType.Tidal:
                            if (tAverage > 0 && tAverage <= cliffLimit && heightAtIndex < upperTidalLimit && heightAtIndex > lowerTidalLimit)
                            {
                                doErode = true;
                            }
                            break;
                        default:
                            return heightMap;
                    }
                    if (doErode)
                    {
                        float blendAmount;
                        if (p_erosionType == ErosionType.Tidal)
                        {
                            // Tidal...
                            float hAverage = hCumulative / (nNeighbours + 1);
                            float dTidalSeaLevel = Mathf.Abs(seaLevel - heightAtIndex);
                            blendAmount = dTidalSeaLevel / tidalRange;
                            float blendHeight = heightAtIndex * blendAmount + hAverage * (1 - blendAmount);
                            float blendTidalSeaLevel = Mathf.Pow(dTidalSeaLevel, 3);
                            heightMap[Tx + xIndex + xShift, Ty + yIndex + yShift] = seaLevel * blendTidalSeaLevel + blendHeight * (1 - blendTidalSeaLevel);
                        }
                        else
                        {
                            // Thermal or Hydraulic...
                            float blendRange;
                            if (p_erosionType == ErosionType.Thermal)
                            {
                                if (tAverage > minSlopeBlendMax)
                                {
                                    blendAmount = 1;
                                }
                                else
                                {
                                    blendRange = minSlopeBlendMax - minSlopeBlendMin;
                                    blendAmount = (tAverage - minSlopeBlendMin) / blendRange; // minSlopeBlendMin = 0; minSlopeBlendMax = 1
                                }
                            }
                            else
                            {
                                if (tAverage < maxSlopeBlendMin)
                                {
                                    blendAmount = 1;
                                }
                                else
                                {
                                    blendRange = maxSlopeBlendMax - maxSlopeBlendMin;
                                    blendAmount = 1 - ((tAverage - maxSlopeBlendMin) / blendRange); // maxSlopeBlendMin = 1; maxSlopeBlendMax = 0
                                }
                            }
                            float m = tMin / 2 * blendAmount;
                            float pointValue = heightMap[Tx + xIndex + xShift, Ty + yIndex + yShift];
                            if (p_erosionMode == ErosionMode.Filter || (p_erosionMode == ErosionMode.Brush && useDifferenceMaps))
                            {
                                // Pass to difference map...
                                float heightDiffAtIndexSoFar = heightDiffMap[Tx + xIndex + xShift, Ty + yIndex + yShift];
                                float heightAtIndexDiff = heightDiffAtIndexSoFar - m;
                                heightDiffMap[Tx + xIndex + xShift, Ty + yIndex + yShift] = heightAtIndexDiff;
                            }
                            else
                            {
                                float pointValueAfter = pointValue - m;
                                if (pointValueAfter < 0) pointValueAfter = 0;
                                heightMap[Tx + xIndex + xShift, Ty + yIndex + yShift] = pointValueAfter;
                            }
                            for (Ny = 0; Ny < yNeighbours; Ny++)
                            {
                                for (Nx = 0; Nx < xNeighbours; Nx++)
                                {
                                    if (Nx != xIndex || Ny != yIndex)
                                    {
                                        if (p_neighbourhood == Neighbourhood.Moore || (p_neighbourhood == Neighbourhood.VonNeumann && (Nx == xIndex || Ny == yIndex)))
                                        {
                                            float neighbourValue = heightMap[Tx + Nx + xShift, Ty + Ny + yShift];
                                            t = pointValue - neighbourValue;
                                            // Only move material downhill...
                                            if (t > 0)
                                            {
                                                float mProportional = m * (t / tCumulative);
                                                if (p_erosionMode == ErosionMode.Filter || (p_erosionMode == ErosionMode.Brush && useDifferenceMaps))
                                                {
                                                    // Pass to difference map...
                                                    float heightDiffAtNeighbourSoFar = heightDiffMap[Tx + Nx + xShift, Ty + Ny + yShift];
                                                    float heightAtNeighbourDiff = heightDiffAtNeighbourSoFar + mProportional;
                                                    heightDiffMap[Tx + Nx + xShift, Ty + Ny + yShift] = heightAtNeighbourDiff;
                                                }
                                                else
                                                {
                                                    neighbourValue += mProportional;
                                                    if (neighbourValue < 0) neighbourValue = 0;
                                                    heightMap[Tx + Nx + xShift, Ty + Ny + yShift] = neighbourValue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if ((p_erosionMode == ErosionMode.Filter || (p_erosionMode == ErosionMode.Brush && useDifferenceMaps)) && p_erosionType != ErosionType.Tidal)
            {
                // Apply height difference map to height map...
                float heightAtCell;
                for (Ty = 0; Ty < Th; Ty++)
                {
                    for (Tx = 0; Tx < Tw; Tx++)
                    {
                        heightAtCell = heightMap[Tx, Ty] + heightDiffMap[Tx, Ty];
                        if (heightAtCell > 1.0f)
                        {
                            heightAtCell = 1.0f;
                        }
                        else if (heightAtCell < 0.0f)
                        {
                            heightAtCell = 0.0f;
                        }
                        heightMap[Tx, Ty] = heightAtCell;
                        heightDiffMap[Tx, Ty] = 0.0f;
                    }
                }
            }            
        }
        return heightMap;
    }







}
