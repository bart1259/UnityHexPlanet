using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GeodesicPoints
{
    public static List<Vector3> GenPoints(int subdivides, float radius) {
        const float X = 0.525731112119133606f;
        const float Z = 0.850650808352039932f;

        List<Vector3> verticies = new List<Vector3>(new Vector3[] {
            new Vector3(-X, 0.0f, Z), new Vector3(X, 0.0f, Z), new Vector3(-X, 0.0f, -Z), new Vector3(X, 0.0f, -Z),
            new Vector3(0.0f, Z, X ), new Vector3(0.0f, Z, -X), new Vector3(0.0f, -Z, X), new Vector3(0.0f, -Z, -X),
            new Vector3(Z, X, 0.0f ), new Vector3(-Z, X, 0.0f), new Vector3(Z, -X, 0.0f), new Vector3(-Z, -X, 0.0f),
            
        });

        List<int> indicies = new List<int>(new int[] {
            1,  4,  0,  4, 9, 0, 4,  5, 9, 8, 5,  4,  1, 8, 4,
            1, 10,  8, 10, 3, 8, 8,  3, 5, 3, 2,  5,  3, 7, 2,
            3, 10,  7, 10, 6, 7, 6, 11, 7, 6, 0, 11,  6, 1, 0,
           10,  1,  6, 11, 0, 9, 2, 11, 9, 5, 2,  9, 11, 2, 7
        });

        // Make sure there is a vertex per index
        List<Vector3> flatVerticies = new List<Vector3>();
        List<int> flatIndicies = new List<int>();
        for (int i = 0; i < indicies.Count; i++)
        {
            flatVerticies.Add(verticies[indicies[i]]);
            flatIndicies.Add(i);
        }
        verticies = flatVerticies;
        indicies = flatIndicies;

        // Subdivide
        for (int i = 0; i < subdivides; i++)
        {
            SubdivideSphere(ref verticies, ref indicies);
        }

        // Scale
        for (int i = 0; i < verticies.Count; i++)
        {
            verticies[i] *= radius;
        }

        
        return verticies.Distinct().ToList();;
    }

    private static void SubdivideSphere(ref List<Vector3> verticies, ref List<int> indicies) {
        
        List<int> newIndicies = new List<int>();

        int triCount = indicies.Count / 3;
        for (int tri = 0; tri < triCount; tri++)
        {
            // Get verticies of triangle we will be subdividing
            int oldVertIndex = (tri * 3);
            int idxA = indicies[oldVertIndex + 0];
            int idxB = indicies[oldVertIndex + 1];
            int idxC = indicies[oldVertIndex + 2];
            Vector3 vA = verticies[idxA];
            Vector3 vB = verticies[idxB];
            Vector3 vC = verticies[idxC];

            // Find new verticies
            Vector3 vAB = Vector3.Lerp(vA, vB, 0.5f);
            vAB = vAB.normalized;
            Vector3 vBC = Vector3.Lerp(vB, vC, 0.5f);
            vBC = vBC.normalized;
            Vector3 vAC = Vector3.Lerp(vA, vC, 0.5f);
            vAC = vAC.normalized;

            // Add new verticies to verticies list
            int newVertIndex = verticies.Count;
            verticies.Add(vAB);
            verticies.Add(vBC);
            verticies.Add(vAC);

            // Add new indicies
            newIndicies.Add(newVertIndex + 0); // Middle Tiangle
            newIndicies.Add(newVertIndex + 1);
            newIndicies.Add(newVertIndex + 2);

            newIndicies.Add(newVertIndex + 2); // A triangle
            newIndicies.Add(idxA);
            newIndicies.Add(newVertIndex + 0);

            newIndicies.Add(newVertIndex + 0); // B triangle
            newIndicies.Add(idxB);
            newIndicies.Add(newVertIndex + 1);

            newIndicies.Add(newVertIndex + 1); // C triangle
            newIndicies.Add(idxC);
            newIndicies.Add(newVertIndex + 2);


        }
        indicies = newIndicies;
    }
}
