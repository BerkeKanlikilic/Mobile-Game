using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject tntPrefab;
    [SerializeField] private SpriteRenderer gridBackground;

    [Header("Grid Settings")]
    [SerializeField] private float spriteSize = 1.0f;  // Default size to 1 unit
    [SerializeField] private Vector2 originOffset = Vector2.zero; // Default offset is 0

    [Header("Grid Background Settings")]
    [SerializeField] private Vector2 margin;

    [Header("Cube Settings")]
    [SerializeField] private float fallDelayRow;
    [SerializeField] private float postFallDelay;
    [SerializeField] private float fallSpeed = 1f;

    private int gridWidth;
    private int gridHeight;
    private Vector2 gridOrigin;
    private GameObject[,] grid;
    private bool[,] visited;
    private List<Cube> foundGroup;


    public bool isAnyCubeMoving { get; private set; } = false;
    private int movingCubeCount = 0;


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

    public void InitializeGrid(LevelData levelData)
    {
        gridWidth = levelData.grid_width;
        gridHeight = levelData.grid_height;
        gridBackground.size = new Vector2(((float)gridWidth / 2) + margin.x, ((float)gridHeight / 2) + margin.y);

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
                
                InitializeCell(x, y, itemCode);

                index++;
            }
        }

        CubeGroupFinder.ScanAndUpdateGrid(grid);
    }

    private void InitializeCell(int x, int y, string itemCode)
    {
        GameObject prefabToInstantiate = GetPrefabBasedOnItemCode(itemCode);

                if (prefabToInstantiate != null)
                {
                    GameObject newCell = Instantiate(prefabToInstantiate, new Vector3(gridOrigin.x + x * spriteSize, gridOrigin.y + y * spriteSize, 0), Quaternion.identity);
                    newCell.transform.localScale = new Vector3(spriteSize, spriteSize, 1);
                    newCell.transform.SetParent(gridParent.transform);

                    Cube cubeComponent = newCell.GetComponent<Cube>();
                    Obstacle obstacleComponent = newCell.GetComponent<Obstacle>();
                    TNT tntComponent = newCell.GetComponent<TNT>();
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
                        cubeComponent.SetCoords(x, y);
                    }
                    else if (obstacleComponent != null)
                    {
                        // logic to update the Obstacle's type and sprite based on itemCode
                        switch (itemCode)
                        {
                            case "bo":
                                obstacleComponent.obstacleType = Obstacle.ObstacleType.Box;
                                break;
                            case "s":
                                obstacleComponent.obstacleType = Obstacle.ObstacleType.Stone;
                                break;
                            case "v":
                                obstacleComponent.obstacleType = Obstacle.ObstacleType.Vase;
                                break;
                        }
                        obstacleComponent.SetCoords(x, y);
                    }
                    else if (tntComponent != null)
                    {
                        tntComponent.SetCoords(x, y);
                    }
                    grid[x, y] = newCell;
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
            case "t":
                return tntPrefab;
            default:
                return null;
        }
    }

    public void UpdateCell(int x, int y, GameObject newCell)
    {
        // Debug.Log($"Updated Cell: {x}, {y}");
        grid[x, y] = newCell;
    }

    public void HandleCellTap(GameObject cell)
    {
        Cube cube = cell?.GetComponent<Cube>();
        TNT tnt = cell?.GetComponent<TNT>();

        if (cube != null)
        {
            Vector2Int coords = cube.GetCoords();

            List<Cube> group = CubeGroupFinder.FindGroup(coords.x, coords.y, cube.cubeType, grid);
            List<Obstacle> obstacles = CubeGroupFinder.FindAdjacentObstacles(group, grid);

            if (group.Count > 1)
            {
                CubeDestroyer.DestroyGroup(group);
                CubeDestroyer.DealDamageToObstacles(obstacles, 1);

                if (group.Count >= 5)
                {
                    // Create TNT at the tapped location
                    GameObject newTNT = Instantiate(tntPrefab, new Vector3(gridOrigin.x + coords.x * spriteSize, gridOrigin.y + coords.y * spriteSize, 0), Quaternion.identity);
                    newTNT.transform.localScale = new Vector3(spriteSize, spriteSize, 1);
                    newTNT.transform.SetParent(gridParent.transform);
                    // Update the grid array
                    grid[coords.x, coords.y] = newTNT;
                    newTNT.GetComponent<TNT>().SetCoords(coords.x, coords.y);
                }
            }
        }
        else if (tnt != null)
        {
            Debug.Log("TNT tapped.");
        }



        // Handle the falling and filling of cubes
        StartCoroutine(FallAndFill());
    }

    private IEnumerator FallAndFill()
    {
        List<(GameObject, Vector2Int)> fallPoints = CalculateFallPoints();

        foreach (var (obj, newCoords) in fallPoints)
        {
            IFallable fallableComponent = obj?.GetComponent<IFallable>();

            if (fallableComponent != null)
            {
                float delay = fallableComponent.GetCoords().y * fallDelayRow;

                // Get the old coordinates from the grid array.
                Vector2Int oldCoords = new Vector2Int(fallableComponent.GetCoords().x, fallableComponent.GetCoords().y);
                fallableComponent.FallTo(newCoords, fallSpeed, spriteSize, gridOrigin, delay);

                // Update the grid array.
                UpdateCell(oldCoords.x, oldCoords.y, null);
                UpdateCell(newCoords.x, newCoords.y, obj);
            }
        }

        yield return new WaitForSeconds(postFallDelay);
    }

    private List<(GameObject, Vector2Int)> CalculateFallPoints()
    {
        List<(GameObject, Vector2Int)> fallPoints = new List<(GameObject, Vector2Int)>();

        for (int x = 0; x < gridWidth; x++)
        {
            int emptyCount = 0;

            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    emptyCount++;
                }
                else
                {
                    Obstacle obstacle = grid[x, y]?.GetComponent<Obstacle>();
                    if (obstacle != null && (obstacle.obstacleType == Obstacle.ObstacleType.Box || obstacle.obstacleType == Obstacle.ObstacleType.Stone))
                    {
                        // Reset the empty count as we can't move cubes below this point.
                        emptyCount = 0;
                    }
                    else if (emptyCount > 0)
                    {
                        Vector2Int newFallPoint = new Vector2Int(x, y - emptyCount);
                        fallPoints.Add((grid[x, y], newFallPoint));
                    }
                }
            }
        }
        return fallPoints;
    }

    public void IncrementMovingCubeCount()
    {
        movingCubeCount++;
        UpdateIsAnyCubeMoving();
    }

    public void DecrementMovingCubeCount()
    {
        movingCubeCount--;
        UpdateIsAnyCubeMoving();
    }

    private void UpdateIsAnyCubeMoving()
    {
        isAnyCubeMoving = movingCubeCount > 0;
        if (!isAnyCubeMoving) CubeGroupFinder.ScanAndUpdateGrid(grid);

        // Debug.Log($"MovingCubeCount: {movingCubeCount} | isAnyCubeMoving: {isAnyCubeMoving}");
    }
}
