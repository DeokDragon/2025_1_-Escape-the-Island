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

    public AudioClip hurtSound; // ���ظ� ���� �� ����� �Ҹ�
    public AudioClip deathSound;
    private AudioSource audioSource; // ����� �ҽ�


    void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponentInChildren<Animator>(); // �ִϸ����� �Ҵ�
        audioSource = GetComponent<AudioSource>(); // ����� �ҽ� �Ҵ�

        bearRenderer = GetComponentInChildren<Renderer>();
        if (bearRenderer != null)
        {
            originalColor = bearRenderer.material.color;
        }

        // Start���� �Ҹ� ����� �ƿ� ���� ����
        // hurtSound �Ǵ� �ٸ� �Ҹ����� �ڵ����� ���� �ʵ��� ������
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;

        // �ǰ� �� �Ҹ��� ����ϵ���
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);  // ���ظ� ���� ���� �Ҹ� ���
        }

        if (bearRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        if (animator != null)
        {
            // ���� ���� Ʈ���� �ʱ�ȭ (�ִϸ����� ���� �ʱ�ȭ ������)
            animator.ResetTrigger("Attack1");

            // Hit Ʈ���� ����
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

        // �׾��� �� �ִϸ����Ϳ� "Dead" ���·� �ٲ��ִ� �ڵ�
        animator.SetBool("IsDead", true); // ���� ���� "IsDead"�� true�� ����
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);  // ���ظ� ���� ���� �Ҹ� ���
        }
        if (bloodEffectPrefab != null)
        {
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
        }

        DropMeat();

        // 2�� �Ŀ� ���� �� ������Ʈ ���� (�ִϸ��̼��� ���� ��)
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