using System.Collections;
using UnityEngine;

public class OpeningSceneController : MonoBehaviour
{
    [Header("씬 설정")]
    public string nextSceneName;

    [Header("오브젝트")]
    public Transform titleObject;
    public GameObject startButton;

    [Header("이동 설정")]
    public Vector3 targetPosition;
    public float moveDuration = 1f;

    [Header("사운드")]
    public AudioClip clickSound;
    private AudioSource audioSource;

    private void Start()
    {
        // 오디오소스 추가 및 설정
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        startButton.SetActive(false);
        StartCoroutine(TitleSequence());
    }

    // 타이틀 이동 → 버튼 등장 순서 진행
    private IEnumerator TitleSequence()
    {
        yield return new WaitForSeconds(1f);

        Vector3 startPos = titleObject.position;
        Vector3 startScale = Vector3.one * 0.5f; // 시작 크기 절반
        Vector3 targetScale = Vector3.one;       // 최종 크기
        float startRotation = 0f;
        float targetRotation = 360f;

        titleObject.position = startPos;
        titleObject.localScale = startScale;
        titleObject.rotation = Quaternion.Euler(0f, 0f, startRotation);

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;

            // 위치 이동
            titleObject.position = Vector3.Lerp(startPos, targetPosition, t);

            // 크기 확대
            titleObject.localScale = Vector3.Lerp(startScale, targetScale, t);

            // Z축 회전
            float zRot = Mathf.Lerp(startRotation, targetRotation, t);
            titleObject.rotation = Quaternion.Euler(0f, 0f, zRot);

            yield return null;
        }

        // 마지막 위치/크기/회전값 정확히 고정
        titleObject.position = targetPosition;
        titleObject.localScale = targetScale;
        titleObject.rotation = Quaternion.Euler(0f, 0f, targetRotation);

        yield return new WaitForSeconds(1f);
        StartCoroutine(FadeInButton());
    }

    // 버튼 서서히 알파값, 크기 증가
    private IEnumerator FadeInButton(float duration = 1f)
    {
        startButton.SetActive(true);

        SpriteRenderer sr = startButton.GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = 0f;
        sr.color = color;

        // 크기 관련 설정
        Transform buttonTransform = startButton.transform;
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 targetScale = Vector3.one;
        buttonTransform.localScale = startScale;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // 알파값 증가
            color.a = Mathf.Clamp01(t);
            sr.color = color;

            // 크기 증가
            buttonTransform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        // 마지막 보정
        color.a = 1f;
        sr.color = color;
        buttonTransform.localScale = targetScale;
    }


    public void OnStartButtonClicked()
    {
        // 클릭 사운드 재생
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);

        // 씬 전환
        FadeManager.Instance.FadeToScene(nextSceneName);
    }
}
