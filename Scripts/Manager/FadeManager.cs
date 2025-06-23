using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [Header("페이드 설정")]
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (fadeImage != null)
                SetAlpha(1f); // 시작 시 화면을 어둡게
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (fadeImage != null)
            StartCoroutine(Fade(1f, 0f)); // 씬 시작 시 페이드 인
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeRoutine(sceneName));
    }

    // 씬 전환 시: 페이드 아웃 → 씬 로드 → 페이드 인
    private IEnumerator FadeRoutine(string sceneName)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.previousScene = SceneManager.GetActiveScene().name;
            Debug.Log("저장된 이전 씬: " + GameManager.Instance.previousScene);
        }

        yield return StartCoroutine(Fade(0f, 1f));
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Fade(1f, 0f));
    }

    // 알파값을 점진적으로 변경하여 페이드 효과 구현
    private IEnumerator Fade(float from, float to)
    {
        if (fadeImage == null) yield break;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(to);
    }

    // 페이드 이미지의 알파값 설정
    private void SetAlpha(float alpha)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        fadeImage.color = new Color(c.r, c.g, c.b, alpha);
    }
}
