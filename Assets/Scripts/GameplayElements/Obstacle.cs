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

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ... existing logic to set initial health based on type
    }

    // Method to handle damage taken by the obstacle
    public void TakeDamage(int damage, bool damageByTNT = false)
    {
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

    // Method to handle the falling behavior could go here

    // Other methods and logic for the Obstacle can go here
}
