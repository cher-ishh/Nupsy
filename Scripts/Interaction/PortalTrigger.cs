using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class PortalTrigger : MonoBehaviour
{
    [Header("씬 이동 설정")]
    public string nextScene;
    public float transitionDelay = 1.5f;

    [Header("잠긴 상태 메시지")]
    [TextArea] public string lockedMessage;

    [Header("UI 아이콘")]
    public GameObject icon;
    [SerializeField] private TMP_InputField inputField;

    [Header("사운드")]
    public AudioClip portalSound;
    private AudioSource audioSource;

    private bool isPlayerInRange;
    private bool hasInteracted;
    private bool isTransitioning;

    private void Start()
    {
        SetIcon(false);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (CanInteract() && Input.GetKeyDown(KeyCode.UpArrow))
        {
            HandlePortal();
        }

        if (hasInteracted &&
            !string.IsNullOrEmpty(nextScene) &&
            !DialogueManager.Instance.IsDialogueActive() &&
            !isTransitioning)
        {
            hasInteracted = false;
            StartCoroutine(TransitionScene());
        }
    }

    private bool CanInteract()
    {
        return isPlayerInRange &&
               !isTransitioning &&
               !DialogueManager.Instance.IsDialogueActive();
    }

    private void HandlePortal()
    {
        hasInteracted = true;

        // 소리 재생
        if (portalSound != null)
            audioSource.PlayOneShot(portalSound);

        if (!string.IsNullOrEmpty(nextScene))
        {
            StartCoroutine(TransitionScene());
        }
        else if (!string.IsNullOrEmpty(lockedMessage))
        {
            DialogueManager.Instance.ShowDialogue(lockedMessage);
            inputField.DeactivateInputField();
            inputField.gameObject.SetActive(false);
        }
    }

    private IEnumerator TransitionScene()
    {
        isTransitioning = true;

        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeToScene(nextScene);
            yield return new WaitForSeconds(transitionDelay);
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            SetIcon(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            hasInteracted = false;
            SetIcon(false);
        }
    }

    private void SetIcon(bool state)
    {
        if (icon != null)
            icon.SetActive(state);
    }
}
