using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour, IFallable
{
    // Enum to specify the type of the obstacle (mostly for debugging or future reference)
    public enum ObstacleType
    {
        Box,
        Stone,
        Vase,
        // Add future types here
    }

    public ObstacleType obstacleType; // For debugging or future reference
    public int health; // Health of the obstacle
    public bool isAffectedByBlast; // Does it get damaged by regular blasts?
    public bool isAffectedByTNT; // Does it get damaged by TNT?
    public bool doesFallDown; // Does it fall down when there's an empty space below?

    public Sprite crackedVaseSprite; // Array to hold different sprite based on health
    private SpriteRenderer spriteRenderer; // Sprite renderer component

    [SerializeField] private GameObject particle;

    [field: SerializeField] public Vector2Int coords { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Setting properties based on type
        switch (obstacleType)
        {
            case ObstacleType.Box:
                isAffectedByBlast = true;
                isAffectedByTNT = true;
                doesFallDown = false;
                health = 1;
                break;
            case ObstacleType.Stone:
                isAffectedByBlast = false;
                isAffectedByTNT = true;
                doesFallDown = false;
                health = 1;
                break;
            case ObstacleType.Vase:
                isAffectedByBlast = true;
                isAffectedByTNT = true;
                doesFallDown = true;
                health = 2;
                break;
            // Add future types here with their properties
        }
    }

    public void SetCoords(int x, int y)
    {
        coords = new Vector2Int(x, y);
    }

    public Vector2Int GetCoords()
    {
        return coords;
    }

    // Method to handle damage taken by the obstacle
    public void TakeDamage(int damage, bool damageByTNT = false)
    {
        //Debug.Log($"Damage: {damage} Remaining HP: {health}");

        // Additional condition for Vase to ensure it doesn't take more than 1 damage from a single blast
        if (obstacleType == ObstacleType.Vase && damage > 1 && !damageByTNT)
        {
            damage = 1;
        }

        if (damageByTNT && !isAffectedByTNT)
            return; // If it's TNT damage and this obstacle isn't affected by TNT, then return
        if (!damageByTNT && !isAffectedByBlast)
            return; // If it's blast damage and this obstacle isn't affected by blast, then return

        health -= damage;

        // If health reaches zero, the obstacle is destroyed
        if (health <= 0)
        {
            GridManager.Instance.UpdateCell(coords.x, coords.y, null);
            GridManager.Instance.DecreaseObstacleCount(obstacleType);
            SpawnParticle();
            Destroy(gameObject);
        }

        // Update sprite if this is a vase
        if (obstacleType == ObstacleType.Vase && health == 1)
        {
            UpdateSprite();
        }
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = crackedVaseSprite;
    }

    public void FallTo(Vector2Int newCoords, float speed, float spriteSize, Vector2 gridOrigin, float delay,
        float originalHeight, float gap)
    {
        coords = newCoords;
        float distance = Vector3.Distance(transform.position,
            new Vector3(gridOrigin.x + newCoords.x * spriteSize, gridOrigin.y + newCoords.y * spriteSize, 0));
        float duration = distance / speed;
        StartCoroutine(FallCoroutine(newCoords, duration, spriteSize, gridOrigin, delay, originalHeight, gap));
    }

    private IEnumerator FallCoroutine(Vector2Int newCoords, float duration, float spriteSize, Vector2 gridOrigin,
        float delay, float originalHeight, float gap)
    {
        GridManager.Instance.IncrementMovingCubeCount();

        yield return new WaitForSeconds(delay);

        float additionalYOffset = newCoords.y >= originalHeight ? gap : 0;
        Vector3 targetPosition = new Vector3(gridOrigin.x + newCoords.x * spriteSize,
            gridOrigin.y + newCoords.y * spriteSize + additionalYOffset, 0);
        float elapsedTime = 0;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        GridManager.Instance.DecrementMovingCubeCount();
    }

    public void SpawnParticle()
    {
        var particleBox = Instantiate(particle,
            new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0),
            Quaternion.identity);
        Destroy(particleBox, 3);
    }
}