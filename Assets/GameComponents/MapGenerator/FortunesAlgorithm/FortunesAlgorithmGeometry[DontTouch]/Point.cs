using System;
using System.Collections.Generic;
using UnityEngine;

// THIS NAMESPACE SHOULD ONLY BE USED FOR THE ALGORITHM!!!
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
            else if (y < p.y || (Mathf.Approximately(y, p.y) && x < p.x)) return -1;
            else return 1;
        }

        public Point Star(Site s) {
            return new Point(x, y + Dist(s));
        }
    }

    public class Vertex : Point {
        public Vertex(float x, float y) : base(x, y) {}
    }

    public class Site : Point {
        public Site(float x, float y) : base(x, y) {}
    }
}