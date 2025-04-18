using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class BearHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;

    public GameObject meatPrefab;
    public GameObject bloodEffectPrefab;

    private Renderer bearRenderer;
    private Color originalColor;

    public bool IsDead { get; private set; } = false;

    public AudioClip hurtSound; // 피해를 입을 때 재생할 소리
    public AudioClip deathSound;
    private AudioSource audioSource; // 오디오 소스


    void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponentInChildren<Animator>(); // 애니메이터 할당
        audioSource = GetComponent<AudioSource>(); // 오디오 소스 할당

        bearRenderer = GetComponentInChildren<Renderer>();
        if (bearRenderer != null)
        {
            originalColor = bearRenderer.material.color;
        }

        // Start에서 소리 재생을 아예 하지 않음
        // hurtSound 또는 다른 소리들이 자동으로 나지 않도록 설정함
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;

        // 피격 시 소리만 재생하도록
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);  // 피해를 입을 때만 소리 재생
        }

        if (bearRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        if (animator != null)
        {
            // 현재 공격 트리거 초기화 (애니메이터 상태 초기화 방지용)
            animator.ResetTrigger("Attack1");

            // Hit 트리거 실행
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        bearRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        bearRenderer.material.color = originalColor;
    }

    public void Die()
    {
        IsDead = true;

        // 죽었을 때 애니메이터에 "Dead" 상태로 바꿔주는 코드
        animator.SetBool("IsDead", true); // 죽을 때만 "IsDead"를 true로 설정
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);  // 피해를 입을 때만 소리 재생
        }
        if (bloodEffectPrefab != null)
        {
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
        }

        DropMeat();

        // 2초 후에 죽은 후 오브젝트 제거 (애니메이션이 끝난 후)
        Destroy(gameObject, 1.5f);
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void DropMeat()
    {
        int meatCount = Random.Range(1, 4);

        for (int i = 0; i < meatCount; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-1f, 1f),
                1f,
                Random.Range(-1f, 1f)
            );

            GameObject meat = Instantiate(meatPrefab, spawnPosition, Quaternion.identity);

            Rigidbody rb = meat.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomForce = new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(3f, 5f),
                    Random.Range(-2f, 2f)
                );
                rb.velocity = Vector3.zero;
                rb.AddForce(randomForce, ForceMode.Impulse);
            }
        }
    }
}