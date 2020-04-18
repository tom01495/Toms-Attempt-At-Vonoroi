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
        //VonoroiDebugger.ShowBorders(model.borderInits);
        //view.ShowVonoroi(model);
    }

    private void StartFortunesAlgorithm(VonoroiModel model) {
        // INPUT
        List<Coordinates> tileCoordinates = Coordinates.CreateRandomList(
            model.bounds,
            model.amountTiles,
            model.minDistanceTiles);

        // DEBUGGER
        VonoroiDebugger.ShowCoordinates(tileCoordinates); // TODO remove this
        VonoroiDebugger debugger = gameObject.GetComponent<VonoroiDebugger>(); // TODO and this

        
        FortunesAlgorithm algorithm = new FortunesAlgorithm(tileCoordinates, debugger);

        // IMPROVE
        algorithm.RemoveShortBoundaries(model.minBoundaryLength);
        algorithm.MakeBoundariesWiggely();
        algorithm.CutCorners(model.bounds);

        // OUTPUT
        (model.borderInits, model.tileInits) = algorithm.GetBordersAndTiles();
    }
}
