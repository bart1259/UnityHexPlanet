using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexChunk
{
    public int id;
    public Vector3 origin;
    public List<int> tileIds;
    public bool isDirty;

    public delegate void OnChunkChangeHandler(HexTile tile);
    public event OnChunkChangeHandler onChunkChange;

    [System.NonSerialized]
    public HexPlanet planet;
    [System.NonSerialized]
    private List<HexTile> _tiles;

    public HexChunk(int id, HexPlanet planet, Vector3 origin) {
        tileIds = new List<int>();
        this.origin = origin;
        this.planet = planet;
        this.id = id;
    }

    public void AddTile(int tileID) {
        tileIds.Add(tileID);
    }

    private void OnChunkTileChange(HexTile tile) {
        if(onChunkChange != null) {
            onChunkChange(tile);
        }
    }

    public List<HexTile> GetTiles() {
        if(_tiles == null) {
            _tiles = new List<HexTile>();
            for (int i = 0; i < tileIds.Count; i++)
            {
                _tiles.Add(planet.GetTile(tileIds[i]));
            }
        }
        return _tiles;
    }

    public Mesh GetMesh() {
        List<HexTile> tiles = GetTiles();
        string a = "";
        float b = 0.0f;
        for (int i = 0; i < tileIds.Count; i++)
        {
            a += tileIds[i] + " ";
            b += planet.GetTile(tileIds[i]).height;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> indicies = new List<int>();
        for (int i = 0; i < tiles.Count; i++)
        {
            // Generate hexagon base
            int baseIndex = vertices.Count;
            vertices.Add(TransformPoint(tiles[i].center, tiles[i].height));
            colors.Add(tiles[i].color);
            for (int j = 0; j < tiles[i].vertices.Count; j++)
            {
                vertices.Add(TransformPoint(tiles[i].vertices[j], tiles[i].height));
                colors.Add(tiles[i].color);

                indicies.Add(baseIndex);
                indicies.Add(baseIndex + j + 1);
                indicies.Add(baseIndex + ((j + 1) % tiles[i].vertices.Count) + 1);
            }

            // Generate walls if needed
            List<HexTile> neighbors = tiles[i].GetNeighbors();
            for (int j = 0; j < neighbors.Count; j++)
            {
                float thisHeight = tiles[i].height;
                float otherHeight = neighbors[j].height;
                if(otherHeight < thisHeight) {
                    baseIndex = vertices.Count;
                    // Add barrier
                    vertices.Add(TransformPoint(tiles[i].vertices[(j + 1) % tiles[i].vertices.Count], otherHeight));
                    vertices.Add(TransformPoint(tiles[i].vertices[j], otherHeight));
                    vertices.Add(TransformPoint(tiles[i].vertices[(j + 1) % tiles[i].vertices.Count], thisHeight));
                    vertices.Add(TransformPoint(tiles[i].vertices[j], thisHeight));

                    colors.Add(tiles[i].color);
                    colors.Add(tiles[i].color);
                    colors.Add(tiles[i].color);
                    colors.Add(tiles[i].color);

                    indicies.Add(baseIndex);
                    indicies.Add(baseIndex + 2);
                    indicies.Add(baseIndex + 1);

                    indicies.Add(baseIndex + 2);
                    indicies.Add(baseIndex + 3);
                    indicies.Add(baseIndex + 1);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetColors(colors);
        mesh.SetTriangles(indicies, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        return mesh;
    }

    public HexTile GetClosestTileAngle(Vector3 input) {
        List<HexTile> tiles = GetTiles();

        HexTile ret = null;
        float closeness = -10000.0f;
        for (int i = 0; i < tiles.Count; i++)
        {
            float similarity = Vector3.Dot(tiles[i].center.normalized, input.normalized);
            if(similarity > closeness) {
                closeness = similarity;
                ret = tiles[i];
            }
        }
        return ret;
    }

    private Vector3 TransformPoint(Vector3 input, float height) {
        return input * (1 + (height / planet.radius));
    }

    public void MakeDirty() {
        isDirty = true;
    }

    public void SetupEvents()
    {
        List<HexTile> tiles = GetTiles();
        foreach (HexTile tile in tiles)
        {
            tile.onTileChange += OnChunkTileChange;
        }
    }
}
