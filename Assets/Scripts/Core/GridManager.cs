using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject tntPrefab;

    [Header("Grid Settings")]
    [SerializeField] private float spriteSize = 1.0f;  // Default size to 1 unit
    [SerializeField] private Vector2 originOffset = Vector2.zero; // Default offset is 0

    [Header("Cube Settings")]
    [SerializeField] private float fallDelay = 0.0f;
    [SerializeField] private float fallSpeed = 1f;

    private int gridWidth;
    private int gridHeight;
    private Vector2 gridOrigin;
    private GameObject[,] grid;

    private bool[,] visited;
    private List<Cube> foundGroup;
    private bool isFalling = false;

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
        gridOrigin = new Vector2((-totalGridWidth / 2 + spriteSize / 2) + originOffset.x, (-totalGridHeight / 2 + spriteSize / 2) + originOffset.y);

        int index = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                string itemCode = levelData.grid[index];
                GameObject prefabToInstantiate = GetPrefabBasedOnItemCode(itemCode);

                if (prefabToInstantiate != null)
                {
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

        CheckAndUpdateSprites();

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
        if (isFalling) return;

        bool isTNTCreated = false;

        // Initialize visited array and found group list
        visited = new bool[gridWidth, gridHeight];
        foundGroup = new List<Cube>();

        // TODO: Get the grid position of the tapped cube (replace these)
        int tappedX = 0;
        int tappedY = 0;

        // Find the coordinates of the tapped cube in the grid
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                // Check if grid[x, y] is not null before calling GetComponent<Cube>()
                Cube cube = grid[x, y]?.GetComponent<Cube>();
                if (cube == tappedCube)
                {
                    tappedX = x;
                    tappedY = y;
                }
            }
        }

        // Start the DFS from the tapped cube
        FindGroup(tappedX, tappedY, tappedCube.cubeType);

        // foundGroup now contains all adjacent cubes of the same color

        // If group size is 2 or more, then we remove the cubes
        if (foundGroup.Count >= 2)
        {
            GameObject newTNT = null;
            // Iterate through each cube in the found group and destroy them
            foreach (var cube in foundGroup)
            {
                isTNTCreated = false;
                // If 5 or more cubes, place TNT at tapped cube position
                if (foundGroup.Count >= 5)
                {
                    if (cube == tappedCube)
                    {
                        Debug.Log($"TNT Created at ({tappedX + 1},{tappedY + 1})");
                        newTNT = Instantiate(tntPrefab, tappedCube.transform.position, Quaternion.identity);
                        newTNT.transform.localScale = new Vector3(spriteSize, spriteSize, 1);
                        newTNT.transform.SetParent(gridParent.transform);
                        isTNTCreated = true;
                    }

                    foreach (var tempCube in foundGroup)
                    {
                        tempCube.ActivateTNTSprite();
                    }
                }

                int cubeX = 0, cubeY = 0;
                // Find the grid position of the cube to be destroyed
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        if (grid[x, y]?.GetComponent<Cube>() == cube)
                        {
                            cubeX = x;
                            cubeY = y;
                        }
                    }
                }

                Destroy(cube.gameObject);

                

                if (!isTNTCreated)
                {
                    grid[cubeX, cubeY] = null; // Set the grid position to null after destroying the cube
                } else
                {
                    grid[cubeX,cubeY] = newTNT;
                }
            }
        }

        // After destroying cubes, handle the falling cubes
        // StartCoroutine(HandleFallingCubes());

        // Debugging output to check the found group
        if (foundGroup.Count >= 2)
        {
            // Debug.Log($"Found group: {foundGroup.Count} {foundGroup[0].cubeType.ToString()}");
        }
    }

    private void BlastDamageAdjacent(int x, int y)
    {
        // Coordinates of adjacent cells
        int[] adjacentX = { x - 1, x + 1, x, x };
        int[] adjacentY = { y, y, y - 1, y + 1 };

        for (int i = 0; i < 4; i++)
        {
            if (adjacentX[i] >= 0 && adjacentX[i] < gridWidth && adjacentY[i] >= 0 && adjacentY[i] < gridHeight)
            {
                Obstacle obstacle = grid[adjacentX[i], adjacentY[i]]?.GetComponent<Obstacle>();
                if (obstacle != null && obstacle.isAffectedByBlast)
                {
                    obstacle.TakeDamage(1);
                }
            }
        }
    }

    private void FindGroup(int x, int y, Cube.CubeType type)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return;

        GameObject gridObject = grid[x, y];
        if (gridObject == null) return;

        Cube cubeComponent = gridObject.GetComponent<Cube>();
        if (cubeComponent == null) return;

        if (visited[x, y] || cubeComponent.cubeType != type) return;

        visited[x, y] = true;
        foundGroup.Add(cubeComponent);

        FindGroup(x + 1, y, type);
        FindGroup(x - 1, y, type);
        FindGroup(x, y + 1, type);
        FindGroup(x, y - 1, type);
    }


    private IEnumerator FallDown(GameObject cube, Vector3 target, float duration)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = cube.transform.position;
        while (elapsedTime < duration)
        {
            cube.transform.position = Vector3.Lerp(startingPosition, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cube.transform.position = target; // Ensure the target position is set accurately after the loop

        CheckAndUpdateSprites();
    }

    // A coroutine to handle each column individually
    private IEnumerator HandleColumnFalling(int x)
    {
        List<Coroutine> coroutines = new List<Coroutine>();

        for (int y = 0; y < gridHeight; y++)
        {
            if (grid[x, y] == null)
            {
                // Wait for the specified delay time
                yield return new WaitForSeconds(fallDelay);

                // Find the next non-null cube in this column
                for (int nextY = y + 1; nextY < gridHeight; nextY++)
                {
                    if (grid[x, nextY] != null)
                    {
                        // Calculate target position
                        Vector3 targetPosition = new Vector3(gridOrigin.x + x * spriteSize, gridOrigin.y + y * spriteSize, 0);

                        // Calculate the distance to the target position
                        float distance = Vector3.Distance(grid[x, nextY].transform.position, targetPosition);

                        // Calculate the fall duration based on the distance and speed
                        float fallDuration = distance / fallSpeed;

                        // Start the coroutine to make the cube fall down and add it to the list
                        coroutines.Add(StartCoroutine(FallDown(grid[x, nextY], targetPosition, fallDuration)));

                        // Update the grid array
                        grid[x, y] = grid[x, nextY];
                        grid[x, nextY] = null;

                        break;
                    }
                }
            }
        }

        // Wait for all coroutines to complete
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }
    }


    // The main function to handle all cubes falling
    private IEnumerator HandleFallingCubes()
    {
        isFalling = true;
        List<Coroutine> coroutines = new List<Coroutine>();

        for (int x = 0; x < gridWidth; x++)
        {
            coroutines.Add(StartCoroutine(HandleColumnFalling(x)));
        }

        // Wait for all coroutines to complete
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        // Start the coroutine to spawn new cubes
        yield return StartCoroutine(SpawnNewCubes());

        isFalling = false;
        CheckAndUpdateSprites();
    }

    private IEnumerator SpawnNewCubes()
    {
        List<Coroutine> spawnCoroutines = new List<Coroutine>();

        for (int x = 0; x < gridWidth; x++)
        {
            spawnCoroutines.Add(StartCoroutine(SpawnNewCubesInColumn(x)));
        }

        foreach (var coroutine in spawnCoroutines)
        {
            yield return coroutine;
        }

        CheckAndUpdateSprites();
    }

    private IEnumerator SpawnNewCubesInColumn(int x)
    {
        int emptyCount = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            if (grid[x, y] == null)
            {
                emptyCount++;
                string itemCode = "rand";
                GameObject prefabToInstantiate = GetPrefabBasedOnItemCode(itemCode);

                float initialY = gridOrigin.y + gridHeight * spriteSize + (emptyCount - 1) * spriteSize;
                GameObject newCell = Instantiate(prefabToInstantiate, new Vector3(gridOrigin.x + x * spriteSize, 1.25f, 0), Quaternion.identity);

                newCell.transform.localScale = new Vector3(spriteSize, spriteSize, 1);
                newCell.transform.SetParent(gridParent.transform);

                Cube cubeComponent = newCell.GetComponent<Cube>();
                if (cubeComponent != null)
                {
                    cubeComponent.cubeType = Cube.CubeType.Random;
                    cubeComponent.UpdateSprite();
                }

                Vector3 targetPosition = new Vector3(gridOrigin.x + x * spriteSize, gridOrigin.y + y * spriteSize, 0);
                float distance = Vector3.Distance(newCell.transform.position, targetPosition);
                float fallDuration = distance / fallSpeed;

                yield return StartCoroutine(FallDown(newCell, targetPosition, fallDuration));

                grid[x, y] = newCell;
            }
        }
    }

    private void CheckAndUpdateSprites()
    {
        // Iterate over the grid and check for groups
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                // Reset visited array for new checking
                visited = new bool[gridWidth, gridHeight];
                foundGroup = new List<Cube>();

                if (grid[x, y] == null) return;

                // Start the DFS from each cube to find groups
                Cube currentCube = grid[x, y]?.GetComponent<Cube>();
                if (currentCube != null)
                {
                    FindGroup(x, y, currentCube.cubeType);

                    // Check the found group and update sprites
                    if (foundGroup.Count >= 5)
                    {
                        foreach (var cube in foundGroup)
                        {
                            cube.ActivateTNTSprite();
                        }
                    }
                    else
                    {
                        foreach (var cube in foundGroup)
                        {
                            cube.DeactivateTNTSprite();
                        }
                    }
                }
            }
        }
    }

    // TODO: Additional methods like destroying obstacles, spawning TNT etc.
}
