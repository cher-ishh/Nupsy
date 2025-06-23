using UnityEngine;

public class SmashHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var movement = other.GetComponent<MiniGamePlayerMovement>();
            var health = other.GetComponent<PlayerHealth>();

            if (movement != null && health != null && movement.IsGrounded())
            {
                health.TakeDamage();
            }
        }
    }
}
