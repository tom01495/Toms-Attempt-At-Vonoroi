using System;
using System.Collections.Generic;
using UnityEngine;

//#nullable enable // Unity doesnt like this

// THIS NAMESPACE SHOULD ONLY BE USED FOR THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry {
    public class Boundary : PointSet {
        private Site higherSite;
        public Site LeftSite { get { return !isPositive ? normal.higherSite : normal.lowerSite; } }
        private Site lowerSite;
        public Site RightSite { get { return !isPositive ? normal.lowerSite : normal.higherSite ; } }

        private Vertex summit; //Vertex?
        public Vertex Summit { get { return normal.summit; } set { normal.summit = value; } }
        private Vertex base_; //Vertex?
        public Vertex Base { get { return normal.base_; } set { normal.base_ = value; } }

        protected Boundary normal;
        protected bool isPositive =  false;

        // =============================== Constructors

        public Boundary(Site leftSite, Site rightSite) {
            bool isHigher = leftSite.CompareTo(rightSite) >= 0;
            if(isHigher) {
                this.higherSite = leftSite;
                this.lowerSite = rightSite;
            } else {
                this.higherSite = rightSite;
                this.lowerSite = leftSite;
            }
            this.normal = this; 
        }
        protected Boundary(Boundary b) { 
            this.normal = b.normal;
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
                c1 = LeftSite.y - m1*RightSite.x;
            }
            float m2 = -1;
            float c2 = -1;
            if(!line2isStraight) {
                m2 = -(C.LeftSite.x - C.RightSite.x)/divY2;
                c2 = C.LeftSite.y - m2*C.RightSite.x;
            }

            // Step 2: Intersection of the two lines
            if(!Mathf.Approximately(m1,m2)) {
                float x;
                float y;
                
                if(line1isStraight) {
                    x = LeftSite.x;
                    y = m2*LeftSite.x + c2;
                } else if (line2isStraight) {
                    x = C.LeftSite.x;
                    y = m1*C.LeftSite.x + c1;
                } else {
                    x = (c2 - c1)/(m1 - m2);
                    y = (m1*c2 - m2*c1)/(m1 - m2);
                }

                Vertex v = new Vertex(x, y, this, C);
                return IsOnLine(v)? v : null;
            }
            return null; // They are parallel
        }

        protected bool IsOnLine(Point p) => true;

        public static bool IsIntersection(Boundary C1, Boundary C2, Point p) {
            if(p.GetType()==typeof(Vertex)) {
                Vertex v = (Vertex)p;
                return v.LeftBoundary == C1 && v.RightBoundary == C2;
            }
            return false;
        }

        // =============================== Overrides 

        public override bool Equals(object obj) {
            if(typeof(Boundary).IsInstanceOfType(obj)) {
                Boundary other = ((Boundary)obj);
                return other.LeftSite.Equals(LeftSite) && other.RightSite.Equals(RightSite);
            }
            return false;
        }

        public override int GetHashCode() { 
            return normal.GetHashCode();
        }
        
        // =============================== Conversion

        public UnsetBorderInit CreateUnsetBorder() {
            if(Base == null || Summit == null) throw new Exception("Base/Summit not set!");
            return new UnsetBorderInit(LeftSite, RightSite, Base, Summit);
        }
        public BoundaryNeg Neg() => new BoundaryNeg(this);
        public BoundaryPos Pos() => new BoundaryPos(this);
        public BoundaryZero Zero() => new BoundaryZero(this);
        public Boundary Normal() => normal;
    }

    public class BoundaryNeg : Boundary { // goes <= that way
        public BoundaryNeg(Boundary b) : base(b) {}

        protected new bool IsOnLine(Point p) {
            Site lowestSite = LeftSite.y < RightSite.y ? LeftSite : RightSite;
            return p.x < lowestSite.x; 
        }
    }

    public class BoundaryPos : Boundary { // goes => that way
        public BoundaryPos(Boundary b) : base(b) { isPositive = true; }

        protected new bool IsOnLine(Point p) {
            Site lowestSite = LeftSite.y < RightSite.y ? LeftSite : RightSite;
            return p.x > lowestSite.x;
        }
    }

    public class BoundaryZero : Boundary { // stays perfectly in the middle
        public BoundaryZero(Boundary b): base(b) {}

        protected new bool IsOnLine(Point p) {
            return true; 
        }
    }
}
