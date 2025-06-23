using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class LLMNpc : MonoBehaviour
{
    [Header("NPC 설정")]
    [TextArea] public string npcPersona;
    [SerializeField] private string apiKey;

    [Header("UI")]
    public GameObject icon;
    [SerializeField] private TMP_InputField inputField;

    private bool isPlayerInRange;
    private bool isTalking;
    private string conversationHistory = "";

    private string relationship;
    private string emotion;

    private string apiUrl => $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent?key={apiKey}";

    private void Start()
    {
        SetIcon(false);

        conversationHistory = "";
        
        if (inputField != null)
            inputField.interactable = false; // 처음엔 비활성화
    }

    private void Update()
    {
        if (!isPlayerInRange || isTalking) return;

        // 스페이스 키로 대화 시작 (입력창 ON)
        if (!inputField.interactable && Input.GetKeyDown(KeyCode.Space))
        {
            inputField.interactable = true;
            inputField.ActivateInputField(); // 포커스 줌
            DialogueManager.Instance.ShowDialogue("대화를 입력하세요...");
        }

        // 입력창에 포커스가 있을 때, ALT 키로 메시지 전송
        if (inputField.interactable && inputField.isFocused && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            string input = inputField.text.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                inputField.text = "";
                inputField.DeactivateInputField();
                inputField.gameObject.SetActive(false);

                StartCoroutine(StartConversation(input));
            }
        }

        // 대화 종료: 엔터 → 대화창 숨김 + 입력창 비활성화
        if (DialogueManager.Instance.IsDialogueActive() && Input.GetKeyDown(KeyCode.Return))
        {
            DialogueManager.Instance.HideDialogue();
            inputField.interactable = false;
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
            inputField.interactable = false;
            DialogueManager.Instance.HideDialogue();
        }
    }

    private void SetIcon(bool state)
    {
        if (icon != null)
            icon.SetActive(state);
    }

    private IEnumerator StartConversation(string userPrompt)
    {
        isTalking = true;

        yield return StartCoroutine(SendChatRequest(userPrompt));

        isTalking = false;
    }

    private IEnumerator SendChatRequest(string userMessage)
    {
        DialogueManager.Instance.ShowDialogue("응답 중...");

        conversationHistory += $"플레이어: {userMessage}\n";

        if (conversationHistory.Length > 1000)
        {
            yield return StartCoroutine(SendSummaryRequest(conversationHistory));
        }

        string systemPrompt =
            $"당신은 플레이어와 감정적으로 관계를 맺는 NPC입니다. 모든 대답은 반말로 해야합니다. 말투는 항상 똑같이 유지해주세요." +
            $"이 게임은 농장형 RPG로 당신과 플레이어는 같은 마을의 주민입니다. 마을은 나무가 많은 숲속 마을이며 남쪽에는 바다가 있습니다. 마을에는 상점이 있으며 현재는 공사중입니다. 마을에는 던전 또한 존재하며 마왕이 살고있습니다. 던전 또한 현재는 접근이 불가능한 상태입니다. 플레이어의 집에는 게임기가 존재하며 미니게임을 플레이할 수 있습니다." +
            $"아래는 당신의 성격과 역할 설명이며, 플레이어와의 친밀도 및 감정 변화에 따라 반응이 달라질 수 있습니다. 게임에 어울리는 대화 위주로 시작해주세요.\n\n" +
            $"성격: {npcPersona}\n" +
            $"친밀도 상태: {relationship}\n" +
            $"최근 감정 인상: {emotion}\n\n" +
            $"지금까지의 대화:\n{conversationHistory}\n\n" +
            "[감정]: (플레이어에게 느끼는 감정을 세 단어 이내로 작성)\n" +
            "[친밀도]: (현재 관계 상태를 한 단어로 작성)\n" +
            "[응답]: (50자 이내의 간단한 대사)\n" +
            "이모티콘, 특수기호는 절대 사용하지 마세요.";

        string jsonBody = "{\"contents\":["
            + "{\"role\":\"user\",\"parts\":[{\"text\":\"" + systemPrompt + "\"}]},"
            + "{\"role\":\"user\",\"parts\":[{\"text\":\"" + userMessage + "\"}]}"
            + "]}";

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            DialogueManager.Instance.ShowDialogue("에러: " + request.error);
            yield break;
        }

        string jsonResult = request.downloadHandler.text;

        string fullReply = ParseGeminiResponse(jsonResult);

        // [감정], [친밀도], [응답] 파싱
        string newEmotion = ExtractBetween(fullReply, "[감정]:", "[친밀도]:").Trim();
        string newRelationship = ExtractBetween(fullReply, "[친밀도]:", "[응답]:").Trim();
        string npcReply = ExtractAfter(fullReply, "[응답]:").Trim();

        Debug.Log($"[감정]: {newEmotion}");
        Debug.Log($"[친밀도]: {newRelationship}");
        Debug.Log($"[응답]: {npcReply}");

        if (!string.IsNullOrEmpty(newEmotion)) emotion = newEmotion;
        if (!string.IsNullOrEmpty(newRelationship)) relationship = newRelationship;

        conversationHistory += $"NPC: {npcReply}\n";
        DialogueManager.Instance.ShowDialogue("NPC: " + npcReply);

        yield return new WaitForSeconds(2.5f);

        inputField.gameObject.SetActive(true);
        inputField.interactable = true;
        inputField.ActivateInputField();
    }

    private string ExtractBetween(string text, string start, string end)
    {
        int startIndex = text.IndexOf(start);
        int endIndex = text.IndexOf(end);
        if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex) return "";
        return text.Substring(startIndex + start.Length, endIndex - startIndex - start.Length);
    }

    private string ExtractAfter(string text, string token)
    {
        int index = text.IndexOf(token);
        if (index == -1) return "";
        return text.Substring(index + token.Length);
    }

    private string ParseGeminiResponse(string json)
    {
        try
        {
            GeminiResponse result = JsonUtility.FromJson<GeminiResponse>(json);
            return result.candidates[0].content.parts[0].text.Trim();
        }
        catch
        {
            return "[감정]: 모름\n[관계]: 모름\n[응답]: 응답 파싱 오류";
        }
    }

    private IEnumerator SendSummaryRequest(string fullConversation)
    {
        string summaryPrompt = "다음 대화를 요약해줘. 100자 이내로 짧고 핵심만:\n" + fullConversation;

        string jsonBody = "{\"contents\":[" +
            "{\"role\":\"user\",\"parts\":[{\"text\":\"" + summaryPrompt + "\"}]}" +
            "]}";

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("요약 실패: " + request.error);
            yield break;
        }

        string jsonResult = request.downloadHandler.text;
        string summary = ParseGeminiResponse(jsonResult);

        Debug.Log("[요약된 히스토리]: " + summary);

        // 요약 결과로 conversationHistory 갱신
        conversationHistory = summary + "\n";
    }

    [System.Serializable]
    public class GeminiResponse
    {
        public Candidate[] candidates;
    }

    [System.Serializable]
    public class Candidate
    {
        public GeminiContent content;
    }

    [System.Serializable]
    public class GeminiContent
    {
        public Part[] parts;
    }

    [System.Serializable]
    public class Part
    {
        public string text;
    }
}
