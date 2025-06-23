using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 3f;
    [SerializeField] private float minWalkTime = 2f;
    [SerializeField] private float maxWalkTime = 4f;

    private Vector2 moveDirection;
    private bool isWalking;
    private float timer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    public bool canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        StartIdle();
    }

    private void Update()
    {
        if (!canMove || (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive()))
        {
            anim.SetFloat("Speed", 0f);
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (isWalking) StartIdle();
            else StartWalking();
        }

        if (isWalking)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
            sr.flipX = moveDirection.x < 0;
        }

        anim.SetFloat("Speed", isWalking ? 1f : 0f);
    }

    private void StartIdle()
    {
        isWalking = false;
        moveDirection = Vector2.zero;
        timer = Random.Range(minIdleTime, maxIdleTime);
    }

    private void StartWalking()
    {
        isWalking = true;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        timer = Random.Range(minWalkTime, maxWalkTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isWalking && (other.CompareTag("Building") || other.CompareTag("Wall")))
        {
            moveDirection = -moveDirection;
            timer = Random.Range(minWalkTime, maxWalkTime);
        }
    }
}
