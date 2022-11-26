using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityHexPlanet {

    [System.Serializable]
    [CreateAssetMenu(menuName = "HexPlanet/PerlinTerrainGenerator", order = 1)]
    public class PerlinTerrainGenerator : BaseTerrainGenerator
    {
        [System.Serializable]
        public class ColorHeight {
            public Color32 color;
            public float maxHeight;
        }

        [Range(1,8)]
        public int octaves = 1;
        [Range(0,1)]
        public float persistence = 0.5f;
        [Range(1, 10)]
        public float lacunarity = 2;

        public float minHeight;
        public float maxHeight;
        public float noiseScaling;
        public List<ColorHeight> colorHeights;


        public override void AfterTileCreation(HexTile newTile) {
            float height = Mathf.Floor(3 * (((maxHeight - minHeight) * GetNoise(newTile.center.normalized.x, newTile.center.normalized.y, newTile.center.normalized.z)) + minHeight)) / 3.0f;
            newTile.height = height;
            
            for(int i = colorHeights.Count - 1; i >= 0; i--) {
                if(height < colorHeights[i].maxHeight) {
                    newTile.color = colorHeights[i].color;
                }
            }
        }

        private float GetNoise(float x, float y, float z) {
            float value = 0.0f;
            float scale = noiseScaling;
            float effect = 1.0f;
            for (int i = 0; i < octaves; i++) {
                
                value += effect * PerlinNoise3D(scale * x, scale * y, scale * z);
                scale *= lacunarity;
                effect *= (1 - persistence);
            }
            return value;
        }

        private static float PerlinNoise3D(float x, float y, float z)
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

    }
}