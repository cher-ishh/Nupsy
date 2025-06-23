using System.Collections;
using UnityEngine;

public class FallingPattern : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private GameObject fallingPrefab;
    [SerializeField] private float spawnInterval = 2f;

    [Header("스폰 범위")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float spawnY = 6f;

    private bool isSpawning = false;
    private Coroutine spawnRoutine;

    public void EnablePattern()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            spawnRoutine = StartCoroutine(SpawnFallingEnemies());
        }
    }

    public void DisablePattern()
    {
        if (isSpawning)
        {
            isSpawning = false;

            if (spawnRoutine != null)
                StopCoroutine(spawnRoutine);
        }
    }

    private IEnumerator SpawnFallingEnemies()
    {
        while (isSpawning)
        {
            SpawnFallingEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnFallingEnemy()
    {
        if (fallingPrefab == null) return;

        float x = Random.Range(minX, maxX);
        Vector2 spawnPosition = new Vector2(x, spawnY);
        float randomAngle = Random.Range(-45f, 45f);

        GameObject obj = Instantiate(fallingPrefab, spawnPosition, Quaternion.identity);
        obj.GetComponent<FallingEnemy>()?.SetDirection(Vector2.down, randomAngle);
    }
}
