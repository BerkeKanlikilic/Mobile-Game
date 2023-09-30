using UnityEngine;

public class LevelManager
{
    private int movesLeft;
    private GameObject[,] grid;
    private int originalHeight;

    public LevelManager(int moveCount, GameObject[,] grid, int originalHeight)
    {
        this.movesLeft = moveCount;
        this.grid = grid;
        this.originalHeight = originalHeight;
    }

    public void OnMoveMade(GameObject[,] updatedGrid)
    {
        movesLeft--;
        Debug.Log($"Remaining Moves: {movesLeft}");
        CheckGameStatus(updatedGrid);
    }

    private void CheckGameStatus(GameObject[,] updatedGrid)
    {
        if (movesLeft <= 0)
        {
            if (AreObstaclesLeft(updatedGrid))
            {
                HandleLoseCondition();
            }
            else
            {
                HandleWinCondition();
            }
        }
        else
        {
            if (!AreObstaclesLeft(updatedGrid))
            {
                HandleWinCondition();
            }
        }
    }

    public void UpdateGrid(GameObject[,] newGrid)
    {
        this.grid = newGrid;
    }

    private bool AreObstaclesLeft(GameObject[,] updatedGrid)
    {
        foreach (GameObject obj in updatedGrid)
        {
            if (obj != null && obj.GetComponent<Obstacle>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private void HandleWinCondition()
    {
        Debug.Log("You Won!");
        GridManager.Instance.UpdateTapAllowance(false);

        int currentLevel = GameManager.Instance.GetCurrentLevel();
        LevelDataManager.Instance?.SetLevelState(currentLevel, LevelState.Unlocked);

        UIManager.Instance.WinLoadingScene();
    }

    private void HandleLoseCondition()
    {
        Debug.Log("You Lost!");
        GridManager.Instance.UpdateTapAllowance(false);

        UIManager.Instance.LoseScene();
    }
}