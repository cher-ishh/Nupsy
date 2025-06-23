using System.Collections;
using UnityEngine;
public class InteractionTrigger : MonoBehaviour
{
    [Header("상호작용 설정")]
    [TextArea] public string interactionMessage;

    [Header("씬 전환 설정")]
    public string nextScene;
    public float transitionDelay = 1.5f;

    [Header("UI")]
    public GameObject icon;

    [Header("사운드")]
    public AudioClip interactSound;
    private AudioSource audioSource;

    private bool isPlayerInRange;
    private bool isTransitioning;

    private void Start()
    {
        SetIcon(false);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (CanInteract() && Input.GetKeyDown(KeyCode.Space))
        {
            HandleInteraction();
        }
    }

    private bool CanInteract()
    {
        return isPlayerInRange &&
               !isTransitioning &&
               !DialogueManager.Instance.IsDialogueActive();
    }

    private void HandleInteraction()
    {
        PlaySound();

        if (!string.IsNullOrEmpty(interactionMessage))
        {
            // 선택지 있는 대화
            if (DialogueManager.Instance.buttonYes != null && DialogueManager.Instance.buttonNo != null)
            {
                DialogueManager.Instance.ShowChoiceDialogue(
                    interactionMessage,
                    () =>
                    {
                        StartCoroutine(TransitionScene());
                    },
                    () =>
                    {
                        DialogueManager.Instance.HideDialogue();
                    }
                );
            }
            else
            {
                // 일반 대화
                DialogueManager.Instance.ShowDialogue(interactionMessage);
            }
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
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
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
            SetIcon(false);
        }
    }

    private void SetIcon(bool state)
    {
        if (icon != null)
            icon.SetActive(state);
    }

    private void PlaySound()
    {
        if (interactSound != null)
            audioSource.PlayOneShot(interactSound);
    }
}
