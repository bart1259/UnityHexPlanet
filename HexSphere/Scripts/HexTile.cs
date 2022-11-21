using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Color color;

    [System.NonSerialized]
    private List<HexTile> _neighbors;

    public HexTile(int id, HexPlanet planet, Vector3 center, List<Vector3> verts) {
        this.id = id;
        this.planet = planet;
        this.center = center;
        this.vertices = verts;
        const float scaling = 0.08f;
        this.height = Mathf.Floor(17.5f * PerlinNoise3D(this.center.x * scaling, this.center.y * scaling, this.center.z * scaling));
        ComputeColor();

        neighborIDs = new List<int>();
    }

    public void ComputeColor() {
        if(this.height > 10) {
            this.color = Color.white;
        } else if(this.height > 9) {
            this.color = Color.gray;
        } else if (this.height > 7) {
            this.color = Color.green;
        } else {
            this.color = Color.blue;
        }
    }

    public static float PerlinNoise3D(float x, float y, float z)
    {
        x += 15;
        y += 25;
        z += 35;
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);
        return (xy + xz + yz + yx + zx + zy) / 6;
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
        ComputeColor();
        if(onTileChange != null) {
            onTileChange(this);
        }
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
}
