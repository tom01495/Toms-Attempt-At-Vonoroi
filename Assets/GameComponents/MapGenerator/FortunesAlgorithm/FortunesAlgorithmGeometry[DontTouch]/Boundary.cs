using System;
using System.Collections.Generic;
using UnityEngine;

//#nullable enable // Unity doesnt like this

// THIS NAMESPACE SHOULD ONLY BE USED FOR THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry
{
    public class Boundary : PointSet {
        public Site LeftSite { get { return model.leftSite; } }
        public Site RightSite { get { return model.rightSite; } }
        public Vertex summit { get { return model.summit; } set { model.summit = value; } }
        public Vertex base_ { get { return model.base_; } set { model.base_ = value; } }

        protected BoundaryModel model;

        protected class BoundaryModel {
            public Site leftSite;
            public Site rightSite;
            public Vertex summit; //Vertex?
            public Vertex base_; //Vertex?

            public BoundaryModel(Site leftSite, Site rightSite) {
                this.leftSite = leftSite;
                this.rightSite = rightSite;

                //this.summit = new Vertex(0,0, new Boundary(this), new Boundary(this)); // TODO remove these 2 whole lines once the code works
                //this.base_ = new Vertex(0,0, new Boundary(this), new Boundary(this)); // (needs to be null by default)
            }
        }

        // =============================== Constructors

        public Boundary(Site leftSite, Site rightSite) {
            this.model = new BoundaryModel(leftSite, rightSite);
        }
        protected Boundary(Boundary b) { this.model = b.model; }
        protected Boundary(BoundaryModel b) { this.model = b; }

        // =============================== Intersection

        public Vertex Intersection(Boundary C) { //Vertex? // TODO need to keep an eye out for neg/pos
            BoundaryModel model2 = C.model;

            float divY1 = model.leftSite.y - model.rightSite.y;
            float divY2 = model2.leftSite.y - model2.rightSite.y;
            bool line1isStraight = Mathf.Approximately(divY1,0);
            bool line2isStraight = Mathf.Approximately(divY2,0);

            // Step 1: Calculating the lines (for y = m*x + c)
            float m1 = -1;
            float c1 = -1;
            if(!line1isStraight) {
                m1 = -(model.leftSite.x - model.rightSite.x)/divY1;
                c1 = model.leftSite.y - m1*model.rightSite.x;
            }
            float m2 = -1;
            float c2 = -1;
            if(!line2isStraight) {
                m2 = -(model2.leftSite.x - model2.rightSite.x)/divY2;
                c2 = model2.leftSite.y - m2*model2.rightSite.x;
            }

            // Step 2: Intersection of the two lines
            if(!Mathf.Approximately(m1,m2)) {
                float x = -1;
                float y = -1;
                if(line1isStraight) {
                    x = model.leftSite.x;
                    y = m2*model.leftSite.x + c2;
                } else if (line2isStraight) {
                    x = model2.leftSite.x;
                    y = m1*model2.leftSite.x + c1;
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

        public static (Boundary, Boundary) IntersectionOf(List<Boundary> boundaries, Vertex p) {
            for(int i = 1; i < boundaries.Count; i++) {
                if(IsIntersection(boundaries[i-1], boundaries[i], p)) {
                    return (boundaries[i-1], boundaries[i]);
                }
            }
            throw new Exception("No boundary intersection found!");
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
            return model.GetHashCode();
        }
        
        // =============================== Conversion

        public UnsetBorderInit CreateUnsetBorder() {
            if(base_ == null || summit == null) throw new Exception("Base/Summit not set!");
            return new UnsetBorderInit(LeftSite, RightSite, base_, summit);
        }
        public BoundaryNeg Neg() => new BoundaryNeg(this);
        public BoundaryPos Pos() => new BoundaryPos(this);
        public BoundaryZero Zero() => new BoundaryZero(this);
    }

    public class BoundaryNeg : Boundary {
        public BoundaryNeg(Boundary b) : base(b) {}

        protected new bool IsOnLine(Point p) {
            return p.y < (LeftSite.y+RightSite.y)/2; 
        }
    }

    public class BoundaryPos : Boundary {
        public BoundaryPos(Boundary b) : base(b) {}

        protected new bool IsOnLine(Point p) {
            return p.y > (LeftSite.y+RightSite.y)/2;
        }
    }

    public class BoundaryZero : Boundary {
        public BoundaryZero(Boundary b): base(b){}

        protected new bool IsOnLine(Point p) {
            return true; 
        }
    }
}
