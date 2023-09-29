using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDestroyer
{
    public static void DestroyGroup(List<GameObject> group)
    {
        foreach (GameObject obj in group)
        {
            Cube cube = obj?.GetComponent<Cube>();
            TNT tnt = obj?.GetComponent<TNT>();

            if (cube != null)
            {
                GridManager.instance.UpdateCell(cube.GetCoords().x, cube.GetCoords().y, null);
                cube.SpawnParticle();
            }
            else if (tnt != null) GridManager.instance.UpdateCell(tnt.GetCoords().x, tnt.GetCoords().y, null);
            else return;
            Object.Destroy(obj);
        }
    }

    public static void DealDamageToObstacles(List<Obstacle> obstacles, int damage, bool damageByTNT = false)
    {
        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.TakeDamage(damage, damageByTNT);
        }
    }

    public static void ExplodeTNT(Vector2Int tntPosition, GameObject[,] grid, int originalHeight, bool isCombo, HashSet<Vector2Int> explodedTNTs = null)
    {
        if (explodedTNTs == null) explodedTNTs = new HashSet<Vector2Int>();

        // If the TNT at this position has already been exploded, exit the function to prevent infinite recursion.
        if (explodedTNTs.Contains(tntPosition)) return;

        explodedTNTs.Add(tntPosition);

        int range = isCombo ? 3 : 2;
        int gridWidth = grid.GetLength(0);
        int gridHeight = originalHeight;

        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++)
            {
                int x = tntPosition.x + i;
                int y = tntPosition.y + j;

                if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
                {
                    GameObject gridObject = grid[x, y];
                    if (gridObject != null)
                    {
                        Cube cubeComponent = gridObject?.GetComponent<Cube>();
                        Obstacle obstacleComponent = gridObject?.GetComponent<Obstacle>();
                        TNT tntComponent = gridObject?.GetComponent<TNT>();
                        if (cubeComponent != null)
                        {
                            cubeComponent.SpawnParticle();
                            Object.Destroy(gridObject);
                            grid[x, y] = null;
                        }
                        else if (obstacleComponent != null)
                        {
                            obstacleComponent.TakeDamage(1, true);
                        }
                        else if (tntComponent != null)
                        {
                            Vector2Int newTNTPosition = new Vector2Int(x, y);
                            List<GameObject> tntGroup = CubeGroupFinder.FindGroup(newTNTPosition.x, newTNTPosition.y, null, grid, originalHeight, true);
                            bool isComboTNT = tntGroup.Count > 1;
                            foreach (GameObject tntObject in tntGroup)
                            {
                                ExplodeTNT(newTNTPosition, grid, originalHeight, isComboTNT, explodedTNTs);
                            }
                        }
                    }
                }
            }
        }

        grid[tntPosition.x, tntPosition.y]?.GetComponent<TNT>().SpawnParticle();
        Object.Destroy(grid[tntPosition.x, tntPosition.y]);
        GridManager.instance.UpdateCell(tntPosition.x, tntPosition.y, null);
    }
}
