using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityHexPlanet {

    [System.Serializable]
    public class BaseTerrainGenerator : ScriptableObject
    {
        public virtual HexTile CreateHexTile(int id, HexPlanet planet, Vector3 centerPosition, List<Vector3> verts) {
            return new HexTile(id, planet, centerPosition, verts);
        }

        public virtual void AfterTileCreation(HexTile newTile) { }
    }
}