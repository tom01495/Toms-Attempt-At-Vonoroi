using System;
using System.Collections.Generic;

// THIS NAMESPACE SHOULD ONLY BE USED FOR THE ALGORITHM!!!
namespace FortunesAlgoritmGeometry
{
    public interface PointSet {}    // A set of infinite points. This can either be a line, area, etc.
    public class Region : PointSet { // TODO Make sure these can be transformed to tiles!
        public Site site;

        public Region(Site p) {
            site = p;
        }

        public static Region InRegion(List<Region> regions, Point p) {
            float minLen = float.MaxValue;
            Region minRegion = null; 

            foreach(Region R in regions) {
                float distance = p.Dist(R.site)/2;
                float angle = (float)(Math.Tanh((p.x - R.site.x) / (p.y - R.site.y)));
                float len = distance / (float)Math.Cos(angle);
                if(len < minLen) {
                    minLen = len;
                    minRegion = R;
                }
            }

            return minRegion;
        }

        public UnsetTileInit CreateUnsetTile() {
            return new UnsetTileInit(site);
        }
    }
}

// public bool Includes(Point p) {
//     float angle = (float)(Math.Tanh((p.x - site.x) / (p.y - site.y))); // the downward angle
//     float distance = (p.Dist(site)/2) / (float)Math.Cos(angle); // the distance between p and the site
//     //return (new Point(p.x, new_y)).ClosestSite() == p; // TODO
//     throw new Exception();
// }