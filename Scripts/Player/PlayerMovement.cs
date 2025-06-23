using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 3f;

    [Header("사운드")]
    public AudioClip walkSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    private Vector2 targetPosition;
    private bool isMoving = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.clip = walkSound;

        targetPosition = rb.position;
    }

    private void Update()
    {
        // 대화 중이면 입력 무시
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
        {
            targetPosition = rb.position;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        }
    }

    private void FixedUpdate()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
        {
            targetPosition = rb.position;
            anim.SetFloat("Speed", 0f);
            StopWalkingSound();
            return;
        }

        MovePlayer();
    }

    private void MovePlayer()
    {
        float distance = Vector2.Distance(rb.position, targetPosition);
        isMoving = distance > 0.1f;

        anim.SetFloat("Speed", isMoving ? 1f : 0f);

        if (isMoving)
        {
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            sr.flipX = newPosition.x < rb.position.x;
            rb.MovePosition(newPosition);
            PlayWalkingSound();
        }
        else
        {
            StopWalkingSound();
        }
    }

    private void PlayWalkingSound()
    {
        if (walkSound != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void StopWalkingSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 벽이나 건물에 부딪히면 이동 중단
        if ((other.CompareTag("Building") || other.CompareTag("Wall")) && isMoving)
        {
            Vector2 direction = targetPosition - rb.position;
            float pushDistance = 0.2f;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                targetPosition = rb.position + new Vector2(direction.x > 0 ? -pushDistance : pushDistance, 0);
            }
            else
            {
                targetPosition = rb.position + new Vector2(0, direction.y > 0 ? -pushDistance : pushDistance);
            }
        }
    }
}
