using System;
using System.Collections.Generic;
using UnityEngine;

// Needed for the fortunes algorithm (and a nice place to put the Possion Disc Sampling). They can be changed to Vector2's and back easily!
public class Coordinates {
    protected float valueX;
    public float x { get { return valueX; } }
    protected float valueY;
    public float y { get { return valueY; } }
    public Vector2 vector { get { return new Vector2(x, y); } }

    public Coordinates(float x, float y) {
        this.valueX = x;
        this.valueY = y;
    }

    public Coordinates(Vector2 v) {
        valueX = v.x;
        valueY = v.y;
    }

    public static Coordinates CreateRandom(Rect bounds) {
        float x = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
        float y = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
        return new Coordinates(x, y);
    }

    // ================================== Poisson Disc Sampling

    // uses http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf & https://www.youtube.com/watch?v=7WcmyxyFO7o 
    public static List<Coordinates> CreateRandomList(Rect bounds, int amount, float minDistance, int k = 30) {
        float cellSize = minDistance/(float)Math.Sqrt(2);

        int width = Mathf.CeilToInt(bounds.width/cellSize);
        int height = Mathf.CeilToInt(bounds.height/cellSize);
        int[,] map = new int[width, height];

        List<Coordinates> outputList = new List<Coordinates>();
        List<Coordinates> activeList = new List<Coordinates>();

        activeList.Add(Coordinates.CreateRandom(bounds));

        while(outputList.Count < amount) {
            if(activeList.Count == 0) throw new Exception("Active list got too small too soon! Choose a smaller min distance!");
            
            bool candidateAccepted = false;
            int index = UnityEngine.Random.Range(0, activeList.Count);
            Coordinates center = activeList[index];

            for (int i = 0; i < k; i++) {
                float angle = UnityEngine.Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Coordinates candidate = new Coordinates(center.vector + dir * UnityEngine.Random.Range(minDistance, 2*minDistance));

                if(bounds.Contains(candidate.vector)){
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

    private static bool IsValid(Coordinates candidate, List<Coordinates> outputList, float cellSize, float minDistance, int[,] map) {
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
                    float sqrDst = (candidate.vector - outputList[outputIndex].vector).sqrMagnitude;
                    if(sqrDst < minDistance*minDistance) {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    // ================================== Overrides

    public override bool Equals(object obj){
        if(typeof(Coordinates).IsInstanceOfType(obj)) {
            Coordinates other = ((Coordinates)obj);
            return Mathf.Approximately(other.x,x) && Mathf.Approximately(other.y,y);
        }
        return false;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override string ToString() {
        return "(" + valueX.ToString() + "|" + valueY.ToString() + ")";
    }
}