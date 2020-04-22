using System;
using System.Collections.Generic;
using UnityEngine;

// Needed for the Possion Disc Sampling
public static class PossionDiscSampling {

    public static Vector2 CreateRandom(Rect bounds) {
        float x = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
        float y = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
        return new Vector2(x, y);
    }

    // uses http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf & https://www.youtube.com/watch?v=7WcmyxyFO7o 
    public static List<Vector2> CreateRandomList(Rect bounds, float minDistance, int amount = int.MaxValue, int k = 30) {
        float cellSize = minDistance/(float)Math.Sqrt(2);

        int width = Mathf.CeilToInt(bounds.width/cellSize);
        int height = Mathf.CeilToInt(bounds.height/cellSize);
        int[,] map = new int[width, height];

        List<Vector2> outputList = new List<Vector2>();
        List<Vector2> activeList = new List<Vector2>();

        activeList.Add(PossionDiscSampling.CreateRandom(bounds));

        while(outputList.Count < amount) {
            if(activeList.Count == 0) {
                if(amount == int.MaxValue) break;
                else throw new Exception("Active list got too small too soon! Choose a smaller min distance!");
            }
            
            bool candidateAccepted = false;
            int index = UnityEngine.Random.Range(0, activeList.Count);
            Vector2 center = activeList[index];

            for (int i = 0; i < k; i++) {
                float angle = UnityEngine.Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = center + dir * UnityEngine.Random.Range(minDistance, 2*minDistance);

                if(bounds.Contains(candidate)){
                    if(IsValid(candidate, outputList, cellSize, minDistance, map)) {
                        candidateAccepted = true;
                        outputList.Add(candidate);
                        activeList.Add(candidate);
                        map[(int)(candidate.x/cellSize),(int)(candidate.y/cellSize)] = outputList.Count;
                        break;
                    }
                }
            }
            if(!candidateAccepted) activeList.RemoveAt(index);
        }

        return outputList;
    }

    private static bool IsValid(Vector2 candidate, List<Vector2> outputList, float cellSize, float minDistance, int[,] map) {
        int cellX = (int)(candidate.x/cellSize);
        int cellY = (int)(candidate.y/cellSize);

        int searchStartX = Mathf.Max(0, cellX - 2);
        int searchEndX   = Mathf.Min(cellX + 2, map.GetLength(0) - 1);
        int searchStartY = Mathf.Max(0, cellY - 2);
        int searchEndY   = Mathf.Min(cellY + 2, map.GetLength(1) - 1);

        for(int x = searchStartX; x <= searchEndX; x++) {
            for(int y = searchStartY; y <= searchEndY; y++) {
                int outputIndex = map[x,y]-1;
                if(outputIndex != -1) {
                    float sqrDst = (candidate - outputList[outputIndex]).sqrMagnitude;
                    if(sqrDst < minDistance*minDistance) {
                        return false;
                    }
                }
            }
        }
        return true;
    }

}
