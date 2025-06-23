using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;

    [Header("하트 이미지")]
    public Image[] heartImages;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    [Header("사운드")]
    public AudioClip hitSound;
    private AudioSource audioSource;

    private MiniGamePlayerMovement movement;

    private void Awake()
    {
        currentHealth = maxHealth;
        movement = GetComponent<MiniGamePlayerMovement>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        UpdateHeartUI();
    }

    public void TakeDamage()
    {
        if (isDead || isInvincible) return;

        if (hitSound != null)
            audioSource.PlayOneShot(hitSound, 1.5f);

        currentHealth--;
        UpdateHeartUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;

        for (int i = 0; i < 5; i++) // 5번 반복
        {
            // 투명
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
            yield return new WaitForSeconds(0.2f);

            // 원래 색
            sr.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }

        isInvincible = false;
    }

    private void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            Sprite targetSprite = i < currentHealth ? fullHeartSprite : emptyHeartSprite;

            if (heartImages[i].sprite != targetSprite)
            {
                StartCoroutine(RotateHeartAndChange(heartImages[i], targetSprite));
            }
        }
    }

    private IEnumerator RotateHeartAndChange(Image heart, Sprite newSprite)
    {
        float angle = 0f;

        // 한 바퀴 돌리기
        while (angle < 360f)
        {
            angle += 10f;
            heart.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }

        // 회전 완료 후 이미지 변경하고 회전 초기화
        heart.sprite = newSprite;
        heart.transform.rotation = Quaternion.identity;
    }

    private void Die()
    {
        isDead = true;
        movement.enabled = false;

        FindFirstObjectByType<MiniGameResultUI>()?.SendMessage("GameOverByDeath");
    }
}
