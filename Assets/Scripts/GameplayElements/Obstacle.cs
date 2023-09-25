using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Enum to specify the type of the obstacle (mostly for debugging or future reference)
    public enum ObstacleType
    {
        Box,
        Stone,
        Vase,
        // Add future types here
    }

    public ObstacleType type; // For debugging or future reference
    public int health; // Health of the obstacle
    public bool isAffectedByBlast; // Does it get damaged by regular blasts?
    public bool isAffectedByTNT; // Does it get damaged by TNT?
    public bool doesFallDown; // Does it fall down when there's an empty space below?

    public Sprite[] sprites; // Array to hold different sprites based on health
    private SpriteRenderer spriteRenderer; // Sprite renderer component

    public GridManager gridManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Setting properties based on type
        switch (type)
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

    // Method to handle damage taken by the obstacle
    public void TakeDamage(int damage, bool damageByTNT = false)
    {
        Debug.Log($"Damage: {damage} Remaining HP: {health}");

        // Additional condition for Vase to ensure it doesn't take more than 1 damage from a single blast
        if (type == ObstacleType.Vase && damage > 1 && !damageByTNT)
        {
            damage = 1;
        }

        if (damageByTNT && !isAffectedByTNT) return; // If it's TNT damage and this obstacle isn't affected by TNT, then return
        if (!damageByTNT && !isAffectedByBlast) return; // If it's blast damage and this obstacle isn't affected by blast, then return

        health -= damage;

        // If health reaches zero, the obstacle is destroyed
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        // Update sprite if this is a vase
        if (type == ObstacleType.Vase)
        {
            UpdateSprite();
        }
    }

    private void UpdateSprite()
    {
        if (health > 0 && health <= sprites.Length)
        {
            spriteRenderer.sprite = sprites[health - 1];
        }
    }

    public void HandleFall()
    {
        // Implement the logic for the obstacle to fall down to the empty cell below.
        // Only for obstacles that fall, i.e., Vase.
        if (doesFallDown)
        {
            // Add falling logic here.
        }
    }

    // Other methods and logic for the Obstacle can go here
}
