using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CubeGroupFinder
{
    private static int gridWidth;
    private static int gridHeight;

    private static bool[,] visited;

    public static List<GameObject> FindGroup(int x, int y, Cube.CubeType? type, GameObject[,] grid, int originalHeight, bool searchForTNT = false)
    {
        gridWidth = grid.GetLength(0);
        gridHeight = originalHeight;
        visited = new bool[gridWidth, gridHeight];

        List<GameObject> group = new List<GameObject>();
        DFS(x, y, type, group, grid, originalHeight, searchForTNT);
        return group;
    }

    public static List<Obstacle> FindAdjacentObstacles(List<GameObject> group, GameObject[,] grid, int originalHeight)
    {
        gridWidth = grid.GetLength(0);
        gridHeight = originalHeight;

        HashSet<Obstacle> adjacentObstacles = new HashSet<Obstacle>();

        foreach (GameObject gameObject in group)
        {
            Cube cube = gameObject.GetComponent<Cube>();
            TNT tnt = gameObject.GetComponent<TNT>();
            Vector2Int crd;

            if (cube != null)
            {
                crd = cube.GetCoords();
            }
            else if (tnt != null)
            {
                crd = tnt.GetCoords();
            }
            else
            {
                continue;
            }

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

    private static void DFS(int x, int y, Cube.CubeType? type, List<GameObject> group, GameObject[,] grid, int originalHeight, bool searchForTNT)
    {
        // Base cases: out of bounds or visited cell
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight || visited[x, y] || grid[x, y] == null)
        {
            return;
        }

        GameObject gridObject = grid[x, y];
        Cube cube = gridObject.GetComponent<Cube>();
        TNT tnt = gridObject.GetComponent<TNT>();

        // If searching for TNT, and a TNT is found
        if (searchForTNT && tnt != null)
        {
            // Mark as visited
            visited[x, y] = true;
            // Add to group
            group.Add(gridObject);
        }
        // If searching for a specific cube type, and a matching cube is found
        else if (!searchForTNT && cube != null && cube.cubeType == type)
        {
            // Mark as visited
            visited[x, y] = true;
            // Add to group
            group.Add(gridObject);
        }
        else
        {
            return;
        }

        // Recursively check all adjacent cells
        DFS(x - 1, y, type, group, grid, originalHeight, searchForTNT); // left
        DFS(x + 1, y, type, group, grid, originalHeight, searchForTNT); // right
        DFS(x, y - 1, type, group, grid, originalHeight, searchForTNT); // down
        DFS(x, y + 1, type, group, grid, originalHeight, searchForTNT); // up
    }


    public static void ScanAndUpdateGrid(GameObject[,] grid, int originalHeight)
    {
        gridHeight = originalHeight;
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] == null) continue;

                Cube currentCube = grid[i, j].GetComponent<Cube>();
                if (currentCube == null) continue;

                // Reset visited array
                visited = new bool[gridWidth, gridHeight];

                // Find the group for the current cube
                List<GameObject> group = FindGroup(i, j, currentCube.cubeType, grid, originalHeight);

                if (group.Count >= 5)
                {
                    // If group size is 5 or more, activate the TNT sprite for all cubes in the group
                    foreach (GameObject obj in group)
                    {
                        Cube cube = obj?.GetComponent<Cube>();
                        if (cube != null)
                        {
                            cube.ActivateTNTSprite();
                        }
                    }
                }
                else
                {
                    // If group size is less than 5, deactivate the TNT sprite for all cubes in the group
                    foreach (GameObject obj in group)
                    {
                        Cube cube = obj?.GetComponent<Cube>();
                        if (cube != null && cube.isTNTActive)
                        {
                            cube.DeactivateTNTSprite();
                        }
                    }
                }
            }
        }
    }
}