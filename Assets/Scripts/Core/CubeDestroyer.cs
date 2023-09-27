using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDestroyer
{
    public static void DestroyGroup(List<Cube> group){
        foreach(Cube cube in group){
            GridManager.instance.UpdateCell(cube.GetCoords().x, cube.GetCoords().y, null);
            Object.Destroy(cube.gameObject);

            // TODO: Animations.
        }
    }

    public static void DealDamageToObstacles(List<Obstacle> obstacles, int damage, bool damageByTNT = false){
        foreach(Obstacle obstacle in obstacles){
            obstacle.TakeDamage(damage, damageByTNT);
        }
    }
}
