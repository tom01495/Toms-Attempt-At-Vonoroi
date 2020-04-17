using System.Collections.Generic;
using FortunesAlgoritmGeometry;
using UnityEngine;

public class VonoroiDebugger : MonoBehaviour {

    public static void ShowCoordinates(List<Coordinates> tileCoordinates) {
        // Vector3 va = new Vector3(0, 0, 0);
        // Vector3 vb = new Vector3(20, 10, 0);
        // Boundary a = new Boundary(new Site(va.x, va.y), new Site(vb.x,vb.y));

        // Vector3 wa = new Vector3(10, 0, 0);
        // Vector3 wb = new Vector3(0, 10, 0);
        // Boundary b = new Boundary(new Site(wa.x, wa.y), new Site(wb.x,wb.y));

        // Debug.DrawLine(va, vb, Color.red, 10000000);
        // Debug.DrawLine(wa, wb, Color.blue, 1000000);

        // Vertex intersect = a.Intersection(b);
        // GameObject go = Instantiate(sphere);
        // go.transform.position = new Vector3(intersect.x, intersect.y, 0);
        // return;

        GameObject go = new GameObject();
        foreach(Coordinates c in tileCoordinates){
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = new Vector3(c.x, c.y, 0);
            obj.transform.parent = go.transform;
        }
    }

    public static void ShowBoundaries(List<Boundary> boundaries) {
        foreach(Boundary b in boundaries) {
            if(b?.Base?.x == null || b?.Base?.y == null) continue;
            if(b?.Summit?.x == null || b?.Summit?.y == null) continue;
            Debug.DrawLine(new Vector3(b.Base.x, b.Base.y, 0), new Vector3(b.Summit.x, b.Summit.y, 0), Color.blue, 10000000);
        }
    }

    public static void ShowBorders(List<BorderInit> borders) {
        foreach(BorderInit b in borders) {
            if(b?.CoordBase?.x == null || b?.CoordBase?.y == null) continue;
            if(b?.CoordSummit?.x == null || b?.CoordSummit?.y == null) continue;
            Debug.DrawLine(new Vector3(b.CoordBase.x, b.CoordBase.y, 0), new Vector3(b.CoordSummit.x, b.CoordSummit.y, 0), Color.red, 10000000);
        }
    }
}