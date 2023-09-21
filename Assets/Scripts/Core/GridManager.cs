using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject[] obstaclePrefabs;
    public GameObject gridParent;

    public float spriteSize = 1.0f;  // Default size to 1 unit

    private int gridWidth;
    private int gridHeight;
    private Vector2 gridOrigin;
    private GameObject[,] grid;

    private bool[,] visited;
    private List<Cube> foundGroup;

    public static GridManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // dummy LevelData for testing
        LevelData levelData = new LevelData
        {
            level_number = 1,
            grid_width = 9,
            grid_height = 10,
            move_count = 20,
            grid = new string[] { "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "bo", "r", "r", "r", "r", "g", "b", "b", "b", "b", "y", "y", "y", "y", "g", "y", "y", "y", "y", "b", "b", "b", "b", "y", "r", "r", "r", "r", "rand", "rand", "rand", "rand", "y", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand", "rand" }
        };

        InitializeGrid(levelData);
    }

    public void InitializeGrid(LevelData levelData)
    {
        gridWidth = levelData.grid_width;
        gridHeight = levelData.grid_height;
        grid = new GameObject[gridWidth, gridHeight];

        float totalGridWidth = gridWidth * spriteSize;
        float totalGridHeight = gridHeight * spriteSize;

        // Calculate the origin so that the grid will be centered
        gridOrigin = new Vector2(-totalGridWidth / 2 + spriteSize / 2, -totalGridHeight / 2 + spriteSize / 2);

        int index = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                string itemCode = levelData.grid[index];
                GameObject prefabToInstantiate = GetPrefabBasedOnItemCode(itemCode);

                if (prefabToInstantiate != null)
                {
                    Debug.Log($"Instantiating {prefabToInstantiate.name} at {new Vector3(gridOrigin.x + x * spriteSize, gridOrigin.y + y * spriteSize, 0)} with scale {new Vector3(spriteSize, spriteSize, 1)}");
                    GameObject newCell = Instantiate(prefabToInstantiate, new Vector3(gridOrigin.x + x * spriteSize, gridOrigin.y + y * spriteSize, 0), Quaternion.identity);
                    newCell.transform.localScale = new Vector3(spriteSize, spriteSize, 1);
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

        // Debugging output
        //for (int y = 0; y < gridHeight; y++)
        //{
        //    for (int x = 0; x < gridWidth; x++)
        //    {
        //        Debug.Log($"Grid[{x},{y}] = {grid[x, y]?.name ?? "Empty"}");
        //    }
        //}
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

    public void CubeTapped(Cube tappedCube)
    {
        // Initialize visited array and found group list
        visited = new bool[gridWidth, gridHeight];
        foundGroup = new List<Cube>();

        // TODO: Get the grid position of the tapped cube (replace these)
        int tappedX = 0;
        int tappedY = 0;

        // Start the DFS from the tapped cube
        FindGroup(tappedX, tappedY, tappedCube.cubeType);

        // foundGroup now contains all adjacent cubes of the same color

        // Debugging output to check the found group
        Debug.Log("Found group:");
        foreach (var cube in foundGroup)
        {
            Debug.Log(cube.cubeType.ToString());
        }
    }

    private void FindGroup(int x, int y, Cube.CubeType type)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return;
        if (visited[x, y] || grid[x, y].GetComponent<Cube>().cubeType != type) return;

        visited[x, y] = true;

        foundGroup.Add(grid[x, y].GetComponent<Cube>());

        FindGroup(x + 1, y, type);
        FindGroup(x - 1, y, type);
        FindGroup(x, y + 1, type);
        FindGroup(x, y - 1, type);
    }

    // TODO: Additional methods like CheckForMatches, HandleFalling, etc.
}
