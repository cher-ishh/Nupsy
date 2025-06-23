using UnityEngine;

public class FallingEnemy : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifetime = 10f;

    private Vector2 moveDirection = Vector2.down;

    void Start()
    {
        Destroy(gameObject, lifetime); // 일정 시간 후 자동 파괴
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    // 방향과 회전 설정
    public void SetDirection(Vector2 direction, float angleOffset)
    {
        moveDirection = direction.normalized;
        transform.rotation = Quaternion.Euler(0f, 0f, angleOffset);
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
