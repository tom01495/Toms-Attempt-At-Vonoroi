using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VonoroiController : MonoBehaviour
{    
    // Start is called before the first frame update
    void Start() {
        VonoroiModel model = gameObject.GetComponent<VonoroiModel>();
        VonoroiView view = gameObject.GetComponent<VonoroiView>();

        //view.ShowLoadingbar() // :p
        StartFortunesAlgorithm(model);
        view.ShowVonoroi(model);
    }

    private void StartFortunesAlgorithm(VonoroiModel model) {
        List<Coordinates> tileCoordinates = Coordinates.CreateRandomList(model.bounds, model.minDistanceTiles);
        FortunesAlgorithm algorithm = new FortunesAlgorithm(tileCoordinates);

        algorithm.RemoveShortBoundaries(model.minBoundaryLength);
        algorithm.MakeBoundariesWiggely();
        algorithm.CutCorners(model.bounds);

        (model.borderInits, model.tileInits) = algorithm.GetBordersAndTiles();
    }
}
