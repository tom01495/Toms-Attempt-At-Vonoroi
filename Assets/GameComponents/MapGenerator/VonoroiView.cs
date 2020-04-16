using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VonoroiView : MonoBehaviour
{
    private GameObject borderObject;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start() {
        borderObject = new GameObject();
        lineRenderer = borderObject.AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 1;
        lineRenderer.endWidth = 1;
    }

    public void ShowVonoroi(VonoroiModel model) {
        ShowBorders(model.borderInits);
    }

    public void ShowBorders(List<BorderInit> borders) {
        foreach(BorderInit b in borders) {
            if(b?.CoordBase?.x == null || b?.CoordBase?.y == null) continue;
            if(b?.CoordSummit?.x == null || b?.CoordSummit?.y == null) continue;
            Debug.DrawLine(new Vector3(b.CoordBase.x, b.CoordBase.y, 0), new Vector3(b.CoordSummit.x, b.CoordSummit.y, 0), Color.red, 10000000);
            // lineRenderer.SetPosition(0, new Vector3(b.CoordBase.x, b.CoordBase.y, 0));
            // lineRenderer.SetPosition(1, new Vector3(b.CoordSummit.x, b.CoordSummit.y, 0));
            // Instantiate(lineRenderer);
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
