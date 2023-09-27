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

    // Call this method whenever a move is made in the game.
    public void OnMoveMade(GameObject[,] updatedGrid)
    {
        movesLeft--;
        Debug.Log($"Remaining Moves: {movesLeft}");
        CheckGameStatus(updatedGrid);
    }

    // Private method to check the game status
    private void CheckGameStatus(GameObject[,] updatedGrid)
    {
        if (movesLeft <= 0)
        {
            if (AreObstaclesLeft(updatedGrid))
            {
                // Handle the lose condition
                HandleLoseCondition();
            }
            else
            {
                // Handle the win condition
                HandleWinCondition();
            }
        } else {
            if(!AreObstaclesLeft(updatedGrid)){
                HandleWinCondition();
            }
        }
    }

    public void UpdateGrid(GameObject[,] newGrid)
    {
        this.grid = newGrid;
    }

    // Check if there are any obstacles left on the grid
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

    // Handle the win condition
    private void HandleWinCondition()
    {
        Debug.Log("You Won!");
        // Implement further logic for handling win condition
    }

    // Handle the lose condition
    private void HandleLoseCondition()
    {
        Debug.Log("You Lost!");
        // Implement further logic for handling lose condition
    }
}