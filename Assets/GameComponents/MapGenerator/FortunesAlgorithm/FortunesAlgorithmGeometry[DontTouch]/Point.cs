using System;
using System.Collections.Generic;
using UnityEngine;

// THIS NAMESPACE SHOULD ONLY BE USED By THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry
{
    public class Point : Coordinates, IComparable<Point>
    {
        public Point(float x, float y) : base(x, y) {}

        public float Dist(Point p) {
            return (float)Math.Sqrt((x - p.x)*(x - p.x) + (y - p.y)*(y - p.y));
        }

        public int CompareTo(Point p) { // A point is lesser if it has a smaller y value
            if(Mathf.Approximately(y, p.y) && Mathf.Approximately(x, p.x)) return 0;
            else if (y > p.y || (Mathf.Approximately(y, p.y) && x > p.x)) return 1;
            else return -1;
        }
    }

    public class Vertex : Point {
        private Boundary leftBoundary;
        public Boundary LeftBoundary { get { return leftBoundary; } }
        private Boundary rightBoundary;
        public Boundary RightBoundary { get { return rightBoundary; } }

        public Vertex(float x, float y, Boundary leftBoundary, Boundary rightBoundary) : base(x, y) {
            this.leftBoundary = leftBoundary;
            this.rightBoundary = rightBoundary;
        }
    }

    public class Site : Point {
        public List<Site> neigbhours = new List<Site>();
        
        public Site(float x, float y) : base(x, y) {}
    }
}

// I have put both of these in the Coord class!!

// public override string ToString(){
//     return "(" + valueX.ToString() + "," + valueY.ToString() + ")";
// }

// public override bool Equals(object obj){
//     if(typeof(Site).IsInstanceOfType(obj)){
//         Site s = obj as Site;
//         return Mathf.Approximately(x, s.x) && Mathf.Approximately(y, s.y);
//     }
//     return false;
// }