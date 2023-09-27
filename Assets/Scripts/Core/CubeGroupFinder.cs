using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CubeGroupFinder
{
    private static int gridWidth;
    private static int gridHeight;

    private static bool[,] visited;

    public static List<Cube> FindGroup(int x, int y, Cube.CubeType type, GameObject[,] grid)
    {
        gridWidth = grid.GetLength(0);
        gridHeight = grid.GetLength(1);
        visited = new bool[gridWidth, gridHeight];

        List<Cube> group = new List<Cube>();
        DFS(x, y, type, group, grid);
        return group;
    }

    public static List<Obstacle> FindAdjacentObstacles(List<Cube> group, GameObject[,] grid)
    {
        gridWidth = grid.GetLength(0);
        gridHeight = grid.GetLength(1);

        HashSet<Obstacle> adjacentObstacles = new HashSet<Obstacle>();

        foreach (Cube cube in group)
        {
            Vector2Int crd = cube.GetCoords();
            int x = crd.x;
            int y = crd.y;

            // Check each neighbor
            List<Vector2Int> neighbors = new List<Vector2Int>
        {
            new Vector2Int(x + 1, y),
            new Vector2Int(x - 1, y),
            new Vector2Int(x, y + 1),
            new Vector2Int(x, y - 1)
        };

            foreach (var neighbor in neighbors)
            {
                // If inside the grid boundaries
                if (neighbor.x >= 0 && neighbor.y >= 0 && neighbor.x < gridWidth && neighbor.y < gridHeight)
                {
                    GameObject neighborObject = grid[neighbor.x, neighbor.y];
                    if (neighborObject != null)
                    {
                        Obstacle obstacleComponent = neighborObject.GetComponent<Obstacle>();
                        if (obstacleComponent != null)
                        {
                            adjacentObstacles.Add(obstacleComponent);
                            // obstacleComponent.gameObject.SetActive(false); // Debug
                        }
                    }
                }
            }
        }

        return adjacentObstacles.ToList();
    }

    private static void DFS(int x, int y, Cube.CubeType type, List<Cube> group, GameObject[,] grid)
    {
        // Base cases: out of bounds or visited cell
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight || visited[x, y] || grid[x, y] == null)
        {
            return;
        }

        Cube cube = grid[x, y].GetComponent<Cube>();

        if (cube == null || cube.cubeType != type)
        {
            return;
        }

        // Mark as visited
        visited[x, y] = true;

        // Add to group
        group.Add(cube);

        // Recursively check all adjacent cells
        DFS(x - 1, y, type, group, grid); // left
        DFS(x + 1, y, type, group, grid); // right
        DFS(x, y - 1, type, group, grid); // down
        DFS(x, y + 1, type, group, grid); // up
    }

    public static void ScanAndUpdateGrid(GameObject[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] == null) continue;

                Cube currentCube = grid[i, j].GetComponent<Cube>();
                if (currentCube == null) continue;

                // Find the group for the current cube
                List<Cube> group = FindGroup(i, j, currentCube.cubeType, grid);

                // Iterate through all the cubes in the group
                foreach (Cube cube in group)
                {
                    if (group.Count >= 5)
                    {
                        // If group size is 5 or more, activate the TNT sprite
                        cube.ActivateTNTSprite();
                    }
                    else
                    {
                        // If group size is less than 5 and the cube is currently a TNT, deactivate the TNT sprite
                        if (cube.isTNTActive)
                        {
                            cube.DeactivateTNTSprite();
                        }
                    }
                }
            }
        }
    }
}