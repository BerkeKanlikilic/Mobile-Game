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
    public TextAsset[] levelTextAssets; // Drag your JSON TextAssets here in the Unity Editor

    public LevelData LoadLevel(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > levelTextAssets.Length)
        {
            Debug.LogError("Invalid level number.");
            return null;
        }

        TextAsset jsonText = levelTextAssets[levelNumber - 1];

        // Deserialize the JSON to a LevelData object
        LevelData levelData = JsonUtility.FromJson<LevelData>(jsonText.text);

        return levelData;
    }
}
