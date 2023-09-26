using System.Collections;
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
    public bool isTNTActive = false;

    [field: SerializeField] public Vector2Int coords { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void setCoords(int x, int y)
    {
        coords = new Vector2Int(x, y);
    }

    public Vector2Int getCoords()
    {
        return coords;
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
        isTNTActive = true;
    }

    public void DeactivateTNTSprite()
    {
        spriteRenderer.sprite = cubeSprites[(int)cubeType];
        isTNTActive = false;
    }

    private void OnMouseDown()
    {
        // Notify GridManager that this cube was tapped
        GridManager.instance.HandleTap(gameObject);
    }

    public void FallTo(Vector2Int newCoords, float duration, float spriteSize, Vector2 gridOrigin)
    {
        coords = newCoords;
        StartCoroutine(FallCoroutine(newCoords, duration, spriteSize, gridOrigin));
    }

    private IEnumerator FallCoroutine(Vector2Int newCoords, float duration, float spriteSize, Vector2 gridOrigin)
    {
        Vector3 targetPosition = new Vector3(gridOrigin.x + newCoords.x * spriteSize, gridOrigin.y + newCoords.y * spriteSize, 0);
        float elapsedTime = 0;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
