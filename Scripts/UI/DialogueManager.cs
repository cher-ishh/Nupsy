using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.03f;

    [Header("선택지 버튼")]
    public GameObject buttonYes;
    public GameObject buttonNo;

    [Header("사운드")]
    public AudioClip clickSound;
    private AudioSource audioSource;

    private Coroutine typingCoroutine;
    private bool isTyping;
    private string currentSentence = "";

    private UnityAction onYesAction;
    private UnityAction onNoAction;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (buttonYes != null) buttonYes.SetActive(false);
        if (buttonNo != null) buttonNo.SetActive(false);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void ShowDialogue(string sentence)
    {
        if (isTyping) return;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        currentSentence = sentence;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    public void ShowChoiceDialogue(string sentence, UnityAction onYes, UnityAction onNo)
    {
        ShowDialogue(sentence);

        onYesAction = onYes;
        onNoAction = onNo;

        if (buttonYes != null) buttonYes.SetActive(true);
        if (buttonNo != null) buttonNo.SetActive(true);

        var yesBtn = buttonYes.GetComponent<UnityEngine.UI.Button>();
        var noBtn = buttonNo.GetComponent<UnityEngine.UI.Button>();

        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();

        yesBtn.onClick.AddListener(OnClickYes);
        noBtn.onClick.AddListener(OnClickNo);
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void HideDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        isTyping = false;
        currentSentence = "";

        if (buttonYes != null) buttonYes.SetActive(false);
        if (buttonNo != null) buttonNo.SetActive(false);
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }

    private void Update()
    {
        if (!IsDialogueActive()) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(1))
        {
            PlayClickSound();

            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentSentence;
                isTyping = false;
            }
            else
            {
                HideDialogue();
            }
        }
    }

    private void OnClickYes()
    {
        PlayClickSound();
        onYesAction?.Invoke();
        HideDialogue();
    }

    private void OnClickNo()
    {
        PlayClickSound();
        onNoAction?.Invoke();
        HideDialogue();
    }

    private void PlayClickSound()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}
