using System;
using System.Collections.Generic;
using UnityEngine;

// THIS NAMESPACE SHOULD ONLY BE USED BY THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry
{
    public interface PointSet {} 

    public class Region : PointSet { // TODO Make sure these can be transformed to tiles!
        private Site site;
        public Site Site { get { return site; } }

        public Region(Site p) { site = p; }

        public static Region InRegion(List<Region> regions, Point p) {
            float minDis = float.MaxValue;
            Region minRegion = null; 

            foreach(Region R in regions) {
                if(!R.InSection(p)) continue;

                // (for y = (x - h)^2)/4p + k
                float p_ = (R.site.y - p.y)/2;
                float h = R.site.x;

                float distance;
                if(p_ == 0) distance = 0;
                else distance = ((p.x - h)*(p.x - h))/(4*p_) + p_;

                if(distance < minDis) {
                    minDis = distance;
                    minRegion = R;
                }
            }

            return minRegion;
        }

        // =============================== Overrides

        public override bool Equals(object obj) {
            Region R = obj as Region;

            if(R != null) { 
                return Site.Equals(R.Site) && Mathf.Approximately(xMin, R.xMin) && Mathf.Approximately(xMax, R.xMax);
            }
            return false;
        }

        public override int GetHashCode() => base.GetHashCode();

        // =============================== Conversion

        protected float xMin = float.MinValue;
        protected float xMax = float.MaxValue;

        public virtual bool InSection(Point p) => true;

        public RegionSection Left(Region regionRight) => new RegionSection(this){xMax = regionRight.Site.x, xMin = this.xMin};
        public RegionSection Right(Region regionLeft) => new RegionSection(this){xMin = regionLeft.Site.x, xMax = this.xMax};
    }

    public class RegionSection : Region {
        public RegionSection(Region R) : base(R.Site) {}

        public override bool InSection(Point p) => xMin <= p.x && p.x <= xMax;
    }
}
