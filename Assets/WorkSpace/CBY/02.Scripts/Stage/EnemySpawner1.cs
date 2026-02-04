using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    Transform GetPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    public Enemy Spawn(GameObject prefab, bool isBoss)
    {
        GameObject obj = Instantiate(
            prefab,
            GetPoint().position,
            Quaternion.identity
        );

        return obj.GetComponent<Enemy>();
    }
}