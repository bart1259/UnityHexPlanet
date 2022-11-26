using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityHexPlanet {

    [System.Serializable]
    public class HexTile
    {
        public delegate void OnTileChangeHandler (HexTile tile);
        public event OnTileChangeHandler onTileChange;

        public int id;
        [HideInInspector]
        public List<int> neighborIDs;
        [HideInInspector]
        public int chunkId;
        public Vector3 center;
        public List<Vector3> vertices;
        public float height;
        [System.NonSerialized]
        public HexPlanet planet;
        public Color32 color;

        [System.NonSerialized]
        private List<HexTile> _neighbors;

        public HexTile(int id, HexPlanet planet, Vector3 center, List<Vector3> verts) {
            this.id = id;
            this.planet = planet;
            this.center = center;
            this.vertices = verts;
            this.height = 0.0f;

            neighborIDs = new List<int>();
        }

        public virtual void AppendToMesh(List<Vector3> meshVerts, List<int> meshIndicies, List<Color32> meshColors) {
            // Generate hexagon base
            int baseIndex = meshVerts.Count;
            meshVerts.Add(TransformPoint(center, height));
            meshColors.Add(color);
            for (int j = 0; j < vertices.Count; j++)
            {
                meshVerts.Add(TransformPoint(vertices[j], height));
                meshColors.Add(color);

                meshIndicies.Add(baseIndex);
                meshIndicies.Add(baseIndex + j + 1);
                meshIndicies.Add(baseIndex + ((j + 1) % vertices.Count) + 1);
            }

            // Generate walls if needed
            List<HexTile> neighbors = GetNeighbors();
            for (int j = 0; j < neighbors.Count; j++)
            {
                float thisHeight = height;
                float otherHeight = neighbors[j].height;
                if(otherHeight < thisHeight) {
                    baseIndex = meshVerts.Count;
                    // Add barrier
                    meshVerts.Add(TransformPoint(vertices[(j + 1) % vertices.Count], otherHeight));
                    meshVerts.Add(TransformPoint(vertices[j], otherHeight));
                    meshVerts.Add(TransformPoint(vertices[(j + 1) % vertices.Count], thisHeight));
                    meshVerts.Add(TransformPoint(vertices[j], thisHeight));

                    meshColors.Add(color);
                    meshColors.Add(color);
                    meshColors.Add(color);
                    meshColors.Add(color);

                    meshIndicies.Add(baseIndex);
                    meshIndicies.Add(baseIndex + 2);
                    meshIndicies.Add(baseIndex + 1);

                    meshIndicies.Add(baseIndex + 2);
                    meshIndicies.Add(baseIndex + 3);
                    meshIndicies.Add(baseIndex + 1);
                }
            }
        }


        private Vector3 TransformPoint(Vector3 input, float height) {
            return input * (1 + (height / planet.radius));
        }

        public void SetChunk(int chunkId) {
            this.chunkId = chunkId;
        }

        public void AddNeighbors(List<HexTile> nbrs) {
            for (int i = 0; i < nbrs.Count; i++)
            {
                neighborIDs.Add(nbrs[i].id);
            }
        }

        public void SetHeight(float newHeight) {
            height = newHeight;
            TriggerMeshRecompute();
        }

        public List<HexTile> GetNeighbors() {
            if(_neighbors == null) {
                _neighbors = new List<HexTile>();
                for (int i = 0; i < neighborIDs.Count; i++)
                {
                    _neighbors.Add(planet.GetTile(neighborIDs[i]));
                }
            }
            return _neighbors;
        }

        protected void TriggerMeshRecompute() {
            if(onTileChange != null) {
                onTileChange(this);
            }
        }
    }
}