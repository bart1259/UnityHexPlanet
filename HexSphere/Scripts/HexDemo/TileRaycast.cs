using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityHexPlanet {

    public class TileRaycast : MonoBehaviour
    {
        private int _raycastMask;
        private HashSet<HexTile> _selectedHexTiles;
        private List<GameObject> _cursors;

        private float _targetGUIHeight = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            _raycastMask = LayerMask.GetMask("HexPlanet");
            _selectedHexTiles = new HashSet<HexTile>();
            _cursors = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast (ray, out hit, 10000, _raycastMask)) {
                HexChunkRenderer hcr = hit.transform.gameObject.GetComponent<HexChunkRenderer>();
                Vector3 planetPosition = hcr.transform.position;
                if(hcr != null) {
                    HexChunk hc = hcr.GetHexChunk();
                    if(hc != null) {
                        HexTile tile = hc.GetClosestTileAngle(hit.point - planetPosition);

                        LineRenderer lr = GetComponent<LineRenderer>();
                        List<Vector3> points = new List<Vector3>();
                        for (int i = 0; i < tile.vertices.Count; i++)
                        {
                            points.Add(tile.vertices[i] + (tile.center.normalized * tile.height) + planetPosition);
                        }
                        lr.positionCount = points.Count;
                        lr.SetPositions(points.ToArray());

                        if (Input.GetMouseButtonDown(0) && !(Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))) {
                            _selectedHexTiles.Clear();
                            resetCursors();
                        }

                        if(Input.GetMouseButton(0)) {
                            
                            if(Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) {
                                _selectedHexTiles.Remove(tile);
                                resetCursors();
                                return;
                            }

                            if(_selectedHexTiles.Add(tile)) {
                                // Add cursor
                                GameObject newCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                newCursor.transform.SetParent(transform);
                                newCursor.transform.position = tile.center + (tile.center.normalized * tile.height) + planetPosition;

                                _cursors.Add(newCursor);
                            }
                            _targetGUIHeight = tile.height;
                            Debug.Log(_selectedHexTiles.Count); 
                        }

                    }
                }
            }
        }

        void OnGUI () {
            // Make a background box
        
            // // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
            // if(GUI.Button(new Rect(20,40,80,20), "Level 1")) {
            //     Application.LoadLevel(1);
            // }
        
            // // Make the second button.
            // if(GUI.Button(new Rect(20,70,80,20), "Level 2")) {
            //     Application.LoadLevel(2);
            // }

            GUI.Label(new Rect(25,5,200,50), "Selected: " + _selectedHexTiles.Count + " tile" + (_selectedHexTiles.Count == 1 ? "" : "s"));
            float newTargetGUIHeight = GUI.HorizontalSlider (new Rect (25, 25, 100, 15), _targetGUIHeight, -2.0f, 15.0f);
            if(newTargetGUIHeight != _targetGUIHeight) {
                foreach (HexTile tile in _selectedHexTiles) {
                    tile.SetHeight(newTargetGUIHeight);
                }
            }
            _targetGUIHeight = newTargetGUIHeight;

            if(GUI.Button(new Rect(25, 45, 30, 20), "+")) {
                foreach (HexTile tile in _selectedHexTiles) {
                    tile.SetHeight(tile.height + 1);
                }
            }
            if(GUI.Button(new Rect(60, 45, 30, 20), "-")) {
                foreach (HexTile tile in _selectedHexTiles) {
                    tile.SetHeight(tile.height - 1);
                }
            }
        }

        private void resetCursors() {
            foreach (GameObject cursor in _cursors) {
                GameObject.Destroy(cursor);
            }
            foreach (HexTile tile in _selectedHexTiles) {
                GameObject newCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newCursor.transform.SetParent(transform);
                newCursor.transform.position = tile.center + (tile.center.normalized * tile.height);
                _cursors.Add(newCursor);
            }
        }
    }
}