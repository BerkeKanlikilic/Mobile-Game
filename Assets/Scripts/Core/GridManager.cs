using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject[] obstaclePrefabs;
    public GameObject gridParent;

    private int gridWidth;
    private int gridHeight;
    private GameObject[,] grid;

    public void InitializeGrid(LevelData levelData)
    {
        gridWidth = levelData.grid_width;
        gridHeight = levelData.grid_height;

        grid = new GameObject[gridWidth, gridHeight];

        int index = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                string itemCode = levelData.grid[index];
                GameObject prefabToInstantiate = GetPrefabBasedOnItemCode(itemCode);

                if (prefabToInstantiate != null)
                {
                    GameObject newCell = Instantiate(prefabToInstantiate, new Vector3(x, y, 0), Quaternion.identity);
                    newCell.transform.SetParent(gridParent.transform);

                    Cube cubeComponent = newCell.GetComponent<Cube>();
                    if (cubeComponent != null)
                    {
                        // logic to update the Cube's type and sprite based on itemCode
                        switch (itemCode)
                        {
                            case "r":
                                cubeComponent.cubeType = Cube.CubeType.Red;
                                break;
                            case "g":
                                cubeComponent.cubeType = Cube.CubeType.Green;
                                break;
                            case "b":
                                cubeComponent.cubeType = Cube.CubeType.Blue;
                                break;
                            case "y":
                                cubeComponent.cubeType = Cube.CubeType.Yellow;
                                break;
                            case "rand":
                                cubeComponent.cubeType = Cube.CubeType.Random;
                                break;
                        }
                        cubeComponent.UpdateSprite();
                    }

                    grid[x, y] = newCell;
                }

                index++;
            }
        }
    }

    private GameObject GetPrefabBasedOnItemCode(string itemCode)
    {
        switch (itemCode)
        {
            case "r":
            case "g":
            case "b":
            case "y":
            case "rand":
                return cubePrefab;
            case "bo":
                return obstaclePrefabs[0]; // Box
            case "s":
                return obstaclePrefabs[1]; // Stone
            case "v":
                return obstaclePrefabs[2]; // Vase
            default:
                Debug.LogError("Unknown item code: " + itemCode);
                return null;
        }
    }

    // TODO: Additional methods like CheckForMatches, HandleFalling, etc.
}
