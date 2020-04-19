using System;
using System.Collections.Generic;
using System.Linq;
using FortunesAlgoritmGeometry;
using UnityEngine;

// For all your testing needs. Go wild on this class! (but try to keep the others as in-piece as possible)
public class VonoroiDebugger : MonoBehaviour {

    public void ShowCoordinates(List<Coordinates> tileCoordinates) {
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
        foreach(Coordinates c in tileCoordinates) {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = new Vector3(c.x, c.y, 0);
            obj.transform.parent = go.transform;
            obj.name = c.ToString();
            spheres.Add(obj);
        }
    }

    List<GameObject> spheres = new List<GameObject>();

    private void OnDrawGizmos() {
        foreach(GameObject sphere in spheres) {
            //drawString(sphere.name, sphere.transform.position);
        }
    }

    static void drawString(string text, Vector3 worldPos, Color? colour = null) {
		UnityEditor.Handles.BeginGUI();
		if (colour.HasValue) GUI.color = colour.Value;
		var view = UnityEditor.SceneView.currentDrawingSceneView;
		Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
		Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
		GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
		UnityEditor.Handles.EndGUI();
	}

    public static void ShowBoundaries(List<Boundary> boundaries, float duration) {
        foreach(Boundary C in boundaries) {
            float divY2 = C.LeftSite.y - C.RightSite.y;
            bool lineIsStraight = Mathf.Approximately(divY2,0);

            float xMin = -1000;
            float xMax = 1000;
            float yMin = -1000;
            float yMax = 1000;

             // (for y = m*x + c)
            float m = -1;
            float c = -1;
            if(!lineIsStraight) {
                m = -(C.LeftSite.x - C.RightSite.x)/divY2;
                c = ((C.LeftSite.y + C.RightSite.y) / 2f) - m*((C.RightSite.x + C.LeftSite.x) / 2f);
            }

            Vector3 start;
            Vector3 end;
            Color lineColor = Color.black;

            // Setting the startpoint <=
            if(!lineIsStraight) { start = new Vector3(xMin, m*xMin + c, 0); }
            else { start = new Vector3((C.RightSite.x + C.LeftSite.x) / 2f,yMin, 0); }

            // Setting the endpoint =>
            if(!lineIsStraight) { end = new Vector3(xMax, m*xMax + c, 0); }
            else { end = new Vector3((C.RightSite.x + C.LeftSite.x) / 2f, yMax, 0); }

            float xMiddle = (C.RightSite.x + C.LeftSite.x) / 2f;

            // Setting the base and summit points
            if(C.Base?.x != null && C.Summit?.x != null) { // Perfect line
                lineColor = Color.green;

                start = new Vector3(C.Base.x, C.Base.y, 0);
                end = new Vector3(C.Summit.x, C.Summit.y, 0); 
            }
            else if(C.Base?.x != null) { // Only the base is set
                lineColor = Color.blue;
                // Gizmos.color = Color.blue;
                // Gizmos.DrawSphere(new Vector3(C.Base.x, C.Base.y, 0), 2);

                // Vector3 base_ = new Vector3(C.Base.x, C.Base.y, 0);
                // if(xMiddle > C.Base.x) { start = base_; }
                // else { end = base_; }
            }
            else if(C.Summit?.x != null) { // Only the summit is set
                lineColor = Color.red;
                // Gizmos.color = Color.red;
                // Gizmos.DrawSphere(new Vector3(C.Summit.x, C.Summit.y, 0), 2);

                // Vector3 summit = new Vector3(C.Summit.x, C.Summit.y, 0);
                // if(xMiddle > C.Summit.x) { start = summit; }
                // else { end = summit; }
            }

            Debug.DrawLine(start, end, lineColor, duration); 
        }
    }

    public static void ShowBorders(List<BorderInit> borders, float duration) {
        foreach(BorderInit b in borders) {
            if(b?.CoordBase?.x == null || b?.CoordBase?.y == null) continue;
            if(b?.CoordSummit?.x == null || b?.CoordSummit?.y == null) continue;

            Vector3 start = new Vector3(b.CoordBase.x, b.CoordBase.y, 0);
            Vector3 end = new Vector3(b.CoordSummit.x, b.CoordSummit.y, 0);

            Debug.DrawLine(start, end, Color.red, duration);
        }
    }

    // ================================== Step By Step

    private Action<List<Point>, List<PointSet>> step;
    private List<Point> Q;
    private List<PointSet> T;
    private List<Boundary> V;

    public void StepByStep(Action<List<Point>, List<PointSet>> step, List<Point> Q, List<PointSet> T, List<Boundary> V){
        this.step = step;
        this.Q = Q;
        this.T = T;
        this.V = V;
    }

    private void Update() {
        if(step != null) {
            if(true) { //Input.GetKey(KeyCode.Period)
                if(Q.Count == 0) {
                    //Debug.Log("Q is empty!"); 
                }
                else {
                    //Debug.Log("Next step");
                    //String insideQ = "Q = {";
                    //Q.ForEach(p => insideQ += p.ToString());
                    //Debug.Log(insideQ + "}");
                    
                    step(Q, T);
                }
            }
            ShowBoundaries(T.OfType<Boundary>().ToList(), Time.deltaTime);
            ShowBoundaries(V, Time.deltaTime);
        }
    }
}