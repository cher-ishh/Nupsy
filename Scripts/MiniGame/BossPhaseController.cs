using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BossPhaseController : MonoBehaviour
{
    [Header("보스 오브젝트")]
    [SerializeField] private Transform bossTransform;

    [Header("패턴 스크립트")]
    [SerializeField] private FlyingPattern flyingPattern;
    [SerializeField] private FallingPattern fallingPattern;
    [SerializeField] private SmashPattern smashPattern;

    [Header("이펙트")]
    [SerializeField] private BossPhaseEffect phaseEffect;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image phaseGaugeImage;

    [Header("사운드")]
    [SerializeField] private AudioClip warningSound;
    private AudioSource audioSource;

    [Header("게임 결과 처리")]
    [SerializeField] private MiniGameResultUI resultUI;

    private float gameDuration = 62f;
    private float elapsedTime = 0f;
    private int currentPhase = 0;
    private bool isGameOver = false;
    private bool timerVisible = false;
    private bool isStarted = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        ActivatePhase(0);
    }

    private void Update()
    {
        if (isGameOver) return;

        if (!isStarted)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                isStarted = true;
                Debug.Log("게임 시작!");
            }
            return;
        }

        elapsedTime += Time.deltaTime;

        if (currentPhase >= 1 && timerVisible && timerText != null)
        {
            timerText.text = (62 - Mathf.FloorToInt(elapsedTime)).ToString();
        }

        if (currentPhase < 1 && elapsedTime >= 2f) ActivatePhase(1);
        if (currentPhase < 2 && elapsedTime >= 12f) ActivatePhase(2);
        if (currentPhase < 3 && elapsedTime >= 32f) ActivatePhase(3);

        if (elapsedTime >= gameDuration)
        {
            GameWin();
            FindFirstObjectByType<PlayerHealth>()?.SetInvincible(true);
        }

        UpdatePhaseGauge();
    }

    private void ActivatePhase(int phase)
    {
        if (phase <= currentPhase) return;
        currentPhase = phase;
        Debug.Log($"페이즈 {phase}");

        PlayWarningSound();

        switch (phase)
        {
            case 1:
                timerVisible = true;
                flyingPattern?.EnablePattern();
                break;
            case 2:
                StartCoroutine(PlayEffectThenEnable(() => fallingPattern?.EnablePattern(), -1.1f, 0.5f, 4));
                break;
            case 3:
                StartCoroutine(PlayEffectThenEnable(() => smashPattern?.EnablePattern(), -0.68f, 0.25f, 8));
                break;
        }

        if (phaseGaugeImage != null)
            phaseGaugeImage.fillAmount = 0f;
    }

    private IEnumerator PlayEffectThenEnable(System.Action enableAction, float offsetY, float scaleTime, int loop)
    {
        Vector3 targetScale = bossTransform.localScale * 1.5f;
        yield return StartCoroutine(phaseEffect.PlayPhaseEffect(targetScale, offsetY, scaleTime, loop));
        enableAction?.Invoke();
    }

    private void UpdatePhaseGauge()
    {
        if (phaseGaugeImage == null) return;

        float phaseStartTime = 0f;
        float phaseDuration = 0f;

        switch (currentPhase)
        {
            case 1: phaseStartTime = 2f; phaseDuration = 10f; break;
            case 2: phaseStartTime = 12f; phaseDuration = 20f; break;
            case 3: phaseStartTime = 32f; phaseDuration = 30f; break;
            default:
                phaseGaugeImage.fillAmount = 0f;
                return;
        }

        float progress = Mathf.Clamp01((elapsedTime - phaseStartTime) / phaseDuration);
        phaseGaugeImage.fillAmount = progress;
    }

    public void GameWin()
    {
        isGameOver = true;
        StopAllPatterns();
        resultUI.ShowGameClearUI();
    }

    public void GameOverByDeath()
    {
        isGameOver = true;
        StopAllPatterns();
        resultUI.ShowGameOverUI();
    }

    private void PlayWarningSound()
    {
        if (warningSound != null)
            audioSource.PlayOneShot(warningSound);
    }

    public void StopAllPatterns()
    {
        if (flyingPattern != null) { flyingPattern.DisablePattern(); flyingPattern.enabled = false; }
        if (fallingPattern != null) { fallingPattern.DisablePattern(); fallingPattern.enabled = false; }
        if (smashPattern != null) { smashPattern.DisablePattern(); smashPattern.enabled = false; }
    }
}
