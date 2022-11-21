# Unity Hex Planet Generator

Some code which creates hexagon based planets in Unity. For performance, the hexagons are seperated into chunks to overcome the overhead of associated with each being it's own object. When a hexagon's height or color changes, the chunk's mesh is automatically recomputed.

This demo also comes with a basic editor which can raise and lower selected tiles and set them to specifc heights.

![UnityHexPlanet](https://user-images.githubusercontent.com/21147581/203176777-610e9e23-b38b-4f98-8bd6-edef96d9d083.PNG)

## Known issues

- The hexegon generation algorithm searches for the 6 nearest neighbors (to make the points of the hexagon) in O(n^2) while an implementation which uses an oct tree could simplify this to O(n*log(n)). This results in a long initial generation of the planet.
