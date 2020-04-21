using System;
using System.Collections.Generic;
using UnityEngine;

// Has all the initial values for the border to use. Use this for BorderModel!!!
public abstract class BorderInit {
    protected Vector2 middle;
    public Vector2 Middle { get { return middle; } }
    public float x { get { return middle.x; } }
    public float y { get { return middle.y; } }

    protected Vector2 base_;
    public Vector2 Base { get { return base_; } }
    protected Vector2 summit;
    public Vector2 Summit { get { return summit; } }
    
    protected TileInit leftTile;
    public TileInit LeftTile { get { return leftTile; } }
    protected TileInit rightTile;
    public TileInit RightTile { get { return rightTile; } }

    protected List<BorderInit> neighboursBase;
    public List<BorderInit> NeighboursBase { get { return neighboursBase; } }
    protected List<BorderInit> neighboursSummit;
    public List<BorderInit> NeighboursSummit { get { return neighboursSummit; } }

    public TileInit OtherTile(TileInit currentTile) {
        if(currentTile == leftTile) return rightTile;
        else if(currentTile == rightTile) return leftTile;
        else throw new Exception("tile not recognized");
    }
}
