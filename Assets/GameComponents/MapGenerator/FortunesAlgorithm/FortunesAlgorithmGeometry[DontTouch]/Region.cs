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

        public Region(Site p) {
            site = p;
        }

        public static Region InRegion(List<Region> regions, Point p) {
            float minLen = float.MaxValue;
            Region minRegion = null; 

            foreach(Region R in regions) {
                if((R as RegionSection) != null) {
                    if(!(R as RegionSection).InSection(p)) continue;
                }

                float distance = p.Dist(R.Site)/2;
                float angle = (float)(Math.Tanh((p.x - R.Site.x) / (p.y - R.Site.y)));
                float len = distance / (float)Math.Cos(angle);
                if(len < minLen) {
                    minLen = len;
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

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        // =============================== Conversion

        public UnsetTileInit CreateUnsetTile() {
            return new UnsetTileInit(Site);
        }

        protected float xMin = float.MinValue;
        protected float xMax = float.MaxValue;

        public RegionSection Left(Region regionRight) => new RegionSection(this){xMax = regionRight.Site.x};
        public RegionSection Right(Region regionLeft) => new RegionSection(this){xMin = regionLeft.Site.x};
        public Region Normal() => new Region(Site);
    }

    public class RegionSection : Region {

        public RegionSection(Region R) : base(R.Site) {}

        public bool InSection(Point p) => xMin <= p.x && p.x <= xMax;
    }
}
