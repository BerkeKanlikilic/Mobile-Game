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
    [SerializeField] private float fallDelay = 0.0f;
    [SerializeField] private float fallSpeed = 1f;
    [SerializeField] private float fallDuration = 0.5f;

    // Debug
    private List<Vector3> debugFallPoints = new List<Vector3>();

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
                        cubeComponent.setCoords(x, y);
                    } else if (obstacleComponent != null)
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
                        obstacleComponent.setCoords(x, y);
                    } else if (tntComponent != null){
                        tntComponent.setCoords(x,y);
                    }
                    grid[x, y] = newCell;
                }

                index++;
            }
        }

        CubeGroupFinder.ScanAndUpdateGrid(grid);

        // Debugging output
        // for (int y = 0; y < gridHeight; y++)
        // {
        //    for (int x = 0; x < gridWidth; x++)
        //    {
        //        Debug.Log($"Grid[{x},{y}] = {grid[x, y]?.name ?? "Empty"}");
        //    }
        // }
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
                Debug.LogError("Unknown item code: " + itemCode);
                return null;
        }
    }

    public void UpdateCell(int x, int y, GameObject newCell)
    {
        // Debug.Log($"Updated Cell: {x}, {y}");
        grid[x, y] = newCell;
    }

    public void HandleTap(GameObject cell)
    {
        if(isFalling) return;

        Cube cube = cell?.GetComponent<Cube>();
        TNT tnt = cell?.GetComponent<TNT>();

        if (cube != null)
        {
            Vector2Int coords = cube.getCoords();

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
                    newTNT.GetComponent<TNT>().setCoords(coords.x, coords.y);
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


    private IEnumerator FallAndFill()
    {
        isFalling = true;
        List<(GameObject, Vector2Int)> fallPoints = CalculateFallPoints();

        foreach (var (obj, newCoords) in fallPoints)
        {
            Cube cubeComponent = obj?.GetComponent<Cube>();
            Obstacle obstacleComponent = obj?.GetComponent<Obstacle>();
            TNT tntComponent = obj?.GetComponent<TNT>();

            if (cubeComponent != null)
            {
                // Get the old coordinates from the grid array.
                Vector2Int oldCoords = new Vector2Int(cubeComponent.getCoords().x, cubeComponent.getCoords().y);
                cubeComponent.FallTo(newCoords, fallDuration, spriteSize, gridOrigin);

                // Update the grid array.
                UpdateCell(oldCoords.x, oldCoords.y, null);
                UpdateCell(newCoords.x, newCoords.y, obj);
            }
            else if (obstacleComponent != null)
            {
                // Get the old coordinates from the grid array.
                Vector2Int oldCoords = new Vector2Int(obstacleComponent.getCoords().x, obstacleComponent.getCoords().y);
                obstacleComponent.FallTo(newCoords, fallDuration, spriteSize, gridOrigin);

                // Update the grid array.
                UpdateCell(oldCoords.x, oldCoords.y, null);
                UpdateCell(newCoords.x, newCoords.y, obj);
            }
            else if (tntComponent != null)
            {
                // Get the old coordinates from the grid array.
                Vector2Int oldCoords = new Vector2Int(tntComponent.getCoords().x, tntComponent.getCoords().y);
                tntComponent.FallTo(newCoords, fallDuration, spriteSize, gridOrigin);

                // Update the grid array.
                UpdateCell(oldCoords.x, oldCoords.y, null);
                UpdateCell(newCoords.x, newCoords.y, obj);
            }
        }

        yield return new WaitForSeconds(fallDuration);
        isFalling = false;
        CubeGroupFinder.ScanAndUpdateGrid(grid);
    }
}
