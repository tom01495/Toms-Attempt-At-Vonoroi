using System;
using System.Collections.Generic;
using UnityEngine;

//#nullable enable // Unity doesnt like this

// THIS NAMESPACE SHOULD ONLY BE USED FOR THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry
{
    public class Boundary : PointSet
    {
        protected Site leftSite;
        public Site LeftSite { get { return leftSite; } }
        protected Site rightSite;
        public Site RightSite { get { return rightSite; } }

        public Vertex summit = new Vertex(0,0); //Vertex?
        public Vertex base_ = new Vertex(0,0); //Vertex?

        public Boundary(Site leftSite, Site rightSite) {
            if(leftSite.x < rightSite.x) {
                this.leftSite = leftSite;
                this.rightSite = rightSite;
            } else {
                this.leftSite = rightSite;
                this.rightSite = leftSite;
            }
        }

        // =============================== Intersection

        public Vertex Intersection(Boundary C) { //Vertex? // TODO need to keep an eye out for neg/pos
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
                return new Vertex(x, y);
            }
            return null; // They are parallel
        }

        public static bool IsIntersection(Boundary C1, Boundary C2, Point p) {
            if(p.GetType()==typeof(Vertex)) {
                Vertex inter = C1.Intersection(C2); //Vertex?
                if(inter != null) { 
                    return Mathf.Approximately(inter.x, p.x) && Mathf.Approximately(inter.y, p.y);
                }
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

        // ================================== Shortcuts
        
        public Boundary Neg() {
            return this;
        }
        public Boundary Pos() {
            return this;
        }
        public Boundary Zero() {
            return this;
        }
        public UnsetBorderInit CreateUnsetBorder() {
            if(base_ == null || summit == null) throw new Exception("Base/Summit not set!");
            return new UnsetBorderInit(leftSite, rightSite, base_, summit);
        }
    }

    public abstract class BoundaryNeg : Boundary { // = the left downward curve 
        public BoundaryNeg(Site left, Site right) : base(left, right) {}
    }
    public abstract class BoundaryPos : Boundary {  // = the right upward curve
        public BoundaryPos(Site left, Site right) : base(left, right) {}
    }
    public abstract class BoundaryZero : Boundary { // = a straight upward line
        public BoundaryZero(Site left, Site right) : base(left, right) {}
    }
}
