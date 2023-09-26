using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] grid;
}

public class LevelLoader : MonoBehaviour
{
    public TextAsset[] levelTextAssets;

    public void LoadLevel(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > levelTextAssets.Length)
        {
            Debug.LogError("Invalid level number.");
            return;
        }
        Debug.Log($"Level {levelNumber} is Loading...");

        TextAsset jsonText = levelTextAssets[levelNumber - 1];

        // Deserialize the JSON to a LevelData object
        LevelData levelData = JsonUtility.FromJson<LevelData>(jsonText.text);

        if(levelData != null) {
            Debug.Log("Complete!");
            GridManager.instance.InitializeGrid(levelData);
        }
        else {
            Debug.LogError("Level Data could not be loaded!");
        }
    }
}
