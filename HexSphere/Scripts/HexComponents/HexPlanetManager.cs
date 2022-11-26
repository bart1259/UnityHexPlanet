using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityHexPlanet {

    [ExecuteAlways]
    public class HexPlanetManager : MonoBehaviour
    {
        [SerializeField] 
        public HexPlanet hexPlanet;
        private HexPlanet _prevHexPlanet;

        // Called when the whole sphere must be regenerated
        public void UpdateRenderObjects()
        {
            // Delete all children
            foreach (Transform child in transform) {
                StartCoroutine(Destroy(child.gameObject));
            }

            if(hexPlanet == null) {
                return;
            }

            HexPlanetHexGenerator.GeneratePlanetTilesAndChunks(hexPlanet);

            for (int i = 0; i < hexPlanet.chunks.Count; i++)
            {
                GameObject chunkGO = new GameObject("Chunk " + i);
                chunkGO.transform.SetParent(transform);
                chunkGO.transform.localPosition = Vector3.zero;
                MeshFilter mf = chunkGO.AddComponent<MeshFilter>();
                MeshCollider mc = chunkGO.AddComponent<MeshCollider>();

                MeshRenderer mr = chunkGO.AddComponent<MeshRenderer>();
                mr.sharedMaterial = hexPlanet.chunkMaterial;

                HexChunkRenderer hcr = chunkGO.AddComponent<HexChunkRenderer>();
                hcr.SetHexChunk(hexPlanet, i);
                hcr.UpdateMesh();

                // Set layer
                int hexPlanetLayer = LayerMask.NameToLayer("HexPlanet");
                if (hexPlanetLayer==-1){
                    throw new UnassignedReferenceException("Layer \"HexPlanet\" must be created in the Layer Manager!");
                }
                chunkGO.layer = hexPlanetLayer;
            }   
        }

        IEnumerator Destroy(GameObject go)
        {
            yield return new WaitForSeconds(0.1f);
            DestroyImmediate(go);
        }
    }
}