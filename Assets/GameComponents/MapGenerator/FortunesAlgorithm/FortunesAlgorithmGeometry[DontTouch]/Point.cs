using System;
using System.Collections.Generic;
using UnityEngine;

// THIS NAMESPACE SHOULD ONLY BE USED By THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry {
    public class Point : IComparable<Point> {
        protected float valueX;
        public float x { get { return valueX; } }
        protected float valueY;
        public float y { get { return valueY; } }
        public Vector2 vector { get { return new Vector2(x, y); } }

        protected float y_star;

        public Point(float x, float y) {
            this.valueX = x;
            this.valueY = y;
        }

        public Point(Vector2 v) {
            valueX = v.x;
            valueY = v.y;
        }

        public float Dist(Point p) {
            return (float)Math.Sqrt((x - p.x)*(x - p.x) + (y - p.y)*(y - p.y));
        }

        public int CompareTo(Point p) { // A point is lesser if it has a smaller y value
            if(Mathf.Approximately(y_star, p.y_star) && Mathf.Approximately(x, p.x)) return 0;
            else if (y_star > p.y_star || (Mathf.Approximately(y_star, p.y_star) && x > p.x)) return 1;
            else return -1;
        }

        // =============================== Overrides

        public override bool Equals(object obj){
            Point other = obj as Point;
            if(other != null) { return Mathf.Approximately(other.x,x) && Mathf.Approximately(other.y,y); }
            return false;
        }

        public override string ToString() => "(" + valueX.ToString() + "|" + valueY.ToString() + ")";
        public override int GetHashCode() => base.GetHashCode();
    }

    public class Vertex : Point {
        private Boundary leftBoundary;
        public Boundary LeftBoundary { get { return leftBoundary; } }
        private Boundary rightBoundary;
        public Boundary RightBoundary { get { return rightBoundary; } }
        private Boundary outgoingBoundary;
        public Boundary OutgoingBoundary { get { return outgoingBoundary; } set { if(outgoingBoundary == null) outgoingBoundary = value; }}

        public Vertex(float x, float y, Boundary leftBoundary, Boundary rightBoundary) : base(x, y) {
            this.leftBoundary = leftBoundary;
            this.rightBoundary = rightBoundary;
            y_star = y - leftBoundary.LeftSite.Dist(this);
        }

        public List<Boundary> OtherBoundaries(int hash) {
            if(hash == leftBoundary.GetHashCode()) return new List<Boundary>(){ rightBoundary, outgoingBoundary };
            if(hash == rightBoundary.GetHashCode()) return new List<Boundary>(){ leftBoundary, outgoingBoundary };
            if(hash == outgoingBoundary.GetHashCode()) return new List<Boundary>(){ leftBoundary, rightBoundary };
            else throw new Exception("Hash doesnt match one of the boundaries!");
        }
    }

    public class Site : Point {
        public List<Site> neighbours = new List<Site>();
        public List<Boundary> boundaries = new List<Boundary>();
        
        public Site(float x, float y) : base(x, y) {}

        public UnsetTileInit CreateUnsetTile() => new UnsetTileInit(this);
    }

    // =============================== Only Needed At The End

    public class UnsetTileInit : TileInit {
        private List<Site> neighbouringSites;
        public List<Site> Neighbours { get { return neighbouringSites; } }
        private List<Boundary> boundaries;
        public List<Boundary> Boundaries { get { return boundaries; } }

        private int hash;

        public UnsetTileInit(Site site) {
            this.neighbouringSites = site.neighbours;
            this.boundaries = site.boundaries;
            this.hash = site.GetHashCode();

            this.middle = site.vector;
        }

        public override int GetHashCode() => hash;

        public TileInit SetTileInit(List<BorderInit> borders) {
            this.borders = borders;
            return (TileInit)this;
        }
    }
}
