using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour, IFallable
{
    [SerializeField] private GameObject explodeParticle;
    [field: SerializeField] public Vector2Int coords { get; private set; }

    public void SetCoords(int x, int y)
    {
        coords = new Vector2Int(x, y);
    }

    public Vector2Int GetCoords()
    {
        return coords;
    }

    private void OnMouseDown()
    {
        // Do not handle tap if any cube is moving
        if (GridManager.instance.isAnyCubeMoving) return;

        // Notify GridManager that this TNT was tapped
        GridManager.instance.HandleCellTap(gameObject);
    }

    public void FallTo(Vector2Int newCoords, float speed, float spriteSize, Vector2 gridOrigin, float delay, float originalHeight, float gap)
    {
        coords = newCoords;
        float distance = Vector3.Distance(transform.position, new Vector3(gridOrigin.x + newCoords.x * spriteSize, gridOrigin.y + newCoords.y * spriteSize, 0));
        float duration = distance / speed;
        StartCoroutine(FallCoroutine(newCoords, duration, spriteSize, gridOrigin, delay, originalHeight, gap));
    }

    private IEnumerator FallCoroutine(Vector2Int newCoords, float duration, float spriteSize, Vector2 gridOrigin, float delay, float originalHeight, float gap)
    {
        GridManager.instance.IncrementMovingCubeCount();

        yield return new WaitForSeconds(delay);

        float additionalYOffset = newCoords.y >= originalHeight ? gap : 0;
        Vector3 targetPosition = new Vector3(gridOrigin.x + newCoords.x * spriteSize, gridOrigin.y + newCoords.y * spriteSize + additionalYOffset, 0);
        float elapsedTime = 0;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        GridManager.instance.DecrementMovingCubeCount();
    }

    public void SpawnParticle()
    {
        GameObject particle = Instantiate(explodeParticle, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0), Quaternion.identity);
        //Destroy(particle, 1);
    }
}
