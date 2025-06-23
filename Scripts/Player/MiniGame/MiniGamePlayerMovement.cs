using UnityEngine;
using System.Collections;

public class MiniGamePlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("점프 체크")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;
    private float horizontal;

    private bool isStarted = false;
    private bool isJumpRotating = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isStarted)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                isStarted = true;
            }
            return;
        }

        // 입력 처리
        horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal != 0)
            Flip(horizontal);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        // 애니메이션 설정
        anim.SetFloat("Speed", Mathf.Abs(horizontal));
    }

    private void FixedUpdate()
    {
        if (!isStarted) return;

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (!isJumpRotating)
            StartCoroutine(JumpRotateZ());
    }

    private void Flip(float direction)
    {
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Sign(direction) * Mathf.Abs(newScale.x);
        transform.localScale = newScale;
    }

    private IEnumerator JumpRotateZ()
    {
        isJumpRotating = true;

        float duration = 0.3f;
        float elapsed = 0f;
        float startZ = transform.eulerAngles.z;
        float endZ = startZ + 360f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.Lerp(startZ, endZ, elapsed / duration);
            transform.rotation = Quaternion.Euler(0f, 0f, z);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, startZ); // 원래 각도로 복구
        isJumpRotating = false;
    }

    public bool IsGrounded() => isGrounded;
}
