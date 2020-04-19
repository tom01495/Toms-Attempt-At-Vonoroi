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
        //List<Coordinates> tileCoordinates = Coordinates.CreateRandomList(model.bounds, model.minDistanceTiles);
        List<Coordinates> tileCoordinates = SavedCoordinates(model);

        // DEBUGGER
        VonoroiDebugger debugger = gameObject.GetComponent<VonoroiDebugger>(); // TODO and this
        debugger.ShowCoordinates(tileCoordinates);

        FortunesAlgorithm algorithm = new FortunesAlgorithm(tileCoordinates, debugger);

        // IMPROVE
        algorithm.RemoveShortBoundaries(model.minBoundaryLength);
        algorithm.MakeBoundariesWiggely();
        algorithm.CutCorners(model.bounds);

        // OUTPUT
        (model.borderInits, model.tileInits) = algorithm.GetBordersAndTiles();
    }

    private List<Coordinates> SavedCoordinates(VonoroiModel model, bool getNew = true){
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
}
