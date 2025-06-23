using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyDetector : MonoBehaviour
{
    [Header("감지 범위")]
    [SerializeField] private CapsuleCollider2D flyingCheckCollider;

    [Header("사운드")]
    public AudioClip attackSound;
    private AudioSource audioSource;

    private readonly List<FlyingEnemy> detectedEnemies = new();

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayAttackSound();

            for (int i = detectedEnemies.Count - 1; i >= 0; i--)
            {
                if (detectedEnemies[i] != null)
                    detectedEnemies[i].DieWithEffect();
            }

            detectedEnemies.Clear();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<FlyingEnemy>();
        if (enemy != null && !detectedEnemies.Contains(enemy))
        {
            detectedEnemies.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var enemy = other.GetComponent<FlyingEnemy>();
        if (enemy != null && detectedEnemies.Contains(enemy))
        {
            detectedEnemies.Remove(enemy);
        }
    }

    private void PlayAttackSound()
    {
        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);
    }
}
