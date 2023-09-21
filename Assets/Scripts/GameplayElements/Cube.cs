using UnityEngine;

public class Cube : MonoBehaviour
{
    public enum CubeType
    {
        Red,
        Green,
        Blue,
        Yellow,
        Random
    }

    public Sprite[] cubeSprites; // 0: Red, 1: Green, 2: Blue, 3: Yellow
    public CubeType cubeType;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateSprite()
    {
        if (cubeType == CubeType.Random)
        {
            cubeType = (CubeType)Random.Range(0, 4); // Randomly set to Red, Green, Blue, or Yellow
        }

        spriteRenderer.sprite = cubeSprites[(int)cubeType];
    }
}
