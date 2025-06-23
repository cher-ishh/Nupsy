using UnityEngine;
using System.Collections;

public class FlyingEnemy : MonoBehaviour
{
    [Header("추격 설정")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float destroyAfterChase = 5f;

    [Header("랜덤 이동 설정")]
    [SerializeField] private float roamSpeed = 1f;
    [SerializeField] private float roamChangeInterval = 2f;

    private Vector2 roamDirection;
    private float roamTimer;
    private bool isChasing = false;
    private float chaseTimer;

    private Transform player;
    private SpriteRenderer sr;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        sr = GetComponent<SpriteRenderer>();
        PickNewRoamDirection();
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            HandleChase();
        }
        else
        {
            HandleRoam();
        }
    }

    private void HandleChase()
    {
        if (!isChasing)
        {
            isChasing = true;
            chaseTimer = 0f;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        UpdateFlip(direction);

        chaseTimer += Time.deltaTime;
        if (chaseTimer >= destroyAfterChase)
        {
            Destroy(gameObject);
        }
    }

    private void HandleRoam()
    {
        roamTimer -= Time.deltaTime;
        if (roamTimer <= 0f)
            PickNewRoamDirection();

        transform.position += (Vector3)(roamDirection * roamSpeed * Time.deltaTime);
        UpdateFlip(roamDirection);
    }

    private void PickNewRoamDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        roamDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        roamTimer = roamChangeInterval;
    }

    private void UpdateFlip(Vector2 dir)
    {
        if (sr != null && dir.x != 0)
            sr.flipX = dir.x < 0;
    }

    public void DieWithEffect()
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(DeathEffect());
    }

    private IEnumerator DeathEffect()
    {
        // 1. 위치 고정
        Vector3 fixedPos = transform.position;
        transform.position = fixedPos;

        // 2. 콜라이더 비활성화
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 3. 초기값 세팅
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;
        Color originalColor = Color.white;
        Color targetColor = Color.red;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float timer = 0f;
        float duration = 0.25f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // 크기 커지기
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            // 색상 바꾸기 (하양 → 빨강)
            sr.color = Color.Lerp(originalColor, targetColor, t);

            // 회전
            float rotZ = Mathf.Lerp(0f, 360f, t * 2f);
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage();
            Destroy(gameObject); // 충돌 후 삭제
        }
    }
}
