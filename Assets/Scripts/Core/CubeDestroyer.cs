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

            if(cube != null) GridManager.instance.UpdateCell(cube.GetCoords().x, cube.GetCoords().y, null);
            else if (tnt != null) GridManager.instance.UpdateCell(tnt.GetCoords().x, tnt.GetCoords().y, null);
            else return;
            Object.Destroy(obj);

            // TODO: Animations.
        }
    }

    public static void DealDamageToObstacles(List<Obstacle> obstacles, int damage, bool damageByTNT = false)
    {
        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.TakeDamage(damage, damageByTNT);
        }
    }

    public static void ExplodeTNT(Vector2Int tntPosition, GameObject[,] grid, int originalHeight, bool isCombo)
    {
        int range = isCombo ? 3 : 2; // 7x7 for combo, 5x5 for single
        int gridWidth = grid.GetLength(0);
        int gridHeight = originalHeight;

        GameObject tappedTNT = grid[tntPosition.x, tntPosition.y];

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
                        Cube cubeComponent = gridObject.GetComponent<Cube>();
                        Obstacle obstacleComponent = gridObject.GetComponent<Obstacle>();
                        if (cubeComponent != null)
                        {
                            // Destroy the cube
                            Object.Destroy(gridObject);
                            grid[x, y] = null;
                        }
                        else if (obstacleComponent != null)
                        {
                            // Deal damage to the obstacle
                            obstacleComponent.TakeDamage(1, true);
                        }
                    }
                }
            }
        }

        Object.Destroy(tappedTNT);
        GridManager.instance.UpdateCell(tntPosition.x, tntPosition.y, null);
    }
}
