using System.Collections.Generic;
using UnityEngine;

// Has all the initial values for the tile to use. Use this for TileModel!!!
public abstract class TileInit : MonoBehaviour {
    protected Vector2 middle;
    public Vector2 Middle { get { return middle; } }
    public float x { get { return middle.x; } }
    public float y { get { return middle.y; } }
    
    protected List<BorderInit> borders;
    public List<BorderInit> Borders { get { return borders; }  }

    public List<TileInit> NeighbouringTiles() {
        return borders.ConvertAll<TileInit>(b => b.OtherTile(this));
    }
}
