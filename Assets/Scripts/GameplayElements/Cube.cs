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
    public Sprite[] tntCubeSprites; // TNT versions of the sprites
    public CubeType cubeType;
    private SpriteRenderer spriteRenderer;

    // Variable to keep track if the TNT sprite is active
    private bool isTNTActive = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (cubeType == CubeType.Random)
        {
            cubeType = (CubeType)Random.Range(0, 4); // Randomly set to Red, Green, Blue, or Yellow
        }

        spriteRenderer.sprite = isTNTActive ? tntCubeSprites[(int)cubeType] : cubeSprites[(int)cubeType];
    }

    // Methods to activate and deactivate TNT sprites
    public void ActivateTNTSprite()
    {
        spriteRenderer.sprite = tntCubeSprites[(int)cubeType];
    }

    public void DeactivateTNTSprite()
    {
        spriteRenderer.sprite = cubeSprites[(int)cubeType];
    }

    private void OnMouseDown()
    {
        // Notify GridManager that this cube was tapped
        GridManager.instance.CubeTapped(this);
    }
}
