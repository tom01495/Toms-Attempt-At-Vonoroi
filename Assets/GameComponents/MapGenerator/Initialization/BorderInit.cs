using System;
using System.Collections.Generic;

// Has all the initial values for the border to use. Use this for BorderController & BorderModel!!!
public abstract class BorderInit {
    protected Coordinates coordBase;
    public Coordinates CoordBase { get { return coordBase; } }
    protected Coordinates coordSummit;
    public Coordinates CoordSummit { get { return coordSummit; } }
    protected Coordinates coordMiddle;
    public Coordinates CoordMiddle { get { return coordMiddle; } }
    
    protected TileInit leftTile;
    public TileInit LeftTile { get { return leftTile; } }
    protected TileInit rightTile;
    public TileInit RightTile { get { return rightTile; } }

    protected List<BorderInit> bordersBase;
    public List<BorderInit> BordersBase { get { return bordersBase; } }
    protected List<BorderInit> bordersSummit;
    public List<BorderInit> BordersSummit { get { return bordersSummit; } }

    public TileInit OtherTile(TileInit currentTile) {
        if(currentTile == leftTile) return rightTile;
        else if(currentTile == rightTile) return leftTile;
        else throw new Exception("tile not recognized");
    }
}

public class UnsetBorderInit : BorderInit {
    protected Coordinates coordLeftTile;
    protected Coordinates coordRightTile;

    public UnsetBorderInit(Coordinates coordLeftTile, Coordinates coordRightTile, Coordinates coordBase, Coordinates coordSummit) {
        this.coordLeftTile = coordLeftTile;
        this.coordRightTile = coordRightTile;
        this.coordBase = coordBase;
        this.coordSummit = coordSummit;
        this.coordMiddle = new Coordinates(
            (coordBase.x + coordSummit.x)/2, 
            (coordBase.y + coordSummit.y)/2
        );
    }

    public BorderInit SetBorderInit(TileInit leftTile, TileInit rightTile, List<BorderInit> bordersBase, List<BorderInit> bordersSummit) {
        this.leftTile = leftTile;
        this.rightTile = rightTile;
        this.bordersBase = bordersBase;
        this.bordersSummit = bordersSummit;
        return (BorderInit)this;
    }
}