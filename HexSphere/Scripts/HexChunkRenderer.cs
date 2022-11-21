using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteAlways]
public class HexChunkRenderer : MonoBehaviour
{
    [HideInInspector]
    public int _renderedChunkId;
    [System.NonSerialized]
    private HexPlanet _planet;
    [System.NonSerialized]
    private HexChunk _chunk;
    
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    public HexChunk GetHexChunk() {
        if(_chunk == null) {
            _chunk = _planet.GetChunk(_renderedChunkId);
        }
        return _chunk;
    }

    public void SetHexChunk(HexPlanet planet, int chunkId) {
        _planet = planet;
        _renderedChunkId = chunkId;
    }

    public void Start() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _planet = GetComponentInParent<HexPlanetRenderer>().hexPlanet;
        _planet.GetChunk(_renderedChunkId).onChunkChange += OnChunkChange;
        UpdateMesh();
    }

    public void Update() {
        if(Application.isPlaying) {
            if(GetHexChunk().isDirty) {
                UpdateMesh();
                GetHexChunk().isDirty = false;
            }
            }
    }

    public void OnChunkChange(HexTile changedTile) {
        _planet.GetChunk(changedTile.chunkId).MakeDirty();
        foreach (HexTile nTile in changedTile.GetNeighbors())
        {
            if (changedTile.chunkId != nTile.chunkId) {
                _planet.GetChunk(nTile.chunkId).MakeDirty();
            }
        }
    }

    public void UpdateMesh()
    {
        if(_meshFilter == null) {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        Mesh newChunkMesh = GetHexChunk().GetMesh();
        _meshFilter.mesh = newChunkMesh;
        _meshCollider.sharedMesh = newChunkMesh;
    }

}
