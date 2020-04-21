using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VonoroiView : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void ShowVonoroi(VonoroiModel model) {
        //ShowBorders(model.borderInits);
        ShowTiles(model.tileInits);
    }

    private void ShowTiles(List<TileInit> tiles) {
        GameObject go = new GameObject();
        foreach(TileInit tile in tiles) {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = new Vector3(tile.x, tile.y, 0);
            obj.transform.parent = go.transform;
        }
    }

    private void ShowBorders(List<BorderInit> borders) {
        foreach(BorderInit border in borders) {
            float divY2 = border.LeftTile.y - border.RightTile.y;
            bool lineIsStraight = Mathf.Approximately(divY2,0);

            float xMin = -1000;
            float xMax = 1000;
            float yMin = -1000;
            float yMax = 1000;

             // (for y = m*x + c)
            float m = -1;
            float c = -1;
            if(!lineIsStraight) {
                m = -(border.LeftTile.x - border.RightTile.x)/divY2;
                c = ((border.LeftTile.y + border.RightTile.y) / 2f) - m*((border.RightTile.x + border.LeftTile.x) / 2f);
            }

            Vector3 start;
            Vector3 end;
            Color lineColor = Color.black;

            // Setting the startpoint <=
            if(!lineIsStraight) { start = new Vector3(xMin, m*xMin + c, 0); }
            else { start = new Vector3((border.RightTile.x + border.LeftTile.x) / 2f,yMin, 0); }

            // Setting the endpoint =>
            if(!lineIsStraight) { end = new Vector3(xMax, m*xMax + c, 0); }
            else { end = new Vector3((border.RightTile.x + border.LeftTile.x) / 2f, yMax, 0); }

            // Setting the base and summit points
            if(border.Base != null && border.Summit != null) { // Perfect line
                lineColor = Color.green;
                start = new Vector3(border.Base.x, border.Base.y, 0);
                end = new Vector3(border.Summit.x, border.Summit.y, 0); 
            }
            else if(border.Base != null) { lineColor = Color.blue; } // Only the base is set
            else if(border.Summit != null) { lineColor = Color.red; } // Only the summit is set

            Debug.DrawLine(start, end, lineColor, float.MaxValue); 
        }
    }
}
