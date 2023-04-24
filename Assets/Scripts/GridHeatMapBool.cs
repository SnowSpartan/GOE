using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHeatMapBool : MonoBehaviour
{
    private Grid<bool> grid;
    private Mesh mesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(Grid<bool> grid)
    {
        this.grid = grid;
        UpdateHeatMap();
    }

    public void UpdateHeatMap()
    {
        GridMesh.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);

        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetHeight(); j++)
            {
                int index = i * grid.GetHeight() + j;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();
                float gridValueNormalized = 0f;

                bool gridValue = grid.GetValue(i, j);
                if (gridValue)
                {
                    gridValueNormalized = 1.0f;
                }

                Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);

                if (gridValue)
                {
                    GridMesh.AddToMeshArrays(vertices, uvs, triangles, index, grid.GetWorldPosition(i, j) + quadSize * .5f + new Vector3(0, -6f, 2f), 45f, quadSize, gridValueUV, gridValueUV);
                    mesh.vertices = vertices;
                    mesh.uv = uvs;
                    mesh.triangles = triangles;
                    mesh.RecalculateBounds();
                }

            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }
}