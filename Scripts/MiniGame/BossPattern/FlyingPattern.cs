using System.Collections;
using UnityEngine;

public class FlyingPattern : MonoBehaviour
{
    [Header("프리팹 설정")]
    [SerializeField] private GameObject[] flyingPrefabs;
    [SerializeField] private float spawnInterval = 3f;

    [Header("스폰 범위")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = 0f;
    [SerializeField] private float maxY = 4f;

    private bool isSpawning = false;
    private Coroutine spawnRoutine;

    public void EnablePattern()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            spawnRoutine = StartCoroutine(SpawnFlyingObjects());
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

    private IEnumerator SpawnFlyingObjects()
    {
        while (isSpawning)
        {
            if (flyingPrefabs.Length == 0)
            {
                Debug.LogWarning("FlyingPattern: 프리팹 배열이 비어 있음!");
                yield break;
            }

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            Vector3 spawnPos = new Vector3(x, y, 0f);

            int index = Random.Range(0, flyingPrefabs.Length);
            Instantiate(flyingPrefabs[index], spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
