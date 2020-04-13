using System;
using System.Collections.Generic;
using UnityEngine;

//#nullable enable // Unity doesnt like this

// THIS NAMESPACE SHOULD ONLY BE USED FOR THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry
{
    public class BoundaryBase
        {
            public Site leftSite;
            public Site rightSite;

            public Vertex summit; //Vertex?
            public Vertex base_; //Vertex?

            public BoundaryBase(Site leftSite, Site rightSite) {
                if(leftSite.x < rightSite.x) {
                    this.leftSite = leftSite;
                    this.rightSite = rightSite;
                } else {
                    this.leftSite = rightSite;
                    this.rightSite = leftSite;
                }

                summit = new Vertex(0,0, this, this);
                base_ = new Vertex(0,0, this, this);
            }

            // =============================== Intersection

            public Vertex Intersection(BoundaryBase C) { //Vertex? // TODO need to keep an eye out for neg/pos
                float divY1 = leftSite.y - rightSite.y;
                float divY2 = C.leftSite.y - C.rightSite.y;
                bool line1isStraight = Mathf.Approximately(divY1,0);
                bool line2isStraight = Mathf.Approximately(divY2,0);

                // Step 1: Calculating the lines (for y = m*x + c)
                float m1 = -1;
                float c1 = -1;
                if(!line1isStraight) {
                    m1 = -(leftSite.x - rightSite.x)/divY1;
                    c1 = leftSite.y - m1*rightSite.x;
                }
                float m2 = -1;
                float c2 = -1;
                if(!line2isStraight) {
                    m2 = -(C.leftSite.x - C.rightSite.x)/divY2;
                    c2 = C.leftSite.y - m2*C.rightSite.x;
                }

                // Step 2: Intersection of the two lines
                if(!Mathf.Approximately(m1,m2)) {
                    float x = -1;
                    float y = -1;
                    if(line1isStraight) {
                        x = leftSite.x;
                        y = m2*leftSite.x + c2;
                    } else if (line2isStraight) {
                        x = C.leftSite.x;
                        y = m1*C.leftSite.x + c1;
                    } else {
                        x = (c2 - c1)/(m1 - m2);
                        y = (m1*c2 - m2*c1)/(m1 - m2);
                    }
                    return new Vertex(x, y, this, C);
                }
                return null; // They are parallel
            }

        }


    public class Boundary : PointSet{
        protected BoundaryBase baseclass;

        // Constructors
        public Boundary(Site leftSite, Site rightSite){
            this.baseclass = new BoundaryBase(leftSite, rightSite);
        }
        public Boundary(BoundaryBase b){ this.baseclass = b; }

        //Base access
        public Site LeftSite { get { return baseclass.leftSite; } }
        public Site RightSite { get { return baseclass.rightSite; } }
        public Vertex summit { get { return baseclass.summit; } set { baseclass.summit = value;}} //Vertex?
        public Vertex base_ { get { return baseclass.base_; } set { baseclass.base_ = value;}} //Vertex?

        //conversion
        public BoundaryNeg Neg() => new BoundaryNeg(this);
        public BoundaryPos Pos() => new BoundaryPos(this);
        public BoundaryZero Zero() => new BoundaryZero(this);
        public static implicit operator BoundaryBase(Boundary b) => b.baseclass;
        public static implicit operator Boundary(BoundaryBase b) => new Boundary(b);

        //Public Methods
        public Vertex Intersection(BoundaryBase C){
            Vertex v = baseclass.Intersection(C);
            if(v == null) return null;
            return IsOnLine(v)? v : null;
        }

        public static bool IsIntersection(BoundaryBase C1, BoundaryBase C2, Point p) {
            if(p.GetType()==typeof(Vertex)) {
                Vertex inter = C1.Intersection(C2); //Vertex?
                if(inter != null) {
                    return Mathf.Approximately(inter.x, p.x) && Mathf.Approximately(inter.y, p.y);
                }
            }
            return false;
        }

        public static (BoundaryBase, BoundaryBase) IntersectionOf(List<BoundaryBase> boundaries, Vertex p) {
            for(int i = 1; i < boundaries.Count; i++) {
                if(IsIntersection(boundaries[i-1], boundaries[i], p)) {
                    return (boundaries[i-1], boundaries[i]);
                }
            }
            throw new Exception("No boundary intersection found!");
        }

        // ================================== Shortcuts

        public UnsetBorderInit CreateUnsetBorder() {
            if(base_ == null || summit == null) throw new Exception("Base/Summit not set!");
            return new UnsetBorderInit(LeftSite, RightSite, base_, summit);
        }

        protected new bool IsOnLine(Point p) => true ;
    }

    public class BoundaryNeg : Boundary { // = the left downward curve
        public BoundaryNeg(BoundaryBase b) : base(b) {}
        public static implicit operator BoundaryBase(BoundaryNeg b) => b.baseclass;
        public static implicit operator BoundaryNeg(BoundaryBase b) => new BoundaryNeg(b);

        protected new bool IsOnLine() { return true; }
    }

    public class BoundaryPos : Boundary {  // = the right upward curve
        public BoundaryPos(BoundaryBase b) : base(b) {}
        public static implicit operator BoundaryBase(BoundaryPos b) => b.baseclass;
        public static implicit operator BoundaryPos(BoundaryBase b) => new BoundaryPos(b);

        protected new bool IsOnLine() { return true; }
    }

    public class BoundaryZero : Boundary { // = a straight upward line
        public BoundaryZero(BoundaryBase b): base(b){}
        public static implicit operator BoundaryBase(BoundaryZero b) => b.baseclass;
        public static implicit operator BoundaryZero(BoundaryBase b) => new BoundaryZero(b);

        protected new bool IsOnLine() { return true; }
    }
}
