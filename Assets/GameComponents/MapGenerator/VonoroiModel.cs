using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VonoroiModel : MonoBehaviour
{
    public float xMin = 0;
    public float yMin = 0;
    public float xMax = 1000;
    public float yMax = 1000;
    public Rect bounds { get { return Rect.MinMaxRect(xMin, yMin, xMax, yMax); } }

    public int amountTiles = 100; // OLD
    public float minDistanceTiles = 5;
    public float minBoundaryLength = 100; // remember, dont make this too large!
    
    public List<BorderInit> borderInits;
    public List<TileInit> tileInits;

    // Start is called before the first frame update
    void Start() {
        
    }
}
