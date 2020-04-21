using System;
using System.Collections.Generic;
using System.Linq;
using FortunesAlgoritmGeometry;
using UnityEngine;

// For all your testing needs. Go wild on this class! (but try to keep the others as in-piece as possible)
public class VonoroiDebugger : MonoBehaviour {

    // ================================== Setters

    private static List<Coordinates> SavedCoordinates(VonoroiModel model, bool getNew = false){
        List<Coordinates> tileCoordinates;
        if(getNew) {
            tileCoordinates = Coordinates.CreateRandomList(model.bounds, model.minDistanceTiles);

            int length = tileCoordinates.Count;
            PlayerPrefs.SetInt("length", length);
            for(int index = 0; index < length; index++) {
                PlayerPrefs.SetFloat(index + "x", tileCoordinates[index].x);
                PlayerPrefs.SetFloat(index + "y", tileCoordinates[index].y);
            }
        } else {
            tileCoordinates = new List<Coordinates>();

            int length = PlayerPrefs.GetInt("length");
            for(int index = 0; index < length; index++) {
                float x = PlayerPrefs.GetFloat(index + "x");
                float y = PlayerPrefs.GetFloat(index + "y");
                tileCoordinates.Add(new Coordinates(x,y));
            }
        }

        return tileCoordinates;
    }

    // ================================== Show Functions

    // public void ShowCoordinates(List<Coordinates> tileCoordinates) {
    //     GameObject go = new GameObject();
    //     foreach(Coordinates c in tileCoordinates) {
    //         GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //         obj.transform.position = new Vector3(c.x, c.y, 0);
    //         obj.transform.parent = go.transform;
    //         obj.name = c.ToString();
    //         spheres.Add(obj);
    //     }
    // }

    // List<GameObject> spheres = new List<GameObject>();

    // private void OnDrawGizmos() {
    //     foreach(GameObject sphere in spheres) {
    //         //drawString(sphere.name, sphere.transform.position);
    //     }
    // }

    private static void drawString(string text, Vector3 worldPos, Color? colour = null) {
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

            // Setting the base and summit points
            if(C.Base?.x != null && C.Summit?.x != null) { // Perfect line
                lineColor = Color.green;
                start = new Vector3(C.Base.x, C.Base.y, 0);
                end = new Vector3(C.Summit.x, C.Summit.y, 0); 
            }
            else if(C.Base?.x != null) { lineColor = Color.blue; } // Only the base is set
            else if(C.Summit?.x != null) { lineColor = Color.red; } // Only the summit is set

            Debug.DrawLine(start, end, lineColor, duration); 
        }
    }

    // ================================== Step By Step

    // private Action<List<Point>, List<PointSet>> step;
    // private List<Point> Q;
    // private List<PointSet> T;
    // private List<Boundary> V;

    // public void StepByStep(Action<List<Point>, List<PointSet>> step, List<Point> Q, List<PointSet> T, List<Boundary> V){
    //     this.step = step;
    //     this.Q = Q;
    //     this.T = T;
    //     this.V = V;
    // }

    // private void Update() {
    //     if(step != null) {
    //         if(Input.GetKey(KeyCode.Period)) { //
    //             if(Q.Count == 0) { Debug.Log("Q is empty!"); }
    //             else {
    //                 Debug.Log("Next step");
    //                 step(Q, T);
    //             }
    //         }
    //         ShowBoundaries(T.OfType<Boundary>().ToList(), Time.deltaTime);
    //         ShowBoundaries(V, Time.deltaTime);
    //     }
    // }
}