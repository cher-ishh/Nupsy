using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashPattern : MonoBehaviour
{
    [Header("보스 설정")]
    [SerializeField] private Transform bossTransform;
    [SerializeField] private List<SpriteRenderer> bossRenderers;

    [Header("히트박스")]
    [SerializeField] private GameObject smashHitbox;
    [SerializeField] private float hitboxActiveTime = 0.2f;

    [Header("패턴 설정")]
    [SerializeField] private float smashInterval = 10f;

    [Header("사운드")]
    [SerializeField] private AudioClip smashSound;
    private AudioSource audioSource;

    private Vector3 originalPosition;
    private List<Color> originalColors = new();

    private bool isActive = false;
    private bool isSmashing = false;
    private float smashTimer = 0f;

    private void Awake()
    {
        foreach (var r in bossRenderers)
        {
            originalColors.Add(r.color);
        }

        if (smashHitbox != null)
            smashHitbox.SetActive(false);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (!isActive || isSmashing) return;

        smashTimer += Time.deltaTime;

        if (smashTimer >= smashInterval)
        {
            StartCoroutine(DoSmash());
        }
    }

    public void EnablePattern()
    {
        isActive = true;
        smashTimer = 0f;
    }

    public void DisablePattern()
    {
        isActive = false;
        isSmashing = false;
        StopAllCoroutines();

        if (smashHitbox != null)
            smashHitbox.SetActive(false);
    }

    private IEnumerator DoSmash()
    {
        isSmashing = true;
        smashTimer = 0f;

        originalPosition = bossTransform.position;
        Vector3 upPosition = originalPosition + Vector3.up * 2f;

        // 경고 깜빡임
        for (int i = 0; i < 4; i++)
        {
            SetBossColor(Color.red);
            yield return new WaitForSeconds(0.1f);
            RestoreBossColor();
            yield return new WaitForSeconds(0.1f);
        }

        // 위로 이동
        yield return MoveTo(bossTransform, originalPosition, upPosition, 0.5f);

        // 잠시 정지
        yield return new WaitForSeconds(1f);

        // 내려찍기
        yield return MoveTo(bossTransform, upPosition, originalPosition, 0.2f);

        // 소리 재생
        PlaySmashSound();

        // 카메라 흔들림
        CameraShake();

        // 히트박스 활성화
        if (smashHitbox != null)
            StartCoroutine(ActivateHitbox());

        // 딜레이 후 정지
        yield return new WaitForSeconds(1f);
        bossTransform.position = originalPosition;

        isSmashing = false;
    }

    private void PlaySmashSound()
    {
        if (smashSound != null)
            audioSource.PlayOneShot(smashSound);
    }

    private IEnumerator ActivateHitbox()
    {
        smashHitbox.SetActive(true);
        yield return new WaitForSeconds(hitboxActiveTime);
        smashHitbox.SetActive(false);
    }

    private IEnumerator MoveTo(Transform obj, Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            obj.position = Vector3.Lerp(from, to, t / duration);
            yield return null;
        }
        obj.position = to;
    }

    private void SetBossColor(Color color)
    {
        foreach (var r in bossRenderers)
        {
            r.color = color;
        }
    }

    private void RestoreBossColor()
    {
        for (int i = 0; i < bossRenderers.Count; i++)
        {
            bossRenderers[i].color = originalColors[i];
        }
    }

    private void CameraShake()
    {
        var cam = Camera.main;
        if (cam == null) return;
        StartCoroutine(ShakeCoroutine(cam.transform, 0.15f, 0.2f));
    }

    private IEnumerator ShakeCoroutine(Transform cam, float duration, float magnitude)
    {
        Vector3 originalPos = cam.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cam.position = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.position = originalPos;
    }

    public bool IsSmashing() => isSmashing;
}
