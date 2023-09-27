using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFallable
{
    Vector2Int GetCoords();
    void FallTo(Vector2Int newCoords, float fallSpeed, float spriteSize, Vector2 gridOrigin, float delay);
}