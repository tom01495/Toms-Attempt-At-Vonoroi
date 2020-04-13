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
        lineRenderer.startWidth = 10;
        lineRenderer.endWidth = 10;
    }

    public void ShowVonoroi(VonoroiModel model) {
        ShowBorders(model.borderInits);
    }

    public void ShowBorders(List<BorderInit> borders) {
        foreach(BorderInit b in borders) {
            lineRenderer.SetPosition(0, new Vector3(b.CoordBase.x, b.CoordBase.y, 0));
            lineRenderer.SetPosition(1, new Vector3(b.CoordSummit.x, b.CoordSummit.y, 0));
            Instantiate(borderObject);
        }
    }
 
    // Update is called once per frame
    void Update() {
        
    }
}
