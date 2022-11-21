# Unity Hex Planet Generator

Some code which creates hexagon based planets in Unity. For performance, the hexagons are seperated into chunks to overcome the overhead of each one being it's own object. When a hexagon's height or color changes, the chunk's mesh is recomputed.

This demo also comes with a basic editor which can raise and lower selected tiles and set them to specifc heights.



## Known issues

- The hexegon generation algorithm searches for the 6 nearest neighbors (to make the points of the hexagon) in O(n^2) while an implementation which uses an oct tree could simplify this to O(n*log(n)). This results in the a slow initial generation of the planet.