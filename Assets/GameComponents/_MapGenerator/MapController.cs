using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {    
    // Start is called before the first frame update
    void Start() {
        MapModel model = gameObject.GetComponent<MapModel>();
        MapView view = gameObject.GetComponent<MapView>();

        //view.ShowLoadingbar() // :p
        StartFortunesAlgorithm(model);
        view.ShowVonoroi(model);
    }

    private void StartFortunesAlgorithm(MapModel model) {
        List<Vector2> tileCoordinates = PossionDiscSampling.CreateRandomList(model.bounds, model.minDistanceTiles);
        FortunesAlgorithm algorithm = new FortunesAlgorithm(tileCoordinates);

        algorithm.RemoveShortBoundaries(model.minBoundaryLength);
        algorithm.MakeBoundariesWiggely();
        algorithm.CutCorners(model.bounds);

        (model.borderInits, model.tileInits) = algorithm.GetBordersAndTiles();
    }
}
