using System;
using System.Collections.Generic;
using UnityEngine;

// THIS NAMESPACE SHOULD ONLY BE USED FOR THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry
{
    public class Point : Coordinates, IComparable<Point> // Abstact and free points in R2
    {
        public Point(float x, float y) : base(x, y) {}

        public float Dist(Point p) { // Euclidean distance to p
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

    public class Vertex : Point { // A vonoroi vertex lies between 3 sites
        public Vertex(float x, float y) : base(x, y) {}
    }

    public class Site : Point { // A site is a predetermined point in R2
        public Site(float x, float y) : base(x, y) {}
    }
}