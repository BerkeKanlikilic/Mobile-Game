using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour
{
    [field: SerializeField] public Vector2Int coords { get; private set; }

    public void setCoords(int x, int y)
    {
        coords = new Vector2Int(x, y);
    }

    public Vector2Int getCoords()
    {
        return coords;
    }

    private void OnMouseDown()
    {
        // Notify GridManager that this TNT was tapped
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
