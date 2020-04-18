using System;
using System.Collections.Generic;

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

        public UnsetTileInit CreateUnsetTile() {
            return new UnsetTileInit(Site);
        }
    }
}
