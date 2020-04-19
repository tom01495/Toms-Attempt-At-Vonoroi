using System;
using System.Collections.Generic;
using UnityEngine;

//#nullable enable // Unity doesnt like this

// THIS NAMESPACE SHOULD ONLY BE USED BY THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry {
    public class Boundary : PointSet {
        protected class BoundaryData {
            private Site higher;
            public Site higherSite { get { return higher; } }
            private Site lower;
            public Site lowerSite { get { return lower; } }
            
            public Vertex summit; //Vertex?
            public Vertex base_; //Vertex?

            public BoundaryData(Site leftSite, Site rightSite) {
                bool leftIsHigher = leftSite.CompareTo(rightSite) >= 0;
                if(leftIsHigher) {
                    higher = leftSite;
                    lower = rightSite;
                } else {
                    higher = rightSite;
                    lower = leftSite;
                }
                leftSite.neigbhours.Add(rightSite);
                rightSite.neigbhours.Add(leftSite);
            }
        }

        public virtual Site LeftSite { get { return data.higherSite; } }
        public virtual Site RightSite { get { return data.lowerSite; } }

        public Vertex Base { 
            get { return data.base_; } 
            set { data.base_ = value; } } // start of the line
        public Vertex Summit { 
            get { return data.summit; } 
            set { if(data.summit == null) data.summit = value;
                  else data.base_ = value; } } // end of the line

        protected BoundaryData data;

        // =============================== Constructors

        public static Boundary CreateSubtype(Site leftSite, Site rightSite) {
            Boundary boundary = new Boundary(leftSite, rightSite);
            if(leftSite.y > rightSite.y) { return boundary.Neg(); }
            else if(leftSite.y < rightSite.y) { return boundary.Pos(); }
            else { return boundary.Zero(); }
        }

        public Boundary(Site leftSite, Site rightSite) {
            data = new BoundaryData(leftSite, rightSite);
        }

        protected Boundary(Boundary b) {
            this.data = b.data;
        }

        // =============================== Intersection

        public Vertex Intersection(Boundary C) { //Vertex?
            float divY1 = LeftSite.y - RightSite.y;
            float divY2 = C.LeftSite.y - C.RightSite.y;
            bool line1isStraight = Mathf.Approximately(divY1,0);
            bool line2isStraight = Mathf.Approximately(divY2,0);

            // Step 1: Calculating the lines (for y = m*x + c)
            float m1 = -1;
            float c1 = -1;
            if(!line1isStraight) {
                m1 = -(LeftSite.x - RightSite.x)/divY1;
                c1 = ((LeftSite.y + RightSite.y) / 2f) - m1*((RightSite.x + LeftSite.x) / 2f);
            }
            float m2 = -1;
            float c2 = -1;
            if(!line2isStraight) {
                m2 = -(C.LeftSite.x - C.RightSite.x)/divY2;
                c2 = ((C.LeftSite.y + C.RightSite.y) / 2f) - m2*((C.RightSite.x + C.LeftSite.x) / 2f);
            }

            // Step 2: Intersection of the two lines
            if(!Mathf.Approximately(m1,m2)) {
                float x;
                float y;

                if(line1isStraight) {
                    x = (RightSite.x + LeftSite.x) /2f;
                    y = m2*x + c2;
                } else if (line2isStraight) {
                    x = (C.RightSite.x + C.LeftSite.x) / 2f;
                    y = m1*x + c1;
                } else {
                    x = (c2 - c1)/(m1 - m2);
                    y = (m1*c2 - m2*c1)/(m1 - m2);
                }

                (Boundary leftBoundary, Boundary rightBoundary) = sortLeftAndRight(this, C);
                Vertex v = new Vertex(x, y, leftBoundary, rightBoundary);
                return IsOnLine(v) && C.IsOnLine(v) ? v : null;
            }
            return null; // They are parallel
        }

        private static (Boundary, Boundary) sortLeftAndRight(Boundary C1, Boundary C2) {
            if(C1.RightSite.Equals(C2.LeftSite)) { return (C1, C2); }
            else if(C2.RightSite.Equals(C1.LeftSite)) { return (C2, C1); }
            else throw new Exception("Boundaries are not neighbours!");
        }

        public static bool IsIntersection(Boundary C1, Boundary C2, Point p) {
            if(typeof(Vertex).IsInstanceOfType(p)) {
                Vertex v = (Vertex)p;
                return v.LeftBoundary == C1 && v.RightBoundary == C2;
            }
            return false;
        }

        // =============================== Overrides

        public override bool Equals(object obj) {
            if(typeof(Boundary).IsInstanceOfType(obj)) {
                Boundary b = obj as Boundary;
                return LeftSite.Equals(b.LeftSite) && RightSite.Equals(b.RightSite);
            }
            return false;
        }

        //Maybe make a function called "bool SameModel(Boundary C)" instead?
        public static bool operator != (Boundary a, Boundary b) =>  !(a == b);
        public static bool operator == (Boundary a, Boundary b) => a?.data == b?.data;

        public override string ToString() {
            return LeftSite.ToString() + RightSite.ToString();
        }

        public override int GetHashCode() {
            return data.GetHashCode();
        }

        // =============================== Conversion

        protected virtual bool IsOnLine(Point p) => true;

        public UnsetBorderInit CreateUnsetBorder() {
            if(Base == null || Summit == null) return null; //throw new Exception("Base/Summit not set!");
            return new UnsetBorderInit(LeftSite, RightSite, Base, Summit);
        }
        public BoundaryNeg Neg() => new BoundaryNeg(this);
        public BoundaryPos Pos() => new BoundaryPos(this);
        public BoundaryZero Zero() => new BoundaryZero(this);
        public Boundary Normal() => new Boundary(this);
    }

    public class BoundaryNeg : Boundary { // goes <= that way
        public BoundaryNeg(Boundary b) : base(b) {}

        public override Site LeftSite { get {return data.higherSite;} }
        public override Site RightSite { get {return data.lowerSite;} }

        protected override bool IsOnLine(Point p){
            return (data.base_ != null)? p.x < data.base_.x :
                                         p.x < data.lowerSite.x; //(LeftSite.x + RightSite.x)/2;
        }
    }

    public class BoundaryPos : Boundary { // goes => that way
        public BoundaryPos(Boundary b) : base(b) {}

        public override Site LeftSite { get {return data.lowerSite;} }
        public override Site RightSite { get {return data.higherSite;} }

        protected override bool IsOnLine(Point p){
            return (data.base_ != null)? p.x > data.base_.x :
                                         p.x > data.lowerSite.x; //(LeftSite.x + RightSite.x)/2;
        }
    }

    public class BoundaryZero : Boundary { // stays perfectly in the middle
        public BoundaryZero(Boundary b): base(b) {}
        protected override bool IsOnLine(Point p) => true;
    }
}
