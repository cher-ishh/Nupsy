using System.Collections;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("보스 파츠들")]
    public Transform[] bossParts;

    [Header("이동 설정")]
    public float bounceHeight = 0.1f;
    public float bounceDuration = 0.3f;
    public float intervalBetweenParts = 0.15f;

    [Header("스매시 체크")]
    public SmashPattern smashPattern;

    private Vector3[] originalPositions;

    private void Start()
    {
        originalPositions = new Vector3[bossParts.Length];

        for (int i = 0; i < bossParts.Length; i++)
        {
            originalPositions[i] = bossParts[i].localPosition;
        }

        StartCoroutine(BounceLoop());
    }

    private IEnumerator BounceLoop()
    {
        while (true)
        {
            if (smashPattern == null || !smashPattern.IsSmashing())
            {
                for (int i = 0; i < bossParts.Length; i++)
                {
                    StartCoroutine(BounceOnePart(bossParts[i], originalPositions[i]));
                    yield return new WaitForSeconds(intervalBetweenParts);
                }

                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator BounceOnePart(Transform part, Vector3 originalPos)
    {
        Vector3 upPos = originalPos + Vector3.up * bounceHeight;
        float t = 0f;

        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            float rate = t / bounceDuration;
            part.localPosition = Vector3.Lerp(originalPos, upPos, rate);
            yield return null;
        }

        t = 0f;

        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            float rate = t / bounceDuration;
            part.localPosition = Vector3.Lerp(upPos, originalPos, rate);
            yield return null;
        }

        part.localPosition = originalPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage();
            }
        }
    }
}
