using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityHexPlanet {

    public static class HexPlanetHexGenerator
    {
        public static void GeneratePlanetTilesAndChunks(HexPlanet planet) {
            List<Vector3> points = GeodesicPoints.GenPoints(planet.subdivisions, planet.radius);
            List<HexTile> tiles = GenHexTiles(planet, ref points);

            List<Vector3> chunkOrigins = GeodesicPoints.GenPoints(planet.chunkSubdivisions, planet.radius);
            List<HexChunk> chunks = GenHexChunks(planet, tiles, chunkOrigins);
            planet.chunks = chunks;
            planet.tiles = tiles;
        }

        private static List<HexChunk> GenHexChunks(HexPlanet planet, List<HexTile> tiles, List<Vector3> chunkCenters) {
            int chunkCount = chunkCenters.Count;
            List<HexChunk> chunks = new List<HexChunk>();
            for (int i = 0; i < chunkCount; i++)
            {
                chunks.Add(new HexChunk(i, planet, chunkCenters[i]));
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                HexChunk bestChunk = (from chunk in chunks
                                    orderby (tiles[i].center - chunk.origin).sqrMagnitude
                                    select chunk).Take(1).ToList()[0];
                bestChunk.AddTile(tiles[i].id);
                tiles[i].SetChunk(bestChunk.id);
            }

            return chunks;
        }

        private static List<HexTile> GenHexTiles(HexPlanet planet, ref List<Vector3> sphereVerts) {
            List<HexTile> hexTiles = new List<HexTile>();

            // Get unique points
            List<Vector3> uniqueVerts = sphereVerts.Distinct().ToList();
            
            // Construct an oct tree out of all points that will be centers of hexes
            OctTree<Vector3> vertOctTree = new OctTree<Vector3>(Vector3.one * (planet.radius * -1.1f), Vector3.one * (planet.radius * 1.1f));
            foreach(Vector3 v in uniqueVerts) {
                vertOctTree.InsertPoint(v, v);
            }

            // Get the maximum distance between two neighbors
            float maxDistBetweenNeighbots = Mathf.Sqrt((from vert in uniqueVerts
                                            orderby (vert - uniqueVerts[uniqueVerts.Count - 1]).sqrMagnitude
                                            select (vert - uniqueVerts[uniqueVerts.Count - 1]).sqrMagnitude).Take(7).ToList()[6]) * 1.2f;

            // Generate the tiles
            for (int i = 0; i < uniqueVerts.Count; i++)
            {
                Vector3 uniqueVert = uniqueVerts[i];
                List<Vector3> closestVerts = vertOctTree.GetPoints(uniqueVert, Vector3.one * maxDistBetweenNeighbots);
                var closest = (from vert in closestVerts 
                            orderby (vert - uniqueVert).sqrMagnitude 
                            select vert).Take(7).ToList();
                if(closest.Count < 7 || (closest[6] - uniqueVert).magnitude > (closest[5] - uniqueVert).magnitude * 1.5) {
                    closest = closest.Take(6).ToList();
                }
                closest = closest.Skip(1).ToList();

                // Order the closest so an increase in index revolves them counter clockwise
                // BUG: This is a hack and might result in bugs
                Vector3 angleAxis = Vector3.up + (Vector3.one * 0.1f);
                closest = (from vert in closest
                        orderby -Vector3.SignedAngle(vert - uniqueVert, angleAxis, uniqueVert)
                        select vert).ToList();

                List<Vector3> hexVerts = new List<Vector3>();
                for (int j = 0; j < closest.Count; j++)
                {
                    hexVerts.Add(Vector3.Lerp(Vector3.Lerp(uniqueVert, closest[j], 0.66666666f), Vector3.Lerp(uniqueVert, closest[(j + 1) % closest.Count], 0.66666666f), 0.5f));
                }

                // Find center vertex
                Vector3 center = Vector3.zero;
                for (int j = 0; j < hexVerts.Count; j++)
                {
                    center += hexVerts[j];
                }
                center /= hexVerts.Count;

                HexTile hexTile = planet.terrainGenerator.CreateHexTile(i, planet, center, hexVerts);
                hexTiles.Add(hexTile);
            }

            // Construct an oct tree out of all points that will be centers of hexes
            OctTree<HexTile> hexOctTree = new OctTree<HexTile>(Vector3.one * (planet.radius * -1.1f), Vector3.one * (planet.radius * 1.1f));
            foreach(HexTile h in hexTiles) {
                hexOctTree.InsertPoint(h, h.center);
            }

            // Find neighbors
            for (int i = 0; i < hexTiles.Count; i++)
            {
                HexTile currentTile = hexTiles[i];
                List<HexTile> closestHexes = hexOctTree.GetPoints(currentTile.center, Vector3.one * maxDistBetweenNeighbots);
                var closest = (from tile in closestHexes 
                            orderby (tile.center - currentTile.center).sqrMagnitude 
                            select tile).Take(7).ToList();
                if((closest[6].center - currentTile.center).magnitude > (closest[5].center - currentTile.center).magnitude * 1.5) {
                    closest = closest.Take(6).ToList(); // Must be a pentagon
                }

                // Exclude self, closest tile
                closest = closest.Skip(1).ToList();

                // Order the tiles based on the vertices
                List<Vector3> verts = currentTile.vertices;
                List<HexTile> orderedNeighbors = new List<HexTile>();
                
                for (int j = 0; j < verts.Count; j++)
                {
                    HexTile a = (from tile in closest orderby -Vector3.Dot(((verts[j] + verts[(j + 1) % verts.Count]) / 2).normalized, tile.center.normalized) select tile).ToList()[0];
                    orderedNeighbors.Add(a);
                }


                currentTile.AddNeighbors(orderedNeighbors);
            }

            // Run each tile through the generator
            foreach (HexTile tile in hexTiles) {
                planet.terrainGenerator.AfterTileCreation(tile);
            }

            return hexTiles;
        }
    }
}