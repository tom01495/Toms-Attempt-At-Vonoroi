using System.Collections.Generic;

// Has all the initial values for the tile to use. Use this for TileController & TileModel!!!
public abstract class TileInit {
    protected Coordinates coordMiddle;
    public Coordinates CoordMiddle { get { return coordMiddle;} }
    
    protected List<BorderInit> borders;
    public List<BorderInit> Borders { get { return borders; }  }

    public List<TileInit> NeighbouringTiles() {
        return borders.ConvertAll<TileInit>(b => b.OtherTile(this));
    }
}

public class UnsetTileInit : TileInit {
    public UnsetTileInit(Coordinates coordMiddle) {
        this.coordMiddle = coordMiddle;
    }

    public TileInit SetTileInit(List<BorderInit> borders) {
        this.borders = borders;
        return (TileInit)this;
    }
}