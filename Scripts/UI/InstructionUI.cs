using UnityEngine;
using System.Collections;
using TMPro;

public class InstructionUI : MonoBehaviour
{
    [Header("설명 UI 오브젝트")]
    public GameObject instructionPanel;
    public TextMeshProUGUI instructionText;

    [Header("표시할 설명 텍스트")]
    [TextArea] public string message;

    [Header("자동 표시 조건")]
    public string expectedPreviousScene;
    public float autoShowDelay = 1f;

    [Header("사운드")]
    public AudioClip toggleSound;
    private AudioSource audioSource;

    private void Start()
    {
        instructionPanel.SetActive(false);

        if (instructionText != null)
            instructionText.text = message;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (GameManager.Instance != null &&
            GameManager.Instance.previousScene == expectedPreviousScene)
        {
            StartCoroutine(ShowInstructionDelayed(autoShowDelay));
        }
    }

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
            return;

        // 엔터 또는 마우스 우클릭 → 설명 UI 끄기
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(1)) && instructionPanel.activeSelf)
        {
            PlayToggleSound();
            StartCoroutine(HideWithScale());
            Time.timeScale = 1f;
        }

        // ESC 또는 마우스 가운데 클릭 → 설명 UI 켜기
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2)) && !instructionPanel.activeSelf)
        {
            PlayToggleSound();
            StartCoroutine(ShowWithScale());
            Time.timeScale = 0f;
        }
    }

    private IEnumerator ShowInstructionDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        instructionPanel.SetActive(true);
    }

    private IEnumerator ShowWithScale()
    {
        instructionPanel.SetActive(true);
        Transform panelTransform = instructionPanel.transform;
        panelTransform.localScale = Vector3.zero;

        while (panelTransform.localScale.x < 1f)
        {
            panelTransform.localScale += new Vector3(0.1f, 0.1f, 0f);
            yield return null;
        }

        panelTransform.localScale = Vector3.one;
    }

    private IEnumerator HideWithScale()
    {
        Transform panelTransform = instructionPanel.transform;

        while (panelTransform.localScale.x > 0f)
        {
            panelTransform.localScale -= new Vector3(0.1f, 0.1f, 0f);
            yield return null;
        }

        panelTransform.localScale = Vector3.zero;
        instructionPanel.SetActive(false);
    }

    private void PlayToggleSound()
    {
        if (toggleSound != null)
            audioSource.PlayOneShot(toggleSound);
    }
}
