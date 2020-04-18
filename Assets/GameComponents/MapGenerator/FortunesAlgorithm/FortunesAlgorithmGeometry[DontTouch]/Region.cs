using System;
using System.Collections.Generic;

// THIS NAMESPACE SHOULD ONLY BE USED BY THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry
{
    public interface PointSet {} 

    public class Region : PointSet { // TODO Make sure these can be transformed to tiles!
        private Site site;
        public Site Site { get { return site; } }

        // =============================== Constructors

        public Region(Site p) {
            site = p;
        }

        // =============================== Functions

        public static Region InRegion(List<Region> regions, Point p) {
            float minLen = float.MaxValue;
            Region minRegion = null; 

            foreach(Region R in regions) {
                if((R as RegionSection) != null) { if(!(R as RegionSection).InSection(p)) continue; }

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
            if(typeof(Region).IsInstanceOfType(obj)) { 
                return this.Site.Equals((obj as Region).Site); 
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

        public virtual RegionSection Left(Region regionRight) => RegionSection.CreateLeft(regionRight.Site);
        public virtual RegionSection Right(Region regionLeft) => RegionSection.CreateRight(regionLeft.Site);
        public Region Normal() => new Region(Site);
    }

    public class RegionSection : Region {
        protected float xMin = float.MinValue;
        protected float xMax = float.MaxValue;

        public static RegionSection CreateLeft(Site p) {
            return new RegionSection(p){xMax = p.x};
        }

        public static RegionSection CreateRight(Site p) {
            return new RegionSection(p){xMin = p.x};
        }

        private RegionSection(Site p) : base(p) {}

        public bool InSection(Point p) => xMin <= p.x && p.x <= xMax;

        // =============================== Conversion

        public override RegionSection Left(Region regionRight) {
            RegionSection Rs = RegionSection.CreateLeft(regionRight.Site);
            Rs.xMin = this.xMin;
            return Rs;
        }

        public override RegionSection Right(Region regionLeft) {
            RegionSection Rs = RegionSection.CreateRight(regionLeft.Site);
            Rs.xMax = this.xMax;
            return Rs;
        }
    }
}
