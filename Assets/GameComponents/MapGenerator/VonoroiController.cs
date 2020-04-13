﻿using System.Collections;
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
        // INPUT
        List<Coordinates> tileCoordinates = Coordinates.CreateRandomList(
            model.bounds, 
            model.amountTiles, 
            model.minDistanceTiles);
        FortunesAlgorithm algorithm = new FortunesAlgorithm(
            tileCoordinates, 
            model.bounds);

        // IMPROVE
        algorithm.RemoveShortBoundaries(model.minBoundaryLength);

        // OUTPUT
        (model.borderInits, model.tileInits) = algorithm.GetBordersAndTiles();
    }
}