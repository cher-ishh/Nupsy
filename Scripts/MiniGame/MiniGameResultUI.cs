using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MiniGameResultUI : MonoBehaviour
{
    [Header("결과 이미지")]
    [SerializeField] private GameObject gameClearImage;
    [SerializeField] private GameObject gameOverImage;

    [Header("사운드")]
    [SerializeField] private AudioClip gameClearSound;
    [SerializeField] private AudioClip gameOverSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void ShowGameClearUI()
    {
        if (gameClearSound != null)
            audioSource.PlayOneShot(gameClearSound);

        if (gameClearImage != null)
            StartCoroutine(ShowResultEffect(gameClearImage));

        DialogueManager.Instance?.ShowChoiceDialogue(
            "보스를 처치했습니다! 다시 도전할까요?",
            () => SceneManager.LoadScene("Mini Game"),
            () => SceneManager.LoadScene("Home"));
    }

    public void ShowGameOverUI()
    {
        if (gameOverSound != null)
            audioSource.PlayOneShot(gameOverSound);

        if (gameOverImage != null)
            StartCoroutine(ShowResultEffect(gameOverImage));

        DialogueManager.Instance?.ShowChoiceDialogue(
            "당신은 쓰러졌습니다... 다시 도전할까요?",
            () => SceneManager.LoadScene("Mini Game"),
            () => SceneManager.LoadScene("Home"));
    }

    private IEnumerator ShowResultEffect(GameObject target)
    {
        if (target == null) yield break;

        target.SetActive(true);
        Transform tf = target.transform;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = new Vector3(0.75f, 0.75f, 0.75f);
        tf.localScale = startScale;
        tf.rotation = Quaternion.identity;

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            tf.localScale = Vector3.Lerp(startScale, endScale, t);
            float zRotation = Mathf.Lerp(0f, 720f, t);
            tf.rotation = Quaternion.Euler(0f, 0f, zRotation);

            yield return null;
        }

        tf.localScale = endScale;
        tf.rotation = Quaternion.identity;
    }
}
