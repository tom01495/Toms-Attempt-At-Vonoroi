using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FortunesAlgoritmGeometry; //TODO: GET RID OF ME

public class VonoroiController : MonoBehaviour
{
    public GameObject sphere;
    // Start is called before the first frame update
    void Start() {
        VonoroiModel model = gameObject.GetComponent<VonoroiModel>();
        VonoroiView view = gameObject.GetComponent<VonoroiView>();

        //view.ShowLoadingbar() // :p
        StartFortunesAlgorithm(model);
        view.ShowVonoroi(model);
    }

    private void StartFortunesAlgorithm(VonoroiModel model) {
        // INPUT
        List<Coordinates> tileCoordinates = Coordinates.CreateRandomList(
            model.bounds,
            model.amountTiles,
            model.minDistanceTiles);
        FortunesAlgorithm algorithm = new FortunesAlgorithm(tileCoordinates);

        DebugVonoroi(tileCoordinates);

        // IMPROVE
        algorithm.RemoveShortBoundaries(model.minBoundaryLength);
        algorithm.MakeBoundariesWiggely();
        algorithm.CutCorners(model.bounds);

        // OUTPUT
        (model.borderInits, model.tileInits) = algorithm.GetBordersAndTiles();
    }

    private void DebugVonoroi(List<Coordinates> tileCoordinates) {
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
            GameObject obj = Instantiate(sphere);
            obj.transform.position = new Vector3(c.x, c.y, 0);
            obj.transform.parent = go.transform;
        }
    }
}
