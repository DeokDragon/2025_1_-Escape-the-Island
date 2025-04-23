using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BearAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private BearHealth bearHealth;

    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        bearHealth = GetComponent<BearHealth>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || bearHealth.IsDead) return;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            // 플레이어가 시야에 있을 때 쫓아가도록 설정
            agent.SetDestination(player.position);

            // NavMeshAgent의 remainingDistance 속성을 사용하여 실제로 움직이고 있는지 확인
            bool isMoving = agent.remainingDistance > agent.stoppingDistance &&
                            agent.velocity.sqrMagnitude > 0.1f;

            // 움직임에 따라 애니메이션 설정
            animator.SetBool("IsMoving", isMoving); // Bool 파라미터로 변경

            // 공격 로직
            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
        }
        else
        {
            // 플레이어가 범위 밖으로 나가면 움직임 중지
            agent.ResetPath();
            animator.SetBool("IsMoving", false);
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        animator.SetTrigger("Attack1");

        StatusController statusController = player.GetComponent<StatusController>();
        if (statusController != null)
        {
            statusController.DecreaseHP(10);
            statusController.PlayHitSound();
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
}
