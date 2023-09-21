using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Here, we multiply by -100 to convert the float to an int, assuming we don't need more precision.
        // This sets objects higher on the screen to render behind those lower on the screen.
        spriteRenderer.sortingOrder = (int)(transform.position.y * 100);
    }
}
